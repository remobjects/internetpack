using System;
using System.IO;
using System.Windows.Forms;
using RemObjects.InternetPack.Http;
using RemObjects.InternetPack.StandardServers;

namespace StandardServersTest
{
	public class MainForm : System.Windows.Forms.Form
	{
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

		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.GroupBox1 = new System.Windows.Forms.GroupBox();
			this.lblUrl = new System.Windows.Forms.Label();
			this.nudCount = new System.Windows.Forms.NumericUpDown();
			this.lblCount = new System.Windows.Forms.Label();
			this.lblServerName = new System.Windows.Forms.Label();
			this.txtServerName = new System.Windows.Forms.TextBox();
			this.txtRoot = new System.Windows.Forms.TextBox();
			this.lblRoot = new System.Windows.Forms.Label();
			this.nudPort = new System.Windows.Forms.NumericUpDown();
			this.lblPort = new System.Windows.Forms.Label();
			this.lblLink = new System.Windows.Forms.LinkLabel();
			this.btnAction = new System.Windows.Forms.Button();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.Label2 = new System.Windows.Forms.Label();
			this.GroupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCount)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPort)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// GroupBox1
			// 
			this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GroupBox1.Controls.Add(this.lblUrl);
			this.GroupBox1.Controls.Add(this.nudCount);
			this.GroupBox1.Controls.Add(this.lblCount);
			this.GroupBox1.Controls.Add(this.lblServerName);
			this.GroupBox1.Controls.Add(this.txtServerName);
			this.GroupBox1.Controls.Add(this.txtRoot);
			this.GroupBox1.Controls.Add(this.lblRoot);
			this.GroupBox1.Controls.Add(this.nudPort);
			this.GroupBox1.Controls.Add(this.lblPort);
			this.GroupBox1.Controls.Add(this.lblLink);
			this.GroupBox1.Controls.Add(this.btnAction);
			this.GroupBox1.Location = new System.Drawing.Point(9, 48);
			this.GroupBox1.Name = "GroupBox1";
			this.GroupBox1.Size = new System.Drawing.Size(526, 144);
			this.GroupBox1.TabIndex = 18;
			this.GroupBox1.TabStop = false;
			this.GroupBox1.Text = "HttpServer";
			// 
			// lblUrl
			// 
			this.lblUrl.AutoSize = true;
			this.lblUrl.Enabled = false;
			this.lblUrl.Location = new System.Drawing.Point(99, 120);
			this.lblUrl.Name = "lblUrl";
			this.lblUrl.Size = new System.Drawing.Size(32, 13);
			this.lblUrl.TabIndex = 8;
			this.lblUrl.Text = "URL:";
			// 
			// nudCount
			// 
			this.nudCount.Location = new System.Drawing.Point(139, 88);
			this.nudCount.Name = "nudCount";
			this.nudCount.Size = new System.Drawing.Size(48, 20);
			this.nudCount.TabIndex = 7;
			this.nudCount.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// lblCount
			// 
			this.lblCount.AutoSize = true;
			this.lblCount.Location = new System.Drawing.Point(16, 90);
			this.lblCount.Name = "lblCount";
			this.lblCount.Size = new System.Drawing.Size(115, 13);
			this.lblCount.TabIndex = 6;
			this.lblCount.Text = "Listener Thread Count:";
			// 
			// lblServerName
			// 
			this.lblServerName.AutoSize = true;
			this.lblServerName.Location = new System.Drawing.Point(59, 67);
			this.lblServerName.Name = "lblServerName";
			this.lblServerName.Size = new System.Drawing.Size(72, 13);
			this.lblServerName.TabIndex = 4;
			this.lblServerName.Text = "&Server Name:";
			// 
			// txtServerName
			// 
			this.txtServerName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtServerName.Location = new System.Drawing.Point(139, 64);
			this.txtServerName.Name = "txtServerName";
			this.txtServerName.Size = new System.Drawing.Size(374, 20);
			this.txtServerName.TabIndex = 5;
			// 
			// txtRoot
			// 
			this.txtRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtRoot.Location = new System.Drawing.Point(139, 40);
			this.txtRoot.Name = "txtRoot";
			this.txtRoot.Size = new System.Drawing.Size(374, 20);
			this.txtRoot.TabIndex = 3;
			// 
			// lblRoot
			// 
			this.lblRoot.AutoSize = true;
			this.lblRoot.Location = new System.Drawing.Point(76, 43);
			this.lblRoot.Name = "lblRoot";
			this.lblRoot.Size = new System.Drawing.Size(55, 13);
			this.lblRoot.TabIndex = 2;
			this.lblRoot.Text = "&RootPath:";
			// 
			// nudPort
			// 
			this.nudPort.Location = new System.Drawing.Point(139, 16);
			this.nudPort.Name = "nudPort";
			this.nudPort.Size = new System.Drawing.Size(48, 20);
			this.nudPort.TabIndex = 1;
			this.nudPort.Value = new decimal(new int[] {
            83,
            0,
            0,
            0});
			this.nudPort.ValueChanged += new System.EventHandler(this.nudPort_ValueChanged);
			// 
			// lblPort
			// 
			this.lblPort.AutoSize = true;
			this.lblPort.Location = new System.Drawing.Point(102, 18);
			this.lblPort.Name = "lblPort";
			this.lblPort.Size = new System.Drawing.Size(29, 13);
			this.lblPort.TabIndex = 0;
			this.lblPort.Text = "&Port:";
			// 
			// lblLink
			// 
			this.lblLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblLink.Enabled = false;
			this.lblLink.Location = new System.Drawing.Point(139, 120);
			this.lblLink.Name = "lblLink";
			this.lblLink.Size = new System.Drawing.Size(166, 16);
			this.lblLink.TabIndex = 9;
			this.lblLink.TabStop = true;
			this.lblLink.Text = "http://localhost:82/index.html";
			this.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblLink_LinkClicked);
			// 
			// btnAction
			// 
			this.btnAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAction.Location = new System.Drawing.Point(401, 115);
			this.btnAction.Name = "btnAction";
			this.btnAction.Size = new System.Drawing.Size(112, 23);
			this.btnAction.TabIndex = 10;
			this.btnAction.Text = "Activate Servers";
			this.btnAction.Click += new System.EventHandler(this.btnAction_Click);
			// 
			// txtLog
			// 
			this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtLog.Location = new System.Drawing.Point(9, 224);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtLog.Size = new System.Drawing.Size(526, 220);
			this.txtLog.TabIndex = 20;
			this.txtLog.WordWrap = false;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(9, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(120, 30);
			this.pictureBox1.TabIndex = 21;
			this.pictureBox1.TabStop = false;
			// 
			// Label2
			// 
			this.Label2.Location = new System.Drawing.Point(9, 200);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(32, 16);
			this.Label2.TabIndex = 19;
			this.Label2.Text = "Log";
			// 
			// MainForm
			// 
			this.AllowDrop = true;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 452);
			this.Controls.Add(this.GroupBox1);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.Label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(550, 480);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Internet Pack Sample Server";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.Closed += new System.EventHandler(this.MainForm_Closed);
			this.GroupBox1.ResumeLayout(false);
			this.GroupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.nudCount)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudPort)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
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

		private EchoServer fEchoServer;
		internal System.Windows.Forms.GroupBox GroupBox1;
		internal System.Windows.Forms.Label lblUrl;
		internal System.Windows.Forms.NumericUpDown nudCount;
		internal System.Windows.Forms.Label lblCount;
		internal System.Windows.Forms.Label lblServerName;
		internal System.Windows.Forms.TextBox txtServerName;
		internal System.Windows.Forms.TextBox txtRoot;
		internal System.Windows.Forms.Label lblRoot;
		internal System.Windows.Forms.NumericUpDown nudPort;
		internal System.Windows.Forms.Label lblPort;
		internal System.Windows.Forms.LinkLabel lblLink;
		internal System.Windows.Forms.Button btnAction;
		internal System.Windows.Forms.TextBox txtLog;
		internal System.Windows.Forms.PictureBox pictureBox1;
		internal System.Windows.Forms.Label Label2;
		private SimpleHttpServer fHttpServer;


		public void OnHttpRequest(object aSender, HttpRequestEventArgs ea)
		{
			AddLog(String.Format("Request to {0}", ea.Request.Header.RequestPath));
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			txtRoot.Text = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "HttpRoot");
			txtServerName.Text = "Internet Pack HTTP Server";
			nudPort.Value = 82;
			nudCount.Value = 5;
		}

		delegate void TextCallback(string text);
		private void AddLog(String line)
		{
			if (this.txtLog.InvokeRequired)
			{
				TextCallback d = new TextCallback(AddLog);
				this.Invoke(d, new object[] { 
                    Environment.NewLine + String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), line) });
			}
			else
			{
				txtLog.AppendText(Environment.NewLine + String.Format("{0}: {1}", DateTime.Now.ToLongTimeString(), line));
			}
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
			DeactivateServers();
		}

		private void nudPort_ValueChanged(object sender, System.EventArgs e)
		{
			lblLink.Text = String.Format("http://localhost:{0}/index.html", nudPort.Value);
		}

		private void lblLink_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			if (File.Exists(fHttpServer.RootPath + "\\index.html"))
				System.Diagnostics.Process.Start(lblLink.Text);
			else
				MessageBox.Show(fHttpServer.RootPath + "\\index.html can not be opened, because it does not exists.", "Warning",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		private void ActivateServers()
		{
			AddLog("Trying to activate servers...");
			fEchoServer = new EchoServer();
			try
			{
				fEchoServer.Open();
				AddLog("EchoServer is active.");
			}
			catch (Exception ex)
			{
				AddLog("Can't activate EchoServer. An exception occured: " + ex.Message);
			}

			fHttpServer = new SimpleHttpServer();
			fHttpServer.Port = Convert.ToInt32(nudPort.Value);
			fHttpServer.RootPath = txtRoot.Text;
			fHttpServer.ServerName = txtServerName.Text;
			if (this.fHttpServer.BindingV4 != null)
				this.fHttpServer.BindingV4.ListenerThreadCount = Convert.ToInt32(this.nudCount.Value);
			fHttpServer.HttpRequest += OnHttpRequest;
			fHttpServer.Open();
			AddLog(String.Format("SimpleHttpServer is active on {0} port.", fHttpServer.Port));
			SetEnable(false);
			AddLog("Servers activated.");
			btnAction.Text = "Deactivate Servers";
		}

		private void DeactivateServers()
		{
			AddLog("Trying to deactivate servers...");
			if (fEchoServer != null) fEchoServer.Close();
			AddLog("EchoServer is closed.");
			if (fHttpServer != null) fHttpServer.Close();
			AddLog("HttpServer is closed.");
			SetEnable(true);
			AddLog("Servers is deactivated");
			btnAction.Text = "Activate Servers";
		}

		private void SetEnable(Boolean mode)
		{
			lblPort.Enabled = mode;
			nudPort.Enabled = mode;
			lblServerName.Enabled = mode;
			txtServerName.Enabled = mode;
			lblRoot.Enabled = mode;
			txtRoot.Enabled = mode;
			lblUrl.Enabled = !mode;
			lblLink.Enabled = !mode;
		}

		private void btnAction_Click(object sender, System.EventArgs e)
		{
			if (btnAction.Text == "Activate Servers")
				ActivateServers();
			else
				DeactivateServers();
		}
	}
}
