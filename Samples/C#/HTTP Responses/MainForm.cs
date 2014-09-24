using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace HttpResponses
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private RemObjects.InternetPack.Http.HttpServer httpServer;
		private System.Windows.Forms.LinkLabel llblinkLabel1;
		private System.Windows.Forms.ListBox lb_Log;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.httpServer = new RemObjects.InternetPack.Http.HttpServer(this.components);
			this.llblinkLabel1 = new System.Windows.Forms.LinkLabel();
			this.lb_Log = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// httpServer
			// 
			this.httpServer.Port = 81;
			this.httpServer.ValidateRequests = false;
			this.httpServer.HttpRequest += new RemObjects.InternetPack.Http.HttpRequestEventHandler(this.httpServer_OnHttpRequest);
			// 
			// llblinkLabel1
			// 
			this.llblinkLabel1.Location = new System.Drawing.Point(8, 8);
			this.llblinkLabel1.Name = "llblinkLabel1";
			this.llblinkLabel1.Size = new System.Drawing.Size(100, 23);
			this.llblinkLabel1.TabIndex = 0;
			this.llblinkLabel1.TabStop = true;
			this.llblinkLabel1.Text = "http://localhost:81";
			this.llblinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llblinkLabel1_LinkClicked);
			// 
			// lb_Log
			// 
			this.lb_Log.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lb_Log.IntegralHeight = false;
			this.lb_Log.Location = new System.Drawing.Point(11, 32);
			this.lb_Log.Name = "lb_Log";
			this.lb_Log.Size = new System.Drawing.Size(371, 228);
			this.lb_Log.TabIndex = 1;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(394, 272);
			this.Controls.Add(this.lb_Log);
			this.Controls.Add(this.llblinkLabel1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(410, 310);
			this.Name = "MainForm";
			this.Text = "Internet Pack HTTP Response Sample";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Closed += new System.EventHandler(this.Form1_Closed);
			this.ResumeLayout(false);

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

		const string sWelcome =
		  "Internet Pack HTTP Responses Test App" +
		  "<br /><br />" +
		  "Valid links:" +
		  "<br />" +
		  "<a href=/home>/home</a> show this page</a>" +
		  "<br />" +
		  "<a href=/file>/file</a> send back a file (this .exe)" +
		  "<br />" +
		  "<a href=/bytes>/bytes</a> send back a buffer of random bytes" +
		  "<br />" +
		  "<a href=/error>/error</a> Display a custom error";

		private void httpServer_OnHttpRequest(Object sender, RemObjects.InternetPack.Http.HttpRequestEventArgs ea)
		{
			String lRequestPath = ea.Request.Header.RequestPath;

			Invoke(new MethodInvoker(delegate()
			{
				lb_Log.Items.Add(lRequestPath);
			}));


			switch (ea.Request.Header.RequestPath)
			{
				case "/":
				case "/home":
					ea.Response.ContentString = sWelcome;
					ea.Response.Header.SetHeaderValue("Content-Type", "text/html");
					break;

				case "/bytes":
					byte[] lBuffer = new byte[256];
					Random lRandom = new Random();
					lRandom.NextBytes(lBuffer);
					ea.Response.ContentBytes = lBuffer;
					ea.Response.Header.SetHeaderValue("Content-Disposition", "filename=random.bin");
					ea.Response.Header.SetHeaderValue("Content-Type", "application/binary");
					break;

				case "/error":
					ea.Response.SendError(555, "Custom Error");
					break;

				case "/file":
					String lExeName = this.GetType().Assembly.Location;
					try
					{
						ea.Response.ContentStream = new FileStream(lExeName,
						  FileMode.Open, FileAccess.Read, FileShare.Read);
						ea.Response.Header.SetHeaderValue("Content-Disposition",
						  String.Format("filename=\"{0}\"", Path.GetFileName(lExeName)));
						ea.Response.Header.SetHeaderValue("Content-Type", "application/binary");
						ea.Response.CloseStream = true; /* default, anyway */
					}
					catch (Exception)
					{
						ea.Response.SendError(System.Net.HttpStatusCode.NotFound, String.Format("File {0} not found", lExeName));
					}
					break;

				default:
					ea.Response.SendError(System.Net.HttpStatusCode.NotFound, "Requested path not found");
					break;
			}

		}

		private void llblinkLabel1_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(llblinkLabel1.Text);
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
			httpServer.Active = true;
		}

		private void Form1_Closed(object sender, System.EventArgs e)
		{
			httpServer.Active = false;
		}
	}
}