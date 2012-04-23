/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemObjects.InternetPack
{
    public interface IConnectionTimeouts
    {
        Int32 Timeout { get; set; }
        Boolean TimeoutEnabled { get; set; }
    }

    public class Connection : Stream, IDisposable, IConnectionTimeouts
    {
        public Connection(Socket socket)
        {
            this.fDataSocket = socket;

            if (this.fDataSocket != null && !EnableNagle)
                this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

            this.Initialize();

            this.DataSocketInitializeServerConnection();
        }

        public Connection(Binding binding)
        {
            this.Init(binding);
            this.Initialize();
        }

        private const Int32 READLINE_BUFFER_SIZE = 1024;
        private const Int32 BUFFER_SIZE = 1024;//64 * 1024;

        private Boolean fTimeoutTimerEnabled;
        private Boolean fBufferedAsync = true;
        private System.Threading.Timer fTimeoutTimer;

        public void Init(Socket socket)
        {
            this.fDataSocket = socket;

            if (!this.EnableNagle)
                this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);

            this.Initialize();
            this.DataSocketInitializeServerConnection();
        }

        public void Init(Binding binding)
        {
            this.fBinding = binding;
            this.fDataSocket = new Socket(binding.AddressFamily, binding.SocketType, binding.Protocol);

            if (!this.EnableNagle)
                this.fDataSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, 1);
        }

        protected virtual void Initialize()
        {
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
#if MONOANDROID
                if (this.fEncoding == null) 
                    this.fEncoding = Encoding.UTF8;
#else
                if (this.fEncoding == null)
                    this.fEncoding = Encoding.Default;
#endif
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
        private Boolean fEnableNagle = false;

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
                    this.fTimeoutTimer.Change(this.fTimeout * 1000, this.fTimeout * 1000);
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

        public Boolean BufferedAsync
        {
            get
            {
                return this.fBufferedAsync;
            }
            set
            {
                this.fBufferedAsync = value;
            }
        }

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
                    this.fTimeoutTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.TimeoutElapsed), null, this.Timeout * 1000, this.Timeout * 1000);
                }
                else if (value && this.fTimeoutTimer != null)
                {
                    this.fTimedOut = false;
                    this.fTimeoutTimer.Dispose();
                    this.fTimeoutTimer = null;
                }
            }
        }

        public Int32 MaxLineLength
        {
            get
            {
                return fMaxLineLength;
            }
            set
            {
                fMaxLineLength = value;
            }
        }
        private Int32 fMaxLineLength;

        public Boolean MaxLineLengthEnabled
        {
            get
            {
                return fMaxLineLengthEnabled;
            }
            set
            {
                fMaxLineLengthEnabled = value;
            }
        }
        private Boolean fMaxLineLengthEnabled;
        #endregion

        #region Timeouts & Limits
        private void TimeoutElapsed(Object sender)
        {
            if (this.fTimeoutTimerEnabled)
            {
                this.fTimedOut = true;
                this.Abort();
            }
        }

        protected void StartTimeoutTimer()
        {
            if (this.TimeoutEnabled)
            {
                this.fTimeoutTimerEnabled = true;
                this.fTimedOut = false;
                this.fTimeoutTimer.Change(this.Timeout * 1000, this.Timeout * 1000);
            }
        }

        protected void StopTimeoutTimer()
        {
            if (this.TimeoutEnabled)
            {
                this.fTimeoutTimerEnabled = false;
                this.fTimeoutTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
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
            StartTimeoutTimer();
            try
            {
                Int32 lReadBytes = DataSocket.Receive(buffer, offset, size, SocketFlags.None);

                if (lReadBytes == 0)
                    DataSocket.Close();

                TriggerOnBytesReceived(lReadBytes);
                return lReadBytes;
            }
            catch (ObjectDisposedException)
            {
                DataSocket.Close();
                return 0;
            }
            catch (SocketException)
            {
                DataSocket.Close();
                return 0;
            }
            finally
            {
                StopTimeoutTimer();
            }
        }

        protected virtual Int32 DataSocketSendAsMuchAsPossible(Byte[] buffer, Int32 offset, Int32 size)
        {
            Int32 lSentBytes = DataSocket.Send(buffer, offset, size, SocketFlags.None);

            TriggerOnBytesSent(lSentBytes);

            return lSentBytes;
        }

        protected virtual void DataSocketConnect(EndPoint endPoint)
        {
            fOriginalEndpoint = endPoint;
            DataSocket.Connect(endPoint);
        }

        protected virtual void DataSocketInitializeServerConnection()
        {
        }

        public void Abort()
        {
            if (DataSocket.Connected)
            {
                DataSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(false, 0));
                DataSocket.Close();
            }
        }

        protected virtual void DataSocketClose()
        {
            if (DataSocket.Connected)
            {
                try
                {
                    DataSocket.Shutdown(SocketShutdown.Both);
                }
                catch (ObjectDisposedException)
                {
                }
                catch (SocketException)
                {
                }
                DataSocket.Close();
            }
        }

        protected virtual void DataSocketClose(Boolean dispose)
        {
            try
            {
                if (DataSocket.Connected)
                    DataSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
            }

            if (dispose)
                DataSocket.Close();
        }

        protected Int32 DataSocketReceive(Byte[] buffer, Int32 offset, Int32 size)
        {
            Int32 lTotalReadBytes = 0;

            do
            {
                Int32 lReadBytes = DataSocketReceiveWhatsAvaiable(buffer, offset, size);
                if (lReadBytes == 0)
                    return lTotalReadBytes;

                size -= lReadBytes;
                offset += lReadBytes;
                lTotalReadBytes += lReadBytes;
            }
            while (size > 0);

            return lTotalReadBytes;
        }

        protected Int32 DataSocketSend(Byte[] buffer, Int32 offset, Int32 size)
        {
            Int32 lTotalSentBytes = 0;

            while (size > 0)
            {
                Int32 lSentBytes = DataSocketSendAsMuchAsPossible(buffer, offset, size);
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

            if (fBuffer != null)
            {
                // still bytes in buffer? if yes, get them first
                Int32 lSize = fBufferEnd - fBufferStart;

                if (lSize > size)
                {
                    // more bytes in buffer than we need?
                    Array.Copy(fBuffer, fBufferStart, buffer, offset, size);
                    fBufferStart += size;

                    return size;
                }
                else
                {
                    // less (or same) number of bytes in buffer then we need?
                    Buffer.BlockCopy(fBuffer, fBufferStart, buffer, offset, lSize);
                    fBuffer = null;

                    if (size > lSize && (DataSocketAvailable > 0 || block))
                    {
                        // if more bytes werer requested, and bytes are available, get them
                        return lSize + DataSocketReceive(buffer, offset + lSize, size - lSize);
                    }

                    return lSize;
                }
            }
            else
            {
                /* otherwise, get data directly from socket */
                if (block)
                    return DataSocketReceive(buffer, offset, size);

                return DataSocketReceiveWhatsAvaiable(buffer, offset, size);
            }
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
                    if (this.fActualSize == -1)
                        return this.fBuffer.Length;

                    return this.fActualSize;
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

            Int32 lRead = 0;
            Int32 lTotal = 0;

            Boolean lDone;
            do
            {
                lDone = false;

                Int32 lBytesToRead = bufferSize;

                DataBlock lBuffer = new DataBlock(lBytesToRead);

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
            Int32 lRead = 0;
            Int32 lTotal = 0;

            Boolean lDone;
            do
            {
                lDone = false;

                Int64 lBytesToRead = bufferSize;
                if (size > -1 && size - lTotal < bufferSize)
                    lBytesToRead = size - lTotal;

                Byte[] lBuffer = new Byte[lBytesToRead];

                lRead = Receive(lBuffer, 0, (Int32)lBytesToRead);

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

            Int32 lSent = 0;
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

                lSent = Send(lBuffer, 0, (Int32)lBytesToSend);

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

        public virtual void SkipBytes(Int32 aSize)
        {
            if (aSize <= 0)
                return;

            Byte[] lBuffer = new Byte[BUFFER_SIZE];
            while (aSize > 0)
            {
                // lBytesRead can be 0 if connection is closed
                Int32 lBytesRead = Read(lBuffer, 0, aSize > BUFFER_SIZE ? BUFFER_SIZE : aSize);

                if (lBytesRead == 0)
                    break;

                aSize -= lBytesRead;
            }
        }
        #endregion

        #region System.IO.Stream Methods
        public override void Flush()
        {
        }

        public override void Close()
        {
            this.DataSocketClose();
            base.Close();

            if (fTimeoutTimer != null)
            {
                fTimeoutTimer.Dispose();
                fTimeoutTimer = null;
            }
        }

        public void Close(Boolean dispose)
        {
            DataSocketClose(dispose);

            if (dispose)
                base.Close();
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

        internal protected virtual void TriggerAsyncHaveIncompleteData()
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
            public readonly Object fAsyncState;

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
            if (fBuffer != null && fBufferEnd - fBufferStart > 0)
            {
                IAsyncResult lAr = new BufferAsyncResult(buffer, offset, count, state);
                callback(lAr);

                return lAr;
            }

            return fDataSocket.BeginReceive(buffer, offset, count, SocketFlags.None, callback, state);
        }

        protected virtual Int32 IntEndRead(IAsyncResult ar)
        {
            if (ar is BufferAsyncResult)
            {
                BufferAsyncResult lBufferResult = (BufferAsyncResult)ar;
                if (fBufferEnd - fBufferStart > lBufferResult.fCount)
                {
                    for (Int32 i = 0; i < lBufferResult.fCount; i++)
                        lBufferResult.fBuffer[lBufferResult.fOffset + i] = fBuffer[i + fBufferStart];

                    fBufferStart += lBufferResult.fCount;

                    return lBufferResult.fCount;
                }
                else
                {
                    Int32 lSize = fBufferEnd - fBufferStart;
                    for (Int32 i = 0; i < lSize; i++)
                        lBufferResult.fBuffer[lBufferResult.fOffset + i] = fBuffer[i + fBufferStart];

                    fBufferStart = 0;
                    fBufferEnd = 0;

                    return lSize;
                }
            }

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
                    if (fBuffer[i] == 10)
                    {
                        Int32 lLen = i - fBufferStart;
                        if (lLen > 0 && fBuffer[i - 1] == 13)
                            lLen--;

                        String lData = Encoding.GetString(fBuffer, fBufferStart, lLen);
                        fBufferStart = i + 1;

                        return lData;
                    }
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
                    if (fBuffer[i] == 10)
                    {
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
            Start,
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
            Int32 lCount;
            try
            {
                lCount = IntEndRead(ar);
                if (lCount == 0)
                {
                    TriggerAsyncDisconnect();
                    return;
                }

                for (Int32 i = 0; i < lCount; i++)
                {
                    if (fBuffer[i] == 10)
                    {
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
                }

                if (lRequest.Data.Length > fMaxLineLength && fMaxLineLengthEnabled)
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
                return;
            }
            catch (SocketException) // disconnect
            {
                TriggerAsyncDisconnect();
                return;
            }
            catch (ArgumentException)
            {
                return;
            }
            catch (Exception) // different platforms throw different exceptions
            {
                TriggerAsyncDisconnect();
                return;
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
            if (!fBufferedAsync)
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
                lCount = IntEndRead(ar);
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
                    return;
                }
                catch (SocketException) // disconnect
                {
                    TriggerAsyncDisconnect();
                    return;
                }
            }
            else
            {
                lRequest.AsyncCallback(lRequest);
            }
        }

        public override Int32 EndRead(IAsyncResult ar)
        {
            if (fBufferedAsync)
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

        public Byte[] CRLF = { 13, 10 };
        public Byte[] LF = { 10 };

        public virtual void WriteLine(String line, params Object[] args)
        {
            WriteLine(String.Format(line, args));
        }

        public virtual void WriteLineLF(String aLine, params Object[] args)
        {
            WriteLineLF(String.Format(aLine, args));
        }

        public virtual void WriteLine(String line)
        {
            Byte[] lBytes = Encoding.GetBytes(line);

            if (lBytes != null && lBytes.Length > 0)
                DataSocketSend(lBytes, 0, lBytes.Length);

            DataSocketSend(CRLF, 0, 2);
        }

        public virtual void WriteLineLF(String line)
        {
            Byte[] lBytes = Encoding.GetBytes(line);

            if (lBytes != null && lBytes.Length > 0)
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
                this.Disconnect();

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

        protected void TriggerOnBytesSent(Int64 count)
        {
            fBytesSent += count;
            if (OnBytesSent != null)
                OnBytesSent(this, EventArgs.Empty);
        }
        protected void TriggerOnBytesReceived(Int64 count)
        {
            fBytesReceived += count;
            if (OnBytesReceived != null)
                OnBytesReceived(this, EventArgs.Empty);
        }
        #endregion
    }

    #region Delegates
    public delegate void ConnectionEventHandler(Object sender, ConnectionEventArgs e);

    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionEventArgs(Connection connection)
            : base()
        {
            this.fConnection = connection;
        }

        public Connection Connection
        {
            get
            {
                return this.fConnection;
            }
        }
        private readonly Connection fConnection;
    }
    #endregion

    [Serializable]
    public class ConnectionClosedException : System.Exception
    {
        public ConnectionClosedException()
            : base("Connection was closed.")
        {
        }

        public ConnectionClosedException(Boolean timeout)
            : base(timeout ? "Timeout executing operation" : "Connection was closed.")
        {
            this.fTimeout = timeout;
        }

        public ConnectionClosedException(String message)
            : base("Connection was closed; " + message)
        {
        }

        public ConnectionClosedException(Exception innerException)
            : base("Connection was closed.", innerException)
        {
        }

        public Boolean Timeout
        {
            get
            {
                return fTimeout;
            }
        }
        private readonly Boolean fTimeout;
    }
}