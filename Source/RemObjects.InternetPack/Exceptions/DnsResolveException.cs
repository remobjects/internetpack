/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2015. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com
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