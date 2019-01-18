/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/
using RemObjects.Elements.RTL;

namespace RemObjects.InternetPack.Dns
{
	public static class DnsLookup
	{
		public static IPAddress ResolveFirst(String hostname)
		{
			IPAddress[] lAddresses = DnsLookup.ResolveAll(hostname);

			if (lAddresses.Length == 0)
			{
				throw new DnsResolveException("Could not resolve HostName {0}", hostname);
			}

			// Try to resolve as IPv4 first
			for (Int32 i = 0; i < lAddresses.Length - 1; i++)
			{
				if (lAddresses[i].AddressFamily == AddressFamily.InterNetwork)
				{
					return lAddresses[i];
				}
			}

			return lAddresses[0];
		}

		#if !NEEDS_PORTING
		public static IPAddress ResolveRandom(String hostname)
		{
			IPAddress[] lAddresses = ResolveAll(hostname);
			if (lAddresses.Length == 0)
			{
				throw new DnsResolveException("Could not resolve hostname {0}", hostname);
			}

			Random lRandom = new Random();
			return lAddresses[lRandom.NextInt(lAddresses.Length)];
		}
		#endif

		#if posix || toffee || darwin
		private rtl.__struct_addrinfo *GetNext(rtl.__struct_addrinfo *Addr)
		{
			return (rtl.__struct_addrinfo *)Addr->ai_next;
		}
		#elif island
		private rtl.ADDRINFOW *GetNext(rtl.ADDRINFOW *Addr)
		{
			return  (rtl.ADDRINFOW*)Addr->ai_next;
		}
		#endif

		public static IPAddress[] ResolveAll(String hostname)
		{
			IPAddress lAddress = TryStringAsIPAddress(hostname);

			if (lAddress != null)
			{
				return new IPAddress[] { lAddress };
			}

			#if echoes
			IPHostEntry lEntry = System.Net.Dns.GetHostEntry(hostname);
			return lEntry.AddressList;
			#elif cooper
			var lTemp = java.net.InetAddress.getAllByName(hostname);
			var lAddresses = new IPAddress[lTemp.length];
			for (int i = 0; i < lTemp.length; i++)
			{
				lAddresses[i] = new IPAddress(lTemp[i].Address);
			}
			return lAddresses;
			#else
			var lString = (RemObjects.Elements.System.String)hostname;
			Byte[] lBytes = new Byte[16];
			Byte[] lBytesIPv4 = new Byte[4];
			var lAll = new List<IPAddress>();

			#if island && windows
			rtl.ADDRINFOW *lAddrInfo;
			rtl.ADDRINFOW *lPtr;
			rtl.SOCKADDR_IN *lSockAddrIPv4;
			sockaddr_in6 *lSockAddr;
			#elif posix || toffee || darwin
			rtl.__struct_addrinfo *lAddrInfo;
			rtl.__struct_addrinfo *lPtr;
			rtl.__struct_sockaddr_in *lSockAddrIPv4;
			rtl.__struct_sockaddr_in6 *lSockAddr;
			#endif

			var lRes = 0;
			#if toffee
			lRes = rtl.getaddrinfo(lString.UTF8String, null, null, &lAddrInfo);
			#elif posix || darwin
			AnsiChar[] lHost = lString.ToAnsiChars(true);
			lRes = rtl.getaddrinfo(&lHost[0], null, null, &lAddrInfo);
			#elif island
			char[] lHost = lString.ToCharArray(true);
			rtl.WSADATA data;
			rtl.WSAStartup(rtl.WINSOCK_VERSION, &data);
			lRes = rtl.GetAddrInfoW(&lHost[0], null, null, &lAddrInfo);
			#endif
			if (lRes == 0)
			{
				for(lPtr = lAddrInfo; lPtr != null; lPtr = GetNext(lPtr))
				{
					switch(lPtr->ai_family)
					{
						case AddressFamily.InterNetwork:
							#if posix || toffee || darwin
							lSockAddrIPv4 = (rtl.__struct_sockaddr_in *)(*lPtr).ai_addr;
							lBytesIPv4[0] = (Byte)((*lSockAddrIPv4).sin_addr.s_addr);
							lBytesIPv4[1] = (Byte)((*lSockAddrIPv4).sin_addr.s_addr >> 8);
							lBytesIPv4[2] = (Byte)((*lSockAddrIPv4).sin_addr.s_addr >> 16);
							lBytesIPv4[3] = (Byte)((*lSockAddrIPv4).sin_addr.s_addr >> 24);
							#elif island && windows
							lSockAddrIPv4 = (rtl.SOCKADDR_IN *)(*lPtr).ai_addr;
							lBytesIPv4[0] = (*lSockAddrIPv4).sin_addr.S_un.S_un_b.s_b1;
							lBytesIPv4[1] = (*lSockAddrIPv4).sin_addr.S_un.S_un_b.s_b2;
							lBytesIPv4[2] = (*lSockAddrIPv4).sin_addr.S_un.S_un_b.s_b3;
							lBytesIPv4[3] = (*lSockAddrIPv4).sin_addr.S_un.S_un_b.s_b4;
							#endif
							lAll.Add(new IPAddress(lBytesIPv4));
							break;

						case AddressFamily.InterNetworkV6:
							#if posix || toffee
							lSockAddr = (rtl.__struct_sockaddr_in6 *)(*lPtr).ai_addr;
							#elif island && windows
							lSockAddr = (sockaddr_in6 *)(*lPtr).ai_addr;
							#endif
							for (int i = 0; i < 16 /*IPv6Length*/; i++)
								#if posix
								lBytes[i] = (*lSockAddr).sin6_addr.__in6_u.__u6_addr8[i];
								#elif toffee && !darwin
								//lBytes[i] = (*lSockAddr).sin6_addr.__u6_addr.__u6_addr8[i];
								#elif darwin
								lBytes[i] = (*lSockAddr).sin6_addr.__u6_addr.__u6_addr8[i];
								#else
								lBytes[i] = (*lSockAddr).sin6_addr.u.Byte[i];
								#endif

							var lNewAddress = new IPAddress(lBytes, (*lSockAddr).sin6_scope_id);
							lAll.Add(lNewAddress);
							break;
					}

				}
				return lAll.ToArray();
			}
			else
				return null;
			#endif
		}

		public static IPAddress TryStringAsIPAddress(String hostname)
		{
			if (hostname.StartsWith("[")) // ipv6
			{
				try
				{
					return IPAddress.Parse(hostname);
				}
				catch (FormatException)
				{
					return null;
				}
			}

			var lFields = hostname.Split('.');
			if (lFields.Count != 4)
			{
				return null;
			}

			for (Int32 i = 0; i < 4; i++)
			{
				if (lFields[i].Length > 3)
				{
					return null;
				}

				for (Int32 j = 0; j < lFields[i].Length; j++)
				{
					if (lFields[i][j] < '0' && lFields[i][j] > '9')
					{
						return null;
					}
				}
			}
			try
			{
				return IPAddress.Parse(hostname);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		#if !NEEDS_PORTING
		public static String ReverseResolve(IPAddress address)
		{
			IPHostEntry lEntry = System.Net.Dns.GetHostEntry(address);
			return lEntry.HostName;
		}
		#endif
	}
}