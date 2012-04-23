/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;

namespace RemObjects.InternetPack.Http
{
#if FULLFRAMEWORK
    [TypeConverter(typeof(ExpandableObjectConverter))]
#endif
    public sealed class HttpProxySettings
    {
        private const String DEFAULT_HOSTNAME = @"192.168.1.1";
        private const Int32 DEFAULT_PORT = 3128;

        public HttpProxySettings()
        {
            this.UseProxy = false;
            this.ProxyHost = HttpProxySettings.DEFAULT_HOSTNAME;
            this.ProxyPort = HttpProxySettings.DEFAULT_PORT;
            this.UserName = String.Empty;
            this.Password = String.Empty;
        }

        [DefaultValue(false)]
        public Boolean UseProxy
        {
            get
            {
                return this.fUseProxy;
            }
            set
            {
                this.fUseProxy = value;
            }
        }
        private Boolean fUseProxy;

        [DefaultValue(HttpProxySettings.DEFAULT_HOSTNAME)]
        public String ProxyHost
        {
            get
            {
                return this.fProxyHost;
            }
            set
            {
                this.fProxyHost = value;
            }
        }
        private String fProxyHost;

        [DefaultValue(HttpProxySettings.DEFAULT_PORT)]
        public Int32 ProxyPort
        {
            get
            {
                return this.fProxyPort;
            }
            set
            {
                this.fProxyPort = value;
            }
        }
        private Int32 fProxyPort;

        [DefaultValue("")]
        public String UserName
        {
            get
            {
                return this.fUserName;
            }
            set
            {
                this.fUserName = value;
            }
        }
        private String fUserName;

        [DefaultValue("")]
        public String Password
        {
            get
            {
                return this.fPassword;
            }
            set
            {
                this.fPassword = value;
            }
        }
        private String fPassword;

        public void CopyProperties(HttpProxySettings source)
        {
            this.UseProxy = source.UseProxy;
            this.ProxyHost = source.ProxyHost;
            this.ProxyPort = source.ProxyPort;
            this.UserName = source.UserName;
            this.Password = source.Password;
        }
    }
}