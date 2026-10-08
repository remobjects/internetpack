/*---------------------------------------------------------------------------
  RemObjects Internet Pack
  (c)opyright RemObjects Software, LLC. 2003-2026. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.Elements.RTL;

namespace RemObjects.InternetPack.WebSocket
{
	public enum WebSocketOpcode
	{
		Continuation = 0x0,
		Text = 0x1,
		Binary = 0x2,
		Close = 0x8,
		Ping = 0x9,
		Pong = 0xA
	}

	public class WebSocketMessage
	{
		public WebSocketMessage(WebSocketOpcode opcode, Byte[] payload)
		{
			this.Opcode = opcode;
			this.Payload = payload ?? new Byte[0];
		}

		public WebSocketMessage(WebSocketOpcode opcode, Byte[] payload, Boolean isFinal) : this(opcode, payload)
		{
			this.IsFinal = isFinal;
		}

		public Boolean IsFinal { get; private set; } = true;

		public WebSocketOpcode Opcode { get; private set; }

		public Byte[] Payload { get; private set; }

		public String Text
		{
			get
			{
				if (this.Opcode != WebSocketOpcode.Text)
					return null;

				return Encoding.UTF8.GetString(this.Payload, 0, this.Payload.Length);
			}
		}
	}

	public class WebSocketConnection : IDisposable
	{
		public WebSocketConnection(Connection connection, Boolean maskOutgoingFrames)
		{
			if (connection == null)
				throw new ArgumentNullException("connection");

			this.fConnection = connection;
			this.fMaskOutgoingFrames = maskOutgoingFrames;
		}

		internal WebSocketConnection(IWebSocketTransport transport)
		{
			this.fTransport = transport;
		}

		private readonly IWebSocketTransport fTransport;
		private readonly Connection fConnection;
		private readonly Boolean fMaskOutgoingFrames;
		private Boolean fClosed;
		#if toffee || cooper
		private readonly Object fSendGate = new Object();
		#else
		private readonly Monitor fSendGate = new Monitor();
		#endif
		private readonly RemObjects.Elements.RTL.List<Byte[]> fFragments = new RemObjects.Elements.RTL.List<Byte[]>();
		private WebSocketOpcode fFragmentOpcode;
		private Int32 fFragmentLength;
		private Boolean fReadingFragments;
		private readonly Random fRandom = new Random();

		public Connection Connection
		{
			get
			{
				return this.fConnection;
			}
		}

		public Boolean Closed
		{
			get
			{
				return this.fClosed || (this.fTransport != null ? this.fTransport.Closed : !this.fConnection.Connected);
			}
		}

		public WebSocketMessage ReadMessage()
		{
			if (this.fTransport != null)
			{
				return this.fTransport.ReadMessage();
			}
			while (!this.Closed)
			{
				WebSocketMessage lMessage = this.ReadFrame();
				switch (lMessage.Opcode)
				{
					case WebSocketOpcode.Ping:
						this.SendFrame(WebSocketOpcode.Pong, lMessage.Payload);
						break;

					case WebSocketOpcode.Pong:
						break;

					case WebSocketOpcode.Close:
						this.fClosed = true;
						if (this.fConnection.Connected)
							this.SendFrame(WebSocketOpcode.Close, lMessage.Payload);
						return lMessage;

					default:
						if (lMessage.Opcode == WebSocketOpcode.Continuation)
						{
							if (!this.fReadingFragments)
								throw new WebSocketException("Unexpected WebSocket continuation frame.");
						}
						else if (this.fReadingFragments)
						{
							throw new WebSocketException("A fragmented WebSocket message is still incomplete.");
						}
						else if (lMessage.IsFinal)
						{
							return lMessage;
						}
						else
						{
							this.fReadingFragments = true;
							this.fFragmentOpcode = lMessage.Opcode;
						}

						if (lMessage.Payload.Length > 2147483647 - this.fFragmentLength)
							throw new WebSocketException("WebSocket message is too large.");
						this.fFragments.Add(lMessage.Payload);
						this.fFragmentLength += lMessage.Payload.Length;
						if (lMessage.IsFinal)
						{
							var lPayload = new Byte[this.fFragmentLength];
							var lOffset = 0;
							foreach (var lPart in this.fFragments)
							{
								Array.Copy(lPart, 0, lPayload, lOffset, lPart.Length);
								lOffset += lPart.Length;
							}
							this.fFragments.RemoveAll();
							this.fFragmentLength = 0;
							this.fReadingFragments = false;
							return new WebSocketMessage(this.fFragmentOpcode, lPayload);
						}
						break;
				}
			}

			return null;
		}

		public String ReadTextMessage()
		{
			WebSocketMessage lMessage = this.ReadMessage();
			if (lMessage == null)
				return null;

			if (lMessage.Opcode == WebSocketOpcode.Close)
				return null;

			if (lMessage.Opcode != WebSocketOpcode.Text)
				throw new WebSocketException(String.Format("Expected a text WebSocket message, got opcode {0}.", lMessage.Opcode));

			return lMessage.Text;
		}

		public void SendText(String message)
		{
			if (message == null)
				message = "";

			Byte[] lPayload = Encoding.UTF8.GetBytes(message);
			this.SendFrame(WebSocketOpcode.Text, lPayload);
		}

		public void SendBinary(Byte[] payload)
		{
			this.SendFrame(WebSocketOpcode.Binary, payload ?? new Byte[0]);
		}

		public void SendPing(Byte[] payload)
		{
			this.SendFrame(WebSocketOpcode.Ping, payload ?? new Byte[0]);
		}

		public void SendPong(Byte[] payload)
		{
			this.SendFrame(WebSocketOpcode.Pong, payload ?? new Byte[0]);
		}

		public void Close()
		{
			if (this.fClosed)
				return;

			this.fClosed = true;
			if (this.fTransport != null)
			{
				this.fTransport.Close();
				return;
			}
			if (this.fConnection.Connected)
				this.SendFrame(WebSocketOpcode.Close, new Byte[0]);
		}

		public void Dispose()
		{
			this.fClosed = true;
			if (this.fTransport != null)
			{
				this.fTransport.Dispose();
				return;
			}
			if (this.fConnection != null)
				this.fConnection.Dispose();
		}

		private WebSocketMessage ReadFrame()
		{
			Byte lFirst = this.ReadByte();
			Byte lSecond = this.ReadByte();

			Boolean lFinal = (lFirst & 0x80) != 0;
			WebSocketOpcode lOpcode = (WebSocketOpcode)(lFirst & 0x0F);
			Boolean lMasked = (lSecond & 0x80) != 0;
			UInt64 lLength = (UInt64)(lSecond & 0x7F);

			if ((lFirst & 0x70) != 0)
				throw new WebSocketException("Unsupported WebSocket extension bits.");
			if (lOpcode != WebSocketOpcode.Continuation && lOpcode != WebSocketOpcode.Text && lOpcode != WebSocketOpcode.Binary &&
				lOpcode != WebSocketOpcode.Close && lOpcode != WebSocketOpcode.Ping && lOpcode != WebSocketOpcode.Pong)
				throw new WebSocketException("Unsupported WebSocket opcode.");
			if ((Byte)lOpcode >= 8 && (!lFinal || lLength > 125))
				throw new WebSocketException("Invalid WebSocket control frame.");

			if (lLength == 126)
				lLength = this.ReadUInt16NetworkOrder();
			else if (lLength == 127)
				lLength = this.ReadUInt64NetworkOrder();

			if (lLength > 2147483647)
				throw new WebSocketException("WebSocket frame is too large.");

			Byte[] lMask = null;
			if (lMasked)
				lMask = this.ReadBytes(4);

			Byte[] lPayload = this.ReadBytes((Int32)lLength);
			if (lMasked)
				this.ApplyMask(lPayload, lMask);

			return new WebSocketMessage(lOpcode, lPayload, lFinal);
		}

		private void SendFrame(WebSocketOpcode opcode, Byte[] payload)
		{
			// The reader sends control replies while the application may send data.
			// Keep each header and payload together on the shared connection.
			lock (this.fSendGate)
				this.SendFrameCore(opcode, payload);
		}

		private void SendFrameCore(WebSocketOpcode opcode, Byte[] payload)
		{
			if (payload == null)
				payload = new Byte[0];

			if (this.fClosed && opcode != WebSocketOpcode.Close)
				throw new WebSocketException("The WebSocket connection is closed.");

			if (this.fTransport != null)
			{
				this.fTransport.Send(opcode, payload);
				return;
			}

			Int32 lHeaderLength = 2;
			if (payload.Length > 65535)
				lHeaderLength += 8;
			else if (payload.Length >= 126)
				lHeaderLength += 2;

			Byte[] lMask = null;
			if (this.fMaskOutgoingFrames)
			{
				lHeaderLength += 4;
				lMask = this.NewMask();
			}

			Byte[] lHeader = new Byte[lHeaderLength];
			Int32 lOffset = 0;
			lHeader[lOffset++] = (Byte)(0x80 | ((Byte)opcode & 0x0F));

			Byte lMaskBit = this.fMaskOutgoingFrames ? (Byte)0x80 : (Byte)0x00;
			if (payload.Length > 65535)
			{
				lHeader[lOffset++] = (Byte)(lMaskBit | 127);
				this.WriteUInt64NetworkOrder(lHeader, ref lOffset, (UInt64)payload.Length);
			}
			else if (payload.Length >= 126)
			{
				lHeader[lOffset++] = (Byte)(lMaskBit | 126);
				this.WriteUInt16NetworkOrder(lHeader, ref lOffset, (UInt16)payload.Length);
			}
			else
			{
				lHeader[lOffset++] = (Byte)(lMaskBit | payload.Length);
			}

			Byte[] lPayload = payload;
			if (this.fMaskOutgoingFrames)
			{
				Array.Copy(lMask, 0, lHeader, lOffset, 4);
				lOffset += 4;

				lPayload = new Byte[payload.Length];
				Array.Copy(payload, 0, lPayload, 0, payload.Length);
				this.ApplyMask(lPayload, lMask);
			}

			this.fConnection.Send(lHeader);
			if (lPayload.Length > 0)
				this.fConnection.Send(lPayload);
		}

		private Byte ReadByte()
		{
			Byte[] lBuffer = this.ReadBytes(1);
			return lBuffer[0];
		}

		private Byte[] ReadBytes(Int32 count)
		{
			Byte[] lResult = new Byte[count];
			Int32 lOffset = 0;
			while (lOffset < count)
			{
				Int32 lRead = this.fConnection.Receive(lResult, lOffset, count - lOffset);
				if (lRead <= 0)
					throw new ConnectionClosedException();

				lOffset += lRead;
			}

			return lResult;
		}

		private UInt16 ReadUInt16NetworkOrder()
		{
			Byte[] lBytes = this.ReadBytes(2);
			return (UInt16)((lBytes[0] << 8) | lBytes[1]);
		}

		private UInt64 ReadUInt64NetworkOrder()
		{
			Byte[] lBytes = this.ReadBytes(8);
			UInt64 lResult = 0;
			for (Int32 i = 0; i < 8; i++)
				lResult = (lResult << 8) | lBytes[i];
			return lResult;
		}

		private void WriteUInt16NetworkOrder(Byte[] buffer, ref Int32 offset, UInt16 value)
		{
			buffer[offset++] = (Byte)((value >> 8) & 0xFF);
			buffer[offset++] = (Byte)(value & 0xFF);
		}

		private void WriteUInt64NetworkOrder(Byte[] buffer, ref Int32 offset, UInt64 value)
		{
			for (Int32 i = 7; i >= 0; i--)
				buffer[offset++] = (Byte)((value >> (i * 8)) & 0xFF);
		}

		private Byte[] NewMask()
		{
			Byte[] lResult = new Byte[4];
			for (Int32 i = 0; i < lResult.Length; i++)
				lResult[i] = (Byte)this.fRandom.NextInt(256);
			return lResult;
		}

		private void ApplyMask(Byte[] payload, Byte[] mask)
		{
			for (Int32 i = 0; i < payload.Length; i++)
				payload[i] = (Byte)(payload[i] ^ mask[i % 4]);
		}
	}

	public class WebSocketException : Exception
	{
		public WebSocketException(String message)
			: base(message)
		{
		}
	}
}
