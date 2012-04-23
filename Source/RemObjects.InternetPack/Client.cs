/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using RemObjects.InternetPack.Dns;

namespace RemObjects.InternetPack
{
    public abstract class Client : System.ComponentModel.Component
    {
        protected Client()
        {
            this.EnableNagle = false;
            this.fBindingV4 = new Binding();
            this.fBindingV6 = new Binding(AddressFamily.InterNetworkV6);
            this.fDnsResolveType = DnsResolveType.Once;
#if FULLFRAMEWORK
            this.fSslOptions = new SslConnectionFactory();
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
            String lHostName = this.HostName;
            this.TriggerOnResolveHostName(ref lHostName);

            if (lHostName == null)
                throw new Exception("No Hostname set");

            this.HostAddress = DnsLookup.ResolveFirst(this.HostName);
            this.TriggerOnResolvedHostName(lHostName, this.HostAddress);
        }

        #region Properties
        [DefaultValue(DnsResolveType.Once)]
        public DnsResolveType DnsResolveType
        {
            get
            {
                return fDnsResolveType;
            }
            set
            {
                fDnsResolveType = value;
            }
        }
        private DnsResolveType fDnsResolveType;

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
        private Binding fBindingV4;

        public Binding BindingV6
        {
            get
            {
                return fBindingV6;
            }
        }
        private Binding fBindingV6;

        public Int32 Port
        {
            get
            {
                return this.fPort;
            }
            set
            {
                this.fPort = value;
            }
        }
        private Int32 fPort;

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

        [Browsable(false)]
        public IConnectionFactory ConnectionFactory
        {
            get
            {
                return this.fConnectionFactory;
            }
            set
            {
                this.fConnectionFactory = value;
            }
        }
        private IConnectionFactory fConnectionFactory;

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
        public SslConnectionFactory SslOptions
        {
            get
            {
                return this.fSslOptions;
            }
            set
            {
                this.fSslOptions = value;
            }
        }
        private SslConnectionFactory fSslOptions;
#endif

        [DefaultValue(false)]
        public Boolean EnableNagle
        {
            get
            {
                return this.fEnableNagle;
            }
            set
            {
                this.fEnableNagle = value;
            }
        }
        private Boolean fEnableNagle;
        #endregion

        #region Events
        public delegate void OnResolveHostNameHandler(Object sender, OnResolveHostNameArgs e);

        public class OnResolveHostNameArgs : EventArgs
        {
            public String HostName
            {
                get
                {
                    return this.fHostName;
                }
                set
                {
                    this.fHostName = value;
                }
            }
            private String fHostName;
        }

        public event OnResolveHostNameHandler OnResolveHostName;

        protected virtual void TriggerOnResolveHostName(ref String hostname)
        {
            if (this.OnResolveHostName == null)
                return;

            OnResolveHostNameArgs lEventArgs = new OnResolveHostNameArgs();
            lEventArgs.HostName = hostname;

            this.OnResolveHostName(this, lEventArgs);

            hostname = lEventArgs.HostName;
        }

        public delegate void OnResolvedHostNameHandler(Object sender, OnResolvedHostNameArgs e);

        public class OnResolvedHostNameArgs : EventArgs
        {
            public String HostName
            {
                get
                {
                    return this.fHostName;
                }
                set
                {
                    this.fHostName = value;
                }
            }
            private String fHostName;

            public IPAddress IPAddress
            {
                get
                {
                    return fIPAddress;
                }
                set
                {
                    fIPAddress = value;
                }
            }
            private IPAddress fIPAddress;
        }

        public event OnResolvedHostNameHandler OnResolvedHostName;

        protected void TriggerOnResolvedHostName(String hostname, IPAddress address)
        {
            if (this.OnResolvedHostName == null)
                return;

            OnResolvedHostNameArgs lEventArgs = new OnResolvedHostNameArgs();
            lEventArgs.HostName = hostname;
            lEventArgs.IPAddress = address;
            hostname = lEventArgs.HostName;
            this.OnResolvedHostName(this, lEventArgs);
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
            if (this.fConnectionFactory != null)
                return this.fConnectionFactory.CreateClientConnection(binding);

            if (this.fConnectionClass != null)
                return (Connection)Activator.CreateInstance(this.fConnectionClass);

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
}
