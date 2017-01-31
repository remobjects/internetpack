/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public class SslConnectionFactory : IConnectionFactory
	{
		private readonly Object fLockRoot;

		public SslConnectionFactory()
		{
			this.fLockRoot = new Object();
			this.UseMono = Environment.OSVersion.Platform != PlatformID.Win32NT;
			this.UseTls = true;
		}

		[Category("Ssl Options")]
		[DefaultValue(false)]
		public Boolean UseMono { get; set; }

		[Category("Ssl Options")]
		[DefaultValue("")]
		public String TargetHostName { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(true)]
		public Boolean UseTls { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(false)]
		public Boolean Enabled { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(null)]
		public String CertificateFileName { get; set; }

		[Category("Ssl Options")]
		[DefaultValue(null)]
		public String CertificateThumbprint { get; set; }

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
		public Boolean IsCertificateLoadPending
		{
			get
			{
				return (this.Certificate == null) && !(String.IsNullOrEmpty(this.CertificateFileName) && String.IsNullOrEmpty(this.CertificateThumbprint));
			}
		}

		public void LoadCertificate()
		{
			// Usual "Double Check To Avoid Lock" pattern
			if (this.Certificate != null)
			{
				return;
			}

			if (String.IsNullOrEmpty(this.CertificateFileName) && String.IsNullOrEmpty(this.CertificateThumbprint))
			{
				throw new CryptographicException("Certificate not set. Either set the certificate directly or provide its file name or thumbprint");
			}

			lock (this.fLockRoot)
			{
				if (this.Certificate != null)
				{
					return;
				}

				if (!String.IsNullOrEmpty(this.CertificateFileName))
				{
					this.LoadCertificateFromFile();
					return;
				}

				this.LoadCertificateFromStore();
			}
		}

		private void LoadCertificateFromFile()
		{
			SslNeedPasswordEventArgs lEventArgs = new SslNeedPasswordEventArgs();
			this.OnNeedPassword(lEventArgs);

			this.Certificate = new X509Certificate2(this.CertificateFileName, lEventArgs.Password, X509KeyStorageFlags.Exportable);
		}

		private void LoadCertificateFromStore()
		{
			// Initial data cleanup, just in case
			String lThumbprint = this.CertificateThumbprint.Replace("\u200e", "").Replace("\u200f", "").Replace(" ", "");

			X509Certificate2 lCertificate = SslConnectionFactory.LoadCertificateFromStore(lThumbprint, StoreLocation.CurrentUser) ??
											SslConnectionFactory.LoadCertificateFromStore(lThumbprint, StoreLocation.LocalMachine);

			if (lCertificate == null)
			{
				throw new CryptographicException("Cannot find certificate with provided thumbprint: " + this.CertificateThumbprint);
			}

			this.Certificate = lCertificate;
		}

		private static X509Certificate2 LoadCertificateFromStore(String thumbprint, StoreLocation location)
		{
			X509Store lCertificateStore = new X509Store(StoreName.My, location);
			try
			{
				lCertificateStore.Open(OpenFlags.ReadOnly);

				X509Certificate2Collection lCertificates = lCertificateStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);

				return (lCertificates.Count != 0) ? lCertificates[0] : null;
			}
			finally
			{
				lCertificateStore.Close();
				// X509Store doesn't implement IDisposable on .NET 3.5
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
			{
				return new Connection(binding);
			}

			if (this.IsCertificateLoadPending)
			{
				this.LoadCertificate();
			}

			return new SslConnection(this, binding);
		}

		public virtual Connection CreateClientConnection(Connection connection)
		{
			if (this.IsCertificateLoadPending)
			{
				this.LoadCertificate();
			}

			return new SslConnection(this, connection);
		}
		#endregion
	}
}