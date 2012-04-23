Public Class MainForm
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        Application.EnableVisualStyles()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call
        msg = New RemObjects.InternetPack.Messages.MailMessage

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

    Friend msg As RemObjects.InternetPack.Messages.MailMessage
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    Friend WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents lblSMTPServer As System.Windows.Forms.Label
    Friend WithEvents txtSMTPServer As System.Windows.Forms.TextBox
    Friend WithEvents groupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents txtUserName As System.Windows.Forms.TextBox
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents lblUserName As System.Windows.Forms.Label
    Friend WithEvents chkUseAuth As System.Windows.Forms.CheckBox
    Friend WithEvents smtpClient As RemObjects.InternetPack.Email.SmtpClient
    Friend WithEvents lblIP As System.Windows.Forms.Label
    Friend WithEvents lblRO As System.Windows.Forms.Label
    Friend WithEvents pictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents groupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents lblSenderName As System.Windows.Forms.Label
    Friend WithEvents lblFromEmail As System.Windows.Forms.Label
    Friend WithEvents txtMessage As System.Windows.Forms.TextBox
    Friend WithEvents txtSenderAddress As System.Windows.Forms.TextBox
    Friend WithEvents label5 As System.Windows.Forms.Label
    Friend WithEvents txtSenderName As System.Windows.Forms.TextBox
    Friend WithEvents txtTo As System.Windows.Forms.TextBox
    Friend WithEvents label6 As System.Windows.Forms.Label
    Friend WithEvents label3 As System.Windows.Forms.Label
    Friend WithEvents txtCC As System.Windows.Forms.TextBox
    Friend WithEvents txtSubject As System.Windows.Forms.TextBox
    Friend WithEvents label7 As System.Windows.Forms.Label
    Friend WithEvents txtBCC As System.Windows.Forms.TextBox
    Friend WithEvents label8 As System.Windows.Forms.Label
    Friend WithEvents btnSendEMail As System.Windows.Forms.Button
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.lblSMTPServer = New System.Windows.Forms.Label
        Me.txtSMTPServer = New System.Windows.Forms.TextBox
        Me.groupBox2 = New System.Windows.Forms.GroupBox
        Me.txtPassword = New System.Windows.Forms.TextBox
        Me.txtUserName = New System.Windows.Forms.TextBox
        Me.lblPassword = New System.Windows.Forms.Label
        Me.lblUserName = New System.Windows.Forms.Label
        Me.chkUseAuth = New System.Windows.Forms.CheckBox
        Me.smtpClient = New RemObjects.InternetPack.Email.SmtpClient
        Me.lblIP = New System.Windows.Forms.Label
        Me.lblRO = New System.Windows.Forms.Label
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.groupBox3 = New System.Windows.Forms.GroupBox
        Me.lblSenderName = New System.Windows.Forms.Label
        Me.lblFromEmail = New System.Windows.Forms.Label
        Me.txtMessage = New System.Windows.Forms.TextBox
        Me.txtSenderAddress = New System.Windows.Forms.TextBox
        Me.label5 = New System.Windows.Forms.Label
        Me.txtSenderName = New System.Windows.Forms.TextBox
        Me.txtTo = New System.Windows.Forms.TextBox
        Me.label6 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.txtCC = New System.Windows.Forms.TextBox
        Me.txtSubject = New System.Windows.Forms.TextBox
        Me.label7 = New System.Windows.Forms.Label
        Me.txtBCC = New System.Windows.Forms.TextBox
        Me.label8 = New System.Windows.Forms.Label
        Me.btnSendEMail = New System.Windows.Forms.Button
        Me.groupBox1.SuspendLayout()
        Me.groupBox2.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.groupBox3.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.lblSMTPServer)
        Me.groupBox1.Controls.Add(Me.txtSMTPServer)
        Me.groupBox1.Controls.Add(Me.groupBox2)
        Me.groupBox1.Controls.Add(Me.chkUseAuth)
        Me.groupBox1.Location = New System.Drawing.Point(7, 6)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(360, 160)
        Me.groupBox1.TabIndex = 15
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Server Information"
        '
        'lblSMTPServer
        '
        Me.lblSMTPServer.Location = New System.Drawing.Point(6, 19)
        Me.lblSMTPServer.Name = "lblSMTPServer"
        Me.lblSMTPServer.Size = New System.Drawing.Size(160, 15)
        Me.lblSMTPServer.TabIndex = 0
        Me.lblSMTPServer.Text = "Outgoing Mail Server (SMTP):"
        '
        'txtSMTPServer
        '
        Me.txtSMTPServer.Location = New System.Drawing.Point(168, 16)
        Me.txtSMTPServer.Name = "txtSMTPServer"
        Me.txtSMTPServer.Size = New System.Drawing.Size(168, 20)
        Me.txtSMTPServer.TabIndex = 1
        Me.txtSMTPServer.Text = "ws7.elitedev.com"
        '
        'groupBox2
        '
        Me.groupBox2.Controls.Add(Me.txtPassword)
        Me.groupBox2.Controls.Add(Me.txtUserName)
        Me.groupBox2.Controls.Add(Me.lblPassword)
        Me.groupBox2.Controls.Add(Me.lblUserName)
        Me.groupBox2.Location = New System.Drawing.Point(16, 72)
        Me.groupBox2.Name = "groupBox2"
        Me.groupBox2.Size = New System.Drawing.Size(336, 80)
        Me.groupBox2.TabIndex = 3
        Me.groupBox2.TabStop = False
        Me.groupBox2.Text = "Login Information"
        '
        'txtPassword
        '
        Me.txtPassword.Enabled = False
        Me.txtPassword.Location = New System.Drawing.Point(88, 48)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.PasswordChar = Global.Microsoft.VisualBasic.ChrW(42)
        Me.txtPassword.Size = New System.Drawing.Size(232, 20)
        Me.txtPassword.TabIndex = 3
        '
        'txtUserName
        '
        Me.txtUserName.Enabled = False
        Me.txtUserName.Location = New System.Drawing.Point(88, 24)
        Me.txtUserName.Name = "txtUserName"
        Me.txtUserName.Size = New System.Drawing.Size(232, 20)
        Me.txtUserName.TabIndex = 1
        '
        'lblPassword
        '
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Enabled = False
        Me.lblPassword.Location = New System.Drawing.Point(21, 51)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Size = New System.Drawing.Size(56, 13)
        Me.lblPassword.TabIndex = 2
        Me.lblPassword.Text = "Password:"
        '
        'lblUserName
        '
        Me.lblUserName.AutoSize = True
        Me.lblUserName.Enabled = False
        Me.lblUserName.Location = New System.Drawing.Point(14, 27)
        Me.lblUserName.Name = "lblUserName"
        Me.lblUserName.Size = New System.Drawing.Size(63, 13)
        Me.lblUserName.TabIndex = 0
        Me.lblUserName.Text = "User Name:"
        '
        'chkUseAuth
        '
        Me.chkUseAuth.Location = New System.Drawing.Point(16, 48)
        Me.chkUseAuth.Name = "chkUseAuth"
        Me.chkUseAuth.Size = New System.Drawing.Size(279, 24)
        Me.chkUseAuth.TabIndex = 2
        Me.chkUseAuth.Text = "My outgoing server (SMTP) requires authentication"
        '
        'smtpClient
        '
        Me.smtpClient.AuthPassword = Nothing
        Me.smtpClient.AuthUser = Nothing
        Me.smtpClient.ConnectionClass = Nothing
        Me.smtpClient.ConnectionFactory = Nothing
        Me.smtpClient.HeloDomain = "remobjects.com"
        Me.smtpClient.HostAddress = Nothing
        Me.smtpClient.HostName = ""
        Me.smtpClient.Port = 25
        Me.smtpClient.UseAuth = False
        '
        'lblIP
        '
        Me.lblIP.AutoSize = True
        Me.lblIP.Font = New System.Drawing.Font("Verdana", 15.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblIP.Location = New System.Drawing.Point(415, 134)
        Me.lblIP.Name = "lblIP"
        Me.lblIP.Size = New System.Drawing.Size(173, 25)
        Me.lblIP.TabIndex = 17
        Me.lblIP.Text = "Internet Pack"
        '
        'lblRO
        '
        Me.lblRO.AutoSize = True
        Me.lblRO.Font = New System.Drawing.Font("Verdana", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblRO.Location = New System.Drawing.Point(464, 102)
        Me.lblRO.Name = "lblRO"
        Me.lblRO.Size = New System.Drawing.Size(124, 23)
        Me.lblRO.TabIndex = 18
        Me.lblRO.Text = "RemObjects"
        '
        'pictureBox1
        '
        Me.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(501, 14)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(88, 80)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.pictureBox1.TabIndex = 19
        Me.pictureBox1.TabStop = False
        '
        'groupBox3
        '
        Me.groupBox3.Controls.Add(Me.lblSenderName)
        Me.groupBox3.Controls.Add(Me.lblFromEmail)
        Me.groupBox3.Controls.Add(Me.txtMessage)
        Me.groupBox3.Controls.Add(Me.txtSenderAddress)
        Me.groupBox3.Controls.Add(Me.label5)
        Me.groupBox3.Controls.Add(Me.txtSenderName)
        Me.groupBox3.Controls.Add(Me.txtTo)
        Me.groupBox3.Controls.Add(Me.label6)
        Me.groupBox3.Controls.Add(Me.label3)
        Me.groupBox3.Controls.Add(Me.txtCC)
        Me.groupBox3.Controls.Add(Me.txtSubject)
        Me.groupBox3.Controls.Add(Me.label7)
        Me.groupBox3.Controls.Add(Me.txtBCC)
        Me.groupBox3.Controls.Add(Me.label8)
        Me.groupBox3.Controls.Add(Me.btnSendEMail)
        Me.groupBox3.Location = New System.Drawing.Point(7, 174)
        Me.groupBox3.Name = "groupBox3"
        Me.groupBox3.Size = New System.Drawing.Size(584, 296)
        Me.groupBox3.TabIndex = 16
        Me.groupBox3.TabStop = False
        Me.groupBox3.Text = "EMail"
        '
        'lblSenderName
        '
        Me.lblSenderName.AutoSize = True
        Me.lblSenderName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblSenderName.Location = New System.Drawing.Point(29, 19)
        Me.lblSenderName.Name = "lblSenderName"
        Me.lblSenderName.Size = New System.Drawing.Size(64, 13)
        Me.lblSenderName.TabIndex = 0
        Me.lblSenderName.Text = "From Name:"
        '
        'lblFromEmail
        '
        Me.lblFromEmail.AutoSize = True
        Me.lblFromEmail.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.lblFromEmail.Location = New System.Drawing.Point(60, 43)
        Me.lblFromEmail.Name = "lblFromEmail"
        Me.lblFromEmail.Size = New System.Drawing.Size(33, 13)
        Me.lblFromEmail.TabIndex = 2
        Me.lblFromEmail.Text = "From:"
        '
        'txtMessage
        '
        Me.txtMessage.Location = New System.Drawing.Point(104, 120)
        Me.txtMessage.Multiline = True
        Me.txtMessage.Name = "txtMessage"
        Me.txtMessage.Size = New System.Drawing.Size(464, 136)
        Me.txtMessage.TabIndex = 13
        Me.txtMessage.Text = "<Please enter message body of the letter>"
        '
        'txtSenderAddress
        '
        Me.txtSenderAddress.Location = New System.Drawing.Point(104, 40)
        Me.txtSenderAddress.Name = "txtSenderAddress"
        Me.txtSenderAddress.Size = New System.Drawing.Size(232, 20)
        Me.txtSenderAddress.TabIndex = 3
        Me.txtSenderAddress.Text = "alexanderk@remobjects.com"
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label5.Location = New System.Drawing.Point(40, 123)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(53, 13)
        Me.label5.TabIndex = 12
        Me.label5.Text = "Message:"
        '
        'txtSenderName
        '
        Me.txtSenderName.Location = New System.Drawing.Point(104, 16)
        Me.txtSenderName.Name = "txtSenderName"
        Me.txtSenderName.Size = New System.Drawing.Size(232, 20)
        Me.txtSenderName.TabIndex = 1
        Me.txtSenderName.Text = "John Doe"
        '
        'txtTo
        '
        Me.txtTo.Location = New System.Drawing.Point(392, 16)
        Me.txtTo.Name = "txtTo"
        Me.txtTo.Size = New System.Drawing.Size(176, 20)
        Me.txtTo.TabIndex = 5
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label6.Location = New System.Drawing.Point(366, 19)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(23, 13)
        Me.label6.TabIndex = 4
        Me.label6.Text = "To:"
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label3.Location = New System.Drawing.Point(47, 91)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(46, 13)
        Me.label3.TabIndex = 10
        Me.label3.Text = "Subject:"
        '
        'txtCC
        '
        Me.txtCC.Location = New System.Drawing.Point(392, 40)
        Me.txtCC.Name = "txtCC"
        Me.txtCC.Size = New System.Drawing.Size(176, 20)
        Me.txtCC.TabIndex = 7
        '
        'txtSubject
        '
        Me.txtSubject.Location = New System.Drawing.Point(104, 88)
        Me.txtSubject.Name = "txtSubject"
        Me.txtSubject.Size = New System.Drawing.Size(464, 20)
        Me.txtSubject.TabIndex = 11
        Me.txtSubject.Text = "<Please enter subject of the letter>"
        '
        'label7
        '
        Me.label7.AutoSize = True
        Me.label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label7.Location = New System.Drawing.Point(365, 43)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(24, 13)
        Me.label7.TabIndex = 6
        Me.label7.Text = "CC:"
        '
        'txtBCC
        '
        Me.txtBCC.Location = New System.Drawing.Point(392, 64)
        Me.txtBCC.Name = "txtBCC"
        Me.txtBCC.Size = New System.Drawing.Size(176, 20)
        Me.txtBCC.TabIndex = 9
        '
        'label8
        '
        Me.label8.AutoSize = True
        Me.label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label8.Location = New System.Drawing.Point(358, 67)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(31, 13)
        Me.label8.TabIndex = 8
        Me.label8.Text = "BCC:"
        '
        'btnSendEMail
        '
        Me.btnSendEMail.Location = New System.Drawing.Point(456, 264)
        Me.btnSendEMail.Name = "btnSendEMail"
        Me.btnSendEMail.Size = New System.Drawing.Size(112, 23)
        Me.btnSendEMail.TabIndex = 14
        Me.btnSendEMail.Text = "Send Email"
        '
        'MainForm
        '
        Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
        Me.ClientSize = New System.Drawing.Size(598, 476)
        Me.Controls.Add(Me.lblIP)
        Me.Controls.Add(Me.lblRO)
        Me.Controls.Add(Me.pictureBox1)
        Me.Controls.Add(Me.groupBox3)
        Me.Controls.Add(Me.groupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "SMTP Client Sample"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.groupBox2.ResumeLayout(False)
        Me.groupBox2.PerformLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.groupBox3.ResumeLayout(False)
        Me.groupBox3.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private Sub chkUseAuth_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkUseAuth.CheckedChanged
        lblUserName.Enabled = chkUseAuth.Checked
        txtUserName.Enabled = chkUseAuth.Checked
        lblPassword.Enabled = chkUseAuth.Checked
        txtPassword.Enabled = chkUseAuth.Checked
    End Sub

    Private Sub btnSendEMail_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendEMail.Click
        ' Clear lists before sending
        msg.To.Clear()
        msg.Cc.Clear()
        msg.Bcc.Clear()
        Try

            msg.To.Add(getRequiredValue(txtTo, "Value of field 'To' is required"))

            Dim cc As String
            cc = txtCC.Text.Trim()
            If cc.Length > 0 Then
                msg.Cc.Add(cc)
            End If


            Dim bcc As String
            bcc = txtBCC.Text.Trim()
            If (bcc.Length > 0) Then
                msg.Bcc.Add(bcc)
            End If

            smtpClient.HostName = getRequiredValue(txtSMTPServer, "Host Name is required")
            smtpClient.HeloDomain = "remobjects.com"

            smtpClient.UseAuth = chkUseAuth.Checked
            smtpClient.AuthUser = txtUserName.Text
            smtpClient.AuthPassword = txtPassword.Text

            msg.From.Name = getRequiredValue(txtSenderName, "From field value is required")
            msg.From.Address = getRequiredValue(txtSenderAddress, "EMail field value is required")
            msg.Subject = getRequiredValue(txtSubject, "Subject of letter is required")
            msg.Contents = getRequiredValue(txtMessage, "Content of letter is required")


            smtpClient.Open()
            smtpClient.SendMessage(msg)
            smtpClient.Close()
            MessageBox.Show("Email has been sent successfully!", "SMTP Client Sample")

        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error during sending letter.", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try


    End Sub

    Private Function getRequiredValue(ByVal ctrl As TextBox, ByVal errorMessage As String) As String

        If ctrl Is Nothing Then
            Return Nothing
        End If

        Dim result As String
        result = ctrl.Text.Trim()
        If result.Length = 0 Then
            ctrl.Focus()
            Err.Raise(errorMessage)
        End If

        Return result

    End Function
End Class
