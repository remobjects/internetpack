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
	#if !toffee && !cooper
	[FlagsAttribute]
	#endif
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

	// Generated from C:\Windows\Microsoft.NET\Framework\v2.0.50727\System.dll
	public class IPHostEntry : Object
	{
		public String HostName { get; set; }
		public String[] Aliases { get; set; }
		public IPAddress[] AddressList { get; set; }
		public IPHostEntry() {}
	}

	#if windows
	public struct sockaddr_in6
	{
		public rtl.USHORT sin6_family;
		public rtl.USHORT sin6_port;
		public rtl.ULONG sin6_flowinfo;
		public rtl.in6_addr sin6_addr;
		public rtl.ULONG sin6_scope_id;
	}
	#endif

	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/4.0/System.dll
	public class IPAddress : Object
	{
		private Int64 fAddress;
		private AddressFamily fFamily;
		private UInt16[] fNumbers = new UInt16[IPv6Length / 2];
		private Int64 fScopeId;
		public static IPAddress Any;
		public static IPAddress Broadcast;
		public static IPAddress Loopback;
		public static IPAddress None;
		public static IPAddress IPv6Any;
		public static IPAddress IPv6Loopback;
		public static IPAddress IPv6None;
		/*private Int32 m_HashCode;
		private static this .cctor();
		internal System.Net.IPAddress(UInt16[] address, Int64 scopeId);*/

		private const int IPv4Length =  4;
		private const int IPv6Length = 16;

		public IPAddress(Byte[] address, Int64 scopeid)
		{
			fFamily = AddressFamily.InterNetworkV6;
			for (int i = 0; i < IPv6Length / 2; i++)
				fNumbers[i] = (ushort)(address[i * 2] * 256 + address[i * 2 + 1]);

			fScopeId = scopeid;
		}

		public IPAddress(Byte[] address)
		{
			if (length(address) == IPv4Length)
			{
				fFamily = AddressFamily.InterNetwork;
				fAddress = ((address[3] << 24 | address[2] <<16 | address[1] << 8| address[0]) & 0x0FFFFFFFF);
			}
			else {
				fFamily = AddressFamily.InterNetworkV6;

				for (int i = 0; i < IPv6Length / 2; i++) {
					fNumbers[i] = (ushort)(address[i * 2] * 256 + address[i * 2 + 1]);
				}
			}
		}

		public IPAddress(Int64 newAddress)
		{
			fFamily = AddressFamily.InterNetwork;
			fAddress = newAddress;
		}

		/*private static Int16 SwapShort(Int16 number);
		private static Int32 SwapInt(Int32 number);
		private static Int64 SwapLong(Int64 number);
		public static Int64 HostToNetworkOrder(Int64 host);
		public static Int32 HostToNetworkOrder(Int32 host);
		public static Int16 HostToNetworkOrder(Int16 host);
		public static Int64 NetworkToHostOrder(Int64 network);
		public static Int32 NetworkToHostOrder(Int32 network);
		public static Int16 NetworkToHostOrder(Int16 network);*/

		public static IPAddress Parse(String ipString)
		{
			IPAddress lAddress;
			if (TryParse(ipString, out lAddress))
				return lAddress;
			else
				throw new Exception(String.Format("'{0}' is not a valid IP address", ipString));
		}

		private static Boolean TryParseIPV4(String ipString, out IPAddress address)
		{
			bool lResult = false;
			var lNumbers = ipString.Trim().Split('.');
			int? lNumber;
			byte[] lBytes = new byte[IPv4Length];

			if (lNumbers.Count == IPv4Length)
			{
				for (int i = 0; i < IPv4Length; i++)
				{
					lNumber = Convert.TryToInt32(lNumbers[i]);
					if (lNumber != null)
						lBytes[i] = lNumber;
					else
						return false;
				}
				address = new IPAddress(lBytes);
				lResult = true;
			}
			else
				lResult = false;

			return lResult;
		}

		   private static Boolean TryParseIPV6(String ipString, out IPAddress address)
		{
			var lString = (RemObjects.Elements.System.String)ipString;
			Byte[] lBytes = new Byte[16];

			#if cooper
			var lInetAddress = java.net.Inet6Address.getByName(ipString);
			lBytes = lInetAddress.getAddress();
			address = new IPAddress(lBytes, 0);
			return true;
			#else
			#if posix || toffee
			rtl.__struct_addrinfo *lAddrInfo;
			rtl.__struct_sockaddr_in6 *lSockAddr;
			var lRes = 0;
            #if toffee
			lRes = rtl.getaddrinfo(lString.UTF8String, null, null, &lAddrInfo);
			#else
			lRes = rtl.getaddrinfo((AnsiChar *)lString.FirstChar, null, null, &lAddrInfo);
			#endif
            if (lRes != 0)
				return false;
			lSockAddr = (rtl.__struct_sockaddr_in6 *)(*lAddrInfo).ai_addr;
			#else
			rtl.PADDRINFOW lAddrInfo;
			sockaddr_in6 *lSockAddr;

			if (rtl.GetAddrInfo(lString.FirstChar, null, null, &lAddrInfo) != 0)
				return false;

			lSockAddr = (sockaddr_in6 *)(*lAddrInfo).ai_addr;
			#endif

			for (int i = 0; i < IPv6Length; i++)
				#if posix
				lBytes[i] = (*lSockAddr).sin6_addr.__in6_u.__u6_addr8[i] = lBytes[i];
				#elif toffee
				lBytes[i] = (*lSockAddr).sin6_addr.__u6_addr.__u6_addr8[i] = lBytes[i];
				#else
				lBytes[i] = (*lSockAddr).sin6_addr.u.Byte[i];
				#endif

			address = new IPAddress(lBytes, (*lSockAddr).sin6_scope_id);
			return true;
			#endif
		}

		public static Boolean TryParse(String ipString, out IPAddress address)
		{
			if (ipString.Contains(':'))
				return TryParseIPV6(ipString, out address);
			else
				return TryParseIPV4(ipString, out address);
		}

		private static IPAddress ParseIPV4(String ip)
		{
			IPAddress lResult;

			if (TryParseIPV4(ip, out lResult))
				return lResult;
			else
				throw new Exception(String.Format("Can not parse '{0}' as IPv4 address", ip));
		}

		private static IPAddress ParseIPV6(String ip)
		{
			IPAddress lResult;

			if (TryParseIPV6(ip, out lResult))
				return lResult;
			else
				throw new Exception(String.Format("Can not parse '{0}' as IPv6 address", ip));
		}

		public Byte[] GetAddressBytes()
		{
			Byte[] lBytes;
			if (AddressFamily == AddressFamily.InterNetworkV6 )
			{
				lBytes = new Byte[IPv6Length];

				int j = 0;
				for ( int i = 0; i < IPv6Length / 2; i++)
				{
					lBytes[j++] = (Byte)((fNumbers[i] >> 8) & 0xFF);
					lBytes[j++] = (Byte)((fNumbers[i]) & 0xFF);
				}
			}
			else
			{
				lBytes = new Byte[IPv4Length];
				lBytes[0] = (Byte)(Address);
				lBytes[1] = (Byte)(Address >> 8);
				lBytes[2] = (Byte)(Address >> 16);
				lBytes[3] = (Byte)(Address >> 24);
			}
			return lBytes;
		}

		/*public static Boolean IsLoopback(IPAddress address);
		private static String ToString(Int64 addr);
		public override String ToString();
		public override Boolean Equals(Object comparand);
		public override Int32 GetHashCode();
		private static Int32 Hash(Int32 i, Int32 j, Int32 k, Int32 l);
		[ObsoleteAttribute("This property is obsolete. Use GetAddressBytes.")]
		*/
		public Int64 Address
		{
			get
			{
				return fAddress;
			}
			set
			{
				fAddress = value;
			}
		 }

		//internal Int64 InternalIPv4Address { get; set; }
		public Boolean IsIPv6LinkLocal { get; set; }
		public Boolean IsIPv6SiteLocal { get; set; }
		public Boolean IsIPv6Multicast { get; set; }
		public Boolean IsIPv6Teredo { get; set; }
		public Int64 ScopeId
		{
			get
			{
				return fScopeId;
			}
			set
			{
				fScopeId = value;
			}
		}

		public AddressFamily AddressFamily
		{
			get
			{
				return fFamily;
			}
			set
			{
				fFamily = value;
			}
		}
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
		public Int32 Port { get; set; }
		public const Int32 MaxPort = 65535;
		public const Int32 MinPort = 0;

		public IPEndPoint(Int64 address, Int32 port)
		{
			Address = new IPAddress(address);
			Port = port;
		}

		public IPEndPoint(IPAddress address, Int32 port)
		{
			Address = address;
			Port = port;
		}

		/*public override EndPoint Create(SocketAddress socketAddress);
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
		Boolean CompletedSynchronously { get; }
		Boolean IsCompleted { get; }
	}

	public class AsyncResult: IAsyncResult
	{
		public Object AsyncState { get; set; }
		public WaitHandle AsyncWaitHandle { get; set; }
		public Boolean CompletedSynchronously { get; set; }
		public Boolean IsCompleted { get; set; }
		public Exception DelayedException { get; set; }
		public Object Data { get; set; }
		public Socket AcceptSocket { get; set; }
		public int NBytes {get; set; }
		public byte[] Buffer { get; set; }
		public Socket AcceptedSocket { get; set; }
		public SocketError Error { get; set; }

		public AsyncResult(Object AnAsyncState)
		{
			AsyncState = AnAsyncState;
			AsyncWaitHandle = new EventWaitHandle();
		}
	}

	public enum HttpStatusCode
	{
		Continue = 100,
		SwitchingProtocols = 101,
		OK = 200,
		Created = 201,
		Accepted = 202,
		NonAuthoritativeInformation = 203,
		NoContent = 204,
		ResetContent = 205,
		PartialContent = 206,
		MultipleChoices = 300,
		Ambiguous = 300,
		MovedPermanently = 301,
		Moved = 301,
		Found = 302,
		Redirect = 302,
		SeeOther = 303,
		RedirectMethod = 303,
		NotModified = 304,
		UseProxy = 305,
		Unused = 306,
		TemporaryRedirect = 307,
		RedirectKeepVerb = 307,
		BadRequest = 400,
		Unauthorized = 401,
		PaymentRequired = 402,
		Forbidden = 403,
		NotFound = 404,
		MethodNotAllowed = 405,
		NotAcceptable = 406,
		ProxyAuthenticationRequired = 407,
		RequestTimeout = 408,
		Conflict = 409,
		Gone = 410,
		LengthRequired = 411,
		PreconditionFailed = 412,
		RequestEntityTooLarge = 413,
		RequestUriTooLong = 414,
		UnsupportedMediaType = 415,
		RequestedRangeNotSatisfiable = 416,
		ExpectationFailed = 417,
		InternalServerError = 500,
		NotImplemented = 501,
		BadGateway = 502,
		ServiceUnavailable = 503,
		GatewayTimeout = 504,
		HttpVersionNotSupported = 505
	}

	public enum SocketError
	{
		Success = 0,
		SocketError = -1,
		Interrupted = 10004,
		AccessDenied = 10013,
		Fault = 10014,
		InvalidArgument = 10022,
		TooManyOpenSockets = 10024,
		WouldBlock = 10035,
		InProgress = 10036,
		AlreadyInProgress = 10037,
		NotSocket = 10038,
		DestinationAddressRequired = 10039,
		MessageSize = 10040,
		ProtocolType = 10041,
		ProtocolOption = 10042,
		ProtocolNotSupported = 10043,
		SocketNotSupported = 10044,
		OperationNotSupported = 10045,
		ProtocolFamilyNotSupported = 10046,
		AddressFamilyNotSupported = 10047,
		AddressAlreadyInUse = 10048,
		AddressNotAvailable = 10049,
		NetworkDown = 10050,
		NetworkUnreachable = 10051,
		NetworkReset = 10052,
		ConnectionAborted = 10053,
		ConnectionReset = 10054,
		NoBufferSpaceAvailable = 10055,
		IsConnected = 10056,
		NotConnected = 10057,
		Shutdown = 10058,
		TimedOut = 10060,
		ConnectionRefused = 10061,
		HostDown = 10064,
		HostUnreachable = 10065,
		ProcessLimit = 10067,
		SystemNotReady = 10091,
		VersionNotSupported = 10092,
		NotInitialized = 10093,
		Disconnecting = 10101,
		TypeNotFound = 10109,
		HostNotFound = 11001,
		TryAgain = 11002,
		NoRecovery = 11003,
		NoData = 11004,
		IOPending = 997,
		OperationAborted = 995,
		NoValue = -999
	}

    public class ContentDisposition
    {
        private String fDisposition;
        private Dictionary<String, String> fParameters = new Dictionary<String, String>();
        private readonly DateTime MinDate = new DateTime(1, 1, 1, 0, 0, 1);
        private const string RFC822Format = "dd MMM yyyy HH':'mm':'ss zz00";

        public ContentDisposition(String disposition)
        {
            if (disposition.IndexOf(';') < 0)
                fDisposition = disposition.Trim();
            else
            {
                var lParts = disposition.Split(';');
				fDisposition = lParts[0].Trim();
				for (int i = 1; i < lParts.Count; i++)
				{
                    var lValues = lParts[i].Split('=');
			        if (lValues.Count == 2)
				        fParameters.Add(lValues[0].Trim(), lValues[1].Trim());
			        else
				        throw new Exception("Wrong format");
                }
            }
        }

        public ContentDisposition(): this("attachment")
        {
            
        }

	    [ToString]
        public override String ToString() 
        {
            StringBuilder lSb = new StringBuilder();
			lSb.Append(DispositionType.ToLower());
			
			foreach(var lItem in fParameters)
                {
                    if (lItem.Value.Length > 0)
                    {
                        lSb.Append ("; " + lItem.Key + "=");

                        if ((lItem.Key == "filename" && lItem.Value.IndexOf(' ') >= 0) || lItem.Key.EndsWith("date"))
                            lSb.Append("\"" + lItem.Value + "\"");
                        else
                            lSb.Append(lItem.Value);
                    }
		    }
			return lSb.ToString();
		}        

	    public String DispositionType { get { return fDisposition; } set { fDisposition = value; }  }

	    public Dictionary<String, String> Parameters { get { return fParameters; } }

	    public String FileName { get { return fParameters["filename"]; } set { fParameters["filename"] = value; } }

	    private DateTime ParseRFC822Date(String aValue)
        {
            return MinDate; 
        }
                                
        public DateTime CreationDate             
        {
            get
            {
				var lDate = fParameters["creation-date"];
                if (lDate != "")
					return ParseRFC822Date(lDate);
				else
					return MinDate;
			}
			
            set
            {
				if (value > MinDate)
					fParameters["creation-date"] = value.ToString(RFC822Format);
				else
					fParameters.Remove("modification-date");
			}
        }

	    public DateTime ModificationDate
        {
            get
            {
				var lDate = fParameters["modification-date"];
                if (lDate != "")
					return ParseRFC822Date(lDate);
				else
					return MinDate;
			}

			set
            {
				if (value > MinDate)
					fParameters["modification-date"] = value.ToString(RFC822Format);
				else
					fParameters.Remove ("modification-date");
			}            
        }

	    public bool Inline
        {
            get
            {
                return fDisposition.CompareToIgnoreCase("inline") == 0;
            }

			set
            {
				if (value)
					fDisposition = "inline";
				else
					fDisposition = "attachment";
			}
        }

	    public DateTime ReadDate
        {
            get
            {
                var lDate = fParameters["read-date"];
                if (lDate != "")
					return ParseRFC822Date(lDate);
                else
                    return MinDate;
			}
			
            set
            {
				if (value > MinDate)
					fParameters["read-date"] = value.ToString(RFC822Format);
				else
					fParameters.Remove("read-date");
			}
        }

	    public long Size
        {
            get
            {
				var lSize = fParameters["size"];
                if (lSize != "")
					return Convert.ToInt64(lSize);
				else
					return -1;
			}
			
            set
            {
				if (value > -1)
					fParameters["size"] = value.ToString();
				else
					fParameters.Remove("size");
			}
        }
    }

    public enum MailPriority
    {
        Low = 1,
        Normal = 3,
        High = 5
    }

    public class ContentType
    {
	    // TODO
        public ContentType(String contentType) { }
	    public ContentType() { }
	    [ToString]
        public override String ToString() { }
	    public String Boundary { get; set; }
	    public String CharSet { get; set; }
	    public String MediaType { get; set; }
	    public String Name { get; set; }
	    public Dictionary<String, String> Parameters { get; set; }
    }

	#endif
}