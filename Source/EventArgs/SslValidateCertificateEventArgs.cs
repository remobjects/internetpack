/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

#if ECHOES
using System.Security.Cryptography.X509Certificates;
#endif

namespace RemObjects.InternetPack
{
	public class SslValidateCertificateEventArgs : EventArgs
	{
		public SslValidateCertificateEventArgs(X509Certificate certificate)
		{
			this.Certificate = certificate;
			this.Cancel = false;
		}

		public X509Certificate Certificate { get; private set; }

		public Boolean Cancel { get; set; }
	}
}