namespace RemObjects.InternetPack
{
	#if !ECHOES
	public class Socket : Object, IDisposable
	{
		#if cooper
		private java.net.Socket fHandle;
		private java.net.ServerSocket fServerHandle;
		private java.net.DatagramSocket fDgramHandle;
		private bool fIsServer = false;
		private java.io.InputStream fSocketInput;
		private java.io.OutputStream fSocketOutput;
		private java.net.InetSocketAddress fSocketAddress;
		#elif posix || toffee || darwin
		private int fHandle;
		public const Int32 FIONREAD = 1074004095;
		#else
		private rtl.SOCKET fHandle;
		#endif

		private List<Thread> fPendingAsyncOps;
		#if cooper || toffee
		private readonly Object fMonitor = new Object();
		#else
		private readonly Monitor fMonitor = new Monitor();
		#endif

		private void AddAsyncOp(Thread op)
		{
			lock(this.fMonitor)
			{
				if (fPendingAsyncOps == null)
					fPendingAsyncOps = new List<Thread>();

				fPendingAsyncOps.Add(op);
			}
		}

		private void ClearAsyncOps()
		{
			foreach(var lThread in fPendingAsyncOps)
			{
				if (lThread.IsAlive)
				{
					lThread.Abort();
				}
			}

			lock(fMonitor);
			{
				fPendingAsyncOps.RemoveAll();
			}
		}

		private void RemoveOp(Thread op)
		{
			lock(fMonitor)
			{
				fPendingAsyncOps.Remove(op);
			}
		}

		public Boolean Connected { get; set; }

		public Socket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocolType)
		{
			AddressFamily = addressFamily;
			SocketType = socketType;
			ProtocolType = protocolType;
			#if cooper
			if ((AddressFamily != AddressFamily.InterNetwork) && (AddressFamily != AddressFamily.InterNetworkV6))
				throw new Exception("Address family not supported on current platform");

			switch(SocketType)
			{
				case SocketType.Stream:
					fHandle = new java.net.Socket();
					break;

				case SocketType.Dgram:
					fDgramHandle = new java.net.DatagramSocket();
					break;

				default:
					throw new Exception("Socket type not supported on current platform");
			}
			#else
			#if posix || toffee || darwin
			fHandle = rtl.socket((rtl.int32_t)addressFamily, (rtl.int32_t)socketType, (rtl.int32_t)protocolType);
			#else
			fHandle = rtl.__Global.socket((rtl.INT)addressFamily, (rtl.INT)socketType, (rtl.INT)protocolType);
			#endif

			if (fHandle < 0)
				throw new Exception("Error creating socket");
			#endif
		}

		#if cooper
		private Socket(java.net.Socket handle)
		{
			fHandle = handle;
			fSocketInput = fHandle.getInputStream();
			fSocketOutput = fHandle.getOutputStream();
		}
		#elif posix || toffee || darwin
		private Socket(int handle)
		{
			fHandle = handle;
		}
		#else
		private Socket(rtl.SOCKET handle)
		{
			fHandle = handle;
		}
		#endif

		static Socket()
		{
			#if windows
			rtl.WSADATA data;
			rtl.WSAStartup(rtl.WINSOCK_VERSION, &data);
			#endif
		}

		public Socket Accept()
		{
			#if cooper
			var lSocket = fServerHandle.accept();
			#else
			#if posix
			rtl.__SOCKADDR_ARG lSockAddr;
			lSockAddr.__sockaddr__ = null;
			var lSocket = rtl.accept(fHandle, lSockAddr, null);
			if (lSocket == -1)
				throw new Exception("Error calling accept function");
			#else
			var lSocket = rtl.accept(fHandle, null, null);
			if (lSocket < 0)
				throw new Exception("Error calling accept function");
			#endif
			#endif

			var lNewSocket = new Socket(lSocket);
			lNewSocket.Connected = true;
			return lNewSocket;
		}

		public IAsyncResult BeginAccept(Socket acceptSocket, Int32 receiveSize, AsyncCallback callback, Object state)
		{
			var lResult = new AsyncResult(state);
			lResult.AcceptSocket = acceptSocket;
			lResult.NBytes = receiveSize;

			var lThread = new Thread(() =>
			{
				lResult.AsyncWaitHandle.WaitOne();
				try
				{
					if (lResult.AcceptSocket != null)
						lResult.AcceptedSocket = acceptSocket.Accept();
					else
						lResult.AcceptedSocket = Accept();

					if (receiveSize < 0)
						throw new Exception("Bad receiveSize value:" + receiveSize.ToString() + " in BeginAccept call");

					if (receiveSize > 0)
					{
						lResult.Buffer = new byte[receiveSize];
						lResult.NBytes = Receive(lResult.Buffer, receiveSize, SocketFlags.None);
					}
				}
				catch(Exception lCurrent)
				{
					lResult.DelayedException = lCurrent;
				}

				RemoveOp((Thread)lResult.Data);
				lResult.IsCompleted = true;
				(lResult.AsyncWaitHandle as EventWaitHandle).Set();
				callback(lResult);
			});
			AddAsyncOp(lThread);
			lResult.Data = lThread;
			lThread.Start();

			return lResult;
		}

		public IAsyncResult BeginAccept(Int32 receiveSize, AsyncCallback callback, Object state)
		{
			return BeginAccept(null, receiveSize, callback, state);
		}

		public IAsyncResult BeginAccept(AsyncCallback callback, Object state)
		{
			return BeginAccept(null, 0, callback, state);
		}

		   public Socket EndAccept(out Byte[] buffer, out Int32 bytesTransferred, IAsyncResult asyncResult)
		{
			AsyncResult lResult = asyncResult as AsyncResult;

			CheckAsyncTerminate(asyncResult);
			CheckAsyncException(asyncResult);

			buffer = lResult.Buffer;
			bytesTransferred = lResult.NBytes;

			return lResult.AcceptedSocket;
		}

		public Socket EndAccept(out Byte[] buffer, IAsyncResult asyncResult)
		{
			int lBytes;
			return EndAccept(out buffer, out lBytes, asyncResult);
		}

		public Socket EndAccept(IAsyncResult result)
		{
			int lBytes;
			byte [] lBuffer;

			return EndAccept(out lBuffer, out lBytes, result);
		}

		public void Bind(EndPoint local_end)
		{
			var lEndPoint = (IPEndPoint)local_end;
			if (lEndPoint.Address == null)
				lEndPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), lEndPoint.Port);
			#if cooper
			var lAddress = java.net.InetAddress.getByAddress(lEndPoint.Address.GetAddressBytes());
			fSocketAddress = new java.net.InetSocketAddress(lAddress, lEndPoint.Port);
			fIsServer = true;
			fServerHandle = new java.net.ServerSocket();
			#else
			void *lPointer;
			int lSize;
			#if posix || toffee || darwin
			rtl.__struct_sockaddr_in lIPv4;
			rtl.__struct_sockaddr_in6 lIPv6;
			#if posix
			rtl.__CONST_SOCKADDR_ARG lSockAddr;
			#endif
			#else
			rtl.SOCKADDR_IN lIPv4;
			sockaddr_in6 lIPv6;
			#endif

			IPEndPointToNative(lEndPoint, out lIPv4, out lIPv6, out lPointer, out lSize);
			#if posix
			lSockAddr.__sockaddr__ = (rtl.__struct_sockaddr *) lPointer;
			lSockAddr.__sockaddr_in__ = (rtl.__struct_sockaddr_in *) lPointer;
			if (rtl.__Global.bind(fHandle, lSockAddr, lSize) != 0)
				throw new Exception("Error calling bind function");
			#elif toffee || darwin
			if (rtl.bind(fHandle, (rtl.__struct_sockaddr *)lPointer, lSize) != 0)
				throw new Exception("Error calling bind function");
			#elif island && windows
			if (rtl.bind(fHandle, lPointer, lSize) != 0)
				throw new Exception("Error calling bind function");
			#else
				throw new Exception("Error calling bind function");
			#endif
			#endif
			LocalEndPoint = lEndPoint;
		}

		#if toffee || darwin
		private int htons(int port)
		{
			return (rtl.__uint16_t)((((rtl.__uint16_t)(port) & 0xff00) >> 8) | (((rtl.__uint16_t)(port) & 0x00ff) << 8));
		}
		#endif

		#if !cooper
		#if posix || toffee || darwin
		private void IPEndPointToNative(IPEndPoint endPoint, out rtl.__struct_sockaddr_in lIPv4, out rtl.__struct_sockaddr_in6 lIPv6, out void *ipPointer, out int ipSize)
		{
			switch (endPoint.AddressFamily)
			{
				case AddressFamily.InterNetworkV6:
					lIPv6.sin6_family = AddressFamily.InterNetworkV6;
					#if toffee || darwin
					lIPv6.sin6_port = htons(endPoint.Port);
					#else
					lIPv6.sin6_port = rtl.htons(endPoint.Port);
					#endif
					lIPv6.sin6_scope_id = endPoint.Address.ScopeId;
					var lBytes = endPoint.Address.GetAddressBytes();
					for (int i = 0; i < 16; i++)
						#if toffee && !darwin
						//lIPv6.sin6_addr.__u6_addr.__u6_addr8[i] = lBytes[i]; //TODO
						#elif darwin
						lIPv6.sin6_addr.__u6_addr.__u6_addr8[i] = lBytes[i];
						#elif posix
						lIPv6.sin6_addr.__in6_u.__u6_addr8[i] = lBytes[i];
						#endif
					ipPointer = &lIPv6;
					ipSize = sizeof(rtl.__struct_sockaddr_in6);
					break;

				default:
					lIPv4.sin_family = AddressFamily.InterNetwork;
					#if toffee || darwin
					lIPv4.sin_port = htons(endPoint.Port);
					#else
					lIPv4.sin_port = rtl.htons(endPoint.Port);
					#endif

					lIPv4.sin_addr.s_addr = endPoint.Address.Address;
					ipSize = sizeof(rtl.__struct_sockaddr_in);
					ipPointer = &lIPv4;
					break;
			}

		}
		#else
		private void IPEndPointToNative(IPEndPoint endPoint, out rtl.SOCKADDR_IN lIPv4, out sockaddr_in6 lIPv6, out void *ipPointer, out int ipSize)
		{
			switch (endPoint.AddressFamily)
			{
				case AddressFamily.InterNetworkV6:
					lIPv6.sin6_family = AddressFamily.InterNetworkV6;
					lIPv6.sin6_port = rtl.htons(endPoint.Port);
					lIPv6.sin6_scope_id = endPoint.Address.ScopeId;
					var lBytes = endPoint.Address.GetAddressBytes();
					for (int i = 0; i < 16; i++)
						lIPv6.sin6_addr.u.Byte[i] = lBytes[i];
					ipPointer = &lIPv6;
					ipSize = sizeof(sockaddr_in6);
					break;

				default:
					lIPv4.sin_family = AddressFamily.InterNetwork;
					lIPv4.sin_port = rtl.htons(endPoint.Port);
					lIPv4.sin_addr.S_un.S_addr = endPoint.Address.Address;
					ipSize = sizeof(rtl.SOCKADDR_IN);
					ipPointer = &lIPv4;
					break;
			}

		}
		#endif
		#endif

		public void Connect(EndPoint remoteEP)
		{
			var lEndPoint = (IPEndPoint)remoteEP;
			#if cooper
			var lAddress = java.net.InetAddress.getByAddress(lEndPoint.Address.GetAddressBytes());
			fHandle = new java.net.Socket(lAddress, lEndPoint.Port);
			fSocketInput = fHandle.getInputStream();
			fSocketOutput = fHandle.getOutputStream();
			#else
			void *lPointer;
			int lSize;
			#if posix || toffee || darwin
			rtl.__struct_sockaddr_in lIPv4;
			rtl.__struct_sockaddr_in6 lIPv6;
			#if posix
			rtl.__CONST_SOCKADDR_ARG lSockAddr;
			#else
			rtl.__struct_sockaddr lSockAddr;
			#endif
			#else
			rtl.SOCKADDR_IN lIPv4;
			sockaddr_in6 lIPv6;
			#endif


			IPEndPointToNative(lEndPoint, out lIPv4, out lIPv6, out lPointer, out lSize);
			var lRes = 0;
			#if posix
			lSockAddr.__sockaddr__ = (rtl.__struct_sockaddr *) lPointer;
			lRes = rtl.connect(fHandle, lSockAddr, lSize);
			#elif toffee || darwin
			lRes = rtl.connect(fHandle, (rtl.__struct_sockaddr *)lPointer, lSize);
			#else
			lRes = rtl.connect(fHandle, lPointer, lSize);
			#endif
			if (lRes != 0)
				throw new Exception("Error connecting socket");
			#endif

			Connected = true;
		}

		public void Connect(String host, Int32 port)
		{
			var lAddress = IPAddress.Parse(host);
			Connect(new IPEndPoint(lAddress, port));
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
				throw new Exception("Error connecting socket");
		}

		public void Connect(IPAddress address, Int32 port)
		{
			var lEndPoint = new IPEndPoint(address, port);
			Connect(lEndPoint);
		}

		public IAsyncResult BeginConnect(IPAddress[] addresses, Int32 port, AsyncCallback callback, Object state)
		{
			IPEndPoint lEndPoint;
			var lResult = new AsyncResult(state);

			var lThread = new Thread(() =>
			{
				lResult.AsyncWaitHandle.WaitOne();
				foreach(IPAddress lAddress in addresses)
				{
					lEndPoint = new IPEndPoint(lAddress, port);
					try
					{
						Connect(lEndPoint);
						lResult.DelayedException = null;
						break;
					}
					catch(Exception lCurrent)
					{
						lResult.DelayedException = lCurrent;
					}
				}

				RemoveOp((Thread)lResult.Data);
				lResult.IsCompleted = true;
				(lResult.AsyncWaitHandle as EventWaitHandle).Set();
				callback(lResult);
			});
			AddAsyncOp(lThread);
			lResult.Data = lThread;
			lThread.Start();

			return lResult;
		}

		public IAsyncResult BeginConnect(EndPoint end_point, AsyncCallback callback, Object state)
		{
			var lIPEndPoint = (IPEndPoint)end_point;
			return BeginConnect(new IPAddress[] {lIPEndPoint.Address}, lIPEndPoint.Port, callback, state);
		}

		public IAsyncResult BeginConnect(String host, Int32 port, AsyncCallback callback, Object state)
		{
			var lAddress = IPAddress.Parse(host);
			return BeginConnect(new IPEndPoint(lAddress, port), callback, state);
		}

		public IAsyncResult BeginConnect(IPAddress address, Int32 port, AsyncCallback callback, Object state)
		{
			var lEndPoint = new IPEndPoint(address, port);
			return BeginConnect(lEndPoint, callback, state);
		}

		public void EndConnect(IAsyncResult result)
		{
			CheckAsyncTerminate(result);
			CheckAsyncException(result);
		}

		private void CheckAsyncException(IAsyncResult result)
		{
			var lAsyncResult = (AsyncResult)result;

			if (lAsyncResult.DelayedException != null)
				throw lAsyncResult.DelayedException;
		}

		private void CheckAsyncTerminate(IAsyncResult result)
		{
			if (!result.IsCompleted)
			{
				result.AsyncWaitHandle.WaitOne();
				(result.AsyncWaitHandle as EventWaitHandle).Set();
			}
		}

		public void Disconnect(Boolean reuseSocket)
		{
			Dispose();
		}

		public IAsyncResult BeginDisconnect(Boolean reuseSocket, AsyncCallback callback, Object state)
		{
			var lResult = new AsyncResult(state);
			var lThread = new Thread(() =>
			{
				lResult.AsyncWaitHandle.WaitOne();
				Disconnect(reuseSocket);
				lResult.IsCompleted = true;
				ClearAsyncOps();
				(lResult.AsyncWaitHandle as EventWaitHandle).Set();
				callback(lResult);
			});
			lThread.Start();

			return lResult;
		}

		public void EndDisconnect(IAsyncResult asyncResult)
		{
			CheckAsyncTerminate(asyncResult);
			CheckAsyncException(asyncResult);
		}

		public void Listen(Int32 backlog)
		{
			#if cooper
			fServerHandle.bind(fSocketAddress, backlog);
			#else
			if (rtl.listen(fHandle, backlog) != 0)
				throw new Exception("Error calling to listen function");
			#endif
		}

		public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags)
		{
			#if cooper
			if (fSocketInput == null)
				throw new Exception("Socket is not connected");
			return fSocketInput.read(buffer, offset, size);
			#else
			void *lPointer;
			lPointer = &buffer[0];
			return rtl.recv(fHandle, (AnsiChar *)lPointer, size, (int)flags);
			#endif
		}

		public Int32 Receive(Byte[] buffer, Int32 size, SocketFlags flags)
		{
			return Receive(buffer, 0, size, flags);
		}

		public Int32 Receive(Byte[] buffer, SocketFlags flags)
		{
			return Receive(buffer, 0, length(buffer), flags);
		}

		public Int32 Receive(Byte[] buffer)
		{
			return Receive(buffer, 0, length(buffer), SocketFlags.None);
		}

		public IAsyncResult BeginReceive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags flags, out SocketError error, AsyncCallback callback, Object state)
		{
			var lResult = new AsyncResult(state);
			lResult.Error = error;

			var lThread = new Thread(() =>
			{
				lResult.AsyncWaitHandle.WaitOne();
				try
				{
					lResult.NBytes = Receive(buffer, offset, size, flags);
				}
				catch(Exception lCurrent)
				{
					if (lResult.Error == SocketError.NoValue)
						lResult.DelayedException = lCurrent;

					lResult.Error = SocketError.SocketError;
				}

				RemoveOp((Thread)lResult.Data);
				lResult.IsCompleted = true;
				(lResult.AsyncWaitHandle as EventWaitHandle).Set();
				callback(lResult);
			});
			AddAsyncOp(lThread);
			lResult.Data = lThread;
			lThread.Start();

			return lResult;
		}

		public IAsyncResult BeginReceive(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socket_flags, AsyncCallback callback, Object state)
		{
			SocketError lError = SocketError.NoValue;

			return BeginReceive(buffer, offset, size, socket_flags, out lError, callback, state);
		}

		   public Int32 EndReceive(IAsyncResult asyncResult, out SocketError errorCode)
		{
			AsyncResult lResult = asyncResult as AsyncResult;

			CheckAsyncTerminate(asyncResult);
			CheckAsyncException(asyncResult);
			errorCode = lResult.Error;

			return lResult.NBytes;
		}

		public Int32 EndReceive(IAsyncResult result)
		{
			SocketError lError;

			return EndReceive(result, out lError);
		}

		public Int32 Send(Byte[] buf, Int32 offset, Int32 size, SocketFlags flags)
		{
			#if cooper
			if (fSocketOutput == null)
				throw new Exception("Socket is not connected");
			fSocketOutput.write(buf, offset, size);
			return size;
			#else
			void *lPointer;
			lPointer = &buf[offset];
			return rtl.send(fHandle, (AnsiChar *)lPointer, size, (int)flags);
			#endif
		}

		public Int32 Send(Byte[] buf, Int32 size, SocketFlags flags)
		{
			return Send(buf, 0, size, flags);
		}

		public Int32 Send(Byte[] buf, SocketFlags flags)
		{
			return Send(buf, 0, length(buf), flags);
		}

		public Int32 Send(Byte[] buf)
		{
			return Send(buf, 0, length(buf), SocketFlags.None);
		}

		public IAsyncResult BeginSend(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socketFlags, out SocketError errorCode, AsyncCallback callback, Object state)
		{
			var lResult = new AsyncResult(state);
			lResult.Error = errorCode;

			var lThread = new Thread(() =>
			{
				lResult.AsyncWaitHandle.WaitOne();
				try
				{
					lResult.NBytes = Send(buffer, offset, size, socketFlags);
				}
				catch(Exception lCurrent)
				{
					if (lResult.Error == SocketError.NoValue)
						lResult.DelayedException = lCurrent;

					lResult.Error = SocketError.SocketError;
				}

				RemoveOp((Thread)lResult.Data);
				lResult.IsCompleted = true;
				(lResult.AsyncWaitHandle as EventWaitHandle).Set();
				callback(lResult);
			});
			AddAsyncOp(lThread);
			lResult.Data = lThread;
			lThread.Start();

			return lResult;
		}

		public IAsyncResult BeginSend(Byte[] buffer, Int32 offset, Int32 size, SocketFlags socket_flags, AsyncCallback callback, Object state)
		{
			SocketError lError = SocketError.NoValue;
			return BeginSend(buffer, offset, size, socket_flags, out lError, callback, state);
		}

		public Int32 EndSend(IAsyncResult asyncResult, out SocketError errorCode)
		{
			AsyncResult lResult = asyncResult as AsyncResult;

			CheckAsyncTerminate(asyncResult);
			CheckAsyncException(asyncResult);
			errorCode = lResult.Error;

			return lResult.NBytes;
		}

		public Int32 EndSend(IAsyncResult result)
		{
			SocketError lError = SocketError.SocketError;
			return EndSend(result, out lError);
		}

		#if cooper
		private void InternalSetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionValue)
		{
			switch(optionName)
			{
				case SocketOptionName.ReuseAddress:
					if (fIsServer)
						fServerHandle.setReuseAddress((bool)optionValue);
					else
						fHandle.setReuseAddress((bool)optionValue);
					break;

				case SocketOptionName.KeepAlive:
					fHandle.setKeepAlive((bool)optionValue);
					break;

				case SocketOptionName.DontLinger:
					fHandle.setSoLinger(false, 0);
					break;

				case SocketOptionName.OutOfBandInline:
					fHandle.setOOBInline((bool)optionValue);
					break;

				case SocketOptionName.SendBuffer:
					fHandle.setSendBufferSize(optionValue);
					break;

				case SocketOptionName.ReceiveBuffer:
					if (fIsServer)
						fServerHandle.setReceiveBufferSize(optionValue);
					else
						fHandle.setReceiveBufferSize(optionValue);
					break;

				case SocketOptionName.NoDelay:
					fHandle.setTcpNoDelay((bool)optionValue);
					break;

				case SocketOptionName.SendTimeout:
				case SocketOptionName.ReceiveTimeout:
					if (fIsServer)
						fServerHandle.setSoTimeout(optionValue);
					else
						fHandle.setSoTimeout(optionValue);
					break;
			}
		}
		#else
		private void InternalSetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, void *optionValue, Int32 optionValueLength)
		{
			if (rtl.setsockopt(fHandle, (int)optionLevel, (int)optionName, (AnsiChar *)optionValue, optionValueLength) != 0)
				throw new Exception("Can not change socket option");
		}
		#endif

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Int32 optionValue)
		{
			#if cooper
			InternalSetSocketOption(optionLevel, optionName, optionValue);
			#else
			void *lValue = &optionValue;
			InternalSetSocketOption(optionLevel, optionName, lValue, sizeof(Int32));
			#endif
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Boolean optionValue)
		{
			#if cooper
			InternalSetSocketOption(optionLevel, optionName, (Int32)optionValue);
			#else
			void *lValue = &optionValue;
			InternalSetSocketOption(optionLevel, optionName, lValue, sizeof(Boolean));
			#endif
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Object optionValue)
		{
			#if cooper
			if (optionName == SocketOptionName.Linger  && optionName == SocketOptionName.DontLinger)
			{
				var lValue = (LingerOption)optionValue;
				fHandle.setSoLinger(lValue.Enabled, lValue.LingerTime);
			}
			#else
			void *lValue = &optionValue;
			InternalSetSocketOption(optionLevel, optionName, lValue, sizeof(Object));
			#endif
		}

		public void SetSocketOption(SocketOptionLevel optionLevel, SocketOptionName optionName, Byte[] optionValue)
		{
			#if cooper
			InternalSetSocketOption(optionLevel, optionName, optionValue[0]);
			#else
			void *lValue = &optionValue[0];
			InternalSetSocketOption(optionLevel, optionName, lValue, length(optionValue));
			#endif
		}

		private new void Dispose()
		{
			#if cooper
			if (!fIsServer)
				fHandle.close();
			else
				fServerHandle.close();
			#else
			#if posix || toffee || darwin
			if (rtl.close(fHandle) != 0)
				throw new Exception("Error closing socket");
			#else
			if (rtl.closesocket(fHandle) != 0)
				throw new Exception("Error closing socket");
			#endif
			#endif

			Connected = false;
		}

		public void Close()
		{
			Dispose();
		}

		public void Shutdown(SocketShutdown how)
		{
			#if cooper
			if (fIsServer)
				fServerHandle.close();
			else
				fHandle.close();
			#else
			if (rtl.shutdown(fHandle, (int)how) != 0)
				throw new Exception("Error closing socket");
			#endif
		}

		public Int32 Available
		{
			get
			{
				#if cooper
				if (fSocketInput != null)
					return fSocketInput.available();
				else
					return 0;
				#else
				rtl.u_long lData = 0;
				var lError = false;
				#if posix || toffee || darwin
				lError = rtl.ioctl(fHandle, FIONREAD, &lData) < 0;
				#else
				var lRes = 0;
				if (rtl.ioctlsocket(fHandle, rtl.FIONREAD, &lData) != 0)
				{
					lRes = rtl.WSAGetLastError();
				}
				lError = (lRes != 0) && (lRes != rtl.WSAEOPNOTSUPP);
				#endif
				if (lError)
					lData = 0;
					//throw new Exception("Error calling ioctl function");

				return lData;
				#endif
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
		public Int32 ErrorCode { get; set; }
		//public SocketError SocketErrorCode { get; set; }
		//public override String Message { get; set; }
	}
	#endif
}