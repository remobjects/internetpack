/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
	#if FULLFRAMEWORK
	[TypeConverter(typeof(ExpandableObjectConverter))]
	#endif
	public sealed class HttpProxySettings
	{
		private const String DEFAULT_HOSTNAME = @"192.168.1.1";
		private const Int32 DEFAULT_PORT = 3128;

		public HttpProxySettings()
		{
			this.UseProxy = false;
			this.ProxyHost = HttpProxySettings.DEFAULT_HOSTNAME;
			this.ProxyPort = HttpProxySettings.DEFAULT_PORT;
			this.UserName = String.Empty;
			this.Password = String.Empty;
		}

		[DefaultValue(false)]
		public Boolean UseProxy { get; set; }

		[DefaultValue(HttpProxySettings.DEFAULT_HOSTNAME)]
		public String ProxyHost { get; set; }

		[DefaultValue(HttpProxySettings.DEFAULT_PORT)]
		public Int32 ProxyPort { get; set; }

		[DefaultValue("")]
		public String UserName { get; set; }

		[DefaultValue("")]
		public String Password { get; set; }

		public void CopyProperties(HttpProxySettings source)
		{
			this.UseProxy = source.UseProxy;
			this.ProxyHost = source.ProxyHost;
			this.ProxyPort = source.ProxyPort;
			this.UserName = source.UserName;
			this.Password = source.Password;
		}
	}
}