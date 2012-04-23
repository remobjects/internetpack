Imports System
Imports System.IO
Imports System.Collections.Specialized
Imports System.Text.RegularExpressions
Imports RemObjects.InternetPack.Ftp
Public Class FtpSyncWorker

    Public Sub New()

    End Sub

#Region " private fields... "
    Private _LocalDirectory As String
    Private _Server As String
    Private _ServerDirectory As String
    Private _Username As String
    Private _Password As String
    Private _DeleteMissing As Boolean
    Private _Upload As Boolean
    Private _Subdirectories As Boolean
    Private _Passive As Boolean
    Private _ShowLog As Boolean
    Private WithEvents _Ftp As FtpClient
#End Region


    Public Sub Log(ByVal Sender As Object, ByVal ea As RemObjects.InternetPack.CommandBased.ClientLogArgs) Handles _Ftp.OnLog
        If _ShowLog Then
            Console.WriteLine(ea.Text)
        End If
    End Sub

    Private Sub _Ftp_OnTransferProgress(ByVal aSender As Object, ByVal ea As RemObjects.InternetPack.Events.TransferProgressEventArgs) Handles _Ftp.OnTransferProgress
        Console.Write(".")
    End Sub

    Public Sub Sync()

        Try
            _Ftp = New FtpClient
            _Ftp.HostName = _Server
            _Ftp.UserName = _Username
            _Ftp.Password = _Password
            _Ftp.Passive = _Passive
            _Ftp.Port = 21

            '_Ftp.OnLog += new RemObjects.InternetPack.CommandBased.ClientLogEvent(Log);
            '	_Ftp.OnTransferProgress +=new RemObjects.InternetPack.Events.TransferProgressEventHandler(_Ftp_OnTransferProgress);
            Console.WriteLine("Connecting to " & _Server)
            _Ftp.Open()
            Try
                _Ftp.Login()
                SyncDirectory(_LocalDirectory, "/" & _ServerDirectory)
            Finally
                Console.WriteLine("Disconnecting")
                _Ftp.Quit()
                _Ftp.Close()
            End Try

        Catch ex As Exception
            Console.WriteLine("Error syncing directory ({0})", ex.Message)
            If ex.StackTrace <> Nothing And _ShowLog Then
                Console.WriteLine(ex.StackTrace)
            End If
            Console.WriteLine("Press enter to continue...")
            Console.ReadLine()
        End Try
    End Sub

    Public Sub SyncDirectory(ByVal aLocalDirectory As String, ByVal aRemoteDirectory As String)
        Dim lOriginalLocalDirectory As String
        lOriginalLocalDirectory = Directory.GetCurrentDirectory()
        Dim lOriginalRemoteDirectory As String
        lOriginalRemoteDirectory = _Ftp.GetCurrentDirectory()
        Try
            Console.WriteLine("Local change directory to " + aLocalDirectory)
            Directory.SetCurrentDirectory(aLocalDirectory)
            Console.WriteLine("Remote change directory to " + aRemoteDirectory)
            _Ftp.ChangeDirectory(aRemoteDirectory)
            Console.WriteLine("")

            Console.WriteLine("Retrieving directory contents")
            _Ftp.List()
            Console.WriteLine("")

            Dim localFiles As StringCollection
            localFiles = New StringCollection
            localFiles.AddRange(Directory.GetFiles(aLocalDirectory))

            If _Upload Then
                Console.WriteLine("Upload not yet implemented")
            End If

            Dim i As Integer
            For i = 0 To _Ftp.CurrentDirectoryContents.Count - 1
                Dim lItem As FtpListingItem
                lItem = _Ftp.CurrentDirectoryContents(i)

                If Not lItem.Directory Then
                    Dim lDownload As Boolean
                    lDownload = False
                    If File.Exists(lItem.FileName) Then
                        Dim lInfo As FileInfo
                        lInfo = New FileInfo(lItem.FileName)

                        If lInfo.LastWriteTime <> lItem.FileDate Then
                            lDownload = True
                        End If

                        If lInfo.Length <> lItem.Size Then
                            lDownload = True
                        End If

                        localFiles.Remove(Path.Combine(aLocalDirectory, lItem.FileName))
                    Else
                        lDownload = True
                    End If

                    If lDownload Then
                        Console.WriteLine("Downloading " & lItem.FileName)
                        Dim lStream As Stream
                        lStream = File.Open(lItem.FileName, FileMode.Create)

                        With lStream
                            _Ftp.Retrieve(lItem, lStream)
                            lStream.Close()
                            File.SetLastWriteTime(lItem.FileName, lItem.FileDate)
                        End With
                        Console.WriteLine("")
                    End If
                End If
            Next

            If _DeleteMissing Then
                Dim ii As Integer
                For ii = 0 To localFiles.Count - 1
                    File.Delete(localFiles(ii))
                Next
            End If

        Finally
            Directory.SetCurrentDirectory(lOriginalLocalDirectory)
            _Ftp.ChangeDirectory(lOriginalRemoteDirectory)
            Console.WriteLine("")
        End Try
    End Sub

    Public Function CheckArgs(ByVal args As String()) As Boolean
        Dim lCount As Integer
        lCount = args.Length
        Dim lBadParam As Boolean
        lBadParam = False

        If (lCount > 0) Then
            _LocalDirectory = args(0)
        Else
            lBadParam = True
        End If

        If (lCount > 1) Then
            Dim lMatch As Match
            lMatch = Regex.Match(args(1), "(?<user>\S+):(?<pass>\S+)@(?<server>[^/\s]+)/(?<dir>\S*)")
            If lMatch.Success Then
                _Username = lMatch.Groups("user").Value
                _Password = lMatch.Groups("pass").Value
                _Server = lMatch.Groups("server").Value
                _ServerDirectory = lMatch.Groups("dir").Value
            Else
                lBadParam = True
                Console.WriteLine("Invalid server parameters")
                Console.WriteLine("")
            End If
        Else
            lBadParam = True
        End If
        Dim i As Integer
        For i = 2 To lCount - 1
            Select Case args(i).ToLower()
                Case "-delete"
                    _DeleteMissing = True
                Case "-upload"
                    _Upload = True
                Case "-passive"
                    _Passive = True
                Case "-s"
                    _Subdirectories = True
                Case "-l"
                    _ShowLog = True
                Case "-help"
                    lBadParam = True
                Case "/help"
                    lBadParam = True
                Case Else
                    Console.WriteLine("Invalid command line paramter '" & args(i) & "'")
                    Console.WriteLine("")
                    lBadParam = True
            End Select
        Next

        If lBadParam Then
            Console.WriteLine("RemObjects Internet Pack - FtpSync Sample")
            Console.WriteLine("")
            Console.WriteLine("  Usage: FtpSync local user:pass@server/remote [-delete] [-passive] [-l]")
            Console.WriteLine()
            Console.WriteLine("  Compares files on local computer against those on the remote server, ")
            Console.WriteLine("  downloading those that are new or have changed.")
            Console.WriteLine()
            Console.WriteLine("  local     : local directory - long file names with spaces should be quoted")
            Console.WriteLine("  user/pass : ftp username & password")
            Console.WriteLine("  server    : ftp server (i.e., ftp.whatever.com")
            Console.WriteLine("  remote    : directory on remote server")
            Console.WriteLine()
            Console.WriteLine("  -delete   : delete local files that don't exist on server")
            Console.WriteLine("  -passive   : turn on 'Passive mode'")
            'Console.WriteLine("  -upload   : reverse comparision & transfer direction")
            'Console.WriteLine("  -s        : recurse through subdirectories")
            Console.WriteLine("  -l        : log - show FTP commands")
            Console.WriteLine()
            Console.WriteLine("Press enter to exit.")
            Console.ReadLine()
        End If
        Return Not lBadParam
    End Function

#Region "public properties..."

    Property LocalDirectory() As String
        Get
            Return _LocalDirectory
        End Get
        Set(ByVal Value As String)
            _LocalDirectory = Value
        End Set
    End Property
    Property Server() As String
        Get
            Return _Server
        End Get
        Set(ByVal Value As String)
            _Server = Value
        End Set
    End Property
    Property ServerDirectory() As String
        Get
            Return _ServerDirectory
        End Get
        Set(ByVal Value As String)
            _ServerDirectory = Value
        End Set
    End Property
    Property DeleteMissing() As Boolean
        Get
            Return _DeleteMissing
        End Get
        Set(ByVal Value As Boolean)
            _DeleteMissing = Value
        End Set
    End Property
    Property Upload() As Boolean
        Get
            Return _Upload
        End Get
        Set(ByVal Value As Boolean)
            _Upload = Value
        End Set
    End Property
    Property Subdirectories() As Boolean
        Get
            Return _Subdirectories
        End Get
        Set(ByVal Value As Boolean)
            _Subdirectories = Value
        End Set
    End Property
    Property Passive() As Boolean
        Get
            Return _Passive
        End Get
        Set(ByVal Value As Boolean)
            _Passive = Value
        End Set
    End Property
#End Region
End Class
