/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/
#if macos || ios
using RemObjects.Elements.RTL.Reflection;
#endif

namespace RemObjects.InternetPack
{
	public abstract class Server : Component
	{
		protected Server()
		{
#if FULLFRAMEWORK
			this.fSslOptions = new SslConnectionFactory();
#endif

#if FULLFRAMEWORK
			if (Socket.SupportsIPv4)
			{
				fBindingV4 = new ServerBinding();
				fBindingV4.Address = IPAddress.Any;
				fBindingV4.AddressFamily = AddressFamily.InterNetwork;
			}

			// This is a workaround for Mono 2.10 / Mac
			// Socket.OSSupportsIPv6 there fails with ArgumentException
			Boolean lIsIPv6Supported;
			try
			{
				lIsIPv6Supported = Socket.OSSupportsIPv6;
			}
			catch (ArgumentException)
			{
				lIsIPv6Supported = (Environment.OS != OperatingSystem.Windows); // Mono reports Unix platform for MacOSX
			}

			if (lIsIPv6Supported)
			{
				fBindingV6 = new ServerBinding();
				fBindingV6.Address = IPAddress.IPv6Any;
				fBindingV6.AddressFamily = AddressFamily.InterNetworkV6;
			}
#else
			fBindingV4 = new ServerBinding();
			fBindingV4.Address = IPAddress.Any;
			fBindingV4.AddressFamily = AddressFamily.InterNetwork;
#endif

			if (fBindingV4 == null && fBindingV6 == null)
				throw new Exception("This host's network stack supports neither IPv4 nor IPv6 Internet Protocol");

			Timeout = Connection.DEFAULT_TIMEOUT;
			TimeoutEnabled = true;
			MaxLineLength = Connection.DEFAULT_MAX_LINE_LENGTH;
			MaxLineLengthEnabled = true;
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing)
				Close();

			base.Dispose(disposing);
		}

		#region Properties
		[Category("Server"), Browsable(false), Obsolete("Please use BindingV4 and BindingV6 instead", false)]
		public ServerBinding Binding
		{
			get
			{
				if (fBindingV4 != null)
					return fBindingV4;

				return fBindingV6;
			}
		}

		[Category("Server"), Browsable(false)]
		public ServerBinding BindingV4
		{
			get
			{
				return fBindingV4;
			}
		}
		private readonly ServerBinding fBindingV4;

		[Category("Server"), Browsable(false)]
		public ServerBinding BindingV6
		{
			get
			{
				return fBindingV6;
			}
		}
		private readonly ServerBinding fBindingV6;

		[Category("Server"), Browsable(true), DefaultValue(true)]
		public Boolean BindV6
		{
			get
			{
				return fBindV6;
			}
			set
			{
				fBindV6 = value;
			}
		}
		private Boolean fBindV6 = true;

		[Category("Server"), Browsable(true), DefaultValue(true)]
		public Boolean BindV4
		{
			get
			{
				return fBindV4;
			}
			set
			{
				fBindV4 = value;
			}
		}
		private Boolean fBindV4 = true;

		[Category("Server")]
		public Int32 Port
		{
			get
			{
				if (fBindingV4 != null)
					return fBindingV4.Port;

				return fBindingV6.Port;
			}
			set
			{
				if (fBindingV4 != null)
					fBindingV4.Port = value;

				if (fBindingV6 != null)
					fBindingV6.Port = value;
			}
		}

		[Category("Server"), DefaultValue(true)]
		public Boolean CloseConnectionsOnShutdown
		{
			get
			{
				return fCloseConnectionsOnShutdown;
			}
			set
			{
				fCloseConnectionsOnShutdown = value;
			}
		}
		private Boolean fCloseConnectionsOnShutdown = true;

		[Category("Server"), DefaultValue(false)]
		public Boolean EnableNagle
		{
			get
			{
				return fEnableNagle;
			}
			set
			{
				fEnableNagle = value;
			}
		}
		private Boolean fEnableNagle;

		protected Int32 DefaultPort
		{
			get
			{
				if (fBindingV4 != null)
					return fBindingV4.DefaultPort;

				return fBindingV6.DefaultPort;
			}
			set
			{
				if (fBindingV4 != null)
				{
					fBindingV4.DefaultPort = value;
					fBindingV4.Port = value;
				}

				if (fBindingV6 != null)
				{
					fBindingV6.DefaultPort = value;
					fBindingV6.Port = value;
				}
			}
		}

		public Boolean ShouldSerializePort()
		{
			return (Port != DefaultPort);
		}

		#if ECHOES
		[Browsable(false), DefaultValue(null)]
		public Type ConnectionClass
		{
			get
			{
				return fConnectionClass;
			}
			set
			{

				if (value != null && !value.IsSubclassOf(typeof(Connection)))
					throw new Exception(String.Format("The assigned Type '{0}' is not a descendant of Connection", value.FullName));
				fConnectionClass = value;
			}
		}
		private Type fConnectionClass;
		#endif

		[Browsable(false), DefaultValue(null)]
		public IConnectionFactory ConnectionFactory
		{
			get
			{
				return fConnectionFactory;
			}
			set
			{
				fConnectionFactory = value;
			}
		}
		private IConnectionFactory fConnectionFactory;

#if FULLFRAMEWORK
		[Category("Server")]
		public SslConnectionFactory SslOptions
		{
			get
			{
				return fSslOptions;
			}
		}
		private readonly SslConnectionFactory fSslOptions;
#endif

#if FULLFRAMEWORK
		[Category("Server"), Browsable(false), DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
		public Boolean Active
		{
			get
			{
				return fActive;
			}
			set
			{
				if (value != Active)
				{
					if (value)
						Open();
					else
						Close();
				}
			}
		}
		protected Boolean fActive;
		#endregion

		#region Properties: Security
		[Category("Security"), DefaultValue(true)]
		public Boolean TimeoutEnabled
		{
			get
			{
				return fTimeoutEnabled;
			}
			set
			{
				fTimeoutEnabled = value;
			}
		}
		private Boolean fTimeoutEnabled;

		[Category("Security"), DefaultValue(Connection.DEFAULT_TIMEOUT)]
		public Int32 Timeout
		{
			get
			{
				return fTimeout;
			}
			set
			{
				fTimeout = value;
			}
		}
		private Int32 fTimeout;

		[Category("Security"), DefaultValue(true)]
		public Boolean MaxLineLengthEnabled
		{
			get
			{
				return fMaxLineLengthEnabled;
			}
			set
			{
				fMaxLineLengthEnabled = value;
			}
		}
		private Boolean fMaxLineLengthEnabled;

		[Category("Security"), DefaultValue(Connection.DEFAULT_MAX_LINE_LENGTH)]
		public Int32 MaxLineLength
		{
			get
			{
				return fMaxLineLength;
			}
			set
			{
				fMaxLineLength = value;
			}
		}
		private Int32 fMaxLineLength;
		#endregion

		#region Methods
		public virtual void Open()
		{
			try
			{
#if FULLFRAMEWORK
				if (this.SslOptions.Enabled)
				{
					this.SslOptions.LoadCertificate();
				}
#endif

				Int32 lActualPort = this.Port;

				Boolean lBindV6 = (this.fBindingV6 != null) && this.fBindV6;
				if (lBindV6)
				{
					this.fBindingV6.EnableNagle = EnableNagle;
					this.fBindingV6.Bind(new Listener(this, this.GetWorkerClass()));
					lActualPort = ((IPEndPoint)this.fBindingV6.ListeningSocket.LocalEndPoint).Port;
				}

				// There is a chance that this will fail on Mono
				// Unfortunately this code shouldn't fail on Mac while it WILL fail on Linux
				// And no one can warrant that suddenly this Mono/Linux issue won't be fixed
				if (this.fBindV4 && (this.fBindingV4 != null))
				{
					try
					{
						if (this.Port == 0)
							this.fBindingV4.Port = lActualPort;
						this.fBindingV4.EnableNagle = EnableNagle;
						this.fBindingV4.Bind(new Listener(this, this.GetWorkerClass()));
						lActualPort = ((IPEndPoint)this.fBindingV4.ListeningSocket.LocalEndPoint).Port;
					}
					catch (SocketException)
					{
						if (!(lBindV6 && Environment.IsMono))
							throw;
					}

				}

				this.Port = lActualPort;

				this.fActive = true;
			}
			catch
			{
				this.Close();
				throw;
			}
		}

		public virtual void Close()
		{
			fActive = false;
			if (fBindingV4 != null && fBindV4)
				fBindingV4.Unbind(true);

			if (fBindingV6 != null && fBindV6)
				fBindingV6.Unbind(true);
		}

		public virtual Type GetWorkerClass()
		{
			return typeof(Worker);
		}
		#endregion
	}

	public class Listener : IListener
	{
		public Listener(Server owner, Type workerClass)
		{
			fOwner = owner;
			fWorkerClass = workerClass;
		}

		#region Properties
		public Socket ListeningSocket
		{
			get
			{
				return fListeningSocket;
			}
			set
			{
				fListeningSocket = value;
			}
		}
		private Socket fListeningSocket;

		public Type WorkerClass
		{
			get
			{
				return fWorkerClass;
			}
		}
		private readonly Type fWorkerClass;

		public Server Owner
		{
			get
			{
				return fOwner;
			}
			set
			{
				fOwner = value;
			}
		}
		private Server fOwner;
		#endregion

		public virtual void Listen()
		{
			WorkerCollection lWorkers = null;
			if (Owner.CloseConnectionsOnShutdown) lWorkers = new WorkerCollection();

			try
			{
				Socket lSocket;
				do
				{
					try
					{
						lSocket = fListeningSocket.Accept();
					}
					catch (ObjectDisposedException)
					{
						return;
					}
					catch (SocketException)
					{
						/* If Accept fails with a SocketException, the socket was ListeningSocket was probably
						* closed, so we'll just exit and terminate the thread. */
						return;
					}

					if (lSocket != null)
					{
						#if !NEEDS_PORTING
						Object lObject = Activator.CreateInstance(WorkerClass);
						#else
						Object lObject = null;
						#endif
						IWorker lWorker = lObject as IWorker;
						lWorker.Owner = Owner;

						if (Owner.ConnectionFactory != null)
						{
							lWorker.DataConnection = Owner.ConnectionFactory.CreateServerConnection(lSocket);
						}
						#if ECHOES
						else if (Owner.ConnectionClass != null)
						{
							lWorker.DataConnection = (Connection)Activator.CreateInstance(Owner.ConnectionClass);
							lWorker.DataConnection.Init(lSocket);
						}
						#endif
						#if FULLFRAMEWORK
						else if (Owner.SslOptions.Enabled)
						{
							lWorker.DataConnection = Owner.SslOptions.CreateServerConnection(lSocket);
						}
						#endif
						else
						{
							lWorker.DataConnection = new Connection(lSocket);
						}

						#if FULLFRAMEWORK
						if (Owner.TimeoutEnabled)
						{
							lWorker.DataConnection.TimeoutEnabled = true;
							lWorker.DataConnection.Timeout = Owner.Timeout;
						}
						#endif

						if (Owner.MaxLineLengthEnabled)
						{
							lWorker.DataConnection.MaxLineLengthEnabled = true;
							lWorker.DataConnection.MaxLineLength = Owner.MaxLineLength;
						}

						try
						{
							lWorker.DataConnection.InitializeServerConnection();
						}
						catch (Exception) // nothing should escape this loop.
						{
							lWorker.DataConnection.Dispose();
							continue;
						}
                        
                        lWorker.Thread = new Thread( () => { lWorker.Work(); });
						try
						{
							#if FULLFRAMEWORK
							lWorker.Thread.Name = String.Format("Internet Pack {0} for {1}", WorkerClass.Name, lSocket.RemoteEndPoint);
							#endif
						}
						catch (SocketException)
						{
							// mono can fail in that code if the remote side has been disconnected. Since we already create a thread we have to run it (leak otherwise)
						}

						if (lWorkers != null)
							lWorkers.Add(lWorker);

						lWorker.Thread.Start();
                    }
				}
				while (lSocket != null);
			}
			finally
			{
				if (lWorkers != null)
					lWorkers.Close();
			}

		}
	}
}