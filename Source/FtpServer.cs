/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

#if toffee || cooper
using RemObjects.Elements.RTL.Reflection;
#endif
using RemObjects.InternetPack.CommandBased;

namespace RemObjects.InternetPack.Ftp
{
	// ftp://ftp.rfc-editor.org/in-notes/rfc959.txt
	#region Event Types
	public delegate void OnFtpUserLoginHandler(Object sender, FtpUserLoginEventArgs e);

	public delegate void OnFtpUserAccountHandler(Object sender, FtpUserAccountArgs e);

	public delegate void OnFtpChangeDirectoryHandler(Object sender, FtpChangeDirectoryArgs e);

	public delegate void OnFtpGetListingHandler(Object sender, FtpGetListingArgs e);

	public delegate void OnFtpRenameHandler(Object sender, FtpRenameEventArgs e);

	public delegate void OnFtpFileHandler(Object sender, FtpFileEventArgs e);

	public delegate void OnFtpTransferHandler(Object sender, FtpTransferEventArgs e);

	public class FtpUserLoginEventArgs : SessionEventArgs
	{
		public FtpUserLoginEventArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
			this.LoginOk = false;
			this.NeedAccount = false;
		}

		public String UserName { get; set; }

		public String Password { get; set; }

		public Boolean LoginOk { get; set; }

		public Boolean NeedAccount { get; set; }
	}

	public class FtpUserAccountArgs : SessionEventArgs
	{
		public FtpUserAccountArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
		}

		public String AccountName { get; set; }

		public Boolean LoginOk { get; set; }
	}

	public class FtpChangeDirectoryArgs : SessionEventArgs
	{
		public FtpChangeDirectoryArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
		}

		public String NewDirectory { get; set; }

		public Boolean ChangeDirOk { get; set; }
	}

	public class FtpGetListingArgs : SessionEventArgs
	{
		public FtpGetListingArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
			this.fListing = new FtpListing();
		}

		public FtpListing Listing
		{
			get
			{
				return this.fListing;
			}
		}
		private readonly FtpListing fListing;
	}

	public class FtpFileEventArgs : SessionEventArgs
	{
		public FtpFileEventArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
		}

		public String FileName { get; set; }

		public Boolean Ok { get; set; }
	}

	public class FtpRenameEventArgs : FtpFileEventArgs
	{
		public FtpRenameEventArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
		}

		public String NewFileName { get; set; }
	}

	public class FtpTransferEventArgs : FtpFileEventArgs
	{
		public FtpTransferEventArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
		}

		public Boolean Append { get; set; }

		public Int64 RestartPoint { get; set; }

		public Stream DataChannel { get; set; }
	}
	#endregion

#if DESIGN
	[System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.FtpServer.bmp")]
#endif
	public class FtpServer : CommandBasedServer
	{
		public FtpServer()
		{
			this.Port = 21;
			this.ServerName = "FTP Server Ready";
			this.QuitReply = "221 Bye";
			this.SystemType = "UNIX";
			this.AsciiEnter = "\r\n";
			this.Detailed500Errors = true;
		}

		#region Properties
		[Category("Server")]
		[DefaultValue("211 Bye")]
		public String QuitReply { get; set; }

		[Category("Server")]
		[DefaultValue("UNIX")]
		public String SystemType { get; set; }

		public String AsciiEnter { get; set; }

		[Category("Server")]
		public String ServerName
		{
			get
			{
				return this.fServerName;
			}
			set
			{
				this.fServerName = value;
				this.Greeting = "220 " + value;
			}
		}
		private String fServerName;

		[Category("Server")]
		[DefaultValue(true)]
		public Boolean Detailed500Errors { get; set; }
		#endregion

		#region Events
		public event OnFtpUserLoginHandler OnUserLogin;
		public event OnFtpUserAccountHandler OnAccount;
		public event OnFtpChangeDirectoryHandler OnChangeDirectory;
		public event OnFtpGetListingHandler OnGetListing;
		public event OnFtpRenameHandler OnRename;
		public event OnFtpFileHandler OnDelete;
		public event OnFtpFileHandler OnMakeDirectory;
		public event OnFtpFileHandler OnDeleteDirectory;
		public event OnFtpTransferHandler OnStoreFile;
		public event OnFtpTransferHandler OnCanStoreFile;
		public event OnFtpTransferHandler OnRetrieveFile;
		public event OnFtpTransferHandler OnCanRetrieveFile;

		internal protected virtual void InvokeOnUserLogin(FtpUserLoginEventArgs e)
		{
			if (this.OnUserLogin != null)
				this.OnUserLogin(this, e);
		}

		internal protected virtual void InvokeOnAccount(FtpUserAccountArgs e)
		{
			if (this.OnAccount != null)
				this.OnAccount(this, e);
		}

		internal protected virtual void InvokeOnChangeDirectory(FtpChangeDirectoryArgs e)
		{
			if (this.OnChangeDirectory != null)
				this.OnChangeDirectory(this, e);
		}

		internal protected virtual void InvokeOnGetListing(FtpGetListingArgs e)
		{
			if (this.OnGetListing != null)
				this.OnGetListing(this, e);
		}

		internal protected virtual void InvokeOnRename(FtpRenameEventArgs e)
		{
			if (this.OnRename != null)
				this.OnRename(this, e);
		}

		internal protected virtual void InvokeOnDelete(FtpFileEventArgs e)
		{
			if (this.OnDelete != null)
				this.OnDelete(this, e);
		}

		internal protected virtual void InvokeOnMakeDirectory(FtpFileEventArgs e)
		{
			if (this.OnMakeDirectory != null)
				this.OnMakeDirectory(this, e);
		}

		internal protected virtual void InvokeOnDeleteDirectory(FtpFileEventArgs e)
		{
			if (this.OnDeleteDirectory != null)
				this.OnDeleteDirectory(this, e);
		}

		internal protected virtual void InvokeOnStoreFile(FtpTransferEventArgs e)
		{
			if (this.OnStoreFile != null)
				this.OnStoreFile(this, e);
		}

		internal protected virtual void InvokeOnRetrieveFile(FtpTransferEventArgs e)
		{
			if (this.OnRetrieveFile != null)
				this.OnRetrieveFile(this, e);
		}

		internal protected virtual void InvokeOnCanStoreFile(FtpTransferEventArgs e)
		{
			if (this.OnCanStoreFile != null)
				this.OnCanStoreFile(this, e);
		}

		internal protected virtual void InvokeOnCanRetrieveFile(FtpTransferEventArgs e)
		{
			if (this.OnCanRetrieveFile != null)
				this.OnCanRetrieveFile(this, e);
		}

		protected internal override void InvokeOnClientDisconnected(SessionEventArgs e)
		{
			base.InvokeOnClientDisconnected(e);

			FtpSession lSession = (FtpSession)e.Session;
			if (lSession.ActiveConnection != null)
			{
				try
				{
					lSession.ActiveConnection.Close();
				}
				catch (Exception)
				{
					// As designed
				}
				lSession.ActiveConnection = null;
			}

			if (lSession.PassiveServer != null)
			{
				try
				{
					lSession.PassiveServer.Close();
				}
				catch (Exception)
				{
					// As designed
				}
				lSession.PassiveServer = null;
			}

			if (lSession.TransferThread != null)
			{
				try
				{
					lSession.TransferThread.Abort();
				}
				catch (Exception)
				{
					// As designed
				}
				lSession.TransferThread = null;
			}
		}
		#endregion

		public static Boolean ValidFilename(String value)
		{
			return value.IndexOfAny(new Char[] { '\\', '|', '>', '<', '*', '?', ':', '"' }) == -1;
		}

		public static String ValidateDirectory(String value)
		{
			StringBuilder lResult = new StringBuilder();
			lResult.Append('/');

			var lTemp = value.Split("/").MutableVersion();
			for (Int32 i = lTemp.Count - 1; i >= 0; i--)
			{
				if (lTemp[i] == ".")
				{
					lTemp[i] = "";

					continue;
				}

				if (lTemp[i] == "..")
				{
					lTemp[i] = "";

					if (i > 0)
						lTemp[i - 1] = "";

					continue;
				}

				if (lTemp[i].IndexOfAny(new Char[] { '\\', '|', '>', '<', '*', '?', ':', '"' }) != -1)
				{
					if (i > 0)
						lTemp[i - 1] = "";
				}
			}

			for (Int32 i = 0; i < lTemp.Count; i++)
			{
				if (lTemp[i].Length == 0)
					continue;

				lResult.Append(lTemp[i]);
				lResult.Append('/');
			}

			if (lResult.Length == 1)
				return lResult.ToString();

			String lTemp2 = lResult.ToString();
			return lTemp2.Substring(0, lTemp2.Length - 1); // remove the last /
		}

		protected override Type GetDefaultSessionClass()
		{
			return typeof(FtpSession);
		}

		protected override void InitCommands()
		{
			this.Commands.Add("QUIT", Cmd_QUIT);
			this.Commands.Add("NOOP", Cmd_NOOP);
			this.Commands.Add("USER", Cmd_USER);
			this.Commands.Add("ALLO", Cmd_NOOP);
			this.Commands.Add("PASS", Cmd_PASS);
			this.Commands.Add("ACCT", Cmd_ACCT);
			this.Commands.Add("CWD", Cmd_CWD);
			this.Commands.Add("CDUP", Cmd_CDUP);
			this.Commands.Add("PWD", Cmd_PWD);
			this.Commands.Add("SYST", Cmd_SYST);
			this.Commands.Add("TYPE", Cmd_TYPE);
			this.Commands.Add("PORT", Cmd_PORT);
			this.Commands.Add("REST", Cmd_REST);
			this.Commands.Add("LIST", Cmd_LIST);
			this.Commands.Add("PASV", Cmd_PASV);
			this.Commands.Add("RNFR", Cmd_RNFR);
			this.Commands.Add("RNTO", Cmd_RNTO);
			this.Commands.Add("DELE", Cmd_DELE);
			this.Commands.Add("RMD", Cmd_RMD);
			this.Commands.Add("MKD", Cmd_MKD);
			this.Commands.Add("STOR", Cmd_STOR);
			this.Commands.Add("APPE", Cmd_APPE);
			this.Commands.Add("RETR", Cmd_RETR);
			this.Commands.Add("ABOR", Cmd_ABOR);

			/* Aliases */
			this.Commands.Add("CD", Cmd_CWD);
		}

		/*
			STRU <SP> <structure-code> <CRLF> // only support file
			MODE <SP> <mode-code> <CRLF> // stream, maybe block if needed

			NLST [<SP> <pathname>] <CRLF> nlist
			SITE <SP> <String> <CRLF> site specific commands
			STAT [<SP> <pathname>] <CRLF> status
		*/

		private static Boolean ValidateConnection(FtpSession session)
		{
			if (!session.Passive)
				return (session.ActiveConnection != null && session.ActiveConnection.Connected);

			if (session.PassiveServer == null)
				return false;

			try
			{
				Connection lConnection = session.PassiveServer.WaitForConnection();
				session.PassiveServer.Close();
				session.PassiveServer = null;
				session.ActiveConnection = lConnection;

				return true;
			}
			catch
			{
				session.PassiveServer = null;
				session.ActiveConnection = null;

				return false;
			}
		}

		public static void Cmd_NOOP(Object sender, CommandEventArgs e)
		{
			e.Connection.WriteLine("200 OK");
		}

		public static void Cmd_RNFR(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				String lNewDir = e.AllParameters;

				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}
				lSession.RenameFrom = lNewDir;
				e.Connection.WriteLine("350 Ready for destination name.");
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_RNTO(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				if (lSession.RenameFrom == null)
				{
					e.Connection.WriteLine("500 Bad sequence of commands.");
					return;
				}

				String lNewDir = e.AllParameters;

				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpRenameEventArgs lEventArgs = new FtpRenameEventArgs(e.Session, e.Connection, e.Server);
				lEventArgs.FileName = lSession.RenameFrom;
				lSession.RenameFrom = null;
				lEventArgs.NewFileName = lNewDir;
				try
				{
					((FtpServer)e.Server).InvokeOnRename(lEventArgs);
				}
				catch (FtpException ex)
				{
					e.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					e.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lEventArgs.Ok)
					e.Connection.WriteLine("250 Rename successful");
				else
					e.Connection.WriteLine("550 Permission Denied");
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_ABOR(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.Transfer && lSession.TransferThread != null)
			{
				lSession.TransferThread.Abort();
				lSession.TransferThread = null;
				e.Connection.WriteLine("250 ABOR successful");
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_DELE(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}
				String lNewDir = e.AllParameters;

				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpFileEventArgs lEventArgs = new FtpFileEventArgs(e.Session, e.Connection, e.Server);
				lEventArgs.FileName = lNewDir;
				try
				{
					((FtpServer)e.Server).InvokeOnDelete(lEventArgs);
				}
				catch (FtpException ex)
				{
					e.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					e.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lEventArgs.Ok)
					e.Connection.WriteLine("250 Delete successful");
				else
					e.Connection.WriteLine("550 Permission Denied");
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_RMD(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				String lNewDir = e.AllParameters;
				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpFileEventArgs lEventArgs = new FtpFileEventArgs(e.Session, e.Connection, e.Server);
				lEventArgs.FileName = lNewDir;
				try
				{
					((FtpServer)e.Server).InvokeOnDeleteDirectory(lEventArgs);
				}
				catch (FtpException ex)
				{
					e.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					e.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lEventArgs.Ok)
					e.Connection.WriteLine("250 RMD command succesfully");
				else
					e.Connection.WriteLine("550 Permission Denied");
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_MKD(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				String lNewDir = e.AllParameters;
				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpFileEventArgs lEventArgs = new FtpFileEventArgs(e.Session, e.Connection, e.Server);
				lEventArgs.FileName = lNewDir;
				try
				{
					((FtpServer)e.Server).InvokeOnMakeDirectory(lEventArgs);
				}
				catch (FtpException ex)
				{
					e.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					e.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lEventArgs.Ok)
					e.Connection.WriteLine(String.Format("257 \"{0}\" - Directory successfully created", lEventArgs.FileName));
				else
					e.Connection.WriteLine("550 Permission Denied");
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_STOR(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				String lNewDir = e.AllParameters;
				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpTransferEventArgs lEventArgs = new FtpTransferEventArgs(e.Session, e.Connection, e.Server);
				lEventArgs.FileName = lNewDir;
				lEventArgs.Append = false;
				lEventArgs.RestartPoint = 0;
				lSession.RestartPoint = 0;

				try
				{
					((FtpServer)lEventArgs.Server).InvokeOnCanStoreFile(lEventArgs);
				}
				catch (FtpException ex)
				{
					lEventArgs.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					lEventArgs.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lEventArgs.Ok)
				{
					if (lSession.Passive)
						e.Connection.WriteLine("125 Data connection already open; transfer starting");

					if (!ValidateConnection(lSession))
					{
						e.Connection.WriteLine("425 Unable to build data connection");
						return;
					}

					if (!lSession.Passive)
						e.Connection.WriteLine("150 Opening data connection");
				}
				else
				{
					lEventArgs.Connection.WriteLine("550 Permission Denied");
					return;
				}
				lSession.State = FtpState.Transfer;

				if (lSession.TransferThread != null)
				{
					lSession.TransferThread.Abort();
					lSession.TransferThread = null;
				}
				lSession.TransferThread = new FtpTransferThread(lEventArgs, true);
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_APPE(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0) || (!ValidFilename(e.AllParameters)))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				String lNewDir = e.AllParameters;
				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpTransferEventArgs lArgs = new FtpTransferEventArgs(e.Session, e.Connection, e.Server);
				lArgs.FileName = lNewDir;
				lArgs.Append = true;
				lArgs.RestartPoint = 0;
				lSession.RestartPoint = 0;

				try
				{
					((FtpServer)lArgs.Server).InvokeOnCanStoreFile(lArgs);
				}
				catch (FtpException ex)
				{
					lArgs.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					lArgs.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lArgs.Ok)
				{
					if (lSession.Passive)
						e.Connection.WriteLine("125 Data connection already open; transfer starting");

					if (!ValidateConnection(lSession))
					{
						e.Connection.WriteLine("425 Unable to build data connection");
						return;
					}
					if (!lSession.Passive)
						e.Connection.WriteLine("150 Opening data connection");
				}
				else
				{
					lArgs.Connection.WriteLine("550 Permission Denied");
					return;
				}
				lSession.State = FtpState.Transfer;

				if (lSession.TransferThread != null)
				{
					lSession.TransferThread.Abort();
					lSession.TransferThread = null;
				}
				lSession.TransferThread = new FtpTransferThread(lArgs, true);
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_RETR(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if ((e.AllParameters.Length == 0))
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					return;
				}

				String lNewDir = e.AllParameters;
				if (lNewDir.StartsWith("/"))
				{
					lNewDir = ValidateDirectory(lNewDir);
				}
				else
				{
					String lTemp = lSession.Directory;
					if (!lTemp.EndsWith("/"))
						lTemp = lTemp + "/";
					lNewDir = ValidateDirectory(lTemp + lNewDir);
				}

				FtpTransferEventArgs lEventArgs = new FtpTransferEventArgs(e.Session, e.Connection, e.Server);
				lEventArgs.FileName = lNewDir;
				lEventArgs.Append = false;
				lEventArgs.RestartPoint = lSession.RestartPoint;
				lEventArgs.DataChannel = lSession.ActiveConnection;
				lSession.RestartPoint = 0;

				try
				{
					((FtpServer)lEventArgs.Server).InvokeOnCanRetrieveFile(lEventArgs);
				}
				catch (FtpException ex)
				{
					lEventArgs.Connection.WriteLine(ex.ToString());
					return;
				}
				catch
				{
					lEventArgs.Connection.WriteLine("500 Internal Error");
					return;
				}

				if (lEventArgs.Ok)
				{
					if (lSession.Passive)
						e.Connection.WriteLine("125 Data connection already open; transfer starting");

					if (!ValidateConnection(lSession))
					{
						e.Connection.WriteLine("425 Unable to build data connection");
						return;
					}

					if (!lSession.Passive)
						e.Connection.WriteLine("150 Opening data connection");

					lSession.State = FtpState.Transfer;
					if (lSession.TransferThread != null)
					{
						lSession.TransferThread.Abort();
						lSession.TransferThread = null;
					}
					lSession.TransferThread = new FtpTransferThread(lEventArgs, false);
				}
				else
				{
					lEventArgs.Connection.WriteLine("550 Permission Denied");
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_PASV(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (lSession.ActiveConnection != null)
				{
					if (lSession.ActiveConnection.Connected)
						lSession.ActiveConnection.Close();

					lSession.ActiveConnection = null;
				}

				if (lSession.PassiveServer == null)
				{
					lSession.PassiveServer = new SimpleServer();

					lSession.PassiveServer.Binding.Address = ((IPEndPoint)e.Connection.LocalEndPoint).Address;
					lSession.PassiveServer.Open();
				}
				lSession.Passive = true;

				Byte[] lAddress;
#if FULLFRAMEWORK
				lAddress = ((IPEndPoint)lSession.PassiveServer.Binding.ListeningSocket.LocalEndPoint).Address.GetAddressBytes();
#endif
#if COMPACTFRAMEWORK
				IPAddress lIPAddress = ((IPEndPoint)lSession.PassiveServer.Binding.ListeningSocket.LocalEndPoint).Address;
				String[] lIPAddressstr = lIPAddress.ToString().Split(new Char[] {'.'});
				lAddress = new Byte[lIPAddressstr.Length];
				for (Int32 i = 0; i < lIPAddressstr.Length; i++)
					lAddress[i] = Byte.Parse(lIPAddressstr[i]);
#endif

				Int32 lPort = ((IPEndPoint)lSession.PassiveServer.Binding.ListeningSocket.LocalEndPoint).Port;
				e.Connection.WriteLine("227 Entering Passive Mode ({0},{1},{2},{3},{4},{5}).", lAddress[0], lAddress[1], lAddress[2], lAddress[3],
				//#if echoes
					//unchecked((Byte)(lPort >> 8)), unchecked((Byte)lPort));
				//#else
					(Byte)(lPort >> 8), (Byte)lPort);
				//#endif
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_LIST(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{

				if (lSession.Passive)
					e.Connection.WriteLine("125 Data connection already open; transfer starting");

				if (!ValidateConnection(lSession))
				{
					e.Connection.WriteLine("425 Unable to build data connection");
				}
				else
				{
					FtpGetListingArgs lListingArgs = new FtpGetListingArgs(e.Session, e.Connection, e.Server);
					if (!lSession.Passive)
						e.Connection.WriteLine("150 Opening data connection");
					try
					{
						((FtpServer)e.Server).InvokeOnGetListing(lListingArgs);
					}
					catch (FtpException ex)
					{
						e.Connection.WriteLine(ex.ToString());
						return;
					}

					for (Int32 i = 0; i < lListingArgs.Listing.Count; i++)
					{
						FtpListingItem lItem = lListingArgs.Listing[i];
						lSession.ActiveConnection.WriteLine(lItem.ToString());
					}

					lSession.ActiveConnection.Close();
					lSession.ActiveConnection = null;

					try
					{
						e.Connection.WriteLine("226 Transfer complete.");
					}
					catch
					{
						e.Connection.WriteLine("425 Error while transfering");
					}
				}

				lSession.RestartPoint = 0;
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_TYPE(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (e.Parameters.Length != 1)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					switch (e.Parameters[0])
					{
						case "I":
							e.Connection.WriteLine("200 Type set to I");
							lSession.Image = true;
							break;

						case "A":
							e.Connection.WriteLine("200 Type set to A");
							lSession.Image = false;
							break;

						default:
							e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
							break;
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_REST(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (e.Parameters.Length != 1)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{

					try
					{
						Int64 lRestPoint = Convert.ToInt64(e.Parameters[0]);
						lSession.RestartPoint = lRestPoint;
						e.Connection.WriteLine(String.Format("350 Restarting at {0}. Send STORE or RETRIEVE to initiate transfer.", lRestPoint));
					}
					catch
					{
						e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_PORT(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (e.Parameters.Length != 1)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					var lNewPort = e.Parameters[0].Split(",");

					if (lNewPort.Count != 6)
					{
						e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
					}
					else
					{
						IPAddress lNewIp;
						Int32 lNewPortw;
						try
						{
							lNewIp = IPAddress.Parse(String.Format("{0}.{1}.{2}.{3}", lNewPort[0], lNewPort[1], lNewPort[2], lNewPort[3]));
							lNewPortw = Convert.ToByte(lNewPort[4]) << 8 | Convert.ToByte(lNewPort[5]);

							lSession.Passive = false;
							try
							{
								lSession.ActiveConnection = Client.Connect(lNewIp, lNewPortw, new Binding());
								e.Connection.WriteLine("200 PORT command succesful");
							}
							catch
							{
								e.Connection.WriteLine("500 Illegal PORT command ");
							}
						}
						catch
						{
							e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
						}
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_QUIT(Object sender, CommandEventArgs e)
		{
			e.Connection.WriteLine(((FtpServer)e.Server).QuitReply);
			e.Connection.Close();
		}

		public static void Cmd_PWD(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (e.Parameters.Length != 0)
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				else
					e.Connection.WriteLine(String.Format("257 \"{0}\" is current directory.", lSession.Directory));
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_CWD(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (e.Parameters.Length == 0)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					String lNewDir = e.AllParameters;
					if (lNewDir.StartsWith("/"))
					{
						lNewDir = ValidateDirectory(lNewDir);
					}
					else
					{
						String lTemp = lSession.Directory;
						if (!lTemp.EndsWith("/"))
							lTemp = lTemp + "/";
						lNewDir = ValidateDirectory(lTemp + lNewDir);
					}

					FtpChangeDirectoryArgs lArgs = new FtpChangeDirectoryArgs(e.Session, e.Connection, e.Server);
					lArgs.NewDirectory = lNewDir;

					try
					{
						((FtpServer)e.Server).InvokeOnChangeDirectory(lArgs);
					}
					catch (FtpException ex)
					{
						e.Connection.WriteLine(ex.ToString());
						return;
					}
					catch
					{
						e.Connection.WriteLine("500 Internal Error");
						return;
					}

					if (lArgs.ChangeDirOk)
					{
						e.Connection.WriteLine("250 CWD command successful, new folder is '{0}'.", lSession.Directory);
						lSession.Directory = lNewDir;
					}
					else
					{
						e.Connection.WriteLine("550 Permission Denied.");
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_CDUP(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
			{
				if (e.Parameters.Length != 0)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					String lNewDir = lSession.Directory;
					if (lNewDir.Length > 2)
					{
						Int32 lNewLen = lNewDir.LastIndexOf('/', lNewDir.Length - 2);
						if (lNewLen <= 0)
							lNewLen = 1;
						lNewDir = lNewDir.Substring(0, lNewLen);
						if (lNewDir.Length == 0)
							lNewDir = "/";
					}

					FtpChangeDirectoryArgs lEventArgs = new FtpChangeDirectoryArgs(e.Session, e.Connection, e.Server);
					lEventArgs.NewDirectory = lNewDir;
					try
					{
						((FtpServer)e.Server).InvokeOnChangeDirectory(lEventArgs);
					}
					catch (FtpException ex)
					{
						e.Connection.WriteLine(ex.ToString());
						return;
					}
					catch
					{
						e.Connection.WriteLine("500 Internal Error");
						return;
					}

					if (lEventArgs.ChangeDirOk)
					{
						e.Connection.WriteLine("250 CDUP command successful.");
						lSession.Directory = lNewDir;
					}
					else
					{
						e.Connection.WriteLine("550 Permission Denied.");
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_SYST(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.LoggedIn)
				e.Connection.WriteLine("215 " + ((FtpServer)e.Server).SystemType);
			else
				e.Connection.WriteLine("503 Bad sequence of commands.");
		}

		public static void Cmd_USER(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.Start || lSession.State == FtpState.PasswordRequired)
			{
				if (e.Parameters.Length != 1)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					lSession.Username = e.Parameters[0];
					e.Connection.WriteLine("331 User name okay, need password.");
					lSession.State = FtpState.PasswordRequired;
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_ACCT(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.AccountRequired)
			{
				if (e.Parameters.Length != 1)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					FtpUserAccountArgs lEventArgs = new FtpUserAccountArgs(e.Session, e.Connection, e.Server);
					lEventArgs.AccountName = e.Parameters[0];
					try
					{
						((FtpServer)e.Server).InvokeOnAccount(lEventArgs);
					}
					catch (FtpException ex)
					{
						e.Connection.WriteLine(ex.ToString());
						return;
					}
					catch
					{
						e.Connection.WriteLine("500 Internal Error");
						return;
					}

					if (lEventArgs.LoginOk)
					{
						e.Connection.WriteLine("230 User logged in, proceed.");
						lSession.State = FtpState.LoggedIn;
					}
					else
					{
						lSession.State = FtpState.Start;
						e.Connection.WriteLine("530 Unable to login");
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		public static void Cmd_PASS(Object sender, CommandEventArgs e)
		{
			FtpSession lSession = (FtpSession)e.Session;

			if (lSession.State == FtpState.PasswordRequired)
			{
				if (e.Parameters.Length != 1)
				{
					e.Connection.WriteLine("501 Syntax error in parameters or arguments.");
				}
				else
				{
					FtpUserLoginEventArgs lEventArgs = new FtpUserLoginEventArgs(e.Session, e.Connection, e.Server);
					lEventArgs.UserName = lSession.Username;
					lEventArgs.Password = e.Parameters[0];
					try
					{
						((FtpServer)e.Server).InvokeOnUserLogin(lEventArgs);
					}
					catch (FtpException ex)
					{
						e.Connection.WriteLine(ex.ToString());
						return;
					}
					catch
					{
						e.Connection.WriteLine("500 Internal Error");
						return;
					}

					if (lEventArgs.LoginOk)
					{
						if (lEventArgs.NeedAccount)
						{
							lSession.State = FtpState.AccountRequired;
							e.Connection.WriteLine("332 Need account for login.");
						}
						else
						{
							lSession.State = FtpState.LoggedIn;
							e.Connection.WriteLine("230 User logged in, proceed.");
						}
					}
					else
					{
						lSession.State = FtpState.Start;
						e.Connection.WriteLine("530 Unable to login");
					}
				}
			}
			else
			{
				e.Connection.WriteLine("503 Bad sequence of commands.");
			}
		}

		internal protected override void HandleCommandException(Connection connection, Exception exception)
		{
			if (this.Detailed500Errors)
			{
				connection.WriteLine(String.Format("500 Internal Error: ({0}) {1}", exception.ToString(), CleanStringForCommandResponse(exception.Message)));
			}
			else
			{
				connection.WriteLine("500 Internal Error");
			}

			// Don't call base, as we wanna keep the connection open.
		}
	}

	public enum FtpState
	{
		Start, // Connection is opened
		PasswordRequired, // USER command was given
		AccountRequired, // PASS command was ok, server requires ACCOUNT
		LoggedIn,
		Transfer
	};

	public class FtpSession : CommandBasedSession
	{
		public FtpSession()
		{
			this.Directory = "/";
			this.RenameFrom = null;
			this.UserData = null;
			this.Image = false;
			this.State = FtpState.Start;
			this.RestartPoint = 0;
			this.PassiveServer = null;
			this.ActiveConnection = null;
			this.TransferThread = null;
			this.Passive = false;
			this.Username = null;
		}

		#region Properties
		public virtual String Directory { get; set; }

		public virtual String RenameFrom { get; set; }

		public Object UserData { get; set; }

		public Boolean Image { get; set; }

		public FtpState State { get; set; }

		public Int64 RestartPoint { get; set; }

		public SimpleServer PassiveServer { get; set; }

		public Connection ActiveConnection { get; set; }

		internal FtpTransferThread TransferThread { get; set; }

		public Boolean Passive
		{
			get
			{
				return this.fPassive;
			}
			set
			{
				this.fPassive = value;

				if (!this.fPassive && this.PassiveServer != null)
				{
					this.PassiveServer.Close();
					this.PassiveServer = null;
				}
			}
		}
		private Boolean fPassive;

		public String Username { get; set; }
		#endregion
	}

	#region Transfer Thread
	class FtpTransferThread
	{
		private readonly FtpTransferEventArgs fEventArgs;
		private readonly Boolean fStore;

		public FtpTransferThread(FtpTransferEventArgs e, Boolean store)
		{
			this.fEventArgs = e;
			this.fStore = store;

			new Thread(() => Execute()).Start();
		}

		public void Abort()
		{
			try
			{
				((FtpSession)this.fEventArgs.Session).ActiveConnection.Close();
			}
			catch (Exception)
			{
				// As designed
			}
		}

		public static String GetFirstLine(String value)
		{
			if (value.IndexOf("\r\n") != -1)
			{
				value = value.Substring(0, value.IndexOf("\r\n"));
			}

			if (value.IndexOf("\n") != -1)
			{
				value = value.Substring(0, value.IndexOf("\n"));
			}

			return value;
		}

		private void Execute()
		{
			Boolean lOk = true;

			try
			{
				if (fStore)
				{
					this.fEventArgs.DataChannel = ((FtpSession)this.fEventArgs.Session).ActiveConnection;
					((FtpServer)this.fEventArgs.Server).InvokeOnStoreFile(this.fEventArgs);
				}
				else
				{
					this.fEventArgs.DataChannel = ((FtpSession)this.fEventArgs.Session).ActiveConnection;
					((FtpServer)this.fEventArgs.Server).InvokeOnRetrieveFile(this.fEventArgs);
				}

			}
			catch (FtpException ex)
			{
				this.fEventArgs.Connection.WriteLine(ex.ToString());
				lOk = false;
			}
			catch (Exception ex)
			{
				this.fEventArgs.Connection.WriteLine("500 Internal Error " + FtpTransferThread.GetFirstLine(ex.Message));
				lOk = false;
			}

			((FtpSession)this.fEventArgs.Session).State = FtpState.LoggedIn;
			try
			{
				((FtpSession)this.fEventArgs.Session).ActiveConnection.Close();
			}
			catch (Exception)
			{
				// As designed
			}

			((FtpSession)this.fEventArgs.Session).ActiveConnection = null;

			if (lOk)
			{
				this.fEventArgs.Connection.WriteLine("226 Transfer complete");
			}

			((FtpSession)this.fEventArgs.Session).TransferThread = null;
		}
	}
	#endregion
}