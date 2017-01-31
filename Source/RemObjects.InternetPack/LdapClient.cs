/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Ldap
{
	// http://tools.ietf.org/html/rfc4511 << ldap
	// http://tools.ietf.org/html/rfc3641 << asn1 (encoding of values)

	public class LdapException : Exception
	{
		public LdapException(String message)
			: base(message)
		{
		}

		public LdapException(String message, Int32 code)
			: base(message)
		{
			this.fCode = code;
		}

		public LdapException(Int32 code)
			: base(ErrorToString(code))
		{
			this.fCode = code;
		}

		public Int32 Code
		{
			get
			{
				return fCode;
			}
		}
		private readonly Int32 fCode;

		public static String ErrorToString(Int32 code)
		{
			switch (code)
			{
				case 0: return "Success";
				case 1: return "Operations Error";
				case 2: return "Protocol Error";
				case 3: return "Timelimit Exceeded";
				case 4: return "Sizelimit Exceeded";
				case 5: return "Compare False";
				case 6: return "Compare True";
				case 7: return "Authentication Method Not Supported";
				case 8: return "Strong Authentication Required";
				case 9: return "Partial Results";
				case 10: return "Referral";
				case 11: return "Administrative Limit Exceeded";
				case 12: return "Unavailable Critical Extension";
				case 13: return "Confidentiality Required";
				case 14: return "SASL Bind In Progress";
				case 16: return "No Such Attribute";
				case 17: return "Undefined Attribute Type";
				case 18: return "Inappropriate Matching";
				case 19: return "Constraint Violation";
				case 20: return "Attribute Or Value Exists";
				case 21: return "Invalid Attribute Syntax";
				case 32: return "No Such Object";
				case 33: return "Alias Problem";
				case 34: return "Invalid DN Syntax";
				case 35: return "Is Leaf";
				case 36: return "Alias Dereferencing Problem";
				case 48: return "Inappropriate Authentication";
				case 49: return "Invalid Credentials";
				case 50: return "Insufficient Access Rights";
				case 51: return "Busy";
				case 52: return "Unavailable";
				case 53: return "Unwilling To Perform";
				case 54: return "Loop Detect";
				case 64: return "Naming Violation";
				case 65: return "Object Class Violation";
				case 66: return "Not Allowed On Non-leaf";
				case 67: return "Not Allowed On RDN";
				case 68: return "Entry Already Exists";
				case 69: return "Object Class Modifications Prohibited";
				case 71: return "Affects Multiple DSAs";
				case 80: return "Other";
				case 81: return "Server Down";
				case 82: return "Local Error";
				case 83: return "Encoding Error";
				case 84: return "Decoding Error";
				case 85: return "Ldap Timeout";
				case 86: return "Authentication Unknown";
				case 87: return "Filter Error";
				case 88: return "User Cancelled";
				case 89: return "Parameter Error";
				case 90: return "No Memory";
				case 91: return "Connect Error";
				case 92: return "Ldap Not Supported";
				case 93: return "Control Not Found";
				case 94: return "No Results Returned";
				case 95: return "More Results To Return";
				case 96: return "Client Loop";
				case 97: return "Referral Limit Exceeded";
				case 112: return "TLS not supported";
				case 113: return "SSL handshake failed";
				case 114: return "SSL Provider not found";
				default: return "Unknown Error";
			}
		}
	}

#if DESIGN
	[System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.FtpClient.bmp")]
#endif
	public class LdapClient : Client
	{
		public const Int32 LdapSPort = 636;
		public const Int32 LdapPort = 389;

		private Int32 fSequenceNumber; // sending requests to ldap uses a sequence id
		private MemoryStream fMemoryStream;
		private BinaryWriter fWriter;

		public LdapClient()
		{
			this.Port = LdapPort;
			this.BindDigest = DigestType.None;
			this.LdapVersion = 3;
			this.fCurrentConnection = null;
		}

		public enum DigestType
		{
			None,
			MD5
		}

		public Boolean LoggedIn
		{
			get
			{
				return fLoggedIn;
			}
		}
		private Boolean fLoggedIn;
#if FULLFRAMEWORK
		public Boolean UseStartTLS { get; set; }

#endif

		public DigestType BindDigest { get; set; }

		public Int32 LdapVersion { get; set; }

		public String BindDN { get; set; }

		public String BindPassword { get; set; }

		public Connection CurrentConnection
		{
			get
			{
				return fCurrentConnection;
			}
		}
		private Connection fCurrentConnection;

		public Boolean Connected
		{
			get
			{
				return (fCurrentConnection != null) && (fCurrentConnection.Connected);
			}
		}

		public static String Escape(String s)
		{
			return s.Replace("\\", "\\5c")
				.Replace("*", "\\2a")
				.Replace("(", "\\28")
				.Replace(")", "\\29")
				.Replace("\0", "\\00")
				.Replace("/", "\\2f");
		}


		public virtual void Open()
		{

#if FULLFRAMEWORK
			if ((SslOptions.Enabled || UseStartTLS) && SslOptions.TargetHostName == null && HostName != null)
				SslOptions.TargetHostName = HostName;
#endif
			if (fCurrentConnection != null)
			{

				if (fCurrentConnection.Connected) { fCurrentConnection.Close(); }
				fCurrentConnection = null;
			}
			fCurrentConnection = Connect();
#if FULLFRAMEWORK
			if (!SslOptions.Enabled && UseStartTLS)
			{
				if (SendStartTLS())
				{
					fCurrentConnection = SslOptions.CreateClientConnection(fCurrentConnection);
					((SslConnection)fCurrentConnection).InitializeClientConnection();
					fCurrentConnection.EnableNagle = EnableNagle;
				}
				else
					throw new LdapException("Could not initialize TLS");
			}
#endif
			fLoggedIn = false;
		}

		public virtual void Close()
		{
			if (fLoggedIn)
				Unbind();
			if (fCurrentConnection != null)
			{
				fCurrentConnection.Close();
				fCurrentConnection = null;
			}
		}

#if FULLFRAMEWORK
		private const String StartTLSObjID = "1.3.6.1.4.1.1466.20037";
		// http://www.networksorcery.com/enp/rfc/rfc2830.txt
		private Boolean SendStartTLS()
		{
			Int32 seqid = SendLdapRequest(Asn1.LDAPEXTREQ, new BerString(Asn1.LDAPOID, StartTLSObjID));
			Response res = ReadResponse();
			return (res.Code == 0 && res.SequenceId == seqid);
		}
#endif

		private Int32 SendLdapRequest(Byte command, params BerValue[] args)
		{
			if (fMemoryStream == null)
			{
				fMemoryStream = new MemoryStream();
				fWriter = new BinaryWriter(fMemoryStream);
			}

			fSequenceNumber++;
			if (fSequenceNumber < 0)
				fSequenceNumber = 1;

			BerSequence lCommandParameters = new BerSequence();
			lCommandParameters.TypeCodeTag = command;
			for (Int32 i = 0; i < args.Length; i++)
				lCommandParameters.Items.Add(args[i]);

			// Now we have a wrapped ldap command; we need to add an integer before it, in a regular sequence
			BerSequence lSequence = new BerSequence();
			// first write the sequence number
			lSequence.Items.Add(new BerInteger(fSequenceNumber));
			// then the comamnd data
			lSequence.Items.Add(lCommandParameters);

			fMemoryStream.SetLength(0);
			lSequence.Write(fWriter);
			fWriter.Flush();

			Byte[] lData = fMemoryStream.ToArray();
			fCurrentConnection.Write(lData, 0, lData.Length);

			return fSequenceNumber;
		}

		private BerValue ReadBerValue()
		{
			Int32 lCode = fCurrentConnection.ReadByte();
			if (lCode < 0)
				throw new ConnectionClosedException();

			Int32 lLength = fCurrentConnection.ReadByte();
			if (lLength < 0)
				throw new ConnectionClosedException();

			if (lLength >= 0x80)
			{
				Byte[] lBuffer = new Byte[lLength & ~0x80];
				fCurrentConnection.Read(lBuffer, 0, lBuffer.Length);
				lLength = 0;
				for (Int32 i = 0; i < lBuffer.Length; i++)
					lLength = lLength << 8 | lBuffer[i];
			}

			Byte[] lData = new Byte[lLength];
			fCurrentConnection.Read(lData, 0, lData.Length);

			return BerValue.Read(lData, lLength, (Byte)lCode);
		}

		private class Response
		{
			public Int32 SequenceId { get; set; }
			public Int32 TypeCode { get; set; } //type code tag
			public Int32 Code { get; set; }
			public String DN { get; set; }
			public String Result { get; set; }
			public BerSequence Referers { get; set; } // referers
			public BerValue[] RestData { get; set; } // rest data; only valid when it's not referers
		}

		private Response ReadResponse()
		{
			BerValue lValue = ReadBerValue();

			if (lValue != null && lValue.Type == BerType.Sequence && ((BerSequence)lValue).Items.Count >= 2 && ((BerSequence)lValue).Items[0].Type == BerType.Integer)
			{
				Response lResponse = new Response();
				lResponse.SequenceId = ((BerInteger)((BerSequence)lValue).Items[0]).Value;

				BerSequence lSubValue = ((BerSequence)lValue).Items[1] as BerSequence;
				if (lSubValue == null)
					return null;

				lResponse.TypeCode = lSubValue.TypeCodeTag;
				if (lSubValue.Items.Count == 0)
					return null;

				if (lResponse.TypeCode == Asn1.LDAPSEARCHENTRY)
				{
					lResponse.RestData = ((List<BerValue>)lSubValue.Items).ToArray();
				}
				else
				{
					if (!(lSubValue.Items[0] is BerInteger))
						return null; // Int32 or sequence

					lResponse.Code = ((BerInteger)lSubValue.Items[0]).Value;

					if (lSubValue.Items.Count > 1 && lSubValue.Items[1].Type == BerType.String)
						lResponse.DN = ((BerString)lSubValue.Items[1]).Value;

					if (lSubValue.Items.Count > 2 && lSubValue.Items[2].Type == BerType.String)
						lResponse.Result = ((BerString)lSubValue.Items[2]).Value;

					if (lSubValue.Items.Count > 3 && lResponse.Code == 10) // 10 = referers
					{
						lResponse.Referers = lSubValue.Items[3] as BerSequence;
					}
					else if (lSubValue.Items.Count > 3)
					{
						lResponse.RestData = new BerValue[lSubValue.Items.Count - 3];
						for (Int32 i = 0; i < lResponse.RestData.Length; i++)
							lResponse.RestData[i] = lSubValue.Items[i + 3];
					}
				}

				return lResponse;
			}

			return null; // crap on the line
		}

		public void Bind()
		{
			this.Bind(BindDN, BindPassword, BindDigest);
		}

		public void Bind(String dn, String password, DigestType digestType)
		{
			if (fCurrentConnection == null)
				Open();

			if (String.IsNullOrEmpty(dn) || String.IsNullOrEmpty(password))
				digestType = DigestType.None;

			switch (digestType)
			{
				case DigestType.MD5:
					{
						Int32 lSequenceId = SendLdapRequest(Asn1.LDAPBINDREQ,
								new BerInteger(LdapVersion),
								new BerString(Asn1.OCTETSTRING, ""),
								new BerSequence(Asn1.LDAPBINDSASL, new BerString(Asn1.OCTETSTRING, "DIGEST-MD5")));

						Response lResponse = ReadResponse();
						if (lResponse.SequenceId != lSequenceId) throw new LdapException("Invalid sequence id in bind response");
						if (lResponse.Code != 14 || lResponse.RestData == null || lResponse.RestData[0].Type != BerType.String) // 14 = Sasl bind in progress
							throw new LdapException(lResponse.Code);


						String lResult = ((BerString)lResponse.RestData[0]).Value;
						String lEncodedResult = Sasl.MD5Login(lResult, dn, password, GetTargetHostName());

						lSequenceId = SendLdapRequest(Asn1.LDAPBINDREQ,
								new BerInteger(LdapVersion),
								new BerString(Asn1.OCTETSTRING, ""),
								new BerSequence(Asn1.LDAPBINDSASL, new BerString(Asn1.OCTETSTRING, lEncodedResult)));

						lResponse = ReadResponse();
						if (lResponse.SequenceId != lSequenceId)
							throw new LdapException("Invalid sequence id in bind response");

						if (lResponse.Code != 14 || lResponse.RestData == null || lResponse.RestData[0].Type != BerType.String) // 14 = Sasl bind in progress
							throw new LdapException(lResponse.Code);

						lSequenceId = SendLdapRequest(Asn1.LDAPBINDREQ,
								new BerInteger(LdapVersion),
								new BerString(Asn1.OCTETSTRING, ""),
								new BerSequence(Asn1.LDAPBINDSASL, new BerString(Asn1.OCTETSTRING, "DIGEST-MD5")));

						lResponse = ReadResponse();
						if (lResponse.SequenceId != lSequenceId)
							throw new LdapException("Invalid sequence id in bind response");

						if (lResponse.Code != 0)
							throw new LdapException(lResponse.Code);
						break;
					}
				default:
					{
						Int32 lSequenceId = SendLdapRequest(Asn1.LDAPBINDREQ,
								new BerInteger(LdapVersion),
								new BerString(Asn1.OCTETSTRING, dn),
								new BerString(Asn1.LDAPBINDPASSWORD, password));
						Response lResponse = ReadResponse();

						if (lResponse.SequenceId != lSequenceId)
							throw new LdapException("Invalid sequence id in bind response");

						if (lResponse.Code != 0)
							throw new LdapException(lResponse.Code);

						break;
					}
			}

			fLoggedIn = true;
		}

		private String GetTargetHostName()
		{
			IPAddress lAddress;
			if (String.IsNullOrEmpty(HostName))
			{
				lAddress = HostAddress;
			}
			else
#if COMPACTFRAMEWORK
				try
				{
					lAddress = IPAddress.Parse(HostName);
				}
				catch
				{
					return HostName;
				}
#else
				if (!IPAddress.TryParse(HostName, out lAddress))
				return HostName;
#endif
			try
			{
				return Dns.DnsLookup.ReverseResolve(lAddress);
			}
			catch (Exception)
			{
				return lAddress.ToString();
			}
		}

		public void Unbind()
		{
			if (fCurrentConnection == null)
				throw new LdapException("Not connected");

			SendLdapRequest(Asn1.LDAPUNBINDREQ);
			fLoggedIn = false;
			Close();
		}

		public enum SearchScope
		{
			BaseObject = 0,
			SingleLevel = 1,
			FullSubtree = 2
		}

		public enum AliasDereferencing
		{
			Never = 0,
			InSearching = 1,
			FindingBase = 2,
			Always = 3
		}

		public const String AllAttributes = "*";

		public const String NoAttributes = "1.1";


		public LdapSearchResults Search(String baseObject, SearchScope scope, AliasDereferencing aliases, Int32 size, Int32 time, Boolean typesOnly,
			String filter, String[] attributes)
		{
			if (attributes == null || attributes.Length == 0)
				attributes = new String[] { AllAttributes };

			BerValue[] attributevalues = new BerValue[attributes.Length];
			for (Int32 i = 0; i < attributes.Length; i++)
				attributevalues[i] = new BerString(Asn1.OCTETSTRING, attributes[i]);

			Int32 lSequenceId = SendLdapRequest(Asn1.LDAPSEARCHREQ,
				new BerString(Asn1.OCTETSTRING, baseObject),
				new BerEnumerated((Int32)scope),
				new BerEnumerated((Int32)aliases),
				new BerInteger(size),
				new BerInteger(time),
				new BerBoolean(typesOnly),
				Asn1.ParseFilter(String.IsNullOrEmpty(filter) ? "(objectclass=*)" : filter),
				new BerSequence(Asn1.SEQUENCE, attributevalues));

			LdapSearchResults lResults = new LdapSearchResults();
			while (true)
			{
				Response lResponse = ReadResponse();

				if (lResponse.SequenceId != lSequenceId)
					throw new LdapException("Invalid sequence id in bind response");

				if (lResponse.Code != 0)
					throw new LdapException(lResponse.Code);

				if (lResponse.TypeCode == Asn1.LDAPSEARCHENTRY)
				{
					if (lResponse.RestData != null && lResponse.RestData.Length > 0 && lResponse.RestData[0].Type == BerType.String)
					{
						LdapObject obj = new LdapObject(((BerString)lResponse.RestData[0]).Value);
						lResults.Add(obj);
						if (lResponse.RestData.Length > 1 && lResponse.RestData[1].Type == BerType.Sequence)
						{
							foreach (BerValue attribute in ((BerSequence)lResponse.RestData[1]).Items)
							{
								if (attribute.Type == BerType.Sequence && ((BerSequence)attribute).Items.Count > 0 && ((BerSequence)attribute).Items[0].Type == BerType.String)
								{
									LdapAttribute att = new LdapAttribute(((BerString)((BerSequence)attribute).Items[0]).Value);
									obj.Attributes.Add(att);
									if (((BerSequence)attribute).Items.Count <= 1 || ((BerSequence)attribute).Items[1].Type != BerType.Sequence)
									{
										continue;
									}

									foreach (BerValue value in ((BerSequence)((BerSequence)attribute).Items[1]).Items)
									{
										switch (value.Type)
										{
											case BerType.BitString:
												att.Binary = true;
												att.Add(((BerBinary)value).Value);
												break;
											case BerType.Boolean:
												att.Binary = false;
												att.Add(((BerBoolean)value).Value.ToString());
												break;
											case BerType.Enumerated:
											case BerType.Integer:
												att.Binary = false;
												att.Add(((BerInteger)value).Value.ToString());
												break;
											case BerType.IpAddress:
												att.Binary = false;
												att.Add(((BerIpAddress)value).Value.ToString());
												break;
											case BerType.Other:
												att.Binary = true;
												att.Add(((BerOther)value).Value);
												break;
											case BerType.String:
												att.Binary = false;
												att.Add(((BerString)value).Value);
												break;
											case BerType.UInteger:
												att.Binary = false;
												att.Add(((BerUInteger)value).Value.ToString());
												break;
										}
									}
								}
							}
						}
					}
				}
				else if (lResponse.TypeCode == Asn1.LDAPSEARCHREF)
				{
					if (lResponse.Referers != null)
					{
						foreach (BerValue val in lResponse.Referers.Items)
						{
							if (val is BerString)
								lResults.Referals.Add(((BerString)val).Value);
						}
					}
					else if (lResponse.RestData != null)
					{
						foreach (BerValue val in lResponse.RestData)
						{
							if (val is BerString)
								lResults.Referals.Add(((BerString)val).Value);
							if (val is BerSequence)
							{
								foreach (BerValue val2 in ((BerSequence)val).Items)
								{
									if (val2 is BerString)
										lResults.Referals.Add(((BerString)val2).Value);
								}
							}
						}
					}
				}
				else if (lResponse.TypeCode == Asn1.LDAPSEARCHDONE)
					break;
				else
					throw new LdapException("Unknown response from server");
			}

			return lResults;
		}
	}

	public class LdapAttribute
	{
		public LdapAttribute()
		{
			this.fValues = new List<Object>();
		}

		public LdapAttribute(String key)
			: this()
		{
			this.fKey = key;
		}

		private List<Object> fValues;

		public String Key
		{
			get
			{
				return fKey;
			}
			set
			{
				fKey = value;
			}
		}
		private String fKey;

		public Boolean Binary
		{
			get
			{
				return fBinary;
			}
			set
			{
				if (fBinary != value)
				{
					fValues.Clear();
					fBinary = value;
				}
			}
		}
		private Boolean fBinary;

		public Int32 Count
		{
			get
			{
				return fValues.Count;
			}
		}

		public Byte[] SingleBinaryValue
		{
			get
			{
				if (!fBinary)
					throw new ArgumentException("Values are not binary");

				if (fValues.Count == 0)
					return null;

				return (Byte[])fValues[0];
			}
		}

		public String SingleStringValue
		{
			get
			{
				if (fBinary)
					throw new ArgumentException("Values are not String values");

				if (fValues.Count == 0)
					return null;

				return (String)fValues[0];
			}
		}

		public void RemoveAt(Int32 index)
		{
			fValues.RemoveAt(index);
		}

		public void Remove(String item)
		{
			fValues.Remove(item);
		}

		public void Remove(Byte[] item)
		{
			fValues.Remove(item);
		}

		public void Add(String item)
		{
			if (fBinary)
				throw new ArgumentException("Values are not String values");

			fValues.Add(item);
		}

		public void Add(Byte[] item)
		{
			if (!fBinary)
				throw new ArgumentException("Values are not binary");

			fValues.Add(item);
		}

		public String GetString(Int32 index)
		{
			if (fBinary)
				throw new ArgumentException("Values are not String values");

			return (String)fValues[index];
		}

		public Byte[] GetBinary(Int32 index)
		{
			if (!fBinary)
				throw new ArgumentException("Values are not binary");

			return (Byte[])fValues[index];
		}

		public void SetString(Int32 index, String value)
		{
			if (fBinary)
				throw new ArgumentException("Values are not String values");

			fValues[index] = value;
		}

		public void SetBinary(Int32 index, Byte[] value)
		{
			if (!fBinary)
				throw new ArgumentException("Values are not binary");

			fValues[index] = value;
		}
	}

	public class LdapAttributes : List<LdapAttribute>
	{
		public new LdapAttribute this[Int32 index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		public LdapAttribute this[String key]
		{
			get
			{
				for (Int32 i = 0; i < Count; i++)
					if (String.Equals(key, base[i].Key, StringComparison.InvariantCultureIgnoreCase))
						return base[i];

				return null;
			}
		}

		public String GetStringAttribute(String key)
		{
			LdapAttribute lAttribute = this[key];
			if (lAttribute == null)
				return null;

			if (lAttribute.Binary)
				return Convert.ToString(lAttribute.SingleBinaryValue);

			return lAttribute.SingleStringValue;
		}
	}

	public class LdapObject
	{
		public LdapObject()
		{
			this.fAttributes = new LdapAttributes();
		}

		public LdapObject(String dn)
			: this()
		{
			this.DN = dn;
		}

		public String DN
		{
			get
			{
				return fDN;
			}
			set
			{
				fDN = value;
			}
		}
		private String fDN;

		public LdapAttributes Attributes
		{
			get
			{
				return fAttributes;
			}
		}
		private LdapAttributes fAttributes;
	}

	public class LdapSearchResults : List<LdapObject>
	{
		public LdapSearchResults()
		{
			this.fReferals = new List<String>();
		}

		private List<String> fReferals;
		public IList<String> Referals
		{
			get
			{
				return fReferals;
			}
		}
	}

	public class LdapUserLookup : Component
	{
		public String UserSearchBase
		{
			get
			{
				return fUserSearchBase;
			}
			set
			{
				fUserSearchBase = value;
			}
		}
		private String fUserSearchBase;

		public String UserFilter
		{
			get
			{
				return fUserFilter;
			}
			set
			{
				fUserFilter = value;
			}
		}
		private String fUserFilter = "(objectClass=inetOrgPerson)";

		public Boolean SearchSubTree
		{
			get
			{
				return fSearchSubTree;
			}
			set
			{
				fSearchSubTree = value;
			}
		}
		private Boolean fSearchSubTree = true;

		public String UserNameField
		{
			get
			{
				return fUserNameField;
			}
			set
			{
				fUserNameField = value;
			}
		}
		private String fUserNameField = "uid";

		public String GroupSearchBase
		{
			get
			{
				return fGroupSearchBase;
			}
			set
			{
				fGroupSearchBase = value;
			}
		}
		private String fGroupSearchBase;

		public String GroupFilter
		{
			get
			{
				return fGroupFilter;
			}
			set
			{
				fGroupFilter = value;
			}
		}
		private String fGroupFilter = "(objectClass=groupOfNames)";

		public String GroupNameField
		{
			get
			{
				return fGroupNameField;
			}
			set
			{
				fGroupNameField = value;
			}
		}
		private String fGroupNameField = "cn";

		public String GroupMemberField
		{
			get
			{
				return fGroupMemberField;
			}
			set
			{
				fGroupMemberField = value;
			}
		}
		private String fGroupMemberField = "member";

		public Boolean SearchGroups
		{
			get
			{
				return fSearchGroups;
			}
			set
			{
				fSearchGroups = value;
			}
		}
		private Boolean fSearchGroups = true;

		public String LookupDN
		{
			get
			{
				return fLookupDN;
			}
			set
			{
				fLookupDN = value;
			}
		}
		private String fLookupDN;

		public String LookupPassword
		{
			get
			{
				return fLookupPassword;
			}
			set
			{
				fLookupPassword = value;
			}
		}
		private String fLookupPassword;

		public String Hostname
		{
			get
			{
				return fHostname;
			}
			set
			{
				fHostname = value;
			}
		}
		private String fHostname;

		public Int32 Port
		{
			get
			{
				return fPort;
			}
			set
			{
				fPort = value;
			}
		}
		private Int32 fPort = LdapClient.LdapPort;

		public Boolean UseStartTLS
		{
			get
			{
				return fUseStartTLS;
			}
			set
			{
				fUseStartTLS = value;
			}
		}
		private Boolean fUseStartTLS;

		/// <summary>
		/// strip the dn of the group base; for example:
		/// GroupBase: ou=groups,cn=company,cn=com
		/// Item:      cn=mygroup,ou=groups,cn=company,cn=com     returns: "mygroup"
		/// Item:      cn=mygroup,ou=list,ou=groups,cn=company,cn=com     returns: "list.mygroup"
		/// </summary>
		public Boolean StripGroupBaseDN
		{
			get
			{
				return fStripGroupBaseDN;
			}
			set
			{
				fStripGroupBaseDN = value;
			}
		}
		private Boolean fStripGroupBaseDN;

#if FULLFRAMEWORK
		public SslConnectionFactory SslOptions
		{
			get
			{
				return fSslOptions;
			}
			set
			{
				fSslOptions = value;
			}
		}
		private SslConnectionFactory fSslOptions = new SslConnectionFactory();
#endif
		public class LookupResults
		{
			public LookupResults()
			{
				this.fGroupMembership = new List<String>();
			}

			public String Username
			{
				get
				{
					return fUsername;
				}
				set
				{
					fUsername = value;
				}
			}
			private String fUsername;

			public String DN
			{
				get
				{
					return fDN;
				}
				set
				{
					fDN = value;
				}
			}
			private String fDN;

			public LdapObject UserObject
			{
				get
				{
					return fUserObject;
				}
				set
				{
					fUserObject = value;
				}
			}
			private LdapObject fUserObject;

			public List<String> GroupMembership
			{
				get
				{
					return fGroupMembership;
				}
			}
			private List<String> fGroupMembership;
		}
		/// <summary>
		/// Thread safe way to execute ldap requests
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public LookupResults Login(String username, String password)
		{
			if (String.IsNullOrEmpty(fUserSearchBase))
				throw new LdapException("UserSearchBase is not set");

			if (fSearchGroups && String.IsNullOrEmpty(fGroupSearchBase))
				throw new LdapException("GroupSearchBase is not set");

			if (String.IsNullOrEmpty(fLookupDN))
				throw new LdapException("LookupDN is not set");

			if (String.IsNullOrEmpty(fLookupPassword))
				throw new LdapException("LookupPassword is not set");

			if (String.IsNullOrEmpty(fHostname))
				throw new LdapException("HostName is not set");

			if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
				return null;

			using (LdapClient lClient = new LdapClient())
			{
#if FULLFRAMEWORK
				lClient.SslOptions = fSslOptions;
				lClient.UseStartTLS = fUseStartTLS;
#endif
				lClient.HostName = fHostname;
				lClient.Port = fPort;

				if (BeforeConnect != null)
					BeforeConnect(this, new LdapEventArgs(lClient));

				lClient.Open();

				if (AfterConnect != null)
					AfterConnect(this, new LdapEventArgs(lClient));

				lClient.BindDN = fLookupDN;
				lClient.BindPassword = fLookupPassword;
				lClient.Bind();

				LookupResults lResult = new LookupResults();

				foreach (LdapObject obj in lClient.Search(
						fUserSearchBase,
						fSearchSubTree ? LdapClient.SearchScope.FullSubtree : LdapClient.SearchScope.SingleLevel,
						LdapClient.AliasDereferencing.Always,
						0, 0, false, fUserFilter,
						new String[] { fUserNameField }))
				{
					if (obj.Attributes.GetStringAttribute(fUserNameField) == username)
					{
						lResult.DN = obj.DN;
						lResult.Username = obj.Attributes.GetStringAttribute(fUserNameField);
						break;
					}
				}

				if (lResult.DN == null)
					return null;

				foreach (LdapObject obj in lClient.Search(lResult.DN, LdapClient.SearchScope.BaseObject, LdapClient.AliasDereferencing.Always, 0, 0, false, null, null))
				{
					lResult.UserObject = obj;
					break;
				}

				if (fSearchGroups)
				{
					String s = "(&" + fGroupFilter + "(" + fGroupMemberField + "=" + lResult.DN + "))";
					//(&(objectClass=groupOfNames)(member=uid=ck,ou=users,dc=remobjects,dc=com))
					//String s = "(objectClass=groupOfNames)";
					//String s = "(member=uid=ck,ou=users,dc=remobjects,dc=com)";
					foreach (LdapObject obj in
						lClient.Search(fGroupSearchBase, fSearchSubTree ? LdapClient.SearchScope.FullSubtree : LdapClient.SearchScope.SingleLevel,
						LdapClient.AliasDereferencing.Always,
						0, 0, false, s, new String[] { fGroupNameField, fGroupMemberField }))
					{
						if (fStripGroupBaseDN)
							lResult.GroupMembership.Add(StripGroupBase(obj.DN));
						else
							lResult.GroupMembership.Add(obj.DN);
					}
				}

				lClient.BindDN = lResult.DN;
				lClient.BindPassword = password;
				try
				{
					lClient.Bind();
				}
				catch (LdapException)
				{
					lResult = null;
				}

				lClient.Unbind();

				if (Disconnected != null)
					Disconnected(this, new LdapEventArgs(lClient));

				return lResult;
			}
		}

		private String StripGroupBase(String dn)
		{
			String[] lItems = dn.Trim().Split(',');
			String[] lGroupDN = fGroupSearchBase.Trim().Split(',');

			for (Int32 i = 0; i < lItems.Length; i++)
				lItems[i] = lItems[i].Trim();

			for (Int32 i = 0; i < lGroupDN.Length; i++)
				lGroupDN[i] = lGroupDN[i].Trim();

			if (lGroupDN.Length >= lItems.Length)
				return dn; // makes no sense

			for (Int32 i = lGroupDN.Length - 1; i >= 0; i--)
				if (0 != String.Compare(lItems[lItems.Length - lGroupDN.Length + i], lGroupDN[i], StringComparison.InvariantCultureIgnoreCase))
					return dn; // shouldn't happen

			String lResult = null;
			for (Int32 i = lItems.Length - lGroupDN.Length - 1; i >= 0; i--)
			{
				String lValue = lItems[i];

				if (lValue.IndexOf("=", StringComparison.Ordinal) != -1)
					lValue = lValue.Substring(lValue.IndexOf('=') + 1);

				lResult = (lResult == null) ? lValue : lResult + "." + lValue;
			}

			return lResult;
		}

		public event EventHandler<LdapEventArgs> BeforeConnect;

		public event EventHandler<LdapEventArgs> AfterConnect;

		public event EventHandler<LdapEventArgs> Disconnected;
	}

	public class LdapEventArgs : EventArgs
	{
		public LdapEventArgs(LdapClient client)
		{
			this.fClient = client;
		}

		public LdapClient Client
		{
			get
			{
				return this.fClient;
			}
		}
		private readonly LdapClient fClient;
	}
}