/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
	[Serializable]
	public class HttpHeaderException : Exception
	{
		public HttpHeaderException()
		{
		}

		public HttpHeaderException(String message)
			: base(message)
		{
		}

		public HttpHeaderException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}