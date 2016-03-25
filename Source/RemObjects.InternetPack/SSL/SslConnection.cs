/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace RemObjects.InternetPack
{
	public class SslConnection : Connection
	{
		#region Private constants
		private Int32 MONO_TIMEOUT_POLL_PERIOD = 500; // 0.5 second
		#endregion

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

			// Just read and write data, no additional timeout management
			public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
			{
				return this.ReceiveWhatsAvailable(buffer, offset, size);
			}
		}

		private class ConnectAsyncResult : IAsyncResult
		{
			#region Private fields
			private readonly AsyncCallback fCallback;
			private volatile Boolean fComplete;
			private Exception fFailure;
			#endregion

			public ConnectAsyncResult(AsyncCallback callback, Object state)
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

				lOwner.CreateSslClientStream();
				lOwner.fSslStream.BeginAuthenticateAsClient(lOwner.fFactory.TargetHostName, EndAuthenticateAsClient, lOwner);
			}

			private void EndAuthenticateAsClient(IAsyncResult ar)
			{
				SslConnection lOwner = (SslConnection)ar.AsyncState;
				try
				{
					lOwner.fSslStream.EndAuthenticateAsClient(ar);
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
		private static System.Security.Authentication.SslProtocols fNetSecurityProtocolTls = System.Security.Authentication.SslProtocols.None;
		#endregion

		#region Private fields
		private readonly SslConnectionFactory fFactory;
		private readonly Connection fInnerConnection;
		private SslStream fSslStream;
		#endregion

		#region Constructors
		public SslConnection(SslConnectionFactory factory, Binding binding)
			: base((Socket)null)
		{
			this.fFactory = factory;
			this.fInnerConnection = new InnerConnection(binding);
			this.fInnerConnection.BufferedAsync = false;
			this.fInnerConnection.AsyncDisconnect += InnerConnection_AsyncDisconnect;
			this.fInnerConnection.AsyncHaveIncompleteData += InnerConnection_AsyncHaveIncompleteData;
		}

		public SslConnection(SslConnectionFactory factory, Socket socket)
			: base((Socket)null)
		{
			this.fFactory = factory;
			this.fInnerConnection = new InnerConnection(socket);
			this.fInnerConnection.BufferedAsync = false;
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

		#region SSL stream management
		private void CreateSslServerStream()
		{
			this.fSslStream = new SslStream(this.fInnerConnection, true, NetSsl_RemoteCertificateValidation);
		}

		private void CreateSslClientStream()
		{
			this.fSslStream = new SslStream(this.fInnerConnection, true, this.NetSsl_RemoteCertificateValidation);
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
			this.CreateSslServerStream();
			this.fSslStream.AuthenticateAsServer(this.fFactory.Certificate, false, this.GetNetSecurityProtocol(), false);
		}

		protected internal override IAsyncResult BeginInitializeServerConnection(AsyncCallback callback, Object state)
		{
			this.CreateSslServerStream();
			return this.fSslStream.BeginAuthenticateAsServer(this.fFactory.Certificate, false, this.GetNetSecurityProtocol(), false, callback, state);
		}

		protected internal override void EndInitializeServerConnection(IAsyncResult ar)
		{
			this.fSslStream.EndAuthenticateAsServer(ar);
		}

		public override void Connect(System.Net.EndPoint endPoint)
		{
			this.fInnerConnection.Connect(endPoint);

			this.InitializeClientConnection();
		}

		public virtual void InitializeClientConnection()
		{
			this.CreateSslClientStream();
			this.fSslStream.AuthenticateAsClient(this.fFactory.TargetHostName, new X509CertificateCollection(), this.GetNetSecurityProtocol(), false);
		}

		protected virtual IAsyncResult BeginInitializeClientConnection(AsyncCallback callback, Object state)
		{
			this.CreateSslClientStream();
			return this.fSslStream.BeginAuthenticateAsClient(this.fFactory.TargetHostName, new X509CertificateCollection(), this.GetNetSecurityProtocol(), false, callback, state);
		}

		protected void EndInitializeClientConnection(IAsyncResult ar)
		{
			this.fSslStream.EndAuthenticateAsClient(ar);
		}

		public override void Connect(System.Net.IPAddress address, Int32 port)
		{
			this.Connect(new System.Net.IPEndPoint(address, port));
		}

		public override IAsyncResult BeginConnect(System.Net.EndPoint endPoint, AsyncCallback callback, Object state)
		{
			ConnectAsyncResult lWrapper = new ConnectAsyncResult(callback, state);
			this.fInnerConnection.BeginConnect(endPoint, lWrapper.ConnectionConnect, this);

			return lWrapper;
		}

		public override IAsyncResult BeginConnect(System.Net.IPAddress address, Int32 port, AsyncCallback callback, Object state)
		{
			return this.BeginConnect(new System.Net.IPEndPoint(address, port), callback, state);
		}

		public override void EndConnect(IAsyncResult ar)
		{
			((ConnectAsyncResult)ar).EndConnect();
		}

		protected override void DataSocketClose()
		{
			if (this.fSslStream != null)
			{
				this.fSslStream.Close();
				this.fSslStream.Dispose();
			}
			this.fInnerConnection.Close();
		}

		protected override void DataSocketClose(Boolean dispose)
		{
			this.DataSocketClose();
		}

		private void InnerConnection_AsyncHaveIncompleteData(Object sender, EventArgs e)
		{
			this.TriggerAsyncHaveIncompleteData();
		}

		private void InnerConnection_AsyncDisconnect(Object sender, EventArgs e)
		{
			this.TriggerAsyncDisconnect();
		}

		#region Read data
		protected override Int32 DataSocketReceiveWhatsAvaiable(Byte[] buffer, Int32 offset, Int32 size)
		{
			return (this.fFactory.UseMono && this.TimeoutEnabled)
				? this.MonoDataSocketReceiveWhatsAvaiable(buffer, offset, size)
				: this.NativeDataSocketReceiveWhatsAvaiable(buffer, offset, size);
		}

		private Int32 NativeDataSocketReceiveWhatsAvaiable(Byte[] buffer, Int32 offset, Int32 size)
		{
			this.StartTimeoutTimer();
			try
			{
				Int32 lBytesRead = this.fSslStream.Read(buffer, offset, size);

				// Say 'Hi!' to Mono bugs
				if (lBytesRead < 0)
				{
					throw new SocketException();
				}

				return lBytesRead;
			}
			catch (IOException)
			{
				throw new SocketException();
			}
			finally
			{
				this.StopTimeoutTimer();
			}
		}

		private Int32 MonoDataSocketReceiveWhatsAvaiable(Byte[] buffer, Int32 offset, Int32 size)
		{
			this.StartTimeoutTimer();
			try
			{
				IAsyncResult lAsyncResult = this.BeginMonoSslRead(buffer, offset, size);
				Int32 lBytesRead;

				// Manual timeout management
				Int32 lTimeout = this.Timeout * 1000;

				while (true)
				{
					Int32 lCurrentTimeout = Math.Min(MONO_TIMEOUT_POLL_PERIOD, lTimeout);
					lTimeout -= lCurrentTimeout;

					if (lAsyncResult.AsyncWaitHandle.WaitOne(lCurrentTimeout))
					{
						lBytesRead = this.EndMonoSslRead(lAsyncResult);
						break;
					}

					if (!this.Connected)
					{
						throw new SocketException();
					}

					if (lTimeout == 0)
					{
						// We got a timeout here
						this.DataSocketClose();
						throw new SocketException();
					}
				}

				// Say 'Hi!' to Mono bugs
				if (lBytesRead < 0)
				{
					throw new SocketException();
				}

				return lBytesRead;
			}
			catch (IOException)
			{
				throw new SocketException();
			}
			finally
			{
				this.StopTimeoutTimer();
			}
		}

		private IAsyncResult BeginMonoSslRead(Byte[] buffer, Int32 offset, Int32 size)
		{
			return this.fSslStream.BeginRead(buffer, offset, size, null, null);
		}

		private Int32 EndMonoSslRead(IAsyncResult ar)
		{
			return this.fSslStream.EndRead(ar);
		}
		#endregion

		#region Write data
		protected override Int32 DataSocketSendAsMuchAsPossible(Byte[] buffer, Int32 offset, Int32 size)
		{
			if (this.fFactory.UseMono && this.TimeoutEnabled)
			{
				this.MonoDataSocketSendAsMuchAsPossible(buffer, offset, size);
			}
			else
			{
				this.NativeDataSocketSendAsMuchAsPossible(buffer, offset, size);
			}

			return size;
		}

		private void NativeDataSocketSendAsMuchAsPossible(Byte[] buffer, Int32 offset, Int32 size)
		{
			this.StartTimeoutTimer();
			try
			{
				this.fSslStream.Write(buffer, offset, size);
			}
			catch (IOException)
			{
				throw new SocketException();
			}
			finally
			{
				this.StopTimeoutTimer();
			}
		}

		private void MonoDataSocketSendAsMuchAsPossible(Byte[] buffer, Int32 offset, Int32 size)
		{
			try
			{
				IAsyncResult lAsyncResult = this.BeginMonoSslWrite(buffer, offset, size);

				// Manual timeout management
				Int32 lTimeout = this.Timeout * 1000;

				while (true)
				{
					Int32 lCurrentTimeout = Math.Min(MONO_TIMEOUT_POLL_PERIOD, lTimeout);
					lTimeout -= lCurrentTimeout;

					if (lAsyncResult.AsyncWaitHandle.WaitOne(lCurrentTimeout))
					{
						this.EndMonoSslWrite(lAsyncResult);
						break;
					}

					if (!this.Connected)
					{
						throw new SocketException();
					}

					if (lTimeout == 0)
					{
						// We got a timeout here
						this.DataSocketClose();
						throw new SocketException();
					}
				}
			}
			catch (IOException)
			{
				throw new SocketException();
			}
			finally
			{
				this.StopTimeoutTimer();
			}
		}

		private IAsyncResult BeginMonoSslWrite(Byte[] buffer, Int32 offset, Int32 size)
		{
			return this.fSslStream.BeginWrite(buffer, offset, size, null, null);
		}

		private void EndMonoSslWrite(IAsyncResult ar)
		{
			this.fSslStream.EndWrite(ar);
		}
		#endregion

		protected override IAsyncResult IntBeginRead(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{
			try
			{
				return this.fSslStream.BeginRead(buffer, offset, count, callback, state);
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
				return this.fSslStream.EndRead(ar);
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
				return this.fSslStream.BeginWrite(buffer, offset, count, callback, state);
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
				this.fSslStream.EndWrite(ar);
			}
			catch (IOException)
			{
				// SocketException is expected in all code that deals with Connection, not IOException
				throw new SocketException();
			}
		}
	}
}