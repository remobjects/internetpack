/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Dns
{
#if FULLFRAMEWORK
	[System.Serializable()]
#endif
	public class DnsResolveException : Exception
	{
		public DnsResolveException(String message)
			: base(message)
		{
		}

		public DnsResolveException(String message, params Object[] args)
			: this(String.Format(message, args))
		{
		}
	}
}