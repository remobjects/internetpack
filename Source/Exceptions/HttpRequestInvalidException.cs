/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
	[Serializable]
	public class HttpRequestInvalidException : Exception
	{
	    #if !cooper
    	[Obsolete("Provide error code using a System.Net.HttpStatusCode value")]
		public HttpRequestInvalidException(Int32 errorCode, String errorMessage)
			: this((HttpStatusCode)errorCode, errorMessage)
		{
		}

		[Obsolete("Provide error code using a System.Net.HttpStatusCode value")]
		public HttpRequestInvalidException(Int32 errorCode, String errorMessage, Exception innerException)
			: this((HttpStatusCode)errorCode, errorMessage, innerException)
		{
		}
        #endif

		public HttpRequestInvalidException(HttpStatusCode errorCode, String errorMessage)
			: this(errorCode, errorMessage, null)
		{
		}

		public HttpRequestInvalidException(HttpStatusCode errorCode, String errorMessage, Exception innerException)
			: base(((Int32)errorCode).ToString() + " " + errorMessage, innerException)
		{
			this.fErrorCode = errorCode;
			this.fErrorMessage = errorMessage;
		}

		public HttpStatusCode ErrorCode
		{
			get
			{
				return this.fErrorCode;
			}
		}
		private readonly HttpStatusCode fErrorCode;

		public String ErrorMessage
		{
			get
			{
				return this.fErrorMessage;
			}
		}
		private readonly String fErrorMessage;
	}
}