/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.InternetPack.Shared.Base;

namespace RemObjects.InternetPack.Ldap
{
	// http://tools.ietf.org/html/rfc3641 << asn1 (encoding of values)
	// http://en.wikipedia.org/wiki/Basic_Encoding_Rules (actual wire format)

	static class Asn1 // static
	{
		// Universal Class Types
		public const Byte EOC = 0;
		public const Byte BOOLEAN = 1;
		public const Byte INTEGER = 2;
		public const Byte BITSTRING = 3;
		public const Byte OCTETSTRING = 4;
		public const Byte NULL = 5;
		public const Byte OBJECTIDENTIFIER = 6;
		public const Byte OBJECTDESCRIPTOR = 7;
		public const Byte EXTERNAL = 8;
		public const Byte REAL = 9;
		public const Byte ENUMERATED = 10;
		public const Byte EMBEDDEDPDV = 11;
		public const Byte UTF8STRING = 12;
		public const Byte RELATIVEOID = 13;
		public const Byte SEQUENCE = 48;
		public const Byte SET = 49;
		public const Byte NUMERICSTRING = 18;
		public const Byte PRINTABLESTRING = 19;
		public const Byte T61STRING = 20;
		public const Byte VIDEOTEXSTRING = 21;
		public const Byte IA5STRING = 22;
		public const Byte UTCTIME = 23;
		public const Byte GENERALIZEDTIME = 24;
		public const Byte GRAPHICSTRING = 25;
		public const Byte VISIBLESTRING = 26;
		public const Byte GENERALSTRING = 27;
		public const Byte UNIVERSALSTRING = 28;
		public const Byte CHARACTERSTRING = 29;
		public const Byte BMPSTRING = 30;
		public const Byte LDAPOID = 0x80;
		public const Byte LDAPVALUE = 0x81;

		public const Byte LDAPSTARTRANGE = LDAPBINDREQ;
		public const Byte LDAPENDRANGE = LDAPEXTRESP;
		public const Byte LDAPBINDREQ = 0x60;
		public const Byte LDAPBINDRESP = 0x61;
		public const Byte LDAPUNBINDREQ = 0x42;

		public const Byte LDAPSEARCHREQ = 0x63;
		public const Byte LDAPSEARCHENTRY = 0x64;
		public const Byte LDAPSEARCHDONE = 0x65;
		public const Byte LDAPSEARCHREF = 0x73;
		public const Byte LDAPMODIFYREQ = 0x66;

		public const Byte LDAPMODIFYRESP = 0x67;
		public const Byte LDAPADDREQ = 0x68;
		public const Byte LDAPADDRESP = 0x69;
		public const Byte LDAPDELREQ = 0x4A;
		public const Byte LDAPDELRESP = 0x6B;
		public const Byte LDAPMODIFYDNREQ = 0x6C;
		public const Byte LDAPMODIFYDNRESP = 0x6D;
		public const Byte LDAPCOMPAREREQ = 0x6E;
		public const Byte LDAPCOMPARERESP = 0x6F;
		public const Byte LDAPABANDONREQ = 0x70;
		public const Byte LDAPEXTREQ = 0x77;
		public const Byte LDAPEXTRESP = 0x78;

		public const Byte LDAPREFERER = 0xA3;
		public const Byte LDAPBINDPASSWORD = 0x80;
		public const Byte LDAPBINDSASL = 0xA3;

		public const Byte LDAPFILTERSUBSTRING_INIT = 0xa0;
		public const Byte LDAPFILTERSUBSTRING_ANY = 0xa1;
		public const Byte LDAPFILTERSUBSTRING_FINAL = 0xa2;

		public const Byte LDAPFILTERAND = 0xa0;
		public const Byte LDAPFILTEROR = 0xa1;
		public const Byte LDAPFILTERNOT = 0xa2;
		public const Byte LDAPFILTEREQUALITYMATCH = 0xa3;
		public const Byte LDAPFILTERSUBSTRING = 0xa4;
		public const Byte LDAPFILTERGREATEROREQUAL = 0xa5;
		public const Byte LDAPFILTERLESSOREQUAL = 0xa6;
		public const Byte LDAPFILTERPRESENT = 0x87;
		public const Byte LDAPFILTERAPPROXMATCH = 0xa8;
		public const Byte LDAPFILTEREXTENSIBLEMATCH = 0xa9;
		// http://www.shrubbery.net/mibs/RFC1155-SMI.txt

		private const Byte APPLICATION = 0x40;
		public const Byte IPADDRESS = APPLICATION + 0; // 4 bytes
		public const Byte COUNTER = APPLICATION + 1; // 4 bytes; but should be read as regular integer
		public const Byte GAUGE = APPLICATION + 2;  //  4 bytes; but should be read as regular integer
		public const Byte TIMETICKS = APPLICATION + 3;  //  4 bytes; but should be read as regular integer
		public const Byte OPAQUE = APPLICATION + 4; // octet String


		public static void Decode(BinaryReader reader, out Byte classType, out Int32 length)
		{
			classType = reader.ReadByte();
			Int32 lLength = reader.ReadByte();
			if (0 == (lLength & 0x80))
			{
				length = lLength;
			}
			else
			{
				lLength &= ~0x80;
				length = 0;
				for (Int32 i = 0; i < lLength; i++)
					length = length << 8 | reader.ReadByte();
			}
		}

		public static void Encode(BinaryWriter writer, Byte classType, Int32 length)
		{
			writer.Write(classType);
			// now write the length
			if (length < 0x80)
			{
				writer.Write((Byte)length);
			}
			else
			{
				Byte t1 = (Byte)length;
				Byte t2 = (Byte)(length >> 8);
				Byte t3 = (Byte)(length >> 16);
				Byte t4 = (Byte)(length >> 24);
				if (t4 != 0)
				{
					writer.Write((Byte)0x84);
					writer.Write(t4);
					writer.Write(t3);
					writer.Write(t2);
					writer.Write(t1);
				}
				else if (t3 != 0)
				{
					writer.Write((Byte)0x83);
					writer.Write(t3);
					writer.Write(t2);
					writer.Write(t1);
				}
				else if (t2 != 0)
				{
					writer.Write((Byte)0x82);
					writer.Write(t2);
					writer.Write(t1);
				}
				else
				{
					writer.Write((Byte)0x81);
					writer.Write(t1);
				}
				// Long form. Two to 127 octets.
				// Bit 8 of first octet has value "1" and bits 7-1 give
				// the number of additional length octets. Second and
				// following octets give the length, base 256, most
				// significant digit first.
			}
		}

		public static Int32 DecodeInt(BinaryReader reader, Int32 maxLength)
		{
			switch (maxLength)
			{
				case 1:
					return reader.ReadSByte();

				case 2:
					return (Int16)((UInt16)reader.ReadByte() << 8 | reader.ReadByte());

				case 3:
					{
						UInt32 lResult = ((UInt32)reader.ReadByte() << 16 | (UInt32)reader.ReadByte() << 8 | reader.ReadByte());
						if (0 != (lResult & (1 << 23)))
							lResult = lResult | 0xff000000;

						return (Int32)lResult;
					}

				case 4:
					return (Int32)((UInt32)reader.ReadByte() << 24 | (UInt32)reader.ReadByte() << 16 | (UInt32)reader.ReadByte() << 8 | reader.ReadByte());

				default:
					{
						UInt32 lResult = reader.ReadByte();
						if (0 != (lResult & 0x80))
							lResult = lResult | 0xffffff00;
						for (Int32 i = 1; i < maxLength; i++)
							lResult = lResult << 8 | reader.ReadByte();

						return (Int32)lResult;
					}
			}
		}

		public static UInt32 DecodeUInt(BinaryReader reader, Int32 maxLength)
		{
			switch (maxLength)
			{
				case 1:
					return reader.ReadByte();

				case 2:
					return (UInt16)((UInt16)reader.ReadByte() << 8 | (UInt16)reader.ReadByte());

				case 3:
					return ((UInt32)reader.ReadByte() << 16 | (UInt32)reader.ReadByte() << 8 | reader.ReadByte());

				case 4:
					return ((UInt32)reader.ReadByte() << 24 | (UInt32)reader.ReadByte() << 16 | (UInt32)reader.ReadByte() << 8 | reader.ReadByte());

				default:
					UInt32 lResult = 0;
					for (Int32 i = 0; i < maxLength; i++)
						lResult = lResult << 8 | reader.ReadByte();

					return lResult;
			}
		}

		// http://www.faqs.org/rfcs/rfc1960.html
		internal enum RFC1960Token
		{
			OpeningParenthesis,
			ClosingParenthesis,
			And,
			Or,
			Not,
			Equal,
			AproxEqual,
			GreaterOrEqual,
			LessOrEqual,
			Present,
			Any,
			Value,
			Error,
			EOF // won't be returned
		}

		private static void RFC1960Next(String value, ref Int32 pos, ref Int32 len, out RFC1960Token tok, out String result)
		{
			pos += len;
			len = 0;
			result = null;

			switch (value[pos])
			{
				case '\0':
					tok = RFC1960Token.EOF;
					break;
				case '(':
					tok = RFC1960Token.OpeningParenthesis;
					len = 1;
					break;
				case ')':
					tok = RFC1960Token.ClosingParenthesis;
					len = 1;
					break;
				case '&':
					tok = RFC1960Token.And;
					len = 1;
					break;
				case '|':
					tok = RFC1960Token.Or;
					len = 1;
					break;
				case '!':
					tok = RFC1960Token.Not;
					len = 1;
					break;
				case '=':
					switch (value[pos + 1])
					{
						case '~':
							len = 2;
							tok = RFC1960Token.AproxEqual;
							break;
						case '*':
							len = 2;
							tok = RFC1960Token.Present;
							break;
						default:
							len = 1;
							tok = RFC1960Token.Equal;
							break;
					}
					break;
				case '>':
					if (value[pos + 1] == '=')
					{
						len = 2;
						tok = RFC1960Token.GreaterOrEqual;
						break;
					}
					len = 0;
					tok = RFC1960Token.Error;
					break;
				case '<':
					if (value[pos + 1] == '=')
					{
						len = 2;
						tok = RFC1960Token.LessOrEqual;
						break;
					}
					len = 0;
					tok = RFC1960Token.Error;
					break;

				case '*':
					len = 1;
					tok = RFC1960Token.Any;
					break;
				default:
					{
						tok = RFC1960Token.Value;
						StringBuilder sb = new StringBuilder(8);
						Boolean stop = false;
						while (!stop)
						{
							switch (value[pos + len])
							{
								case '<':
								case '~':
								case '>':
								case '=':
								case '\0':
								case '*':
								case '(':
								case ')':
								case '/':
									stop = true;
									break;
								case '\\':
									sb.Append((char)(Byte.Parse(value[pos + len + 1].ToString()) * 16 +
									Byte.Parse(value[pos + len + 2].ToString())));
									len += 2;
									break;
								default:
									sb.Append(value[pos + len]);
									break;


							}
							len++;
						}
						len--;
						result = sb.ToString();
						break;
					}

			}
		}

		internal static BerValue ParseFilter(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			if (tok != RFC1960Token.OpeningParenthesis)
				throw new LdapException("Opening parenthesis expected at " + pos);
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			BerValue result;
			switch (tok)
			{
				case RFC1960Token.And:
					result = ParseAnd(filter, ref pos, ref len, ref tok, ref res);
					break;
				case RFC1960Token.Or:
					result = ParseOr(filter, ref pos, ref len, ref tok, ref res);
					break;
				case RFC1960Token.Not:
					result = ParseNot(filter, ref pos, ref len, ref tok, ref res);
					break;
				default:
					result = ParseSimple(filter, ref pos, ref len, ref tok, ref res);
					break;
			}

			if (tok != RFC1960Token.ClosingParenthesis)
				throw new LdapException("Opening parenthesis expected at " + pos);
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			return result;
		}

		private static String ParseValue(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			if (tok != RFC1960Token.Value)
				throw new LdapException("Value expected at " + pos);
			String s = res;
			do
			{
				RFC1960Next(filter, ref pos, ref len, out tok, out res);
				switch (tok)
				{
					case RFC1960Token.Value:
						s += res;
						break;
					case RFC1960Token.And:
					case RFC1960Token.AproxEqual:
					case RFC1960Token.Equal:
					case RFC1960Token.GreaterOrEqual:
					case RFC1960Token.LessOrEqual:
					case RFC1960Token.Not:
					case RFC1960Token.Or:
					case RFC1960Token.Present:
						s += filter.Substring(pos, len);
						break;
					default:
						return s;
				}
			} while (true);
		}

		private static BerValue ParseAnd(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			return new BerSequence(Asn1.LDAPFILTERAND, ParseFilterList(filter, ref pos, ref len, ref tok, ref res));
		}

		private static BerValue ParseOr(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			return new BerSequence(Asn1.LDAPFILTEROR, ParseFilterList(filter, ref pos, ref len, ref tok, ref res));
		}

		private static BerValue ParseNot(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			return new BerSequence(Asn1.LDAPFILTERNOT, ParseFilter(filter, ref pos, ref len, ref tok, ref res));
		}

		private static BerValue[] ParseFilterList(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			List<BerValue> val = new List<BerValue>();
			val.Add(ParseFilter(filter, ref pos, ref len, ref tok, ref res));
			while (tok == RFC1960Token.OpeningParenthesis)
				val.Add(ParseFilter(filter, ref pos, ref len, ref tok, ref res));
			return val.ToArray();
		}

		private static BerValue ParseSimple(String filter, ref Int32 pos, ref Int32 len, ref RFC1960Token tok, ref String res)
		{
			if (tok != RFC1960Token.Value)
				throw new LdapException("Attribute name expected at " + pos);
			String attributename = res;
			BerValue result;
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			switch (tok)
			{
				// <simple> ::= <attr> <filtertype> <value>
				// <filtertype> ::= <equal> | <approx> | <ge> | <le>
				// <equal> ::= '='
				// <approx> ::= '~='
				// <ge> ::= '>='
				// <le> ::= '<='
				case RFC1960Token.Equal:
					{
						RFC1960Next(filter, ref pos, ref len, out tok, out res);

						String currval = ParseValue(filter, ref pos, ref len, ref tok, ref res);

						if (tok != RFC1960Token.Any)
						{
							result = new BerSequence(Asn1.LDAPFILTEREQUALITYMATCH, new BerString(OCTETSTRING, attributename), new BerString(OCTETSTRING, currval));
							break;
						}
						BerSequence items = new BerSequence();
						items.Items.Add(new BerString(LDAPFILTERSUBSTRING_INIT, currval));
						currval = null;
						while (tok == RFC1960Token.Any)
						{
							if (currval != null)
							{
								items.Items.Add(new BerString(LDAPFILTERSUBSTRING_ANY, currval));
							}
							RFC1960Next(filter, ref pos, ref len, out tok, out res);
							currval = ParseValue(filter, ref pos, ref len, ref tok, ref res);
						}
						items.Items.Add(new BerString(LDAPFILTERSUBSTRING_FINAL, currval));
						result = new BerSequence(Asn1.LDAPFILTERSUBSTRING, new BerString(OCTETSTRING, attributename), items);
						break;
					}
				// supports * too
				case RFC1960Token.AproxEqual:
					RFC1960Next(filter, ref pos, ref len, out tok, out res);
					result = new BerSequence(Asn1.LDAPFILTERAPPROXMATCH, new BerString(OCTETSTRING, attributename), new BerString(OCTETSTRING, ParseValue(filter, ref pos, ref len, ref tok, ref res)));
					break;
				case RFC1960Token.GreaterOrEqual:
					RFC1960Next(filter, ref pos, ref len, out tok, out res);
					result = new BerSequence(Asn1.LDAPFILTERGREATEROREQUAL, new BerString(OCTETSTRING, attributename), new BerString(OCTETSTRING, ParseValue(filter, ref pos, ref len, ref tok, ref res)));
					break;
				case RFC1960Token.LessOrEqual:
					RFC1960Next(filter, ref pos, ref len, out tok, out res);
					result = new BerSequence(Asn1.LDAPFILTERLESSOREQUAL, new BerString(OCTETSTRING, attributename), new BerString(OCTETSTRING, ParseValue(filter, ref pos, ref len, ref tok, ref res)));
					break;
				case RFC1960Token.Present:
					RFC1960Next(filter, ref pos, ref len, out tok, out res);
					result = new BerString(Asn1.LDAPFILTERPRESENT, attributename);
					break;
				default:
					throw new LdapException("expression expected at " + pos);
			}
			return result;
		}

		internal static BerValue ParseFilter(String filter)
		{
			filter += "\0\0\0\0";
			Int32 len = 0;
			Int32 pos = 0;
			String res;
			RFC1960Token tok;
			RFC1960Next(filter, ref pos, ref len, out tok, out res);
			BerValue val = ParseFilter(filter, ref pos, ref len, ref tok, ref res);

			if (tok != RFC1960Token.EOF)
				throw new LdapException("EOF expected at " + pos);
			return val;
		}
	}

	enum BerType
	{
		Integer,
		UInteger,
		Enumerated,
		Boolean,
		BitString,
		String,
		IpAddress,
		Sequence,
		Other
	}

	abstract class BerValue
	{
		protected BerValue()
		{
		}

		public abstract BerType Type { get; }

		public abstract Byte TypeTag { get; }

		public abstract Int32 Length { get; }

		protected abstract void IntWrite(BinaryWriter writer);

		protected abstract void IntRead(BinaryReader reader, Byte code, Int32 length);

		public void Write(BinaryWriter writer)
		{
			Asn1.Encode(writer, TypeTag, Length);
			this.IntWrite(writer);
		}

		public Int32 TotalLength
		{
			get
			{
				Int32 lLength = this.Length;
				if (lLength < 0x80)
					lLength++;
				else if ((lLength & 0xff000000) != 0)
					lLength += 5;
				else if ((lLength & 0x00ff0000) != 0)
					lLength += 4;
				else if ((lLength & 0x0000ff00) != 0)
					lLength += 3;
				else
					lLength += 2;
				lLength++; // char code

				return lLength;
			}
		}

		public static BerValue Read(BinaryReader reader)
		{
			Int32 lLength;
			Byte lClassType;
			Asn1.Decode(reader, out lClassType, out lLength);

			return Read(reader, lLength, lClassType);
		}

		public static BerValue Read(Byte[] val, Int32 len, Byte code)
		{
			return Read(new BinaryReader(new MemoryStream(val, false)), len, code);
		}

		public static BerValue Read(BinaryReader reader, Int32 length, Byte code)
		{
			BerValue lResult;

			switch (code)
			{
				case Asn1.BITSTRING:
					lResult = new BerBinary();
					break;

				case Asn1.ENUMERATED:
					lResult = new BerEnumerated();
					break;

				case Asn1.BOOLEAN:
					lResult = new BerBoolean();
					break;

				case Asn1.INTEGER:
					lResult = new BerInteger();
					break;

				case Asn1.COUNTER:
				case Asn1.GAUGE:
				case Asn1.TIMETICKS:
					lResult = new BerUInteger();
					break;

				case Asn1.LDAPREFERER:
				case Asn1.SET:
				case Asn1.SEQUENCE:
					lResult = new BerSequence();
					break;

				case Asn1.IPADDRESS:
					lResult = new BerIpAddress();
					break;

				case Asn1.OPAQUE:
				case Asn1.OCTETSTRING:
					lResult = new BerString();
					break;

				default:
					if (code >= Asn1.LDAPSTARTRANGE && code <= Asn1.LDAPENDRANGE)
						lResult = new BerSequence();
					else
						lResult = new BerOther();
					break;
			}

			lResult.IntRead(reader, code, length);

			return lResult;
		}
	}

	class BerInteger : BerValue
	{
		public BerInteger()
		{
		}

		public BerInteger(Int32 val)
		{
			this.Value = val;
		}

		public override BerType Type
		{
			get
			{
				return BerType.Integer;
			}
		}

		public override Int32 Length
		{
			get
			{
				if (this.Value < 0)
				{
					if (this.Value >= -128)
						return 1;

					if (this.Value >= -32768)
						return 2;

					if (this.Value >= -8388608)
						return 3;

					return 4;
				}
				else
				{
					if (this.Value < 128)
						return 1;

					if (this.Value < 32768)
						return 2;

					if (this.Value < 8388608)
						return 3;

					return 4;
				}
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return Asn1.INTEGER;
			}
		}

		public Int32 Value { get; set; }

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.Value = Asn1.DecodeInt(reader, length);
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			if (this.Value < 0)
			{
				if (this.Value >= -128)
				{
					writer.Write((Byte)this.Value);
				}
				else if (this.Value >= -32768)
				{
					UInt16 lValue = (UInt16)this.Value;
					writer.Write((Byte)(lValue >> 8));
					writer.Write((Byte)(lValue));
				}
				else if (Value >= -8388608)
				{
					UInt32 lValue = (UInt32)this.Value;
					writer.Write((Byte)(lValue >> 16));
					writer.Write((Byte)(lValue >> 8));
					writer.Write((Byte)(lValue));
				}
				else
				{
					UInt32 lValue = (UInt32)this.Value;
					writer.Write((Byte)(lValue >> 24));
					writer.Write((Byte)(lValue >> 16));
					writer.Write((Byte)(lValue >> 8));
					writer.Write((Byte)(lValue));
				}
			}
			else
			{
				if (this.Value < 128)
				{
					writer.Write((Byte)this.Value);
				}
				else if (Value < 32768)
				{
					UInt16 newval = (UInt16)Value;
					writer.Write((Byte)(newval >> 8));
					writer.Write((Byte)(newval));
				}
				else if (this.Value < 8388608)
				{
					UInt32 lValue = (UInt32)this.Value;
					writer.Write((Byte)(lValue >> 16));
					writer.Write((Byte)(lValue >> 8));
					writer.Write((Byte)(lValue));
				}
				else
				{
					UInt32 lValue = (UInt32)this.Value;
					writer.Write((Byte)(lValue >> 24));
					writer.Write((Byte)(lValue >> 16));
					writer.Write((Byte)(lValue >> 8));
					writer.Write((Byte)(lValue));
				}
			}
			// Primitive. Contents octets give the value of the integer, base 256, in two's
			// complement form, most significant digit first, with the minimum number of
			// octets. The value 0 is encoded as a single 00 octet.

			// Some example BER encodings (which also happen to be DER encodings) are given in Table 3.
			// Integer
			// value    BER encoding
			// 0        02 01 00
			// 127        02 01 7F
			// 128        02 02 00 80
			// 256        02 02 01 00
			// -128    02 01 80
			// -129    02 02 FF 7F
		}

		public override String ToString()
		{
			return Value.ToString();
		}
	}

	class BerUInteger : BerValue
	{
		public BerUInteger()
		{
			this.TypeCodeTag = Asn1.COUNTER;
		}

		public override BerType Type
		{
			get
			{
				return BerType.UInteger;
			}
		}

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.TypeCodeTag = code;
			this.Value = Asn1.DecodeUInt(reader, length);
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			if (this.Value < 256)
			{
				writer.Write((Byte)this.Value);
			}
			else if (this.Value < 65536)
			{
				writer.Write((Byte)(this.Value >> 8));
				writer.Write((Byte)this.Value);
			}
			else if (this.Value < 16777216)
			{
				writer.Write((Byte)(this.Value >> 16));
				writer.Write((Byte)(this.Value >> 8));
				writer.Write((Byte)this.Value);
			}
			else
			{
				writer.Write((Byte)(this.Value >> 24));
				writer.Write((Byte)(this.Value >> 16));
				writer.Write((Byte)(this.Value >> 8));
				writer.Write((Byte)this.Value);
			}
		}

		public override Int32 Length
		{
			get
			{
				if (this.Value < 256)
					return 1;

				if (this.Value < 65536)
					return 2;

				if (this.Value < 16777216)
					return 3;

				return 4;
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return this.TypeCodeTag;
			}
		}

		public Byte TypeCodeTag { get; set; }

		public UInt32 Value { get; set; }

		public override String ToString()
		{
			return this.Value.ToString();
		}
	}


	class BerBinary : BerValue
	{
		public BerBinary()
		{
		}

		public override BerType Type
		{
			get
			{
				return BerType.BitString;
			}
		}

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.Value = reader.Read(length);
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			writer.Write(this.Value);
		}

		public override Int32 Length
		{
			get
			{
				return this.Value.Length;
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return Asn1.BITSTRING;
			}
		}

		public Byte[] Value { get; set; }

		public override String ToString()
		{
			return (this.Value == null) ? "" : Convert.ToString(this.Value);
		}
	}

	class BerBoolean : BerInteger
	{
		public BerBoolean()
		{
		}

		public BerBoolean(Boolean value)
		{
			this.Value = value;
		}

		public new Boolean Value
		{
			get
			{
				return base.Value != 0;
			}
			set
			{
				base.Value = value ? 0xff : 0;
			}
		}

		public override BerType Type
		{
			get
			{
				return BerType.Boolean;
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return Asn1.BOOLEAN;
			}
		}

		public override String ToString()
		{
			return this.Value.ToString();
		}
	}

	class BerEnumerated : BerInteger
	{
		public BerEnumerated()
		{
		}

		public BerEnumerated(Int32 value)
		{
			this.Value = value;
		}

		public override BerType Type
		{
			get
			{
				return BerType.Enumerated;
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return Asn1.ENUMERATED;
			}
		}
	}

	class BerSequence : BerValue
	{
		public BerSequence()
		{
			this.fItems = new List<BerValue>();
			this.TypeCodeTag = Asn1.SEQUENCE;
		}

		public BerSequence(Byte code, params BerValue[] args)
			: this()
		{
			this.TypeCodeTag = code;
			if (args != null)
				this.fItems.Add(args);
		}

		public IList<BerValue> Items
		{
			get
			{
				return this.fItems;
			}
		}
		private readonly List<BerValue> fItems;

		public override BerType Type
		{
			get
			{
				return BerType.Sequence;
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return this.TypeCodeTag;
			}
		}

		public Byte TypeCodeTag { get; set; }

		public override Int32 Length
		{
			get
			{
				Int32 lLength = 0;
				for (Int32 i = 0; i < this.fItems.Count; i++)
				{
					lLength += this.fItems[i].TotalLength;
				}

				return lLength;
			}
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			for (Int32 i = 0; i < this.fItems.Count; i++)
			{
				this.fItems[i].Write(writer);
			}
		}

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.TypeCodeTag = code;
			Byte[] lData = reader.Read(length);
			this.fItems.RemoveAll();

			BinaryReader lReader = new BinaryReader(new MemoryStream(lData, false));
			while (lReader.PeekChar() >= 0)
				this.fItems.Add(BerValue.Read(lReader));
		}

		public override String ToString()
		{
			StringBuilder lResult = new StringBuilder();
			lResult.Append("{");

			for (Int32 i = 0; i < this.Items.Count; i++)
			{
				if (i != 0)
					lResult.Append(", ");

				lResult.Append(this.Items[i].ToString());
			}
			lResult.Append("}");

			return lResult.ToString();
		}
	}

	class BerString : BerValue
	{
		public BerString()
		{
			this.TypeCodeTag = Asn1.OCTETSTRING;
		}

		public BerString(Byte typeTag, String value)
			: this()
		{
			this.Value = value;
			this.TypeCodeTag = typeTag;
		}

		public override BerType Type
		{
			get
			{
				return BerType.String;
			}
		}

		public override Int32 Length
		{
			get
			{
				return this.Value == null ? 0 : System.Text.Encoding.UTF8.GetByteCount(this.Value);
			}
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			if (this.Value != null)
				writer.Write(Encoding.UTF8.GetBytes(this.Value));
		}

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.TypeCodeTag = code;
			Byte[] lValue = reader.Read(length);
			this.Value = Encoding.UTF8.GetString(lValue, 0, lValue.Length);
		}

		public override Byte TypeTag
		{
			get
			{
				return this.TypeCodeTag;
			}
		}

		public Byte TypeCodeTag { get; set; }

		public String Value { get; set; }

		public override String ToString()
		{
			if (this.Value == null)
				return "";

			return "\"" + this.Value.Replace("\"", "\"\"") + "\"";
		}
	}

	class BerIpAddress : BerValue
	{
		public BerIpAddress()
		{
		}

		public override BerType Type
		{
			get
			{
				return BerType.IpAddress;
			}
		}

		public override Byte TypeTag
		{
			get
			{
				return Asn1.IPADDRESS;
			}
		}

		public override Int32 Length
		{
			get
			{
				return (this.fValue.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6) ? 16 : 4;
			}
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			Byte[] lValue = this.fValue.GetAddressBytes();

			if (this.fValue.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
				writer.Write(lValue, 0, 16);
			else
				writer.Write(lValue, 0, 4);
		}

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.fValue = new IPAddress(reader.Read(length));
		}

		public IPAddress Value
		{
			get
			{
				return this.fValue;
			}
			set
			{
				this.fValue = value;
			}
		}
		private IPAddress fValue;

		public override String ToString()
		{
			if (this.Value == null)
				return "";

			return this.Value.ToString();
		}
	}

	class BerOther : BerValue
	{
		public BerOther()
		{
		}

		public override BerType Type
		{
			get
			{
				return BerType.Other;
			}
		}

		public override Int32 Length
		{
			get
			{
				return Value.Length;
			}
		}

		protected override void IntWrite(BinaryWriter writer)
		{
			writer.Write(Value);
		}

		protected override void IntRead(BinaryReader reader, Byte code, Int32 length)
		{
			this.TypeCodeTag = code;
			this.Value = reader.Read(length);
		}

		public override Byte TypeTag
		{
			get
			{
				return this.TypeCodeTag;
			}
		}

		public Byte[] Value { get; set; }

		public Byte TypeCodeTag { get; set; }

		public override String ToString()
		{
			return (this.Value == null) ? "" : Convert.ToString(this.Value);
		}
	}
}