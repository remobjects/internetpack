Imports RemObjects.InternetPack.StandardServers
Imports RemObjects.InternetPack.Http
Imports System.IO

Public Class MainForm
    Inherits System.Windows.Forms.Form

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
    Friend WithEvents pictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnAction As System.Windows.Forms.Button
    Friend WithEvents nudPort As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblLink As System.Windows.Forms.LinkLabel
    Friend WithEvents lblPort As System.Windows.Forms.Label
    Friend WithEvents lblRoot As System.Windows.Forms.Label
    Friend WithEvents txtRoot As System.Windows.Forms.TextBox
    Friend WithEvents txtServerName As System.Windows.Forms.TextBox
    Friend WithEvents lblServerName As System.Windows.Forms.Label
    Friend WithEvents nudCount As System.Windows.Forms.NumericUpDown
    Friend WithEvents lblCount As System.Windows.Forms.Label
    Friend WithEvents lblUrl As System.Windows.Forms.Label
    Friend WithEvents txtLog As System.Windows.Forms.TextBox
    Friend WithEvents gbHttpServer As System.Windows.Forms.GroupBox

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.lblLink = New System.Windows.Forms.LinkLabel
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.btnAction = New System.Windows.Forms.Button
        Me.nudPort = New System.Windows.Forms.NumericUpDown
        Me.lblPort = New System.Windows.Forms.Label
        Me.Label2 = New System.Windows.Forms.Label
        Me.txtLog = New System.Windows.Forms.TextBox
        Me.gbHttpServer = New System.Windows.Forms.GroupBox
        Me.lblUrl = New System.Windows.Forms.Label
        Me.nudCount = New System.Windows.Forms.NumericUpDown
        Me.lblCount = New System.Windows.Forms.Label
        Me.lblServerName = New System.Windows.Forms.Label
        Me.txtServerName = New System.Windows.Forms.TextBox
        Me.txtRoot = New System.Windows.Forms.TextBox
        Me.lblRoot = New System.Windows.Forms.Label
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPort, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbHttpServer.SuspendLayout()
        CType(Me.nudCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblLink
        '
        Me.lblLink.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblLink.Enabled = False
        Me.lblLink.Location = New System.Drawing.Point(139, 120)
        Me.lblLink.Name = "lblLink"
        Me.lblLink.Size = New System.Drawing.Size(168, 16)
        Me.lblLink.TabIndex = 9
        Me.lblLink.TabStop = True
        Me.lblLink.Text = "http://localhost:82/index.html"
        '
        'pictureBox1
        '
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(8, 8)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(120, 30)
        Me.pictureBox1.TabIndex = 11
        Me.pictureBox1.TabStop = False
        '
        'btnAction
        '
        Me.btnAction.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAction.Location = New System.Drawing.Point(402, 115)
        Me.btnAction.Name = "btnAction"
        Me.btnAction.Size = New System.Drawing.Size(112, 23)
        Me.btnAction.TabIndex = 10
        Me.btnAction.Text = "Activate Servers"
        '
        'nudPort
        '
        Me.nudPort.Location = New System.Drawing.Point(139, 16)
        Me.nudPort.Name = "nudPort"
        Me.nudPort.Size = New System.Drawing.Size(48, 20)
        Me.nudPort.TabIndex = 1
        Me.nudPort.Value = New Decimal(New Integer() {83, 0, 0, 0})
        '
        'lblPort
        '
        Me.lblPort.AutoSize = True
        Me.lblPort.Location = New System.Drawing.Point(102, 18)
        Me.lblPort.Name = "lblPort"
        Me.lblPort.Size = New System.Drawing.Size(29, 13)
        Me.lblPort.TabIndex = 0
        Me.lblPort.Text = "&Port:"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(8, 200)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(32, 16)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Log"
        '
        'txtLog
        '
        Me.txtLog.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtLog.Location = New System.Drawing.Point(8, 224)
        Me.txtLog.Multiline = True
        Me.txtLog.Name = "txtLog"
        Me.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.txtLog.Size = New System.Drawing.Size(528, 222)
        Me.txtLog.TabIndex = 2
        Me.txtLog.WordWrap = False
        '
        'gbHttpServer
        '
        Me.gbHttpServer.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.gbHttpServer.Controls.Add(Me.lblUrl)
        Me.gbHttpServer.Controls.Add(Me.nudCount)
        Me.gbHttpServer.Controls.Add(Me.lblCount)
        Me.gbHttpServer.Controls.Add(Me.lblServerName)
        Me.gbHttpServer.Controls.Add(Me.txtServerName)
        Me.gbHttpServer.Controls.Add(Me.txtRoot)
        Me.gbHttpServer.Controls.Add(Me.lblRoot)
        Me.gbHttpServer.Controls.Add(Me.nudPort)
        Me.gbHttpServer.Controls.Add(Me.lblPort)
        Me.gbHttpServer.Controls.Add(Me.lblLink)
        Me.gbHttpServer.Controls.Add(Me.btnAction)
        Me.gbHttpServer.Location = New System.Drawing.Point(8, 48)
        Me.gbHttpServer.Name = "gbHttpServer"
        Me.gbHttpServer.Size = New System.Drawing.Size(528, 144)
        Me.gbHttpServer.TabIndex = 0
        Me.gbHttpServer.TabStop = False
        Me.gbHttpServer.Text = "HttpServer"
        '
        'lblUrl
        '
        Me.lblUrl.AutoSize = True
        Me.lblUrl.Enabled = False
        Me.lblUrl.Location = New System.Drawing.Point(99, 120)
        Me.lblUrl.Name = "lblUrl"
        Me.lblUrl.Size = New System.Drawing.Size(32, 13)
        Me.lblUrl.TabIndex = 8
        Me.lblUrl.Text = "URL:"
        '
        'nudCount
        '
        Me.nudCount.Location = New System.Drawing.Point(139, 88)
        Me.nudCount.Name = "nudCount"
        Me.nudCount.Size = New System.Drawing.Size(48, 20)
        Me.nudCount.TabIndex = 7
        Me.nudCount.Value = New Decimal(New Integer() {5, 0, 0, 0})
        '
        'lblCount
        '
        Me.lblCount.AutoSize = True
        Me.lblCount.Location = New System.Drawing.Point(16, 90)
        Me.lblCount.Name = "lblCount"
        Me.lblCount.Size = New System.Drawing.Size(115, 13)
        Me.lblCount.TabIndex = 6
        Me.lblCount.Text = "Listener Thread Count:"
        '
        'lblServerName
        '
        Me.lblServerName.AutoSize = True
        Me.lblServerName.Location = New System.Drawing.Point(59, 67)
        Me.lblServerName.Name = "lblServerName"
        Me.lblServerName.Size = New System.Drawing.Size(72, 13)
        Me.lblServerName.TabIndex = 4
        Me.lblServerName.Text = "&Server Name:"
        '
        'txtServerName
        '
        Me.txtServerName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtServerName.Location = New System.Drawing.Point(139, 64)
        Me.txtServerName.Name = "txtServerName"
        Me.txtServerName.Size = New System.Drawing.Size(376, 20)
        Me.txtServerName.TabIndex = 5
        Me.txtServerName.Text = "TextBox3"
        '
        'txtRoot
        '
        Me.txtRoot.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRoot.Location = New System.Drawing.Point(139, 40)
        Me.txtRoot.Name = "txtRoot"
        Me.txtRoot.Size = New System.Drawing.Size(376, 20)
        Me.txtRoot.TabIndex = 3
        Me.txtRoot.Text = "TextBox2"
        '
        'lblRoot
        '
        Me.lblRoot.AutoSize = True
        Me.lblRoot.Location = New System.Drawing.Point(76, 43)
        Me.lblRoot.Name = "lblRoot"
        Me.lblRoot.Size = New System.Drawing.Size(55, 13)
        Me.lblRoot.TabIndex = 2
        Me.lblRoot.Text = "&RootPath:"
        '
        'MainForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(534, 442)
        Me.Controls.Add(Me.gbHttpServer)
        Me.Controls.Add(Me.txtLog)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.pictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(550, 480)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Internet Pack Sample Server"
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPort, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbHttpServer.ResumeLayout(False)
        Me.gbHttpServer.PerformLayout()
        CType(Me.nudCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private fEchoServer As EchoServer
    Private WithEvents fHttpServer As SimpleHttpServer

    Private Sub btn_Action_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click
        If btnAction.Text = "Activate Servers" Then
            ActivateServers()
        Else
            DeactivateServers()
        End If
    End Sub

    Delegate Sub StatusEventHandler(ByVal Message As String)

    Private Sub ThreadSaveSetStatus(ByVal aStatus As String)
        AddLog(String.Format("Request to {0}", aStatus))
    End Sub

    Public Sub OnHttpRequest(ByVal aSender As Object, ByVal ea As OnHttpRequestArgs) Handles fHttpServer.OnHttpRequest
        Dim myDelegate As StatusEventHandler
        myDelegate = AddressOf ThreadSaveSetStatus
        Invoke(myDelegate, ea.Request.Header.RequestPath)
    End Sub

    Private Sub Form1_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed
        DeactivateServers()
    End Sub

    Private Sub ActivateServers()
        AddLog("Trying to activate servers...")
        fEchoServer = New EchoServer
        Try
            fEchoServer.Open()
            AddLog("EchoServer is active.")
        Catch ex As Exception
            AddLog("Can't activate EchoServer. An exception occured: " + ex.Message)
        End Try

        fHttpServer = New SimpleHttpServer
        fHttpServer.Port = nudPort.Value
        fHttpServer.RootPath = txtRoot.Text
        fHttpServer.ServerName = txtServerName.Text
        If (Not Me.fHttpServer.BindingV4 Is Nothing) Then
            Me.fHttpServer.BindingV4.ListenerThreadCount = Me.nudCount.Value
        End If
        fHttpServer.Open()
        AddLog(String.Format("SimpleHttpServer is active on {0} port.", fHttpServer.Port))
        SetEnable(False)
        AddLog("Servers activated.")
        btnAction.Text = "Deactivate Servers"
    End Sub

    Private Sub SetEnable(ByVal mode As Boolean)
        lblPort.Enabled = mode
        nudPort.Enabled = mode
        lblServerName.Enabled = mode
        txtServerName.Enabled = mode
        lblRoot.Enabled = mode
        txtRoot.Enabled = mode
        lblUrl.Enabled = Not mode
        lblLink.Enabled = Not mode
    End Sub

    Private Sub DeactivateServers()
        AddLog("Trying to deactivate servers...")
        If Not fEchoServer Is Nothing Then
            fEchoServer.Close()
        End If
        AddLog("EchoServer is closed.")
        If Not fHttpServer Is Nothing Then
            fHttpServer.Close()
        End If
        AddLog("HttpServer is closed.")
        SetEnable(True)
        AddLog("Servers is deactivated")
        btnAction.Text = "Activate Servers"
    End Sub

    Private Sub lbl_Link_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblLink.Click
        If (File.Exists(fHttpServer.RootPath + "\index.html")) Then
            System.Diagnostics.Process.Start(lblLink.Text)
        Else
            MessageBox.Show(fHttpServer.RootPath + "\index.html can not be opened, because it does not exists.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub nudPort_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudPort.ValueChanged
        lblLink.Text = String.Format("http://localhost:{0}/index.html", nudPort.Value)
    End Sub

    Private Sub AddLog(ByVal line As String)
        txtLog.AppendText(Environment.NewLine & String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), line))
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        txtRoot.Text = Path.Combine(Path.GetDirectoryName(Me.GetType().Assembly.Location), "HttpRoot")
        txtServerName.Text = "Internet Pack HTTP Server"
        nudPort.Value = 82
        nudCount.Value = 5
    End Sub
End Class