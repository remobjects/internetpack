/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Net.Sockets;

namespace RemObjects.InternetPack.Http
{
#if DESIGN
    [System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.HttpServer.bmp")]
#endif
    public class HttpServer : Server
    {
        public HttpServer()
            : base()
        {
            this.DefaultPort = 80;
            this.KeepAlive = true;
            this.ServerName = sServerName;
            this.ValidateRequests = true;
        }

#if FULLFRAMEWORK
        public HttpServer(IContainer container)
            : this()
        {
            container.Add(this);
        }
#endif

        public override Type GetWorkerClass()
        {
            return typeof(HttpWorker);
        }

        #region Properties
        [DefaultValue(true), Category("Server")]
        public Boolean KeepAlive
        {
            get
            {
                return fKeepAlive;
            }
            set
            {
                fKeepAlive = value;
            }
        }
        private Boolean fKeepAlive;

        [DefaultValue(true), Category("Server")]
        public Boolean ValidateRequests
        {
            get
            {
                return fValidateRequests;
            }
            set
            {
                fValidateRequests = value;
            }
        }
        private Boolean fValidateRequests;

        [DefaultValue(sServerName), Category("Server")]
        public String ServerName
        {
            get
            {
                return fServerName;
            }
            set
            {
                fServerName = value;
            }
        }
        private String fServerName;

        public const String sServerName = "Internet Pack HTTP Server";
        #endregion

        #region Events
        public event OnHttpRequestHandler OnHttpRequest;

        protected virtual void TriggerOnHttpRequest(Connection connection, HttpServerRequest request, HttpServerResponse response)
        {
            if (OnHttpRequest != null)
            {
                OnHttpRequestArgs lEventArgs = new OnHttpRequestArgs(connection, request, response);
                OnHttpRequest(this, lEventArgs);
            }
        }
        #endregion

        #region Methods for implementation in descendand classes
        internal protected virtual void HandleHttpRequest(Connection connection, HttpServerRequest request, HttpServerResponse response)
        {
            this.TriggerOnHttpRequest(connection, request, response);
        }
        #endregion
    }

    public class HttpWorker : Worker
    {
        public HttpWorker()
        {
        }

        public new HttpServer Owner
        {
            get
            {
                return (HttpServer)base.Owner;
            }
        }

        protected override void DoWork()
        {
            try
            {
                ProcessRequests();
            }
            catch (SocketException ex)
            {
                /* 10054 means the connection was closed by the client while reading from the socket.
                 * we'll just terminate the thread gracefully, as if this was expected. */
                /* ToDo: provide some sort of notification to the server object via event. */
                if (ex.ErrorCode != 10054)
                    throw ex;
            }
        }

        public virtual void ProcessRequests()
        {
            do
            {
                try
                {
                    HttpHeaders lHeaders = HttpHeaders.Create(DataConnection);
                    if (lHeaders == null)
                    {
                        DataConnection.Close(); // disconnected
                        break;
                    }
                    HttpServerRequest lRequest = new HttpServerRequest(DataConnection, lHeaders);

                    /* ToDo: make (some) validation optinal, for speed purposes */
                    if (Owner.ValidateRequests)
                        lRequest.Validate();

                    HttpServerResponse lResponse = new HttpServerResponse();
                    lResponse.KeepAlive = (lRequest.KeepAlive && Owner.KeepAlive);
                    lResponse.Header.SetHeaderValue("Server", Owner.ServerName);

                    Owner.HandleHttpRequest(DataConnection, lRequest, lResponse);

                    lRequest.FlushContent();

                    lResponse.WriteToConnection(DataConnection);
                    if (!lRequest.KeepAlive)
                        DataConnection.Close(false);
                }
                catch (HttpRequestInvalidException e)
                {
                    e.Response.Header.SetHeaderValue("Server", Owner.ServerName);
                    e.Response.WriteToConnection(DataConnection);
                    DataConnection.Close();
                }
                catch (ConnectionClosedException)
                {
                    DataConnection.Close(false);
                }
                catch (SocketException)
                {
                    DataConnection.Close(false);
                }
                catch (Exception e)
                {
                    HttpServerResponse lResponse = new HttpServerResponse();
                    lResponse.Header.SetHeaderValue("Server", Owner.ServerName);
                    lResponse.SendError(500, "Server Error while processing HTTP request.", e);
                    lResponse.WriteToConnection(DataConnection);
                    DataConnection.Close();
                }

                if (!Owner.KeepAlive)
                    DataConnection.Close();
            }
            while (Owner.KeepAlive && DataConnection.Connected);
        }
    }

    public delegate void OnHttpRequestHandler(Object sender, OnHttpRequestArgs e);

    public class OnHttpRequestArgs : ConnectionEventArgs
    {
        public OnHttpRequestArgs(Connection connection, HttpServerRequest request, HttpServerResponse response)
            : base(connection)
        {
            this.fRequest = request;
            this.fResponse = response;
        }

        public HttpServerRequest Request
        {
            get
            {
                return this.fRequest;
            }
        }
        private readonly HttpServerRequest fRequest;

        public HttpServerResponse Response
        {
            get
            {
                return this.fResponse;
            }
        }
        private readonly HttpServerResponse fResponse;
    }
}
