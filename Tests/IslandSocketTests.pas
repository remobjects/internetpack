namespace InternetPackIslandSocketTests;

type
  IPSocket = RemObjects.InternetPack.Socket;
  IPAddress = RemObjects.InternetPack.IPAddress;
  IPEndPoint = RemObjects.InternetPack.IPEndPoint;
  IPFamily = RemObjects.InternetPack.AddressFamily;
  IPFlags = RemObjects.InternetPack.SocketFlags;
  Program = class
  private

    class var fChecks: Integer;

    class method Check(aValue: Boolean; aMessage: String);
    begin
      if not aValue then
        raise new Exception('FAIL: '+aMessage);
      inc(fChecks);
      writeLn('PASS: '+aMessage);
    end;

    class method NewSocket(aIPv6: Boolean := false): IPSocket;
    begin
      result := new IPSocket(if aIPv6 then IPFamily.InterNetworkV6 else IPFamily.InterNetwork,
                             RemObjects.InternetPack.SocketType.Stream, RemObjects.InternetPack.ProtocolType.Tcp);
    end;

    class method SendAll(aSocket: IPSocket; aBytes: array of Byte);
    begin
      var lOffset := 0;
      while lOffset < length(aBytes) do
        lOffset := lOffset + aSocket.Send(aBytes, lOffset, length(aBytes) - lOffset, IPFlags.None);
    end;

    class method TestAsync(aIPv6: Boolean);
    begin
      using lListener := NewSocket(aIPv6) do begin
        lListener.Bind(new IPEndPoint(IPAddress.Parse(if aIPv6 then '::1' else '127.0.0.1'), 0));
        lListener.Listen(2);
        using lAcceptedEvent := new EventWaitHandle(false, false) do
        using lConnectedEvent := new EventWaitHandle(false, false) do
        using lReceivedEvent := new EventWaitHandle(false, false) do
        using lSentEvent := new EventWaitHandle(false, false) do begin
          var lAccept := lListener.BeginAccept(ar -> lAcceptedEvent.Set, nil);
          using lClient := NewSocket(aIPv6) do begin
            var lConnect := lClient.BeginConnect(lListener.LocalEndPoint, ar -> lConnectedEvent.Set, nil);
            Check(lConnectedEvent.Wait(3000), 'BeginConnect callback');
            lClient.EndConnect(lConnect);
            Check(lAcceptedEvent.Wait(3000), 'BeginAccept callback');
            using lAccepted := lListener.EndAccept(lAccept) do begin
              Check(lAccepted.AddressFamily = lClient.AddressFamily, 'adapter accepted family');
              Check(IPEndPoint(lAccepted.RemoteEndPoint).Port = IPEndPoint(lClient.LocalEndPoint).Port, 'adapter endpoint port');
              var lBuffer := new Byte[7];
              var lRead := lAccepted.BeginReceive(lBuffer, 2, 3, IPFlags.None, ar -> lReceivedEvent.Set, nil);
              var lPayload: array of Byte := [99, 51, 52, 53, 98];
              var lSend := lClient.BeginSend(lPayload, 1, 3, IPFlags.None, ar -> lSentEvent.Set, nil);
              Check(lSentEvent.Wait(3000), 'BeginSend callback');
              Check(lClient.EndSend(lSend) = 3, 'EndSend size');
              Check(lReceivedEvent.Wait(3000), 'BeginReceive callback');
              Check(lAccepted.EndReceive(lRead) = 3, 'EndReceive size');
              Check((lBuffer[2] = 51) and (lBuffer[3] = 52) and (lBuffer[4] = 53) and (lBuffer[0] = 0), 'adapter receive offset');
              using lConnection := new RemObjects.InternetPack.Connection(lAccepted) do begin
                lConnection.TimeoutEnabled := false;
                lConnection.WriteLine('adapter connection');
                using lClientConnection := new RemObjects.InternetPack.Connection(lClient) do begin
                  lClientConnection.TimeoutEnabled := false;
                  Check(lClientConnection.ReadLine = 'adapter connection', 'Connection line I/O');
                end;
              end;
            end;
          end;
        end;
      end;
    end;

    class method TestAcceptShutdown;
    begin
      using lListener := NewSocket do begin
        lListener.Bind(new IPEndPoint(IPAddress.Parse('127.0.0.1'), 0));
        lListener.Listen(1);
        using lCompleted := new EventWaitHandle(false, false) do begin
          var lAccept := lListener.BeginAccept(ar -> lCompleted.Set, nil);
          Thread.Sleep(50);
          lListener.Close;
          Check(lCompleted.Wait(3000), 'adapter closed accept callback');
          try
            lListener.EndAccept(lAccept);
            Check(false, 'closed accept throws');
          except
            on e: RemObjects.InternetPack.SocketAcceptInterruptedException do
              Check(true, 'adapter preserves intentional interruption');
          end;
        end;
      end;
      using lSocket := NewSocket do begin
        try
          lSocket.Accept;
          Check(false, 'invalid accept throws');
        except
          on e: RemObjects.InternetPack.SocketException do
            Check(e.ErrorCode <> 0, 'adapter preserves native error');
        end;
      end;
    end;

    class method TestAsyncFailure;
    begin
      using lSocket := NewSocket do
      using lCompleted := new EventWaitHandle(false, false) do begin
        lSocket.Close;
        var lBuffer := new Byte[1];
        var lRead := lSocket.BeginReceive(lBuffer, 0, 1, IPFlags.None, ar -> lCompleted.Set, nil);
        Check(lCompleted.Wait(3000), 'failed async receive callback');
        try
          lSocket.EndReceive(lRead);
          Check(false, 'failed async receive throws');
        except
          on e: RemObjects.InternetPack.SocketException do
            Check(e.ErrorCode <> 0, 'failed async receive preserves native error');
        end;
      end;
    end;

    class method TestHttp;
    begin
      using lListener := NewSocket do begin
        lListener.Bind(new IPEndPoint(IPAddress.Parse('127.0.0.1'), 0));
        lListener.Listen(1);
        using lCompleted := new EventWaitHandle(false, false) do begin
          var lFailure: Exception;
          var lServer := new Thread(-> begin
            try
              using lAccepted := lListener.Accept do
              using lConnection := new RemObjects.InternetPack.Connection(lAccepted) do begin
                lConnection.TimeoutEnabled := false;
                // TCP may split the request across writes. Consume all headers
                // before closing the connection after the response.
                while length(lConnection.ReadLine) > 0 do begin end;
                var lResponse := RemObjects.Elements.RTL.Encoding.ASCII.GetBytes('HTTP/1.1 200 OK'#13#10'Content-Length: 2'#13#10'Connection: close'#13#10#13#10'OK');
                SendAll(lAccepted, lResponse);
              end;
            except
              on e: Exception do
                lFailure := e;
            end;
            lCompleted.Set;
          end);
          lServer.Start;
          var lClient := new RemObjects.InternetPack.Http.HttpClient;
          lClient.KeepAlive := false;
          using lResponse := lClient.GetResponse('http://127.0.0.1:'+IPEndPoint(lListener.LocalEndPoint).Port.ToString+'/') do
            Check(lResponse.ContentString = 'OK', 'plain HTTP client response');
          Check(lCompleted.Wait(3000) and (lFailure = nil), 'HTTP peer completed');
        end;
      end;
    end;

  public

    class method Main(aArgs: array of String): Integer;
    begin
      try
        Check(length(RemObjects.InternetPack.Dns.DnsLookup.ResolveAll('localhost')) > 0, 'adapter DNS');
        Check(IPAddress.Parse('fe80::1234%7').ScopeId = 7, 'adapter scoped IPv6');
        TestAsync(false);
        TestAsync(true);
        TestAcceptShutdown;
        TestAsyncFailure;
        writeLn('INTERNETPACK TRANSPORT TESTS PASSED: '+fChecks.ToString);
        if (length(aArgs) = 0) or (aArgs[0] <> '--http') then
          exit 0;
        TestHttp;
        writeLn('INTERNETPACK SOCKET TESTS PASSED: '+fChecks.ToString);
      except
        on e: Exception do begin
          writeLn(e.ToString);
          exit 1;
        end;
      end;
    end;

  end;

end.
