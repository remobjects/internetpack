/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack
{
	public interface ICertificateBuilder
	{
		Byte[] Export(String subject, String issuer, String password, Boolean isServer);
	}
}
