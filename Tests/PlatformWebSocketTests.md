# Platform WebSocket tests

The Oxygene test source is compiled with Elements for both Echoes and Toffee. No API credentials are used.

1. Run `python3 Tests/websocket_fixture.py` (loopback port 18765).
2. Build `Tests/PlatformWebSocketTests.elements` and `Tests/PlatformWebSocketTests.macOS.elements` in Debug, then run each executable without arguments.
3. For a trusted TLS smoke test, pass a WebSocket echo URL, e.g. `wss://ws.postman-echo.com/raw`. This sends only the fixed probe text.
4. To test certificate rejection, generate an untrusted localhost certificate with OpenSSL, run `python3 Tests/websocket_fixture.py --cert <certificate.pem> --key <key.pem>` (port 18766), and pass `wss://localhost:18766/ reject` to each executable. Do not install the certificate into a trust store.

Loopback checks cover upgrade headers, fragmented UTF-8, interleaved Ping, 40 KB binary data, empty messages, concurrent sends, peer close, and disposal unblocking a pending read. The fixture uses only the Python standard library.

## Transport behavior

- Toffee uses `NSURLSessionWebSocketTask`; runtime availability is checked before use.
- Echoes NETSTANDARD/NETCORE uses `ClientWebSocket`. Legacy .NET 4.0 retains the existing transport.
- Platform libraries perform TLS verification and frame processing. No certificate-validation override is installed.
- Sends and receives are separately serialized, allowing one of each concurrently.
- `Dispose`/`Close` terminate the transport and release pending reads; they do not wait indefinitely for a close handshake.
- Raw `Connection` is null for platform-backed connections. The raw-connection constructor is unchanged.
- Manual `SendPing`/`SendPong` are rejected on platform transports; their libraries handle control frames automatically.
- Platform transports use platform proxy configuration. Explicit InternetPack proxy settings are rejected rather than silently ignored.
