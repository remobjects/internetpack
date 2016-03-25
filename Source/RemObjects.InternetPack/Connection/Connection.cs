/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2015. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemObjects.InternetPack
{
	public class Connection : Stream, IDisposable, IConnectionTimeouts
	{
		public Connection(Socket socket)
		{
			this.fDataSocket = socket;

			if (this.fDataSocket != null)
				this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

			this.SetDefaultValues();
		}

		public Connection(Binding binding)
		{
			this.Init(binding);

			this.SetDefaultValues();
		}

		private const Int32 READLINE_BUFFER_SIZE = 1024;
		private const Int32 BUFFER_SIZE = 1024;//64 * 1024;

		private Boolean fTimeoutTimerEnabled;
		private System.Threading.Timer fTimeoutTimer;

		public void Init(Socket socket)
		{
			this.fDataSocket = socket;

			if (!this.EnableNagle)
				this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

			this.SetDefaultValues();
		}

		private void Init(Binding binding)
		{
			this.fBinding = binding;
			this.fDataSocket = new Socket(binding.AddressFamily, binding.SocketType, binding.Protocol);

			if (!this.EnableNagle)
				this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
		}

		private void SetDefaultValues()
		{
			this.fEnableNagle = false;
			this.BufferedAsync = true;
			this.Timeout = DEFAULT_TIMEOUT;
			this.MaxLineLength = DEFAULT_MAX_LINE_LENGTH;
		}

		public override String ToString()
		{
			return String.Format("{0} Local: {1} Remote {2}", this.GetType().Name, this.LocalEndPoint, this.RemoteEndPoint);
		}

		#region Properties
		public const Int32 DEFAULT_TIMEOUT = 5 * 60; /* 5 minutes */
		public const Int32 DEFAULT_MAX_LINE_LENGTH = 4096;

		public Binding Binding
		{
			get
			{
				if (this.fBinding != null)
					return this.fBinding;

				return new Binding(DataSocket.AddressFamily);
			}
		}
		private Binding fBinding;

		public EndPoint LocalEndPoint
		{
			get
			{
				return this.DataSocket.LocalEndPoint;
			}
		}

		public EndPoint RemoteEndPoint
		{
			get
			{
				return this.DataSocket.RemoteEndPoint;
			}
		}

		public Encoding Encoding
		{
			get
			{
				if (this.fEncoding == null)
					this.fEncoding = Encoding.Default;

				return this.fEncoding;
			}
			set
			{
				this.fEncoding = value;
			}
		}
		private Encoding fEncoding;

		public virtual Boolean EnableNagle
		{
			get
			{
				return fEnableNagle;
			}
			set
			{
				fEnableNagle = value;
			}
		}
		private Boolean fEnableNagle;

		public virtual Boolean Secure
		{
			get
			{
				return false;
			}
		}

		public Int32 Timeout
		{
			get
			{
				return this.fTimeout;
			}
			set
			{
				this.fTimeout = value;

				if (this.fTimeoutTimer != null)
				{
					this.fTimedOut = false;
					//this.fTimeoutTimer.Change(this.Timeout * 1000, this.Timeout * 1000);
				}
			}
		}
		private Int32 fTimeout;

		public Boolean TimedOut
		{
			get
			{
				return this.fTimedOut & this.TimeoutEnabled;
			}
		}
		private Boolean fTimedOut;

		public Boolean BufferedAsync { get; set; }

		public Boolean TimeoutEnabled
		{
			get
			{
				return this.fTimeoutTimer != null;
			}
			set
			{
				if (value && this.fTimeoutTimer == null)
				{
					this.fTimedOut = false;
					this.fTimeoutTimer = new System.Threading.Timer(this.TimeoutElapsed, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
				}
				else if (!value && this.fTimeoutTimer != null)
				{
					this.fTimedOut = false;
					this.fTimeoutTimer.Dispose();
					this.fTimeoutTimer = null;
				}
			}
		}

		public Int32 MaxLineLength { get; set; }

		public Boolean MaxLineLengthEnabled { get; set; }
		#endregion

		#region Timeouts & Limits
		private void TimeoutElapsed(Object sender)
		{
			if (this.fTimeoutTimerEnabled && !this.fTimedOut)
			{
				this.fTimedOut = true;
				this.Abort();
			}
		}

		protected void StartTimeoutTimer()
		{
			if (!this.TimeoutEnabled)
			{
				return;
			}

			this.fTimeoutTimerEnabled = true;
			this.fTimedOut = false;
			this.fTimeoutTimer.Change(this.Timeout * 1000, System.Threading.Timeout.Infinite);
		}

		protected void StopTimeoutTimer()
		{
			if (!this.TimeoutEnabled)
			{
				return;
			}

			this.fTimeoutTimerEnabled = false;
			this.fTimeoutTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
		}
		#endregion

		#region DataSocket access
		public virtual System.Net.Sockets.Socket DataSocket
		{
			get
			{
				return fDataSocket;
			}
		}
		private System.Net.Sockets.Socket fDataSocket;

		public virtual Boolean DataSocketConnected
		{
			get
			{
				return DataSocket.Connected;
			}
		}

		public virtual Int32 DataSocketAvailable
		{
			get
			{
				return this.DataSocket.Available;
			}
		}

		public Boolean Connected
		{
			get
			{
				return this.DataSocketConnected;
			}
		}

		public EndPoint OriginalEndpoint
		{
			get
			{
				return this.fOriginalEndpoint;
			}
			protected set
			{
				this.fOriginalEndpoint = value;
			}
		}
		private EndPoint fOriginalEndpoint;

		internal protected virtual void InitializeServerConnection()
		{
		}

		internal protected virtual IAsyncResult BeginInitializeServerConnection(AsyncCallback callback, Object state)
		{
			return null;
		}

		internal protected virtual void EndInitializeServerConnection(IAsyncResult ar)
		{
		}

		protected virtual Int32 DataSocketReceiveWhatsAvaiable(Byte[] buffer, Int32 offset, Int32 size)
		{
			this.StartTimeoutTimer();
			try
			{
				Int32 lReadBytes = DataSocket.Receive(buffer, offset, size, SocketFlags.None);

				if (lReadBytes == 0)
					this.DataSocket.Close();

				this.TriggerOnBytesReceived(lReadBytes);
				return lReadBytes;
			}
			catch (ObjectDisposedException)
			{
				this.DataSocket.Close();
				return 0;
			}
			catch (SocketException)
			{
				this.DataSocket.Close();
				return 0;
			}
			finally
			{
				this.StopTimeoutTimer();
			}
		}

		protected virtual Int32 DataSocketSendAsMuchAsPossible(Byte[] buffer, Int32 offset, Int32 size)
		{
			this.StartTimeoutTimer();
			try
			{
				Int32 lSentBytes = DataSocket.Send(buffer, offset, size, SocketFlags.None);

				TriggerOnBytesSent(lSentBytes);

				return lSentBytes;
			}
			finally
			{
				this.StopTimeoutTimer();
			}
		}

		protected virtual void DataSocketConnect(EndPoint endPoint)
		{
			fOriginalEndpoint = endPoint;
			DataSocket.Connect(endPoint);
		}

		private void Abort()
		{
			if (!this.DataSocket.Connected)
				return;

			this.DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(false, 0));

			this.DataSocketClose();
		}

		protected virtual void DataSocketClose()
		{
			try
			{
				if (!this.DataSocket.Connected)
					return;

				try
				{
					this.DataSocket.Shutdown(SocketShutdown.Both);
				}
				catch (SocketException)
				{
				}

				try
				{
					this.DataSocket.Close();
				}
				catch (SocketException)
				{
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		protected virtual void DataSocketClose(Boolean dispose)
		{
			try
			{
				if (this.DataSocket.Connected)
					this.DataSocket.Shutdown(SocketShutdown.Both);
			}
			catch (Exception)
			{
				// Suppress any exceptions
			}

			if (dispose)
				this.DataSocket.Close();
		}

		private Int32 DataSocketReceive(Byte[] buffer, Int32 offset, Int32 size)
		{
			Int32 lTotalReadBytes = 0;

			do
			{
				Int32 lReadBytes = this.DataSocketReceiveWhatsAvaiable(buffer, offset, size);
				if (lReadBytes == 0)
				{
					return lTotalReadBytes;
				}

				size -= lReadBytes;
				offset += lReadBytes;
				lTotalReadBytes += lReadBytes;
			}
			while (size > 0);

			return lTotalReadBytes;
		}

		private Int32 DataSocketSend(Byte[] buffer, Int32 offset, Int32 size)
		{
			Int32 lTotalSentBytes = 0;

			while (size > 0)
			{
				Int32 lSentBytes = this.DataSocketSendAsMuchAsPossible(buffer, offset, size);
				size -= lSentBytes;
				offset += lSentBytes;
				lTotalSentBytes += lSentBytes;
			}
			return lTotalSentBytes;
		}

		protected Int32 DataSocketSend(Byte[] buffer)
		{
			return DataSocketSend(buffer, 0, buffer.Length);
		}
		#endregion

		#region Connection Methods
		public virtual void Connect(EndPoint endPoint)
		{
			DataSocketConnect(endPoint);
		}

		public virtual void Connect(IPAddress address, Int32 port)
		{
			this.DataSocketConnect(new IPEndPoint(address, port));
		}

		public virtual void Disconnect()
		{
			this.DataSocketClose();
		}
		#endregion

		#region Data Access
		public Int32 Available
		{
			get
			{
				Int32 lAvailable = DataSocketAvailable;

				if (fBuffer != null)
					lAvailable += fBufferEnd - fBufferStart;

				return lAvailable;
			}
		}

		private Int32 Receive(Byte[] buffer, Int32 offset, Int32 size, Boolean block)
		{
			// If there is no buffer allocated
			if (this.fBuffer == null)
			{
				if (block)
					return this.DataSocketReceive(buffer, offset, size);

				return this.DataSocketReceiveWhatsAvaiable(buffer, offset, size);
			}

			// still bytes in buffer? if yes, get them first
			Int32 lSize = fBufferEnd - fBufferStart;

			if (lSize > size)
			{
				// more bytes in buffer than we need?
				Array.Copy(fBuffer, fBufferStart, buffer, offset, size);
				fBufferStart += size;

				return size;
			}

			// less (or same) number of bytes in buffer then we need?
			Buffer.BlockCopy(fBuffer, fBufferStart, buffer, offset, lSize);
			fBuffer = null;

			// if more bytes werer requested, and bytes are available, get them
			if (size > lSize && (DataSocketAvailable > 0 || block))
				return lSize + DataSocketReceive(buffer, offset + lSize, size - lSize);

			return lSize;
		}

		public Int32 Receive(Byte[] buffer, Int32 offset, Int32 size)
		{
			return Receive(buffer, offset, size, true);
		}

		public Int32 ReceiveWhatsAvailable(Byte[] buffer, Int32 offset, Int32 size)
		{
			return Receive(buffer, offset, size, false);
		}

		public Int32 Receive(Byte[] buffer)
		{
			return Receive(buffer, 0, buffer.Length, true);
		}

		private class DataBlock
		{
			private readonly Byte[] fBuffer;
			private Int32 fActualSize = -1;

			public DataBlock(Int32 size)
			{
				this.fBuffer = new Byte[size];
			}

			public Byte[] Buffer
			{
				get
				{
					return this.fBuffer;
				}
			}

			public Int32 ActualSize
			{
				get
				{
					return (this.fActualSize == -1) ? this.fBuffer.Length : this.fActualSize;
				}
				set
				{
					this.fActualSize = value;
				}
			}
		}

		/// <summary>
		/// Get data from connection until all bytes have been read
		/// (i.e., until Receive() returns 0). Uses a constant buffersize.
		/// </summary>
		/// <returns></returns>
		public Byte[] ReceiveAllRemaining()
		{
			return ReceiveAllRemaining(BUFFER_SIZE);
		}

		/// <summary>
		/// Get data from connection until all bytes have been read
		/// (i.e., until Receive() returns 0). Uses a buffersize passed as a parameter.
		/// </summary>
		/// <param name="bufferSize">Size of receive buffer used for each pass</param>
		/// <returns></returns>
		public Byte[] ReceiveAllRemaining(Int32 bufferSize)
		{
			Byte[] result = null;

			ArrayList lBlocks = new ArrayList();

			Int32 lTotal = 0;

			Boolean lDone;
			do
			{
				lDone = false;

				Int32 lBytesToRead = bufferSize;

				DataBlock lBuffer = new DataBlock(lBytesToRead);

				Int32 lRead;
				try
				{
					lRead = Receive(lBuffer.Buffer, 0, lBytesToRead);
				}
				catch (ObjectDisposedException)
				{
					lRead = 0;
				}
				catch (SocketException)
				{
					lRead = 0;
				}

				if (lRead > 0)
				{
					lBuffer.ActualSize = lRead;
					lBlocks.Add(lBuffer);
					lTotal += lRead;
				}
				else
				{
					lDone = true;
				}
			}
			while (!lDone);

			if (lTotal > 0)
			{
				Int32 lCurrent = 0;

				Byte[] lNewBuffer = new Byte[lTotal];

				for (Int32 i = 0; i < lBlocks.Count; i++)
				{
					DataBlock lData = (DataBlock)(lBlocks[i]);
					Buffer.BlockCopy(lData.Buffer, 0, lNewBuffer, lCurrent, lData.ActualSize);
					lCurrent += lData.ActualSize;
				}

				result = lNewBuffer;
			}

			return result;
		}

		public void ReceiveToStream(Stream stream)
		{
			ReceiveToStream(stream, -1, BUFFER_SIZE);
		}

		public void ReceiveToStream(Stream stream, Int64 size)
		{
			ReceiveToStream(stream, size, BUFFER_SIZE);
		}

		public void ReceiveToStream(Stream stream, Int64 size, Int32 bufferSize)
		{
			Int32 lTotal = 0;

			Boolean lDone;
			do
			{
				lDone = false;

				Int64 lBytesToRead = bufferSize;
				if (size > -1 && size - lTotal < bufferSize)
					lBytesToRead = size - lTotal;

				Byte[] lBuffer = new Byte[lBytesToRead];

				Int32 lRead = Receive(lBuffer, 0, (Int32)lBytesToRead);

				if (lRead > 0)
				{
					stream.Write(lBuffer, 0, lRead);
					lTotal += lRead;
				}
				else
				{
					lDone = true;
				}

				if (size > -1 && lTotal >= size)
					lDone = true;
			}
			while (!lDone);
		}

		public void SendFromStream(Stream stream)
		{
			SendFromStream(stream, -1, BUFFER_SIZE);
		}

		public void SendFromStream(Stream stream, Int64 size)
		{
			SendFromStream(stream, size, BUFFER_SIZE);
		}

		public void SendFromStream(Stream stream, Int64 size, Int32 bufferSize)
		{
			if (size == -1)
				size = stream.Length - stream.Position;

			Int32 lTotal = 0;

			Boolean lDone;
			do
			{
				lDone = false;

				Int64 lBytesToSend = bufferSize;
				if (size - lTotal < bufferSize)
					lBytesToSend = size - lTotal;

				Byte[] lBuffer = new Byte[lBytesToSend];
				stream.Read(lBuffer, 0, (Int32)lBytesToSend);

				Int32 lSent = Send(lBuffer, 0, (Int32)lBytesToSend);

				lTotal += lSent;

				if (lTotal >= size)
					lDone = true;
			}
			while (!lDone);
		}

		public Int32 Send(Byte[] buffer, Int32 offset, Int32 size)
		{
			return DataSocketSend(buffer, offset, size);
		}

		public Int32 Send(Byte[] buffer)
		{
			return DataSocketSend(buffer, 0, buffer.Length);
		}

		public virtual void SkipBytes(Int32 size)
		{
			if (size <= 0)
				return;

			Byte[] lBuffer = new Byte[BUFFER_SIZE];
			while (size > 0)
			{
				// lBytesRead can be 0 if connection is closed
				Int32 lBytesRead = Read(lBuffer, 0, size > BUFFER_SIZE ? BUFFER_SIZE : size);

				if (lBytesRead == 0)
					break;

				size -= lBytesRead;
			}
		}
		#endregion

		#region System.IO.Stream Methods
		public override void Flush()
		{
		}

		public override void Close()
		{
			this.DataSocketClose(true);
			base.Close();

			if (fTimeoutTimer != null)
			{
				this.fTimeoutTimer.Dispose();
				this.fTimeoutTimer = null;
			}
		}

		public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
		{
			return Receive(buffer, offset, size, true);
		}

		public override Int64 Seek(Int64 offset, SeekOrigin origin)
		{
			throw new Exception(String.Format("{0} does not support seeking", this.GetType().FullName));
		}

		public override void SetLength(Int64 length)
		{
			throw new Exception(String.Format("{0} does not support SetLength", this.GetType().FullName));
		}

		public override void Write(Byte[] buffer, Int32 offset, Int32 size)
		{
			DataSocketSend(buffer, offset, size);
		}

		public override Boolean CanRead
		{
			get
			{
				return true;
			}
		}

		public override Boolean CanSeek
		{
			get
			{
				return false;
			}
		}

		public override Boolean CanWrite
		{
			get
			{
				return true;
			}
		}

		public override Int64 Length
		{
			get
			{
				return fPosition + DataSocketAvailable;
			}
		}

		public override Int64 Position
		{
			get
			{
				return fPosition;
			}
			set
			{
				Seek(value, SeekOrigin.Begin);
				fPosition = value;
			}
		}
		private Int64 fPosition;
		#endregion

		#region Async
		private class AsyncRequest : IAsyncResult
		{
			public Byte[] AsyncBuffer
			{
				get
				{
					return fAsyncBuffer;
				}
				set
				{
					fAsyncBuffer = value;
				}
			}
			private Byte[] fAsyncBuffer;

			public Int32 AsyncOffset
			{
				get
				{
					return fAsyncOffset;
				}
				set
				{
					fAsyncOffset = value;
				}
			}
			private Int32 fAsyncOffset;

			public Int32 AsyncCount
			{
				get
				{
					return fAsyncCount;
				}
				set
				{
					fAsyncCount = value;
				}
			}
			private Int32 fAsyncCount;

			public AsyncCallback AsyncCallback
			{
				get
				{
					return fAsyncCallback;
				}
				set
				{
					fAsyncCallback = value;
				}
			}
			private AsyncCallback fAsyncCallback;

			public Object AsyncState
			{
				get
				{
					return fState;
				}
				set
				{
					fState = value;
				}
			}
			private Object fState;

			public Int32 AsyncRest
			{
				get
				{
					return fAsyncRest;
				}
				set
				{
					fAsyncRest = value;
				}
			}
			private Int32 fAsyncRest;

			Object IAsyncResult.AsyncState
			{
				get
				{
					return fState;
				}
			}

			public Boolean CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get
				{
					return null;
				}
			}

			public Boolean IsCompleted
			{
				get
				{
					return fAsyncRest == 0;
				}
			}
		}

		/// <summary>
		/// Raised when an async read fails and the socket is disconnected; Will not work when not using async reading
		/// </summary>
		public event EventHandler AsyncDisconnect;

		/// <summary>
		/// Raised when there is data received but it's not complete yet. can be used for timeout handling.
		/// </summary>
		public event EventHandler AsyncHaveIncompleteData;

		internal protected virtual void TriggerAsyncDisconnect()
		{
			if (AsyncDisconnect != null)
				AsyncDisconnect(this, EventArgs.Empty);
		}

		protected virtual void TriggerAsyncHaveIncompleteData()
		{
			if (AsyncHaveIncompleteData != null)
				AsyncHaveIncompleteData(this, EventArgs.Empty);
		}

		private class BufferAsyncResult : IAsyncResult
		{
			public BufferAsyncResult(Byte[] buffer, Int32 offset, Int32 count, Object aState)
			{
				fBuffer = buffer;
				fOffset = offset;
				fCount = count;
				fAsyncState = aState;
			}

			#region IAsyncResult Members
			public Object AsyncState
			{
				get
				{
					return fAsyncState;
				}
			}
			private readonly Object fAsyncState;

			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get
				{
					return null;
				}
			}

			public Boolean CompletedSynchronously
			{
				get
				{
					return true;
				}
			}

			public Boolean IsCompleted
			{
				get
				{
					return true;
				}
			}

			public Byte[] Buffer
			{
				get
				{
					return this.fBuffer;
				}
				set
				{
					this.fBuffer = value;
				}
			}
			public Byte[] fBuffer;

			public Int32 Offset
			{
				get
				{
					return this.fOffset;
				}
				set
				{
					this.fOffset = value;
				}
			}
			public Int32 fOffset;

			public Int32 Count
			{
				get
				{
					return this.fCount;
				}
				set
				{
					this.fCount = value;
				}
			}
			public Int32 fCount;
			#endregion
		}

		protected virtual IAsyncResult IntBeginRead(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{

			return fDataSocket.BeginReceive(buffer, offset, count, SocketFlags.None, callback, state);
		}

		protected virtual Int32 IntEndRead(IAsyncResult ar)
		{
			return fDataSocket.EndReceive(ar);
		}

		protected virtual void IntEndWrite(IAsyncResult ar)
		{
			this.fDataSocket.EndSend(ar);
		}

		protected virtual IAsyncResult IntBeginWrite(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{
			return this.fDataSocket.BeginSend(buffer, offset, count, SocketFlags.None, callback, state);
		}

		// returns null if there's nothing in the buffer; returns "" if it's an empty line else the line in the buffer
		public String BufferReadLine()
		{
			if (!BufferedAsync)
				throw new Exception("BufferedAsync must be true to use BeginReadLine");

			if (fBuffer != null)
			{
				for (Int32 i = fBufferStart; i < fBufferEnd; i++)
				{
					if (fBuffer[i] != 10)
						continue;

					Int32 lLen = i - fBufferStart;
					if (lLen > 0 && fBuffer[i - 1] == 13)
						lLen--;

					String lData = Encoding.GetString(fBuffer, fBufferStart, lLen);
					fBufferStart = i + 1;

					return lData;
				}
			}

			return null;
		}

		public IAsyncResult BeginReadLine(AsyncCallback callback, Object state)
		{
			if (!BufferedAsync)
				throw new Exception("BufferedAsync must be true to use BeginReadLine");

			AsyncReadLineRequest lRequest;
			if (fBuffer != null)
			{
				for (Int32 i = fBufferStart; i < fBufferEnd; i++)
				{
					if (fBuffer[i] != 10)
						continue;

					Int32 lLen = i - fBufferStart;

					if (lLen > 0 && fBuffer[i - 1] == 13)
						lLen--;

					lRequest = new AsyncReadLineRequest(state, callback);
					lRequest.Data.Write(fBuffer, fBufferStart, lLen);

					lRequest.State = AsyncReadLineState.SyncDone;
					fBufferStart = i + 1;

					callback(lRequest);

					return lRequest;
				}
			}

			lRequest = new AsyncReadLineRequest(state, callback);
			if (fBuffer != null)
				lRequest.Data.Write(fBuffer, fBufferStart, fBufferEnd - fBufferStart);
			else
				fBuffer = new Byte[BUFFER_SIZE];

			fBufferStart = 0;
			fBufferEnd = 0;

			try
			{
				IntBeginRead(fBuffer, 0, fBuffer.Length, new AsyncCallback(IntReadLineCallback), lRequest);
			}
			catch (ObjectDisposedException) // disconnect from this side
			{
				TriggerAsyncDisconnect();
				throw;
			}
			catch (SocketException) // disconnect
			{
				TriggerAsyncDisconnect();
				throw;
			}

			return lRequest;
		}

		private enum AsyncReadLineState
		{
			Done,
			MaxLineLengthReached,
			SyncDone
		}

		private class AsyncReadLineRequest : IAsyncResult
		{
			public AsyncReadLineRequest(Object state, AsyncCallback callback)
			{
				fAsyncState = state;
				fCallback = callback;
				this.fData = new MemoryStream(1024);
			}

			#region IAsyncResult Members
			public Object AsyncState
			{
				get
				{
					return fAsyncState;
				}
			}
			private readonly Object fAsyncState;

			public System.Threading.WaitHandle AsyncWaitHandle
			{
				get
				{
					return null;
				}
			}

			public Boolean CompletedSynchronously
			{
				get
				{
					return fState == AsyncReadLineState.SyncDone;
				}
			}

			public Boolean IsCompleted
			{
				get
				{
					return fState >= AsyncReadLineState.Done;
				}
			}

			public AsyncReadLineState State
			{
				get
				{
					return this.fState;
				}
				set
				{
					this.fState = value;
				}
			}
			private AsyncReadLineState fState;

			public MemoryStream Data
			{
				get
				{
					return this.fData;
				}
			}
			private readonly MemoryStream fData;

			public AsyncCallback Callback
			{
				get
				{
					return this.fCallback;
				}
			}
			private readonly AsyncCallback fCallback;
			#endregion
		}

		private void IntReadLineCallback(IAsyncResult ar)
		{
			AsyncReadLineRequest lRequest = (AsyncReadLineRequest)ar.AsyncState;
			try
			{
				Int32 lCount = IntEndRead(ar);
				if (lCount == 0)
				{
					TriggerAsyncDisconnect();
					return;
				}

				for (Int32 i = 0; i < lCount; i++)
				{
					if (fBuffer[i] != 10)
						continue;

					if (i > 0 && fBuffer[i - 1] == 13)
						lRequest.Data.Write(fBuffer, 0, i - 1);
					else
						lRequest.Data.Write(fBuffer, 0, i);

					fBufferStart = i + 1;
					fBufferEnd = lCount;
					lRequest.State = AsyncReadLineState.Done;
					lRequest.Callback(lRequest);

					return;
				}

				if (this.MaxLineLengthEnabled && (lRequest.Data.Length > this.MaxLineLength))
				{
					fBufferStart = 0;
					fBufferEnd = lCount;
					lRequest.State = AsyncReadLineState.MaxLineLengthReached;
					lRequest.Callback(lRequest);

					return;
				}

				lRequest.Data.Write(fBuffer, 0, lCount);
				TriggerAsyncHaveIncompleteData();
				IntBeginRead(fBuffer, 0, fBuffer.Length, new AsyncCallback(IntReadLineCallback), lRequest);
			}
			catch (ObjectDisposedException) // disconnect from this side
			{
				TriggerAsyncDisconnect();
			}
			catch (SocketException) // disconnect
			{
				TriggerAsyncDisconnect();
			}
			catch (ArgumentException)
			{
			}
			catch (Exception) // different platforms throw different exceptions
			{
				TriggerAsyncDisconnect();
			}
		}

		public String EndReadLine(IAsyncResult ar)
		{
			AsyncReadLineRequest lRequest = (AsyncReadLineRequest)ar;
			try
			{
				if (lRequest.State == AsyncReadLineState.MaxLineLengthReached)
				{
					TriggerAsyncDisconnect();
					Disconnect();
					throw new ConnectionClosedException("Size limit for ReadLine() was exceeded.");
				}

				Byte[] lBuffer = lRequest.Data.GetBuffer();
				if (lRequest.Data.Length > 0 && lBuffer[lRequest.Data.Length - 1] == 13)
					return Encoding.GetString(lBuffer, 0, (Int32)lRequest.Data.Length - 1); // on the rare occasion that the split is between the #13 and the #10

				return Encoding.GetString(lBuffer, 0, (Int32)lRequest.Data.Length);
			}
			finally
			{
				lRequest.Data.Close();
			}
		}

		public override IAsyncResult BeginRead(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{
			if (!this.BufferedAsync)
				return IntBeginRead(buffer, offset, count, callback, state);

			AsyncRequest lRequest = new AsyncRequest();
			lRequest.AsyncBuffer = buffer;
			lRequest.AsyncOffset = offset;
			lRequest.AsyncCount = count;
			lRequest.AsyncRest = count;
			lRequest.AsyncCallback = callback;
			lRequest.AsyncState = state;

			try
			{
				if (fBuffer != null && fBufferEnd - fBufferStart > 0)
				{
					IAsyncResult lAr = new BufferAsyncResult(buffer, offset, count, lRequest);
					IntReadCallback(lAr);

					return lAr;
				}

				IntBeginRead(buffer, offset, count, new AsyncCallback(IntReadCallback), lRequest);
			}
			catch (ObjectDisposedException) // disconnect from this side
			{
				TriggerAsyncDisconnect();
				throw;
			}
			catch (SocketException) // disconnect
			{
				TriggerAsyncDisconnect();
				throw;
			}

			return lRequest;
		}

		private void IntReadCallback(IAsyncResult ar)
		{
			AsyncRequest lRequest = (AsyncRequest)ar.AsyncState;
			Int32 lCount;

			try
			{
				BufferAsyncResult lBufferResult = ar as BufferAsyncResult;
				if (lBufferResult != null)
				{
					if (fBufferEnd - fBufferStart > lBufferResult.fCount)
					{
						for (Int32 i = 0; i < lBufferResult.fCount; i++)
							lBufferResult.fBuffer[lBufferResult.fOffset + i] = fBuffer[i + fBufferStart];

						fBufferStart += lBufferResult.fCount;

						lCount = lBufferResult.fCount;
					}
					else
					{
						Int32 lSize = fBufferEnd - fBufferStart;
						for (Int32 i = 0; i < lSize; i++)
							lBufferResult.fBuffer[lBufferResult.fOffset + i] = fBuffer[i + fBufferStart];

						fBufferStart = 0;
						fBufferEnd = 0;

						lCount = lSize;
					}
				}
				else
				{
					lCount = IntEndRead(ar);
				}
			}
			catch (ObjectDisposedException) // disconnect from this side
			{
				TriggerAsyncDisconnect();
				return;
			}
			catch (SocketException) // disconnect
			{
				TriggerAsyncDisconnect();
				return;
			}
			catch (ArgumentException) // disconnect
			{
				return;
			}

			if (lCount == 0)
			{
				TriggerAsyncDisconnect();
				return;
			}

			lRequest.AsyncRest = lRequest.AsyncRest - lCount;
			if (lRequest.AsyncRest > 0)
			{
				lRequest.AsyncOffset = lRequest.AsyncOffset + lCount;
				try
				{
					TriggerAsyncHaveIncompleteData();
					IntBeginRead(lRequest.AsyncBuffer, lRequest.AsyncOffset, lRequest.AsyncRest, new AsyncCallback(IntReadCallback), lRequest);
				}
				catch (ObjectDisposedException) // disconnect from this side
				{
					TriggerAsyncDisconnect();
				}
				catch (SocketException) // disconnect
				{
					TriggerAsyncDisconnect();
				}
			}
			else
			{
				lRequest.AsyncCallback(lRequest);
			}
		}

		public override Int32 EndRead(IAsyncResult ar)
		{
			if (this.BufferedAsync)
				return ((AsyncRequest)ar).AsyncCount - ((AsyncRequest)ar).AsyncRest;

			return IntEndRead(ar);
		}

		public override IAsyncResult BeginWrite(Byte[] buffer, Int32 offset, Int32 count, AsyncCallback callback, Object state)
		{
			try
			{
				return IntBeginWrite(buffer, offset, count, callback, state);
			}
			catch (SocketException) // disconnect
			{
				TriggerAsyncDisconnect();
				throw;
			}
		}

		public override void EndWrite(IAsyncResult ar)
		{
			try
			{
				IntEndWrite(ar);
			}
			catch (SocketException) // disconnect
			{
				TriggerAsyncDisconnect();
				throw;
			}
			catch (ObjectDisposedException)
			{
				TriggerAsyncDisconnect();
				throw;
			}
		}

		public virtual IAsyncResult BeginConnect(EndPoint endPoint, AsyncCallback callback, Object state)
		{
			return fDataSocket.BeginConnect(endPoint, callback, state);
		}

		public virtual IAsyncResult BeginConnect(IPAddress address, Int32 port, AsyncCallback callback, Object state)
		{
			return fDataSocket.BeginConnect(new IPEndPoint(address, port), callback, state);
		}

		public virtual void EndConnect(IAsyncResult ar)
		{
			fDataSocket.EndConnect(ar);
		}
		#endregion

		#region ReadLine/WriteLine
		private Byte[] fBuffer;
		private Int32 fBufferStart;
		private Int32 fBufferEnd;

		public virtual String ReadLine()
		{
			Boolean lDone = false;
			String lResult = "";

			while (!lDone)
			{
				if (fBuffer == null)
				{
					fBuffer = new Byte[READLINE_BUFFER_SIZE];
					fBufferStart = 0;
					fBufferEnd = DataSocketReceiveWhatsAvaiable(fBuffer, 0, fBuffer.Length);

					if (fBufferEnd == 0)
						throw new ConnectionClosedException();
				}

				Int32 i = fBufferStart;
				while (i < fBufferEnd && !lDone)
				{

					if (fBuffer[i] == 10)
						lDone = true;

					i++;
				}

				if (lDone)
				{
					if ((i - 2 >= fBufferStart) && fBuffer[i - 2] == 13)
					{
						// if last character is 13, of yes we have a Windows style CRLF line end and must discard the CR, too
						lResult = lResult + Encoding.GetString(fBuffer, fBufferStart, i - fBufferStart - 2);
					}
					else
					{
						// else just discard the 10 (LF)
						lResult = lResult + Encoding.GetString(fBuffer, fBufferStart, i - fBufferStart - 1);
					}
				}
				else
				{
					lResult = lResult + Encoding.GetString(fBuffer, fBufferStart, i - fBufferStart);
				}

				if (lDone)
				{
					/* Keep remaining buffer for future reads. */
					fBufferStart = i;

					if (fBufferStart == fBufferEnd)
						fBuffer = null;
				}
				else
				{
					fBuffer = null;
				}

				// Enforce Line Length checking
				if (MaxLineLengthEnabled && lResult.Length > MaxLineLength)
				{
					Disconnect();
					throw new ConnectionClosedException("Size limit for ReadLine() was exceeded.");
				}
			}

			return lResult;
		}

		public readonly Byte[] CRLF = { 13, 10 };
		public readonly Byte[] LF = { 10 };

		public virtual void WriteLine(String line, params Object[] args)
		{
			WriteLine(String.Format(line, args));
		}

		public void WriteLineLF(String line, params Object[] args)
		{
			this.WriteLineLF(String.Format(line, args));
		}

		public virtual void WriteLine(String line)
		{
			Byte[] lBytes = Encoding.GetBytes(line);

			if (lBytes.Length > 0)
				DataSocketSend(lBytes, 0, lBytes.Length);

			DataSocketSend(CRLF, 0, 2);
		}

		public virtual void WriteLineLF(String line)
		{
			Byte[] lBytes = Encoding.GetBytes(line);

			if (lBytes.Length > 0)
				DataSocketSend(lBytes, 0, lBytes.Length);
			DataSocketSend(LF, 0, 1);
		}
		#endregion

		#region Connection Pooling properties
		public DateTime LastUsed
		{
			get
			{
				return fLastUsed;
			}
			set
			{
				fLastUsed = value;
			}
		}
		private DateTime fLastUsed;

		public ConnectionPool Pool
		{
			get
			{
				return fPool;
			}
			set
			{
				fPool = value;
			}
		}
		private ConnectionPool fPool;
		#endregion

		#region IDisposable Members
#if FULLFRAMEWORK
		public new void Dispose()
#endif
#if COMPACTFRAMEWORK
        public void Dispose()
#endif
		{
			if (this.Connected)
			{
				this.Disconnect();
			}

			if (this.fTimeoutTimer != null)
			{
				this.fTimeoutTimer.Dispose();
				this.fTimeoutTimer = null;
			}
		}
		#endregion

		#region Statistics
		public void ResetStatistics()
		{
			fBytesSent = 0;

			/* if we have still bytes in the buffer, we'll still want to count then against future "received" counts */
			if (fBuffer != null)
				fBytesReceived = fBufferEnd - fBufferStart;
			else
				fBytesReceived = 0;
		}

		public Int64 BytesSent
		{
			get
			{
				return fBytesSent;
			}
		}
		private Int64 fBytesSent;

		public Int64 BytesReceived
		{
			get
			{
				return fBytesReceived;
			}
		}
		private Int64 fBytesReceived;

		public event EventHandler OnBytesSent;

		public event EventHandler OnBytesReceived;

		protected virtual void TriggerOnBytesSent(Int64 count)
		{
			fBytesSent += count;
			if (OnBytesSent != null)
				OnBytesSent(this, EventArgs.Empty);
		}
		protected virtual void TriggerOnBytesReceived(Int64 count)
		{
			fBytesReceived += count;
			if (OnBytesReceived != null)
				OnBytesReceived(this, EventArgs.Empty);
		}
		#endregion
	}
}