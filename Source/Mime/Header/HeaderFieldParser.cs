// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//

using RemObjects.InternetPack.Messages.Mime.Decode;
using RemObjects.InternetPack.Messages.Mime;

namespace RemObjects.InternetPack.Messages.Mime.Header
{
	/// <summary>
	/// Class that can parse different fields in the header sections of a MIME message.
	/// </summary>
	static class HeaderFieldParser
	{
		/// <summary>
		/// Parses the Content-Transfer-Encoding header.
		/// </summary>
		/// <param name="headerValue">The value for the header to be parsed</param>
		/// <returns>A <see cref="ContentTransferEncoding"/></returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headerValue"/> is <see langword="null"/></exception>
		/// <exception cref="ArgumentException">If the <paramref name="headerValue"/> could not be parsed to a <see cref="ContentTransferEncoding"/></exception>
		public static ContentTransferEncoding ParseContentTransferEncoding(String headerValue)
		{
			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			switch (headerValue.Trim().ToUpperInvariant())
			{
				case "7BIT":
					return ContentTransferEncoding.SevenBit;

				case "8BIT":
					return ContentTransferEncoding.EightBit;

				case "QUOTED-PRINTABLE":
					return ContentTransferEncoding.QuotedPrintable;

				case "BASE64":
					return ContentTransferEncoding.Base64;

				case "BINARY":
					return ContentTransferEncoding.Binary;

				// If a wrong argument is passed to this parser method, then we assume
				// default encoding, which is SevenBit.
				// This is to ensure that we do not throw exceptions, even if the email not MIME valid.
				default:
					return ContentTransferEncoding.SevenBit;
			}
		}

		/// <summary>
		/// Parses an ImportanceType from a given Importance header value.
		/// </summary>
		/// <param name="headerValue">The value to be parsed</param>
		/// <returns>A <see cref="MailPriority"/>. If the <paramref name="headerValue"/> is not recognized, Normal is returned.</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headerValue"/> is <see langword="null"/></exception>
		public static MailPriority ParseImportance(String headerValue)
		{
			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			switch (headerValue.ToUpperInvariant())
			{
				case "5":
				case "HIGH":
					return MailPriority.High;

				case "3":
				case "NORMAL":
					return MailPriority.Normal;

				case "1":
				case "LOW":
					return MailPriority.Low;

				default:
					return MailPriority.Normal;
			}
		}

		/// <summary>
		/// Parses a the value for the header Content-Type to
		/// a <see cref="ContentType"/> object.
		/// </summary>
		/// <param name="headerValue">The value to be parsed</param>
		/// <returns>A <see cref="ContentType"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headerValue"/> is <see langword="null"/></exception>
		public static ContentType ParseContentType(String headerValue)
		{
			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			// We create an empty Content-Type which we will fill in when we see the values
			ContentType contentType = new ContentType();

			// Now decode the parameters
			List<KeyValuePair<String, String>> parameters = Rfc2231Decoder.Decode(headerValue);

			foreach (KeyValuePair<String, String> keyValuePair in parameters)
			{
				String key = keyValuePair.Key.ToUpperInvariant().Trim();
				String value = keyValuePair.Value.Trim();
				switch (key)
				{
					case "":
						// This is the MediaType - it has no key since it is the first one mentioned in the
						// headerValue and has no = in it.
						contentType.MediaType = value;
						break;

					case "BOUNDARY":
						contentType.Boundary = Utility.RemoveQuotesIfAny(value);
						break;

					case "CHARSET":
						contentType.CharSet = Utility.RemoveQuotesIfAny(value);
						break;

					case "NAME":
						contentType.Name = EncodedWord.Decode(Utility.RemoveQuotesIfAny(value));
						break;

					default:
						// This is to shut up the code help that is saying that contentType.Parameters
						// can be null - which it cant!
						if (contentType.Parameters == null)
							throw new Exception("The ContentType parameters property is null. This will never be thrown.");

						// We add the unknown value to our parameters list
						// "Known" unknown values are:
						// - title
						// - report-type
						contentType.Parameters.Add(key, value);
						break;
				}
			}

			return contentType;
		}

		/// <summary>
		/// Parses a the value for the header Content-Disposition to a <see cref="ContentDisposition"/> object.
		/// </summary>
		/// <param name="headerValue">The value to be parsed</param>
		/// <returns>A <see cref="ContentDisposition"/> object</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="headerValue"/> is <see langword="null"/></exception>
		public static ContentDisposition ParseContentDisposition(String headerValue)
		{
			if (headerValue == null)
				throw new ArgumentNullException("headerValue");

			// See http://www.ietf.org/rfc/rfc2183.txt for RFC definition

			// Create empty ContentDisposition - we will fill in details as we read them
			ContentDisposition contentDisposition = new ContentDisposition();

			// Now decode the parameters
			List<KeyValuePair<String, String>> parameters = Rfc2231Decoder.Decode(headerValue);

			foreach (KeyValuePair<String, String> keyValuePair in parameters)
			{
				String key = keyValuePair.Key.ToUpperInvariant().Trim();
				String value = keyValuePair.Value;
				switch (key)
				{
					case "":
						// This is the DispisitionType - it has no key since it is the first one
						// and has no = in it.
						contentDisposition.DispositionType = value;
						break;

					// The correct name of the parameter is filename, but some emails also contains the parameter
					// name, which also holds the name of the file. Therefore we use both names for the same field.
					case "NAME":
					case "FILENAME":
						// The filename might be in qoutes, and it might be encoded-word encoded
						contentDisposition.FileName = EncodedWord.Decode(Utility.RemoveQuotesIfAny(value));
						break;

					case "CREATION-DATE":
						// Notice that we need to create a new DateTime because of a failure in .NET 2.0.
						// The failure is: you cannot give contentDisposition a DateTime with a Kind of UTC
						// It will set the CreationDate correctly, but when trying to read it out it will throw an exception.
						// It is the same with ModificationDate and ReadDate.
						// This is fixed in 4.0 - maybe in 3.0 too.
						// Therefore we create a new DateTime which have a DateTimeKind set to unspecified
						DateTime creationDate = new DateTime(Rfc2822DateTime.StringToDate(Utility.RemoveQuotesIfAny(value)).Ticks);
						contentDisposition.CreationDate = creationDate;
						break;

					case "MODIFICATION-DATE":
						DateTime midificationDate = new DateTime(Rfc2822DateTime.StringToDate(Utility.RemoveQuotesIfAny(value)).Ticks);
						contentDisposition.ModificationDate = midificationDate;
						break;

					case "READ-DATE":
						DateTime readDate = new DateTime(Rfc2822DateTime.StringToDate(Utility.RemoveQuotesIfAny(value)).Ticks);
						contentDisposition.ReadDate = readDate;
						break;

					case "SIZE":
						contentDisposition.Size = Convert.ToInt32(Utility.RemoveQuotesIfAny(value));
						break;

					default:
						throw new ArgumentException("Unknown parameter in Content-Disposition. Ask developer to fix! Parameter: " + key);
				}
			}

			return contentDisposition;
		}

		/// <summary>
		/// Parses an ID like Message-Id and Content-Id.<br/>
		/// Example:<br/>
		/// <c>&lt;test@test.com&gt;</c><br/>
		/// into<br/>
		/// <c>test@test.com</c>
		/// </summary>
		/// <param name="headerValue">The id to parse</param>
		/// <returns>A parsed ID</returns>
		public static String ParseId(String headerValue)
		{
			// Remove whitespace in front and behind since
			// whitespace is allowed there
			// Remove the last > and the first <
			return headerValue.Trim().TrimEnd(new [] {'>'}).TrimStart(new [] {'<'});
		}

		/// <summary>
		/// Parses multiple IDs from a single String like In-Reply-To.
		/// </summary>
		/// <param name="headerValue">The value to parse</param>
		/// <returns>A list of IDs</returns>
		public static List<String> ParseMultipleIDs(String headerValue)
		{
			List<String> returner = new List<String>();

			// Split the String by >
			// We cannot use ' ' (space) here since this is a possible value:
			// <test@test.com><test2@test.com>
			var ids = (headerValue.Trim() as String).Split('>');
			foreach (String id in ids)
			{
				if (id != "")
					returner.Add(ParseId(id));
			}

			return returner;
		}
	}
}