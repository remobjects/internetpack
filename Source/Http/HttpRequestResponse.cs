/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.InternetPack.Events;

namespace RemObjects.InternetPack.Http
{
	#region Request/Response Base Classes
	public abstract class HttpRequestResponse
	{
		protected HttpRequestResponse()
		{
			this.Header = new HttpHeaders();
			this.Encoding = Encoding.ASCII;
		}

		protected HttpRequestResponse(HttpHeaders header)
		{
			this.Header = header;
			this.Encoding = Encoding.ASCII;
		}

		public HttpHeaders Header { get; set; }

		public Encoding Encoding { get; set; }

		protected abstract Boolean Client { get; }

		protected abstract Boolean Server { get; }

		#region Events
		public event TransferStartEventHandler OnTransferStart;

		public event TransferEndEventHandler OnTransferEnd;

		public event TransferProgressEventHandler OnTransferProgress;

		public void CloneEvents(HttpRequestResponse source)
		{
            #if cooper || island || toffee
            foreach(TransferStartEventHandler item in source.OnTransferStart)
            {
                #if cooper
                OnTransferStart.add(item);
                #elif island
                OnTransferStart.Add(item);
                #elif toffee
                OnTransferStart.addObject(item);
                #endif
            }

            foreach(TransferEndEventHandler item in source.OnTransferEnd)
            {
                #if cooper
                OnTransferEnd.add(item);
                #elif island
                OnTransferEnd.Add(item);
                #else
                OnTransferEnd.addObject(item);
                #endif
            }

            foreach(TransferProgressEventHandler item in source.OnTransferProgress)
            {
                #if cooper
                OnTransferProgress.add(item);
                #elif island
                OnTransferProgress.Add(item);
                #else                
                OnTransferProgress.addObject(item);
                #endif
            }
            #elif echoes
            OnTransferStart = source.OnTransferStart;
			OnTransferEnd = source.OnTransferEnd;
			OnTransferProgress = source.OnTransferProgress;
            #endif
		}

		protected Boolean HasOnTransferProgress
		{
			get
			{
				return this.OnTransferProgress != null;
			}
		}

		protected virtual void TriggerOnTransferStart(TransferDirection direction, Int64 size)
		{
			if (OnTransferStart != null)
				OnTransferStart(this, new TransferStartEventArgs(direction, size));
		}

		protected virtual void TriggerOnTransferEnd(TransferDirection direction)
		{
			if (OnTransferEnd != null)
				OnTransferEnd(this, new TransferEndEventArgs(direction));
		}

		protected virtual void TriggerOnTransferProgress(TransferDirection direction, Int64 position)
		{
			if (OnTransferProgress != null)
				OnTransferProgress(this, new TransferProgressEventArgs(direction, position));
		}
		#endregion
	}

	public abstract class HttpIncomingRequestResponse : HttpRequestResponse
	{
		protected HttpIncomingRequestResponse(Connection connection, HttpHeaders headers)
			: base(headers)
		{
			this.DataConnection = connection;
		}

		public Connection DataConnection { get; private set; }

		const Int32 BUFFER_SIZE = 64 * 1024;

		#region Properties: Content
		public Byte[] ContentBytes
		{
			get
			{
				if (fContentBytes == null)
				{
					if (HasContentLength) /* Server must always have on ContentLength */
					{
						fContentBytes = new Byte[ContentLength];
						Int32 lRead = ContentStream.Read(fContentBytes, 0, fContentBytes.Length);
						if (lRead != fContentBytes.Length)
							throw new Exception("Unexpected end of response");
					}
					else
					{
						Boolean lChunked = Chunked;
						Boolean lKeepAlive = KeepAlive;
						if (lKeepAlive && !lChunked)
							throw new Exception("Content-Length or Chunked Transfer-Encoding required for Keep-Alive.");

						MemoryStream lResult = new MemoryStream();

						if (lChunked)
						{
							Int32 lNextChunkSize = ReadChunkSize();
							while (lNextChunkSize > 0)
							{
								Byte[] lBuffer = new Byte[lNextChunkSize];
								Int32 lRead = DataConnection.Receive(lBuffer, 0, lNextChunkSize);
								if (lRead != lNextChunkSize) throw new Exception("Unexpected end of HTTP chunk.");

								// todo: refactor, duped below
								lResult.Write(lBuffer, 0, lRead);

								if (DataConnection.ReadLine() != "") throw new Exception("Invalid data at end of HTTP chunk.");
								lNextChunkSize = ReadChunkSize();
							}

							// Skip footer
							while (DataConnection.ReadLine() != "")
							{
							}
						}
						else
						{
							Int32 lRead;
							do
							{
								Byte[] lBuffer = new Byte[BUFFER_SIZE];
								lRead = DataConnection.Read(lBuffer, 0, BUFFER_SIZE);
								lResult.Write(lBuffer, 0, lRead);
							}
							while (lRead > 0);
						}

						fContentBytes = lResult.ToArray();

						if (!lKeepAlive)
							DataConnection.Close();
					}
				}

				return fContentBytes;
			}
		}
		private Byte[] fContentBytes;

		public Stream ContentStream
		{
			get
			{
				if (fContentStream == null)
				{
					if (HasContentLength || Server)
						fContentStream = new HttpIncomingStream(this);
					else
						fContentStream = DataConnection;
				}
				return fContentStream;
			}
		}
		private Stream fContentStream;

		public String ContentString
		{
			get
			{
				if (fContentString == null)
					fContentString = Encoding.GetString(ContentBytes, 0, ContentBytes.Length);

				return fContentString;
			}
		}
		private String fContentString;

		public Boolean HasContentLength
		{
			get
			{
				String lLength = Header.GetHeaderValue("Content-Length");
				if (!String.IsNullOrEmpty(lLength))
				{
					try
					{
						// TryPArse is not available on CF
						Convert.ToInt32(lLength);
						return true;
					}
					catch (Exception)
					{
						// As designed
					}
				}

				return false;
			}
		}

		public Int32 ContentLength
		{
			get
			{
				if (fContentLength == -1)
				{
					String lLength = Header.GetHeaderValue("Content-Length");
					if (String.IsNullOrEmpty(lLength))
						throw new HttpHeaderException("No Content-Lengh specified in header.");

					fContentLength = Convert.ToInt32(lLength);
				}

				return fContentLength;
			}
		}
		private Int32 fContentLength = -1;

		public Boolean KeepAlive
		{
			get
			{
				String lConnection = Header.GetHeaderValue("Connection");

				return lConnection != null && lConnection.ToLowerInvariant() == "keep-alive";
			}
		}

		public Boolean Chunked
		{
			get
			{
				String lTransferEncoding = Header.GetHeaderValue("Transfer-Encoding");

				return lTransferEncoding != null && lTransferEncoding.ToLower() == "chunked";
			}
		}
		#endregion

		public Boolean FlushContent()
		{
			if (!HasContentLength)
				return true; //nothing to flush

			if (fContentBytes != null)
				return true; // we already read entire response

			if (fContentStream != null)
			{
				if (fContentStream is HttpIncomingStream)
					return (fContentStream as HttpIncomingStream).FlushContent();

				return true;
			}

			if (ContentLength > 16 * 1204)
				return false;

			DataConnection.SkipBytes(ContentLength);
			return true;
		}

		public virtual void Validate()
		{
			if (Header.RequestVersion == "1.1")
			{
				if (Header.GetHeaderValue("Transfer-Encoding") == "chunked")
					throw new HttpRequestInvalidException(HttpStatusCode.InternalServerError, "Bad Request: Chunked encoding not supported, yet");
			}
		}

		private Int32 ReadChunkSize()
		{
			String lLine = DataConnection.ReadLine();
			Int32 lPos = lLine.IndexOf(';');
			if (lPos >= 0)
				lLine = lLine.Substring(0, lPos);
			lLine = lLine.Trim();

			return Convert.HexStringToInt32(lLine);
		}

	}

	public abstract class HttpOutgoingRequestResponse : HttpRequestResponse
	{
		protected internal HttpOutgoingRequestResponse()
		{
			CloseStream = true;
			ContentSource = ContentSource.ContentNone;
		}

		protected internal HttpOutgoingRequestResponse(HttpHeaders header)
			: base(header)
		{
			CloseStream = true;
			ContentSource = ContentSource.ContentNone;
		}

		#region Properties: Content
		public ContentSource ContentSource { get; private set; }

		public Byte[] ContentBytes
		{
			get
			{
				return fContentBytes;
			}
			set
			{
				fContentBytes = value;
				ContentSource = ContentSource.ContentBytes;
			}
		}
		private Byte[] fContentBytes;

		public Stream ContentStream
		{
			get
			{
				return fContentStream;
			}
			set
			{
				fContentStream = value;
				ContentSource = ContentSource.ContentStream;
			}
		}
		private Stream fContentStream;

		public String ContentString
		{
			get
			{
				return fContentString;
			}
			set
			{
				fContentString = value;
				ContentSource = ContentSource.ContentString;
			}
		}
		private String fContentString;

		public Boolean CloseStream { get; set; }

		public Boolean KeepAlive
		{
			get
			{
				String lConnection = Header.GetHeaderValue("Connection");
				return (lConnection != null && lConnection == "Keep-Alive"); //ToDo: use global String;
			}
			set
			{
				if (value)
					Header.SetHeaderValue("Connection", "Keep-Alive");
				else
					Header.SetHeaderValue("Connection", "Close");
			}
		}
		#endregion

		#region Event Handlers
		protected virtual void HandleOnBytesSent(Object sender, EventArgs e)
		{
			TriggerOnTransferProgress(TransferDirection.Send, ((Connection)sender).BytesSent);
		}
		#endregion

		#region Write to Connection
		const Int32 BUFFER_SIZE = 256 * 1024; /* for now */

		public virtual void FinalizeHeader()
		{
			if (ContentSource == ContentSource.ContentString)
				ContentBytes = Encoding.GetBytes(ContentString);

			switch (ContentSource)
			{
				case ContentSource.ContentString:
				case ContentSource.ContentBytes:
					if (ContentBytes != null)
						Header.SetHeaderValue("Content-Length", ContentBytes.Length.ToString());
					else
						Header.SetHeaderValue("Content-Length", 0.ToString());
					break;

				case ContentSource.ContentStream:
					if (ContentStream != null)
						Header.SetHeaderValue("Content-Length", (ContentStream.Length - ContentStream.Position).ToString());
					else
						Header.SetHeaderValue("Content-Length", 0.ToString());
					break;

				case ContentSource.ContentNone:
					Header.SetHeaderValue("Content-Length", 0.ToString());
					break;
			}
		}

		public virtual void WriteToConnection(Connection connection)
		{
			WriteHeaderToConnection(connection);
			WriteBodyToConnection(connection);
		}

		public virtual void WriteHeaderToConnection(Connection connection)
		{
			FinalizeHeader();
			Header.WriteHeader(connection);
		}

		public virtual void WriteBodyToConnection(Connection connection)
		{
			try
			{
				if (HasOnTransferProgress)
				{
					connection.OnBytesSent += HandleOnBytesSent;
					connection.ResetStatistics();
				}

				switch (ContentSource)
				{
					case ContentSource.ContentBytes:
						TriggerOnTransferStart(TransferDirection.Send, ContentBytes.Length);
						connection.Send(ContentBytes);
						TriggerOnTransferEnd(TransferDirection.Send);
						break;

					case ContentSource.ContentStream:
						TriggerOnTransferStart(TransferDirection.Send, ContentStream.Length);
						Byte[] lBuffer = new Byte[BUFFER_SIZE];
						Int32 lBytesRead;
						do
						{
							lBytesRead = ContentStream.Read(lBuffer, 0, BUFFER_SIZE);
							if (lBytesRead != 0) connection.Send(lBuffer, 0, lBytesRead);
						}
						while (lBytesRead > 0);

						if (CloseStream)
							ContentStream.Close();

						TriggerOnTransferEnd(TransferDirection.Send);
						break;

					case ContentSource.ContentNone:
						// No action needed
						break;
				}

				if (HasOnTransferProgress)
					connection.OnBytesSent -= HandleOnBytesSent;
			}
			finally
			{
				fContentBytes = null;
				fContentStream = null;
				fContentString = null;
			}
		}
		#endregion
	}
	#endregion

	public class HttpServerRequest : HttpIncomingRequestResponse
	{
		private readonly String fPath;
		private readonly String fQuery;

		public HttpServerRequest(Connection connection, HttpHeaders headers)
			: base(connection, headers)
		{
			String lPath = Header.RequestPath;
			Int32 lStart = lPath.IndexOf("?");

			if (lStart > -1)
			{
				fQuery = lPath.Substring(lStart + 1);
				fPath = lPath.Substring(0, lStart);
			}
			else
			{
				fPath = lPath;
			}
		}

		protected override Boolean Client
		{
			get
			{
				return false;
			}
		}

		protected override Boolean Server
		{
			get
			{
				return true;
			}
		}

		public override void Validate()
		{
			base.Validate();
			if (Header.RequestVersion != "1.1")
			{
				return;
			}

			if (Header.GetHeaderValue("Host") == null)
				throw new HttpRequestInvalidException(HttpStatusCode.InternalServerError, "Bad Request: No Host Header");
		}

		public QueryString QueryString
		{
			get
			{
				return fQueryString ?? (fQueryString = new QueryString(fQuery));
			}
		}
		private QueryString fQueryString;

		public String Path
		{
			get
			{
				return fPath;
			}
		}
	}

	public class HttpServerResponse : HttpOutgoingRequestResponse
	{
		private const String DEFAULT_SERVER_NAME = "RemObjects Internet Pack for .NET HTTP Server";

		public HttpServerResponse()
			: base(new HttpHeaders())
		{
			this.HttpCode = HttpStatusCode.OK;
		}

		protected override Boolean Client
		{
			get
			{
				return false;
			}
		}

		protected override Boolean Server
		{
			get
			{
				return true;
			}
		}

		#region Properties
		[Obsolete("Access HTTP code using the HttpCode property")]
		public Int32 Code
		{
			get
			{
				return (Int32)this.HttpCode;
			}
			set
			{
				this.HttpCode = (HttpStatusCode)value;
			}
		}

		[Obsolete("Access HTTP code using the HttpCode property")]
		public String Status
		{
			get
			{
				return this.HttpCode.ToString();
			}
			set
			{
				// Read-only property
			}
		}

		public HttpStatusCode HttpCode { get; set; }

		public String ResponseText
		{
			get
			{
				return this.HttpCode.ToString();
			}
		}
		#endregion

		#region Methods
		[Obsolete("Provide HTTP code using System.Net.HttpStatusCode value as 1st parameter")]
		public void SendError(Int32 responseCode, String message)
		{
			this.SendError((HttpStatusCode)responseCode, message);
		}

		[Obsolete("Provide HTTP code using System.Net.HttpStatusCode value as 1st parameter")]
		public void SendError(Int32 responseCode, Exception ex)
		{
			this.SendError((HttpStatusCode)responseCode, ex);
		}

		[Obsolete("Provide HTTP code using System.Net.HttpStatusCode value as 1st parameter")]
		public void SendErrorWithCustomBody(Int32 responseCode, String message)
		{
			this.SendError((HttpStatusCode)responseCode, message);
		}

		public void SendError(HttpStatusCode responseCode, String message)
		{
			this.HttpCode = responseCode;

			String lMessageHtml = String.Format("<h1>Error {0} {1}</h1><p>{2}</p><hr /><p>{3}</p>", this.HttpCode, this.ResponseText, message, DEFAULT_SERVER_NAME);
			this.ContentBytes = Encoding.ASCII.GetBytes(lMessageHtml);

			this.Header.ContentType = "text/html";
		}

		public void SendError(HttpStatusCode responseCode, Exception ex)
		{
			this.HttpCode = responseCode;

#if FULLFRAMEWORK
			String lMessageHtml = String.Format("<h1>Error {0} {1}</h1><p>{2}: {3}</p><p>{4}</p><hr /><p>{3}</p>",
													responseCode, ex.GetType().Name, ex.GetType().FullName, ex.Message, ex.StackTrace);
#else
			String lMessageHtml = String.Format("<h1>Error {0} {1}</h1><p>{2}: {3}</p><p>{4}</p><hr />",
				responseCode, ex.ToString(), ex.ToString(), ex.Message, DEFAULT_SERVER_NAME);
#endif
			this.ContentBytes = Encoding.ASCII.GetBytes(lMessageHtml);
			this.Header.ContentType = "text/html";
		}

		public void SendErrorWithCustomBody(HttpStatusCode responseCode, String message)
		{
			this.HttpCode = responseCode;
			this.ContentBytes = Encoding.ASCII.GetBytes(message);
			this.Header.ContentType = "text/html";
		}

		public override void FinalizeHeader()
		{
			base.FinalizeHeader();
			this.Header.SetResponseHeader("1.1", this.HttpCode);
		}
		#endregion
	}

	public class HttpClientRequest : HttpOutgoingRequestResponse
	{
		public HttpClientRequest()
		{
			this.RequestType = RequestType.Get;
		}

		protected override Boolean Client
		{
			get
			{
				return true;
			}
		}

		protected override Boolean Server
		{
			get
			{
				return false;
			}
		}

		#region Properties
		public Url URL { get; set; }

		public Boolean UseProxy { get; set; }

		public RequestType RequestType { get; set; }
		#endregion

		#region Methods
		public override void FinalizeHeader()
		{
			base.FinalizeHeader();

			String lRequestTypeString;
			switch (this.RequestType)
			{
				case RequestType.Get:
					lRequestTypeString = "GET";
					break;

				case RequestType.Post:
					lRequestTypeString = "POST";
					break;

				case RequestType.Put:
					lRequestTypeString = "PUT";
					break;

				case RequestType.Delete:
					lRequestTypeString = "DELETE";
					break;

				case RequestType.Head:
					lRequestTypeString = "HEAD";
					break;

				default:
					throw new HttpHeaderException("Invalid Request Type specified");
			}

			// If connection goes thru proxy we have to provide full target Url (it is used by proxy to forward request).
			// Otherwise only path (fe '/bin') is needed
			// Always providing full target Url will breack the backward compatibility
			this.Header.SetRequestHeader("1.1", lRequestTypeString, this.UseProxy ? URL.ToAbsoluteString() : URL.PathAndQueryString);

			this.Header.SetHeaderValue("Host", URL.HostAndPort);
		}
		#endregion
	}

	public class HttpClientResponse : HttpIncomingRequestResponse, IDisposable
	{
		protected internal HttpClientResponse(Connection connection, HttpHeaders headers)
			: base(connection, headers)
		{
		}

		protected override Boolean Client
		{
			get
			{
				return true;
			}
		}

		protected override Boolean Server
		{
			get
			{
				return false;
			}
		}

		#region Properties
		[Obsolete("Access HTTP code using the HttpCode property")]
		public Int32 Code
		{
			get
			{
				return (Int32)this.Header.HttpCode;
			}
		}

		public HttpStatusCode HttpCode
		{
			get
			{
				return this.Header.HttpCode;
			}
		}
		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			if (KeepAlive)
			{
				if (FlushContent())
				{
					if (DataConnection.Pool != null)
						DataConnection.Pool.ReleaseConnection(DataConnection);
				}
				else
				{
					DataConnection.Dispose();
				}
			}
			else
			{
				DataConnection.Dispose();
			}
		}
		#endregion
	}

	public class HttpIncomingStream : Stream
	{
		public HttpIncomingStream(HttpIncomingRequestResponse owner)
		{
			if (owner.Chunked)
				throw new Exception("ContentStream is currently not supported for Chunked HTTP transfer.");

			this.fOwner = owner;
		}

		private HttpIncomingRequestResponse fOwner;

		internal Boolean FlushContent()
		{
			Int32 lLeft = (Int32)Length - (Int32)fPosition;

			if (lLeft > 16 * 1024)
				return false;

			fOwner.DataConnection.SkipBytes(lLeft);

			return true;
		}

		#region System.IO.Stream Methods
		public override void Flush()
		{
			/* no-op? */
		}

		public override void Close()
		{
			/* no-op? */
		}

		public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
		{
			if (size > Length - fPosition) size = (Int32)Length - (Int32)fPosition;

			if (size <= 0) return 0;
			Int32 lResult = fOwner.DataConnection.Receive(buffer, offset, size);
			fPosition += lResult;
			return lResult;
		}

		public override Int64 Seek(Int64 offset, SeekOrigin origin)
		{
			throw new Exception(String.Format("{0} does not support seeking", this.ToString()));
		}

		/*public override void SetLength(Int64 length)
		{
			throw new Exception(String.Format("{0} does not support SetLength", this.GetType().FullName));
		}*/

		public override Int32 Write(Byte[] buffer, Int32 offset, Int32 size)
		{
			throw new Exception(String.Format("{0} is a read-only Stream", this.ToString()));
		}

		public override Boolean CanRead
		{
			get
			{
				return true;
			}
		}

		public override Boolean CanSeek
		{
			get
			{
				return false;
			}
		}

		public override Boolean CanWrite
		{
			get
			{
				return false;
			}
		}

		public override Int64 Length
		{
			get
			{
				return fOwner.ContentLength;
			}
		}

		public override Int64 Position
		{
			get
			{
				return fPosition;
			}
			set
			{
				Seek(value, SeekOrigin.Begin);
				fPosition = value;
			}
		}
		private Int64 fPosition;
		#endregion
	}

	public enum ContentSource
	{
		ContentNone,
		ContentBytes,
		ContentStream,
		ContentString
	}

	public enum RequestType
	{
		Get,
		Post,
		Put,
		Delete,
		Head
	}

	public class QueryString
	{
		private readonly String fQuery;
        private Dictionary<String, String> fData;

		public QueryString(String query)
		{
			fData = new Dictionary<String, String>();
            fQuery = query;
			if (String.IsNullOrEmpty(fQuery))
			{
				return;
			}

			var lParams = fQuery.Split("&");
			foreach (string lParam in lParams)
			{
				Int32 lEqual = lParam.IndexOf('=');
				if (lEqual > -1)
				{
					String lName = lParam.Substring(0, lEqual).Trim().ToLower();
					String lValue = lParam.Substring(lEqual + 1);
					fData[lName] = fData.ContainsKey(lName) ? fData[lName] + "," + lValue : lValue;
				}
				else
				{
					fData.Add(lParam, null);
				}
			}
		}

		#if echoes || island
        public override String ToString()
        #else
        public String ToString()
        #endif
		{
			return this.fQuery ?? "";
		}

		public new String this[object key]
		{
			get
			{
				return fData[(String)key];
			}
		}

	}
}