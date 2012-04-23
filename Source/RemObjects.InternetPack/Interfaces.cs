/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Net.Sockets;
using System.Threading;

namespace RemObjects.InternetPack
{
    public interface IOwned
    {
        Server Owner { get; set; }
    }

    public interface IListener : IOwned
    {
        void Listen();
        Socket ListeningSocket { get; set; }
    }

    public interface IWorker : IOwned
    {
        void Work();
        Connection DataConnection { get; set; }
        Thread Thread { get; set; }
        event EventHandler Done;
    }

    public interface IAsyncWorker : IOwned
    {
        Connection DataConnection { get; set; }
        void Setup();
    }

    public interface IConnectionFactory
    {
        Connection CreateServerConnection(Socket socket);
        Connection CreateClientConnection(Binding binding);
    }
}