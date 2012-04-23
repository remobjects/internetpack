namespace SMTP_Client_Sample;

interface

uses
  System.Windows.Forms,
  System.Drawing;

type
  /// <summary>
  /// Summary description for MainForm.
  /// </summary>
  MainForm = class(System.Windows.Forms.Form)
  {$REGION Windows Form Designer generated fields}
  private
    smtpClient: RemObjects.InternetPack.Email.SmtpClient;
    btnSendEMail: System.Windows.Forms.Button;
    label8: System.Windows.Forms.Label;
    txtBCC: System.Windows.Forms.TextBox;
    label7: System.Windows.Forms.Label;
    txtSubject: System.Windows.Forms.TextBox;
    txtCC: System.Windows.Forms.TextBox;
    label3: System.Windows.Forms.Label;
    label6: System.Windows.Forms.Label;
    txtTo: System.Windows.Forms.TextBox;
    txtSenderName: System.Windows.Forms.TextBox;
    label5: System.Windows.Forms.Label;
    txtSenderAddress: System.Windows.Forms.TextBox;
    txtMessage: System.Windows.Forms.TextBox;
    lblFromEmail: System.Windows.Forms.Label;
    lblSenderName: System.Windows.Forms.Label;
    groupBox3: System.Windows.Forms.GroupBox;
    chkUseAuth: System.Windows.Forms.CheckBox;
    lblUserName: System.Windows.Forms.Label;
    lblPassword: System.Windows.Forms.Label;
    txtUserName: System.Windows.Forms.TextBox;
    txtPassword: System.Windows.Forms.TextBox;
    groupBox2: System.Windows.Forms.GroupBox;
    txtSMTPServer: System.Windows.Forms.TextBox;
    lblSMTPServer: System.Windows.Forms.Label;
    groupBox1: System.Windows.Forms.GroupBox;
    pictureBox1: System.Windows.Forms.PictureBox;
    lblRO: System.Windows.Forms.Label;
    lblIP: System.Windows.Forms.Label;
    msg: RemObjects.InternetPack.Messages.MailMessage;
    components: System.ComponentModel.Container := nil;
    method InitializeComponent;
  {$ENDREGION}
  private
    method btnSendEMail_Click(sender: System.Object; e: System.EventArgs);
    method getRequiredValue(ctrl: TextBox; errorMessage: String): String;
    method chkUseAuth_CheckedChanged(sender: System.Object; e: System.EventArgs);
  protected
    method Dispose(aDisposing: boolean); override;
  public
    constructor;
    class method Main;
  end;

implementation

{$REGION Construction and Disposition}
constructor MainForm;
begin
  //
  // Required for Windows Form Designer support
  //
  InitializeComponent();

  //
  // TODO: Add any constructor code after InitializeComponent call
  //
  msg := new RemObjects.InternetPack.Messages.MailMessage();
end;

method MainForm.Dispose(aDisposing: boolean);
begin
  if aDisposing then begin
    if assigned(components) then
      components.Dispose();

    //
    // TODO: Add custom disposition code here
    //
  end;
  inherited Dispose(aDisposing);
end;
{$ENDREGION}

{$REGION Windows Form Designer generated code}
method MainForm.InitializeComponent;
begin
  var resources: System.ComponentModel.ComponentResourceManager := new System.ComponentModel.ComponentResourceManager(typeOf(MainForm));
  self.lblIP := new System.Windows.Forms.Label();
  self.lblRO := new System.Windows.Forms.Label();
  self.pictureBox1 := new System.Windows.Forms.PictureBox();
  self.groupBox1 := new System.Windows.Forms.GroupBox();
  self.lblSMTPServer := new System.Windows.Forms.Label();
  self.txtSMTPServer := new System.Windows.Forms.TextBox();
  self.groupBox2 := new System.Windows.Forms.GroupBox();
  self.txtPassword := new System.Windows.Forms.TextBox();
  self.txtUserName := new System.Windows.Forms.TextBox();
  self.lblPassword := new System.Windows.Forms.Label();
  self.lblUserName := new System.Windows.Forms.Label();
  self.chkUseAuth := new System.Windows.Forms.CheckBox();
  self.groupBox3 := new System.Windows.Forms.GroupBox();
  self.lblSenderName := new System.Windows.Forms.Label();
  self.lblFromEmail := new System.Windows.Forms.Label();
  self.txtMessage := new System.Windows.Forms.TextBox();
  self.txtSenderAddress := new System.Windows.Forms.TextBox();
  self.label5 := new System.Windows.Forms.Label();
  self.txtSenderName := new System.Windows.Forms.TextBox();
  self.txtTo := new System.Windows.Forms.TextBox();
  self.label6 := new System.Windows.Forms.Label();
  self.label3 := new System.Windows.Forms.Label();
  self.txtCC := new System.Windows.Forms.TextBox();
  self.txtSubject := new System.Windows.Forms.TextBox();
  self.label7 := new System.Windows.Forms.Label();
  self.txtBCC := new System.Windows.Forms.TextBox();
  self.label8 := new System.Windows.Forms.Label();
  self.btnSendEMail := new System.Windows.Forms.Button();
  self.smtpClient := new RemObjects.InternetPack.Email.SmtpClient();
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).BeginInit();
  self.groupBox1.SuspendLayout();
  self.groupBox2.SuspendLayout();
  self.groupBox3.SuspendLayout();
  self.SuspendLayout();
  // 
  // lblIP
  // 
  self.lblIP.AutoSize := true;
  self.lblIP.Font := new System.Drawing.Font('Verdana', 15.75, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.lblIP.Location := new System.Drawing.Point(415, 134);
  self.lblIP.Name := 'lblIP';
  self.lblIP.Size := new System.Drawing.Size(173, 25);
  self.lblIP.TabIndex := 17;
  self.lblIP.Text := 'Internet Pack';
  // 
  // lblRO
  // 
  self.lblRO.AutoSize := true;
  self.lblRO.Font := new System.Drawing.Font('Verdana', 14.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.lblRO.Location := new System.Drawing.Point(464, 102);
  self.lblRO.Name := 'lblRO';
  self.lblRO.Size := new System.Drawing.Size(124, 23);
  self.lblRO.TabIndex := 18;
  self.lblRO.Text := 'RemObjects';
  // 
  // pictureBox1
  // 
  self.pictureBox1.BackColor := System.Drawing.SystemColors.ActiveBorder;
  self.pictureBox1.Image := (resources.GetObject('pictureBox1.Image') as System.Drawing.Image);
  self.pictureBox1.Location := new System.Drawing.Point(501, 14);
  self.pictureBox1.Name := 'pictureBox1';
  self.pictureBox1.Size := new System.Drawing.Size(88, 80);
  self.pictureBox1.SizeMode := System.Windows.Forms.PictureBoxSizeMode.StretchImage;
  self.pictureBox1.TabIndex := 19;
  self.pictureBox1.TabStop := false;
  // 
  // groupBox1
  // 
  self.groupBox1.Controls.Add(self.lblSMTPServer);
  self.groupBox1.Controls.Add(self.txtSMTPServer);
  self.groupBox1.Controls.Add(self.groupBox2);
  self.groupBox1.Controls.Add(self.chkUseAuth);
  self.groupBox1.Location := new System.Drawing.Point(7, 6);
  self.groupBox1.Name := 'groupBox1';
  self.groupBox1.Size := new System.Drawing.Size(360, 160);
  self.groupBox1.TabIndex := 15;
  self.groupBox1.TabStop := false;
  self.groupBox1.Text := 'Server Information';
  // 
  // lblSMTPServer
  // 
  self.lblSMTPServer.Location := new System.Drawing.Point(6, 19);
  self.lblSMTPServer.Name := 'lblSMTPServer';
  self.lblSMTPServer.Size := new System.Drawing.Size(160, 15);
  self.lblSMTPServer.TabIndex := 0;
  self.lblSMTPServer.Text := 'Outgoing Mail Server (SMTP):';
  // 
  // txtSMTPServer
  // 
  self.txtSMTPServer.Location := new System.Drawing.Point(168, 16);
  self.txtSMTPServer.Name := 'txtSMTPServer';
  self.txtSMTPServer.Size := new System.Drawing.Size(168, 20);
  self.txtSMTPServer.TabIndex := 1;
  self.txtSMTPServer.Text := 'ws7.elitedev.com';
  // 
  // groupBox2
  // 
  self.groupBox2.Controls.Add(self.txtPassword);
  self.groupBox2.Controls.Add(self.txtUserName);
  self.groupBox2.Controls.Add(self.lblPassword);
  self.groupBox2.Controls.Add(self.lblUserName);
  self.groupBox2.Location := new System.Drawing.Point(16, 72);
  self.groupBox2.Name := 'groupBox2';
  self.groupBox2.Size := new System.Drawing.Size(336, 80);
  self.groupBox2.TabIndex := 3;
  self.groupBox2.TabStop := false;
  self.groupBox2.Text := 'Login Information';
  // 
  // txtPassword
  // 
  self.txtPassword.Enabled := false;
  self.txtPassword.Location := new System.Drawing.Point(88, 48);
  self.txtPassword.Name := 'txtPassword';
  self.txtPassword.PasswordChar := '*';
  self.txtPassword.Size := new System.Drawing.Size(232, 20);
  self.txtPassword.TabIndex := 3;
  // 
  // txtUserName
  // 
  self.txtUserName.Enabled := false;
  self.txtUserName.Location := new System.Drawing.Point(88, 24);
  self.txtUserName.Name := 'txtUserName';
  self.txtUserName.Size := new System.Drawing.Size(232, 20);
  self.txtUserName.TabIndex := 1;
  // 
  // lblPassword
  // 
  self.lblPassword.AutoSize := true;
  self.lblPassword.Enabled := false;
  self.lblPassword.Location := new System.Drawing.Point(25, 51);
  self.lblPassword.Name := 'lblPassword';
  self.lblPassword.Size := new System.Drawing.Size(56, 13);
  self.lblPassword.TabIndex := 2;
  self.lblPassword.Text := 'Password:';
  // 
  // lblUserName
  // 
  self.lblUserName.AutoSize := true;
  self.lblUserName.Enabled := false;
  self.lblUserName.Location := new System.Drawing.Point(18, 27);
  self.lblUserName.Name := 'lblUserName';
  self.lblUserName.Size := new System.Drawing.Size(63, 13);
  self.lblUserName.TabIndex := 0;
  self.lblUserName.Text := 'User Name:';
  // 
  // chkUseAuth
  // 
  self.chkUseAuth.Location := new System.Drawing.Point(16, 48);
  self.chkUseAuth.Name := 'chkUseAuth';
  self.chkUseAuth.Size := new System.Drawing.Size(279, 24);
  self.chkUseAuth.TabIndex := 2;
  self.chkUseAuth.Text := 'My outgoing server (SMTP) requires authentication';
  self.chkUseAuth.CheckedChanged += new System.EventHandler(@self.chkUseAuth_CheckedChanged);
  // 
  // groupBox3
  // 
  self.groupBox3.Controls.Add(self.lblSenderName);
  self.groupBox3.Controls.Add(self.lblFromEmail);
  self.groupBox3.Controls.Add(self.txtMessage);
  self.groupBox3.Controls.Add(self.txtSenderAddress);
  self.groupBox3.Controls.Add(self.label5);
  self.groupBox3.Controls.Add(self.txtSenderName);
  self.groupBox3.Controls.Add(self.txtTo);
  self.groupBox3.Controls.Add(self.label6);
  self.groupBox3.Controls.Add(self.label3);
  self.groupBox3.Controls.Add(self.txtCC);
  self.groupBox3.Controls.Add(self.txtSubject);
  self.groupBox3.Controls.Add(self.label7);
  self.groupBox3.Controls.Add(self.txtBCC);
  self.groupBox3.Controls.Add(self.label8);
  self.groupBox3.Controls.Add(self.btnSendEMail);
  self.groupBox3.Location := new System.Drawing.Point(7, 174);
  self.groupBox3.Name := 'groupBox3';
  self.groupBox3.Size := new System.Drawing.Size(584, 296);
  self.groupBox3.TabIndex := 16;
  self.groupBox3.TabStop := false;
  self.groupBox3.Text := 'EMail';
  // 
  // lblSenderName
  // 
  self.lblSenderName.AutoSize := true;
  self.lblSenderName.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.lblSenderName.Location := new System.Drawing.Point(29, 19);
  self.lblSenderName.Name := 'lblSenderName';
  self.lblSenderName.Size := new System.Drawing.Size(64, 13);
  self.lblSenderName.TabIndex := 0;
  self.lblSenderName.Text := 'From Name:';
  // 
  // lblFromEmail
  // 
  self.lblFromEmail.AutoSize := true;
  self.lblFromEmail.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.lblFromEmail.Location := new System.Drawing.Point(60, 43);
  self.lblFromEmail.Name := 'lblFromEmail';
  self.lblFromEmail.Size := new System.Drawing.Size(33, 13);
  self.lblFromEmail.TabIndex := 2;
  self.lblFromEmail.Text := 'From:';
  // 
  // txtMessage
  // 
  self.txtMessage.Location := new System.Drawing.Point(104, 120);
  self.txtMessage.Multiline := true;
  self.txtMessage.Name := 'txtMessage';
  self.txtMessage.Size := new System.Drawing.Size(464, 136);
  self.txtMessage.TabIndex := 13;
  self.txtMessage.Text := '<Please enter message body of the letter>';
  // 
  // txtSenderAddress
  // 
  self.txtSenderAddress.Location := new System.Drawing.Point(104, 40);
  self.txtSenderAddress.Name := 'txtSenderAddress';
  self.txtSenderAddress.Size := new System.Drawing.Size(232, 20);
  self.txtSenderAddress.TabIndex := 3;
  self.txtSenderAddress.Text := 'alexanderk@remobjects.com';
  // 
  // label5
  // 
  self.label5.AutoSize := true;
  self.label5.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.label5.Location := new System.Drawing.Point(44, 123);
  self.label5.Name := 'label5';
  self.label5.Size := new System.Drawing.Size(53, 13);
  self.label5.TabIndex := 12;
  self.label5.Text := 'Message:';
  // 
  // txtSenderName
  // 
  self.txtSenderName.Location := new System.Drawing.Point(104, 16);
  self.txtSenderName.Name := 'txtSenderName';
  self.txtSenderName.Size := new System.Drawing.Size(232, 20);
  self.txtSenderName.TabIndex := 1;
  self.txtSenderName.Text := 'John Doe';
  // 
  // txtTo
  // 
  self.txtTo.Location := new System.Drawing.Point(392, 16);
  self.txtTo.Name := 'txtTo';
  self.txtTo.Size := new System.Drawing.Size(176, 20);
  self.txtTo.TabIndex := 5;
  // 
  // label6
  // 
  self.label6.AutoSize := true;
  self.label6.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.label6.Location := new System.Drawing.Point(366, 19);
  self.label6.Name := 'label6';
  self.label6.Size := new System.Drawing.Size(23, 13);
  self.label6.TabIndex := 4;
  self.label6.Text := 'To:';
  // 
  // label3
  // 
  self.label3.AutoSize := true;
  self.label3.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.label3.Location := new System.Drawing.Point(51, 91);
  self.label3.Name := 'label3';
  self.label3.Size := new System.Drawing.Size(46, 13);
  self.label3.TabIndex := 10;
  self.label3.Text := 'Subject:';
  // 
  // txtCC
  // 
  self.txtCC.Location := new System.Drawing.Point(392, 40);
  self.txtCC.Name := 'txtCC';
  self.txtCC.Size := new System.Drawing.Size(176, 20);
  self.txtCC.TabIndex := 7;
  // 
  // txtSubject
  // 
  self.txtSubject.Location := new System.Drawing.Point(104, 88);
  self.txtSubject.Name := 'txtSubject';
  self.txtSubject.Size := new System.Drawing.Size(464, 20);
  self.txtSubject.TabIndex := 11;
  self.txtSubject.Text := '<Please enter subject of the letter>';
  // 
  // label7
  // 
  self.label7.AutoSize := true;
  self.label7.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.label7.Location := new System.Drawing.Point(365, 43);
  self.label7.Name := 'label7';
  self.label7.Size := new System.Drawing.Size(24, 13);
  self.label7.TabIndex := 6;
  self.label7.Text := 'CC:';
  // 
  // txtBCC
  // 
  self.txtBCC.Location := new System.Drawing.Point(392, 64);
  self.txtBCC.Name := 'txtBCC';
  self.txtBCC.Size := new System.Drawing.Size(176, 20);
  self.txtBCC.TabIndex := 9;
  // 
  // label8
  // 
  self.label8.AutoSize := true;
  self.label8.Font := new System.Drawing.Font('Microsoft Sans Serif', 8.25, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (204 as System.Byte));
  self.label8.Location := new System.Drawing.Point(358, 67);
  self.label8.Name := 'label8';
  self.label8.Size := new System.Drawing.Size(31, 13);
  self.label8.TabIndex := 8;
  self.label8.Text := 'BCC:';
  // 
  // btnSendEMail
  // 
  self.btnSendEMail.Location := new System.Drawing.Point(456, 264);
  self.btnSendEMail.Name := 'btnSendEMail';
  self.btnSendEMail.Size := new System.Drawing.Size(112, 23);
  self.btnSendEMail.TabIndex := 14;
  self.btnSendEMail.Text := 'Send Email';
  self.btnSendEMail.Click += new System.EventHandler(@self.btnSendEMail_Click);
  // 
  // smtpClient
  // 
  self.smtpClient.AuthPassword := nil;
  self.smtpClient.AuthUser := nil;
  self.smtpClient.ConnectionClass := nil;
  self.smtpClient.ConnectionFactory := nil;
  self.smtpClient.HeloDomain := 'remobjects.com';
  self.smtpClient.HostAddress := nil;
  self.smtpClient.HostName := '';
  self.smtpClient.Port := 25;
  self.smtpClient.UseAuth := false;
  // 
  // MainForm
  // 
  self.AutoScaleBaseSize := new System.Drawing.Size(5, 13);
  self.ClientSize := new System.Drawing.Size(598, 476);
  self.Controls.Add(self.lblIP);
  self.Controls.Add(self.lblRO);
  self.Controls.Add(self.pictureBox1);
  self.Controls.Add(self.groupBox1);
  self.Controls.Add(self.groupBox3);
  self.FormBorderStyle := System.Windows.Forms.FormBorderStyle.FixedDialog;
  self.Icon := (resources.GetObject('$this.Icon') as System.Drawing.Icon);
  self.MaximizeBox := false;
  self.Name := 'MainForm';
  self.StartPosition := System.Windows.Forms.FormStartPosition.CenterScreen;
  self.Text := 'SMTP Client Sample';
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).EndInit();
  self.groupBox1.ResumeLayout(false);
  self.groupBox1.PerformLayout();
  self.groupBox2.ResumeLayout(false);
  self.groupBox2.PerformLayout();
  self.groupBox3.ResumeLayout(false);
  self.groupBox3.PerformLayout();
  self.ResumeLayout(false);
  self.PerformLayout();
end;
{$ENDREGION}

{$REGION Application Entry Point}
[STAThread]
class method MainForm.Main;
begin
  Application.EnableVisualStyles();

  try
    with lForm := new MainForm() do
      Application.Run(lForm);
  except
    on E: Exception do begin
      MessageBox.Show(E.Message);
    end;
  end;
end;
{$ENDREGION}

method MainForm.chkUseAuth_CheckedChanged(sender: System.Object; e: System.EventArgs);
begin
  lblUserName.Enabled := chkUseAuth.Checked;
	txtUserName.Enabled := chkUseAuth.Checked;
	lblPassword.Enabled := chkUseAuth.Checked;
	txtPassword.Enabled := chkUseAuth.Checked;
end;

method MainForm.btnSendEMail_Click(sender: System.Object; e: System.EventArgs);
var 
  cc: String;
  bcc: String;
begin
  // Clear lists before sending
	msg.To.Clear();
	msg.Cc.Clear();
	msg.Bcc.Clear();
	try
		msg.To.Add(getRequiredValue(txtTo, 'Value of field "To" is required'));

    cc := txtCC.Text.Trim();
		if (cc.Length > 0) then msg.Cc.Add(cc);

		bcc := txtBCC.Text.Trim();
		if (bcc.Length > 0) then msg.Bcc.Add(bcc);

		smtpClient.HostName := getRequiredValue(txtSMTPServer, 'Host Name is required');
		smtpClient.HeloDomain := 'remobjects.com';

		smtpClient.UseAuth := chkUseAuth.Checked;
		smtpClient.AuthUser := txtUserName.Text;
		smtpClient.AuthPassword := txtPassword.Text;

		msg.From.Name := getRequiredValue(txtSenderName, 'From field value is required');
		msg.From.Address := getRequiredValue(txtSenderAddress, 'EMail field value is required');
		msg.Subject := getRequiredValue(txtSubject, 'Subject of letter is required');
		msg.Contents := getRequiredValue(txtMessage, 'Content of letter is required');

			
		smtpClient.Open();
		smtpClient.SendMessage(msg);
		smtpClient.Close();
		MessageBox.Show('Email has been sent successfully!', 'SMTP Client Sample');
except
    on ex: Exception do begin
			MessageBox.Show(ex.Message, 'Error during sending letter.', MessageBoxButtons.OK, MessageBoxIcon.Error);
		end;
end;

end;

method MainForm.getRequiredValue(ctrl: TextBox; errorMessage: String): String;
begin
	if (not Assigned(ctrl)) then Exit(nil);
	
  result := ctrl.Text.Trim();
	if (result.Length = 0) then begin			
	  ctrl.Focus();
		raise new Exception(errorMessage);
	end;
end;


		
end.