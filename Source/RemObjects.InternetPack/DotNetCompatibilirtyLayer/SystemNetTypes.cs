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

	public class IPAddress {
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
		Object AsyncState { get; set; }
		WaitHandle AsyncWaitHandle { get; set; }
		Boolean CompletedSynchronously { get; set; }
		Boolean IsCompleted { get; set; }
	}

	#endif
}