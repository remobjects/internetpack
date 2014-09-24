/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2013. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Http
{
    public class HttpRequestEventArgs : ConnectionEventArgs
    {
        public HttpRequestEventArgs(Connection connection, HttpServerRequest request, HttpServerResponse response)
            : base(connection)
        {
            this.fRequest = request;
            this.fResponse = response;
            this.Handled = false;
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

        public Boolean Handled
        {
            get
            {
                return this.fHandled;
            }
            set
            {
                this.fHandled = value;
            }
        }
        private Boolean fHandled;
    }


    public delegate void HttpRequestEventHandler(Object sender, HttpRequestEventArgs e);
}
