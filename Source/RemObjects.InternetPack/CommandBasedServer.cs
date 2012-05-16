/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Net;

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

        private readonly Object fSession;
        public Object Session
        {
            get
            {
                return this.fSession;
            }
        }

        private readonly Connection fConnection;
        public Connection Connection
        {
            get
            {
                return this.fConnection;
            }
        }

        private readonly Server fServer;
        public Server Server
        {
            get
            {
                return this.fServer;
            }
        }
    }

    public class CommandEventArgs : SessionEventArgs
    {
        public CommandEventArgs(Object session, Connection connection, Server server)
            : base(session, connection, server)
        {
        }

        public String Command
        {
            get
            {
                return this.fCommand;
            }
            set
            {
                this.fCommand = value;
            }
        }
        private String fCommand;

        public String AllParameters
        {
            get
            {
                return this.fAllParameters;
            }
            set
            {
                this.fAllParameters = value;
            }
        }
        private String fAllParameters;

        public String[] Parameters
        {
            get
            {
                return this.fParameters;
            }
            set
            {
                this.fParameters = value;
            }
        }
        private String[] fParameters;
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

        public String UnknownCommand
        {
            get
            {
                return this.fUnknownCommand;
            }
            set
            {
                this.fUnknownCommand = value;
            }
        }
        private String fUnknownCommand;

        public String Greeting
        {
            get
            {
                return this.fGreeting;
            }
            set
            {
                this.fGreeting = value;
            }
        }
        private String fGreeting;

        public Type SessionClass
        {
            get
            {
                return this.fSessionClass;
            }
            set
            {
                this.fSessionClass = value;
            }
        }
        private Type fSessionClass;
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
            if (this.SessionClass != null)
                return (CommandBasedSession)Activator.CreateInstance(this.SessionClass);

            return (CommandBasedSession)Activator.CreateInstance(this.GetDefaultSessionClass());
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
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return this.fRemoteEndPoint;
            }
            set
            {
                this.fRemoteEndPoint = value;
            }
        }
        private IPEndPoint fRemoteEndPoint;

        public IPEndPoint LocalEndPoint
        {
            get
            {
                return this.fLocalEndPoint;
            }
            set
            {
                this.fLocalEndPoint = value;
            }
        }
        private IPEndPoint fLocalEndPoint;
    }

    class CommandBasedWorker : Worker
    {
        private Char[] SPACE = { ' ' };

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
                        lArgs.Parameters = lArgs.AllParameters.Split(SPACE);
                    }

                    OnCommandHandler lCommandHandler;
                    lServer.Commands.TryGetValue(lArgs.Command, out lCommandHandler);

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
            }

            lServer.InvokeOnClientDisconnected(new SessionEventArgs(lSession, DataConnection, lServer));
        }
    }
}