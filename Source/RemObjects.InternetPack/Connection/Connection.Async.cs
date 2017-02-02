/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack
{
	public partial class Connection
	{
		#if ECHOES
		private class AsyncRequest : IAsyncResult
		{
			public Byte[] AsyncBuffer { get; set; }

			public Int32 AsyncOffset { get; set; }

			public Int32 AsyncCount { get; set; }

			public AsyncCallback AsyncCallback { get; set; }

			public Object AsyncState { get; set; }

			public Int32 AsyncRest { get; set; }

			Object IAsyncResult.AsyncState
			{
				get
				{
					return AsyncState;
				}
			}

			public Boolean CompletedSynchronously
			{
				get
				{
					return false;
				}
			}

			public WaitHandle AsyncWaitHandle
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
					return this.AsyncRest == 0;
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

		protected internal virtual void TriggerAsyncDisconnect()
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
				Buffer = buffer;
				Offset = offset;
				Count = count;
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

			public WaitHandle AsyncWaitHandle
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

			public Byte[] Buffer { get; set; }

			public Int32 Offset { get; set; }

			public Int32 Count { get; set; }
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
				IntBeginRead(fBuffer, 0, fBuffer.Length, IntReadLineCallback, lRequest);
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

			public WaitHandle AsyncWaitHandle
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
				IntBeginRead(fBuffer, 0, fBuffer.Length, IntReadLineCallback, lRequest);
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

				IntBeginRead(buffer, offset, count, IntReadCallback, lRequest);
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
					if (fBufferEnd - fBufferStart > lBufferResult.Count)
					{
						for (Int32 i = 0; i < lBufferResult.Count; i++)
							lBufferResult.Buffer[lBufferResult.Offset + i] = fBuffer[i + fBufferStart];

						fBufferStart += lBufferResult.Count;

						lCount = lBufferResult.Count;
					}
					else
					{
						Int32 lSize = fBufferEnd - fBufferStart;
						for (Int32 i = 0; i < lSize; i++)
							lBufferResult.Buffer[lBufferResult.Offset + i] = fBuffer[i + fBufferStart];

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
					IntBeginRead(lRequest.AsyncBuffer, lRequest.AsyncOffset, lRequest.AsyncRest, IntReadCallback, lRequest);
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
			return this.BufferedAsync ? ((AsyncRequest)ar).AsyncCount - ((AsyncRequest)ar).AsyncRest : IntEndRead(ar);
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
		#endif
	}
}