namespace RemObjects.InternetPack
{
	#if !ECHOES
	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public enum AddressFamily
	{
		Unknown = -1,
		Unspecified = 0,
		Unix = 1,
		InterNetwork = 2,
		ImpLink = 3,
		Pup = 4,
		Chaos = 5,
		NS = 6,
		Ipx = 6,
		Iso = 7,
		Osi = 7,
		Ecma = 8,
		DataKit = 9,
		Ccitt = 10,
		Sna = 11,
		DecNet = 12,
		DataLink = 13,
		Lat = 14,
		HyperChannel = 15,
		AppleTalk = 16,
		NetBios = 17,
		VoiceView = 18,
		FireFox = 19,
		Banyan = 21,
		Atm = 22,
		InterNetworkV6 = 23,
		Cluster = 24,
		Ieee12844 = 25,
		Irda = 26,
		NetworkDesigners = 28,
		Max = 29
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public enum ProtocolType
	{
		IP = 0,
		Icmp = 1,
		Igmp = 2,
		Ggp = 3,
		Tcp = 6,
		Pup = 12,
		Udp = 17,
		Idp = 22,
		IPv6 = 41,
		ND = 77,
		Raw = 255,
		Unspecified = 0,
		Ipx = 1000,
		Spx = 1256,
		SpxII = 1257,
		Unknown = -1,
		IPv4 = 4,
		IPv6RoutingHeader = 43,
		IPv6FragmentHeader = 44,
		IPSecEncapsulatingSecurityPayload = 50,
		IPSecAuthenticationHeader = 51,
		IcmpV6 = 58,
		IPv6NoNextHeader = 59,
		IPv6DestinationOptions = 60,
		IPv6HopByHopOptions = 0
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public enum SocketType
	{
		Stream = 1,
		Dgram = 2,
		Raw = 3,
		Rdm = 4,
		Seqpacket = 5,
		Unknown = -1
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	[FlagsAttribute]
	public enum SocketFlags
	{
		None = 0,
		OutOfBand = 1,
		Peek = 2,
		DontRoute = 4,
		MaxIOVectorLength = 16,
		Truncated = 256,
		ControlDataTruncated = 512,
		Broadcast = 1024,
		Multicast = 2048,
		Partial = 32768
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/System.dll
	public enum SocketOptionLevel
	{
		Socket = 65535,
		IP = 0,
		IPv6 = 41,
		Tcp = 6,
		Udp = 17
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/System.dll
	public enum SocketOptionName
	{
		Debug = 1,
		AcceptConnection = 2,
		ReuseAddress = 4,
		KeepAlive = 8,
		DontRoute = 16,
		Broadcast = 32,
		UseLoopback = 64,
		Linger = 128,
		OutOfBandInline = 256,
		DontLinger = -129,
		ExclusiveAddressUse = -5,
		SendBuffer = 4097,
		ReceiveBuffer = 4098,
		SendLowWater = 4099,
		ReceiveLowWater = 4100,
		SendTimeout = 4101,
		ReceiveTimeout = 4102,
		Error = 4103,
		Type = 4104,
		MaxConnections = 2147483647,
		IPOptions = 1,
		HeaderIncluded = 2,
		TypeOfService = 3,
		IpTimeToLive = 4,
		MulticastInterface = 9,
		MulticastTimeToLive = 10,
		MulticastLoopback = 11,
		AddMembership = 12,
		DropMembership = 13,
		DontFragment = 14,
		AddSourceMembership = 15,
		DropSourceMembership = 16,
		BlockSource = 17,
		UnblockSource = 18,
		PacketInformation = 19,
		NoDelay = 1,
		BsdUrgent = 2,
		Expedited = 2,
		NoChecksum = 1,
		ChecksumCoverage = 20,
		HopLimit = 21,
		UpdateAcceptContext = 28683,
		UpdateConnectContext = 28688
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/System.dll
	public enum SocketShutdown
	{
		Receive = 0,
		Send = 1,
		Both = 2
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/System.dll
	public class IPAddress : Object
	{
		/*private Int64 m_Address;
		private AddressFamily m_Family;
		private UInt16[] m_Numbers;
		private Int64 m_ScopeId;
		public static IPAddress Any;
		public static IPAddress Broadcast;
		public static IPAddress Loopback;
		public static IPAddress None;
		public static IPAddress IPv6Any;
		public static IPAddress IPv6Loopback;
		public static IPAddress IPv6None;
		private Int32 m_HashCode;
		private static this .cctor();
		internal System.Net.IPAddress(UInt16[] address, Int64 scopeId);
		public System.Net.IPAddress(Byte[] address, Int64 scopeid);
		public System.Net.IPAddress(Byte[] address);
		public System.Net.IPAddress(Int64 newAddress);
		private static Int16 SwapShort(Int16 number);
		private static Int32 SwapInt(Int32 number);
		private static Int64 SwapLong(Int64 number);
		public static Int64 HostToNetworkOrder(Int64 host);
		public static Int32 HostToNetworkOrder(Int32 host);
		public static Int16 HostToNetworkOrder(Int16 host);
		public static Int64 NetworkToHostOrder(Int64 network);
		public static Int32 NetworkToHostOrder(Int32 network);
		public static Int16 NetworkToHostOrder(Int16 network);*/
		public static IPAddress Parse(String ipString) {}
		/*public static Boolean TryParse(String ipString, out IPAddress address);
		private static IPAddress ParseIPV4(String ip);
		private static IPAddress ParseIPV6(String ip);
		public Byte[] GetAddressBytes();
		public static Boolean IsLoopback(IPAddress address);
		private static String ToString(Int64 addr);
		public override String ToString();
		public override Boolean Equals(Object comparand);
		public override Int32 GetHashCode();
		private static Int32 Hash(Int32 i, Int32 j, Int32 k, Int32 l);
		[ObsoleteAttribute("This property is obsolete. Use GetAddressBytes.")]
		public Int64 Address { get; set; }*/
		//internal Int64 InternalIPv4Address { get; set; }
		public Boolean IsIPv6LinkLocal { get; set; }
		public Boolean IsIPv6SiteLocal { get; set; }
		public Boolean IsIPv6Multicast { get; set; }
		public Boolean IsIPv6Teredo { get; set; }
		public Int64 ScopeId { get; set; }
		public AddressFamily AddressFamily { get; set; }
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public abstract class EndPoint : Object
	{
		/*protected System.Net.EndPoint();
		public virtual EndPoint Create(SocketAddress socketAddress);
		public virtual SocketAddress Serialize();
		private static Exception NotImplemented();*/
		public virtual AddressFamily AddressFamily { get; set; }
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public class IPEndPoint : EndPoint
	{
		public IPAddress Address { get; set; }
		/*private IPAddress address;*/
		public Int32 Port { get; set; }
		/*private Int32 port;
		public static const Int32 MaxPort = 65535;
		public static const Int32 MinPort = 0;
		public System.Net.IPEndPoint(Int64 address, Int32 port);
		public System.Net.IPEndPoint(IPAddress address, Int32 port);
		public override EndPoint Create(SocketAddress socketAddress);
		public override SocketAddress Serialize();
		public override String ToString();
		public override Boolean Equals(Object comparand);
		public override Int32 GetHashCode();
		public override AddressFamily AddressFamily { get; set; }*/
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public class LingerOption : Object
	{
		public LingerOption(Boolean enable, Int32 seconds)
		{
			Enabled = enable;
			LingerTime = seconds;
		}
		public Boolean Enabled { get; private set; }
		public Int32 LingerTime { get; private set; }
	}

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/mscorlib.dll
	//[ComVisibleAttribute(true)]
	public interface IAsyncResult
	{
		Object AsyncState { get; }
		WaitHandle AsyncWaitHandle { get; }
		Boolean CompletedSynchronously { get;}
		Boolean IsCompleted { get; }
	}

	#endif
}