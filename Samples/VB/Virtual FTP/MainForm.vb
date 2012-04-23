Imports RemObjects.InternetPack.Ftp.VirtualFtp
Imports System.IO

Public Class MainForm
    Inherits System.Windows.Forms.Form

    Private fRootFolder As VirtualFolder
    Private fUserManager As IFtpUserManager
    Private fFtpServer As VirtualFtpServer

    Const port As Integer = 4444

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
    Friend WithEvents label1 As System.Windows.Forms.Label
    Friend WithEvents pictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents btnStartStop As System.Windows.Forms.Button
    Friend WithEvents llShortcut As System.Windows.Forms.LinkLabel
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.label1 = New System.Windows.Forms.Label
        Me.llShortcut = New System.Windows.Forms.LinkLabel
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.txtLog = New System.Windows.Forms.TextBox
        Me.GroupBox1 = New System.Windows.Forms.GroupBox
        Me.btnStartStop = New System.Windows.Forms.Button
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'label1
        '
        Me.label1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label1.Location = New System.Drawing.Point(3, 39)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(309, 24)
        Me.label1.TabIndex = 5
        Me.label1.Text = "In order to login on ftp please use login: test; password: test."
        Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'llShortcut
        '
        Me.llShortcut.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.llShortcut.Location = New System.Drawing.Point(4, 63)
        Me.llShortcut.Name = "llShortcut"
        Me.llShortcut.Size = New System.Drawing.Size(300, 33)
        Me.llShortcut.TabIndex = 4
        Me.llShortcut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pictureBox1
        '
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(8, 8)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(120, 30)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pictureBox1.TabIndex = 3
        Me.pictureBox1.TabStop = False
        '
        'txtLog
        '
        Me.txtLog.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLog.Location = New System.Drawing.Point(8, 16)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtLog.Size = New System.Drawing.Size(282, 108)
        Me.txtLog.TabIndex = 6
        Me.txtLog.WordWrap = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.txtLog)
        Me.GroupBox1.Location = New System.Drawing.Point(8, 104)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(298, 132)
        Me.GroupBox1.TabIndex = 7
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Log"
        '
        'btnStartStop
        '
        Me.btnStartStop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnStartStop.Location = New System.Drawing.Point(231, 9)
        Me.btnStartStop.Name = "btnStartStop"
        Me.btnStartStop.Size = New System.Drawing.Size(75, 23)
        Me.btnStartStop.TabIndex = 8
        Me.btnStartStop.Text = "Start"
        '
        'MainForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(314, 242)
        Me.Controls.Add(Me.btnStartStop)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.llShortcut)
        Me.Controls.Add(Me.pictureBox1)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(330, 280)
        Me.Name = "MainForm"
        Me.Text = "Virtual FTP"
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Sub linkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles llShortcut.LinkClicked
        If llShortcut.Text <> "" Then
            System.Diagnostics.Process.Start(llShortcut.Text)
        End If
    End Sub

    Public Sub StartServer(ByVal aPort As String)
        Dim lDiskFolder As String
        lDiskFolder = Path.GetDirectoryName(GetType(MainForm).Assembly.Location) & "..\..\..\"

        fRootFolder = New VirtualFolder(Nothing, "[ROOT]")
        fRootFolder.Add(New VirtualFolder(Nothing, "virtual"))
        fRootFolder.Add(New DiscFolder(Nothing, "drive-c", "c:\"))
        fRootFolder.Add(New DiscFolder(Nothing, "disc", lDiskFolder))
        fRootFolder.Add(New EmptyFile(Nothing, "=== Welcome to the FTP ==="))

        fUserManager = New UserManager
        CType(fUserManager, UserManager).AddUser("test", "test")

        fFtpServer = New VirtualFtpServer
        fFtpServer.Port = aPort
        fFtpServer.Timeout = 60 * 1000 '/* 1 minute */
        If Not IsNothing(fFtpServer.BindingV4) Then
            fFtpServer.BindingV4.ListenerThreadCount = 10
        Else
            fFtpServer.BindingV6.ListenerThreadCount = 10
        End If
        fFtpServer.RootFolder = fRootFolder
        fFtpServer.UserManager = fUserManager
        fFtpServer.ServerName = "VirtualFTP Sample - powered by RemObjects Internet Pack for .NET"

        fFtpServer.Open()

        Debug.Write("VirtualFTP 0.3 BETA - started up")
    End Sub

    Public Sub StopServer()
        If Not IsNothing(fFtpServer) Then
            fFtpServer.Close()
        End If
    End Sub

    Private Sub btnStartStop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartStop.Click
        If btnStartStop.Text = "Start" Then
            addToLog("Starting Virtual FTP on " & port.ToString() & " port...")
            StartServer(port)
            llShortcut.Text = String.Format("ftp://localhost:{0}/", port)
            addToLog("Virtual FTP is running under " & Environment.OSVersion.ToString())
            btnStartStop.Text = "Stop"
        Else
            addToLog("Shutting down Virtual FTP ...")
            StopServer()
            llShortcut.Text = ""
            addToLog("Virtual FTP is stopped.")
            btnStartStop.Text = "Start"
        End If
    End Sub

    Private Sub addToLog(ByVal line As String)
        txtLog.Text = txtLog.Text & _
          System.DateTime.Now.ToLongTimeString() & _
          ": " & _
          line & Environment.NewLine
    End Sub

    Private Sub Form1_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        StopServer()
    End Sub
End Class
