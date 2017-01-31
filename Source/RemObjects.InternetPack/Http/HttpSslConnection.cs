/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
	public class HttpSslConnection : SslConnection
	{
		private readonly HttpsConnectionFactory fHttpsConnectionFactory;

		public HttpSslConnection(HttpsConnectionFactory factory, Binding binding)
			: base(factory, binding)
		{
			this.fHttpsConnectionFactory = factory;
		}

		public HttpSslConnection(HttpsConnectionFactory factory, Socket socket)
			: base(factory, socket)
		{
			this.fHttpsConnectionFactory = factory;
		}

		public HttpSslConnection(HttpsConnectionFactory factory, Connection connection)
			: base(factory, connection)
		{
			this.fHttpsConnectionFactory = factory;
		}

		private String ComposeSslTunnelRequest()
		{
			StringBuilder lSslTunnelRequest = new StringBuilder(1024);

			String lFullHostName = this.fHttpsConnectionFactory.TargetHostName + ":" + this.fHttpsConnectionFactory.TargetPort;

			lSslTunnelRequest.Append("CONNECT ");
			lSslTunnelRequest.Append(lFullHostName);
			lSslTunnelRequest.AppendLine("  HTTP/1.1");

			lSslTunnelRequest.Append("Host: ");
			lSslTunnelRequest.AppendLine(lFullHostName);

			if (!String.IsNullOrEmpty(this.fHttpsConnectionFactory.ProxySettings.UserName))
			{
				Byte[] lByteData = Encoding.UTF8.GetBytes(this.fHttpsConnectionFactory.ProxySettings.UserName + ":" + this.fHttpsConnectionFactory.ProxySettings.Password);

				lSslTunnelRequest.Append("Proxy-Authorization: Basic ");
				lSslTunnelRequest.Append(Convert.ToBase64String(lByteData, 0, lByteData.Length));
				lSslTunnelRequest.AppendLine();
			}
			lSslTunnelRequest.AppendLine();

			return lSslTunnelRequest.ToString();
		}

		private void ParseSslTunnelResponse(Byte[] rawResponseData)
		{
			String lResonse = Encoding.UTF8.GetString(rawResponseData);
			if (String.IsNullOrEmpty(lResonse) || (lResonse.Length < 9))
			{
				throw new System.IO.IOException("Proxy server didn't send an answer for SSL tunnel request");
			}

			// Parse result

			// Its first line should look like "HTTP/1.1 200 Blind-Connection Established"
			// So we check does the response contain 200 or not
			// We check only next 3 chars after the first " "

			Int32 lSpacePos = lResonse.IndexOf(' ') + 1;
			if (lSpacePos > lResonse.Length - 3)
			{
				throw new SocketException();
			}

			String lHttpResultCode = lResonse.Substring(lSpacePos, 3);
			if (lHttpResultCode != "200")
			{
				throw new SocketException();
			}
		}

		private void SendSslTunnelRequest()
		{
			if (!this.fHttpsConnectionFactory.ProxySettings.UseProxy)
			{
				return;
			}

			this.DataSocket.Send(Encoding.UTF8.GetBytes(this.ComposeSslTunnelRequest()));

			Byte[] lRawResponse = new Byte[1024];
			this.DataSocket.Receive(lRawResponse);

			this.ParseSslTunnelResponse(lRawResponse);
		}

		public override void InitializeClientConnection()
		{
			this.SendSslTunnelRequest();

			base.InitializeClientConnection();
		}

		protected override IAsyncResult BeginInitializeClientConnection(AsyncCallback callback, Object state)
		{
			this.SendSslTunnelRequest();

			return base.BeginInitializeClientConnection(callback, state);
		}
	}
}