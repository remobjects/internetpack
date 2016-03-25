namespace HTTPSpy;

interface

type
  Program = public static class
  public
    class method Main;
  end;
  
implementation

uses 
  System.Windows.Forms;

[STAThread]
class method Program.Main;
begin
  try
    Application.EnableVisualStyles();
    Application.Run(new MainForm());
  except
    on E: Exception do begin
      MessageBox.Show(E.Message);
    end;
  end;
end;

end.