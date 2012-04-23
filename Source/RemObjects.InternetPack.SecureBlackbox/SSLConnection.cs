using System;
using System.Net;
using System.Net.Sockets;
using RemObjects.InternetPack.Core;
using SecureBlackbox;
using SecureBlackbox.SSLSocket;
using SecureBlackbox.SSLSocket.Client;
using SecureBlackbox.SSLSocket.Server;

namespace RemObjects.InternetPack.SecureBlackbox
{
	/// <summary>
	/// SSLConnection is a base class for SSL-enabled connections 
	/// </summary>
	public abstract class SSLConnection :  RemObjects.InternetPack.Core.Connection
	{
		protected ElSSLSocket fDataSocket = null;

		public SSLConnection() : base((System.Net.Sockets.Socket)null)
		{
		}

		public SSLConnection(System.Net.Sockets.Socket aSocket) 
			: base((Socket)aSocket)
		{
		}

		public SSLConnection(RemObjects.InternetPack.Core.Binding aBinding)
			:base(aBinding)
		{
		}

		protected System.Net.Sockets.Socket Socket
		{
			get
			{
				return base.DataSocket;
			}
		}

		#region ConnectionOverrides

		protected new ElSSLSocket DataSocket		{			get              			{				return fDataSocket;			}			set			{				fDataSocket = value;			}		}        
		protected override int DataSocketAvailable 
		{	
			get	{	return DataSocket.Available; } 
		}

		protected override int DataSocketReceiveWhatsAvaiable(byte[] aBuffer, int aOffset, int aSize)
		{
			StartTimeoutTimer();
			try
			{
				byte[] tmp = new byte[aSize];
				int result = DataSocket.Receive(tmp);
				Array.Copy(tmp, 0, aBuffer, aOffset, result);
				return result;
			}
			finally
			{
				StopTimeoutTimer();
			}
		}

		protected override int DataSocketSendAsMuchAsPossible(byte[] aBuffer, int aOffset, int aSize)
		{
			byte[] tmp = new byte[aSize];
			Array.Copy(aBuffer, aOffset, tmp, 0, aSize);
			return DataSocket.Send(tmp);
		}
		#endregion

		#region Properties
		public bool get_CipherSuites(short Index)
		{
			return DataSocket.get_CipherSuites(Index);
		}
		
		public void set_CipherSuites(short Index, bool Value)
		{
			DataSocket.set_CipherSuites(Index, Value);
		}
		
		public short CipherSuite
		{
			get
			{
				return DataSocket.CipherSuite;
			}
			set
			{
				DataSocket.CipherSuite = value;
			}
		}
		
		public short CurrentVersion
		{
			get
			{
				return DataSocket.CurrentVersion;
			}
			set
			{
				DataSocket.CurrentVersion = value;
			}
		}

		
		public short Versions
		{
			get
			{
				return DataSocket.Versions;
			}
			set
			{
				DataSocket.Versions = value;
			}
		}
		
		public override bool DataSocketConnected
		{
			get 
			{
				return DataSocket.Connected;
			}
		}
		#endregion
	}

	public class ClientSSLConnection : SSLConnection
	{
		public ClientSSLConnection(
			System.Net.Sockets.Socket aSocket, 
			IPEndPoint remoteEndPoint, 
			IClientSSLConnectionSettings aSettings
			) 
			: base(aSocket)
		{
			fDataSocket = new ElClientSSLSocket();
			fDataSocket.Socket = aSocket;
			InitializeSSLSocket(aSettings);
			((ElClientSSLSocket)fDataSocket).Connect(remoteEndPoint);
		}

		public ClientSSLConnection(
			RemObjects.InternetPack.Core.Binding aBinding,
			IClientSSLConnectionSettings aSettings
			)
			: base(new Socket(aBinding.AddressFamily, aBinding.SocketType, aBinding.Protocol))
		{
			fDataSocket = new ElClientSSLSocket();
			fDataSocket.Socket = base.Socket;
			InitializeSSLSocket(aSettings);
			((ElClientSSLSocket)fDataSocket).Connect(new IPEndPoint(aBinding.Address, aBinding.Port));
		}

		private void InitializeSSLSocket(IClientSSLConnectionSettings aSettings)
		{
			((ElClientSSLSocket)fDataSocket).OnCertificateValidate += new SBClient.TSBValidateCertificateEvent(OnSecureClientCertificateValidate);
			((ElClientSSLSocket)fDataSocket).OnCertificateNeeded += new SBClient.TSBCertificateNeededEvent(OnSecureClientCertificateNeeded);
			((ElClientSSLSocket)fDataSocket).OnCertificateNeededEx += new SBClient.TSBCertificateNeededExEvent(OnSecureClientCertificateNeededEx);
            ((ElClientSSLSocket)fDataSocket).OnCertificateChoose += new SBClient.TSBChooseCertificateEvent(OnSecureClientCertificateChoose);
			
			if (aSettings != null)
			{
				if (aSettings.get_OnCertificateValidate() != null)
					this.OnCertificateValidate += aSettings.get_OnCertificateValidate();
				if (aSettings.get_OnCertificateNeeded() != null)
					this.OnCertificateNeeded += aSettings.get_OnCertificateNeeded();
				if (aSettings.get_OnCertificateNeededEx() != null)
					this.OnCertificateNeededEx += aSettings.get_OnCertificateNeededEx();
				if (aSettings.get_OnCertificateChoose() != null)
					this.OnCertificateChoose += aSettings.get_OnCertificateChoose();

				for (short i = SBConstants.Unit.SB_SUITE_FIRST; 
					i < SBConstants.Unit.SB_SUITE_LAST; i++)
					set_CipherSuites(i, aSettings.get_CipherSuites(i));

				this.CertStorage = aSettings.CertStorage;
				this.Versions = aSettings.Versions;
			}
		}

		#region Socket operation overrides
		protected override void DataSocketConnect(EndPoint aEndPoint)
		{
			((ElClientSSLSocket) DataSocket).Connect(aEndPoint);		}
		
		protected override void DataSocketClose()
		{						((ElClientSSLSocket) DataSocket).Close(true);		}
		#endregion
	
		#region Properties and events

		public SBCustomCertStorage.TElCustomCertStorage CertStorage 
		{ 
			get
			{
				return ((ElClientSSLSocket) DataSocket).CertStorage;
			}
			set
			{
				((ElClientSSLSocket) DataSocket).CertStorage = value;
			}
		}

		public SBClient.TSBCloseReason CloseReason
		{
			get
			{
				return ((ElClientSSLSocket) DataSocket).CloseReason;
			}
		}
		
		public event SBClient.TSBValidateCertificateEvent OnCertificateValidate;
		public event SBClient.TSBCertificateNeededEvent OnCertificateNeeded;
		public event SBClient.TSBCertificateNeededExEvent OnCertificateNeededEx;
		public event SBClient.TSBChooseCertificateEvent OnCertificateChoose;

		#endregion

		private void OnSecureClientCertificateValidate(Object sender,
			SBX509.TElX509Certificate certificate, ref bool validate)
		{
			if (OnCertificateValidate != null)
				OnCertificateValidate(this, certificate, ref validate);
		}

		private void OnSecureClientCertificateNeeded(Object sender, 
			ref byte[] CertificateBuffer, ref int CertificateSize,
			ref byte[] PrivateKeyBuffer, ref int PrivateKeySize,
			SBClient.TClientCertificateType ClientCertificateType)
		{
			if (OnCertificateNeeded != null)
				OnCertificateNeeded(this, ref CertificateBuffer, ref CertificateSize,
					ref PrivateKeyBuffer, ref PrivateKeySize, ClientCertificateType);		
		}

		private void OnSecureClientCertificateNeededEx(Object sender,
			ref SBX509.TElX509Certificate certificate)
		{
			if (OnCertificateNeededEx != null)
				OnCertificateNeededEx(this, ref certificate);
		}

		private void OnSecureClientCertificateChoose(object Sender, 
			SBX509.TElX509Certificate[] Certificates, ref int CertificateIndex)
		{
			if (OnCertificateChoose != null)
				OnCertificateChoose(this, Certificates, ref CertificateIndex);
		}		
	}


	public class ServerSSLConnection : SSLConnection
	{
		public ServerSSLConnection(System.Net.Sockets.Socket aSocket, IServerSSLConnectionSettings aSettings)
		{
			fDataSocket = new ElServerSSLSocket(aSocket);
			InitializeSSLSocket(aSettings);
			((ElServerSSLSocket)DataSocket).OpenSSLSession();
		}

		private void InitializeSSLSocket(IServerSSLConnectionSettings aSettings)
		{
			((ElServerSSLSocket)fDataSocket).OnCertificateValidate += 
				new SBServer.TSBCertificateValidateEvent(OnSecureServerCertificateValidate);
			
			if (aSettings != null)
			{
				if (aSettings.get_OnCertificateValidate() != null)
					this.OnCertificateValidate += aSettings.get_OnCertificateValidate();

				for (short i = SBConstants.Unit.SB_SUITE_FIRST; 
                    i < SBConstants.Unit.SB_SUITE_LAST; i++)
                    set_CipherSuites(i, aSettings.get_CipherSuites(i));

				this.CertStorage = aSettings.CertStorage;
				this.Versions = aSettings.Versions;

				this.ClientAuthentication = aSettings.ClientAuthentication;
				this.ClientCertStorage = aSettings.ClientCertStorage;
				this.ForceCertificateChain = aSettings.ForceCertificateChain;
				this.SessionPool = aSettings.SessionPool;
			}
		}

		#region Socket operation overrides
		protected override void DataSocketInitializeServerConnection()
		{
		}
		
		protected override void DataSocketClose()
		{						((ElServerSSLSocket) DataSocket).Close(false);		}
		#endregion

		#region Properties and events
		public bool ForceCertificateChain 
		{
			get 
			{
				return ((ElServerSSLSocket) DataSocket).ForceCertificateChain;
			}
			set
			{
				((ElServerSSLSocket) DataSocket).ForceCertificateChain = value;
			}
		}

		public SBSessionPool.TElSessionPool SessionPool 
		{ 
			get
			{
				return ((ElServerSSLSocket) DataSocket).SessionPool;
			}
			set
			{
				((ElServerSSLSocket) DataSocket).SessionPool = value;
			}
		}

		public SBCustomCertStorage.TElMemoryCertStorage CertStorage 
		{ 
			get
			{
				return ((ElServerSSLSocket) DataSocket).CertStorage;
			}
			set
			{
				((ElServerSSLSocket) DataSocket).CertStorage = value;
			}
		}

		public SBCustomCertStorage.TElCustomCertStorage ClientCertStorage 
		{ 
			get
			{
				return ((ElServerSSLSocket) DataSocket).ClientCertStorage;
			}
			set
			{
				((ElServerSSLSocket) DataSocket).ClientCertStorage = value;
			}
		}

		public bool ClientAuthentication 
		{ 
			get
			{
				return ((ElServerSSLSocket) DataSocket).ClientAuthentication;
			}
			set
			{
				((ElServerSSLSocket) DataSocket).ClientAuthentication = value;
			}
		}

		public event SBServer.TSBCertificateValidateEvent OnCertificateValidate;
		
		#endregion

		private void OnSecureServerCertificateValidate(object Sender, SBX509.TElX509Certificate certificate, ref bool validate)
		{
			if (this.OnCertificateValidate != null)
				this.OnCertificateValidate(this, certificate, ref validate);
		}
	}
}
