namespace RemObjects.InternetPack.Tests;

uses RemObjects.InternetPack.WebSocket;

type
  Program = public static class
  private
    class method Check(aValue: Boolean; aMessage: String);
    begin
      if not aValue then
        raise new Exception(aMessage);
    end;
  public
    class method Main(aArgs: array of String);
    begin
      if (length(aArgs) > 1) and (aArgs[1] = 'reject') then begin
        var lRejected := false;
        try
          new WebSocketClient().ConnectWebSocket(aArgs[0]).Dispose;
        except
          on Exception do
            lRejected := true;
        end;
        Check(lRejected, 'Untrusted TLS certificate must be rejected');
        writeLn('PASS: untrusted TLS certificate rejected');
        exit;
      end;
      if length(aArgs) > 0 then begin
        var lSecure := new WebSocketClient().ConnectWebSocket(aArgs[0]);
        lSecure.SendText('InternetPack secure WebSocket probe');
        Check(lSecure.ReadTextMessage = 'InternetPack secure WebSocket probe', 'Secure echo');
        lSecure.Dispose;
        writeLn('PASS: secure WebSocket handshake and text echo');
        exit;
      end;
      var lHeaders := new Dictionary<String, String>;
      lHeaders['X-IP-Test'] := 'native';
      var lSocket := new WebSocketClient().ConnectWebSocket('ws://127.0.0.1:18765/', lHeaders);
      lSocket.SendText('é ocean');
      Check(lSocket.ReadTextMessage = 'é ocean', 'Fragmented UTF-8 text and ping');
      var lBytes := new Byte[40000];
      for i: Integer := 0 to lBytes.Length - 1 do
        lBytes[i] := Byte(i mod 251);
      lSocket.SendBinary(lBytes);
      var lReply := lSocket.ReadMessage;
      Check((lReply.Opcode = WebSocketOpcode.Binary) and (lReply.Payload.Length = lBytes.Length), 'Binary length and type');
      for i: Integer := 0 to lBytes.Length - 1 do
        Check(lReply.Payload[i] = lBytes[i], 'Binary contents');
      lSocket.SendText('');
      Check(lSocket.ReadTextMessage = '', 'Empty message');
      var lWritersDone := new &Event;
      var lWriters := lSocket;
      Thread.Async(-> begin
        for i: Integer := 1 to 20 do
          lWriters.SendText('async-' + i.ToString);
        lWritersDone.Set;
      end);
      for i: Integer := 1 to 20 do
        lSocket.SendText('main-' + i.ToString);
      var lReplies := new HashSet<String>;
      for i: Integer := 1 to 40 do
        Check(lReplies.Add(lSocket.ReadTextMessage), 'Concurrent sends delivered once');
      Check(lWritersDone.WaitFor(5000), 'Concurrent writer finished');
      Check(lReplies.Count = 40, 'All concurrent messages received');
      lSocket.SendText('close');
      Check(lSocket.ReadTextMessage = nil, 'Peer close');
      lSocket.Dispose;

      lSocket := new WebSocketClient().ConnectWebSocket('ws://127.0.0.1:18765/', lHeaders);
      var lDone := new &Event;
      var lStarted := new &Event;
      var lReading := lSocket;
      Thread.Async(-> begin
        lStarted.Set;
        try
          lReading.ReadMessage;
        except
          on Exception do begin end;
        end;
        lDone.Set;
      end);
      lStarted.WaitFor;
      Thread.Sleep(100);
      lSocket.Dispose;
      Check(lDone.WaitFor(5000), 'Dispose must unblock receive');
      lSocket.Dispose;
      writeLn('PASS: platform headers, fragmented UTF-8, binary, empty messages, peer close, and receive cancellation');
    end;
  end;
end.
