/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack
{
	public static class CertificateBuilderFactory
	{
		public static ICertificateBuilder Create()
		{
			return CertificateBuilderFactory.Create("SHA1");
		}

		public static ICertificateBuilder Create(String hashAlgorithm)
		{
			return Environment.OSVersion.Platform == PlatformID.Win32NT
				? (ICertificateBuilder)new NetCertificateBuilder(hashAlgorithm)
				: new MonoCertificateBuilder(new MonoSecurityTypeProvider(), hashAlgorithm);
		}
	}
}