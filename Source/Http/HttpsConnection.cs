/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
	public class HttpsConnectionFactory : SslConnectionFactory
	{
		public HttpsConnectionFactory(HttpProxySettings proxySettings)
		{
			this.ProxySettings = proxySettings;
			this.TargetPort = 8099;
		}

		[Category("Ssl Options")]
		[DefaultValue(8099)]
		public Int32 TargetPort { get; set; }

		[Browsable(false)]
		public HttpProxySettings ProxySettings { get; private set; }

		public override Connection CreateClientConnection(Binding binding)
		{
			if (!this.Enabled)
			{
				return new Connection(binding);
			}

			if (this.IsCertificateLoadPending)
			{
				this.LoadCertificate();
			}

			return new HttpSslConnection(this, binding);
		}

		public override Connection CreateClientConnection(Connection connection)
		{
			if (this.IsCertificateLoadPending)
			{
				this.LoadCertificate();
			}

			return new HttpSslConnection(this, connection);
		}
	}
}