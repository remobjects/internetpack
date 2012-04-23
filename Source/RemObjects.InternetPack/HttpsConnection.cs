/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Text;

namespace RemObjects.InternetPack.Http
{
    public class HttpsConnectionFactory : SslConnectionFactory
    {
        public HttpsConnectionFactory(HttpProxySettings proxySettings)
        {
            this.ProxySettings = proxySettings;
            this.TargetPort = 8099;
        }

        [DefaultValue(8099)]
        [Category("Ssl Options")]
        public Int32 TargetPort
        {
            get
            {
                return this.fTargetPort;
            }
            set
            {
                this.fTargetPort = value;
            }
        }
        private Int32 fTargetPort;

        [Browsable(false)]
        public HttpProxySettings ProxySettings
        {
            get
            {
                return this.fProxySettings;
            }
            protected set
            {
                this.fProxySettings = value;
            }
        }
        private HttpProxySettings fProxySettings;

        public override Connection CreateClientConnection(Binding binding)
        {
            if (!this.Enabled)
                return new Connection(binding);

            if (this.HasCertificate)
                this.LoadCertificate();

            return new HttpSslConnection(this, binding);
        }

        public override Connection CreateClientConnection(Connection connection)
        {
            if (this.HasCertificate)
                this.LoadCertificate();

            return new HttpSslConnection(this, connection);
        }
    }

    public class HttpSslConnection : SslConnection
    {
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

        private HttpsConnectionFactory fHttpsConnectionFactory;

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
                Byte[] lByteData = System.Text.Encoding.UTF8.GetBytes(this.fHttpsConnectionFactory.ProxySettings.UserName + ":" + this.fHttpsConnectionFactory.ProxySettings.Password);

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
                throw new System.IO.IOException("Proxy server didn't send an answer for SSL tunnel request");

            // Parse result

            // Its first line should look like "HTTP/1.1 200 Blind-Connection Established"
            // So we check does the response contain 200 or not
            // We check only next 3 chars after the first " "

            Int32 lSpacePos = lResonse.IndexOf(' ') + 1;
            if (lSpacePos > lResonse.Length - 3)
                throw new SocketException();

            String lHttpResultCode = lResonse.Substring(lSpacePos, 3);
            if (String.Equals(lHttpResultCode, "407", StringComparison.Ordinal))
                throw new SocketException();

            if (!String.Equals(lHttpResultCode, "200", StringComparison.Ordinal))
                throw new SocketException();
        }

        private void SendSslTunnelRequest()
        {
            if (!this.fHttpsConnectionFactory.ProxySettings.UseProxy)
                return;

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

        public override IAsyncResult BeginInitializeClientConnection(AsyncCallback callback, Object state)
        {
            this.SendSslTunnelRequest();

            return base.BeginInitializeClientConnection(callback, state);
        }
    }
}
