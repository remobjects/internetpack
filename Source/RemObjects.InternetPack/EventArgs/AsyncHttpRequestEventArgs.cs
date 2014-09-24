/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2013. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.Http
{
    public class AsyncHttpRequestEventArgs : ConnectionEventArgs
    {
        public AsyncHttpRequestEventArgs(Connection connection, AsyncHttpContext context)
            : base(connection)
        {
            this.fContext = context;
        }

        public AsyncHttpContext Context
        {
            get
            {
                return this.fContext;
            }
        }
        private readonly AsyncHttpContext fContext;
    }


    public delegate void AsyncHttpRequestEventHandler(Object sender, AsyncHttpRequestEventArgs e);
}
