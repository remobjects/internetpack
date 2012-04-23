/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.IO;

namespace RemObjects.InternetPack.Http
{
#if DESIGN
    [System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.SimpleHttpServer.bmp")]
#endif
    public class SimpleHttpServer : HttpServer
    {
        #region Propeties
        [Category("Server")]
        public String RootPath
        {
            get
            {
                return this.fRootPath;
            }
            set
            {
                this.fRootPath = value;
            }
        }
        private String fRootPath;
        #endregion

        #region Overriden Methods
        protected internal override void HandleHttpRequest(Connection connection, HttpServerRequest request, HttpServerResponse response)
        {
            base.HandleHttpRequest(connection, request, response);

            if (response.ContentSource == ContentSource.ContentNone)
            {
                if (request.Header.RequestType == "GET")
                {
                    String lPath = RootPath + request.Header.RequestPath.Replace('/', Path.DirectorySeparatorChar);
                    if (lPath.IndexOf("..") == -1)
                    {
                        if (File.Exists(lPath))
                        {
                            response.Header.ContentType = "text/html";
                            response.ContentStream = new FileStream(lPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                            response.CloseStream = true; /* Response will close stream once it's been sent */
                        }
                        else
                        {
                            response.SendError(404, String.Format("File '{0}' not found.", lPath));
                        }
                    }
                    else
                    {
                        response.SendError(403, String.Format("Bad Request: Path '{0}' contains '..' which is invalid.", lPath));
                    }
                }
                else
                {
                    response.SendError(500, String.Format("Request Type '{0}' not supported.", request.Header.RequestType));
                }
            }
        }
        #endregion
    }
}
