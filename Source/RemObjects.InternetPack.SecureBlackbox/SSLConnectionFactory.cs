using System;
using System.Net;
using System.Net.Sockets;
using RemObjects.InternetPack.Core;
using SecureBlackbox;
using SecureBlackbox.SSLSocket.Server;
using SecureBlackbox.SSLSocket.Client;

namespace RemObjects.InternetPack.SecureBlackbox
{
	public interface ISSLConnectionSettings
	{
		bool get_CipherSuites(short Index);
	
	
		short Versions { get; }
	}

	public interface IClientSSLConnectionSettings : ISSLConnectionSettings
	{
		SBCustomCertStorage.TElCustomCertStorage CertStorage { get; }
		
		SBClient.TSBValidateCertificateEvent get_OnCertificateValidate();
		SBClient.TSBCertificateNeededEvent get_OnCertificateNeeded();
		SBClient.TSBCertificateNeededExEvent get_OnCertificateNeededEx();
		SBClient.TSBChooseCertificateEvent get_OnCertificateChoose();
	}

	public interface IServerSSLConnectionSettings : ISSLConnectionSettings
	{
		bool ClientAuthentication { get; }

		SBCustomCertStorage.TElMemoryCertStorage CertStorage { get; }

		SBCustomCertStorage.TElCustomCertStorage ClientCertStorage { get; }

		bool ForceCertificateChain { get; }

		SBSessionPool.TElSessionPool SessionPool { get; }

		SBServer.TSBCertificateValidateEvent get_OnCertificateValidate();
	}
	
	/// <summary>
	/// Summary description for SSLConnectionFactory.
	/// </summary>
	public abstract class  SSLConnectionFactory : RemObjects.InternetPack.Core.IConnectionFactory,
		ISSLConnectionSettings
	{
		private bool[] ciperSuites = new bool[SBConstants.Unit.SB_SUITE_LAST];
		private short versions = SBConstants.Unit.sbSSL2 | 
			SBConstants.Unit.sbSSL3 | 
			SBConstants.Unit.sbTLS1;
		
		#region IConnectionFactory Members

		public virtual Connection CreateServerConnection(System.Net.Sockets.Socket aSocket)
		{
			throw new InvalidOperationException();
		}

	    public virtual Connection CreateClientConnection(RemObjects.InternetPack.Core.Binding aBinding)
    	{
			throw new InvalidOperationException();
	    }

		#endregion

		#region ISSLConnectionSettings Members
		
		public bool get_CipherSuites(short Index)
		{
			if (Index < SBConstants.Unit.SB_SUITE_FIRST)
				throw new IndexOutOfRangeException();
			return ciperSuites[Index];		
		}

		public void set_CipherSuites(short Index, bool Value)
		{
			if (Index < SBConstants.Unit.SB_SUITE_FIRST)
				throw new IndexOutOfRangeException();
			ciperSuites[Index] = Value;		
		}

		public short Versions 
		{ 
			get
			{
				return versions;
			}
			set
			{
				versions = value;
			}
		}

		#endregion
	}

	public class SSLServerConnectionFactory : SSLConnectionFactory, IServerSSLConnectionSettings
	{
		private bool clientAuthentication = false; 
		private SBCustomCertStorage.TElMemoryCertStorage certStorage;
		private SBCustomCertStorage.TElCustomCertStorage clientCertStorage;
		private bool forceCertificateChain = false;
		private SBSessionPool.TElSessionPool sessionPool;

		public event SBServer.TSBCertificateValidateEvent OnCertificateValidate;

		public SBServer.TSBCertificateValidateEvent get_OnCertificateValidate()
		{
			return OnCertificateValidate;
		}

		public override Connection CreateServerConnection(System.Net.Sockets.Socket aSocket)
		{
			ServerSSLConnection conn = new ServerSSLConnection(aSocket, this);
			return conn;
		}

		public SBCustomCertStorage.TElMemoryCertStorage CertStorage 
		{ 
			get
			{
				return certStorage;	
			}
			set
			{
				certStorage = value;
			}
		}

		public bool ClientAuthentication 
		{	 
			get
			{
				return clientAuthentication;
			}
			set
			{
				clientAuthentication = value;
			}
		}

		public SBCustomCertStorage.TElCustomCertStorage ClientCertStorage 
		{ 
			get
			{
				return clientCertStorage;
			}
			set
			{
				clientCertStorage = value;
			}
		}

		public bool ForceCertificateChain 
		{ 
			get
			{
				return forceCertificateChain;	
			}
			set
			{
				forceCertificateChain = value;
			}
		}

		public SBSessionPool.TElSessionPool SessionPool 
		{ 
			get
			{
				return sessionPool;
			}
			set
			{
				sessionPool = value;
			}
		}
	}

	public class SSLClientConnectionFactory : SSLConnectionFactory, IClientSSLConnectionSettings 
	{
		private SBCustomCertStorage.TElCustomCertStorage certStorage;

		public event SBClient.TSBValidateCertificateEvent OnCertificateValidate;
		public event SBClient.TSBCertificateNeededEvent OnCertificateNeeded;
		public event SBClient.TSBCertificateNeededExEvent OnCertificateNeededEx;
		public event SBClient.TSBChooseCertificateEvent OnCertificateChoose;
		
		public override Connection CreateClientConnection(RemObjects.InternetPack.Core.Binding aBinding)
		{
			return new ClientSSLConnection(aBinding, this);
		}

		public SBCustomCertStorage.TElCustomCertStorage CertStorage 
		{ 
			get
			{
				return certStorage;	
			}
			set
			{
				certStorage = value;
			}
		}

		public SBClient.TSBValidateCertificateEvent get_OnCertificateValidate()
		{
			return OnCertificateValidate;
		}

		public SBClient.TSBCertificateNeededEvent get_OnCertificateNeeded()
		{
			return OnCertificateNeeded;
		}

		public SBClient.TSBCertificateNeededExEvent get_OnCertificateNeededEx()
		{
			return OnCertificateNeededEx;
		}

		public SBClient.TSBChooseCertificateEvent get_OnCertificateChoose()
		{
			return OnCertificateChoose;
		}

	}
}
