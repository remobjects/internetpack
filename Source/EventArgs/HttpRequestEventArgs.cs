/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

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

		public Boolean Handled { get; set; }
	}

	public delegate void HttpRequestEventHandler(Object sender, HttpRequestEventArgs e);
}