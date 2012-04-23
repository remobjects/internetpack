// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//

using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using RemObjects.InternetPack.Messages.Mime.Decode;

namespace RemObjects.InternetPack.Messages.Mime.Header
{
    ///<summary>
    /// Utility class that divides a message into a body and a header.<br/>
    /// The header is then parsed to a strongly typed <see cref="MessageHeader"/> object.
    ///</summary>
    static class HeaderExtractor
    {
        /// <summary>
        /// Find the end of the header section in a Byte array.<br/>
        /// The headers have ended when a blank line is found
        /// </summary>
        /// <param name="messageContent">The full message stored as a Byte array</param>
        /// <returns>The position of the line just after the header end blank line</returns>
        private static Int32 FindHeaderEndPosition(Byte[] messageContent)
        {
            // Convert the Byte array into a stream
            using (Stream stream = new MemoryStream(messageContent))
            {
                while (true)
                {
                    // Read a line from the stream. We know headers are in US-ASCII
                    // therefore it is not problem to read them as such
                    String line = StreamUtility.ReadLineAsAscii(stream);

                    // The end of headers is signaled when a blank line is found
                    // or if the line is null - in which case the email is actually an email with
                    // only headers but no body
                    if (String.IsNullOrEmpty(line))
                        return (Int32)stream.Position;
                }
            }
        }

        /// <summary>
        /// Extract the header part and body part of a message.<br/>
        /// The headers are then parsed to a strongly typed <see cref="MessageHeader"/> object.
        /// </summary>
        /// <param name="fullRawMessage">The full message in bytes where header and body needs to be extracted from</param>
        /// <param name="headers">The extracted header parts of the message</param>
        /// <param name="body">The body part of the message</param>
        /// <exception cref="ArgumentNullException">If <paramref name="fullRawMessage"/> is <see langword="null"/></exception>
        public static void ExtractHeadersAndBody(Byte[] fullRawMessage, out MessageHeader headers, out Byte[] body)
        {
            if (fullRawMessage == null)
                throw new ArgumentNullException("fullRawMessage");

            // Find the end location of the headers
            Int32 endOfHeaderLocation = FindHeaderEndPosition(fullRawMessage);

            // The headers are always in ASCII - therefore we can convert the header part into a String
            // using US-ASCII encoding
            String headersString = Encoding.ASCII.GetString(fullRawMessage, 0, endOfHeaderLocation);

            // Now parse the headers to a NameValueCollection
            NameValueCollection headersUnparsedCollection = ExtractHeaders(headersString);

            // Use the NameValueCollection to parse it into a strongly-typed MessageHeader header
            headers = new MessageHeader(headersUnparsedCollection);

            // Since we know where the headers end, we also know where the body is
            // Copy the body part into the body parameter
            body = new Byte[fullRawMessage.Length - endOfHeaderLocation];
            Array.Copy(fullRawMessage, endOfHeaderLocation, body, 0, body.Length);
        }

        //     token          = 1*<any CHAR except CTLs or separators>
        private static readonly char[] SEPARATORS = new char[] { '(', ')', '<', '>', '@', ',', ';', '\\', '"', '/', '[', ']', '?', '=', '{', '}', ' ' };

        /// <summary>
        /// Method that takes a full message and extract the headers from it.
        /// </summary>
        /// <param name="messageContent">The message to extract headers from. Does not need the body part. Needs the empty headers end line.</param>
        /// <returns>A collection of Name and Value pairs of headers</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="messageContent"/> is <see langword="null"/></exception>
        private static NameValueCollection ExtractHeaders(String messageContent)
        {
            if (messageContent == null)
                throw new ArgumentNullException("messageContent");

            NameValueCollection headers = new NameValueCollection();

            using (StringReader messageReader = new StringReader(messageContent))
            {
                // Read until all headers have ended.
                // The headers ends when an empty line is encountered
                // An empty message might actually not have an empty line, in which
                // case the headers end with null value.
                String line;
                Boolean lIsFirstLine = true;
                while (!String.IsNullOrEmpty(line = messageReader.ReadLine()))
                {
                    // Split into name and value
                    String[] splittedValue = Utility.GetHeadersValue(line);

                    Int32 lSeparatorPos = line.IndexOfAny(SEPARATORS);
                    if (lIsFirstLine && lSeparatorPos != -1 && (line.IndexOf(':') == -1 || line.IndexOf(':') > line.IndexOf(' ')))
                    {
                        // not header field-name was found, mb Request-Line
                        headers.Add("_Request-Line_", line);
                        lIsFirstLine = false;
                        continue;
                    }

                    // First index is header name
                    String headerName = splittedValue[0];

                    // Second index is the header value.
                    // Use a StringBuilder since the header value may be continued on the next line
                    StringBuilder headerValue = new StringBuilder(splittedValue[1]);

                    // Keep reading until we would hit next header
                    // This if for handling multi line headers
                    while (IsMoreLinesInHeaderValue(messageReader))
                    {
                        // Unfolding is accomplished by simply removing any CRLF
                        // that is immediately followed by WSP
                        // This was done using ReadLine (it discards CRLF)
                        // See http://tools.ietf.org/html/rfc822#section-3.1.1 for more information
                        String moreHeaderValue = messageReader.ReadLine();

                        // If this exception is ever raised, there is an serious algorithm failure
                        // IsMoreLinesInHeaderValue does not return true if the next line does not exist
                        // This check is only included to stop the nagging "possibly null" code analysis hint
                        if (moreHeaderValue == null)
                            throw new ArgumentException("This will never happen");

                        // Simply append the line just read to the header value
                        headerValue.Append(moreHeaderValue);
                    }

                    // Now we have the name and full value. Add it
                    headers.Add(headerName, headerValue.ToString());
                }
            }

            return headers;
        }

        /// <summary>
        /// Check if the next line is part of the current header value we are parsing by
        /// peeking on the next character of the <see cref="TextReader"/>.<br/>
        /// This should only be called while parsing headers.
        /// </summary>
        /// <param name="reader">The reader from which the header is read from</param>
        /// <returns><see langword="true"/> if multi-line header. <see langword="false"/> otherwise</returns>
        private static Boolean IsMoreLinesInHeaderValue(TextReader reader)
        {
            Int32 peek = reader.Peek();
            if (peek == -1)
                return false;

            char peekChar = (char)peek;

            // A multi line header must have a whitespace character
            // on the next line if it is to be continued
            return peekChar == ' ' || peekChar == '\t';
        }
    }
}