/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace RemObjects.InternetPack
{
	public class SslConnectionFactory : IConnectionFactory
	{
		public SslConnectionFactory()
		{
			this.UseMono = Environment.OSVersion.Platform != PlatformID.Win32NT;
			this.UseTls = false;
		}

		[Category("Ssl Options")]
		[DefaultValue("")]
		public String TargetHostName { get; set; }

		[Category("Ssl Options")]
		public Boolean UseMono { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(false)]
		public Boolean UseTls { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(false)]
		public Boolean Enabled { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(null)]
		public String CertificateFileName { get; set; }

		[Category("Ssl Options")]
		public event EventHandler<SslNeedPasswordEventArgs> NeedPassword;

		protected virtual void OnNeedPassword(SslNeedPasswordEventArgs e)
		{
			if (this.NeedPassword != null)
			{
				this.NeedPassword(this, e);
			}
		}

		[Browsable(false)]
		public X509Certificate2 Certificate { get; set; }

		[Browsable(false)]
		public Boolean HasCertificate
		{
			get
			{
				return !((this.Certificate == null) && String.IsNullOrEmpty(this.CertificateFileName));
			}
		}

		protected void LoadCertificate()
		{
			lock (this)
			{
				if (this.Certificate != null)
				{
					return;
				}

				if (String.IsNullOrEmpty(this.CertificateFileName))
				{
					throw new InvalidOperationException("Certificate not set and CertificateFileName is empty");
				}

				SslNeedPasswordEventArgs lEventArgs = new SslNeedPasswordEventArgs();
				this.OnNeedPassword(lEventArgs);

				this.Certificate = new X509Certificate2(this.CertificateFileName, lEventArgs.Password, X509KeyStorageFlags.Exportable);
			}
		}

		[Category("Ssl Options")]
		public event EventHandler<SslValidateCertificateEventArgs> ValidateRemoteCertificate;

		protected virtual void OnValidateRemoteCertificate(SslValidateCertificateEventArgs e)
		{
			if (this.ValidateRemoteCertificate != null)
			{
				this.ValidateRemoteCertificate(this, e);
			}
		}

		public Boolean OnValidateRemoteCertificate(X509Certificate certificate)
		{
			SslValidateCertificateEventArgs lEventArgs = new SslValidateCertificateEventArgs(certificate);
			this.OnValidateRemoteCertificate(lEventArgs);

			return (!lEventArgs.Cancel);
		}

		#region IConnectionFactory Members
		public virtual Connection CreateServerConnection(Socket socket)
		{
			if (!this.Enabled)
			{
				return new Connection(socket);
			}

			if (this.Certificate == null)
			{
				this.LoadCertificate();
			}

			return new SslConnection(this, socket);
		}

		public virtual Connection CreateClientConnection(Binding binding)
		{
			if (!this.Enabled)
				return new Connection(binding);

			if (this.HasCertificate)
			{
				this.LoadCertificate();
			}

			return new SslConnection(this, binding);
		}

		public virtual Connection CreateClientConnection(Connection connection)
		{
			if (this.HasCertificate)
			{
				this.LoadCertificate();
			}

			return new SslConnection(this, connection);
		}
		#endregion
	}
}