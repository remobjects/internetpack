/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack
{
	abstract class CertificateBuilder : ICertificateBuilder
	{
		protected CertificateBuilder()
		{
		}

		protected DateTime GetCertificateStartDate()
		{
			return DateTime.UtcNow.AddSeconds(-1);
		}

		protected DateTime GetCertificateEndDate()
		{
			return DateTime.UtcNow.AddYears(10);
		}

		protected String GetCertificatePurpose(Boolean isServer)
		{
			return isServer ? "1.3.6.1.5.5.7.3.1" : "1.3.6.1.5.5.7.3.2";
		}

		public abstract Byte[] Export(String subject, String issuer, String password, Boolean isServer);
	}
}