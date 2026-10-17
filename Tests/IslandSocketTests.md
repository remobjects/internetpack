# InternetPack Island socket adapter tests

`IslandSocketTests.elements` is an Oxygene executable using the public
InternetPack API against an explicitly selected native IslandRTL build.
Default execution tests DNS, scoped IPv6, IPv4/IPv6 asynchronous connect/accept/
send/receive, offsets, endpoint ports, Connection line I/O, close cancellation,
and native error propagation through EndReceive. It prints
`INTERNETPACK TRANSPORT TESTS PASSED` only after those checks complete.

Pass `--http` to additionally exercise the full plain HTTP client against an
in-process socket peer. This wider smoke test succeeds on macOS. The native Fuchsia locale gap that
previously prevented HTTP initialization has been fixed in IslandRTL. All 29
transport and HTTP checks now pass on the arm64 VIM3 with matching IslandRTL/RTL2
references. Socket transport success alone does not establish HTTP client support.

Build IslandRTL first using its `Tests/SocketTests.md` instructions. Rebuild RTL2
and InternetPack consumers together, particularly on macOS where native
exception ancestry is now explicit. Then:

```sh
RO_DIR=/Users/mh/Code/RemObjects
SOCKET_OUT=/tmp/island-socket-validation
mono "$RO_DIR/Elements/Bin/EBuild.exe" "$RO_DIR/RTL2/Source/RemObjects.Elements.RTL/Elements.RTL.elements" \
  --targets:Island.Fuchsia --configuration:Debug --setting:Architecture=arm64 \
  --setting:AdditionalReferencePaths="$SOCKET_OUT/rtl/Fuchsia/arm64" \
  --out:"$SOCKET_OUT/rtl2" --intermediatebasefolder:"$SOCKET_OUT/rtl2-obj"
mono "$RO_DIR/Elements/Bin/EBuild.exe" "$RO_DIR/internetpack/Source/RemObjects.InternetPack.elements" \
  --targets:Island.Fuchsia --configuration:Debug --setting:Architecture=arm64 \
  --setting:AdditionalReferencePaths="$SOCKET_OUT/rtl/Fuchsia/arm64;$SOCKET_OUT/rtl2/Island/Fuchsia/arm64" \
  --out:"$SOCKET_OUT/ip" --intermediatebasefolder:"$SOCKET_OUT/ip-obj"
mono "$RO_DIR/Elements/Bin/EBuild.exe" "$RO_DIR/internetpack/Tests/IslandSocketTests.elements" \
  --targets:Island.Fuchsia --setting:Architecture=arm64 \
  --setting:SocketRTLReference="$SOCKET_OUT/rtl/Fuchsia/arm64/Island.fx" \
  --setting:InternetPackReference="$SOCKET_OUT/ip/Island/Fuchsia/Fuchsia/arm64/RemObjects.InternetPack.fx" \
  --setting:AdditionalReferencePaths="$SOCKET_OUT/rtl2/Island/Fuchsia/arm64" \
  --out:"$SOCKET_OUT/ip-tests" --intermediatebasefolder:"$SOCKET_OUT/ip-test-obj" \
  --setting:CreatePackage=False --nocache
```

Check both reference paths in the build log. For macOS select
`Island.Darwin.macOS`, use `rtl/macOS/arm64/Island.fx`, and use
`ip/Island/Darwin/macOS/arm64/RemObjects.InternetPack.fx`. Windows and Linux
are also selectable targets. Fuchsia capability routing, packaging, and live
log capture are described in IslandRTL's test instructions. For the optional
Fuchsia HTTP probe, supply `"args": ["--http"]` in a custom component manifest.

## Remaining Fuchsia surface

- Socket options: the tested Fuchsia fdio provider rejects SO_LINGER with
  EOPNOTSUPP; the adapter preserves that error. No unsupported option is silently
  treated as successful.
- Runtime prerequisites: IslandRTL now implements Fuchsia timers, libc current/
  invariant locales, file seeking/truncation and secure random via zx_cprng_draw.
  Its FuchsiaRuntimeTests project exercises these and their RTL2 wrappers.
  Full system internationalization is outside libc locale support.
- TLS/HTTPS/WSS: the non-.NET SslStream compatibility surface contains stubs;
  secure WebSockets explicitly reject unsupported targets. A real TLS provider,
  certificate verification, trust roots, and secure transport integration remain.
- Server worker creation: Fuchsia reflection invocation now uses IslandRTL's
  existing architecture-specific FFI. Validate worker factories and full server
  lifecycle before claiming HttpServer/TcpServer support.
- Protocol-specific FTP, SMTP, POP3, LDAP, MIME, and WebSocket behavior needs its
  own runtime acceptance; successful library compilation does not validate it.

## Distribution

The InternetPack project now exposes `Island.Fuchsia`; the central Elements
reference packaging script includes its Fuchsia outputs. Elements vendors both
IslandRTL and InternetPack via normal subtree imports. These changes live in the
owning repositories; perform the normal source synchronization before a release
build. No subtree merge, commit, installed SDK replacement, or release was done
as part of this port. The pre-existing untracked InternetPack Build directory
was left untouched.
