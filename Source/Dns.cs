/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

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
			return lAddresses[lRandom.Next(lAddresses.Length)];
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
            #if posix || macos || ios            
            rtl.__struct_addrinfo *lAddrInfo;
            rtl.__struct_sockaddr_in6 *lSockAddr;
            #if macos || ios
            if (rtl.getaddrinfo(lString.UTF8String, null, null, &lAddrInfo) != 0)
            #else
            if (rtl.getaddrinfo((AnsiChar *)lString.FirstChar, null, null, &lAddrInfo) != 0)
            #endif
                /*return false*/;
            lSockAddr = (rtl.__struct_sockaddr_in6 *)(*lAddrInfo).ai_addr;
            #else
            rtl.PADDRINFOW lAddrInfo;
            sockaddr_in6 *lSockAddr;
            
            if (rtl.GetAddrInfo(lString.FirstChar, null, null, &lAddrInfo) != 0)
                /*return false*/;

            lSockAddr = (sockaddr_in6 *)(*lAddrInfo).ai_addr;
            #endif

            for (int i = 0; i < 16 /*IPv6Length*/; i++)
                #if posix
                lBytes[i] = (*lSockAddr).sin6_addr.__in6_u.__u6_addr8[i];
                #elif macos || ios
                lBytes[i] = (*lSockAddr).sin6_addr.__u6_addr.__u6_addr8[i];
                #else
                lBytes[i] = (*lSockAddr).sin6_addr.u.Byte[i];
                #endif

            /*
            address = new IPAddress(lBytes, (*lSockAddr).sin6_scope_id);
            return true;
            #endif
            */


            #endif
            // TODO other platforms!!!!
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