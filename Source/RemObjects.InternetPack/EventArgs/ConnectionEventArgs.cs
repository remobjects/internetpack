/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
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