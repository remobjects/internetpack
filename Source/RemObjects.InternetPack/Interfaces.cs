/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

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