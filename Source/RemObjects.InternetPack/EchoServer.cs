/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack.StandardServers
{
#if DESIGN
    [System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.EchoServer.bmp")]
#endif
    public class EchoServer : Server
    {
        public EchoServer()
        {
            this.DefaultPort = 7; // Default port for echo service
            this.Port = this.DefaultPort;
        }

        public override Type GetWorkerClass()
        {
            return typeof(EchoWorker);
        }
    }

    public class EchoWorker : Worker
    {
        protected override void DoWork()
        {
            Int32 lReceived;

            do
            {
                Byte[] lReceiveBuffer = new Byte[256]; /* a tiny buffer is good enough for echo server */
                lReceived = this.DataConnection.Read(lReceiveBuffer, 0, lReceiveBuffer.Length);

                if (lReceived > 0)
                    this.DataConnection.Write(lReceiveBuffer, 0, lReceived);
            }
            while (lReceived > 0);
        }
    }
}
