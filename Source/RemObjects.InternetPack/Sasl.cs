/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	static class Sasl
	{
		public static String MD5Login(String saslChallenge, String username, String password, String hostname)
		{
			throw new NotImplementedException();
		}

		public class SaslString
		{
			private readonly IDictionary<String, String> fValues;

			public SaslString()
			{
				this.fValues = new Dictionary<String, String>(16, StringComparer.OrdinalIgnoreCase);
			}

			public SaslString(String source)
				: this()
			{
				this.Parse(source);
			}

			public Boolean IsMisformed
			{
				get
				{
					return fIsMisformed;
				}
			}
			private Boolean fIsMisformed;

			public IDictionary<String, String> Values
			{
				get
				{
					return fValues;
				}
			}

			public String this[String name]
			{
				get
				{
					if (fValues.ContainsKey(name))
						return fValues[name];

					return null;
				}
				set
				{
					if (value == null)
						fValues.Remove(name);
					else
						fValues[name] = value;
				}
			}

			private void AddKeyValuePair(String key, String value)
			{
				if (String.IsNullOrEmpty(key))
				{
					this.fIsMisformed = true;
					return;
				}

				this.fValues[key] = value;
			}

			private void DecodeQuotedPair(String source, Int32 startIndex, Int32 endIndex)
			{
				if ((endIndex - startIndex) < 5)
				{
					this.fIsMisformed = true;
					return;
				}

				String lDequotedString = source.Substring(startIndex, (endIndex - startIndex) - 1);
				Int32 lSeparatorIndex = lDequotedString.IndexOfAny(new char[] { ':', '=' });
				if ((lSeparatorIndex < 1) || ((lSeparatorIndex + 1) == lDequotedString.Length))
				{
					this.fIsMisformed = true;
				}
				else
				{
					String lKey = lDequotedString.Substring(0, lSeparatorIndex);
					String lValue = lDequotedString.Substring(lSeparatorIndex + 1, (lDequotedString.Length - lSeparatorIndex) - 1);
					this.AddKeyValuePair(lKey, lValue);
				}
			}

			private String DequoteString(String source, Int32 startIndex, Int32 endIndex)
			{
				if (endIndex > (startIndex + 1))
					return source.Substring(startIndex, (endIndex - startIndex) - 1).Replace("\"\"", "\"");

				return String.Empty;
			}

			public void Clear()
			{
				fValues.Clear();
			}

			public void Remove(String name)
			{
				fValues.Remove(name);
			}

			public void Parse(String loginString)
			{
				if (String.IsNullOrEmpty(loginString))
				{
					fIsMisformed = true;
					return;
				}

				Char[] lLoginString = loginString.ToCharArray();

				ParserState lParserState = ParserState.EntryStart;
				Int32 lParserPosition = 0;
				Int32 lLoginStringLength = lLoginString.Length;
				String lKey = String.Empty;

				Int32 lTokenStart = 0;
				while (true)
				{
					if (lParserPosition >= lLoginStringLength)
					{
						switch (lParserState)
						{
							case ParserState.Key:
							case ParserState.KeyQuoted:
							case ParserState.KeyQuotedStart:
							case ParserState.KeyValueSeparator:
							case ParserState.ValueQuotedStart:
							case ParserState.ValueQuoted:
							case ParserState.Recovering:
								fIsMisformed = true;
								break;

							case ParserState.KeyQuotedEnd:
								this.DecodeQuotedPair(loginString, lTokenStart, lParserPosition);
								break;

							case ParserState.Value:
								AddKeyValuePair(lKey, loginString.Substring(lTokenStart, lParserPosition - lTokenStart));
								break;

							case ParserState.ValueQuotedEnd:
								this.AddKeyValuePair(lKey, this.DequoteString(loginString, lTokenStart, lParserPosition));
								break;
						}
						//lParserState = ParserState.LoginStringEnd;

						return;
					}

					Char lChar = lLoginString[lParserPosition];
					switch (lParserState)
					{
						case ParserState.EntryStart:
							if ((lChar == ':') || (lChar == '='))
							{
								lParserState = ParserState.Recovering;
								fIsMisformed = true;
							}
							else if (lChar == '"')
							{
								lParserState = ParserState.KeyQuotedStart;
							}
							else if (lChar != ',')
							{
								lParserState = ParserState.Key;
								lTokenStart = lParserPosition;
								lKey = String.Empty;
							}
							break;

						case ParserState.Key:
							if ((lChar == ':') || (lChar == '='))
							{
								lParserState = ParserState.KeyValueSeparator;
								lKey = loginString.Substring(lTokenStart, lParserPosition - lTokenStart);
							}
							else if (lChar == ',')
							{
								lParserState = ParserState.EntryStart;
								fIsMisformed = true;
							}
							break;

						case ParserState.KeyQuotedStart:
							lParserState = (lChar != '"') ? ParserState.KeyQuoted : ParserState.KeyQuotedEnd;
							lTokenStart = lParserPosition;
							lKey = String.Empty;
							break;

						case ParserState.KeyQuoted:
							if (lChar == '"')
								lParserState = ParserState.KeyQuotedEnd;
							break;

						case ParserState.KeyQuotedEnd:
							if ((lChar == ':') || (lChar == '='))
							{
								lParserState = ParserState.KeyValueSeparator;
								lKey = DequoteString(loginString, lTokenStart, lParserPosition);
							}
							else if (lChar == '"')
								lParserState = ParserState.KeyQuoted;
							else if (lChar == ',')
							{
								lParserState = ParserState.EntryStart;
								this.DecodeQuotedPair(loginString, lTokenStart, lParserPosition);
							}
							break;

						case ParserState.KeyValueSeparator:
							switch (lChar)
							{
								case ':':
								case '=':
									lParserState = ParserState.Recovering;
									fIsMisformed = true;
									break;
								case ',':
									lParserState = ParserState.EntryStart;
									AddKeyValuePair(lKey, String.Empty);
									break;
								case '"':
									lParserState = ParserState.ValueQuotedStart;
									break;
								default:

									lParserState = ParserState.Value;
									lTokenStart = lParserPosition;
									break;
							}
							break;

						case ParserState.Value:
							if (lChar == ',')
							{
								lParserState = ParserState.EntryStart;
								AddKeyValuePair(lKey, loginString.Substring(lTokenStart, lParserPosition - lTokenStart));
							}
							break;

						case ParserState.ValueQuotedStart:
							lParserState = (lChar != '"') ? ParserState.ValueQuoted : ParserState.ValueQuotedEnd;
							lTokenStart = lParserPosition;
							break;

						case ParserState.ValueQuoted:
							if (lChar == '"')
								lParserState = ParserState.ValueQuotedEnd;
							break;

						case ParserState.ValueQuotedEnd:
							switch (lChar)
							{
								case '"':
									lParserState = ParserState.ValueQuoted;
									break;
								case ':':
								case '=':
									lParserState = ParserState.Recovering;
									fIsMisformed = true;
									break;
								case ',':
									lParserState = ParserState.EntryStart;
									AddKeyValuePair(lKey, this.DequoteString(loginString, lTokenStart, lParserPosition));
									break;
							}
							break;

						case ParserState.Recovering:
							if (lChar == ',')
								lParserState = ParserState.EntryStart;
							break;
					}
					lParserPosition++;
				}
			}

			public override String ToString()
			{
				StringBuilder lResult = new StringBuilder();
				Boolean lFirst = true;

				foreach (KeyValuePair<String, String> entry in fValues)
				{
					if (lFirst)
						lFirst = false;
					else
						lResult.Append(',');

					if (entry.Key == null)
						continue;

					lResult.Append(entry.Key);
					lResult.Append("=");
					if (entry.Value != null)
					{
						if (entry.Value.IndexOfAny(new char[] { ',', '"', '\'' }) != -1)
						{
							lResult.Append('"');
							lResult.Append(entry.Value.Replace("\"", "\"\""));
							lResult.Append('"');
						}
						else
						{
							lResult.Append(entry.Value);
						}
					}
				}

				return lResult.ToString();
			}

			private enum ParserState
			{
				EntryStart,
				Key,
				KeyQuotedStart,
				KeyQuoted,
				KeyQuotedEnd,
				KeyValueSeparator,
				Value,
				ValueQuotedStart,
				ValueQuoted,
				ValueQuotedEnd,
				//LoginStringEnd,
				Recovering
			}
		}
	}
}