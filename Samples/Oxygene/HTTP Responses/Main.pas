namespace HTTPResponses;

interface

uses
  System.Windows.Forms,
  System.Drawing,
  System.Diagnostics,
  System.IO;
  
type
  /// <summary>
  /// Summary description for MainForm.
  /// </summary>
  MainForm = class(System.Windows.Forms.Form)
  {$REGION Windows Form Designer generated fields}
  private
    components: System.ComponentModel.IContainer;
    lb_Log: System.Windows.Forms.ListBox;
    httpServer: RemObjects.InternetPack.Http.HttpServer;
    llblinkLabel1: System.Windows.Forms.LinkLabel;
    method InitializeComponent;
  {$ENDREGION}
  private
    method httpServer_OnHttpRequest(aSender: System.Object; ea: RemObjects.InternetPack.Http.OnHttpRequestArgs);
    method MainForm_Closed(sender: System.Object; e: System.EventArgs);
    method MainForm_Load(sender: System.Object; e: System.EventArgs);
    method llblinkLabel1_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
  protected
    method Dispose(aDisposing: boolean); override;
  public
    constructor;
    class method Main;
  end;

implementation

{$REGION Construction and Disposition}
constructor MainForm;
begin
  //
  // Required for Windows Form Designer support
  //
  InitializeComponent();

  //
  // TODO: Add any constructor code after InitializeComponent call
  //
end;

method MainForm.Dispose(aDisposing: boolean);
begin
  if aDisposing then begin
    if assigned(components) then
      components.Dispose();

    //
    // TODO: Add custom disposition code here
    //
  end;
  inherited Dispose(aDisposing);
end;
{$ENDREGION}

{$REGION Windows Form Designer generated code}
method MainForm.InitializeComponent;
begin
  self.components := new System.ComponentModel.Container();
  var resources: System.ComponentModel.ComponentResourceManager := new System.ComponentModel.ComponentResourceManager(typeOf(MainForm));
  self.llblinkLabel1 := new System.Windows.Forms.LinkLabel();
  self.httpServer := new RemObjects.InternetPack.Http.HttpServer(self.components);
  self.lb_Log := new System.Windows.Forms.ListBox();
  self.SuspendLayout();
  // 
  // llblinkLabel1
  // 
  self.llblinkLabel1.Location := new System.Drawing.Point(8, 10);
  self.llblinkLabel1.Name := 'llblinkLabel1';
  self.llblinkLabel1.Size := new System.Drawing.Size(100, 23);
  self.llblinkLabel1.TabIndex := 2;
  self.llblinkLabel1.TabStop := true;
  self.llblinkLabel1.Text := 'http://localhost:82';
  self.llblinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(@self.llblinkLabel1_LinkClicked);
  // 
  // httpServer
  // 
  self.httpServer.Port := 82;
  self.httpServer.ValidateRequests := false;
  self.httpServer.OnHttpRequest += new RemObjects.InternetPack.Http.OnHttpRequestHandler(@self.httpServer_OnHttpRequest);
  // 
  // lb_Log
  // 
  self.lb_Log.Anchor := ((((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Bottom) 
        or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.lb_Log.IntegralHeight := false;
  self.lb_Log.Location := new System.Drawing.Point(11, 32);
  self.lb_Log.Name := 'lb_Log';
  self.lb_Log.Size := new System.Drawing.Size(371, 228);
  self.lb_Log.TabIndex := 3;
  // 
  // MainForm
  // 
  self.AutoScaleBaseSize := new System.Drawing.Size(5, 13);
  self.ClientSize := new System.Drawing.Size(394, 272);
  self.Controls.Add(self.lb_Log);
  self.Controls.Add(self.llblinkLabel1);
  self.Icon := (resources.GetObject('$this.Icon') as System.Drawing.Icon);
  self.MinimumSize := new System.Drawing.Size(410, 310);
  self.Name := 'MainForm';
  self.Text := 'Internet Pack HTTP Response Sample';
  self.Load += new System.EventHandler(@self.MainForm_Load);
  self.Closed += new System.EventHandler(@self.MainForm_Closed);
  self.ResumeLayout(false);
end;
{$ENDREGION}

{$REGION Application Entry Point}
[STAThread]
class method MainForm.Main;
begin
  Application.EnableVisualStyles();

  try
    with lForm := new MainForm() do
      Application.Run(lForm);
  except
    on E: Exception do begin
      MessageBox.Show(E.Message);
    end;
  end;
end;
{$ENDREGION}

const sWelcome: String =
      'Internet Pack HTTP Responses Test App'+
      '<br /><br />'+
      'Valid links:'+
      '<br />'+
      '<a href=/home>/home</a> show this page</a>'+
      '<br />'+
      '<a href=/file>/file</a> send back a file (this .exe)'+
      '<br />'+
      '<a href=/bytes>/bytes</a> send back a buffer of random bytes'+
      '<br />'+
      '<a href=/error>/error</a> Display a custom error';

method MainForm.llblinkLabel1_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
begin
  Process.Start(llblinkLabel1.Text);
end;

method MainForm.MainForm_Load(sender: System.Object; e: System.EventArgs);
begin
  httpServer.Active := true;
end;

method MainForm.MainForm_Closed(sender: System.Object; e: System.EventArgs);
begin
  httpServer.Active := false;
end;

method MainForm.httpServer_OnHttpRequest(aSender: System.Object; ea: RemObjects.InternetPack.Http.OnHttpRequestArgs);
var 
    lBuffer: array of byte;
    lExeName: String;
    lRandom: Random;
begin
  lb_Log.Invoke(method begin 
      lb_Log.Items.Add(ea.Request.Header.RequestPath);
    end);

  case (ea.Request.Header.RequestPath) of
    //---------------------------------------------------
    '/', '/home': begin
      ea.Response.ContentString := sWelcome;
      ea.Response.Header.SetHeaderValue('Content-Type', 'text/html');
    end;
    //---------------------------------------------------
    '/bytes': begin
      lBuffer := new byte[256];
      lRandom := new Random();
      lRandom.NextBytes(lBuffer);
      ea.Response.ContentBytes := lBuffer;
      ea.Response.Header.SetHeaderValue('Content-Disposition', 'filename=random.bin');
      ea.Response.Header.SetHeaderValue('Content-Type', 'application/binary');
    end;
    //---------------------------------------------------
    '/error': begin
      ea.Response.SendError(555, 'Custom Error', 'A custom error message');
    end;
    //---------------------------------------------------
    '/file': begin
      lExeName := Self.GetType().Assembly.Location;
      try
        ea.Response.ContentStream := 
          new FileStream(
            lExeName, 
            FileMode.Open, 
            FileAccess.Read, 
            FileShare.Read
          );
        ea.Response.Header.SetHeaderValue(
          'Content-Disposition',
          String.Format('filename="{0}"', 
          Path.GetFileName(lExeName))
        );
        ea.Response.Header.SetHeaderValue('Content-Type', 'application/binary');
        ea.Response.CloseStream := true; 
      except
          on e: Exception do begin               
            ea.Response.SendError(404, String.Format('File {0} not found', lExeName), e);
          end;
      end;  
  end
    //---------------------------------------------------
    else ea.Response.SendError(404, 'Requested path not found');
    //---------------------------------------------------
  end;  
end;

end.