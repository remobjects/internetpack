namespace SampleServer;

interface

uses
  System.Windows.Forms,
  System.Drawing,
  RemObjects.InternetPack.StandardServers,
  RemObjects.InternetPack.Http,
  System.IO;
type
  MainForm = class(System.Windows.Forms.Form)
  {$REGION Windows Form Designer generated fields}
  private
    components: System.ComponentModel.Container := nil;
    method InitializeComponent;
  assembly
      pictureBox1: System.Windows.Forms.PictureBox;
      Label2: System.Windows.Forms.Label;
      txtLog: System.Windows.Forms.TextBox;
      btnAction: System.Windows.Forms.Button;
      lblLink: System.Windows.Forms.LinkLabel;
      lblPort: System.Windows.Forms.Label;
      nudPort: System.Windows.Forms.NumericUpDown;
      lblRoot: System.Windows.Forms.Label;
      txtRoot: System.Windows.Forms.TextBox;
      txtServerName: System.Windows.Forms.TextBox;
      lblServerName: System.Windows.Forms.Label;
      lblCount: System.Windows.Forms.Label;
      nudCount: System.Windows.Forms.NumericUpDown;
      lblUrl: System.Windows.Forms.Label;
    GroupBox1: System.Windows.Forms.GroupBox;
  {$ENDREGION}
  private
    fEchoServer: EchoServer;
    fHttpServer: SimpleHttpServer;
    method nudPort_ValueChanged(sender: System.Object; e: System.EventArgs);
    method MainForm_Closed(sender: System.Object; e: System.EventArgs);
    method MainForm_Load(sender: System.Object; e: System.EventArgs);
    method lblLink_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
    method ActivateServers();
    method DeactivateServers();
    method SetEnable(mode: Boolean);
    method AddLog(line: String);
    method LogRequest(aSender: object; ea: OnHttpRequestArgs);
  method btnAction_Click(sender: System.Object; e: System.EventArgs);
  protected
    method Dispose(aDisposing: boolean); override;
  public
    constructor;
    class method Main;
  end;


  LogRequestDelegate = delegate(request: String);


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
  var resources: System.ComponentModel.ComponentResourceManager := new System.ComponentModel.ComponentResourceManager(typeOf(MainForm));
  self.GroupBox1 := new System.Windows.Forms.GroupBox();
  self.lblUrl := new System.Windows.Forms.Label();
  self.nudCount := new System.Windows.Forms.NumericUpDown();
  self.lblCount := new System.Windows.Forms.Label();
  self.lblServerName := new System.Windows.Forms.Label();
  self.txtServerName := new System.Windows.Forms.TextBox();
  self.txtRoot := new System.Windows.Forms.TextBox();
  self.lblRoot := new System.Windows.Forms.Label();
  self.nudPort := new System.Windows.Forms.NumericUpDown();
  self.lblPort := new System.Windows.Forms.Label();
  self.lblLink := new System.Windows.Forms.LinkLabel();
  self.btnAction := new System.Windows.Forms.Button();
  self.txtLog := new System.Windows.Forms.TextBox();
  self.Label2 := new System.Windows.Forms.Label();
  self.pictureBox1 := new System.Windows.Forms.PictureBox();
  self.GroupBox1.SuspendLayout();
  (self.nudCount as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.nudPort as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).BeginInit();
  self.SuspendLayout();
  // 
  // GroupBox1
  // 
  self.GroupBox1.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.GroupBox1.Controls.Add(self.lblUrl);
  self.GroupBox1.Controls.Add(self.nudCount);
  self.GroupBox1.Controls.Add(self.lblCount);
  self.GroupBox1.Controls.Add(self.lblServerName);
  self.GroupBox1.Controls.Add(self.txtServerName);
  self.GroupBox1.Controls.Add(self.txtRoot);
  self.GroupBox1.Controls.Add(self.lblRoot);
  self.GroupBox1.Controls.Add(self.nudPort);
  self.GroupBox1.Controls.Add(self.lblPort);
  self.GroupBox1.Controls.Add(self.lblLink);
  self.GroupBox1.Controls.Add(self.btnAction);
  self.GroupBox1.Location := new System.Drawing.Point(8, 47);
  self.GroupBox1.Name := 'GroupBox1';
  self.GroupBox1.Size := new System.Drawing.Size(528, 144);
  self.GroupBox1.TabIndex := 0;
  self.GroupBox1.TabStop := false;
  self.GroupBox1.Text := 'HttpServer';
  // 
  // lblUrl
  // 
  self.lblUrl.AutoSize := true;
  self.lblUrl.Enabled := false;
  self.lblUrl.Location := new System.Drawing.Point(98, 120);
  self.lblUrl.Name := 'lblUrl';
  self.lblUrl.Size := new System.Drawing.Size(32, 13);
  self.lblUrl.TabIndex := 8;
  self.lblUrl.Text := 'URL:';
  // 
  // nudCount
  // 
  self.nudCount.Location := new System.Drawing.Point(139, 88);
  self.nudCount.Name := 'nudCount';
  self.nudCount.Size := new System.Drawing.Size(48, 20);
  self.nudCount.TabIndex := 7;
  self.nudCount.Value := new System.Decimal(array of System.Int32([5,
      0,
      0,
      0]));
  // 
  // lblCount
  // 
  self.lblCount.AutoSize := true;
  self.lblCount.Location := new System.Drawing.Point(15, 90);
  self.lblCount.Name := 'lblCount';
  self.lblCount.Size := new System.Drawing.Size(115, 13);
  self.lblCount.TabIndex := 6;
  self.lblCount.Text := 'Listener Thread Count:';
  // 
  // lblServerName
  // 
  self.lblServerName.AutoSize := true;
  self.lblServerName.Location := new System.Drawing.Point(58, 67);
  self.lblServerName.Name := 'lblServerName';
  self.lblServerName.Size := new System.Drawing.Size(72, 13);
  self.lblServerName.TabIndex := 4;
  self.lblServerName.Text := '&Server Name:';
  // 
  // txtServerName
  // 
  self.txtServerName.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.txtServerName.Location := new System.Drawing.Point(139, 64);
  self.txtServerName.Name := 'txtServerName';
  self.txtServerName.Size := new System.Drawing.Size(376, 20);
  self.txtServerName.TabIndex := 5;
  self.txtServerName.Text := 'TextBox3';
  // 
  // txtRoot
  // 
  self.txtRoot.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.txtRoot.Location := new System.Drawing.Point(139, 40);
  self.txtRoot.Name := 'txtRoot';
  self.txtRoot.Size := new System.Drawing.Size(376, 20);
  self.txtRoot.TabIndex := 3;
  self.txtRoot.Text := 'TextBox2';
  // 
  // lblRoot
  // 
  self.lblRoot.AutoSize := true;
  self.lblRoot.Location := new System.Drawing.Point(75, 43);
  self.lblRoot.Name := 'lblRoot';
  self.lblRoot.Size := new System.Drawing.Size(55, 13);
  self.lblRoot.TabIndex := 2;
  self.lblRoot.Text := '&RootPath:';
  // 
  // nudPort
  // 
  self.nudPort.Location := new System.Drawing.Point(139, 16);
  self.nudPort.Name := 'nudPort';
  self.nudPort.Size := new System.Drawing.Size(48, 20);
  self.nudPort.TabIndex := 1;
  self.nudPort.Value := new System.Decimal(array of System.Int32([83,
      0,
      0,
      0]));
  self.nudPort.ValueChanged += new System.EventHandler(@self.nudPort_ValueChanged);
  // 
  // lblPort
  // 
  self.lblPort.AutoSize := true;
  self.lblPort.Location := new System.Drawing.Point(101, 18);
  self.lblPort.Name := 'lblPort';
  self.lblPort.Size := new System.Drawing.Size(29, 13);
  self.lblPort.TabIndex := 0;
  self.lblPort.Text := '&Port:';
  // 
  // lblLink
  // 
  self.lblLink.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.lblLink.Enabled := false;
  self.lblLink.Location := new System.Drawing.Point(139, 120);
  self.lblLink.Name := 'lblLink';
  self.lblLink.Size := new System.Drawing.Size(168, 16);
  self.lblLink.TabIndex := 9;
  self.lblLink.TabStop := true;
  self.lblLink.Text := 'http://localhost:82/index.html';
  self.lblLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(@self.lblLink_LinkClicked);
  // 
  // btnAction
  // 
  self.btnAction.Anchor := ((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.btnAction.Location := new System.Drawing.Point(403, 115);
  self.btnAction.Name := 'btnAction';
  self.btnAction.Size := new System.Drawing.Size(112, 23);
  self.btnAction.TabIndex := 10;
  self.btnAction.Text := 'Activate Servers';
  self.btnAction.Click += new System.EventHandler(@self.btnAction_Click);
  // 
  // txtLog
  // 
  self.txtLog.Anchor := ((((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Bottom) 
        or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.txtLog.Location := new System.Drawing.Point(8, 223);
  self.txtLog.Multiline := true;
  self.txtLog.Name := 'txtLog';
  self.txtLog.ScrollBars := System.Windows.Forms.ScrollBars.Both;
  self.txtLog.Size := new System.Drawing.Size(528, 222);
  self.txtLog.TabIndex := 2;
  self.txtLog.WordWrap := false;
  // 
  // Label2
  // 
  self.Label2.Location := new System.Drawing.Point(8, 199);
  self.Label2.Name := 'Label2';
  self.Label2.Size := new System.Drawing.Size(32, 16);
  self.Label2.TabIndex := 1;
  self.Label2.Text := 'Log';
  // 
  // pictureBox1
  // 
  self.pictureBox1.Image := (resources.GetObject('pictureBox1.Image') as System.Drawing.Image);
  self.pictureBox1.Location := new System.Drawing.Point(8, 7);
  self.pictureBox1.Name := 'pictureBox1';
  self.pictureBox1.Size := new System.Drawing.Size(120, 30);
  self.pictureBox1.TabIndex := 17;
  self.pictureBox1.TabStop := false;
  // 
  // MainForm
  // 
  self.AutoScaleBaseSize := new System.Drawing.Size(5, 13);
  self.ClientSize := new System.Drawing.Size(534, 442);
  self.Controls.Add(self.GroupBox1);
  self.Controls.Add(self.txtLog);
  self.Controls.Add(self.Label2);
  self.Controls.Add(self.pictureBox1);
  self.FormBorderStyle := System.Windows.Forms.FormBorderStyle.FixedDialog;
  self.Icon := (resources.GetObject('$this.Icon') as System.Drawing.Icon);
  self.MinimumSize := new System.Drawing.Size(550, 480);
  self.Name := 'MainForm';
  self.StartPosition := System.Windows.Forms.FormStartPosition.CenterScreen;
  self.Text := 'Internet Pack Sample Server';
  self.Load += new System.EventHandler(@self.MainForm_Load);
  self.Closed += new System.EventHandler(@self.MainForm_Closed);
  self.GroupBox1.ResumeLayout(false);
  self.GroupBox1.PerformLayout();
  (self.nudCount as System.ComponentModel.ISupportInitialize).EndInit();
  (self.nudPort as System.ComponentModel.ISupportInitialize).EndInit();
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).EndInit();
  self.ResumeLayout(false);
  self.PerformLayout();
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

method MainForm.btnAction_Click(sender: System.Object; e: System.EventArgs);
begin
  If btnAction.Text = 'Activate Servers' Then
      ActivateServers()
    Else
      DeactivateServers();
end;

 method MainForm.ActivateServers();
 begin
    AddLog('Trying to activate servers...');
    fEchoServer := New EchoServer();
    try
        fEchoServer.Open();
        AddLog('EchoServer is active.');
    except on ex: Exception do
        AddLog('Can''t activate EchoServer. An exception occured: ' + ex.Message);
    end;


    fHttpServer := New SimpleHttpServer();
    fHttpServer.Port := Convert.ToInt32(nudPort.Value);
    fHttpServer.RootPath := txtRoot.Text;
    fHttpServer.ServerName := txtServerName.Text;
    fHttpServer.Binding.ListenerThreadCount := Convert.ToInt32(nudCount.Value);
    //fHttpServer.OnHttpRequest += new LogRequestDelegate(@Self.LogRequest);
    fHttpServer.OnHttpRequest += new OnHttpRequestHandler(LogRequest);
    fHttpServer.Open();
    AddLog(String.Format('SimpleHttpServer is active on {0} port.', fHttpServer.Port));
    SetEnable(False);
    AddLog('Servers activated.');
    btnAction.Text := 'Deactivate Servers';
  end;
   
  method MainForm.LogRequest(aSender: object; ea: OnHttpRequestArgs);
  begin
    AddLog(String.Format('Request to {0}', ea.Request.Header.RequestPath))
  end;

   
  method MainForm.SetEnable(mode: Boolean);
  begin
    lblPort.Enabled := mode;
    nudPort.Enabled := mode;
    lblServerName.Enabled := mode;
    txtServerName.Enabled := mode;
    lblRoot.Enabled := mode;
    txtRoot.Enabled := mode;
    lblUrl.Enabled := Not mode;
    lblLink.Enabled := Not mode;
  end;
  
  method MainForm.DeactivateServers();
  begin
    AddLog('Trying to deactivate servers...');    
    If Assigned(fEchoServer) Then
      fEchoServer.Close();
    AddLog('EchoServer is closed.');
    If Assigned(fHttpServer) Then
      fHttpServer.Close();
    AddLog('HttpServer is closed.');
    SetEnable(True);
    AddLog('Servers is deactivated');
    btnAction.Text := 'Activate Servers';
  end;

method MainForm.lblLink_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
begin
  if (File.Exists(fHttpServer.RootPath + '\index.html')) then
    System.Diagnostics.Process.Start(lblLink.Text)
  else 
    MessageBox.Show(fHttpServer.RootPath + '\index.html can not be opened, because it does not exists.', 'Warning', 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
end;

method MainForm.MainForm_Load(sender: System.Object; e: System.EventArgs);
begin
  txtRoot.Text := Path.Combine(Path.GetDirectoryName(Self.GetType().Assembly.Location), 'HttpRoot');
  txtServerName.Text := 'Internet Pack HTTP Server';
  nudPort.Value := 82;
  nudCount.Value := 5;
end;

method MainForm.AddLog(line: String);
begin
  txtLog.Invoke(method begin
      txtLog.AppendText(Environment.NewLine + String.Format('{0}: {1}', DateTime.Now.ToLongTimeString(), line));
  end);
end;

method MainForm.MainForm_Closed(sender: System.Object; e: System.EventArgs);
begin
  DeactivateServers();
end;

method MainForm.nudPort_ValueChanged(sender: System.Object; e: System.EventArgs);
begin
  lblLink.Text := String.Format('http://localhost:{0}/index.html', nudPort.Value);
end;

end.