/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.InternetPack.Dns;

namespace RemObjects.InternetPack
{
	public abstract class Client : Component
	{
		protected Client()
		{
			this.EnableNagle = false;
			this.fBindingV4 = new Binding();
			this.fBindingV6 = new Binding(AddressFamily.InterNetworkV6);
			this.DnsResolveType = DnsResolveType.Once;
#if FULLFRAMEWORK
			this.SslOptions = new SslConnectionFactory();
#endif
		}

		protected virtual void ResolveHostNameIfNeeded()
		{
			if (String.IsNullOrEmpty(this.HostName))
				return;

			if ((this.HostAddress == null) || (this.DnsResolveType == DnsResolveType.Always))
				this.ResolveHostName();
		}

		protected virtual void ResolveHostName()
		{
			OnResolveHostNameEventArgs lEventArgs = new OnResolveHostNameEventArgs(this.HostName);

			this.TriggerOnResolveHostName(lEventArgs);

			if (String.IsNullOrEmpty(lEventArgs.HostName))
			{
				throw new Exception("No Hostname set");
			}

			this.HostName = lEventArgs.HostName;
			this.HostAddress = DnsLookup.ResolveFirst(this.HostName);

			this.TriggerOnResolvedHostName(new OnResolvedHostNameEventArgs(this.HostName, this.HostAddress));
		}

		#region Properties
		[DefaultValue(DnsResolveType.Once)]
		public DnsResolveType DnsResolveType { get; set; }

		[Obsolete("Please use BindingV4 and BindingV6 instead", false)]
		public Binding Binding
		{
			get
			{
				return this.BindingV4;
			}
		}

		public Binding BindingV4
		{
			get
			{
				return this.fBindingV4;
			}
		}
		private readonly Binding fBindingV4;

		public Binding BindingV6
		{
			get
			{
				return fBindingV6;
			}
		}
		private readonly Binding fBindingV6;

		public Int32 Port { get; set; }

		public String HostName
		{
			get
			{
				return this.fHostName;
			}
			set
			{
				if (this.fHostName != value)
				{
					this.fHostName = value;
					this.fHostAddress = null;
				}
			}
		}
		private String fHostName;

		public IPAddress HostAddress
		{
			get
			{
				return this.fHostAddress;
			}
			set
			{
				this.fHostAddress = value;
				if (this.fHostAddress != null)
					this.fHostName = this.fHostAddress.ToString();
				else
					this.fHostName = null;
			}
		}
		private IPAddress fHostAddress;

		#if ECHOES
		[Browsable(false)]
		public Type ConnectionClass
		{
			get
			{
				return this.fConnectionClass;
			}
			set
			{

				if (value != null && !value.IsSubclassOf(typeof(Connection)))
					throw new Exception(String.Format("The assigned Type '{0}' is not a descendant of Connection", value.FullName));

				this.fConnectionClass = value;
			}
		}
		private Type fConnectionClass;
		#endif

		[Browsable(false)]
		public IConnectionFactory ConnectionFactory { get; set; }

		protected ConnectionPool ConnectionPool
		{
			get
			{
				return this.fConnectionPool;
			}
			set
			{
				if (value != this.fConnectionPool)
				{
					if (this.fConnectionPool != null)
						this.fConnectionPool.Dispose();
				}

				this.fConnectionPool = value;
			}
		}
		private ConnectionPool fConnectionPool;

#if FULLFRAMEWORK
		public SslConnectionFactory SslOptions { get; set; }
#endif

		[DefaultValue(false)]
		public Boolean EnableNagle { get; set; }
		#endregion

		#region Events
		public event OnResolveHostNameEventHandler OnResolveHostName;

		protected virtual void TriggerOnResolveHostName(OnResolveHostNameEventArgs e)
		{
			if (this.OnResolveHostName == null)
				return;

			this.OnResolveHostName(this, e);
		}

		public event OnResolvedHostNameEventHandler OnResolvedHostName;

		protected virtual void TriggerOnResolvedHostName(OnResolvedHostNameEventArgs e)
		{
			if (this.OnResolvedHostName == null)
				return;

			this.OnResolvedHostName(this, e);
		}
		#endregion

		#region Methods
		public virtual Connection Connect()
		{
			this.ResolveHostNameIfNeeded();

			return this.Connect(this.HostAddress, this.Port);
		}

		public virtual Connection Connect(String hostname, Int32 port)
		{
			return this.Connect(Dns.DnsLookup.ResolveFirst(hostname), port);
		}

		public virtual Connection Connect(IPAddress host, Int32 port)
		{
			return this.GetConnection(host, port);
		}

		public virtual Connection ConnectNew(String hostname, Int32 port)
		{
			IPAddress lHostAddress = Dns.DnsLookup.ResolveFirst(hostname);

			return GetNewConnection(lHostAddress, port);
		}

		public virtual Connection ConnectNew(IPAddress host, Int32 port)
		{
			return GetNewConnection(host, port);
		}

		protected virtual Connection GetConnection(IPAddress host, Int32 port)
		{
			if (this.fConnectionPool != null)
				return this.fConnectionPool.GetConnection(new IPEndPoint(host, port));

			Binding lBinding;
			switch (host.AddressFamily)
			{
				case AddressFamily.InterNetwork:
					lBinding = this.fBindingV4;
					break;

				case AddressFamily.InterNetworkV6:
					lBinding = this.fBindingV6;
					break;

				default:
					lBinding = new Binding(host.AddressFamily);
					break;
			}

			Connection lConnection = this.NewConnection(lBinding);
			lConnection.Connect(host, port);

			return lConnection;
		}

		private Connection GetNewConnection(IPAddress host, Int32 port)
		{
			if (this.fConnectionPool != null)
				return this.fConnectionPool.GetNewConnection(new IPEndPoint(host, port));

			return this.GetConnection(host, port);
		}

		protected virtual Connection NewConnection(Binding binding)
		{
			if (this.ConnectionFactory != null)
			{
				return this.ConnectionFactory.CreateClientConnection(binding);
			}

			#if ECHOES
			if (this.fConnectionClass != null)
			{
				return (Connection)Activator.CreateInstance(this.fConnectionClass);
			}
			#endif
			#if FULLFRAMEWORK
			if (this.SslOptions.Enabled)
			{
				Connection lSslConnection = this.SslOptions.CreateClientConnection(binding);
				lSslConnection.EnableNagle = this.EnableNagle;

				return lSslConnection;
			}
			#endif
			Connection lConnection = new Connection(binding);
			lConnection.EnableNagle = this.EnableNagle;

			return lConnection;
		}

		public static Connection Connect(String hostname, Int32 port, Binding binding)
		{
			IPAddress lHostAddress = Dns.DnsLookup.ResolveFirst(hostname);

			return Client.Connect(lHostAddress, port, binding);
		}

		public static Connection Connect(IPAddress host, Int32 port, Binding binding)
		{
			Connection lConnection = new Connection(binding);
			lConnection.Connect(host, port);

			return lConnection;
		}

		protected virtual void ReleaseConnection(Connection connection)
		{
			if (fConnectionPool != null)
				fConnectionPool.ReleaseConnection(connection);
			else
				connection.Dispose();
		}
		#endregion
	}

	public enum DnsResolveType
	{
		Once,
		Always
	}

	public class OnResolveHostNameEventArgs : EventArgs
	{
		public OnResolveHostNameEventArgs(String hostname)
		{
			this.HostName = hostname;
		}

		public String HostName { get; set; }
	}

	public delegate void OnResolveHostNameEventHandler(Object sender, OnResolveHostNameEventArgs e);

	public class OnResolvedHostNameEventArgs : EventArgs
	{
		public OnResolvedHostNameEventArgs(String hostname, IPAddress address)
		{
			this.fHostName = hostname;
			this.fHostAddress = address;
		}

		public String HostName
		{
			get
			{
				return this.fHostName;
			}
		}
		private readonly String fHostName;

		public IPAddress HostAddress
		{
			get
			{
				return this.fHostAddress;
			}
		}

		private readonly IPAddress fHostAddress;
	}

	public delegate void OnResolvedHostNameEventHandler(Object sender, OnResolvedHostNameEventArgs e);
}