/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace RemObjects.InternetPack
{
    public class SslValidateCertificateArgs : EventArgs
    {
        public SslValidateCertificateArgs(X509Certificate certificate)
        {
            this.fCertificate = certificate;
            this.Cancel = false;
        }

        public X509Certificate Certificate
        {
            get
            {
                return this.fCertificate;
            }
        }
        private X509Certificate fCertificate;

        public Boolean Cancel
        {
            get
            {
                return this.fCancel;
            }
            set
            {
                this.fCancel = value;
            }
        }
        private Boolean fCancel;
    }

    public class SslNeedPasswordArgs : EventArgs
    {
        public SecureString Password
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
        private SecureString fPassword;

        public String PasswordString
        {
            get
            {
                return null;
            }
            set
            {
                if (value == null)
                {
                    this.fPassword = null;
                    return;
                }

                this.fPassword = new SecureString();
                for (Int32 i = 0; i < value.Length; i++)
                    this.fPassword.AppendChar(value[i]);
            }
        }
    }

    public class SslConnectionFactory : IConnectionFactory
    {
        [DefaultValue(""), Category("Ssl Options")]
        public String TargetHostName
        {
            get
            {
                return this.fTargetHostName;
            }
            set
            {
                this.fTargetHostName = value;
            }
        }
        private String fTargetHostName;

        [DefaultValue(false), Category("Ssl Options")]
        public Boolean UseMono
        {
            get
            {
                return this.fUseMono;
            }
            set
            {
                this.fUseMono = value;
            }
        }
        private Boolean fUseMono;

        [DefaultValue(false), Category("Ssl Options")]
        public Boolean Enabled
        {
            get
            {
                return this.fEnabled;
            }
            set
            {
                fEnabled = value;
            }
        }
        private Boolean fEnabled;

        [DefaultValue(null), Category("Ssl Options")]
        public String CertificateFileName
        {
            get
            {
                return this.fCertificateFileName;
            }
            set
            {
                this.fCertificateFileName = value;
            }
        }
        private String fCertificateFileName;

        [Category("Ssl Options")]
        public event EventHandler<SslNeedPasswordArgs> NeedPassword;

        protected virtual void OnNeedPassword(SslNeedPasswordArgs e)
        {
            if (this.NeedPassword != null)
                this.NeedPassword(this, e);
        }

        [Browsable(false)]
        public X509Certificate2 Certificate
        {
            get
            {
                return this.fCertificate;
            }
            set
            {
                this.fCertificate = value;
            }
        }
        private X509Certificate2 fCertificate;

        [Browsable(false)]
        public Boolean HasCertificate
        {
            get
            {
                return this.fCertificate != null || !String.IsNullOrEmpty(this.fCertificateFileName);
            }
        }

        protected void LoadCertificate()
        {
            lock (this)
            {
                if (this.Certificate != null)
                    return;

                if (String.IsNullOrEmpty(this.CertificateFileName))
                    throw new Exception("Certificate not set and CertificateFileName is empty");

                SslNeedPasswordArgs lArgs = new SslNeedPasswordArgs();
                this.OnNeedPassword(lArgs);

                this.Certificate = new X509Certificate2(this.CertificateFileName, lArgs.Password, X509KeyStorageFlags.Exportable);
            }
        }

        private Assembly fMonoAssembly;

        public Assembly GetMonoAssembly()
        {
            if (!this.UseMono)
                return null;

            if (this.fMonoAssembly != null)
                return this.fMonoAssembly;

            try
            {
                this.fMonoAssembly = Assembly.Load("Mono.Security, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
            }
            catch (Exception)
            {
                this.fUseMono = false;
            }

            return this.fMonoAssembly;
        }

        [Category("Ssl Options")]
        public event EventHandler<SslValidateCertificateArgs> ValidateRemoteCertificate;

        protected virtual void OnValidateRemoteCertificate(SslValidateCertificateArgs e)
        {
            if (this.ValidateRemoteCertificate != null)
                this.ValidateRemoteCertificate(this, e);
        }

        public Boolean OnValidateRemoteCertificate(X509Certificate certificate)
        {
            SslValidateCertificateArgs lEventArgs = new SslValidateCertificateArgs(certificate);
            this.OnValidateRemoteCertificate(lEventArgs);

            return (!lEventArgs.Cancel);
        }

        #region IConnectionFactory Members
        public virtual Connection CreateServerConnection(Socket socket)
        {
            if (!this.Enabled)
                return new Connection(socket);

            if (this.Certificate == null)
                this.LoadCertificate();

            return new SslConnection(this, socket);
        }

        public virtual Connection CreateClientConnection(Binding binding)
        {
            if (!this.Enabled)
                return new Connection(binding);

            if (this.HasCertificate)
                this.LoadCertificate();

            return new SslConnection(this, binding);
        }

        public virtual Connection CreateClientConnection(Connection connection)
        {
            if (this.HasCertificate)
                this.LoadCertificate();

            return new SslConnection(this, connection);
        }
        #endregion
    }

    public class SslConnection : Connection
    {
        protected class InnerConnection : Connection
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

        private SslConnectionFactory fFactory;
        private Connection fInnerConnection;
        private Stream fSsl;

        public SslConnection(SslConnectionFactory factory, Binding binding)
            : base((Socket)null)
        {
            fFactory = factory;
            fInnerConnection = new InnerConnection(binding);
            fInnerConnection.BufferedAsync = false;
            fInnerConnection.AsyncDisconnect += new EventHandler(InnerConnection_AsyncDisconnect);
            fInnerConnection.AsyncHaveIncompleteData += new EventHandler(InnerConnection_AsyncHaveIncompleteData);
        }

        public SslConnection(SslConnectionFactory factory, Socket socket)
            : base((Socket)null)
        {
            fFactory = factory;
            fInnerConnection = new InnerConnection(socket);
            fInnerConnection.BufferedAsync = false;
            fInnerConnection.AsyncDisconnect += new EventHandler(InnerConnection_AsyncDisconnect);
            fInnerConnection.AsyncHaveIncompleteData += new EventHandler(InnerConnection_AsyncHaveIncompleteData);
        }

        public SslConnection(SslConnectionFactory factory, Connection connection)
            : base((Socket)null)
        {
            fFactory = factory;
            fInnerConnection = connection;
            fInnerConnection.BufferedAsync = false;
            fInnerConnection.AsyncDisconnect += new EventHandler(InnerConnection_AsyncDisconnect);
            fInnerConnection.AsyncHaveIncompleteData += new EventHandler(InnerConnection_AsyncHaveIncompleteData);
        }

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

        private Boolean Ssl_RemoteCertificateValidation(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return this.fFactory.OnValidateRemoteCertificate(certificate);
        }

        private AsymmetricAlgorithm MonoSsl_GetPrivateKey(X509Certificate certificate, String targetHost)
        {
            return this.fFactory.Certificate.PrivateKey;
        }

        private Boolean MonoSsl_RemoteCertificateValidation(X509Certificate certificate, Int32[] errors)
        {
            return fFactory.OnValidateRemoteCertificate(certificate);
        }

        private void CreateMonoServerStream()
        {
            Type lType = this.fFactory.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.SslServerStream", true);
            this.fSsl = (Stream)Activator.CreateInstance(lType, fInnerConnection, this.fFactory.Certificate, false, false);

            Object o = Delegate.CreateDelegate(fFactory.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.PrivateKeySelectionCallback", true), this, "MonoSsl_GetPrivateKey", false, true);
            lType.InvokeMember("set_PrivateKeyCertSelectionDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { o });

            o = Delegate.CreateDelegate(fFactory.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.CertificateValidationCallback", true), this, "MonoSsl_RemoteCertificateValidation", false, true);
            lType.InvokeMember("set_ClientCertValidationDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { o });
        }

        protected internal override void InitializeServerConnection()
        {
            if (this.fFactory.GetMonoAssembly() != null)
            {
                this.CreateMonoServerStream();
            }
            else
            {
                this.fSsl = new SslStream(this.fInnerConnection, true, new RemoteCertificateValidationCallback(Ssl_RemoteCertificateValidation));
                ((SslStream)this.fSsl).AuthenticateAsServer(this.fFactory.Certificate);
            }
        }

        internal protected override IAsyncResult BeginInitializeServerConnection(AsyncCallback callback, Object state)
        {
            if (this.fFactory.UseMono)
            {
                this.CreateMonoServerStream();
                return null;
            }

            this.fSsl = new SslStream(this.fInnerConnection, true, new RemoteCertificateValidationCallback(Ssl_RemoteCertificateValidation));
            return ((SslStream)this.fSsl).BeginAuthenticateAsServer(this.fFactory.Certificate, callback, state);
        }

        protected internal override void EndInitializeServerConnection(IAsyncResult ar)
        {
            if (!this.fFactory.UseMono)
                ((SslStream)this.fSsl).EndAuthenticateAsServer(ar);
        }

        private void CreateMonoClientStream()
        {
            Type lType = fFactory.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.SslClientStream", true);
            this.fSsl = (Stream)Activator.CreateInstance(lType, this.fInnerConnection, this.fFactory.TargetHostName, false);

            Object o = Delegate.CreateDelegate(fFactory.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.PrivateKeySelectionCallback", true), this, "MonoSsl_GetPrivateKey", false, true);
            lType.InvokeMember("set_PrivateKeyCertSelectionDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { o });

            o = Delegate.CreateDelegate(fFactory.GetMonoAssembly().GetType("Mono.Security.Protocol.Tls.CertificateValidationCallback", true), this, "MonoSsl_RemoteCertificateValidation", false, true);
            lType.InvokeMember("set_ServerCertValidationDelegate", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, this.fSsl, new Object[] { o });
        }

        public override void Connect(System.Net.EndPoint endPoint)
        {
            this.fInnerConnection.Connect(endPoint);

            this.InitializeClientConnection();
        }

        public virtual void InitializeClientConnection()
        {
            if (this.fFactory.GetMonoAssembly() != null)
            {
                this.CreateMonoClientStream();
            }
            else
            {
                this.fSsl = new SslStream(this.fInnerConnection, true, new RemoteCertificateValidationCallback(this.Ssl_RemoteCertificateValidation));
                ((SslStream)this.fSsl).AuthenticateAsClient(this.fFactory.TargetHostName);
            }
        }

        public virtual IAsyncResult BeginInitializeClientConnection(AsyncCallback callback, Object state)
        {
            if (this.fFactory.GetMonoAssembly() != null)
            {
                this.CreateMonoClientStream();
                return null;
            }

            this.fSsl = new SslStream(this.fInnerConnection, true, new RemoteCertificateValidationCallback(Ssl_RemoteCertificateValidation));
            return ((SslStream)this.fSsl).BeginAuthenticateAsClient(this.fFactory.TargetHostName, callback, state);
        }

        public void EndInitializeClientConnection(IAsyncResult ar)
        {
            if (this.fFactory.GetMonoAssembly() == null)
                ((SslStream)this.fSsl).EndAuthenticateAsClient(ar);
        }

        public override void Connect(System.Net.IPAddress address, Int32 port)
        {
            this.Connect(new System.Net.IPEndPoint(address, port));
        }

        private class ConnectWrapper : IAsyncResult
        {
            public ConnectWrapper(AsyncCallback callback, Object state)
            {
                this.fAsyncState = state;
                this.fCallback = callback;
            }

            private AsyncCallback fCallback;
            private volatile Boolean fComplete;

            public Object AsyncState
            {
                get
                {
                    return this.fAsyncState;
                }
                set
                {
                    this.fAsyncState = value;
                }
            }
            private Object fAsyncState;

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get
                {
                    if (this.fWaitHandle != null)
                        return this.fWaitHandle;

                    lock (this)
                    {
                        if (this.fWaitHandle == null)
                            this.fWaitHandle = new System.Threading.ManualResetEvent(fComplete);
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

            public void Dispose()
            {
                if (this.fWaitHandle != null)
                    this.fWaitHandle.Close();

                this.fWaitHandle = null;
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
                            this.fWaitHandle.Set();
                    }
                    this.fCallback(ar);

                    return;
                }

                if (lOwner.fFactory.GetMonoAssembly() != null)
                {
                    lOwner.CreateMonoClientStream();
                    fComplete = true;
                    lock (this)
                    {
                        if (this.fWaitHandle != null)
                            this.fWaitHandle.Set();
                    }
                    this.fCallback(ar);

                    return;
                }

                lOwner.fSsl = new SslStream(lOwner.fInnerConnection, true, new RemoteCertificateValidationCallback(lOwner.Ssl_RemoteCertificateValidation));
                ((SslStream)lOwner.fSsl).BeginAuthenticateAsClient(lOwner.fFactory.TargetHostName, new AsyncCallback(SslAuthenticateAsClient), lOwner);
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
                            this.fWaitHandle.Set();
                    }
                    this.fCallback(ar);
                    return;
                }

                lOwner.CreateMonoClientStream();
                this.fComplete = true;
                lock (this)
                {
                    if (this.fWaitHandle != null)
                        this.fWaitHandle.Set();
                }
                this.fCallback(ar);
                return;
            }

            private Exception fFailure;
            public void EndConnect()
            {
                if (!fComplete)
                    AsyncWaitHandle.WaitOne();

                this.Dispose();
                if (this.fFailure != null)
                    throw this.fFailure;
            }
        }

        public override IAsyncResult BeginConnect(System.Net.EndPoint endPoint, AsyncCallback callback, Object state)
        {
            ConnectWrapper lWrapper = new ConnectWrapper(callback, state);
            this.fInnerConnection.BeginConnect(endPoint, new AsyncCallback(lWrapper.ConnectionConnect), this);

            return lWrapper;
        }

        public override IAsyncResult BeginConnect(System.Net.IPAddress address, Int32 port, AsyncCallback callback, Object state)
        {
            return this.BeginConnect(new System.Net.IPEndPoint(address, port), callback, state);
        }

        public override void EndConnect(IAsyncResult ar)
        {
            ((ConnectWrapper)ar).EndConnect();
        }

        protected override void DataSocketClose()
        {
            fSsl.Dispose();
            this.fInnerConnection.Close();
        }

        protected override void DataSocketClose(Boolean dispose)
        {
            fSsl.Dispose();
            this.fInnerConnection.Close(dispose);
        }

        void InnerConnection_AsyncHaveIncompleteData(Object sender, EventArgs e)
        {
            this.TriggerAsyncHaveIncompleteData();
        }

        void InnerConnection_AsyncDisconnect(Object sender, EventArgs e)
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