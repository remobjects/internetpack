// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//

using System;
using System.Text;
using RemObjects.InternetPack.Messages.Mime.Header;

namespace RemObjects.InternetPack.Messages.Mime
{
	/// <summary>
	/// This is the root of the email tree structure.<br/>
	/// <see cref="Mime.MessagePart"/> for a description about the structure.<br/>
	/// <br/>
	/// A Message (this class) contains the headers of an email message such as:
	/// <code>
	///  - To
	///  - From
	///  - Subject
	///  - Content-Type
	///  - Message-ID
	/// </code>
	/// which are located in the <see cref="Headers"/> property.<br/>
	/// <br/>
	/// Use the <see cref="MimeMessage.MessagePart"/> property to find the actual content of the email message.
	/// </summary>
	/// <example>
	/// Examples are available on the <a href="http://hpop.sourceforge.net/">project homepage</a>.
	/// </example>
	public class MimeMessage
	{
		#region Public properties
		/// <summary>
		/// Headers of the Message.
		/// </summary>
		public MessageHeader Headers
		{
			get
			{
				return fHeaders;
			}
			private set
			{
				fHeaders = value;
			}
		}
		private MessageHeader fHeaders;

		/// <summary>
		/// This is the body of the email Message.<br/>
		/// <br/>
		/// If the body was parsed for this Message, this property will never be <see langword="null"/>.
		/// </summary>
		public MessagePart MessagePart
		{
			get
			{
				return fMessagePart;
			}
			private set
			{
				fMessagePart = value;
			}
		}
		private MessagePart fMessagePart;

		/// <summary>
		/// The raw content from which this message has been constructed.<br/>
		/// These bytes can be persisted and later used to recreate the Message.
		/// </summary>
		public Byte[] RawMessage
		{
			get
			{
				return fRawMessage;
			}
			private set
			{
				fRawMessage = value;
			}
		}
		private Byte[] fRawMessage;
		#endregion

		#region Constructors
		// todo check if can remove this
		public MimeMessage()
		{
		}

		/// <summary>
		/// Convenience constructor for <see cref="Mime.MimeMessage(Byte[], Boolean)"/>.<br/>
		/// <br/>
		/// Creates a message from a Byte array. The full message including its body is parsed.
		/// </summary>
		/// <param name="rawMessageContent">The Byte array which is the message contents to parse</param>
		public MimeMessage(Byte[] rawMessageContent)
			: this(rawMessageContent, true)
		{
		}

		/// <summary>
		/// Constructs a message from a Byte array.<br/>
		/// <br/>
		/// The headers are always parsed, but if <paramref name="parseBody"/> is <see langword="false"/>, the body is not parsed.
		/// </summary>
		/// <param name="rawMessageContent">The Byte array which is the message contents to parse</param>
		/// <param name="parseBody"><see langword="true"/> if the body should be parsed, <see langword="false"/> if only headers should be parsed out of the <paramref name="rawMessageContent"/> Byte array</param>
		public MimeMessage(Byte[] rawMessageContent, Boolean parseBody)
		{
			RawMessage = rawMessageContent;

			// Find the headers and the body parts of the Byte array
			MessageHeader headersTemp;
			Byte[] body;
			HeaderExtractor.ExtractHeadersAndBody(rawMessageContent, out headersTemp, out body);

			// Set the Headers property
			Headers = headersTemp;

			// Should we also parse the body?
			if (parseBody)
			{
				// Parse the body into a MessagePart
				MessagePart = new MessagePart(body, Headers);
			}
		}
		#endregion

		public void ToHttpResponseBody(StringBuilder sb)
		{
			foreach (MessagePart part in this.MessagePart.MessageParts)
			{
				sb.AppendFormat("--{0}\r\n", this.Headers.ContentType.Boundary);
				part.ToHttpResponseBody(sb);
			}
			sb.AppendFormat("--{0}--\r\n", this.Headers.ContentType.Boundary);
		}
	}
}