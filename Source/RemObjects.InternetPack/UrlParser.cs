/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

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

        private static IDictionary<String, Int32> fProtocols;

        public static void RegisterProtocol(String name, Int32 port)
        {
            if (fProtocols.ContainsKey(name))
                throw new Exception(String.Format("Protocol '{0}' already registered.", name));
            fProtocols.Add(name, port);
        }

        public static Int32 GetProtocolDefaultPort(String protocol)
        {
            if (fProtocols.ContainsKey(protocol))
                return (Int32)fProtocols[protocol];

            return -1;
        }
    }

#if FULLFRAMEWORK
    [TypeConverter(typeof(UrlConverter))]
#endif
    [Serializable]
    public class UrlParser : MarshalByRefObject
    {
        public UrlParser()
        {
            this.fProtocol = DEFAULT_PROTOCOL;
            this.fDefaultPort = KnownProtocols.GetProtocolDefaultPort(fProtocol);
            this.fPort = fDefaultPort;
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
                if (this.Params != null && this.Params.Length != 0)
                    return this.Path + "?" + this.Params;

                return this.Path;
            }
        }

        [Browsable(false)]
        public String HostnameAndPort
        {
            get
            {
                if (this.Port != this.fDefaultPort)
                    return this.Hostname + ":" + this.Port.ToString();

                return this.Hostname;
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

                if (this.fPort == this.fDefaultPort)
                    this.fPort = KnownProtocols.GetProtocolDefaultPort(this.fProtocol);

                this.fDefaultPort = KnownProtocols.GetProtocolDefaultPort(this.fProtocol);
            }
        }
        private String fProtocol;

        public String Hostname
        {
            get
            {
                return this.fHostname;
            }
            set
            {
                this.fHostname = value;
            }
        }
        private String fHostname;

        public Int32 Port
        {
            get
            {
                return this.fPort;
            }
            set
            {
                this.fPort = value;
            }
        }
        private Int32 fPort;
        private Int32 fDefaultPort;

        public String Path
        {
            get
            {
                return this.fPath;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                    this.fPath = "/";
                else
                    this.fPath = value;
            }
        }
        private String fPath;

        public String Params
        {
            get
            {
                return this.fParams;
            }
            set
            {
                this.fParams = value;
            }
        }
        private String fParams;
        #endregion

        public override String ToString()
        {
            return this.Url;
        }

        const String DEFAULT_PROTOCOL = "http";

        public void Parse(String url)
        {
            Int32 lProtocolPosition = url.IndexOf("://");
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
            if (type == typeof(String))
                return true;

            return base.CanConvertFrom(context, type);
        }

        public override Object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, Object value)
        {
            if (value is String)
                return new UrlParser(value as String);

            return base.ConvertFrom(context, culture, value);
        }

        public override Object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, Object value, Type destinationType)
        {
            if (destinationType == typeof(String) && value is UrlParser)
                return ((UrlParser)value).ToString();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
#endif
    #endregion

    [Serializable]
    public class UrlParserException : Exception
    {
        public UrlParserException()
            : base()
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
