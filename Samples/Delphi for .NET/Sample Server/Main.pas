unit Main;

interface

uses
  System.Drawing, System.Collections, System.ComponentModel,
  System.Windows.Forms, System.Data, System.Resources,
  System.IO, System.Diagnostics,
  RemObjects.InternetPack.StandardServers,
  RemObjects.InternetPack.Http;

type
  TWinForm2 = class(System.Windows.Forms.Form)
  {$REGION 'Designer Managed Code'}
  strict private
    /// <summary>
    /// Required designer variable.
    /// </summary>
    Components: System.ComponentModel.Container;
    pictureBox1: System.Windows.Forms.PictureBox;
    btn_Deactivate: System.Windows.Forms.Button;
    btn_Activate: System.Windows.Forms.Button;
    lbl_Link: System.Windows.Forms.LinkLabel;
    SimpleHttpServer1: RemObjects.InternetPack.Http.SimpleHttpServer;
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    procedure InitializeComponent;
    procedure btn_Activate_Click(sender: System.Object; e: System.EventArgs);
    procedure btn_Deactivate_Click(sender: System.Object; e: System.EventArgs);
    procedure TWinForm2_Closed(sender: System.Object; e: System.EventArgs);
    procedure LinkLabel1_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
  {$ENDREGION}
  strict protected
    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    procedure Dispose(Disposing: Boolean); override;
  private
    fEchoServer:EchoServer;
    fHttpServer:SimpleHttpServer;
  public
    constructor Create;
  end;

  [assembly: RuntimeRequiredAttribute(TypeOf(TWinForm2))]

implementation

{$REGION 'Windows Form Designer generated code'}
/// <summary>
/// Required method for Designer support -- do not modify
/// the contents of this method with the code editor.
/// </summary>
procedure TWinForm2.InitializeComponent;
var
  resources: System.Resources.ResourceManager;
begin
  resources := System.Resources.ResourceManager.Create(TypeOf(TWinForm2));
  Self.pictureBox1 := System.Windows.Forms.PictureBox.Create;
  Self.btn_Deactivate := System.Windows.Forms.Button.Create;
  Self.btn_Activate := System.Windows.Forms.Button.Create;
  Self.lbl_Link := System.Windows.Forms.LinkLabel.Create;
  Self.SimpleHttpServer1 := RemObjects.InternetPack.Http.SimpleHttpServer.Create;
  Self.SuspendLayout;
  // 
  // pictureBox1
  // 
  Self.pictureBox1.Anchor := (System.Windows.Forms.AnchorStyles((System.Windows.Forms.AnchorStyles.Bottom 
    or System.Windows.Forms.AnchorStyles.Left)));
  Self.pictureBox1.Image := (System.Drawing.Image(resources.GetObject('pictureBox1.Image')));
  Self.pictureBox1.Location := System.Drawing.Point.Create(8, 63);
  Self.pictureBox1.Name := 'pictureBox1';
  Self.pictureBox1.Size := System.Drawing.Size.Create(120, 30);
  Self.pictureBox1.TabIndex := 6;
  Self.pictureBox1.TabStop := False;
  // 
  // btn_Deactivate
  // 
  Self.btn_Deactivate.Enabled := False;
  Self.btn_Deactivate.Location := System.Drawing.Point.Create(128, 8);
  Self.btn_Deactivate.Name := 'btn_Deactivate';
  Self.btn_Deactivate.Size := System.Drawing.Size.Create(112, 23);
  Self.btn_Deactivate.TabIndex := 5;
  Self.btn_Deactivate.Text := 'Deactivate Servers';
  Include(Self.btn_Deactivate.Click, Self.btn_Deactivate_Click);
  // 
  // btn_Activate
  // 
  Self.btn_Activate.Location := System.Drawing.Point.Create(8, 8);
  Self.btn_Activate.Name := 'btn_Activate';
  Self.btn_Activate.Size := System.Drawing.Size.Create(112, 23);
  Self.btn_Activate.TabIndex := 4;
  Self.btn_Activate.Text := 'Activate Servers';
  Include(Self.btn_Activate.Click, Self.btn_Activate_Click);
  // 
  // lbl_Link
  // 
  Self.lbl_Link.Location := System.Drawing.Point.Create(8, 40);
  Self.lbl_Link.Name := 'lbl_Link';
  Self.lbl_Link.Size := System.Drawing.Size.Create(160, 16);
  Self.lbl_Link.TabIndex := 7;
  Self.lbl_Link.TabStop := True;
  Self.lbl_Link.Text := 'http://localhost:81/index.html';
  Include(Self.lbl_Link.LinkClicked, Self.LinkLabel1_LinkClicked);
  // 
  // SimpleHttpServer1
  // 
  Self.SimpleHttpServer1.Active := False;
  Self.SimpleHttpServer1.ConnectionClass := nil;
  Self.SimpleHttpServer1.RootPath := '';
  Self.SimpleHttpServer1.ValidateRequests := False;
  // 
  // TWinForm2
  // 
  Self.AutoScaleBaseSize := System.Drawing.Size.Create(5, 13);
  Self.ClientSize := System.Drawing.Size.Create(304, 101);
  Self.Controls.Add(Self.lbl_Link);
  Self.Controls.Add(Self.pictureBox1);
  Self.Controls.Add(Self.btn_Deactivate);
  Self.Controls.Add(Self.btn_Activate);
  Self.Icon := (System.Drawing.Icon(resources.GetObject('$this.Icon')));
  Self.Name := 'TWinForm2';
  Self.Text := 'Internet Pack Sample Server (Delphi)';
  Include(Self.Load, Self.btn_Activate_Click);
  Include(Self.Closed, Self.TWinForm2_Closed);
  Self.ResumeLayout(False);
end;
{$ENDREGION}

procedure TWinForm2.Dispose(Disposing: Boolean);
begin
  if Disposing then
  begin
    if Components <> nil then
      Components.Dispose();
  end;
  inherited Dispose(Disposing);
end;

constructor TWinForm2.Create;
begin
  inherited Create;
  //
  // Required for Windows Form Designer support
  //
  InitializeComponent;
  //
  // TODO: Add any constructor code after InitializeComponent call
  //
end;

procedure TWinForm2.LinkLabel1_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
begin
  Process.Start(lbl_Link.Text);
end;

procedure TWinForm2.TWinForm2_Closed(sender: System.Object; e: System.EventArgs);
begin
  btn_Deactivate_Click(self, e);
end;

procedure TWinForm2.btn_Activate_Click(sender: System.Object; e: System.EventArgs);
begin
  fEchoServer := EchoServer.Create();
  fEchoServer.Open();

  fHttpServer := SimpleHttpServer.Create();
  fHttpServer.Port := 81; // avoid conflict if IIS is installed, too
  fHttpServer.RootPath := Path.GetDirectoryName(GetType().Assembly.Location)+'\HttpRoot';
  fHttpServer.ServerName := 'Internet Pack HTTP Server';
  fHttpServer.Open();

  btn_Activate.Enabled := false;
  btn_Deactivate.Enabled := true;
end;

procedure TWinForm2.btn_Deactivate_Click(sender: System.Object; e: System.EventArgs);
begin
  if (fEchoServer <> nil) then fEchoServer.Close();
  if (fHttpServer <> nil) then fHttpServer.Close();
  btn_Activate.Enabled := true;
  btn_Deactivate.Enabled := false;
end;

end.
