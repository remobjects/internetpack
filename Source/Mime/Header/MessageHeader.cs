// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//

using RemObjects.InternetPack.Messages.Mime.Decode;

namespace RemObjects.InternetPack.Messages.Mime.Header
{
	/// <summary>
	/// Class that holds all headers for a message<br/>
	/// Headers which are unknown the the parser will be held in the <see cref="UnknownHeaders"/> collection.<br/>
	/// <br/>
	/// This class cannot be instantiated from outside the library.
	/// </summary>
	/// <remarks>
	/// See <a href="http://tools.ietf.org/html/rfc4021">RFC 4021</a> for a large list of headers.<br/>
	/// </remarks>
	public sealed class MessageHeader
	{
		#region Properties
		/// <summary>
		/// All headers which were not recognized and explicitly dealt with.<br/>
		/// This should mostly be custom headers, which are marked as X-[name].<br/>
		/// <br/>
		/// This list will be empty if all headers were recognized and parsed.
		/// </summary>
		/// <remarks>
		/// If you as a user, feels that a header in this collection should
		/// be parsed, feel free to notify the developers.
		/// </remarks>
		public NameValueCollection UnknownHeaders
		{
			get
			{
				return fUnknownHeaders;
			}
			private set
			{
				fUnknownHeaders = value;
			}
		}
		private NameValueCollection fUnknownHeaders;

		/// <summary>
		/// A human readable description of the body<br/>
		/// <br/>
		/// <see langword="null"/> if no Content-Description header was present in the message.
		/// </summary>
		public String ContentDescription
		{
			get
			{
				return fContentDescription;
			}
			private set
			{
				fContentDescription = value;
			}
		}
		private String fContentDescription;

		/// <summary>
		/// ID of the content part (like an attached image). Used with MultiPart messages.<br/>
		/// <br/>
		/// <see langword="null"/> if no Content-ID header field was present in the message.
		/// </summary>
		/// <see cref="MessageId">For an ID of the message</see>
		public String ContentId
		{
			get
			{
				return fContentId;
			}
			private set
			{
				fContentId = value;
			}
		}
		private String fContentId;

		/// <summary>
		/// Message keywords<br/>
		/// <br/>
		/// The list will be empty if no Keywords header was present in the message
		/// </summary>
		public List<String> Keywords
		{
			get
			{
				return fKeywords;
			}
			private set
			{
				fKeywords = value;
			}
		}
		private List<String> fKeywords;

		/// <summary>
		/// A List of emails to people who wishes to be notified when some event happens.<br/>
		/// These events could be email:
		/// <list type="bullet">
		///   <item>deletion</item>
		///   <item>printing</item>
		///   <item>received</item>
		///   <item>...</item>
		/// </list>
		/// The list will be empty if no Disposition-Notification-To header was present in the message
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc3798">RFC 3798</a> for details</remarks>
		public MessageAddresses DispositionNotificationTo
		{
			get
			{
				return fDispositionNotificationTo;
			}
			private set
			{
				fDispositionNotificationTo = value;
			}
		}
		private MessageAddresses fDispositionNotificationTo;

		/// <summary>
		/// This is the Received headers. This tells the path that the email went.<br/>
		/// <br/>
		/// The list will be empty if no Received header was present in the message
		/// </summary>
		public List<String> Received
		{
			get
			{
				return fReceived;
			}
			private set
			{
				fReceived = value;
			}
		}
		private List<String> fReceived;

		/// <summary>
		/// Importance of this email.<br/>
		/// <br/>
		/// The importance level is set to normal, if no Importance header field was mentioned or it contained
		/// unknown information. This is the expected behavior according to the RFC.
		/// </summary>
		public MailPriority Importance
		{
			get
			{
				return fImportance;
			}
			private set
			{
				fImportance = value;
			}
		}
		private MailPriority fImportance;

		/// <summary>
		/// This header describes the Content encoding during transfer.<br/>
		/// <br/>
		/// If no Content-Transfer-Encoding header was present in the message, it is set
		/// to the default of <see cref="Header.ContentTransferEncoding.SevenBit">SevenBit</see> in accordance to the RFC.
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc2045#section-6">RFC 2045 section 6</a> for details</remarks>
		public ContentTransferEncoding ContentTransferEncoding
		{
			get
			{
				return fContentTransferEncoding;
			}
			private set
			{
				fContentTransferEncoding = value;
			}
		}
		private ContentTransferEncoding fContentTransferEncoding;

		/// <summary>
		/// Carbon Copy. This specifies who got a copy of the message.<br/>
		/// <br/>
		/// The list will be empty if no Cc header was present in the message
		/// </summary>
		public MessageAddresses Cc
		{
			get
			{
				return fCc;
			}
			private set
			{
				fCc = value;
			}
		}
		private MessageAddresses fCc;

		/// <summary>
		/// Blind Carbon Copy. This specifies who got a copy of the message, but others
		/// cannot see who these persons are.<br/>
		/// <br/>
		/// The list will be empty if no Received Bcc was present in the message
		/// </summary>
		public MessageAddresses Bcc
		{
			get
			{
				return fBcc;
			}
			private set
			{
				fBcc = value;
			}
		}
		private MessageAddresses fBcc;

		/// <summary>
		/// Specifies who this mail was for<br/>
		/// <br/>
		/// The list will be empty if no To header was present in the message
		/// </summary>
		public MessageAddresses To
		{
			get
			{
				return fTo;
			}
			private set
			{
				fTo = value;
			}
		}
		private MessageAddresses fTo;

		/// <summary>
		/// Specifies who sent the email<br/>
		/// <br/>
		/// <see langword="null"/> if no From header field was present in the message
		/// </summary>
		public MessageAddress From
		{
			get
			{
				return fFrom;
			}
			private set
			{
				fFrom = value;
			}
		}
		private MessageAddress fFrom;

		/// <summary>
		/// Specifies who a reply to the message should be sent to<br/>
		/// <br/>
		/// <see langword="null"/> if no Reply-To header field was present in the message
		/// </summary>
		public MessageAddress ReplyTo
		{
			get
			{
				return fReplyTo;
			}
			private set
			{
				fReplyTo = value;
			}
		}
		private MessageAddress fReplyTo;

		/// <summary>
		/// The message identifier(s) of the original message(s) to which the
		/// current message is a reply.<br/>
		/// <br/>
		/// The list will be empty if no In-Reply-To header was present in the message
		/// </summary>
		public List<String> InReplyTo
		{
			get
			{
				return fInReplyTo;
			}
			private set
			{
				fInReplyTo = value;
			}
		}
		private List<String> fInReplyTo;

		/// <summary>
		/// The message identifier(s) of other message(s) to which the current
		/// message is related to.<br/>
		/// <br/>
		/// The list will be empty if no References header was present in the message
		/// </summary>
		public List<String> References
		{
			get
			{
				return fReferences;
			}
			private set
			{
				fReferences = value;
			}
		}
		private List<String> fReferences;

		/// <summary>
		/// This is the sender of the email address.<br/>
		/// <br/>
		/// <see langword="null"/> if no Sender header field was present in the message
		/// </summary>
		/// <remarks>
		/// The RFC states that this field can be used if a secretary
		/// is sending an email for someone she is working for.
		/// The email here will then be the secretary's email, and
		/// the Reply-To field would hold the address of the person she works for.<br/>
		/// RFC states that if the Sender is the same as the From field,
		/// sender should not be included in the message.
		/// </remarks>
		public MessageAddress Sender
		{
			get
			{
				return fSender;
			}
			private set
			{
				fSender = value;
			}
		}
		private MessageAddress fSender;

		/// <summary>
		/// The Content-Type header field.<br/>
		/// <br/>
		/// If not set, the ContentType is created by the default "text/plain; charset=us-ascii" which is
		/// defined in <a href="http://tools.ietf.org/html/rfc2045#section-5.2">RFC 2045 section 5.2</a>.<br/>
		/// If set, the default is overridden.
		/// </summary>
		public ContentType ContentType
		{
			get
			{
				return fContentType;
			}
			private set
			{
				fContentType = value;
			}
		}
		private ContentType fContentType;

		/// <summary>
		/// Used to describe if a <see cref="MessagePart"/> is to be displayed or to be though of as an attachment.<br/>
		/// Also contains information about filename if such was sent.<br/>
		/// <br/>
		/// <see langword="null"/> if no Content-Disposition header field was present in the message
		/// </summary>
		public ContentDisposition ContentDisposition
		{
			get
			{
				return fContentDisposition;
			}
			private set
			{
				fContentDisposition = value;
			}
		}
		private ContentDisposition fContentDisposition;

		/// <summary>
		/// The Date when the email was sent.<br/>
		/// This is the raw value. <see cref="DateSent"/> for a parsed up <see cref="DateTime"/> value of this field.<br/>
		/// <br/>
		/// <see langword="null"/> if no Date header field was present in the message
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc5322#section-3.6.1">RFC 5322 section 3.6.1</a> for more details</remarks>
		public String Date
		{
			get
			{
				return fDate;
			}
			private set
			{
				fDate = value;
			}
		}
		private String fDate;

		/// <summary>
		/// The Date when the email was sent.<br/>
		/// This is the parsed equivalent of <see cref="Date"/>.<br/>
		/// Notice that the <see cref="TimeZone"/> of the <see cref="DateTime"/> object is in UTC and has NOT been converted
		/// to local <see cref="TimeZone"/>.
		/// </summary>
		/// <remarks>See <a href="http://tools.ietf.org/html/rfc5322#section-3.6.1">RFC 5322 section 3.6.1</a> for more details</remarks>
		public DateTime DateSent
		{
			get
			{
				return fDateSent;
			}
			private set
			{
				fDateSent = value;
			}
		}
		private DateTime fDateSent;

		/// <summary>
		/// An ID of the message that is SUPPOSED to be in every message according to the RFC.<br/>
		/// The ID is unique.<br/>
		/// <br/>
		/// <see langword="null"/> if no Message-ID header field was present in the message
		/// </summary>
		public String MessageId
		{
			get
			{
				return fMessageId;
			}
			private set
			{
				fMessageId = value;
			}
		}
		private String fMessageId;

		/// <summary>
		/// The Mime Version.<br/>
		/// This field will almost always show 1.0<br/>
		/// <br/>
		/// <see langword="null"/> if no Mime-Version header field was present in the message
		/// </summary>
		public String MimeVersion
		{
			get
			{
				return fMimeVersion;
			}
			private set
			{
				fMimeVersion = value;
			}
		}
		private String fMimeVersion;

		/// <summary>
		/// A single Mail Address with no username inside.<br/>
		/// This is a trace header field, that should be in all messages.<br/>
		/// Replies should be sent to this address.<br/>
		/// <br/>
		/// <see langword="null"/> if no Return-Path header field was present in the message
		/// </summary>
		public MessageAddress ReturnPath
		{
			get
			{
				return fReturnPath;
			}
			private set
			{
				fReturnPath = value;
			}
		}
		private MessageAddress fReturnPath;

		/// <summary>
		/// The subject line of the message in decoded, one line state.<br/>
		/// This should be in all messages.<br/>
		/// <br/>
		/// <see langword="null"/> if no Subject header field was present in the message
		/// </summary>
		public String Subject
		{
			get
			{
				return fSubject;
			}
			private set
			{
				fSubject = value;
			}
		}
		private String fSubject;

		/// <summary>
		/// Represents RequestLine of http response.
		/// Not reqiured cat can appear in some messages.
		/// Example : POST /odata/$batch HTTP/1.1
		/// </summary>
		public String RequestLine
		{
			get
			{
				return fRequestLine;
			}
			private set
			{
				fRequestLine = value;
			}
		}
		private String fRequestLine;
		#endregion

		private readonly Dictionary<String, String> fAllHeaders = new Dictionary<String, String>();

		public MessageHeader()
		{
			// Create empty lists as defaults. We do not like null values
			// List with an initial capacity set to zero will be replaced
			// when a corrosponding header is found
			this.To = new MessageAddresses();
			this.Cc = new MessageAddresses();
			this.Bcc = new MessageAddresses();
			this.Received = new List<String>();
			this.Keywords = new List<String>();
			this.InReplyTo = new List<String>();
			this.References = new List<String>();
			this.DispositionNotificationTo = new MessageAddresses();
			this.UnknownHeaders = new NameValueCollection();
		}

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a MessageHeader
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <returns>A valid MessageHeader object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headers"/> is <see langword="null"/></exception>
		public MessageHeader(NameValueCollection headers)
			: this()
		{
			if (headers == null)
				throw new ArgumentNullException("headers");

			// Default importancetype is Normal (assumed if not set)
			Importance = MailPriority.Normal;

			// 7BIT is the default ContentTransferEncoding (assumed if not set)
			ContentTransferEncoding = ContentTransferEncoding.SevenBit;

			// text/plain; charset=us-ascii is the default ContentType
			//ContentType = new ContentType("text/plain; charset=us-ascii");
			ContentType = new ContentType("application/atom+xml");

			// Now parse the actual headers
			ParseHeaders(headers);
		}

		/// <summary>
		/// Parses a <see cref="NameValueCollection"/> to a <see cref="MessageHeader"/>
		/// </summary>
		/// <param name="headers">The collection that should be traversed and parsed</param>
		/// <returns>A valid <see cref="MessageHeader"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headers"/> is <see langword="null"/></exception>
		private void ParseHeaders(NameValueCollection headers)
		{
			if (headers == null)
				throw new ArgumentNullException("headers");

			// Now begin to parse the header values
			foreach (String headerName in headers.Keys)
			{
				String[] headerValues = headers.GetValues(headerName);
				if (headerValues != null)
				{
					foreach (String headerValue in headerValues)
					{
						ParseHeader(headerName, headerValue);
					}
				}
			}
		}

		private static MessageAddresses ParseAddresses(String input)
		{
			return MessageAddresses.ParseAddresses(Utility.SplitStringWithCharNotInsideQuotes(input, ','));
		}

		private void HeaderValueChanged(String headerName, String headerValue)
		{
			if (fAllHeaders.ContainsKey(headerName))
				fAllHeaders[headerName] = headerValue;
			else
				fAllHeaders.Add(headerName, headerValue);
		}

		#region Header fields parsing
		/// <summary>
		/// Parses a single header and sets member variables according to it.
		/// </summary>
		/// <param name="headerName">The name of the header</param>
		/// <param name="headerValue">The value of the header in unfolded state (only one line)</param>
		/// <exception cref="ArgumentNullException">If <paramref name="headerName"/> or <paramref name="headerValue"/> is <see langword="null"/></exception>
		public void ParseHeader(String headerName, String headerValue)
		{
			if (headerName == null)
				throw new ArgumentNullException("headerName");

			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			HeaderValueChanged(headerName, headerValue);

			switch (headerName.ToUpperInvariant())
			{
				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "TO":
					this.To = MessageHeader.ParseAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "CC":
					this.Cc = MessageHeader.ParseAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.3
				case "BCC":
					this.Bcc = MessageHeader.ParseAddresses(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.2
				case "FROM":
					// There is only one MailAddress in the from field
					this.From = MessageAddress.ParseAddress(headerValue);
					break;

				// http://tools.ietf.org/html/rfc5322#section-3.6.2
				// The implementation here might be wrong
				case "REPLY-TO":
					// This field may actually be a list of addresses, but no
					// such case has been encountered
					this.ReplyTo = MessageAddress.ParseAddress(headerValue);
					break;

				// http://tools.ietf.org/html/rfc5322#section-3.6.2
				case "SENDER":
					this.Sender = MessageAddress.ParseAddress(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.5
				// RFC 5322:
				// The "Keywords:" field contains a comma-separated list of one or more
				// words or quoted-strings.
				// The field are intended to have only human-readable content
				// with information about the message
				case "KEYWORDS":
					var keywordsTemp = headerValue.Split(",");
					foreach (String keyword in keywordsTemp)
					{
						// Remove the quotes if there is any
						Keywords.Add(Utility.RemoveQuotesIfAny(keyword.Trim()));
					}
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.7
				case "RECEIVED":
					// Simply add the value to the list
					Received.Add(headerValue.Trim());
					break;

				case "IMPORTANCE":
					Importance = HeaderFieldParser.ParseImportance(headerValue.Trim());
					break;

				// See http://tools.ietf.org/html/rfc3798#section-2.1
				case "DISPOSITION-NOTIFICATION-TO":
					this.DispositionNotificationTo = MessageHeader.ParseAddresses(headerValue);
					break;

				case "MIME-VERSION":
					MimeVersion = headerValue.Trim();
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.5
				case "SUBJECT":
				case "THREAD-TOPIC":
					Subject = EncodedWord.Decode(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.7
				case "RETURN-PATH":
					// Return-paths does not include a username, but we
					// may still use the address parser
					this.ReturnPath = MessageAddress.ParseAddress(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.4
				// Example Message-ID
				// <33cdd74d6b89ab2250ecd75b40a41405@nfs.eksperten.dk>
				case "MESSAGE-ID":
					MessageId = HeaderFieldParser.ParseId(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.4
				case "IN-REPLY-TO":
					InReplyTo = HeaderFieldParser.ParseMultipleIDs(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.4
				case "REFERENCES":
					References = HeaderFieldParser.ParseMultipleIDs(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc5322#section-3.6.1))
				case "DATE":
					Date = headerValue.Trim();
					DateSent = Rfc2822DateTime.StringToDate(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc2045#section-6
				// See ContentTransferEncoding class for more details
				case "CONTENT-TRANSFER-ENCODING":
					ContentTransferEncoding = HeaderFieldParser.ParseContentTransferEncoding(headerValue.Trim());
					break;

				// See http://tools.ietf.org/html/rfc2045#section-8
				case "CONTENT-DESCRIPTION":
					// Human description of for example a file. Can be encoded
					ContentDescription = EncodedWord.Decode(headerValue.Trim());
					break;

				// See http://tools.ietf.org/html/rfc2045#section-5.1
				// Example: Content-type: text/plain; charset="us-ascii"
				case "CONTENT-TYPE":
					ContentType = HeaderFieldParser.ParseContentType(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc2183
				case "CONTENT-DISPOSITION":
					ContentDisposition = HeaderFieldParser.ParseContentDisposition(headerValue);
					break;

				// See http://tools.ietf.org/html/rfc2045#section-7
				// Example: <foo4*foo1@bar.net>
				case "CONTENT-ID":
					ContentId = HeaderFieldParser.ParseId(headerValue);
					break;

				case "_REQUEST-LINE_":
					// header could contain request-line that could not be parsed as simple key value pair, so store entire line
					// Example: POST /odata/$batch HTTP/1.1
					RequestLine = headerValue;
					break;

				default:
					// This is an unknown header

					// Custom headers are allowed. That means headers
					// that are not mentionen in the RFC.
					// Such headers start with the letter "X"
					// We do not have any special parsing of such

					// Add it to unknown headers
					UnknownHeaders.Add(headerName, headerValue);
					break;
			}
		}
		#endregion

		public String this[String name]
		{
			get
			{
				if (this.fAllHeaders.ContainsKey(name))
					return fAllHeaders[name];

				return null;
			}
			set
			{
				this.ParseHeader(name, value);
			}
		}

		[ToString]
        public override String ToString()
		{
			StringBuilder lResult = new StringBuilder();
			Store(lResult);
			return lResult.ToString();
		}

		public void Store(StringBuilder sb)
		{
			foreach (string key in fAllHeaders.Keys)
				if (!key.StartsWith("_"))
					sb.Append(String.Format("{0}:{1}\r\n", key, fAllHeaders[key]));
		}
	}

	/// <summary>
	/// <see cref="Enum"/> that describes the ContentTransferEncoding header field
	/// </summary>
	/// <remarks>See <a href="http://tools.ietf.org/html/rfc2045#section-6">RFC 2045 section 6</a> for more details</remarks>
	public enum ContentTransferEncoding
	{
		/// <summary>
		/// 7 bit Encoding
		/// </summary>
		SevenBit,

		/// <summary>
		/// 8 bit Encoding
		/// </summary>
		EightBit,

		/// <summary>
		/// Quoted Printable Encoding
		/// </summary>
		QuotedPrintable,

		/// <summary>
		/// Base64 Encoding
		/// </summary>
		Base64,

		/// <summary>
		/// Binary Encoding
		/// </summary>
		Binary
	}
}