using System;
using System.Windows.Forms;

namespace SMTP_Client_Sample
{
	public class MainForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox txtUserName;
		private System.Windows.Forms.TextBox txtPassword;
		private System.Windows.Forms.Label lblSenderName;
		private System.Windows.Forms.TextBox txtSenderName;
		private System.Windows.Forms.Label lblSMTPServer;
		private System.Windows.Forms.TextBox txtSMTPServer;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label lblRO;
		private System.Windows.Forms.Label lblIP;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txtSenderAddress;
		private System.Windows.Forms.TextBox txtSubject;
		private System.Windows.Forms.TextBox txtMessage;
		private System.Windows.Forms.TextBox txtTo;
		private System.Windows.Forms.TextBox txtCC;
		private System.Windows.Forms.TextBox txtBCC;
		private RemObjects.InternetPack.Email.SmtpClient smtpClient;
		private System.Windows.Forms.CheckBox chkUseAuth;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblUserName;
		private System.Windows.Forms.Label lblPassword;
		private System.Windows.Forms.Button btnSendEMail;
		private System.Windows.Forms.Label lblFromEmail;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private RemObjects.InternetPack.Messages.MailMessage msg;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			msg = new RemObjects.InternetPack.Messages.MailMessage();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.chkUseAuth = new System.Windows.Forms.CheckBox();
			this.lblUserName = new System.Windows.Forms.Label();
			this.lblPassword = new System.Windows.Forms.Label();
			this.txtUserName = new System.Windows.Forms.TextBox();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblSenderName = new System.Windows.Forms.Label();
			this.txtSenderName = new System.Windows.Forms.TextBox();
			this.txtSenderAddress = new System.Windows.Forms.TextBox();
			this.lblFromEmail = new System.Windows.Forms.Label();
			this.lblSMTPServer = new System.Windows.Forms.Label();
			this.txtSMTPServer = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.lblRO = new System.Windows.Forms.Label();
			this.lblIP = new System.Windows.Forms.Label();
			this.txtSubject = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.txtMessage = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnSendEMail = new System.Windows.Forms.Button();
			this.txtTo = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.txtCC = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.txtBCC = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.smtpClient = new RemObjects.InternetPack.Email.SmtpClient();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkUseAuth
			// 
			this.chkUseAuth.Location = new System.Drawing.Point(16, 48);
			this.chkUseAuth.Name = "chkUseAuth";
			this.chkUseAuth.Size = new System.Drawing.Size(279, 24);
			this.chkUseAuth.TabIndex = 2;
			this.chkUseAuth.Text = "My outgoing server (SMTP) requires authentication";
			this.chkUseAuth.CheckedChanged += new System.EventHandler(this.chkUseAuth_CheckedChanged);
			// 
			// lblUserName
			// 
			this.lblUserName.AutoSize = true;
			this.lblUserName.Enabled = false;
			this.lblUserName.Location = new System.Drawing.Point(19, 27);
			this.lblUserName.Name = "lblUserName";
			this.lblUserName.Size = new System.Drawing.Size(63, 13);
			this.lblUserName.TabIndex = 0;
			this.lblUserName.Text = "User Name:";
			// 
			// lblPassword
			// 
			this.lblPassword.AutoSize = true;
			this.lblPassword.Enabled = false;
			this.lblPassword.Location = new System.Drawing.Point(26, 51);
			this.lblPassword.Name = "lblPassword";
			this.lblPassword.Size = new System.Drawing.Size(56, 13);
			this.lblPassword.TabIndex = 2;
			this.lblPassword.Text = "Password:";
			// 
			// txtUserName
			// 
			this.txtUserName.Enabled = false;
			this.txtUserName.Location = new System.Drawing.Point(88, 24);
			this.txtUserName.Name = "txtUserName";
			this.txtUserName.Size = new System.Drawing.Size(232, 20);
			this.txtUserName.TabIndex = 1;
			// 
			// txtPassword
			// 
			this.txtPassword.Enabled = false;
			this.txtPassword.Location = new System.Drawing.Point(88, 48);
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.PasswordChar = '*';
			this.txtPassword.Size = new System.Drawing.Size(232, 20);
			this.txtPassword.TabIndex = 3;
			// 
			// lblSenderName
			// 
			this.lblSenderName.AutoSize = true;
			this.lblSenderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblSenderName.Location = new System.Drawing.Point(34, 19);
			this.lblSenderName.Name = "lblSenderName";
			this.lblSenderName.Size = new System.Drawing.Size(64, 13);
			this.lblSenderName.TabIndex = 0;
			this.lblSenderName.Text = "From Name:";
			// 
			// txtSenderName
			// 
			this.txtSenderName.Location = new System.Drawing.Point(104, 16);
			this.txtSenderName.Name = "txtSenderName";
			this.txtSenderName.Size = new System.Drawing.Size(232, 20);
			this.txtSenderName.TabIndex = 1;
			this.txtSenderName.Text = "John Doe";
			// 
			// txtSenderAddress
			// 
			this.txtSenderAddress.Location = new System.Drawing.Point(104, 40);
			this.txtSenderAddress.Name = "txtSenderAddress";
			this.txtSenderAddress.Size = new System.Drawing.Size(232, 20);
			this.txtSenderAddress.TabIndex = 3;
			this.txtSenderAddress.Text = "alexanderk@remobjects.com";
			// 
			// lblFromEmail
			// 
			this.lblFromEmail.AutoSize = true;
			this.lblFromEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblFromEmail.Location = new System.Drawing.Point(65, 43);
			this.lblFromEmail.Name = "lblFromEmail";
			this.lblFromEmail.Size = new System.Drawing.Size(33, 13);
			this.lblFromEmail.TabIndex = 2;
			this.lblFromEmail.Text = "From:";
			// 
			// lblSMTPServer
			// 
			this.lblSMTPServer.Location = new System.Drawing.Point(6, 19);
			this.lblSMTPServer.Name = "lblSMTPServer";
			this.lblSMTPServer.Size = new System.Drawing.Size(160, 15);
			this.lblSMTPServer.TabIndex = 0;
			this.lblSMTPServer.Text = "Outgoing Mail Server (SMTP):";
			// 
			// txtSMTPServer
			// 
			this.txtSMTPServer.Location = new System.Drawing.Point(168, 16);
			this.txtSMTPServer.Name = "txtSMTPServer";
			this.txtSMTPServer.Size = new System.Drawing.Size(168, 20);
			this.txtSMTPServer.TabIndex = 1;
			this.txtSMTPServer.Text = "ws7.elitedev.com";
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(502, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(88, 80);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox1.TabIndex = 14;
			this.pictureBox1.TabStop = false;
			// 
			// lblRO
			// 
			this.lblRO.AutoSize = true;
			this.lblRO.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblRO.Location = new System.Drawing.Point(465, 104);
			this.lblRO.Name = "lblRO";
			this.lblRO.Size = new System.Drawing.Size(124, 23);
			this.lblRO.TabIndex = 3;
			this.lblRO.Text = "RemObjects";
			// 
			// lblIP
			// 
			this.lblIP.AutoSize = true;
			this.lblIP.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.lblIP.Location = new System.Drawing.Point(416, 136);
			this.lblIP.Name = "lblIP";
			this.lblIP.Size = new System.Drawing.Size(173, 25);
			this.lblIP.TabIndex = 2;
			this.lblIP.Text = "Internet Pack";
			// 
			// txtSubject
			// 
			this.txtSubject.Location = new System.Drawing.Point(104, 88);
			this.txtSubject.Name = "txtSubject";
			this.txtSubject.Size = new System.Drawing.Size(464, 20);
			this.txtSubject.TabIndex = 11;
			this.txtSubject.Text = "<Please enter subject of the letter>";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label3.Location = new System.Drawing.Point(52, 91);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(46, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Subject:";
			// 
			// txtMessage
			// 
			this.txtMessage.Location = new System.Drawing.Point(104, 120);
			this.txtMessage.Multiline = true;
			this.txtMessage.Name = "txtMessage";
			this.txtMessage.Size = new System.Drawing.Size(464, 136);
			this.txtMessage.TabIndex = 13;
			this.txtMessage.Text = "<Please enter message body of the letter>";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label5.Location = new System.Drawing.Point(45, 123);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 13);
			this.label5.TabIndex = 12;
			this.label5.Text = "Message:";
			// 
			// btnSendEMail
			// 
			this.btnSendEMail.Location = new System.Drawing.Point(456, 264);
			this.btnSendEMail.Name = "btnSendEMail";
			this.btnSendEMail.Size = new System.Drawing.Size(112, 23);
			this.btnSendEMail.TabIndex = 14;
			this.btnSendEMail.Text = "Send Email";
			this.btnSendEMail.Click += new System.EventHandler(this.btnSendEMail_Click);
			// 
			// txtTo
			// 
			this.txtTo.Location = new System.Drawing.Point(392, 16);
			this.txtTo.Name = "txtTo";
			this.txtTo.Size = new System.Drawing.Size(176, 20);
			this.txtTo.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label6.Location = new System.Drawing.Point(366, 19);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(23, 13);
			this.label6.TabIndex = 4;
			this.label6.Text = "To:";
			// 
			// txtCC
			// 
			this.txtCC.Location = new System.Drawing.Point(392, 40);
			this.txtCC.Name = "txtCC";
			this.txtCC.Size = new System.Drawing.Size(176, 20);
			this.txtCC.TabIndex = 7;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label7.Location = new System.Drawing.Point(365, 43);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(24, 13);
			this.label7.TabIndex = 6;
			this.label7.Text = "CC:";
			// 
			// txtBCC
			// 
			this.txtBCC.Location = new System.Drawing.Point(392, 64);
			this.txtBCC.Name = "txtBCC";
			this.txtBCC.Size = new System.Drawing.Size(176, 20);
			this.txtBCC.TabIndex = 9;
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label8.Location = new System.Drawing.Point(358, 67);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(31, 13);
			this.label8.TabIndex = 8;
			this.label8.Text = "BCC:";
			// 
			// smtpClient
			// 
			this.smtpClient.AuthPassword = null;
			this.smtpClient.AuthUser = null;
			this.smtpClient.ConnectionClass = null;
			this.smtpClient.ConnectionFactory = null;
			this.smtpClient.HeloDomain = "remobjects.com";
			this.smtpClient.HostAddress = null;
			this.smtpClient.HostName = "";
			this.smtpClient.Port = 25;
			this.smtpClient.UseAuth = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblSMTPServer);
			this.groupBox1.Controls.Add(this.txtSMTPServer);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.chkUseAuth);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(360, 160);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Server Information";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtPassword);
			this.groupBox2.Controls.Add(this.txtUserName);
			this.groupBox2.Controls.Add(this.lblPassword);
			this.groupBox2.Controls.Add(this.lblUserName);
			this.groupBox2.Location = new System.Drawing.Point(16, 72);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(336, 80);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Login Information";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.lblSenderName);
			this.groupBox3.Controls.Add(this.lblFromEmail);
			this.groupBox3.Controls.Add(this.txtMessage);
			this.groupBox3.Controls.Add(this.txtSenderAddress);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.txtSenderName);
			this.groupBox3.Controls.Add(this.txtTo);
			this.groupBox3.Controls.Add(this.label6);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.txtCC);
			this.groupBox3.Controls.Add(this.txtSubject);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.txtBCC);
			this.groupBox3.Controls.Add(this.label8);
			this.groupBox3.Controls.Add(this.btnSendEMail);
			this.groupBox3.Location = new System.Drawing.Point(8, 176);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(584, 296);
			this.groupBox3.TabIndex = 1;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "EMail";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(600, 478);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lblIP);
			this.Controls.Add(this.lblRO);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SMTP Client Sample";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}

		private void chkUseAuth_CheckedChanged(object sender, System.EventArgs e)
		{
			lblUserName.Enabled = chkUseAuth.Checked;
			txtUserName.Enabled = chkUseAuth.Checked;
			lblPassword.Enabled = chkUseAuth.Checked;
			txtPassword.Enabled = chkUseAuth.Checked;
		}

		private String getRequiredValue(TextBox ctrl, String errorMessage)
		{
			if (ctrl == null) return null;
			String result = ctrl.Text.Trim();
			if (result.Length == 0)
			{
				ctrl.Focus();
				throw new Exception(errorMessage);
			}
			return result;
		}

		private void btnSendEMail_Click(object sender, System.EventArgs e)
		{
			// Clear lists before sending
			msg.To.Clear();
			msg.Cc.Clear();
			msg.Bcc.Clear();
			try
			{
				msg.To.Add(getRequiredValue(txtTo, "Value of field 'To' is required"));

				String cc = txtCC.Text.Trim();
				if (cc.Length > 0) msg.Cc.Add(cc);

				String bcc = txtBCC.Text.Trim();
				if (bcc.Length > 0) msg.Bcc.Add(bcc);


				smtpClient.HostName = getRequiredValue(txtSMTPServer, "Host Name is required");
				smtpClient.HeloDomain = "remobjects.com";

				smtpClient.UseAuth = chkUseAuth.Checked;
				smtpClient.AuthUser = txtUserName.Text;
				smtpClient.AuthPassword = txtPassword.Text;

				msg.From.Name = getRequiredValue(txtSenderName, "From field value is required");
				msg.From.Address = getRequiredValue(txtSenderAddress, "EMail field value is required");
				msg.Subject = getRequiredValue(txtSubject, "Subject of letter is required");
				msg.Contents = getRequiredValue(txtMessage, "Content of letter is required");


				smtpClient.Open();
				smtpClient.SendMessage(msg);
				smtpClient.Close();
				MessageBox.Show("Email has been sent successfully!", "SMTP Client Sample");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error during sending letter.", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
