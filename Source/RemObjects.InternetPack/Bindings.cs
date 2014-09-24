/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2013. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemObjects.InternetPack
{
#if FULLFRAMEWORK
    [TypeConverter(typeof(BindingConverter))]
#endif
    public class Binding
    {
        public Binding()
        {
            this.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork;
            this.Protocol = System.Net.Sockets.ProtocolType.Tcp;
        }

        public Binding(AddressFamily addressFamily)
        {
            this.AddressFamily = addressFamily;
            this.Protocol = System.Net.Sockets.ProtocolType.Tcp;
        }

        #region Properties
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

        public Int32 DefaultPort
        {
            get
            {
                return this.fDefaultPort;
            }
            set
            {
                this.fDefaultPort = value;
            }
        }
        private Int32 fDefaultPort;

        public IPAddress Address
        {
            get
            {
                return this.fAddress;
            }
            set
            {
                this.fAddress = value;
            }
        }
        private IPAddress fAddress;

        public AddressFamily AddressFamily
        {
            get
            {
                return this.fAddressFamily;
            }
            set
            {
                this.fAddressFamily = value;
            }
        }
        private AddressFamily fAddressFamily;

        public SocketType SocketType
        {
            get
            {
                return this.fSocketType;
            }
            protected set
            {
                this.fSocketType = value;
            }
        }
        private SocketType fSocketType;

        public ProtocolType Protocol
        {
            get
            {
                return this.fProtocol;
            }
            set
            {
                this.fProtocol = value;

                switch (this.Protocol)
                {
                    case ProtocolType.Tcp:
                        fSocketType = System.Net.Sockets.SocketType.Stream;
                        break;

                    case ProtocolType.Udp:
                        fSocketType = SocketType.Dgram;
                        break;
                }
            }
        }
        private ProtocolType fProtocol;
        #endregion

        public Boolean ShouldSerializePort()
        {
            return (this.Port != this.DefaultPort);
        }
    }

    public class ServerBinding : Binding
    {
        public ServerBinding()
        {
            this.fListenerThreadCount = 1;
            this.fMaxWaitConnections = 10;
            this.fEnableNagle = false;
            this.fReuseAddress = true;
            this.fEnableNagle = false;
        }

        #region Properties
        [Browsable(false)]
        public Socket ListeningSocket
        {
            get
            {
                return this.fListeningSocket;
            }
        }
        private Socket fListeningSocket;

        [Browsable(false)]
        private Boolean ReuseAddress
        {
            get
            {
                return fReuseAddress;
            }
            set
            {
                fReuseAddress = value;
            }
        }
        private Boolean fReuseAddress;

        [Browsable(false)]
        public IPEndPoint EndPoint
        {
            get
            {
                return this.fEndPoint;
            }
        }
        private IPEndPoint fEndPoint;

        [DefaultValue(10)]
        public Int32 MaxWaitConnections
        {
            get
            {
                return this.fMaxWaitConnections;
            }
            set
            {
                this.fMaxWaitConnections = value;
            }
        }
        private Int32 fMaxWaitConnections;

        [DefaultValue(1)]
        public Int32 ListenerThreadCount
        {
            get
            {
                return this.fListenerThreadCount;
            }
            set
            {
                this.fListenerThreadCount = value;
            }
        }
        private Int32 fListenerThreadCount;

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

        private Thread[] fListenThreads;

        public virtual void Bind(IListener listener)
        {
            this.fEndPoint = new IPEndPoint(this.Address, this.Port);
            this.fListeningSocket = new Socket(this.AddressFamily, this.SocketType, this.Protocol);
            if (!this.EnableNagle)
                this.fListeningSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
#if FULLFRAMEWORK
            if (this.ReuseAddress)
                this.fListeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#endif
            this.fListeningSocket.Bind(this.fEndPoint);
            this.fListeningSocket.Listen(this.MaxWaitConnections);
            this.fListenThreads = new Thread[ListenerThreadCount];

            for (Int32 i = 0; i < this.fListenThreads.Length; i++)
            {
                listener.ListeningSocket = this.fListeningSocket;
                fListenThreads[i] = new Thread(new ThreadStart(listener.Listen));
#if FULLFRAMEWORK
                fListenThreads[i].Name = String.Format("Internet Pack Listener {0} for {1}", i, this.EndPoint.ToString());
#endif
                fListenThreads[i].Start();
            }
        }

        public virtual void Unbind()
        {
            this.Unbind(true);
        }

        public virtual void Unbind(Boolean block)
        {
            if (this.fListeningSocket == null)
                return;

            this.fListeningSocket.Close();
#if FULLFRAMEWORK
            if (block && this.fListenThreads != null)
                for (Int32 i = 0; i < this.fListenThreads.Length; i++)
                    this.fListenThreads[i].Join();
#endif
        }

        public virtual void BindUnthreaded()
        {
            this.fEndPoint = new IPEndPoint(this.Address, this.Port);
            this.fListeningSocket = new Socket(this.AddressFamily, this.SocketType, this.Protocol);
            if (!this.EnableNagle)
                this.fListeningSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
#if FULLFRAMEWORK
            if (this.ReuseAddress)
                this.fListeningSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
#endif
            this.fListeningSocket.Bind(this.fEndPoint);
        }

        public virtual Connection Accept()
        {
            return new Connection(this.fListeningSocket.Accept());
        }
    }

    #region DualBinding
    internal class DualServerBinding : ServerBinding
    {
        public DualServerBinding()
            : base()
        {
            this.fBindingV6 = new ServerBinding();
        }

        #region Properties
        [Browsable(false)]
        public Socket ListeningSocketV4
        {
            get
            {
                return this.ListeningSocket;
            }
        }
        private ServerBinding fBindingV6;

        [Browsable(false)]
        public Socket ListeningSocketV6
        {
            get
            {
                return this.fBindingV6.ListeningSocket;
            }
        }

        [Browsable(false)]
        public IPEndPoint EndPointV4
        {
            get
            {
                return this.EndPoint;
            }
        }

        [Browsable(false)]
        public IPEndPoint EndPointV6
        {
            get
            {
                return this.fBindingV6.EndPoint;
            }
        }
        #endregion

        public override void Bind(IListener listener)
        {
            base.Bind(listener);
            this.fBindingV6.Bind(listener);
        }

        public override void Unbind(Boolean block)
        {
            base.Unbind(block);
            this.fBindingV6.Unbind(block);
        }

        public override void BindUnthreaded()
        {
            base.BindUnthreaded();
            this.fBindingV6.BindUnthreaded();
        }
    }
    #endregion

    #region Binding Lists
    public class Bindings
    {
    }

    public class ServerBindings : Bindings
    {
        public void Bind(Listener listener)
        {
        }

        public void Unbind(Boolean block)
        {
        }
    }
    #endregion

    #region BindingConverter class
#if FULLFRAMEWORK
    class BindingConverter : ExpandableObjectConverter
    {
        public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type type)
        {
            return base.CanConvertFrom(context, type);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
        {
            if (destinationType == typeof(String) && (value is Binding))
            {
                Binding lBinding = (Binding)value;
                return lBinding.Address.ToString() + ":" + lBinding.Port.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endif
    #endregion
}
