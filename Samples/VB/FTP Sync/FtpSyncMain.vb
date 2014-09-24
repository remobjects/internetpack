Module FtpSyncMain
    Sub Main(ByVal args As String())
        Dim lWorker As FtpSyncWorker
        lWorker = New FtpSyncWorker
        If lWorker.CheckArgs(args) Then
            lWorker.Sync()
        End If
    End Sub
End Module
