/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;

namespace RemObjects.InternetPack.Http
{
#if DESIGN
	[System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Client), "Glyphs.HttpClient.bmp")]
#endif
	public class HttpClient : Client
	{
		public HttpClient()
		{
			this.Version = DEFAULT_HTTP_VERSION;
			this.UserAgent = DEFAULT_HTTP_USERAGENT;
			this.Accept = DEFAULT_HTTP_ACCEPT;
			this.Timeout = DEFAULT_TIMEOUT;
			this.TimeoutEnabled = true;
			this.KeepAlive = true;
			this.fProxySettings = new HttpProxySettings();
#if FULLFRAMEWORK
			this.SslOptions = new HttpsConnectionFactory(this.fProxySettings);
#endif
			this.UserName = "";
			this.Password = "";
		}

		private const String DEFAULT_HTTP_VERSION = "1.1";
		private const String DEFAULT_HTTP_USERAGENT = "RemObjects Internet Pack HTTP Client";
		private const String DEFAULT_HTTP_ACCEPT = "text/html, image/gif, image/jpeg, image/png, */*";
		private const Int32 DEFAULT_TIMEOUT = 1 * 60;  /* 1 minute */
		private const String CHARSET_KEY = "charset=";

		private Connection fConnection;
		private String fConnectionUrl;

		#region Properties
		[Category("Http")]
		[DefaultValue(DEFAULT_HTTP_VERSION)]
		public String Version { get; set; }

		[Category("Http")]
		[DefaultValue(true)]
		public Boolean KeepAlive
		{
			get
			{
				return this.fKeepAlive;
			}
			set
			{
				if (this.UseConnectionPooling && !value)
				{
					this.UseConnectionPooling = false;
				}

				this.fKeepAlive = value;

				if (!this.fKeepAlive)
				{
					this.DisposeHttpConnection();
				}
			}
		}
		private Boolean fKeepAlive;

		[Browsable(false)]
		public ConnectionPool CustomConnectionPool
		{
			get
			{
				return this.ConnectionPool;
			}
			set
			{
				this.ConnectionPool = value;

				if (!KeepAlive && value != null)
				{
					KeepAlive = true;
				}
			}
		}

		[Category("Http")]
		[DefaultValue(false)]
		public Boolean UseConnectionPooling
		{
			get
			{
				return this.ConnectionPool != null;
			}
			set
			{
				if (!value)
				{
					this.ConnectionPool = null;
					return;
				}

				if (this.ConnectionPool == null)
				{
					this.ConnectionPool = DefaultPool.ConnectionPool;
				}

				if (!this.KeepAlive)
				{
					this.KeepAlive = true;
				}
			}
		}

		[Category("Http")]
		[DefaultValue(DEFAULT_HTTP_USERAGENT)]
		public String UserAgent { get; set; }

		[Category("Http")]
		[DefaultValue(DEFAULT_HTTP_ACCEPT)]
		public String Accept { get; set; }

		[Category("Http")]
		[DefaultValue(DEFAULT_TIMEOUT)]
		public Int32 Timeout { get; set; }

		[Category("Http")]
		[DefaultValue(true)]
		public Boolean TimeoutEnabled { get; set; }

		[Category("Basic authentication")]
		public String UserName { get; set; }

		[Category("Basic authentication")]
		public String Password { get; set; }

#if FULLFRAMEWORK
		[Category("Http Proxy settings")]
		[Browsable(true)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#else
        [Browsable(false)]
#endif
		public HttpProxySettings ProxySettings
		{
			get
			{
				return this.fProxySettings;
			}
		}
		private readonly HttpProxySettings fProxySettings;
		#endregion

		#region Methods
		public String Get(String url)
		{
			return Get(url, null);
		}

		private static System.Text.Encoding GetEncodingFromContentType(String contentType)
		{
			Int32 lStartPos = contentType.IndexOf(HttpClient.CHARSET_KEY, StringComparison.Ordinal);
			if (lStartPos == -1)
				return System.Text.Encoding.ASCII;

			lStartPos += HttpClient.CHARSET_KEY.Length;
			Int32 lEndPos = contentType.IndexOf(";", lStartPos, StringComparison.Ordinal);
			if (lEndPos == -1)
				lEndPos = contentType.Length;

			String lCharsetName = contentType.Substring(lStartPos, lEndPos - lStartPos).Trim();

			if (String.IsNullOrEmpty(lCharsetName))
				return System.Text.Encoding.ASCII;

			lCharsetName = lCharsetName.ToLower(CultureInfo.InvariantCulture);

			if (String.Equals(lCharsetName, "utf-7", StringComparison.Ordinal))
				return System.Text.Encoding.UTF7;

			if (String.Equals(lCharsetName, "utf-8", StringComparison.Ordinal))
				return System.Text.Encoding.UTF8;

			if (String.Equals(lCharsetName, "unicode", StringComparison.Ordinal))
				return System.Text.Encoding.Unicode;

			if (String.Equals(lCharsetName, "unicodeFFFE", StringComparison.Ordinal))
				return System.Text.Encoding.BigEndianUnicode;

			return System.Text.Encoding.ASCII;
		}

		private static void SetAuthorizationHeader(HttpHeaders headers, String header, String username, String password)
		{
			if (String.IsNullOrEmpty(username))
				return;

			Byte[] lByteData = System.Text.Encoding.UTF8.GetBytes(username + ":" + password);
			String lAuthData = "Basic " + Convert.ToBase64String(lByteData, 0, lByteData.Length);

			headers.SetHeaderValue(header, lAuthData);
		}

		public String Get(String url, System.Text.Encoding encoding)
		{
			using (HttpClientResponse response = GetResponse(url))
			{
				response.Encoding = (encoding != null) ? encoding : GetEncodingFromContentType(response.Header.ContentType);
				return response.ContentString;
			}
		}

		public Byte[] GetBytes(String url)
		{
			using (HttpClientResponse response = GetResponse(url))
			{
				return response.ContentBytes;
			}
		}

		public HttpClientResponse GetResponse(String url)
		{
			HttpClientRequest lRequest = new HttpClientRequest();
			lRequest.Url.Parse(url);
			lRequest.Header.RequestType = "GET";
			lRequest.Header.SetHeaderValue("Accept", Accept);
			lRequest.Header.SetHeaderValue("User-Agent", UserAgent);
			lRequest.KeepAlive = KeepAlive;

			return this.Dispatch(lRequest);
		}

		public String Post(String url, Byte[] content)
		{
			HttpClientRequest lRequest = new HttpClientRequest();
			lRequest.Url.Parse(url);
			lRequest.RequestType = RequestType.Post;
			lRequest.Header.SetHeaderValue("Accept", Accept);
			lRequest.Header.SetHeaderValue("User-Agent", UserAgent);
			lRequest.KeepAlive = KeepAlive;
			lRequest.ContentBytes = content;

			using (HttpClientResponse response = this.Dispatch(lRequest))
				return response.ContentString;
		}

		public String Post(String url, Stream content)
		{
			HttpClientRequest lRequest = new HttpClientRequest();
			lRequest.Url.Parse(url);
			lRequest.RequestType = RequestType.Post;
			lRequest.Header.SetHeaderValue("Accept", Accept);
			lRequest.Header.SetHeaderValue("User-Agent", UserAgent);
			lRequest.KeepAlive = KeepAlive;
			lRequest.ContentStream = content;

			using (HttpClientResponse response = this.Dispatch(lRequest))
				return response.ContentString;
		}

		public void Abort()
		{
			if (this.fConnection == null)
				return;

			if (this.fConnection.Connected)
				try
				{
					this.fConnection.Disconnect();
				}
				catch (Exception)
				{
					// As designed
				}

			this.DisposeHttpConnection();
		}

		public HttpClientResponse TryDispatch(HttpClientRequest request)
		{
			HttpClient.SetAuthorizationHeader(request.Header, "Authorization", this.UserName, this.Password);

			String lHostname = request.Url.Hostname;
			Int32 lPort = request.Url.Port;
			Boolean lSslConnection = String.Equals(request.Url.Protocol, "https", StringComparison.OrdinalIgnoreCase);

			// Settings for connection thru Http Proxy
			// Note that Request should think that it uses direct connection when SSL is enabled because
			// proxy server tunnels SSL data AS IS, without adjusting its HTTP headers
			request.UseProxy = this.ProxySettings.UseProxy && !lSslConnection;
			if (this.ProxySettings.UseProxy)
			{
				lHostname = this.ProxySettings.ProxyHost;
				lPort = this.ProxySettings.ProxyPort;

				HttpClient.SetAuthorizationHeader(request.Header, "Proxy-Authorization", this.ProxySettings.UserName, this.ProxySettings.Password);
			}

			Connection lConnection = this.GetHttpConnection(lSslConnection, request.Url.Hostname, request.Url.Port, lHostname, lPort);

			try
			{
				lConnection.Timeout = Timeout;
				lConnection.TimeoutEnabled = TimeoutEnabled;
				request.WriteHeaderToConnection(lConnection);
			}
			catch (ObjectDisposedException)
			{
				lConnection = this.GetNewHttpConnection(lHostname, lPort);
				request.WriteHeaderToConnection(lConnection);
			}
			catch (ConnectionClosedException)
			{
				lConnection = this.GetNewHttpConnection(lHostname, lPort);
				request.WriteHeaderToConnection(lConnection);
			}
			catch (System.Net.Sockets.SocketException)
			{
				lConnection = this.GetNewHttpConnection(lHostname, lPort);
				request.WriteHeaderToConnection(lConnection);
			}

			request.WriteBodyToConnection(lConnection);


			HttpClientResponse lResponse;
			do
			{
				HttpHeaders lHeaders = HttpHeaders.Create(lConnection);
				if (lHeaders == null)
					throw new ConnectionClosedException();
				lResponse = new HttpClientResponse(lConnection, lHeaders);
			}
			while (lResponse.Header.HttpCode == HttpStatusCode.Continue); // 100 CONTINUE means useless response.

			if (!lResponse.KeepAlive)
			{
				this.fConnectionUrl = null;
				this.fConnection = null;
			}

			if (lResponse.HttpCode == HttpStatusCode.ProxyAuthenticationRequired)
				throw new HttpException("Proxy authorization failed", lResponse);

			return lResponse;
		}

		public HttpClientResponse Dispatch(HttpClientRequest request)
		{
			HttpClientResponse lResponse = this.TryDispatch(request);

			if ((Int32)lResponse.HttpCode >= 400)
			{
				if (lResponse.HasContentLength)
					throw new HttpException(lResponse.ContentString, lResponse);

				throw new HttpException(lResponse.Header.ToString(), lResponse);
			}

			return lResponse;
		}

		private void DisposeHttpConnection()
		{
			if (this.fConnection != null)
			{
				this.fConnection.Dispose();
				this.fConnection = null;
			}

			this.fConnectionUrl = null;
		}

		protected Connection GetHttpConnection(Boolean enableSSL, String targetHost, Int32 targetPort, String connectionHost, Int32 connectionPort)
		{
#if FULLFRAMEWORK
			if (enableSSL)
			{
				this.SslOptions.Enabled = true;
				this.SslOptions.TargetHostName = targetHost;
				((HttpsConnectionFactory)this.SslOptions).TargetPort = targetPort;
			}
			else
			{
				this.SslOptions.Enabled = false;
			}
#endif

			if (!this.KeepAlive || (this.ConnectionPool != null))
				return this.Connect(connectionHost, connectionPort); // pooling class will make sure we use the right connection

			String lUrl = connectionHost + ':' + connectionPort;

			if (this.fConnection != null)
			{
				if ((this.fConnectionUrl == lUrl) && this.fConnection.Connected)
					return this.fConnection;

				this.fConnection.Dispose();
			}

			this.fConnectionUrl = lUrl;
			this.fConnection = this.Connect(connectionHost, connectionPort);

			return this.fConnection;
		}

		private Connection GetNewHttpConnection(String hostname, Int32 port)
		{
			if (this.ConnectionPool != null)
				return this.ConnectNew(hostname, port);

			this.DisposeHttpConnection();

			this.fConnectionUrl = hostname + ':' + port;
			this.fConnection = this.ConnectNew(hostname, port);

			return this.fConnection;
		}
		#endregion
	}

	[Serializable]
	public class HttpException : Exception
	{
		public HttpException(HttpClientResponse response)
		{
			this.fResponse = response;
		}

		public HttpException(String message, HttpClientResponse response)
			: base(message)
		{
			this.fResponse = response;
		}

		public HttpException(String message, HttpClientResponse response, Exception innerException)
			: base(message, innerException)
		{
			this.fResponse = response;
		}

		#region Properties
		public HttpClientResponse Response
		{
			get
			{
				return this.fResponse;
			}
		}
		private readonly HttpClientResponse fResponse;
		#endregion
	}
}
