/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/
#if toffee || cooper
using RemObjects.Elements.RTL.Reflection;
#endif

namespace RemObjects.InternetPack.CommandBased
{
	public class SessionEventArgs : EventArgs
	{
		public SessionEventArgs(Object session, Connection connection, Server server)
		{
			this.fConnection = connection;
			this.fSession = session;
			this.fServer = server;
		}

		public Object Session
		{
			get
			{
				return this.fSession;
			}
		}
		private readonly Object fSession;

		public Connection Connection
		{
			get
			{
				return this.fConnection;
			}
		}
		private readonly Connection fConnection;

		public Server Server
		{
			get
			{
				return this.fServer;
			}
		}
		private readonly Server fServer;
	}

	public class CommandEventArgs : SessionEventArgs
	{
		public CommandEventArgs(Object session, Connection connection, Server server)
			: base(session, connection, server)
		{
		}

		public String Command { get; set; }

		public String AllParameters { get; set; }

		public String[] Parameters { get; set; }
	}

	public delegate void OnCommandHandler(Object sender, CommandEventArgs e);

	public delegate void OnClientConnectedHandler(Object sender, SessionEventArgs e);

	public delegate void OnClientDisconnectedHandler(Object sender, SessionEventArgs e);

	public abstract class CommandBasedServer : Server
	{
		protected CommandBasedServer()
		{
			this.UnknownCommand = "500 {0}: Command not understood";
			this.Greeting = "GREETING";
			this.fCommands = new Dictionary<String, OnCommandHandler>(16);
			this.InitCommands();
		}

		#region Properties
		public Dictionary<String, OnCommandHandler> Commands
		{
			get
			{
				return this.fCommands;
			}
		}
		private readonly Dictionary<String, OnCommandHandler> fCommands;

		public String UnknownCommand { get; set; }

		public String Greeting { get; set; }

		public Type SessionClass { get; set; }
		#endregion

		#region Events
		public event OnClientConnectedHandler OnClientConnected;

		public event OnClientDisconnectedHandler OnClientDisconnected;

		protected internal virtual void InvokeOnClientConnected(SessionEventArgs e)
		{
			if (this.OnClientConnected != null)
				this.OnClientConnected(this, e);
		}

		protected internal virtual void InvokeOnClientDisconnected(SessionEventArgs e)
		{
			if (this.OnClientDisconnected != null)
				this.OnClientDisconnected(this, e);
		}
		#endregion

		protected internal virtual CommandBasedSession CreateSession()
		{
			Type lType;
            if (this.SessionClass != null)
                lType = this.SessionClass;
            else
                lType = this.GetDefaultSessionClass();

            #if echoes
            return (CommandBasedSession)Activator.CreateInstance(lType);
            #elif cooper
            return (CommandBasedSession)Class.getDeclaredConstructor(lType).newInstance();
            #elif island
            return (CommandBasedSession)lType.Instantiate();
            #elif toffee
            return (CommandBasedSession)lType.init();
            #endif
		}

		protected abstract void InitCommands();

		public void AddCustomCommand(String name, OnCommandHandler handler)
		{
			this.fCommands.Add(name, handler);
		}

		public override Type GetWorkerClass()
		{
			return typeof(CommandBasedWorker);
		}

		protected virtual Type GetDefaultSessionClass()
		{
			return typeof(CommandBasedSession);
		}

		protected static String CleanStringForCommandResponse(String rawValue)
		{
			return rawValue.Replace("\n", "").Replace("\r", "");
		}

		protected internal virtual void HandleCommandException(Connection connection, Exception ex)
		{
			/* On error, just close the connection, descendand classes may implement
			 * additional/different behavior */
			connection.Close();
		}
	}

	public class CommandBasedSession
	{
		public IPEndPoint RemoteEndPoint { get; set; }

		public IPEndPoint LocalEndPoint { get; set; }
	}

	class CommandBasedWorker : Worker
	{
		protected override void DoWork()
		{
			CommandBasedServer lServer = (CommandBasedServer)Owner;
			CommandBasedSession lSession = lServer.CreateSession();
			lSession.RemoteEndPoint = ((IPEndPoint)DataConnection.RemoteEndPoint);
			lSession.LocalEndPoint = ((IPEndPoint)DataConnection.RemoteEndPoint);

			try
			{
				lServer.InvokeOnClientConnected(new SessionEventArgs(lSession, DataConnection, lServer));
				DataConnection.WriteLine(lServer.Greeting);
				CommandEventArgs lArgs = new CommandEventArgs(lSession, DataConnection, lServer);
				String lCmdLine;

				while (DataConnection.Connected)
				{
					lCmdLine = DataConnection.ReadLine().Trim();

					if (lCmdLine.Length == 0)
						continue;
					Int32 tempidx = lCmdLine.IndexOf(' ');

					if (tempidx == -1)
					{
						lArgs.Command = lCmdLine.ToUpper();
						lArgs.AllParameters = "";
						lArgs.Parameters = new String[0];
					}
					else
					{
						lArgs.Command = lCmdLine.Substring(0, tempidx).ToUpper();
						lArgs.AllParameters = lCmdLine.Substring(tempidx + 1).Trim();
						lArgs.Parameters = lArgs.AllParameters.Split(" ").ToArray();
					}

					OnCommandHandler lCommandHandler;
					lCommandHandler = lServer.Commands[lArgs.Command];

					if (lCommandHandler == null)
					{
						DataConnection.WriteLine(String.Format(lServer.UnknownCommand, lArgs.Command));
					}
					else
					{
						try
						{
							lCommandHandler(lServer, lArgs);
						}
						catch (Exception ex)
						{
							lServer.HandleCommandException(DataConnection, ex);
						}
					}
				}
			}
			catch (Exception)
			{
				// As designed
			}

			lServer.InvokeOnClientDisconnected(new SessionEventArgs(lSession, DataConnection, lServer));
		}
	}
}