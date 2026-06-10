/*---------------------------------------------------------------------------
  RemObjects Internet Pack
  (c)opyright RemObjects Software, LLC. 2003-2026. All rights reserved.
---------------------------------------------------------------------------*/

using RemObjects.Elements.RTL;
using RemObjects.InternetPack.Http;

namespace RemObjects.InternetPack.WebSocket
{
	public class WebSocketClient : HttpClient
	{
		private const String WEBSOCKET_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
		private const String DEFAULT_WEBSOCKET_VERSION = "13";

		public WebSocketConnection ConnectWebSocket(String url)
		{
			return this.ConnectWebSocket(url, null);
		}

		public WebSocketConnection ConnectWebSocket(String url, Dictionary<String, String> headers)
		{
			if (String.IsNullOrEmpty(url))
				throw new ArgumentNullException("url");

			String lHttpUrl = this.HttpUrlForWebSocketUrl(url);
			String lKey = this.NewWebSocketKey();

			HttpClientRequest lRequest = new HttpClientRequest();
			lRequest.URL = UrlParser.UrlWithString(lHttpUrl);
			lRequest.Header.RequestType = "GET";
			lRequest.Header.SetHeaderValue("User-Agent", this.UserAgent);
			lRequest.Header.SetHeaderValue("Upgrade", "websocket");
			lRequest.Header.SetHeaderValue("Connection", "Upgrade");
			lRequest.Header.SetHeaderValue("Sec-WebSocket-Key", lKey);
			lRequest.Header.SetHeaderValue("Sec-WebSocket-Version", DEFAULT_WEBSOCKET_VERSION);

			if (headers != null)
			{
				foreach (String lHeaderName in headers.Keys)
					lRequest.Header.SetHeaderValue(lHeaderName, headers[lHeaderName]);
			}

			HttpClientResponse lResponse = this.Dispatch(lRequest);
			this.ValidateUpgradeResponse(lResponse, lKey);

			return new WebSocketConnection(lResponse.DataConnection, true);
		}

		private String HttpUrlForWebSocketUrl(String url)
		{
			if (url.StartsWith("ws://", true))
				return "http://" + url.Substring(5);

			if (url.StartsWith("wss://", true))
				return "https://" + url.Substring(6);

			return url;
		}

		private void ValidateUpgradeResponse(HttpClientResponse response, String key)
		{
			if (response.HttpCode != HttpStatusCode.SwitchingProtocols)
				throw new WebSocketException(String.Format("Expected HTTP 101 Switching Protocols, got {0}.", response.HttpCode));

			String lUpgrade = response.Header.GetHeaderValue("Upgrade");
			if (lUpgrade == null || !String.Equals(lUpgrade.ToLowerInvariant(), "websocket"))
				throw new WebSocketException("The server did not confirm the WebSocket upgrade.");

			String lConnection = response.Header.GetHeaderValue("Connection");
			if (lConnection == null || lConnection.ToLowerInvariant().IndexOf("upgrade") < 0)
				throw new WebSocketException("The server did not keep the upgraded connection.");

			String lExpectedAccept = this.AcceptForKey(key);
			String lActualAccept = response.Header.GetHeaderValue("Sec-WebSocket-Accept");
			if (!String.Equals(lExpectedAccept, lActualAccept))
				throw new WebSocketException("The server returned an invalid Sec-WebSocket-Accept value.");
		}

		private String NewWebSocketKey()
		{
			Byte[] lBytes = Guid.NewGuid().ToByteArray();
			return Convert.ToBase64String(lBytes, 0, lBytes.Length);
		}

		private String AcceptForKey(String key)
		{
			Byte[] lBytes = Encoding.ASCII.GetBytes(key + WEBSOCKET_GUID);
			Byte[] lHash = this.Sha1(lBytes);
			return Convert.ToBase64String(lHash, 0, lHash.Length);
		}

		private Byte[] Sha1(Byte[] data)
		{
			UInt64 lBitLength = (UInt64)data.Length * 8;
			Int32 lPaddingLength = 1;
			while (((data.Length + lPaddingLength + 8) % 64) != 0)
				lPaddingLength++;

			Byte[] lMessage = new Byte[data.Length + lPaddingLength + 8];
			Array.Copy(data, 0, lMessage, 0, data.Length);
			lMessage[data.Length] = 0x80;

			for (Int32 i = 0; i < 8; i++)
				lMessage[lMessage.Length - 1 - i] = (Byte)((lBitLength >> (8 * i)) & 0xFF);

			UInt32 lH0 = 1732584193;
			UInt32 lH1 = 4023233417;
			UInt32 lH2 = 2562383102;
			UInt32 lH3 = 271733878;
			UInt32 lH4 = 3285377520;
			UInt32[] lWords = new UInt32[80];

			for (Int32 lOffset = 0; lOffset < lMessage.Length; lOffset += 64)
			{
				for (Int32 i = 0; i < 16; i++)
				{
					Int32 lIndex = lOffset + (i * 4);
					lWords[i] = ((UInt32)lMessage[lIndex] << 24) |
						((UInt32)lMessage[lIndex + 1] << 16) |
						((UInt32)lMessage[lIndex + 2] << 8) |
						lMessage[lIndex + 3];
				}

				for (Int32 i = 16; i < 80; i++)
					lWords[i] = this.RotateLeft(lWords[i - 3] ^ lWords[i - 8] ^ lWords[i - 14] ^ lWords[i - 16], 1);

				UInt32 lA = lH0;
				UInt32 lB = lH1;
				UInt32 lC = lH2;
				UInt32 lD = lH3;
				UInt32 lE = lH4;

				for (Int32 i = 0; i < 80; i++)
				{
					UInt32 lF;
					UInt32 lK;
					if (i < 20)
					{
						lF = (lB & lC) | ((~lB) & lD);
						lK = 1518500249;
					}
					else if (i < 40)
					{
						lF = lB ^ lC ^ lD;
						lK = 1859775393;
					}
					else if (i < 60)
					{
						lF = (lB & lC) | (lB & lD) | (lC & lD);
						lK = 2400959708;
					}
					else
					{
						lF = lB ^ lC ^ lD;
						lK = 3395469782;
					}

					UInt32 lTemp = this.Add(this.Add(this.Add(this.Add(this.RotateLeft(lA, 5), lF), lE), lK), lWords[i]);
					lE = lD;
					lD = lC;
					lC = this.RotateLeft(lB, 30);
					lB = lA;
					lA = lTemp;
				}

				lH0 = this.Add(lH0, lA);
				lH1 = this.Add(lH1, lB);
				lH2 = this.Add(lH2, lC);
				lH3 = this.Add(lH3, lD);
				lH4 = this.Add(lH4, lE);
			}

			Byte[] lResult = new Byte[20];
			this.WriteUInt32NetworkOrder(lResult, 0, lH0);
			this.WriteUInt32NetworkOrder(lResult, 4, lH1);
			this.WriteUInt32NetworkOrder(lResult, 8, lH2);
			this.WriteUInt32NetworkOrder(lResult, 12, lH3);
			this.WriteUInt32NetworkOrder(lResult, 16, lH4);
			return lResult;
		}

		private UInt32 RotateLeft(UInt32 value, Int32 bits)
		{
			return (UInt32)((value << bits) | (value >> (32 - bits)));
		}

		private UInt32 Add(UInt32 left, UInt32 right)
		{
			return (UInt32)(((UInt64)left + right) & 0xFFFFFFFF);
		}

		private void WriteUInt32NetworkOrder(Byte[] buffer, Int32 offset, UInt32 value)
		{
			buffer[offset] = (Byte)((value >> 24) & 0xFF);
			buffer[offset + 1] = (Byte)((value >> 16) & 0xFF);
			buffer[offset + 2] = (Byte)((value >> 8) & 0xFF);
			buffer[offset + 3] = (Byte)(value & 0xFF);
		}
	}
}
