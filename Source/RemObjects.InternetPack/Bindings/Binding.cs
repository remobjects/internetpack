/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

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
		public Int32 Port { get; set; }

		public Int32 DefaultPort { get; set; }

		public IPAddress Address { get; set; }

		public AddressFamily AddressFamily { get; set; }

		public SocketType SocketType { get; protected set; }

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
						this.SocketType = System.Net.Sockets.SocketType.Stream;
						break;

					case ProtocolType.Udp:
						this.SocketType = SocketType.Dgram;
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