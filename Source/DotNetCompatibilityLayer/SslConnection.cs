namespace RemObjects.InternetPack
{
	#if !echoes

	public sealed class OidCollection
	{
		//public System.Security.Cryptography.OidCollection();
		//public Int32 Add(Oid oid);
		//public OidEnumerator GetEnumerator();
		//private new IEnumerator System.Collections.IEnumerable.GetEnumerator();
		//private new void System.Collections.ICollection.CopyTo(Array array, Int32 index);
		//public void CopyTo(Oid[] array, Int32 index);
		//public Oid this[String oid] { get; set; }
		//public Oid this[Int32 index] { get; set; }
		//public new Int32 Count { get; set; }
		//public new Boolean IsSynchronized { get; set; }
		//public new Object SyncRoot { get; set; }
	}

	public enum X509RevocationMode
	{
		NoCheck = 0,
		Online = 1,
		Offline = 2
	}

	public enum X509VerificationFlags
	{
		NoFlag = 0,
		IgnoreNotTimeValid = 1,
		IgnoreCtlNotTimeValid = 2,
		IgnoreNotTimeNested = 4,
		IgnoreInvalidBasicConstraints = 8,
		AllowUnknownCertificateAuthority = 16,
		IgnoreWrongUsage = 32,
		IgnoreInvalidName = 64,
		IgnoreInvalidPolicy = 128,
		IgnoreEndRevocationUnknown = 256,
		IgnoreCtlSignerRevocationUnknown = 512,
		IgnoreCertificateAuthorityRevocationUnknown = 1024,
		IgnoreRootRevocationUnknown = 2048,
		AllFlags = 4095
	}

	public enum X509RevocationFlag
	{
		EndCertificateOnly = 0,
		EntireChain = 1,
		ExcludeRoot = 2
	}

	public abstract class CollectionBase
	{

	}

	public class X509CertificateCollection : CollectionBase
	{
		/*public System.Security.Cryptography.X509Certificates.X509CertificateCollection(X509Certificate[] @value);
		public System.Security.Cryptography.X509Certificates.X509CertificateCollection(X509CertificateCollection @value);
		public System.Security.Cryptography.X509Certificates.X509CertificateCollection();
		public Int32 Add(X509Certificate @value);
		public void AddRange(X509CertificateCollection @value);
		public void AddRange(X509Certificate[] @value);
		public Boolean Contains(X509Certificate @value);
		public void CopyTo(X509Certificate[] array, Int32 index);
		public Int32 IndexOf(X509Certificate @value);
		public void Insert(Int32 index, X509Certificate @value);
		public X509CertificateCollection.X509CertificateEnumerator GetEnumerator();
		public void Remove(X509Certificate @value);
		public override Int32 GetHashCode();
		public X509Certificate this[Int32 index] { get; set; }*/
	}

	public class X509Certificate2Collection : X509CertificateCollection
	{
		/*public System.Security.Cryptography.X509Certificates.X509Certificate2Collection(X509Certificate2[] certificates);
		public System.Security.Cryptography.X509Certificates.X509Certificate2Collection(X509Certificate2Collection certificates);
		public System.Security.Cryptography.X509Certificates.X509Certificate2Collection(X509Certificate2 certificate);
		 public System.Security.Cryptography.X509Certificates.X509Certificate2Collection();
		public Int32 Add(X509Certificate2 certificate);
		public void AddRange(X509Certificate2Collection certificates);
		public void AddRange(X509Certificate2[] certificates);
		public Boolean Contains(X509Certificate2 certificate);
		public void Insert(Int32 index, X509Certificate2 certificate);
		public X509Certificate2Enumerator GetEnumerator();
		public void Remove(X509Certificate2 certificate);
		public void RemoveRange(X509Certificate2Collection certificates);
		public void RemoveRange(X509Certificate2[] certificates);
		public X509Certificate2Collection Find(X509FindType findType, Object findValue, Boolean validOnly);
		public void Import(String fileName, String password, X509KeyStorageFlags keyStorageFlags);
		public void Import(String fileName);
		public void Import(Byte[] rawData, String password, X509KeyStorageFlags keyStorageFlags);
		public void Import(Byte[] rawData);
		public Byte[] Export(X509ContentType contentType, String password);
		public Byte[] Export(X509ContentType contentType);
		public X509Certificate2 this[Int32 index] { get; set; }*/
	}

	public sealed class X509ChainPolicy
	{
		public X509ChainPolicy() {}
		public void Reset() {}
		public OidCollection ApplicationPolicy { get; set; }
		public OidCollection CertificatePolicy { get; set; }
		public X509RevocationMode RevocationMode { get; set; }
		public X509RevocationFlag RevocationFlag { get; set; }
		public X509VerificationFlags VerificationFlags { get; set; }
		public DateTime VerificationTime { get; set; }
		public TimeSpan UrlRetrievalTimeout { get; set; }
		public X509Certificate2Collection ExtraStore { get; set; }
	}

	public enum X509ChainStatusFlags
	{
		NoError = 0,
		NotTimeValid = 1,
		NotTimeNested = 2,
		Revoked = 4,
		NotSignatureValid = 8,
		NotValidForUsage = 16,
		UntrustedRoot = 32,
		RevocationStatusUnknown = 64,
		Cyclic = 128,
		InvalidExtension = 256,
		InvalidPolicyConstraints = 512,
		InvalidBasicConstraints = 1024,
		InvalidNameConstraints = 2048,
		HasNotSupportedNameConstraint = 4096,
		HasNotDefinedNameConstraint = 8192,
		HasNotPermittedNameConstraint = 16384,
		HasExcludedNameConstraint = 32768,
		PartialChain = 65536,
		CtlNotTimeValid = 131072,
		CtlNotSignatureValid = 262144,
		CtlNotValidForUsage = 524288,
		OfflineRevocation = 16777216,
		NoIssuanceChainPolicy = 33554432
	}

	public struct X509ChainStatus
	{
		private X509ChainStatusFlags m_status;
		private String m_statusInformation;
		public X509ChainStatusFlags Status { get; set; }
		public String StatusInformation { get; set; }
	}

	public sealed class X509ChainElementCollection
	{
		/*public X509ChainElementEnumerator GetEnumerator();
		private new IEnumerator System.Collections.IEnumerable.GetEnumerator();
		private new void System.Collections.ICollection.CopyTo(Array array, Int32 index);
		public void CopyTo(X509ChainElement[] array, Int32 index);
		internal System.Security.Cryptography.X509Certificates.X509ChainElementCollection();
		public X509ChainElement this[Int32 index] { get; set; }
		public new Int32 Count { get; set; }
		public new Boolean IsSynchronized { get; set; }
		public new Object SyncRoot { get; set; }*/
	}

	public class X509Chain
	{
		public static X509Chain Create() {}
		//public X509Chain(IntPtr chainContext) {}
		public X509Chain(Boolean useMachineContext) {}

		public X509Chain() {}
		public Boolean Build(X509Certificate2 certificate) {}
		public void Reset() {}
		//public IntPtr ChainContext { get; set; }
		public X509ChainPolicy ChainPolicy { get; set; }
		public X509ChainStatus[] ChainStatus { get; set; }
		public X509ChainElementCollection ChainElements { get; set; }
	}

	public class X509Certificate
	{

	}

	public class X509Certificate2
	{

	}

	public enum SslPolicyErrors
	{
		None = 0,
		RemoteCertificateNotAvailable = 1,
		RemoteCertificateNameMismatch = 2,
		RemoteCertificateChainErrors = 4
	}

	public abstract class AuthenticatedStream : Stream
	{
		protected AuthenticatedStream(Stream innerStream, Boolean leaveInnerStreamOpen) {}
		//protected override void Dispose(Boolean disposing) {}
		public Boolean LeaveInnerStreamOpen { get; set; }
		protected Stream InnerStream { get; set; }
		public abstract Boolean IsAuthenticated { get; set; }
		public abstract Boolean IsMutuallyAuthenticated { get; set; }
		public abstract Boolean IsEncrypted { get; set; }
		public abstract Boolean IsSigned { get; set; }
		public abstract Boolean IsServer { get; set; }
	}

	public enum SslProtocols
	{
		None = 0,
		Ssl2 = 12,
		Ssl3 = 48,
		Tls = 192,
		Default = 240
	}

	public enum CipherAlgorithmType
	{
		None = 0,
		Rc2 = 26114,
		Rc4 = 26625,
		Des = 26113,
		TripleDes = 26115,
		Aes = 26129,
		Aes128 = 26126,
		Aes192 = 26127,
		Aes256 = 26128,
		Null = 24576
	}

	public enum HashAlgorithmType
	{
		None = 0,
		Md5 = 32771,
		Sha1 = 32772
	}

	public enum ExchangeAlgorithmType
	{
		None = 0,
		RsaSign = 9216,
		RsaKeyX = 41984,
		DiffieHellman = 43522
	}

	public enum ChannelBindingKind
	{
		Unknown = 0,
		Unique = 25,
		Endpoint = 26
	}

	public class ChannelBinding
	{
		/*public org.ietf.jgss.ChannelBinding(SByte[] arg1);
		public org.ietf.jgss.ChannelBinding(InetAddress arg1, InetAddress arg2, SByte[] arg3);
		public InetAddress getInitiatorAddress();
		public InetAddress getAcceptorAddress();
		public SByte[] getApplicationData();
		public Boolean @equals(Object arg1);
		public Integer hashCode();
		private InetAddress initiator;
		private InetAddress acceptor;
		private SByte[] appData;
		public override InetAddress InitiatorAddress { get; set; }
		public override InetAddress AcceptorAddress { get; set; }
		public override SByte[] ApplicationData { get; set; }*/
	}

	public abstract class TransportContext
	{
		public abstract ChannelBinding GetChannelBinding(ChannelBindingKind kind);
		protected TransportContext() {}
	}

	public delegate Boolean RemoteCertificateValidationCallback (Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);
	public delegate X509Certificate LocalCertificateSelectionCallback (Object sender, String targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, String[] acceptableIssuers);

	public enum EncryptionPolicy
	{
		RequireEncryption = 0,
		AllowNoEncryption = 1,
		NoEncryption = 2
	}

	public class SslStream : AuthenticatedStream
	{
		public SslStream(Stream innerStream, Boolean leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback, EncryptionPolicy encryptionPolicy) : this(innerStream, true) {}
		public SslStream(Stream innerStream, Boolean leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback, LocalCertificateSelectionCallback userCertificateSelectionCallback) : this(innerStream, true) {}
		public SslStream(Stream innerStream, Boolean leaveInnerStreamOpen, RemoteCertificateValidationCallback userCertificateValidationCallback) : this(innerStream, true) {}
		public SslStream(Stream innerStream, Boolean leaveInnerStreamOpen) {}
		public SslStream(Stream innerStream) : this(innerStream, true) {}
		public virtual void AuthenticateAsClient(String targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, Boolean checkCertificateRevocation) {}
		public virtual void AuthenticateAsClient(String targetHost) {}
		public virtual IAsyncResult BeginAuthenticateAsClient(String targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, Boolean checkCertificateRevocation, AsyncCallback asyncCallback, Object asyncState) {}
		public virtual IAsyncResult BeginAuthenticateAsClient(String targetHost, AsyncCallback asyncCallback, Object asyncState) {}
		public virtual void EndAuthenticateAsClient(IAsyncResult asyncResult) {}
		public virtual void AuthenticateAsServer(X509Certificate serverCertificate, Boolean clientCertificateRequired, SslProtocols enabledSslProtocols, Boolean checkCertificateRevocation) {}
		public virtual void AuthenticateAsServer(X509Certificate serverCertificate) {}
		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, Boolean clientCertificateRequired, SslProtocols enabledSslProtocols, Boolean checkCertificateRevocation, AsyncCallback asyncCallback, Object asyncState) {}
		public virtual IAsyncResult BeginAuthenticateAsServer(X509Certificate serverCertificate, AsyncCallback asyncCallback, Object asyncState) {}
		public virtual void EndAuthenticateAsServer(IAsyncResult asyncResult) {}
		//public override void SetLength(Int64 @value) {}
		public override Int64 Seek(Int64 offset, SeekOrigin origin) {}
		public override void Flush() {}
		public override void Close() {}
		//protected override void Dispose(Boolean disposing) {}
		public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count) {}
		public override Int32 Write(Byte[] buffer, Int32 offset, Int32 count) {}
		public void Write(Byte[] buffer) {}
		public /*override*/ IAsyncResult BeginRead(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback asyncCallback, Object asyncState) {}
		public /*override*/ Int32 EndRead(IAsyncResult asyncResult) {}
		public /*override*/ IAsyncResult BeginWrite(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback asyncCallback, Object asyncState) {}
		public /*override*/ void EndWrite(IAsyncResult asyncResult) {}
		public TransportContext TransportContext { get; set; }
		public override Boolean IsAuthenticated { get; set; }
		public override Boolean IsMutuallyAuthenticated { get; set; }
		public override Boolean IsEncrypted { get; set; }
		public override Boolean IsSigned { get; set; }
		public override Boolean IsServer { get; set; }
		public virtual SslProtocols SslProtocol { get; set; }
		public virtual Boolean CheckCertRevocationStatus { get; set; }
		public virtual X509Certificate LocalCertificate { get; set; }
		public virtual X509Certificate RemoteCertificate { get; set; }
		public virtual CipherAlgorithmType CipherAlgorithm { get; set; }
		public virtual Int32 CipherStrength { get; set; }
		public virtual HashAlgorithmType HashAlgorithm { get; set; }
		public virtual Int32 HashStrength { get; set; }
		public virtual ExchangeAlgorithmType KeyExchangeAlgorithm { get; set; }
		public virtual Int32 KeyExchangeStrength { get; set; }
		public override Boolean CanSeek { get; set; }
		public override Boolean CanRead { get; set; }
		public /*override*/ Boolean CanTimeout { get; set; }
		public override Boolean CanWrite { get; set; }
		public /*override*/ Int32 ReadTimeout { get; set; }
		public /*override*/ Int32 WriteTimeout { get; set; }
		public override Int64 Length { get; set; }
		public override Int64 Position { get; set; }
	}

	#endif
}