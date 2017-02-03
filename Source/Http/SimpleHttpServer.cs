/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
#if DESIGN
	[System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.SimpleHttpServer.bmp")]
#endif
	public class SimpleHttpServer : HttpServer
	{
		#region Propeties
		[Category("Server")]
		public String RootPath { get; set; }
		#endregion

		#region Overriden Methods
		protected override void HandleHttpRequest(Connection connection, HttpServerRequest request, HttpServerResponse response)
		{
			base.HandleHttpRequest(connection, request, response);

			if (response.ContentSource != ContentSource.ContentNone)
			{
				return;
			}

			if (request.Header.RequestType != "GET")
			{
				response.SendError(HttpStatusCode.BadRequest, String.Format("Request Type '{0}' not supported.", request.Header.RequestType));
				return;
			}

			String lPath = RootPath + request.Header.RequestPath.Replace('/', Path.DirectorySeparatorChar);
			if (lPath.IndexOf("..") > -1)
			{
				response.SendError(HttpStatusCode.Forbidden, String.Format("Bad Request: Path '{0}' contains '..' which is invalid.", lPath));
				return;
			}

			if (!File.Exists(lPath))
			{
				response.SendError(HttpStatusCode.NotFound, String.Format("File '{0}' not found.", lPath));
				return;
			}

			response.Header.ContentType = "text/html";
			response.ContentStream = new FileStream(lPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			response.CloseStream = true; /* Response will close stream once it's been sent */
		}
		#endregion
	}
}