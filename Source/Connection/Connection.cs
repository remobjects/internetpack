/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public partial class Connection : Stream, IDisposable, IConnectionTimeouts
	{
		#region Private constants
		private const Int32 READLINE_BUFFER_SIZE = 1024;
		private const Int32 BUFFER_SIZE = 1024;//64 * 1024;
		internal const Int32 DEFAULT_TIMEOUT = 5 * 60; /* 5 minutes */
		internal const Int32 DEFAULT_MAX_LINE_LENGTH = 4096;
		#endregion

		#region Private fields
		#if toffee || cooper
		private readonly Object fSyncRoot = new Object();
		#else
		private readonly Monitor fSyncRoot = new Monitor();
		#endif
		private Boolean fTimeoutTimerEnabled;
		private Timer fTimeoutTimer;
		#endregion

		public Connection(Socket socket)
		{
			this.fDataSocket = socket;

			if (this.fDataSocket != null)
			{
				this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
			}

			this.SetDefaultValues();
		}

		public Connection(Binding binding)
		{
			this.Init(binding);

			this.SetDefaultValues();
		}

		public void Init(Socket socket)
		{
			this.fDataSocket = socket;

			if (!this.EnableNagle)
			{
				this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
			}

			this.SetDefaultValues();
		}

		private void Init(Binding binding)
		{
			this.fBinding = binding;
			this.fDataSocket = new Socket(binding.AddressFamily, binding.SocketType, binding.Protocol);

			if (!this.EnableNagle)
			{
				this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
			}
		}

		private void SetDefaultValues()
		{
			this.fEnableNagle = false;
			this.BufferedAsync = true;
			this.Timeout = DEFAULT_TIMEOUT;
			this.MaxLineLength = DEFAULT_MAX_LINE_LENGTH;
		}

		[ToString]
		public override String ToString()
		{
			#if toffee || cooper
			return String.Format("{0} Local: {1} Remote {2}", this.ToString(), this.LocalEndPoint, this.RemoteEndPoint);
			#else
			return String.Format("{0} Local: {1} Remote {2}", this.GetType().Name, this.LocalEndPoint, this.RemoteEndPoint);
			#endif
		}

		#region Properties
		public Binding Binding
		{
			get
			{
				return this.fBinding ?? new Binding(DataSocket.AddressFamily);
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

		public RemObjects.Elements.RTL.Encoding Encoding
		{
			get
			{
				if (this.fEncoding == null)
					return RemObjects.Elements.RTL.Encoding.Default;
				else
					return this.fEncoding;
				//return this.fEncoding ?? (this.fEncoding = Encoding.Default);
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
					this.fTimeoutTimer.Change(this.Timeout * 1000, this.Timeout * 1000);
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
					this.fTimeoutTimer = new Timer(this.TimeoutElapsed, null, -1/*Timeout.Infinite*/, -1/*Timeout.Infinite*/);
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
			this.fTimeoutTimer.Change(this.Timeout * 1000, -1/*Timeout.Infinite*/);
		}

		protected void StopTimeoutTimer()
		{
			if (!this.TimeoutEnabled)
			{
				return;
			}

			this.fTimeoutTimerEnabled = false;
			this.fTimeoutTimer.Change(-1/*Timeout.Infinite*/, -1/*Timeout.Infinite*/);
		}
		#endregion

		#region DataSocket access
		public virtual Socket DataSocket
		{
			get
			{
				return fDataSocket;
			}
		}
		private Socket fDataSocket;

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

		public EndPoint OriginalEndpoint { get; protected set; }

		protected internal virtual void InitializeServerConnection()
		{
		}

		protected internal virtual IAsyncResult BeginInitializeServerConnection(AsyncCallback callback, Object state)
		{
			return null;
		}

		protected internal virtual void EndInitializeServerConnection(IAsyncResult ar)
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
			this.OriginalEndpoint = endPoint;
			this.DataSocket.Connect(endPoint);
		}

		private void Abort()
		{
			// On older Mono versions a racing condition can arise here
			lock (fSyncRoot)
			{
				if (!this.DataSocket.Connected)
				{
					return;
				}

				try
				{
					this.DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(false, 0));
				}
				catch (Exception)
				{
					// Ignore anything. This connection is dead anyway
				}

				this.DataSocketClose();
			}
		}

		protected virtual void DataSocketClose()
		{
			// On older Mono versions a racing condition can arise here
			lock (fSyncRoot)
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
		}

		protected virtual void DataSocketClose(Boolean dispose)
		{
			// On older Mono versions a racing condition can arise here
			lock (fSyncRoot)
			{
				try
				{
					if (this.DataSocket.Connected)
					{
						this.DataSocket.Shutdown(SocketShutdown.Both);
					}
				}
				catch (Exception)
				{
					// Suppress any exceptions
				}

				if (dispose)
				{
					this.DataSocket.Close();
				}
			}
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
			//fBuffer.BlockCopy(fBuffer, fBufferStart, buffer, offset, lSize);
			#if toffee || cooper
			Array.Copy(fBuffer, fBufferStart, buffer, offset, lSize);
			#else
			fBuffer.Copy(fBuffer, fBufferStart, buffer, offset, lSize);
			#endif
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

			List<DataBlock> lBlocks = new List<DataBlock>();

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
					//Buffer.BlockCopy(lData.Buffer, 0, lNewBuffer, lCurrent, lData.ActualSize);
					Array.Copy(lData.Buffer, 0, lNewBuffer, lCurrent, lData.ActualSize);
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

/*
		#if ECHOES
		public override Int32 Read(Byte[] buffer, Int32 offset, Int32 size)
		{
			return Receive(buffer, offset, size, true);
		}

		public override void Write(Byte[] buffer, Int32 offset, Int32 size)
		{
			DataSocketSend(buffer, offset, size);
		}
		#else if ISLAND

		/*public override bool IsValid()
		{
			return Connected;
		}
		public override Int32 Read(void *buf, Int32 Count)
		{
			#warning Implement for Island
		}
		public override Int32 Write(void *buf, Int32 Count)
		{
			#warning Implement for Island
		}*/

		public override Int32 Read(Byte[] buffer, Int32 offset, Int32 count)
		{
			return Receive(buffer, offset, count, true);
		}

		public override Int32 Write(Byte[] buffer, Int32 offset, Int32 count)
		{
			 return DataSocketSend(buffer, offset, count);
		}

		//#endif

		public override Int64 Seek(Int64 offset, SeekOrigin origin)
		{
			#if toffee || cooper
			throw new Exception(String.Format("{0} does not support seeking", this.ToString()));
			#else
			throw new Exception(String.Format("{0} does not support seeking", this.GetType().Name));
			#endif
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

		/*public override void SetLength(Int64 length)
		{
			throw new Exception(String.Format("{0} does not support SetLength", this.GetType().Name));
		}*/

		#if !cooper
		public override Int64 GetLength()
		{
			return Position + DataSocketAvailable;
		}

		public override Int64 GetPosition()
		{
			return Position;
		}
		#endif

		public override Int64 Position
		{
			get
			{
				return base.Position;
			}
			set
			{
				Seek(value, SeekOrigin.Begin);
				Position = value;
			}
		}

		#if !toffee && !cooper
		public override void SetPosition(Int64 value)
		{
			Seek(value, SeekOrigin.Begin);
			Position = value;
		}
		#endif
		#endregion

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
					String ToWrite = "";
					for(int j = 0; j < fBufferEnd;j++)
							ToWrite = ToWrite + chr(fBuffer[j]);

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
		#else
		public void Dispose()
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
		#endif
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