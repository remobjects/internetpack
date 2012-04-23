/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
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

        public HttpHeaders Header
        {
            get
            {
                return fHeader;
            }
            set
            {
                fHeader = value;
            }
        }
        private HttpHeaders fHeader;

        public Encoding Encoding
        {
            get
            {
                return fEncoding;
            }
            set
            {
                fEncoding = value;
            }
        }
        private Encoding fEncoding;

        protected abstract Boolean Client { get; }

        protected abstract Boolean Server { get; }

        #region Events
        public event TransferStartEventHandler OnTransferStart;

        public event TransferEndEventHandler OnTransferEnd;

        public event TransferProgressEventHandler OnTransferProgress;

        public void CloneEvents(HttpRequestResponse source)
        {
            OnTransferStart = source.OnTransferStart;
            OnTransferEnd = source.OnTransferEnd;
            OnTransferProgress = source.OnTransferProgress;
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

        public Connection DataConnection
        {
            get
            {
                return fDataConnection;
            }
            private set
            {
                this.fDataConnection = value;
            }
        }
        private Connection fDataConnection;

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

                            // skip footer
                            while (DataConnection.ReadLine() != "") ;
                        }
                        else
                        {
                            Int32 lRead = 0;
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
                        Int32.Parse(lLength);
                        return true;
                    }
                    catch (Exception)
                    {
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

                    fContentLength = Int32.Parse(lLength);
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

                return lConnection != null && lConnection.ToLower(CultureInfo.InvariantCulture) == "keep-alive";
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
                    throw new HttpRequestInvalidException(500, "Bad Request: Chunked encoding not supported, yet");
            }
        }

        private Int32 ReadChunkSize()
        {
            String lLine = DataConnection.ReadLine();
            Int32 lPos = lLine.IndexOf(';');
            if (lPos >= 0)
                lLine = lLine.Substring(0, lPos);
            lLine = lLine.Trim();

            return Int32.Parse(lLine, NumberStyles.AllowHexSpecifier);
        }

    }

    public abstract class HttpOutgoingRequestResponse : HttpRequestResponse
    {
        protected internal HttpOutgoingRequestResponse()
            : base()
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
        public ContentSource ContentSource
        {
            get
            {
                return fContentSource;
            }
            set
            {
                fContentSource = value;
            }
        }
        private ContentSource fContentSource;

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

        public Boolean CloseStream
        {
            get
            {
                return fCloseStream;
            }
            set
            {
                fCloseStream = value;
            }
        }
        private Boolean fCloseStream;

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
                Byte[] lBuffer;

                if (HasOnTransferProgress)
                {
                    connection.OnBytesSent += new EventHandler(HandleOnBytesSent);
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
                        lBuffer = new Byte[BUFFER_SIZE];
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
                        /* Nothing to do */
                        break;
                }

                if (HasOnTransferProgress)
                    connection.OnBytesSent -= new EventHandler(HandleOnBytesSent);
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
        protected internal HttpServerRequest(Connection connection, HttpHeaders headers)
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
            if (Header.RequestVersion == "1.1")
            {
                if (Header.GetHeaderValue("Host") == null)
                    throw new HttpRequestInvalidException(500, "Bad Request: No Host Header");
            }
        }

        private String fPath;
        private String fQuery;

        public QueryString QueryString
        {
            get
            {
                if (fQueryString == null)
                    fQueryString = new QueryString(fQuery);
                return fQueryString;
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
        protected internal HttpServerResponse()
            : base(new HttpHeaders())
        {
            Code = 200;
            ResponseText = "OK";
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
        public Int32 Code
        {
            get
            {
                return fCode;
            }
            set
            {
                fCode = value;
            }
        }
        private Int32 fCode;

        public String ResponseText
        {
            get
            {
                return fResponseText;
            }
            set
            {
                fResponseText = value;
            }
        }
        private String fResponseText;
        #endregion

        #region Methods
        public void SendError(Int32 responseCode, String responseText, String message)
        {
            this.Code = responseCode;
            this.ResponseText = responseText;

            String lMessageHtml = String.Format("<h1>Error {0} {1}</h1><p>{2}</p><hr /><p>{3}</p>", responseCode, responseText, message, DEFAULT_SERVER_NAME);
            this.ContentBytes = Encoding.ASCII.GetBytes(lMessageHtml);

            this.Header.ContentType = "text/html";
        }

        public void SendError(Int32 responseCode, String message)
        {
            SendError(responseCode, "ERROR", message);
        }

        public void SendError(Int32 responseCode, String responseText, Exception ex)
        {
            this.Code = responseCode;
            this.ResponseText = responseText;
#if FULLFRAMEWORK
            String lMessageHtml = String.Format("<h1>Error {0} {1}</h1><p>{2}: {3}</p><p>{4}</p><hr /><p>{3}</p>",
                responseCode, ex.GetType().Name, ex.GetType().FullName, ex.Message, ex.StackTrace.ToString(), DEFAULT_SERVER_NAME);
#else
            String lMessageHtml = String.Format("<h1>Error {0} {1}</h1><p>{2}: {3}</p><p>{4}</p><hr />",
                responseCode, ex.GetType().Name, ex.GetType().FullName, ex.Message, DEFAULT_SERVER_NAME);
#endif
            this.ContentBytes = Encoding.ASCII.GetBytes(lMessageHtml);
            this.Header.ContentType = "text/html";
        }

        public void SendErrorWithCustomBody(Int32 responseCode, String responseText, String message)
        {
            this.Code = responseCode;
            this.ResponseText = responseText;
            this.ContentBytes = Encoding.ASCII.GetBytes(message);
            this.Header.ContentType = "text/html";
        }

        private const String DEFAULT_SERVER_NAME = "RemObjects Internet Pack for .NET HTTP Server";

        public override void FinalizeHeader()
        {
            base.FinalizeHeader();
            Header.SetResponseHeader("1.1", Code, ResponseText);
        }
        #endregion
    }

    public class HttpClientRequest : HttpOutgoingRequestResponse
    {
        public HttpClientRequest()
            : base()
        {
            this.RequestType = RequestType.Get;
            this.Url = new UrlParser();
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
        public UrlParser Url
        {
            get
            {
                return this.fUrl;
            }
            set
            {
                this.fUrl = value;
            }
        }
        private UrlParser fUrl;

        public Boolean UseProxy
        {
            get
            {
                return this.fUseProxy;
            }
            set
            {
                this.fUseProxy = value;
            }
        }
        private Boolean fUseProxy;

        public RequestType RequestType
        {
            get
            {
                return this.fRequestType;
            }
            set
            {
                this.fRequestType = value;
            }
        }
        private RequestType fRequestType;
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
            this.Header.SetRequestHeader("1.1", lRequestTypeString, this.UseProxy ? this.Url.ToString() : this.Url.PathAndParams);

            this.Header.SetHeaderValue("Host", this.Url.HostnameAndPort);
        }
        #endregion
    }

    public class HttpClientResponse : HttpIncomingRequestResponse, IDisposable
    {
        protected internal HttpClientResponse(Connection connection, HttpHeaders headers)
            : base(connection, headers)
        {
            this.fCode = Int32.Parse(Header.ResponseCode);
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
        public Int32 Code
        {
            get
            {
                return fCode;
            }
        }
        private Int32 fCode;
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
            throw new Exception(String.Format("{0} does not support seeking", this.GetType().FullName));
        }

        public override void SetLength(Int64 length)
        {
            throw new Exception(String.Format("{0} does not support SetLength", this.GetType().FullName));
        }

        public override void Write(Byte[] buffer, Int32 offset, Int32 size)
        {
            throw new Exception(String.Format("{0} is a read-only Stream", this.GetType().FullName));
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

    public class QueryString : Hashtable
    {
        public QueryString(String query)
            : base()
        {
            fQuery = query;
            if (fQuery != null && fQuery.Length > 0)
            {
                String[] lParams = fQuery.Split(new char[] { '&' });
                for (Int32 i = 0; i < lParams.Length; i++)
                {
                    Int32 lEqual = lParams[i].IndexOf('=');
                    if (lEqual > -1)
                    {
                        String lName = lParams[i].Substring(0, lEqual).Trim().ToLower();
                        String lValue = lParams[i].Substring(lEqual + 1);
                        if (ContainsKey(lName))
                            base[lName] = this[lName] + "," + lValue;
                        else
                            base[lName] = lValue;
                    }
                    else
                    {
                        this.Add(lParams[i], null);
                    }
                }
            }

        }

        private String fQuery;

        public override String ToString()
        {
            if (fQuery == null)
                return "";

            return fQuery;
        }

        public new String this[object key]
        {
            get
            {
                return (String)base[key];
            }
        }

    }

    [Serializable]
    public class HttpRequestInvalidException : System.ApplicationException
    {
        public HttpRequestInvalidException(Int32 errorCode, String errorMessage)
            : this(errorCode, errorMessage, null)
        {
        }

        public HttpRequestInvalidException(Int32 errorCode, String errorMessage, Exception innerException)
            : base(errorCode.ToString() + " " + errorMessage, innerException)
        {
            this.fResponse = new HttpServerResponse();
            this.fResponse.Code = errorCode;
            this.fResponse.ResponseText = errorMessage;
            this.fResponse.ContentString = errorMessage;
        }

        public HttpServerResponse Response
        {
            get
            {
                return this.fResponse;
            }
        }
        private readonly HttpServerResponse fResponse;
    }
}
