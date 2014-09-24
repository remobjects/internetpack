namespace FtpSync;

interface

type
  ConsoleApp = class
  public
    class method Main(args: array of string);
  end;


implementation


class method ConsoleApp.Main(args: array of string);
begin
  with lWorker := new FtpSyncWorker() do
     if (lWorker.CheckArgs(args)) then lWorker.Sync();
end;


end.