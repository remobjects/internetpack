namespace RemObjects.InternetPack.Tests;

uses
  RemObjects.InternetPack,
  RemObjects.InternetPack.WebSocket;

type
  MemoryConnection = public class(Connection)
  public

    constructor(aInput: array of Byte);
    begin
      inherited constructor(nil as System.Net.Sockets.Socket);
      fInput := aInput;
    end;

    property Written := new RemObjects.Elements.RTL.List<Byte>;
    property DataSocketConnected: Boolean read true; override;
    property DataSocketAvailable: Int32 read fInput.Length - fOffset; override;

  protected

    method DataSocketReceiveWhatsAvaiable(aBuffer: array of Byte; aOffset: Int32; aSize: Int32): Int32; override;
    begin
      result := Math.Min(aSize, fInput.Length - fOffset);
      System.Array.Copy(fInput, fOffset, aBuffer, aOffset, result);
      inc(fOffset, result);
    end;

    method DataSocketSendAsMuchAsPossible(aBuffer: array of Byte; aOffset: Int32; aSize: Int32): Int32; override;
    begin
      for i: Integer := 0 to aSize - 1 do
        Written.Add(aBuffer[aOffset + i]);
      System.Threading.Thread.Yield;
      result := aSize;
    end;

  private

    field fInput: array of Byte;
    field fOffset: Integer;

  end;

  Program = public static class
  private

    class method Check(aValue: Boolean; aMessage: String);
    begin
      if not aValue then
        raise new Exception(aMessage);
    end;

    class method Reject(aInput: array of Byte);
    begin
      var lRejected := false;
      try
        new WebSocketConnection(new MemoryConnection(aInput), false).ReadMessage;
      except
        on WebSocketException do
          lRejected := true;
      end;
      Check(lRejected, "Invalid frame must be rejected");
    end;

  public

    class method Main;
    begin
      var lConnection := new MemoryConnection([Byte(1), 1, $c3, $89, 1, 7, $80, 1, $a9]);
      var lSocket := new WebSocketConnection(lConnection, false);
      Check(lSocket.ReadTextMessage = "é", "UTF-8 must decode after fragmented message reassembly");
      Check((lConnection.Written.Count = 3) and (lConnection.Written[0] = $8a) and
        (lConnection.Written[2] = 7), "Interleaved ping must produce pong");
      Reject([Byte($80), 0]);
      Reject([Byte($09), 0]);
      Reject([Byte($89), 126]);
      Reject([Byte($c1), 0]);
      Reject([Byte($83), 0]);
      Reject([Byte(1), 0, $81, 0]);

      var lWrites := new MemoryConnection([]);
      var lConcurrent := new WebSocketConnection(lWrites, false);
      var lText := System.Threading.Tasks.Task.Run(-> begin
        for i: Integer := 1 to 1000 do
          lConcurrent.SendText("abc");
      end);
      var lControl := System.Threading.Tasks.Task.Run(-> begin
        for i: Integer := 1 to 1000 do
          lConcurrent.SendPong([Byte(7)]);
      end);
      System.Threading.Tasks.Task.WaitAll([lText, lControl]);
      var lPosition := 0;
      var lCount := 0;
      while lPosition < lWrites.Written.Count do begin
        var lOpcode := lWrites.Written[lPosition];
        var lLength := lWrites.Written[lPosition + 1];
        Check(((lOpcode = $81) and (lLength = 3)) or ((lOpcode = $8a) and (lLength = 1)), "Concurrent writes must preserve frame boundaries");
        if lOpcode = $81 then
          Check((lWrites.Written[lPosition + 2] = 97) and (lWrites.Written[lPosition + 4] = 99), "Text payload intact")
        else
          Check(lWrites.Written[lPosition + 2] = 7, "Control payload intact");
        inc(lPosition, 2 + lLength);
        inc(lCount);
      end;
      Check(lCount = 2000, "Every frame written exactly once");
      writeLn("PASS: fragmentation, control frames, malformed frames, and 2000 concurrent frame writes");
    end;

  end;

end.
