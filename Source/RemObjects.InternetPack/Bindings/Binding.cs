/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2014. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

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
}