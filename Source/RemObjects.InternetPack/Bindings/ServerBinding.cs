/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2015. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace RemObjects.InternetPack
{
	public class ServerBinding : Binding
	{
		private const Int32 MAX_WAIT_CONNECTIONS = 50;

		public ServerBinding()
		{
			this.ListenerThreadCount = 1;
			this.MaxWaitConnections = MAX_WAIT_CONNECTIONS;
			this.EnableNagle = false;
			this.ReuseAddress = true;
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
		public Boolean ReuseAddress
		{
			get
			{
				return this.fReuseAddress;
			}
			set
			{
				this.fReuseAddress = value;
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

		[DefaultValue(MAX_WAIT_CONNECTIONS)]
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
				fListenThreads[i] = new Thread(listener.Listen);
#if FULLFRAMEWORK
				fListenThreads[i].Name = String.Format("Internet Pack Listener {0} for {1}", i, this.EndPoint);
#endif
				fListenThreads[i].Start();
			}
		}

		public void Unbind()
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
}