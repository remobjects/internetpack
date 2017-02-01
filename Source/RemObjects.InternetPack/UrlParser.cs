/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Http
{
	public static class KnownProtocols
	{
		static KnownProtocols()
		{
			fProtocols = new Dictionary<String, Int32>(8);
			fProtocols.Add("ftp", 21);
			fProtocols.Add("ssh", 22);
			fProtocols.Add("telnet", 23);
			fProtocols.Add("smtp", 25);
			fProtocols.Add("http", 80);
			fProtocols.Add("pop3", 110);
			fProtocols.Add("https", 443);
		}

		private static readonly IDictionary<String, Int32> fProtocols;

		public static void RegisterProtocol(String name, Int32 port)
		{
			if (fProtocols.ContainsKey(name))
				throw new Exception(String.Format("Protocol '{0}' already registered.", name));
			fProtocols.Add(name, port);
		}

		public static Int32 GetProtocolDefaultPort(String protocol)
		{
			return fProtocols.ContainsKey(protocol) ? (Int32)fProtocols[protocol] : -1;
		}
	}

#if FULLFRAMEWORK
	[TypeConverter(typeof(UrlConverter))]
#endif
	[Serializable]
	[Obsolete("Use RemObjects.Elements.RTL.Url instead")]
	public class UrlParser : MarshalByRefObject
	{
		private Int32 fDefaultPort;

		public UrlParser()
		{
			this.fProtocol = DEFAULT_PROTOCOL;
			this.fDefaultPort = KnownProtocols.GetProtocolDefaultPort(fProtocol);
			this.Port = this.fDefaultPort;
		}

		public UrlParser(String url)
		{
			this.Parse(url);
		}

		#region Properties
		public String Url
		{
			get
			{
				return String.Format("{0}://{1}{2}", this.Protocol, this.HostnameAndPort, this.PathAndParams);
			}
			set
			{
				this.Parse(value);
			}
		}

		[Browsable(false)]
		public String PathAndParams
		{
			get
			{
				return !String.IsNullOrEmpty(this.Params) ? this.Path + "?" + this.Params : this.Path;
			}
		}

		[Browsable(false)]
		public String HostnameAndPort
		{
			get
			{
				return this.Port != this.fDefaultPort ? this.Hostname + ":" + this.Port.ToString() : this.Hostname;
			}
		}

		public String Protocol
		{
			get
			{
				return this.fProtocol;
			}
			set
			{
				this.fProtocol = value;

				if (this.Port == this.fDefaultPort)
					this.Port = KnownProtocols.GetProtocolDefaultPort(this.fProtocol);

				this.fDefaultPort = KnownProtocols.GetProtocolDefaultPort(this.fProtocol);
			}
		}
		private String fProtocol;

		public String Hostname { get; set; }

		public Int32 Port { get; set; }


		public String Path
		{
			get
			{
				return this.fPath;
			}
			set
			{
				this.fPath = String.IsNullOrEmpty(value) ? "/" : value;
			}
		}
		private String fPath;

		public String Params { get; set; }
		#endregion

		public override String ToString()
		{
			return this.Url;
		}

		const String DEFAULT_PROTOCOL = "http";

		public void Parse(String url)
		{
			Int32 lProtocolPosition = url.IndexOf("://", StringComparison.Ordinal);
			if (lProtocolPosition >= 0)
			{
				this.Protocol = url.Substring(0, lProtocolPosition);
				url = url.Substring(lProtocolPosition + 3); /* skip over :// */
			}
			else
			{
				this.Protocol = DEFAULT_PROTOCOL;
			}

			String lHostAndPort;
			lProtocolPosition = url.IndexOf('/');
			if (lProtocolPosition == -1)
				lProtocolPosition = url.IndexOf('?');

			if (lProtocolPosition >= 0)
			{
				lHostAndPort = url.Substring(0, lProtocolPosition);
				url = url.Substring(lProtocolPosition);
			}
			else
			{
				lHostAndPort = url;
				url = "";
			}

			if (lHostAndPort.StartsWith("["))
			{
				lProtocolPosition = lHostAndPort.IndexOf(']');

				if (lProtocolPosition > 0)
				{
					this.Hostname = lHostAndPort.Substring(1, lProtocolPosition - 1);
					String lRest = lHostAndPort.Substring(lProtocolPosition + 1).Trim();
					if (lRest.StartsWith(":"))
					{
						String lPort = lRest.Substring(1);
						try
						{
							this.Port = Int32.Parse(lPort);
						}
						catch (FormatException ex)
						{
							throw new UrlParserException(String.Format("Invalid Port specification '{0}'", lPort), ex);
						}
					}
				}
				else
				{
					throw new UrlParserException(String.Format("Invalid IPv6 host name specification '{0}'", lHostAndPort));
				}
			}
			else
			{
				lProtocolPosition = lHostAndPort.IndexOf(':');

				if (lProtocolPosition >= 0)
				{
					this.Hostname = lHostAndPort.Substring(0, lProtocolPosition);
					String lPort = lHostAndPort.Substring(lProtocolPosition + 1);
					try
					{
						this.Port = Int32.Parse(lPort);
					}
					catch (FormatException ex)
					{
						throw new UrlParserException(String.Format("Invalid Port specification '{0}'", lPort), ex);
					}
				}
				else
				{
					this.Hostname = lHostAndPort;
					this.Port = fDefaultPort;
				}
			}

			lProtocolPosition = url.IndexOf('?');
			if (lProtocolPosition >= 0)
			{
				this.Path = url.Substring(0, lProtocolPosition);
				this.Params = url.Substring(lProtocolPosition + 1);
			}
			else
			{
				if (url.Length == 0)
					url = "/";
				this.Path = url;
				this.Params = null;
			}
		}
	}

	#region UrlConverter class
#if FULLFRAMEWORK
	class UrlConverter : ExpandableObjectConverter
	{
		public UrlConverter()
		{
		}

		public override Boolean CanConvertFrom(ITypeDescriptorContext context, Type type)
		{
			return type == typeof(String) || base.CanConvertFrom(context, type);
		}

		public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
		{
			return value is String ? new UrlParser((String)value) : base.ConvertFrom(context, culture, value);
		}

		public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
		{
			return destinationType == typeof(String) && value is UrlParser
				? ((UrlParser)value).ToString()
				: base.ConvertTo(context, culture, value, destinationType);
		}
	}
#endif
	#endregion

	[Serializable]
	public class UrlParserException : Exception
	{
		public UrlParserException()
		{
		}

		public UrlParserException(String message)
			: base(message)
		{
		}

		public UrlParserException(String message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}