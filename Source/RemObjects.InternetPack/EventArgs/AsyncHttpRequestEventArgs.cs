/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

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