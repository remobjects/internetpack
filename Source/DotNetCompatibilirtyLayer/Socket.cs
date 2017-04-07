namespace RemObjects.InternetPack
{
	#if !ECHOES
	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public class OldSocket : Object, IDisposable
	{

		/*private Boolean islistening;
		private Boolean useoverlappedIO;
		private static const Int32 SOCKET_CLOSED = 10004;
		private static String timeout_exc_msg;
		private Queue readQ;
		private Queue writeQ;
		private static Int32 ipv4Supported;
		private static Int32 ipv6Supported;
		private Int32 linger_timeout;
		private IntPtr socket;
		private AddressFamily address_family;
		private SocketType socket_type;
		private ProtocolType protocol_type;
		public Boolean Blocking { get; set; }
		internal Boolean blocking;
		private List<Thread> blocking_threads;
		public Boolean IsBound { get; set; }
		private Boolean isbound;*/
		public Boolean Connected { get; set; }
		/*private Boolean connected;
		private Boolean closed;
		internal Boolean disposed;
		private Boolean connect_in_progress;
		internal EndPoint seed_endpoint;*/
		public OldSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType) {}
		/*private static this .cctor();
		public Socket(SocketInformation socketInformation);
		internal Socket(AddressFamily family, SocketType type, ProtocolType proto, IntPtr sock);
		private static void AddSockets(List<Socket> sockets, IList list, String name);
		private static void Select_internal(ref Socket[] sockets, Int32 microSeconds, out Int32 error);
		public static void Select(IList checkRead, IList checkWrite, IList checkError, Int32 microSeconds);
		private void SocketDefaults();
		private static Int32 Available_internal(IntPtr socket, out Int32 error);
		private static SocketAddress LocalEndPoint_internal(IntPtr socket, Int32 family, out Int32 error);
		public Boolean AcceptAsync(SocketAsyncEventArgs e);
		private static IntPtr Accept_internal(IntPtr sock, out Int32 error, Boolean blocking);
		internal void Accept(Socket acceptSocket);*/
		public Socket Accept() {}
		public IAsyncResult BeginAccept(Socket acceptSocket, Int32 receiveSize, AsyncCallback callback, Object state) {}
		public IAsyncResult BeginAccept(Int32 receiveSize, AsyncCallback callback, Object state) {}
		public IAsyncResult BeginAccept(AsyncCallback callback, Object state) {}
		/*public IAsyncResult BeginConnect(IPAddress[] addresses, Int32 port, AsyncCallback callback, Object state);
		public IAsyncResult BeginConnect(EndPoint end_point, AsyncCallback callback, Object state);
		public IAsyncResult BeginConnect(String host, Int32 port, AsyncCallback callback, Object state);
		public IAsyncResult BeginConnect(IPAddress address, Int32 port, AsyncCallback callback, Object state);
		public IAsyncResult BeginDisconnect(Boolean reuseSocket, AsyncCallback callback, Object state);
		private void CheckRange(Byte[] buffer, Int32 offset, Int32 size);
		[CLSCompliantAttribute(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, Object state);
		[CLSCompliantAttribute(false)]
		public IAsyncResult BeginReceive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, Object state);
		public IAsyncResult BeginReceive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags, out SocketError error, AsyncCallback callback, Object state);
		public IAsyncResult BeginReceive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socket_flags, AsyncCallback callback, Object state);
		public IAsyncResult BeginReceiveFrom(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socket_flags, ref EndPoint remote_end, AsyncCallback callback, Object state);
		public IAsyncResult BeginReceiveMessageFrom(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, ref EndPoint remoteEP, AsyncCallback callback, Object state);
		[CLSCompliantAttribute(false)]
		public IAsyncResult BeginSend(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, Object state);
		public IAsyncResult BeginSend(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, AsyncCallback callback, Object state);
		public IAsyncResult BeginSend(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, Object state);
		public IAsyncResult BeginSend(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socket_flags, AsyncCallback callback, Object state);
		public IAsyncResult BeginSendFile(String fileName, Byte[] preBuffer, Byte[] postBuffer, TransmitFileOptions flags, AsyncCallback callback, Object state);
		public IAsyncResult BeginSendFile(String fileName, AsyncCallback callback, Object state);
		public IAsyncResult BeginSendTo(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socket_flags, EndPoint remote_end, AsyncCallback callback, Object state);
		private static void Bind_internal(IntPtr sock, SocketAddress sa, out Int32 error);*/
		public void Bind(EndPoint local_end) {}
		public void Connect(EndPoint remoteEP) {}
		public void Connect(String host, Int32 port) {}
		public void Connect(IPAddress[] addresses, Int32 port) {}
		public void Connect(IPAddress address, Int32 port) {}
		/*public Boolean DisconnectAsync(SocketAsyncEventArgs e);
		private static void Disconnect_internal(IntPtr sock, Boolean reuse, out Int32 error);*/
		public void Disconnect(Boolean reuseSocket) {}
		/*public SocketInformation DuplicateAndClose(Int32 targetProcessId);
		public Socket EndAccept(out Byte[] buffer, out Int32 bytesTransferred, IAsyncResult asyncResult);
		public Socket EndAccept(out Byte[] buffer, IAsyncResult asyncResult);*/
		public Socket EndAccept(IAsyncResult result) {}
		/*public void EndConnect(IAsyncResult result);
		public void EndDisconnect(IAsyncResult asyncResult);
		public Int32 EndReceiveMessageFrom(IAsyncResult asyncResult, ref SocketFlags socketFlags, ref EndPoint endPoint, out IPPacketInformation ipPacketInformation);
		public void EndSendFile(IAsyncResult asyncResult);
		public Int32 EndSendTo(IAsyncResult result);
		private static void GetSocketOption_arr_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, ref Byte[] byte_val, out Int32 error);
		public Object GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName);
		public Byte[] GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 length);
		public void GetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue);
		private static Int32 WSAIoctl(IntPtr sock, Int32 ioctl_code, Byte[] input, Byte[] output, out Int32 error);
		public Int32 IOControl(IOControlCode ioControlCode, Byte[] optionInValue, Byte[] optionOutValue);
		public Int32 IOControl(Int32 ioctl_code, Byte[] in_value, Byte[] out_value);
		private static void Listen_internal(IntPtr sock, Int32 backlog, out Int32 error);*/
		public void Listen(Int32 backlog) {}
		/*public Boolean Poll(Int32 time_us, SelectMode mode);
		[CLSCompliantAttribute(false)]
		public Int32 Receive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode);
		[CLSCompliantAttribute(false)]
		public Int32 Receive(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags);
		public Int32 Receive(IList<ArraySegment<Byte>> buffers);
		public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags, out SocketError error);*/
		public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags) {}
		public Int32 Receive(Byte[] buffer, Int32 size, SocketFlags flags) {}
		public Int32 Receive(Byte[] buffer, SocketFlags flags) {}
		public Int32 Receive(Byte[] buffer) {}
		/*public Boolean ReceiveFromAsync(SocketAsyncEventArgs e);
		public Int32 ReceiveFrom(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags, ref EndPoint remoteEP);
		public Int32 ReceiveFrom(Byte[] buffer, Int32 size, SocketFlags flags, ref EndPoint remoteEP);
		public Int32 ReceiveFrom(Byte[] buffer, SocketFlags flags, ref EndPoint remoteEP);
		public Int32 ReceiveFrom(Byte[] buffer, ref EndPoint remoteEP);
		private static Int32 RecvFrom_internal(IntPtr sock, Byte[] buffer, Int32 offset, Int32 count, SocketFlags flags, ref SocketAddress sockaddr, out Int32 error);
		internal Int32 ReceiveFrom_nochecks(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags, ref EndPoint remote_end);
		internal Int32 ReceiveFrom_nochecks_exc(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags, ref EndPoint remote_end, Boolean throwOnError, out Int32 error);
		public Boolean ReceiveMessageFromAsync(SocketAsyncEventArgs e);
		public Int32 ReceiveMessageFrom(Byte[] buffer, Int32 offset, Int32 size, ref SocketFlags socketFlags, ref EndPoint remoteEP, out IPPacketInformation ipPacketInformation);
		public Boolean SendPacketsAsync(SocketAsyncEventArgs e);
		[CLSCompliantAttribute(false)]
		public Int32 Send(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags, out SocketError errorCode);
		public Int32 Send(IList<ArraySegment<Byte>> buffers, SocketFlags socketFlags);
		public Int32 Send(IList<ArraySegment<Byte>> buffers);
		public Int32 Send(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags, out SocketError error);*/
		public Int32 Send(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags) {}
		public Int32 Send(Byte[] buf, Int32 size, SocketFlags flags) {}
		public Int32 Send(Byte[] buf, SocketFlags flags) {}
		public Int32 Send(Byte[] buf) {}
		/*public void SendFile(String fileName, Byte[] preBuffer, Byte[] postBuffer, TransmitFileOptions flags);
		public void SendFile(String fileName);
		private static Boolean SendFile(IntPtr sock, String filename, Byte[] pre_buffer, Byte[] post_buffer, TransmitFileOptions flags);
		public Boolean SendToAsync(SocketAsyncEventArgs e);
		public Int32 SendTo(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags, EndPoint remote_end);
		public Int32 SendTo(Byte[] buffer, Int32 size, SocketFlags flags, EndPoint remote_end);
		public Int32 SendTo(Byte[] buffer, SocketFlags flags, EndPoint remote_end);
		public Int32 SendTo(Byte[] buffer, EndPoint remote_end);
		private static Int32 SendTo_internal(IntPtr sock, Byte[] buffer, Int32 offset, Int32 count, SocketFlags flags, SocketAddress sa, out Int32 error);
		internal Int32 SendTo_nochecks(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags, EndPoint remote_end);*/
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionValue) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Boolean optionValue) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Object optionValue) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue) {}
		/*internal static void CheckProtocolSupport();
		private void RegisterForBlockingSyscall();
		private void UnRegisterForBlockingSyscall();
		private void AbortRegisteredThreads();
		private IntPtr Socket_internal(AddressFamily family, SocketType type, ProtocolType proto, out Int32 error);
		protected override void Finalize();
		private static void Blocking_internal(IntPtr socket, Boolean block, out Int32 error);
		private static SocketAddress RemoteEndPoint_internal(IntPtr socket, Int32 family, out Int32 error);
		private void Linger(IntPtr handle);
		private static void cancel_blocking_socket_operation(Thread thread);
		protected virtual void Dispose(Boolean disposing);*/
		private new void /*System.IDisposable.*/Dispose() {}
		/*private static void Close_internal(IntPtr socket, out Int32 error);
		public void Close(Int32 timeout);*/
		public void Close() {}
		/*private static void Connect_internal(IntPtr sock, SocketAddress sa, out Int32 error);
		public Boolean ReceiveAsync(SocketAsyncEventArgs e);
		public Boolean SendAsync(SocketAsyncEventArgs e);
		private static Boolean Poll_internal(IntPtr socket, SelectMode mode, Int32 timeout, out Int32 error);
		private static Int32 Receive_internal(IntPtr sock, Socket.WSABUF[] bufarray, SocketFlags flags, out Int32 error);
		private static Int32 Receive_internal(IntPtr sock, Byte[] buffer, Int32 offset, Int32 count, SocketFlags flags, out Int32 error);
		internal Int32 Receive_nochecks(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags, out SocketError error);
		private static void GetSocketOption_obj_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, out Object obj_val, out Int32 error);
		private static Int32 Send_internal(IntPtr sock, Socket.WSABUF[] bufarray, SocketFlags flags, out Int32 error);
		private static Int32 Send_internal(IntPtr sock, Byte[] buf, Int32 offset, Int32 count, SocketFlags flags, out Int32 error);
		internal Int32 Send_nochecks(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags, out SocketError error);
		private static void Shutdown_internal(IntPtr socket, SocketShutdown how, out Int32 error);*/
		public void Shutdown(SocketShutdown how) {}
		/*private static void SetSocketOption_internal(IntPtr socket, SocketOptionLevel level, SocketOptionName name, Object obj_val, Byte[] byte_val, Int32 int_val, out Int32 error);
		private void ThrowIfUpd();
		private IAsyncResult BeginMConnect(Socket.SocketAsyncResult req);
		private Boolean GetCheckedIPs(SocketAsyncEventArgs e, out IPAddress[] addresses);
		private Boolean ConnectAsyncReal(SocketAsyncEventArgs e);
		public Boolean ConnectAsync(SocketAsyncEventArgs e);
		private Exception InvalidAsyncOp(String method);
		public Int32 EndReceive(IAsyncResult asyncResult, out SocketError errorCode);
		public Int32 EndReceive(IAsyncResult result);
		public Int32 EndSend(IAsyncResult asyncResult, out SocketError errorCode);
		public Int32 EndSend(IAsyncResult result);
		public Int32 EndReceiveFrom(IAsyncResult result, ref EndPoint end_point);
		private static void socket_pool_queue(Socket.SocketAsyncCall d, Socket.SocketAsyncResult r);*/
		public Int32 Available { get; set; }
		/*public Boolean DontFragment { get; set; }
		public Boolean EnableBroadcast { get; set; }
		public Boolean ExclusiveAddressUse { get; set; }
		public LingerOption LingerState { get; set; }
		public Boolean MulticastLoopback { get; set; }
		public Boolean UseOnlyOverlappedIO { get; set; }
		public IntPtr Handle { get; set; }*/
		public EndPoint LocalEndPoint { get; set; }
		public SocketType SocketType { get; set; }
		/*public Int32 SendTimeout { get; set; }
		public Int32 ReceiveTimeout { get; set; }
		public static Boolean SupportsIPv4 { get; set; }
		[ObsoleteAttribute("Use OSSupportsIPv6 instead")]
		public static Boolean SupportsIPv6 { get; set; }
		public static Boolean OSSupportsIPv6 { get; set; }*/
		public AddressFamily AddressFamily { get; set; }
		public ProtocolType ProtocolType { get; set; }
		/*public Boolean NoDelay { get; set; }
		public Int32 ReceiveBufferSize { get; set; }
		public Int32 SendBufferSize { get; set; }
		public Int16 Ttl { get; set; }*/
		public EndPoint RemoteEndPoint { get; set; }
	}

	public class Socket : Object, IDisposable
	{
		private rtl.SOCKET fHandle;
                    
        public Boolean Connected { get; set; }
		public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)        
        {
            AddressFamily = addressFamily;
            SocketType = socketType;
            ProtocolType = protocolType;
            fHandle = rtl.__Global.socket((rtl.Int)addressFamily, (rtl.Int)socketType, (rtl.Int)protocolType);
            // TODO check exception
        }

        private Socket(rtl.SOCKET handle)
        {
            fHandle = handle;
        }

        static Socket()
        {
            rtl.WSADATA data;
            rtl.WSAStartup(rtl.WINSOCK_VERSION, &data);
        }
                
        public Socket Accept()
        {            
            var lSocket = rtl.accept(fHandle, null, null);
            if (lSocket == rtl.INVALID_SOCKET)
                ; // raise error

            return new Socket(lSocket);
        }

		//public IAsyncResult BeginAccept(Socket acceptSocket, Int32 receiveSize, AsyncCallback callback, Object state) {}
		//public IAsyncResult BeginAccept(Int32 receiveSize, AsyncCallback callback, Object state) {}
		public IAsyncResult BeginAccept(AsyncCallback callback, Object state) {}
		        
        public void Bind(EndPoint local_end)       
        {
            var lEndPoint = (IPEndPoint)local_end;
            void *lPointer;
            int lSize;
            rtl.SOCKADDR_IN lIPv4;
            sockaddr_in6 lIPv6;

            switch (lEndPoint.AddressFamily)
            {
                case AddressFamily.InterNetworkV6:
                    lIPv6.sin6_family = AddressFamily.InterNetworkV6;
                    lIPv6.sin6_port = rtl.htons(lEndPoint.Port);
                    lIPv6.sin6_scope_id = lEndPoint.Address.ScopeId;
                    var lBytes = lEndPoint.Address.GetAddressBytes();
                    for (int i = 0; i < 16; i++)
                        lIPv6.sin6_addr.u.Byte[i] = lBytes[i];
                    lPointer = &lIPv6;
                    lSize = sizeof(sockaddr_in6);
                    break;

                default:
                    lIPv4.sin_family = AddressFamily.InterNetwork;
                    lIPv4.sin_port = rtl.htons(lEndPoint.Port);
                    lIPv4.sin_addr.S_un.S_addr = lEndPoint.Address.Address;
                    lPointer = &lIPv4;
                    lSize = sizeof(rtl.SOCKADDR_IN);
            }            
            if (rtl.bind(fHandle, lPointer, lSize) != 0)
                ; // throw exception
        }

		public void Connect(EndPoint remoteEP)
        {

        }

		public void Connect(String host, Int32 port)        
        {
            
        }

		public void Connect(IPAddress[] addresses, Int32 port)
        {
            IPEndPoint lEndPoint;
            Exception lEx = null;
            foreach(IPAddress lAddress in addresses)
            {
                lEndPoint = new IPEndPoint(lAddress, port);
                try
                {
                    Connect(lEndPoint);
                    lEx = null;
                    break;
                }
                catch(Exception lCurrent)
                {
                    lEx = lCurrent;
                }
            }

            if (lEx != null)
                throw lEx;

            if (!Connected)
                ; // throw exception
        }

		public void Connect(IPAddress address, Int32 port)
        {
            var lEndPoint = new IPEndPoint(address, port);
            Connect(lEndPoint);
        }
		
        public void Disconnect(Boolean reuseSocket)
        {
            Dispose();
        }

		public Socket EndAccept(IAsyncResult result) {}
		
        public void Listen(Int32 backlog)
        {
            if (rtl.listen(fHandle, backlog) != 0)
                ; // TODO check exception
        }

		public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags) {}
		public Int32 Receive(Byte[] buffer, Int32 size, SocketFlags flags) {}
		public Int32 Receive(Byte[] buffer, SocketFlags flags) {}
		public Int32 Receive(Byte[] buffer) {}
		public Int32 Send(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags) {}
		public Int32 Send(Byte[] buf, Int32 size, SocketFlags flags) {}
		public Int32 Send(Byte[] buf, SocketFlags flags) {}
		public Int32 Send(Byte[] buf) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionValue) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Boolean optionValue) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Object optionValue) {}
		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue) {}
		
        private new void Dispose() 
        {
            if (rtl.closesocket(fHandle) != 0)
                ; // raise exception
            
            Connected = false;
        }
		
        public void Close() 
        {
            Dispose();
        }
		
        public void Shutdown(SocketShutdown how) 
        {
            if (rtl.shutdown(fHandle, (int)how) != 0)
                ; // throw exception
        }

		public Int32 Available
        {
            get
            {
                rtl.u_long lData;
                if (rtl.ioctlsocket(fHandle,rtl.FIONREAD, &lData) != 0)
                    ; // throw exception

                return lData;
            }
        }

		public EndPoint LocalEndPoint { get; set; }
		public SocketType SocketType { get; set; }
		public AddressFamily AddressFamily { get; set; }
		public ProtocolType ProtocolType { get; set; }
		public EndPoint RemoteEndPoint { get; set; }
	}


	// Generated from /Users/mh/Xcode/DerivedData/Fire-beiaefoboptwvtbxtvecylpnprxy/Build/Products/Debug/Fire.app/Contents/Resources/Mono/lib/mono/2.0/System.dll
	public class SocketException : Exception
	{
		//public SocketException(Int32 error);
		//public SocketException();
		public /*override*/ Int32 ErrorCode { get; set; }
		//public SocketError SocketErrorCode { get; set; }
		//public override String Message { get; set; }
	}
	#endif
}