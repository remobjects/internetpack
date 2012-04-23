namespace VirtualFTP;

interface

uses
  System.Windows.Forms,
  System.Drawing,
  System.IO,
  RemObjects.InternetPack.Ftp.VirtualFtp;

type
  /// <summary>
  /// Summary description for MainForm.
  /// </summary>
  MainForm = class(System.Windows.Forms.Form)
  {$REGION Windows Form Designer generated fields}
  private
    components: System.ComponentModel.Container := nil;
    method InitializeComponent;
  assembly
      pictureBox1: System.Windows.Forms.PictureBox;
      llShortcut: System.Windows.Forms.LinkLabel;
      label1: System.Windows.Forms.Label;
      txtLog: System.Windows.Forms.TextBox;
      GroupBox1: System.Windows.Forms.GroupBox;
    btnStartStop: System.Windows.Forms.Button;
  {$ENDREGION}
  private
    fRootFolder: VirtualFolder;
    fUserManager: IFtpUserManager;
    fFtpServer: VirtualFtpServer;
    port: Integer := 4444;
    method MainForm_Closed(sender: System.Object; e: System.EventArgs);
    method llShortcut_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
    method addToLog(line: String);
    method btnStartStop_Click(sender: System.Object; e: System.EventArgs);
  protected
    method Dispose(aDisposing: boolean); override;
  public
    constructor;
    class method Main;
    method StartServer(aPort: Integer);
    method StopServer();
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
  var resources: System.ComponentModel.ComponentResourceManager := new System.ComponentModel.ComponentResourceManager(typeOf(MainForm));
  self.btnStartStop := new System.Windows.Forms.Button();
  self.GroupBox1 := new System.Windows.Forms.GroupBox();
  self.txtLog := new System.Windows.Forms.TextBox();
  self.label1 := new System.Windows.Forms.Label();
  self.llShortcut := new System.Windows.Forms.LinkLabel();
  self.pictureBox1 := new System.Windows.Forms.PictureBox();
  self.GroupBox1.SuspendLayout();
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).BeginInit();
  self.SuspendLayout();
  // 
  // btnStartStop
  // 
  self.btnStartStop.Anchor := ((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.btnStartStop.Location := new System.Drawing.Point(231, 8);
  self.btnStartStop.Name := 'btnStartStop';
  self.btnStartStop.Size := new System.Drawing.Size(75, 23);
  self.btnStartStop.TabIndex := 13;
  self.btnStartStop.Text := 'Start';
  self.btnStartStop.Click += new System.EventHandler(@self.btnStartStop_Click);
  // 
  // GroupBox1
  // 
  self.GroupBox1.Anchor := ((((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Bottom) 
        or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.GroupBox1.Controls.Add(self.txtLog);
  self.GroupBox1.Location := new System.Drawing.Point(8, 103);
  self.GroupBox1.Name := 'GroupBox1';
  self.GroupBox1.Size := new System.Drawing.Size(298, 132);
  self.GroupBox1.TabIndex := 12;
  self.GroupBox1.TabStop := false;
  self.GroupBox1.Text := 'Log';
  // 
  // txtLog
  // 
  self.txtLog.Anchor := ((((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Bottom) 
        or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.txtLog.Location := new System.Drawing.Point(8, 16);
  self.txtLog.Multiline := true;
  self.txtLog.Name := 'txtLog';
  self.txtLog.ScrollBars := System.Windows.Forms.ScrollBars.Both;
  self.txtLog.Size := new System.Drawing.Size(282, 108);
  self.txtLog.TabIndex := 6;
  self.txtLog.WordWrap := false;
  // 
  // label1
  // 
  self.label1.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.label1.Location := new System.Drawing.Point(3, 38);
  self.label1.Name := 'label1';
  self.label1.Size := new System.Drawing.Size(309, 24);
  self.label1.TabIndex := 11;
  self.label1.Text := 'In order to login on ftp please use login: test; password: test.';
  self.label1.TextAlign := System.Drawing.ContentAlignment.MiddleCenter;
  // 
  // llShortcut
  // 
  self.llShortcut.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.llShortcut.Location := new System.Drawing.Point(4, 62);
  self.llShortcut.Name := 'llShortcut';
  self.llShortcut.Size := new System.Drawing.Size(300, 33);
  self.llShortcut.TabIndex := 10;
  self.llShortcut.TextAlign := System.Drawing.ContentAlignment.MiddleCenter;
  self.llShortcut.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(@self.llShortcut_LinkClicked);
  // 
  // pictureBox1
  // 
  self.pictureBox1.Image := (resources.GetObject('pictureBox1.Image') as System.Drawing.Image);
  self.pictureBox1.Location := new System.Drawing.Point(8, 7);
  self.pictureBox1.Name := 'pictureBox1';
  self.pictureBox1.Size := new System.Drawing.Size(120, 30);
  self.pictureBox1.SizeMode := System.Windows.Forms.PictureBoxSizeMode.AutoSize;
  self.pictureBox1.TabIndex := 9;
  self.pictureBox1.TabStop := false;
  // 
  // MainForm
  // 
  self.AutoScaleBaseSize := new System.Drawing.Size(5, 13);
  self.ClientSize := new System.Drawing.Size(314, 242);
  self.Controls.Add(self.btnStartStop);
  self.Controls.Add(self.GroupBox1);
  self.Controls.Add(self.label1);
  self.Controls.Add(self.llShortcut);
  self.Controls.Add(self.pictureBox1);
  self.Icon := (resources.GetObject('$this.Icon') as System.Drawing.Icon);
  self.MinimumSize := new System.Drawing.Size(330, 280);
  self.Name := 'MainForm';
  self.Text := 'Virtual FTP';
  self.Closed += new System.EventHandler(@self.MainForm_Closed);
  self.GroupBox1.ResumeLayout(false);
  self.GroupBox1.PerformLayout();
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

method MainForm.btnStartStop_Click(sender: System.Object; e: System.EventArgs);
begin
  If (btnStartStop.Text = 'Start') Then Begin
      addToLog('Starting Virtual FTP on ' + port.ToString() + ' port...');
      StartServer(port);
      llShortcut.Text := String.Format('ftp://localhost:{0}/', port);
      addToLog('Virtual FTP is running under ' + Environment.OSVersion.ToString());
      btnStartStop.Text := 'Stop';
  End Else Begin
      addToLog('Shutting down Virtual FTP ...');
      StopServer();
      llShortcut.Text := '';
      addToLog('Virtual FTP is stopped.');
      btnStartStop.Text := 'Start';
  End
end;

method MainForm.addToLog(line: String);
begin
  txtLog.Invoke(method begin
      txtLog.Text := txtLog.Text + 
        System.DateTime.Now.ToLongTimeString() +
        ': ' +
        line + 
        Environment.NewLine;
    end);
end;

method MainForm.StartServer(aPort: Integer);
var lDiskFolder: String;
begin
    lDiskFolder := Path.GetDirectoryName(typeof(Self).Assembly.Location) + '..\..\..\';

    fRootFolder := New VirtualFolder(nil, '[ROOT]');
    fRootFolder.Add(New VirtualFolder(nil, 'virtual'));
    fRootFolder.Add(New DiscFolder(nil, 'drive-c', 'c:\'));
    fRootFolder.Add(New DiscFolder(nil, 'disc', lDiskFolder));
    fRootFolder.Add(New EmptyFile(nil, '=== Welcome to the FTP ==='));

    fUserManager := New UserManager();
    UserManager(fUserManager).AddUser('test', 'test');

    fFtpServer := New VirtualFtpServer();
    fFtpServer.Port := aPort;
    fFtpServer.Timeout := 60 * 1000; {* 1 minute *}
    if fFtpServer.BindingV4<>nil then
      fFtpServer.BindingV4.ListenerThreadCount := 10
    else
      fFtpServer.BindingV6.ListenerThreadCount := 10;
    fFtpServer.RootFolder := fRootFolder;
    fFtpServer.UserManager := fUserManager;
    fFtpServer.ServerName := 'VirtualFTP Sample - powered by RemObjects Internet Pack for .NET';

    fFtpServer.Open();

    addToLog('VirtualFTP 0.3 BETA - started up');
  end;

method MainForm.StopServer();
begin
  If (fFTPServer<>nil) Then
    fFtpServer.Close()
end;
 
method MainForm.llShortcut_LinkClicked(sender: System.Object; e: System.Windows.Forms.LinkLabelLinkClickedEventArgs);
begin
  If llShortcut.Text <> '' Then
    System.Diagnostics.Process.Start(llShortcut.Text);
end;

method MainForm.MainForm_Closed(sender: System.Object; e: System.EventArgs);
begin
  StopServer();
end;

end.