/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2014. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;

namespace RemObjects.InternetPack
{
	public class ConnectionEventArgs : EventArgs
	{
		public ConnectionEventArgs(Connection connection)
			: base()
		{
			this.fConnection = connection;
		}

		public Connection Connection
		{
			get
			{
				return this.fConnection;
			}
		}
		private readonly Connection fConnection;
	}
}