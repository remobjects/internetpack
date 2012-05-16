/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System.Net;

namespace RemObjects.InternetPack
{
    public class SimpleServer
    {
        public SimpleServer()
        {
            this.fBinding = new ServerBinding();
            this.fBinding.Address = IPAddress.Any;
        }

        #region Properties
        public ServerBinding Binding
        {
            get
            {
                return this.fBinding;
            }
        }
        private ServerBinding fBinding;
        #endregion

        public void Open()
        {
            this.Binding.BindUnthreaded();
            this.Binding.ListeningSocket.Listen(this.Binding.MaxWaitConnections);
        }

        public void Close()
        {
            this.Binding.Unbind();
        }

        public Connection WaitForConnection()
        {
            return this.Binding.Accept();
        }
    }
}