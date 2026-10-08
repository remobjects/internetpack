using RemObjects.Elements.RTL;

namespace RemObjects.InternetPack.WebSocket
{
	// Platform transports own TLS and framing. Connection is null for these
	// message-based backends; raw InternetPack connections remain supported.
	internal interface IWebSocketTransport : IDisposable
	{
		Boolean Closed { get; }
		WebSocketMessage ReadMessage();
		void Send(WebSocketOpcode opcode, Byte[] payload);
		void Close();
	}

#if ECHOES && (NETSTANDARD || NETCORE)
	internal class PlatformWebSocketTransport : IWebSocketTransport
	{
		private readonly System.Net.WebSockets.ClientWebSocket fSocket = new System.Net.WebSockets.ClientWebSocket();
		private readonly System.Threading.CancellationTokenSource fLifetime = new System.Threading.CancellationTokenSource();
		private readonly Object fReadGate = new Object();

		public PlatformWebSocketTransport(String url, Dictionary<String, String> headers, Int32 timeout)
		{
			try
			{
				if (headers != null)
				{
					foreach (var name in headers.Keys)
					{
						fSocket.Options.SetRequestHeader(name, headers[name]);
					}
				}
				using (var connect = new System.Threading.CancellationTokenSource())
				{
					connect.CancelAfter(System.TimeSpan.FromSeconds(timeout));
					fSocket.ConnectAsync(new System.Uri(url), connect.Token).GetAwaiter().GetResult();
				}
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public Boolean Closed { get { return fSocket.State != System.Net.WebSockets.WebSocketState.Open; } }

		public WebSocketMessage ReadMessage()
		{
			lock (fReadGate)
			{
				using (var message = new System.IO.MemoryStream())
				{
					var buffer = new Byte[16384];
					while (true)
					{
						var part = fSocket.ReceiveAsync(new System.ArraySegment<Byte>(buffer), fLifetime.Token).GetAwaiter().GetResult();
						if (part.MessageType == System.Net.WebSockets.WebSocketMessageType.Close)
						{
							return new WebSocketMessage(WebSocketOpcode.Close, null);
						}
						if (message.Length + part.Count > Int32.MaxValue)
						{
							throw new WebSocketException("WebSocket message is too large.");
						}
						message.Write(buffer, 0, part.Count);
						if (part.EndOfMessage)
						{
							return new WebSocketMessage(part.MessageType == System.Net.WebSockets.WebSocketMessageType.Text ? WebSocketOpcode.Text : WebSocketOpcode.Binary, message.ToArray());
						}
					}
				}
			}
		}

		public void Send(WebSocketOpcode opcode, Byte[] payload)
		{
			if (opcode != WebSocketOpcode.Text && opcode != WebSocketOpcode.Binary)
			{
				throw new WebSocketException("This platform manages WebSocket control frames automatically.");
			}
			fSocket.SendAsync(new System.ArraySegment<Byte>(payload), opcode == WebSocketOpcode.Text ? System.Net.WebSockets.WebSocketMessageType.Text : System.Net.WebSockets.WebSocketMessageType.Binary, true, fLifetime.Token).GetAwaiter().GetResult();
		}

		public void Close()
		{
			// Abort also releases a blocked receive; never wait for peer close.
			fLifetime.Cancel();
			fSocket.Abort();
		}

		public void Dispose()
		{
			Close();
			fSocket.Dispose();
		}
	}
#endif

#if TOFFEE
	internal class PlatformWebSocketTransport : IWebSocketTransport
	{
		private Foundation.NSURLSession fSession;
		private Foundation.NSURLSessionWebSocketTask fTask;
		private readonly Object fReadGate = new Object();
		private Boolean fClosed;

		public PlatformWebSocketTransport(String url, Dictionary<String, String> headers, Int32 timeout)
		{
			if (Foundation.NSClassFromString("NSURLSessionWebSocketTask") == null)
			{
				throw new WebSocketException("Secure WebSocket transport requires a newer operating system.");
			}
			try
			{
				var request = Foundation.NSMutableURLRequest.requestWithURL(Foundation.NSURL.URLWithString(url));
				request.timeoutInterval = timeout;
				if (headers != null)
				{
					foreach (var name in headers.Keys)
					{
						request.setValue(headers[name]) forHTTPHeaderField(name);
					}
				}
				fSession = Foundation.NSURLSession.sessionWithConfiguration(Foundation.NSURLSessionConfiguration.ephemeralSessionConfiguration);
				fTask = fSession.webSocketTaskWithRequest(request);
				fTask.resume();
				// A successful pong confirms TLS and the upgrade completed before
				// returning the connection or accepting application messages.
				var ready = new Event();
				Foundation.NSError error = null;
				fTask.sendPingWithPongReceiveHandler(e => { error = e; ready.Set(); });
				if (!ready.WaitFor(timeout * 1000))
				{
					throw new WebSocketException("WebSocket connection timed out.");
				}
				Check(error);
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		public Boolean Closed { get { return fClosed || fTask.state == Foundation.NSURLSessionTaskState.NSURLSessionTaskStateCompleted; } }

		private void Check(Foundation.NSError error)
		{
			if (error != null)
			{
				throw new WebSocketException(error.localizedDescription);
			}
		}

		public WebSocketMessage ReadMessage()
		{
			lock (fReadGate)
			{
				var ready = new Event();
				Foundation.NSError error = null;
				Foundation.NSURLSessionWebSocketMessage message = null;
				fTask.receiveMessageWithCompletionHandler((m, e) => { message = m; error = e; ready.Set(); });
				ready.WaitFor();
				if (fClosed || (message == null && fTask.closeCode != Foundation.NSURLSessionWebSocketCloseCode.NSURLSessionWebSocketCloseCodeInvalid))
				{
					return new WebSocketMessage(WebSocketOpcode.Close, null);
				}
				Check(error);
				if (message.type == Foundation.NSURLSessionWebSocketMessageType.NSURLSessionWebSocketMessageTypeString)
				{
					return new WebSocketMessage(WebSocketOpcode.Text, Encoding.UTF8.GetBytes(message.@string));
				}
				return new WebSocketMessage(WebSocketOpcode.Binary, new ImmutableBinary(message.data).ToArray());
			}
		}

		public void Send(WebSocketOpcode opcode, Byte[] payload)
		{
			if (opcode != WebSocketOpcode.Text && opcode != WebSocketOpcode.Binary)
			{
				throw new WebSocketException("This platform manages WebSocket control frames automatically.");
			}
			var message = opcode == WebSocketOpcode.Text
				? new Foundation.NSURLSessionWebSocketMessage withString(Encoding.UTF8.GetString(payload))
				: new Foundation.NSURLSessionWebSocketMessage withData((Foundation.NSData)new ImmutableBinary(payload));
			var ready = new Event();
			Foundation.NSError error = null;
			fTask.sendMessage(message) completionHandler(e => { error = e; ready.Set(); });
			ready.WaitFor();
			Check(error);
		}

		public void Close()
		{
			fClosed = true;
			fTask?.cancelWithCloseCode(Foundation.NSURLSessionWebSocketCloseCode.NSURLSessionWebSocketCloseCodeNormalClosure) reason(null);
			fSession?.invalidateAndCancel();
		}

		public void Dispose()
		{
			Close();
		}
	}
#endif
}
