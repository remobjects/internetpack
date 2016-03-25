using System;
using System.IO;
using System.Windows.Forms;
using RemObjects.InternetPack.Ftp.VirtualFtp;

namespace VirtualFtp
{
	public class MainForm : System.Windows.Forms.Form
	{
		const int port = 4444;
		private System.Windows.Forms.Button btnStartStop;
		private System.Windows.Forms.GroupBox GroupBox1;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.LinkLabel llShortcut;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.PictureBox pictureBox1;

		private VirtualFolder fRootFolder;
		private IFtpUserManager fUserManager;
		private VirtualFtpServer fFtpServer;

		#region Boilerplate stuff
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			InitializeComponent();
		}

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

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.btnStartStop = new System.Windows.Forms.Button();
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.llShortcut = new System.Windows.Forms.LinkLabel();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.GroupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// btnStartStop
			// 
			this.btnStartStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStartStop.Location = new System.Drawing.Point(231, 8);
			this.btnStartStop.Name = "btnStartStop";
			this.btnStartStop.Size = new System.Drawing.Size(75, 23);
			this.btnStartStop.TabIndex = 18;
			this.btnStartStop.Text = "Start";
			this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
			// 
			// GroupBox1
			// 
			this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GroupBox1.Controls.Add(this.txtLog);
			this.GroupBox1.Location = new System.Drawing.Point(8, 103);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(298, 132);
			this.GroupBox1.TabIndex = 17;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "Log";
			// 
			// txtLog
			// 
			this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLog.Location = new System.Drawing.Point(8, 16);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(282, 108);
			this.txtLog.TabIndex = 6;
			this.txtLog.WordWrap = false;
			// 
			// llShortcut
			// 
			this.llShortcut.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.llShortcut.Location = new System.Drawing.Point(4, 62);
			this.llShortcut.Name = "llShortcut";
			this.llShortcut.Size = new System.Drawing.Size(300, 33);
			this.llShortcut.TabIndex = 15;
			this.llShortcut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.llShortcut.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llShortcut_LinkClicked);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(3, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(309, 24);
			this.label1.TabIndex = 16;
			this.label1.Text = "In order to login on ftp please use login: test; password: test.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 7);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(120, 30);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 14;
			this.pictureBox1.TabStop = false;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(314, 242);
			this.Controls.Add(this.btnStartStop);
			this.Controls.Add(this.GroupBox1);
			this.Controls.Add(this.llShortcut);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(330, 280);
			this.Name = "MainForm";
			this.Tag = "";
			this.Text = "Virtual FTP";
			this.Closed += new System.EventHandler(this.MainForm_Closed);
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}
		#endregion

		private void btnStartStop_Click(object sender, System.EventArgs e)
		{
			if (btnStartStop.Text == "Start")
			{
				addToLog("Starting Virtual FTP on " + port.ToString() + " port...");
				StartServer();
				llShortcut.Text = String.Format("ftp://127.0.0.1:{0}/", port);
				addToLog("Virtual FTP is running under " + Environment.OSVersion);
				btnStartStop.Text = "Stop";
			}
			else
			{
				addToLog("Shutting down Virtual FTP ...");
				StopServer();
				llShortcut.Text = "";
				addToLog("Virtual FTP is stopped.");
				btnStartStop.Text = "Start";
			}
		}

		private void llShortcut_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			if (llShortcut.Text != "") System.Diagnostics.Process.Start(llShortcut.Text);
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
			StopServer();
		}

		private void StartServer()
		{
			string lDiskFolder = Path.GetDirectoryName(typeof(MainForm).Assembly.Location) + @"\FtpRoot";

			fRootFolder = new VirtualFolder(null, "[ROOT]");
			fRootFolder.Add(new VirtualFolder(null, "virtual"));
			fRootFolder.Add(new DiscFolder(null, "drive-c", @"c:\"));
			fRootFolder.Add(new DiscFolder(null, "disc", lDiskFolder));
			fRootFolder.Add(new EmptyFile(null, "=== Welcome to the FTP ==="));

			fUserManager = new UserManager();
			((UserManager)fUserManager).AddUser("test", "test");

			fFtpServer = new VirtualFtpServer();
			fFtpServer.Port = port;
			fFtpServer.Timeout = 60 * 1000; /* 1 minute */
			if (this.fFtpServer.BindingV4 != null)
				this.fFtpServer.BindingV4.ListenerThreadCount = 10;
			fFtpServer.RootFolder = fRootFolder;
			fFtpServer.UserManager = fUserManager;
			fFtpServer.ServerName = "VirtualFTP Sample - powered by RemObjects Internet Pack for .NET";

			fFtpServer.Open();

			addToLog("VirtualFTP 0.3 BETA - started up");
		}

		private void StopServer()
		{
			if (fFtpServer != null)
				fFtpServer.Close();
		}

		private void addToLog(String line)
		{
			txtLog.Text = txtLog.Text +
				System.DateTime.Now.ToLongTimeString() +
				": " +
				line +
				Environment.NewLine;
		}
	}
}