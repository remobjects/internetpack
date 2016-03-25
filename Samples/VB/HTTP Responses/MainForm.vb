
Public Class MainForm
    Inherits Form

    Const sWelcome As String =
        "Internet Pack HTTP Responses Test App" &
        "<br /><br />" &
        "Valid links:" &
        "<br />" &
        "<a href=/home>/home</a> show this page</a>" &
        "<br />" &
        "<a href=/file>/file</a> send back a file (this .exe)" &
        "<br />" &
        "<a href=/bytes>/bytes</a> send back a buffer of random bytes" &
        "<br />" &
        "<a href=/error>/error</a> Display a custom error"

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        Application.EnableVisualStyles()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents lb_Log As System.Windows.Forms.ListBox
    Friend WithEvents llblinkLabel1 As System.Windows.Forms.LinkLabel
    Friend WithEvents HttpServer As RemObjects.InternetPack.Http.HttpServer
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.lb_Log = New System.Windows.Forms.ListBox
        Me.llblinkLabel1 = New System.Windows.Forms.LinkLabel
        Me.HttpServer = New RemObjects.InternetPack.Http.HttpServer(Me.components)
        Me.SuspendLayout()
        '
        'lb_Log
        '
        Me.lb_Log.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lb_Log.IntegralHeight = False
        Me.lb_Log.Location = New System.Drawing.Point(11, 34)
        Me.lb_Log.Name = "lb_Log"
        Me.lb_Log.Size = New System.Drawing.Size(371, 226)
        Me.lb_Log.TabIndex = 3
        '
        'llblinkLabel1
        '
        Me.llblinkLabel1.Location = New System.Drawing.Point(8, 10)
        Me.llblinkLabel1.Name = "llblinkLabel1"
        Me.llblinkLabel1.Size = New System.Drawing.Size(100, 23)
        Me.llblinkLabel1.TabIndex = 2
        Me.llblinkLabel1.TabStop = True
        Me.llblinkLabel1.Text = "http://localhost:82"
        '
        'HttpServer
        '
        Me.HttpServer.Port = 82
        Me.HttpServer.ValidateRequests = False
        '
        'MainForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(394, 272)
        Me.Controls.Add(Me.lb_Log)
        Me.Controls.Add(Me.llblinkLabel1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(410, 310)
        Me.Name = "MainForm"
        Me.Text = "Internet Pack HTTP Response Sample"
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private Sub llblinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As Windows.Forms.LinkLabelLinkClickedEventArgs) Handles llblinkLabel1.LinkClicked
        Process.Start(llblinkLabel1.Text)
    End Sub

    Public Sub Invoke_LogMessage(ByVal aString As String)
        lb_Log.Items.Add(aString)
    End Sub

    Private Delegate Sub InvokeDelegate(ByVal aString As String)

    Private Sub HttpServer_OnHttpRequest(ByVal sender As Object, ByVal ea As RemObjects.InternetPack.Http.HttpRequestEventArgs) Handles HttpServer.HttpRequest
        Invoke(New InvokeDelegate(AddressOf Invoke_LogMessage), ea.Request.Header.RequestPath)
        Select Case ea.Request.Header.RequestPath
            Case "/", "/home"
                ea.Response.ContentString = sWelcome
                ea.Response.Header.SetHeaderValue("Content-Type", "text/html")


            Case "/bytes"
                Dim lBuffer(256) As Byte
                Dim lRandom As Random
                lRandom = New Random

                lRandom.NextBytes(lBuffer)
                ea.Response.ContentBytes = lBuffer
                ea.Response.Header.SetHeaderValue("Content-Disposition", "filename=random.bin")
                ea.Response.Header.SetHeaderValue("Content-Type", "application/binary")

            Case "/error"
                ea.Response.SendError(555, "Custom Error")

            Case "/file"
                Dim lExeName As String = Me.GetType().Assembly.Location
                Try
                    ea.Response.ContentStream = New System.IO.FileStream(lExeName, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read)
                    ea.Response.Header.SetHeaderValue("Content-Disposition", String.Format("filename={0}", System.IO.Path.GetFileName(lExeName)))
                    ea.Response.Header.SetHeaderValue("Content-Type", "application/binary")
                    ea.Response.CloseStream = True ' default, anyway
                Catch e As Exception
                    ea.Response.SendError(System.Net.HttpStatusCode.NotFound, String.Format("File {0} not found", lExeName))
                End Try

            Case Else
                ea.Response.SendError(System.Net.HttpStatusCode.NotFound, "Requested path not found")
        End Select

    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
        HttpServer.Active = True
    End Sub

    Private Sub Form1_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        HttpServer.Active = False
    End Sub

End Class
