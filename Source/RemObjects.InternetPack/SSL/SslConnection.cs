/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace RemObjects.InternetPack
{
	public class SslConnection : Connection
	{
		#region Nested classes
		private sealed class InnerConnection : Connection
		{
			public InnerConnection(Socket socket)
				: base(socket)
			{
			}

			public InnerConnection(Binding binding)
				: base(binding)
			{
			}

			public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
			{
				return this.ReceiveWhatsAvailable(buffer, offset, size);
			}
		}

		private class ConnectAsyncReult : IAsyncResult
		{
			#region Private fields
			private readonly AsyncCallback fCallback;
			private volatile Boolean fComplete;
			private Exception fFailure;
			#endregion

			public ConnectAsyncReult(AsyncCallback callback, Object state)
			{
				this.AsyncState = state;
				this.fCallback = callback;
			}

			public Object AsyncState { get; private set; }

			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get
				{
					if (this.fWaitHandle != null)
					{
						return this.fWaitHandle;
					}

					lock (this)
					{
						if (this.fWaitHandle == null)
						{
							this.fWaitHandle = new System.Threading.ManualResetEvent(fComplete);
						}
					}

					return this.fWaitHandle;
				}
			}
			private volatile System.Threading.ManualResetEvent fWaitHandle;

			public Boolean CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public Boolean IsCompleted
			{
				get
				{
					return this.fComplete;
				}
			}

			public void ConnectionConnect(IAsyncResult ar)
			{
				SslConnection lOwner = (SslConnection)ar.AsyncState;
				try
				{
					lOwner.fInnerConnection.EndConnect(ar);
				}
				catch (Exception ex)
				{
					this.fFailure = ex;
					this.fComplete = true;
					lock (this)
					{
						if (this.fWaitHandle != null)
						{
							this.fWaitHandle.Set();
						}
					}
					this.fCallback(ar);

					return;
				}

				if (lOwner.GetMonoAssembly() != null)
				{
					lOwner.CreateMonoClientStream();
					this.fComplete = true;
					lock (this)
					{
						if (this.fWaitHandle != null)
						{
							this.fWaitHandle.Set();
						}
					}
					this.fCallback(ar);

					return;
				}

				lOwner.fSsl = new SslStream(lOwner.fInnerConnection, true, lOwner.NetSsl_RemoteCertificateValidation);
				((SslStream)lOwner.fSsl).BeginAuthenticateAsClient(lOwner.fFactory.TargetHostName, SslAuthenticateAsClient, lOwner);
			}

			private void SslAuthenticateAsClient(IAsyncResult ar)
			{
				SslConnection lOwner = (SslConnection)ar.AsyncState;
				try
				{
					((SslStream)lOwner.fSsl).EndAuthenticateAsClient(ar);
				}
				catch (Exception ex)
				{
					this.fFailure = ex;
					this.fComplete = true;
					lock (this)
					{
						if (this.fWaitHandle != null)
						{
							this.fWaitHandle.Set();
						}
					}
					this.fCallback(ar);
					return;
				}

				lOwner.CreateMonoClientStream();
				this.fComplete = true;
				lock (this)
				{
					if (this.fWaitHandle != null)
					{
						this.fWaitHandle.Set();
					}
				}
				this.fCallback(ar);
			}

			private void Dispose()
			{
				if (this.fWaitHandle != null)
				{
					this.fWaitHandle.Close();
				}

				this.fWaitHandle = null;
			}

			public void EndConnect()
			{
				if (!fComplete)
				{
					this.AsyncWaitHandle.WaitOne();
				}

				this.Dispose();
				if (this.fFailure != null)
				{
					throw this.fFailure;
				}
			}
		}
		#endregion

		#region Private static cache
		private static Object fMonoSecurityProtocolDefault;
		private static Object fMonoSecurityProtocolTls;
		private static System.Security.Authentication.SslProtocols fNetSecurityProtocolTls = System.Security.Authentication.SslProtocols.None;
		#endregion

		#region Private fields
		private readonly SslConnectionFactory fFactory;
		private readonly Connection fInnerConnection;
		private Stream fSsl;
		#endregion

		#region Constructors
		public SslConnection(SslConnectionFactory factory, Binding binding)
			: base((Socket)null)
		{
			this.fFactory = factory;
			this.fInnerConnection = new InnerConnection(binding) { BufferedAsync = false };
			this.fInnerConnection.AsyncDisconnect += InnerConnection_AsyncDisconnect;
			this.fInnerConnection.AsyncHaveIncompleteData += InnerConnection_AsyncHaveIncompleteData;
		}

		public SslConnection(SslConnectionFactory factory, Socket socket)
			: base((Socket)null)
		{
			this.fFactory = factory;
			this.fInnerConnection = new InnerConnection(socket) { BufferedAsync = false };
			this.fInnerConnection.AsyncDisconnect += InnerConnection_AsyncDisconnect;
			this.fInnerConnection.AsyncHaveIncompleteData += InnerConnection_AsyncHaveIncompleteData;
		}

		public SslConnection(SslConnectionFactory factory, Connection connection)
			: base((Socket)null)
		{
			this.fFactory = factory;
			this.fInnerConnection = connection;
			this.fInnerConnection.BufferedAsync = false;
			this.fInnerConnection.AsyncDisconnect += InnerConnection_AsyncDisconnect;
			this.fInnerConnection.AsyncHaveIncompleteData += InnerConnection_AsyncHaveIncompleteData;
		}
		#endregion

		#region Properties
		public override Int32 DataSocketAvailable
		{
			get
			{
				return fInnerConnection.DataSocketAvailable;
			}
		}

		public override Socket DataSocket
		{
			get
			{
				return this.fInnerConnection.DataSocket;
			}
		}

		public override Boolean DataSocketConnected
		{
			get
			{
				return fInnerConnection.Connected;
			}
		}

		public override Boolean EnableNagle
		{
			get
			{
				return fInnerConnection.EnableNagle;
			}
			set
			{
				fInnerConnection.EnableNagle = value;
			}
		}

		public override Boolean Secure
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Mono SSL stream management
		private void CreateMonoServerStream()
		{
			Type lType = this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.SslServerStream", true);
			this.fSsl = (Stream)Activator.CreateInstance(lType, this.fInnerConnection, this.fFactory.Certificate, false, false, this.GetMonoSecurityProtocol());

			// TODO Add type caching
			Object lPrivateKeySelectionCallback = Delegate.CreateDelegate(this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.PrivateKeySelectionCallback", true), this, "MonoSsl_GetPrivateKey", false, true);
			lType.InvokeMember("set_PrivateKeyCertSelectionDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { lPrivateKeySelectionCallback });

			Object lCertificateValidationCallback = Delegate.CreateDelegate(this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.CertificateValidationCallback", true), this, "MonoSsl_RemoteCertificateValidation", false, true);
			lType.InvokeMember("set_ClientCertValidationDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { lCertificateValidationCallback });
		}

		private void CreateMonoClientStream()
		{
			Type lType = this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.SslClientStream", true);
			this.fSsl = (Stream)Activator.CreateInstance(lType, this.fInnerConnection, this.fFactory.TargetHostName, false, this.GetMonoSecurityProtocol());

			// TODO Add type caching
			Object lPrivateKeySelectionCallback = Delegate.CreateDelegate(this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.PrivateKeySelectionCallback", true), this, "MonoSsl_GetPrivateKey", false, true);
			lType.InvokeMember("set_PrivateKeyCertSelectionDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { lPrivateKeySelectionCallback });

			Object lCertificateValidationCallback = Delegate.CreateDelegate(this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.CertificateValidationCallback", true), this, "MonoSsl_RemoteCertificateValidation", false, true);
			lType.InvokeMember("set_ServerCertValidationDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { lCertificateValidationCallback });
		}

		private Assembly GetMonoAssembly()
		{
			if (!this.fFactory.UseMono)
			{
				return null;
			}

			Assembly lAssembly = SslMonoSecurityWrapper.GetAssembly();

			this.fFactory.UseMono = lAssembly != null;

			return lAssembly;
		}

		private Object GetMonoSecurityProtocol()
		{
			// Lock-less approach
			// In the worst case there will be an excessive Reflection call (or a couple of them)
			if ((SslConnection.fMonoSecurityProtocolDefault != null) && (SslConnection.fMonoSecurityProtocolTls != null))
			{
				return this.fFactory.UseTls ? SslConnection.fMonoSecurityProtocolTls : SslConnection.fMonoSecurityProtocolDefault;
			}

			Type lSecurityProtocolType = this.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.SecurityProtocolType");
			Int32 lTlsValue = (Int32)Enum.Parse(lSecurityProtocolType, "Tls");
			Int32 lDefaultValue = (Int32)Enum.Parse(lSecurityProtocolType, "Default");

			SslConnection.fMonoSecurityProtocolTls = Enum.ToObject(lSecurityProtocolType, lTlsValue);
			SslConnection.fMonoSecurityProtocolDefault = Enum.ToObject(lSecurityProtocolType, lDefaultValue);

			return this.fFactory.UseTls ? SslConnection.fMonoSecurityProtocolTls : SslConnection.fMonoSecurityProtocolDefault;
		}

		private AsymmetricAlgorithm MonoSsl_GetPrivateKey(X509Certificate certificate, String targetHost)
		{
			return this.fFactory.Certificate.PrivateKey;
		}

		private Boolean MonoSsl_RemoteCertificateValidation(X509Certificate certificate, Int32[] errors)
		{
			return fFactory.OnValidateRemoteCertificate(certificate);
		}
		#endregion

		#region .NET SSL stream management
		private void CreateNetServerStream()
		{
			SslStream lStream = new SslStream(this.fInnerConnection, true, NetSsl_RemoteCertificateValidation);
			lStream.AuthenticateAsServer(this.fFactory.Certificate, false, this.GetNetSecurityProtocol(), false);

			this.fSsl = lStream;
		}

		private void CreateNetClientStream()
		{
			SslStream lStream = new SslStream(this.fInnerConnection, true, this.NetSsl_RemoteCertificateValidation);
			lStream.AuthenticateAsClient(this.fFactory.TargetHostName, new X509CertificateCollection(), this.GetNetSecurityProtocol(), false);

			this.fSsl = lStream;
		}

		private System.Security.Authentication.SslProtocols GetNetSecurityProtocol()
		{
			if (SslConnection.fNetSecurityProtocolTls == System.Security.Authentication.SslProtocols.None)
			{
				// In the worst case these Reflection calls will be executed several times
				try
				{
					SslConnection.fNetSecurityProtocolTls = System.Security.Authentication.SslProtocols.Tls | (System.Security.Authentication.SslProtocols)Enum.Parse(typeof(System.Security.Authentication.SslProtocols), "Tls12");
				}
				catch (ArgumentException)
				{
					// Enum.Parse will fail on .NET less than 4.5
					SslConnection.fNetSecurityProtocolTls = System.Security.Authentication.SslProtocols.Tls;
				}
			}

			return this.fFactory.UseTls ? SslConnection.fNetSecurityProtocolTls : System.Security.Authentication.SslProtocols.Default;
		}

		private Boolean NetSsl_RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
		{
			return this.fFactory.OnValidateRemoteCertificate(certificate);
		}
		#endregion

		protected internal override void InitializeServerConnection()
		{
			if (this.GetMonoAssembly() != null)
			{
				this.CreateMonoServerStream();
			}
			else
			{
				this.CreateNetServerStream();
			}
		}

		protected internal override IAsyncResult BeginInitializeServerConnection(AsyncCallback callback, Object state)
		{
			if (this.fFactory.UseMono)
			{
				this.CreateMonoServerStream();
				return null;
			}

			this.fSsl = new SslStream(this.fInnerConnection, true, NetSsl_RemoteCertificateValidation);
			return ((SslStream)this.fSsl).BeginAuthenticateAsServer(this.fFactory.Certificate, false, this.GetNetSecurityProtocol(), false, callback, state);
		}

		protected internal override void EndInitializeServerConnection(IAsyncResult ar)
		{
			if (!this.fFactory.UseMono)
			{
				((SslStream)this.fSsl).EndAuthenticateAsServer(ar);
			}
		}

		public override void Connect(System.Net.EndPoint endPoint)
		{
			this.fInnerConnection.Connect(endPoint);

			this.InitializeClientConnection();
		}

		public virtual void InitializeClientConnection()
		{
			if (this.GetMonoAssembly() != null)
			{
				this.CreateMonoClientStream();
			}
			else
			{
				this.CreateNetClientStream();
			}
		}

		protected virtual IAsyncResult BeginInitializeClientConnection(AsyncCallback callback, Object state)
		{
			if (this.GetMonoAssembly() != null)
			{
				this.CreateMonoClientStream();
				return null;
			}

			this.fSsl = new SslStream(this.fInnerConnection, true, NetSsl_RemoteCertificateValidation);
			return ((SslStream)this.fSsl).BeginAuthenticateAsClient(this.fFactory.TargetHostName, new X509CertificateCollection(), this.GetNetSecurityProtocol(), false, callback, state);
		}

		protected void EndInitializeClientConnection(IAsyncResult ar)
		{
			if (this.GetMonoAssembly() == null)
			{
				((SslStream)this.fSsl).EndAuthenticateAsClient(ar);
			}
		}

		public override void Connect(System.Net.IPAddress address, Int32 port)
		{
			this.Connect(new System.Net.IPEndPoint(address, port));
		}

		public override IAsyncResult BeginConnect(System.Net.EndPoint endPoint, AsyncCallback callback, Object state)
		{
			ConnectAsyncReult lWrapper = new ConnectAsyncReult(callback, state);
			this.fInnerConnection.BeginConnect(endPoint, lWrapper.ConnectionConnect, this);

			return lWrapper;
		}

		public override IAsyncResult BeginConnect(System.Net.IPAddress address, Int32 port, AsyncCallback callback, Object state)
		{
			return this.BeginConnect(new System.Net.IPEndPoint(address, port), callback, state);
		}

		public override void EndConnect(IAsyncResult ar)
		{
			((ConnectAsyncReult)ar).EndConnect();
		}

		protected override void DataSocketClose()
		{
			this.fSsl.Dispose();
			this.fInnerConnection.Close();
		}

		protected override void DataSocketClose(Boolean dispose)
		{
			this.fSsl.Dispose();
			this.fInnerConnection.Close();
		}

		private void InnerConnection_AsyncHaveIncompleteData(Object sender, EventArgs e)
		{
			this.TriggerAsyncHaveIncompleteData();
		}

		private void InnerConnection_AsyncDisconnect(Object sender, EventArgs e)
		{
			this.TriggerAsyncDisconnect();
		}

		protected override Int32 DataSocketReceiveWhatsAvaiable(Byte[] buffer, Int32 offset, Int32 size)
		{
			try
			{
				return this.fSsl.Read(buffer, offset, size);
			}
			catch (IOException)
			{
				throw new SocketException();
			}
		}

		protected override Int32 DataSocketSendAsMuchAsPossible(Byte[] buffer, Int32 offset, Int32 size)
		{
			try
			{
				this.fSsl.Write(buffer, offset, size);
				return size;
			}
			catch (IOException)
			{
				throw new SocketException();
			}
		}

		protected override IAsyncResult IntBeginRead(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{
			try
			{
				return this.fSsl.BeginRead(buffer, offset, count, callback, state);
			}
			catch (IOException)
			{
				throw new SocketException();
			}
		}

		protected override Int32 IntEndRead(IAsyncResult ar)
		{
			try
			{
				return this.fSsl.EndRead(ar);
			}
			catch (IOException)
			{
				throw new SocketException();
			}
		}

		protected override IAsyncResult IntBeginWrite(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{
			try
			{
				return this.fSsl.BeginWrite(buffer, offset, count, callback, state);
			}
			catch (IOException)
			{
				throw new SocketException();
			}
		}

		protected override void IntEndWrite(IAsyncResult ar)
		{
			try
			{
				this.fSsl.EndWrite(ar);
			}
			catch (IOException)
			{
				// SocketException is expected in all code that deals with Connection, not IOException
				throw new SocketException();
			}
		}
	}
}