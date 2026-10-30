<img src="http://www.elementscompiler.com/images/product-logos/InternetPack-256.png" width="128" align="right" />

# Elements RTL Internet Pack

Part of the "[Elements RTL](http://www.elementscompiler.com/elements/rtl)" (RTL2) project, this is a cross platform TCP/IP network library for the Elements compiler, including an abstraction of both low-level IP client and server classes, as well as implementations of common TCP/IP protocols such as HTTP, POP and more.

## HTTP request bodies

The synchronous `HttpServer` used by ESP accepts HTTP/1.1 `Transfer-Encoding:
chunked` requests. Before dispatch it buffers and decodes the body, removes
Transfer-Encoding and Trailer, and sets Content-Length to the decoded byte count.
ContentBytes, ContentString and ContentStream then expose the same decoded body.
This framing step also runs when optional ValidateRequests checks are disabled.
It does not provide incremental application execution or reduce body memory use.

Chunk extensions are ignored; trailers are consumed and discarded rather than
merged into application headers. Framing requires CRLF. Each framing line is
limited to 8192 bytes and the trailer section to 64 KiB; decoded bodies use the
existing Int32 length model and available process memory, with additional memory
needed for buffering. Connection receive timeouts still apply. Applications and
fronting proxies should enforce their deployment-specific upload limits.
Ambiguous Transfer-Encoding plus Content-Length is rejected with 400; unsupported
transfer codings with 501; invalid framing with 400, and length overflow with 413.
The synchronous server honors Expect: 100-continue for chunked bodies.
The separate AsyncHttpServer parser is outside this support contract.

Changes to this library belong in this standalone repository. Elements imports
committed owner changes through its existing mergeInternetPack.sh script; do not
patch the Source/InternetPack subtree directly. Deploy matching InternetPack and
Elements.Web binaries when the latter uses new transport capabilities.

## Origins

Internet Pack was originally written as a pure .NET library using Visual C#, for [Remoting SDK](http://www.remotingsdk.com). It is in the process of being ported/upgraded to use RTL2 and our cross-platform Hydrogene compiler, and be supported on *all* Elements platforms: .NET, Java/Android, Cocoa and Island.

The original .NET version is preserved in the `master-legacy` branch, but will no longer be maintained.

## Requirements

As of this writing, the Internet Pack library requires the latest Elements 9.2 beta compiler to be built and to be used.

## See Also

* [RemObjects Elements Homepage](http://www.elementscompiler.com/)
* [RemObjects Elements Docs](http://docs.elementscompiler.com/)
<!--* [Internet Pack](http://docs.elementscompiler.com/API/InternetPack/) on docs.elementscompiler.com-->
* [Elements RTL](http://www.elementscompiler.com/elements/rtl) Homepage
* [Elements RTL](https://www.github.com/RemObjects/RTL2/) on GitHub

	
