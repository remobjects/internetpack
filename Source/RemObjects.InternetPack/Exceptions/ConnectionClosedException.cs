/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	[Serializable]
	public class ConnectionClosedException : Exception
	{
		public ConnectionClosedException()
			: base("Connection was closed.")
		{
		}

		public ConnectionClosedException(Boolean timeout)
			: base(timeout ? "Timeout executing operation" : "Connection was closed.")
		{
			this.fTimeout = timeout;
		}

		public ConnectionClosedException(String message)
			: base("Connection was closed; " + message)
		{
		}

		public ConnectionClosedException(Exception innerException)
			: base("Connection was closed.", innerException)
		{
		}

		public Boolean Timeout
		{
			get
			{
				return fTimeout;
			}
		}
		private readonly Boolean fTimeout;
	}
}