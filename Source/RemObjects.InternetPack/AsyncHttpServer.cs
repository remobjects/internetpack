/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RemObjects.InternetPack.Http
{
#if DESIGN
	[System.Drawing.ToolboxBitmap(typeof(RemObjects.InternetPack.Server), "Glyphs.HttpServer.bmp")]
#endif
	public class AsyncHttpServer : AsyncServer
	{
		private const String DEFAULT_SERVER_NAME = "Internet Pack HTTP Server";

		public AsyncHttpServer()
		{
			this.DefaultPort = 80;
			this.KeepAlive = true;
			this.ServerName = DEFAULT_SERVER_NAME;
			this.MaxPostSize = 4194304;
		}

#if FULLFRAMEWORK
		public AsyncHttpServer(IContainer container)
			: this()
		{
			if (container != null)
				container.Add(this);
		}
#endif

		public override Type GetWorkerClass()
		{
			return typeof(AsyncHttpWorker);
		}

		#region Properties
		[Category("Server")]
		[DefaultValue(true)]
		public Boolean KeepAlive { get; set; }

		[Category("Server")]
		[DefaultValue(true)]
		public Boolean ValidateRequests { get; set; }

		[Category("Server")]
		[DefaultValue(DEFAULT_SERVER_NAME)]
		public String ServerName { get; set; }

		[Category("Server")]
		[DefaultValue(4194304)]
		public Int32 MaxPostSize { get; set; }
		#endregion

		public event AsyncHttpRequestEventHandler BeforeHaveData;
		public event AsyncHttpRequestEventHandler HttpRequest;
		public event AsyncHttpRequestEventHandler HttpResponseSent;
		public event AsyncHttpRequestEventHandler HttpResponseFailed;

		protected internal virtual void TriggerBeforeHaveData(AsyncHttpRequestEventArgs e)
		{
			if (this.BeforeHaveData != null)
				this.BeforeHaveData(this, e);
		}

		protected internal virtual void TriggerHttpRequest(AsyncHttpRequestEventArgs e)
		{
			if (this.HttpRequest != null)
				this.HttpRequest(this, e);
		}

		protected internal virtual void TriggerHttpResponseSent(AsyncHttpRequestEventArgs e)
		{
			if (this.HttpResponseSent != null)
				this.HttpResponseSent(this, e);
		}

		protected internal virtual void TriggerHttpResponseFailed(AsyncHttpRequestEventArgs e)
		{
			if (this.HttpResponseFailed != null)
				this.HttpResponseFailed(this, e);
		}

		protected internal virtual AsyncHttpContext NewContext(AsyncHttpWorker worker)
		{
			return new AsyncHttpContext(worker);
		}
	}

	public class AsyncHttpRequest : HttpRequestResponse
	{
		protected override Boolean Client
		{
			get
			{
				return false;
			}
		}

		protected override Boolean Server
		{
			get
			{
				return true;
			}
		}

		public Byte[] ContentBytes { get; set; }
	}

	public class AsyncHttpContext
	{
		public AsyncHttpContext(AsyncHttpWorker worker)
		{
			this.fWorker = worker;
			this.fCurrentRequest = new AsyncHttpRequest();
			this.fCurrentResponse = new HttpServerResponse();
		}

		private readonly AsyncHttpWorker fWorker;

		#region Properties
		public Connection Connection
		{
			get
			{
				return this.fWorker.DataConnection;
			}
		}

		public AsyncHttpRequest CurrentRequest
		{
			get
			{
				return this.fCurrentRequest;
			}
		}
		private readonly AsyncHttpRequest fCurrentRequest;

		public HttpServerResponse CurrentResponse
		{
			get
			{
				return this.fCurrentResponse;
			}
		}
		private readonly HttpServerResponse fCurrentResponse;

		public Boolean ResponseSent { get; set; }

		public Object UserData { get; set; }
		#endregion

		public void SendResponse()
		{
			if (this.ResponseSent)
				return;

			this.ResponseSent = true;
			this.CurrentResponse.FinalizeHeader();
			this.fWorker.SendResponse();
		}
	}

	public class AsyncHttpWorker : AsyncWorker
	{
		#region Private fields
		private AsyncHttpServer fOwner;
		private AsyncHttpContext fContext;
		private Int32 fBodyOffset;
		private Byte[] fBodyBuffer;
		#endregion

		public override void Setup()
		{
			this.DataConnection.MaxLineLength = 8096;
			this.DataConnection.MaxLineLengthEnabled = true;
			this.DataConnection.AsyncDisconnect += DisconnectCallback;
			this.fContext = this.fOwner.NewContext(this);
			try
			{
				this.DataConnection.BeginReadLine(HeaderFirstLineCallback, null);
			}
			catch (SocketException)
			{
				Done();
			}
			catch (ObjectDisposedException)
			{
				Done();
			}
		}

		public override Server Owner
		{
			get
			{
				return base.Owner;
			}
			set
			{
				base.Owner = value;
				this.fOwner = (AsyncHttpServer)value;
			}
		}

		private void DisconnectCallback(Object sender, EventArgs e)
		{
			this.Done();
		}

		#region Header reader callbacks
		private void HeaderFirstLineCallback(IAsyncResult ar)
		{
			fContext.ResponseSent = false;

			String lFirst;
			try
			{
				lFirst = this.DataConnection.EndReadLine(ar);
			}
			catch (ConnectionClosedException)
			{
				Done();
				return;
			}
			catch (SocketException)
			{
				Done();
				return;
			}
			catch (ObjectDisposedException)
			{
				Done();
				return;
			}

			try
			{
				this.fContext.CurrentRequest.Header.FirstHeader = lFirst;
				this.fContext.CurrentRequest.Header.ParseFirstLine();
			}
			catch (HttpHeaderException)
			{
				this.SendInvalidRequest();
				return;
			}
			catch (UrlParserException)
			{
				this.SendInvalidRequest();
				return;
			}

			try
			{
				this.DataConnection.BeginReadLine(HeaderLinesCallback, null);
			}
			catch (SocketException)
			{
				Done();
			}
			catch (ObjectDisposedException)
			{
				Done();
			}
		}

		private void HeaderLinesCallback(IAsyncResult ar)
		{
			String lHeaderLine;
			try
			{
				lHeaderLine = this.DataConnection.EndReadLine(ar);
			}
			catch (ConnectionClosedException)
			{
				Done();
				return;
			}
			catch (SocketException)
			{
				Done();
				return;
			}
			catch (ObjectDisposedException)
			{
				Done();
				return;
			}

			// HTTP Request Type is already known
			String lHttpMethod = this.fContext.CurrentRequest.Header.RequestType;
			Boolean lRequireBody = (lHttpMethod == "POST") || (lHttpMethod == "PUT") || (lHttpMethod == "MERGE");

			Boolean lHaveData = true;
			while (lHaveData)
			{
				lHaveData = false;
				if (lHeaderLine == "")
				{
					// we've got the last line. Process it
					if (lRequireBody)
					{
						Int64 lContentLength;
#if FULLFRAMEWORK
						if (!Int64.TryParse(fContext.CurrentRequest.Header.GetHeaderValue("Content-Length"), out lContentLength))
#else
						if (!LongHelper.TryParse(fContext.CurrentRequest.Header.GetHeaderValue("Content-Length"), out lContentLength))
#endif
							lContentLength = 0;

						if (lContentLength > ((AsyncHttpServer)this.Owner).MaxPostSize)
						{
							this.SendInvalidRequest(new Exception("Content-Length too large"));
							return;
						}

						try
						{
							((AsyncHttpServer)this.Owner).TriggerBeforeHaveData(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
						}
						catch (Exception ex)
						{
							this.SendInvalidRequest(ex);
							return;
						}

						if (this.fContext.ResponseSent)
							return; // already triggered the required functions.

						try
						{
							Byte[] lData = new Byte[(Int32)lContentLength];
							DataConnection.BeginRead(lData, 0, (Int32)lContentLength, WantBodyCallback, lData);
						}
						catch (SocketException)
						{
							Done();
						}
						catch (ObjectDisposedException)
						{
							Done();
						}

						return;
					}
					else
					{
						try
						{
							this.fOwner.TriggerHttpRequest(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
							return;
						}
						catch (Exception ex)
						{
							this.SendInvalidRequest(ex);
							return;
						}
					}
				}

				if (fContext.CurrentRequest.Header.Count >= fContext.CurrentRequest.Header.MaxHeaderLines && fContext.CurrentRequest.Header.MaxHeaderLinesEnabled)
				{
					SendInvalidRequest();
					return;
				}

				Int32 lPosition = lHeaderLine.IndexOf(":", StringComparison.Ordinal);
				if (lPosition == -1)
				{
					SendInvalidRequest();
					return;
				}

				String lName = lHeaderLine.Substring(0, lPosition);
				String lValue = null;
				if (lHeaderLine.Length > lPosition + 1)
					lValue = lHeaderLine.Substring(lPosition + 2);

				fContext.CurrentRequest.Header.SetHeaderValue(lName, lValue);
				lHeaderLine = DataConnection.BufferReadLine();
				if (lHeaderLine != null)
				{
					lHaveData = true;
					continue;
				}

				try
				{
					DataConnection.BeginReadLine(HeaderLinesCallback, null);
				}
				catch (SocketException)
				{
					Done();
				}
				catch (ObjectDisposedException)
				{
					Done();
				}
			}
		}

		private void WantBodyCallback(IAsyncResult ar)
		{
			try
			{
				this.DataConnection.EndRead(ar);
			}
			catch (ConnectionClosedException)
			{
				Done();
				return;
			}
			catch (SocketException)
			{
				Done();
				return;
			}
			catch (ObjectDisposedException)
			{
				Done();
				return;
			}

			this.fContext.CurrentRequest.ContentBytes = (Byte[])ar.AsyncState;

			try
			{
				this.fOwner.TriggerHttpRequest(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
			}
			catch (Exception ex)
			{
				this.SendInvalidRequest(ex);
			}
		}
		#endregion

		private void SendInvalidRequest(Exception ex)
		{
			this.fContext.CurrentResponse.Header.SetHeaderValue("Server", this.fOwner.ServerName);

			if (ex != null)
				this.fContext.CurrentResponse.SendError(HttpStatusCode.InternalServerError, ex);
			else
				this.fContext.CurrentResponse.SendError(HttpStatusCode.InternalServerError, "Server Error while processing HTTP request.");

			if (this.fContext.CurrentResponse.ContentBytes == null)
			{
				this.fContext.CurrentResponse.ContentBytes = this.fContext.CurrentResponse.ContentString != null
																? this.fContext.CurrentResponse.Encoding.GetBytes(fContext.CurrentResponse.ContentString)
																: new Byte[0];
			}

			this.fContext.CurrentResponse.FinalizeHeader();
			Byte[] lHeader = Encoding.ASCII.GetBytes(this.fContext.CurrentResponse.Header.ToString());
			Byte[] lData = new Byte[fContext.CurrentResponse.ContentBytes.Length + lHeader.Length];

			Array.Copy(lHeader, 0, lData, 0, lHeader.Length);
			Array.Copy(fContext.CurrentResponse.ContentBytes, 0, lData, lHeader.Length, fContext.CurrentResponse.ContentBytes.Length);

			this.DataConnection.BeginWrite(lData, 0, lData.Length, InvalidRequestCallback, null);
		}

		private void SendInvalidRequest()
		{
			this.SendInvalidRequest(null);
		}

		private void InvalidRequestCallback(IAsyncResult ar)
		{
			try
			{
				this.DataConnection.EndWrite(ar);
				this.DataConnection.TriggerAsyncDisconnect();
				this.DataConnection.Disconnect();
			}
			catch (ConnectionClosedException)
			{
				Done();
			}
			catch (SocketException)
			{
				Done();
			}
			catch (ObjectDisposedException)
			{
				Done();
			}
		}

		public void SendResponse()
		{
			try
			{
				fContext.CurrentResponse.KeepAlive = ((AsyncHttpServer)AsyncOwner).KeepAlive;
				Byte[] lHeader = Encoding.ASCII.GetBytes(fContext.CurrentResponse.Header.ToString());

				if (lHeader.Length >= 4096 || fContext.CurrentResponse.ContentSource == ContentSource.ContentNone)
				{
					fBodyOffset = 0;

					switch (fContext.CurrentResponse.ContentSource)
					{
						case ContentSource.ContentBytes:
							DataConnection.BeginWrite(lHeader, 0, fBodyOffset, ResponseBodyCallback, fContext.CurrentResponse.ContentBytes);
							break;

						case ContentSource.ContentString:
							Byte[] lBuffer = fContext.CurrentResponse.Encoding.GetBytes(fContext.CurrentResponse.ContentString);
							DataConnection.BeginWrite(lHeader, 0, fBodyOffset, ResponseBodyCallback, lBuffer);
							break;

						case ContentSource.ContentStream:
							fContext.CurrentResponse.ContentStream.Position = 0;
							DataConnection.BeginWrite(lHeader, 0, fBodyOffset, ResponseBodyCallback, fContext.CurrentResponse.ContentStream);
							break;

						default:
							DataConnection.BeginWrite(lHeader, 0, fBodyOffset, ResponseBodyCallback, null);
							break;
					}
				}
				else
				{
					if (fBodyBuffer == null)
						fBodyBuffer = new Byte[4096];

					Array.Copy(lHeader, 0, fBodyBuffer, 0, lHeader.Length);
					fBodyOffset = fBodyBuffer.Length - lHeader.Length;

					switch (fContext.CurrentResponse.ContentSource)
					{
						case ContentSource.ContentBytes:
							if (fBodyOffset > fContext.CurrentResponse.ContentBytes.Length)
								fBodyOffset = fContext.CurrentResponse.ContentBytes.Length;
							Array.Copy(fContext.CurrentResponse.ContentBytes, 0, fBodyBuffer, lHeader.Length, fBodyOffset);
							DataConnection.BeginWrite(fBodyBuffer, 0, fBodyOffset + lHeader.Length, ResponseBodyCallback, fContext.CurrentResponse.ContentBytes);
							break;

						case ContentSource.ContentString:
							Byte[] lBuffer = fContext.CurrentResponse.Encoding.GetBytes(fContext.CurrentResponse.ContentString);
							if (fBodyOffset > lBuffer.Length)
								fBodyOffset = lBuffer.Length;
							Array.Copy(lBuffer, 0, fBodyBuffer, lHeader.Length, fBodyOffset);
							DataConnection.BeginWrite(fBodyBuffer, 0, fBodyOffset + lHeader.Length, ResponseBodyCallback, lBuffer);
							break;

						case ContentSource.ContentStream:
							fContext.CurrentResponse.ContentStream.Position = 0;
							fBodyOffset = fContext.CurrentResponse.ContentStream.Read(fBodyBuffer, lHeader.Length, fBodyOffset);
							DataConnection.BeginWrite(fBodyBuffer, 0, fBodyOffset + lHeader.Length, ResponseBodyCallback, fContext.CurrentResponse.ContentStream);
							break;

						default:
							DataConnection.BeginWrite(lHeader, 0, fBodyOffset, ResponseBodyCallback, null);
							break;
					}
				}
			}
			catch (ConnectionClosedException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
			}
			catch (SocketException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
			}
			catch (ObjectDisposedException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
			}
		}

		private void ResponseBodyCallback(IAsyncResult ar)
		{
			try
			{
				DataConnection.EndWrite(ar);

				if (ar.AsyncState is Stream)
				{
					Stream lData = (Stream)ar.AsyncState;
					Int32 lLen = lData.Read(fBodyBuffer, 0, fBodyBuffer.Length);
					if (lLen != 0)
					{
						DataConnection.BeginWrite(fBodyBuffer, 0, lLen, ResponseBodyCallback, lData);
						return;
					}
				}
				else if (ar.AsyncState is Byte[])
				{
					Byte[] lData = (Byte[])ar.AsyncState;
					Int32 lLen = fBodyBuffer.Length;
					if (fBodyOffset + lLen > lData.Length)
						lLen = lData.Length - fBodyOffset;
					if (lLen != 0)
					{
						Array.Copy(lData, fBodyOffset, fBodyBuffer, 0, lLen);
						fBodyOffset += lLen;
						DataConnection.BeginWrite(fBodyBuffer, 0, lLen, ResponseBodyCallback, lData);
						return;
					}
				}
			}
			catch (ConnectionClosedException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
				return;
			}
			catch (SocketException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
				return;
			}
			catch (ObjectDisposedException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
				return;
			}

			//AsyncHttpContext lOldContext = fContext;

			fContext = this.fOwner.NewContext(this);
			try
			{
				DataConnection.BeginReadLine(HeaderFirstLineCallback, null);
			}
			catch (SocketException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
				return;
			}

			catch (ObjectDisposedException)
			{
				this.fOwner.TriggerHttpResponseFailed(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
				Done();
				return;
			}
			this.fOwner.TriggerHttpResponseSent(new AsyncHttpRequestEventArgs(this.DataConnection, this.fContext));
		}
	}
}
