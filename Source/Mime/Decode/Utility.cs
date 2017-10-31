// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//

namespace RemObjects.InternetPack.Messages.Mime.Decode
{
	/// <summary>
	/// Contains common operations needed while decoding.
	/// </summary>
	static class Utility
	{
		/// <summary>
		/// Separate header name and header value.
		/// </summary>
		/// <exception cref="ArgumentNullException">If <paramref name="rawHeader"/> is <see langword="null"/></exception>
		public static String[] GetHeadersValue(String rawHeader)
		{
			if (rawHeader == null)
				throw new ArgumentNullException("rawHeader");

			String[] array = new String[] { String.Empty, String.Empty };
			Int32 indexOfColon = rawHeader.IndexOf(':');

			// Check if it is allowed to make substring calls
			if (indexOfColon >= 0 && rawHeader.Length >= indexOfColon + 1)
			{
				array[0] = rawHeader.Substring(0, indexOfColon).Trim();
				array[1] = rawHeader.Substring(indexOfColon + 1).Trim();
			}

			return array;
		}

		/// <summary>
		/// Remove quotes, if found, around the String.
		/// </summary>
		/// <param name="text">Text with quotes or without quotes</param>
		/// <returns>Text without quotes</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="text"/> is <see langword="null"/></exception>
		public static String RemoveQuotesIfAny(String text)
		{
			if (text == null)
				throw new ArgumentNullException("text");

			String returner = text;

			if (returner.StartsWith("\""))
				returner = returner.Substring(1);
			if (returner.EndsWith("\""))
				returner = returner.Substring(0, returner.Length - 1);

			return returner;
		}

		/// <summary>
		/// Split a String into a list of strings using a specified character.<br/>
		/// Everything inside quotes are ignored.
		/// </summary>
		/// <param name="input">A String to split</param>
		/// <param name="toSplitAt">The character to use to split with</param>
		/// <returns>A List of strings that was delimited by the <paramref name="toSplitAt"/> character</returns>
		public static List<String> SplitStringWithCharNotInsideQuotes(String input, char toSplitAt)
		{
			List<String> elements = new List<String>();

			Int32 lastSplitLocation = 0;
			Boolean insideQuote = false;

			char[] characters = input.ToCharArray();

			for (Int32 i = 0; i < characters.Length; i++)
			{
				char character = characters[i];
				if (character == '\"')
					insideQuote = !insideQuote;

				// Only split if we are not inside quotes
				if (character == toSplitAt && !insideQuote)
				{
					// We need to split
					Int32 length = i - lastSplitLocation;
					elements.Add(input.Substring(lastSplitLocation, length));

					// Update last split location
					// + 1 so that we do not include the character used to split with next time
					lastSplitLocation = i + 1;
				}
			}

			// Add the last part
			elements.Add(input.Substring(lastSplitLocation, input.Length - lastSplitLocation));

			return elements;
		}

		/// <summary>
		/// Parse a character set into an encoding.
		/// </summary>
		/// <param name="characterSet">The character set to parse</param>
		/// <returns>An encoding which corresponds to the character set</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="characterSet"/> is <see langword="null"/></exception>
		public static Encoding ParseCharsetToEncoding(String characterSet)
		{
			if (characterSet == null)
				throw new ArgumentNullException("characterSet");

			String charSetUpper = characterSet.ToUpperInvariant();
			#if echoes
			if (charSetUpper.Contains("WINDOWS") || charSetUpper.Contains("CP"))
			{
				// It seems the character set contains an codepage value, which we should use to parse the encoding
				charSetUpper = charSetUpper.Replace("CP", ""); // Remove cp
				charSetUpper = charSetUpper.Replace("WINDOWS", ""); // Remove windows
				charSetUpper = charSetUpper.Replace("-", ""); // Remove - which could be used as cp-1554

				// Now we hope the only thing left in the characterSet is numbers.
				Int32 codepageNumber = System.Convert.ToInt32(charSetUpper, CultureInfo.InvariantCulture);

				return System.Text.Encoding.GetEncoding(codepageNumber);
			}
			#endif

			// Some emails incorrectly specify the encoding to be utf8 - but it has to be utf-8
			if (characterSet.EqualsIgnoringCaseInvariant("utf8"))
				characterSet = "utf-8";

			// It seems there is no codepage value in the characterSet. It must be a named encoding
			return Encoding.GetEncoding(characterSet);
		}
	}

	/// <summary>
	/// Utility to help reading bytes and strings of a <see cref="Stream"/>
	/// </summary>
	static class StreamUtility
	{
		/// <summary>
		/// Read a line from the stream.
		/// A line is interpreted as all the bytes read until a CRLF or LF is encountered.<br/>
		/// CRLF pair or LF is not included in the String.
		/// </summary>
		/// <param name="stream">The stream from which the line is to be read</param>
		/// <returns>A line read from the stream returned as a Byte array or <see langword="null"/> if no bytes were readable from the stream</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/></exception>
		private static Byte[] ReadLineAsBytes(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException("stream");

			using (MemoryStream memoryStream = new MemoryStream())
			{
				while (true)
				{
					Int32 justRead = stream.ReadByte();
					if (justRead == -1 && memoryStream.Length > 0)
						break;

					// Check if we started at the end of the stream we read from
					// and we have not read anything from it yet
					if (justRead == -1 && memoryStream.Length == 0)
						return null;

					char readChar = (char)justRead;

					// Do not write \r or \n
					if (readChar != '\r' && readChar != '\n')
						memoryStream.WriteByte((Byte)justRead);

					// Last point in CRLF pair
					if (readChar == '\n')
						break;
				}

				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Read a line from the stream. <see cref="ReadLineAsBytes"/> for more documentation.
		/// </summary>
		/// <param name="stream">The stream to read from</param>
		/// <returns>A line read from the stream or <see langword="null"/> if nothing could be read from the stream</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="stream"/> is <see langword="null"/></exception>
		public static String ReadLineAsAscii(Stream stream)
		{
			Byte[] readFromStream = ReadLineAsBytes(stream);
			return readFromStream != null ? Encoding.ASCII.GetString(readFromStream) : null;
		}
	}
}