/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace RemObjects.InternetPack
{
    public class SimpleServer
    {
        public static readonly SimplePortPool Pool = new SimplePortPool();
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

        public bool Open()
        {
            var retries = 3;
            while (retries > 0)
            {
                try
                {
                    this.Binding.Port = Pool.GetNextSocketPort();
                    this.Binding.BindUnthreaded();
                    this.Binding.ListeningSocket.Listen(this.Binding.MaxWaitConnections);
                    break;
                }
                catch (SocketException)
                {
                    this.Binding.Unbind();
                }

                --retries;
            }

            return retries > 0;
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

    public class SimplePortPool
    {
        int _minPort = 5000;
        int _maxPort = 5010;
        int _nextPort;

        public void SetPortRange(int min, int max)
        {
            if (min >= max)
                throw new InvalidOperationException();

            _minPort = min;
            _maxPort = max;

        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public int GetNextSocketPort()
        {
            if (_minPort > _maxPort)
                throw new InvalidOperationException("Max port should be greater than min port");

            if (_nextPort < _minPort || _maxPort < _nextPort)
                _nextPort = _minPort;
            var port = _nextPort;
            ++_nextPort;
            return port;
        }
    }
}