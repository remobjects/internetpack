/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.InternetPack.Shared.Base;
using RemObjects.InternetPack.Messages.Mime.Decode;

namespace RemObjects.InternetPack.Messages
{
	public class MailMessage
	{
		private readonly HeaderFields fFields = new HeaderFields();
		//private MessageAttachments fAttachments = new MessageAttachments();

		#region Properties
		public IMessageEncoder Encoder { get; set; }

		public MessageAddresses To
		{
			get
			{
				return fTo;
			}
		}
		private readonly MessageAddresses fTo = new MessageAddresses();

		public MessageAddresses Cc
		{
			get
			{
				return fCc;
			}
		}
		private readonly MessageAddresses fCc = new MessageAddresses();

		public MessageAddresses Bcc
		{
			get
			{
				return fBcc;
			}
		}
		private readonly MessageAddresses fBcc = new MessageAddresses();

		public MessageAddress From
		{
			get
			{
				return fFrom;
			}
			set
			{
				fFrom = value;
			}
		}
		private MessageAddress fFrom = new MessageAddress();

		public MessageAddress Sender
		{
			get
			{
				return fSender;
			}
		}
		private readonly MessageAddress fSender = new MessageAddress();

		public String MessageId
		{
			get
			{
				return fFields["Message-Id"].Value;
			}
			set
			{
				fFields["Message-Id"].Value = value;
			}
		}

		public String UserAgent
		{
			get
			{
				return fFields["User-Agent"].Value;
			}
			set
			{
				fFields["User-Agent"].Value = value;
			}
		}

		public String Subject
		{
			get
			{
				return fFields["Subject"].Value;
			}
			set
			{
				fFields["Subject"].Value = value;
			}
		}

		// Since date might be in a different timezone, we just give the original value too
		public String Date
		{
			get
			{
				return fFields["Date"].Value;
			}
			set
			{
				fFields["Date"].Value = value;
			}
		}

		// Return the date in the timezone of the current computer
		public DateTime DateHome
		{
			get
			{
				return GetDateHome();
			}
			set
			{
				SetDateHome(value);
			}
		}

		// Newsgroup, for newsgroup articles
		public String Newsgroups
		{
			get
			{
				return fFields["Newsgroups"].Value;
			}
			set
			{
				fFields["Newsgroups"].Value = value;
			}
		}

		public HeaderFields Fields
		{
			get
			{
				return fFields;
			}
		}

		public String Contents { get; set; }
		#endregion

		private void SetDateHome(DateTime value)
		{
			Date = value.ToString("r");
		}

		private DateTime GetDateHome()
		{
			String lDate = Date.Trim();
			Int32 lIndex = lDate.IndexOf('(');

			if (lIndex >= 0)
				lDate = lDate.Substring(0, lIndex).Trim();

			Rfc2822DateTime.StringToDate(lDate);
			// TODO review
			/*try
			{
				return System.DateTime.Parse(lDate, new System.Globalization.DateTimeFormatInfo(), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
			}
			catch
			{
				return System.DateTime.ParseExact(lDate, "ddd, d MMM yyyy HH':'mm':'ss zzz", new System.Globalization.DateTimeFormatInfo(), System.Globalization.DateTimeStyles.AllowWhiteSpaces);
			}*/
		}

		public void ValidateEncoder()
		{
			if (this.Encoder == null)
				this.Encoder = new DefaultEncoder();
		}

		public void EncodeMessage(Stream destination)
		{
			this.ValidateEncoder();
			this.Encoder.EncodeMessage(this, destination);
		}

		public void DecodeMessage(Stream source)
		{
			this.ValidateEncoder();
			this.Encoder.DecodeMessage(this, source);
		}

		public void SaveToFile(String filename)
		{
			using (FileStream stream = new FileStream(filename, FileOpenMode.Create))
				this.EncodeMessage(stream);
		}
	}

	public class DefaultEncoder : IMessageEncoder
	{
		private const String CRLF = "\r\n";

		private static String SplitMultiLine(String source, Int32 length)
		{
			StringBuilder lRes = new StringBuilder();
			Int32 i;

			while (source.Length > length)
			{
				i = source.LastIndexOf(' ', length);
				if (i < (length / 3))
				{
					i = source.IndexOf(' ', length);
					if (i == -1) break;
				}

				lRes.Append(source.Substring(0, i));
				source = " " + source.Substring(i + 1);
				if (source.Trim().Length == 0) break;

				lRes.Append("\r\n");
			}

			lRes.Append(source);

			return lRes.ToString();
		}

		public void EncodeMessage(MailMessage source, Stream destination)
		{
			StreamWriter lWrter = new StreamWriter(destination);

			if (DefaultEncoder.NeedEncoding(source))
				lWrter.WriteLine("MIME-Version: 1.0");

			for (Int32 i = 0; i < source.Fields.Count; i++)
			{
				String lKey = source.Fields.GetKey(i);
				HeaderField lValue = source.Fields[lKey];

				if (lKey.Equals("Subject"))
				{
					lValue.Value = DefaultEncoder.EncodeUtf8Base64(lValue.Value);
					lWrter.WriteLine(lKey + ": " + lValue);
					continue;
				}

				lWrter.WriteLine(SplitMultiLine(lKey + ": " + lValue, 80));
			}

			String lFromEncoded = DefaultEncoder.EncodeUtf8Base64(source.From.Name);
			String lFrom = String.Format("From: {0} <{1}>", lFromEncoded, source.From.Address);

			lWrter.WriteLine(lFrom);

			if (source.Sender.IsSet())
				lWrter.WriteLine(SplitMultiLine("Sender: " + source.Sender, 80));

			if (source.To.Count > 0)
				lWrter.WriteLine(SplitMultiLine("To: " + source.To, 80));

			if (source.Cc.Count > 0)
				lWrter.WriteLine(SplitMultiLine("Cc: " + source.Cc, 80));

			if (-1 != DefaultEncoder.ContainsUnicodeSymbols(source.Contents))
			{
				lWrter.WriteLine("Content-Type: text/plain; charset=UTF-8; format=flowed");
				lWrter.WriteLine("Content-Transfer-Encoding: 8bit");
			}

			lWrter.WriteLine();
			lWrter.Write(source.Contents);

			if (!source.Contents.EndsWith("\r\n")) // always add an enter
				lWrter.WriteLine();

			lWrter.Flush();
		}

		private String RemoveQuotes(String value)
		{
			if (value.StartsWith("\"") && value.EndsWith("\""))
				return value.Substring(1, value.Length - 2);

			return value;
		}

		private void ParseHeader(MailMessage message, String header)
		{
			StringReader lStringReader = new StringReader(header);
			HeaderField lField = null;

			String lLine;
			while (!String.IsNullOrEmpty((lLine = lStringReader.ReadLine())))
			{
				if ((lLine[0] == ' ' || lLine[0] == '\t') && (lField != null))
				{ // header continuation or property
					lField.Value += lLine;
				}
				else
				{ // header
					Int32 lPosition = lLine.IndexOf(':');
					String lName = lLine.Substring(0, lPosition);
					String lValue;

					if ((lPosition + 2) < lLine.Length)
						lValue = lLine.Substring(lPosition + 2);
					else
						lValue = String.Empty;

					lField = new HeaderField(RemoveQuotes(lValue));
					message.Fields.Add(lName, lField);
				}
			}

			lField = message.Fields["To"];
			if (lField != null)
			{
				message.Fields.Remove("To");
				SetAddresses(message.To, lField.Value);
			}

			lField = message.Fields["From"];
			if (lField != null)
			{
				message.Fields.Remove("From");
				message.From.FromString(lField.Value);
			}

			lField = message.Fields["Sender"];
			if (lField != null)
			{
				message.Fields.Remove("Sender");
				message.Sender.FromString(lField.Value);
			}

			lField = message.Fields["CC"];
			if (lField != null)
			{
				message.Fields.Remove("CC");
				SetAddresses(message.Cc, lField.Value);
			}

			lField = message.Fields["bcc"];
			if (lField != null)
			{
				message.Fields.Remove("bcc");
				SetAddresses(message.Bcc, lField.Value);
			}
		}

		private void SetAddresses(MessageAddresses address, String names)
		{
			var lAddresses = names?.Split(";");
			foreach (string a in lAddresses)
				address.Add(a.Trim());
		}

		public void DecodeMessage(MailMessage destination, Stream source)
		{
			TextReader lReader = new StreamReader(source);
			String lMessage = lReader.ReadToEnd();
			String lBody;

			//Debug.Write(lMessage);
			if (lMessage.StartsWith(CRLF))
			{
				lBody = lMessage.Substring(2);
			}
			else
			{
				Int32 lHeaderEnd = lMessage.IndexOf(CRLF + CRLF);
				if (lHeaderEnd < 0)
				{
					throw (new Exception("Invalid email message"));
				}

				ParseHeader(destination, lMessage.Substring(0, lHeaderEnd));

				lBody = lMessage.Substring(lHeaderEnd + 4);
			}

			destination.Contents = lBody;
		}

		public static String EncodeUtf8Base64(String value)
		{
			String lStartTag = "=?UTF-8?B?";
			String lEndTag = "?=";

			Int32 i = ContainsUnicodeSymbols(value);

			if (i != -1)
			{
				StringBuilder lResult = new StringBuilder();
				if (i > 0)
					lResult.Append(value.Substring(0, i));

				lResult.Append(lStartTag);
				// TODO review
				var lBytes = Encoding.UTF8.GetBytes(value.Substring(i));
				lResult.Append(Convert.ToBase64String(lBytes, 0, length(lBytes)));
				lResult.Append(lEndTag);

				return lResult.ToString();
			}

			return value;

		}

		public static Boolean NeedEncoding(MailMessage message)
		{
			if (null == message)
				throw new ArgumentNullException();

			Boolean lIsUnicode = (-1 == ContainsUnicodeSymbols(message.Subject));
			if (!lIsUnicode)
				return true;

			lIsUnicode = (-1 == ContainsUnicodeSymbols(message.Contents));
			if (!lIsUnicode)
				return true;

			return true;
		}

		public static Int32 ContainsUnicodeSymbols(String value)
		{
			for (Int32 i = 0; i < value.Length; i++)
			{
				if (value[i] > '\u007F')
				{
					return i;
				}
			}

			return -1;
		}
	}
}