namespace HTTPSpy;

interface

uses
  System.Windows.Forms,
  System.Data,
  System.Drawing,
  System.Text,
  RemObjects.InternetPack.Http;

type
  MainForm = class(System.Windows.Forms.Form)
  {$REGION Windows Form Designer generated field}
  private
    tabControl1: System.Windows.Forms.TabControl;
    dvHeaders: System.Data.DataView;
    dataGrid1: System.Windows.Forms.DataGrid;
    tabPage2: System.Windows.Forms.TabPage;
    splitter2: System.Windows.Forms.Splitter;
    dataGrid2: System.Windows.Forms.DataGrid;
    dvParams: System.Data.DataView;
    httpClient1: RemObjects.InternetPack.Http.HttpClient;
    cbKeepAlive: System.Windows.Forms.CheckBox;
    btnSubmit: System.Windows.Forms.Button;
    rbPost: System.Windows.Forms.RadioButton;
    label1: System.Windows.Forms.Label;
    rbGet: System.Windows.Forms.RadioButton;
    edUrl: System.Windows.Forms.TextBox;
    pictureBox1: System.Windows.Forms.PictureBox;
    pnlpanel2: System.Windows.Forms.Panel;
    dataColumn4: System.Data.DataColumn;
    dataColumn3: System.Data.DataColumn;
    tblParams: System.Data.DataTable;
    dataColumn2: System.Data.DataColumn;
    dataColumn1: System.Data.DataColumn;
    tblHeaders: System.Data.DataTable;
    dataSet1: System.Data.DataSet;
    dataGrid3: System.Windows.Forms.DataGrid;
    splitter1: System.Windows.Forms.Splitter;
    rbHex: System.Windows.Forms.RadioButton;
    pnlpanel1: System.Windows.Forms.Panel;
    edResult: System.Windows.Forms.TextBox;
    tabPage1: System.Windows.Forms.TabPage;
    rbText: System.Windows.Forms.RadioButton;
    dataColumn6: System.Data.DataColumn;
    dataColumn5: System.Data.DataColumn;
    tblResponseHeaders: System.Data.DataTable;
    components: System.ComponentModel.Container := nil;
    method InitializeComponent;
  {$ENDREGION}
  private
    _LastResultString : String;
    _LastResultBytes: Array of byte;
    _LastLength: Integer := 0;
    hexWidth: Integer := 16;

    method MainForm_Load(sender: System.Object; e: System.EventArgs);
    method rbText_CheckedChanged(sender: System.Object; e: System.EventArgs);
    method btnSubmit_Click(sender: System.Object; e: System.EventArgs);
    method AddHeader(Name: string; Value: string);
    method AddResponseHeader(Name: string; Value: string);
    method ShowResponse(aResponse: HttpClientResponse);
    method SetResultText();    
  protected
    method Dispose(aDisposing: boolean); override;
  public
    constructor;
  end;

implementation

{$REGION Construction and Disposition}
constructor MainForm;
begin
  //
  // Required for Windows Form Designer support
  //
  InitializeComponent();
end;

method MainForm.Dispose(aDisposing: boolean);
begin
  if aDisposing then begin
    if assigned(components) then
      components.Dispose();
  end;
  inherited Dispose(aDisposing);
end;
{$ENDREGION}

{$REGION Windows Form Designer generated code}
method MainForm.InitializeComponent;
begin
  var resources: System.ComponentModel.ComponentResourceManager := new System.ComponentModel.ComponentResourceManager(typeOf(MainForm));
  self.tblResponseHeaders := new System.Data.DataTable();
  self.dataColumn5 := new System.Data.DataColumn();
  self.dataColumn6 := new System.Data.DataColumn();
  self.rbText := new System.Windows.Forms.RadioButton();
  self.tabPage1 := new System.Windows.Forms.TabPage();
  self.edResult := new System.Windows.Forms.TextBox();
  self.pnlpanel1 := new System.Windows.Forms.Panel();
  self.rbHex := new System.Windows.Forms.RadioButton();
  self.splitter1 := new System.Windows.Forms.Splitter();
  self.dataGrid3 := new System.Windows.Forms.DataGrid();
  self.dataSet1 := new System.Data.DataSet();
  self.tblHeaders := new System.Data.DataTable();
  self.dataColumn1 := new System.Data.DataColumn();
  self.dataColumn2 := new System.Data.DataColumn();
  self.tblParams := new System.Data.DataTable();
  self.dataColumn3 := new System.Data.DataColumn();
  self.dataColumn4 := new System.Data.DataColumn();
  self.pnlpanel2 := new System.Windows.Forms.Panel();
  self.pictureBox1 := new System.Windows.Forms.PictureBox();
  self.edUrl := new System.Windows.Forms.TextBox();
  self.rbGet := new System.Windows.Forms.RadioButton();
  self.label1 := new System.Windows.Forms.Label();
  self.rbPost := new System.Windows.Forms.RadioButton();
  self.btnSubmit := new System.Windows.Forms.Button();
  self.cbKeepAlive := new System.Windows.Forms.CheckBox();
  self.httpClient1 := new RemObjects.InternetPack.Http.HttpClient();
  self.dvParams := new System.Data.DataView();
  self.dataGrid2 := new System.Windows.Forms.DataGrid();
  self.splitter2 := new System.Windows.Forms.Splitter();
  self.tabPage2 := new System.Windows.Forms.TabPage();
  self.dataGrid1 := new System.Windows.Forms.DataGrid();
  self.dvHeaders := new System.Data.DataView();
  self.tabControl1 := new System.Windows.Forms.TabControl();
  (self.tblResponseHeaders as System.ComponentModel.ISupportInitialize).BeginInit();
  self.tabPage1.SuspendLayout();
  self.pnlpanel1.SuspendLayout();
  (self.dataGrid3 as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.dataSet1 as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.tblHeaders as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.tblParams as System.ComponentModel.ISupportInitialize).BeginInit();
  self.pnlpanel2.SuspendLayout();
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.dvParams as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.dataGrid2 as System.ComponentModel.ISupportInitialize).BeginInit();
  self.tabPage2.SuspendLayout();
  (self.dataGrid1 as System.ComponentModel.ISupportInitialize).BeginInit();
  (self.dvHeaders as System.ComponentModel.ISupportInitialize).BeginInit();
  self.tabControl1.SuspendLayout();
  self.SuspendLayout();
  // 
  // tblResponseHeaders
  // 
  self.tblResponseHeaders.Columns.AddRange(array of System.Data.DataColumn([self.dataColumn5,
      self.dataColumn6]));
  self.tblResponseHeaders.TableName := 'ResponseHeaders';
  // 
  // dataColumn5
  // 
  self.dataColumn5.ColumnName := 'Name';
  // 
  // dataColumn6
  // 
  self.dataColumn6.ColumnName := 'Value';
  // 
  // rbText
  // 
  self.rbText.Anchor := ((System.Windows.Forms.AnchorStyles.Bottom or System.Windows.Forms.AnchorStyles.Left) as System.Windows.Forms.AnchorStyles);
  self.rbText.Checked := true;
  self.rbText.Location := new System.Drawing.Point(8, 4);
  self.rbText.Name := 'rbText';
  self.rbText.Size := new System.Drawing.Size(60, 20);
  self.rbText.TabIndex := 5;
  self.rbText.TabStop := true;
  self.rbText.Text := 'Text';
  self.rbText.CheckedChanged += new System.EventHandler(@self.rbText_CheckedChanged);
  // 
  // tabPage1
  // 
  self.tabPage1.Controls.Add(self.edResult);
  self.tabPage1.Controls.Add(self.pnlpanel1);
  self.tabPage1.Controls.Add(self.splitter1);
  self.tabPage1.Controls.Add(self.dataGrid3);
  self.tabPage1.Location := new System.Drawing.Point(4, 22);
  self.tabPage1.Name := 'tabPage1';
  self.tabPage1.Padding := new System.Windows.Forms.Padding(5);
  self.tabPage1.Size := new System.Drawing.Size(660, 480);
  self.tabPage1.TabIndex := 0;
  self.tabPage1.Text := 'Result';
  // 
  // edResult
  // 
  self.edResult.Dock := System.Windows.Forms.DockStyle.Fill;
  self.edResult.Location := new System.Drawing.Point(5, 213);
  self.edResult.Multiline := true;
  self.edResult.Name := 'edResult';
  self.edResult.ReadOnly := true;
  self.edResult.ScrollBars := System.Windows.Forms.ScrollBars.Vertical;
  self.edResult.Size := new System.Drawing.Size(650, 235);
  self.edResult.TabIndex := 4;
  // 
  // pnlpanel1
  // 
  self.pnlpanel1.Controls.Add(self.rbHex);
  self.pnlpanel1.Controls.Add(self.rbText);
  self.pnlpanel1.Dock := System.Windows.Forms.DockStyle.Bottom;
  self.pnlpanel1.Location := new System.Drawing.Point(5, 448);
  self.pnlpanel1.Name := 'pnlpanel1';
  self.pnlpanel1.Size := new System.Drawing.Size(650, 27);
  self.pnlpanel1.TabIndex := 10;
  // 
  // rbHex
  // 
  self.rbHex.Anchor := ((System.Windows.Forms.AnchorStyles.Bottom or System.Windows.Forms.AnchorStyles.Left) as System.Windows.Forms.AnchorStyles);
  self.rbHex.Location := new System.Drawing.Point(68, 4);
  self.rbHex.Name := 'rbHex';
  self.rbHex.Size := new System.Drawing.Size(60, 20);
  self.rbHex.TabIndex := 6;
  self.rbHex.Text := 'Hex';
  // 
  // splitter1
  // 
  self.splitter1.Dock := System.Windows.Forms.DockStyle.Top;
  self.splitter1.Location := new System.Drawing.Point(5, 208);
  self.splitter1.Name := 'splitter1';
  self.splitter1.Size := new System.Drawing.Size(650, 5);
  self.splitter1.TabIndex := 8;
  self.splitter1.TabStop := false;
  // 
  // dataGrid3
  // 
  self.dataGrid3.CaptionText := 'Result Headers';
  self.dataGrid3.DataMember := '';
  self.dataGrid3.DataSource := self.tblResponseHeaders;
  self.dataGrid3.Dock := System.Windows.Forms.DockStyle.Top;
  self.dataGrid3.HeaderForeColor := System.Drawing.SystemColors.ControlText;
  self.dataGrid3.Location := new System.Drawing.Point(5, 5);
  self.dataGrid3.Name := 'dataGrid3';
  self.dataGrid3.PreferredColumnWidth := 300;
  self.dataGrid3.ReadOnly := true;
  self.dataGrid3.Size := new System.Drawing.Size(650, 203);
  self.dataGrid3.TabIndex := 7;
  // 
  // dataSet1
  // 
  self.dataSet1.DataSetName := 'NewDataSet';
  self.dataSet1.Locale := new System.Globalization.CultureInfo('en-US');
  self.dataSet1.Tables.AddRange(array of System.Data.DataTable([self.tblHeaders,
      self.tblParams,
      self.tblResponseHeaders]));
  // 
  // tblHeaders
  // 
  self.tblHeaders.Columns.AddRange(array of System.Data.DataColumn([self.dataColumn1,
      self.dataColumn2]));
  self.tblHeaders.TableName := 'Headers';
  // 
  // dataColumn1
  // 
  self.dataColumn1.ColumnName := 'Name';
  // 
  // dataColumn2
  // 
  self.dataColumn2.ColumnName := 'Value';
  // 
  // tblParams
  // 
  self.tblParams.Columns.AddRange(array of System.Data.DataColumn([self.dataColumn3,
      self.dataColumn4]));
  self.tblParams.TableName := 'Params';
  // 
  // dataColumn3
  // 
  self.dataColumn3.ColumnName := 'Name';
  // 
  // dataColumn4
  // 
  self.dataColumn4.ColumnName := 'Value';
  // 
  // pnlpanel2
  // 
  self.pnlpanel2.Controls.Add(self.pictureBox1);
  self.pnlpanel2.Controls.Add(self.edUrl);
  self.pnlpanel2.Controls.Add(self.rbGet);
  self.pnlpanel2.Controls.Add(self.label1);
  self.pnlpanel2.Controls.Add(self.rbPost);
  self.pnlpanel2.Controls.Add(self.btnSubmit);
  self.pnlpanel2.Controls.Add(self.cbKeepAlive);
  self.pnlpanel2.Dock := System.Windows.Forms.DockStyle.Top;
  self.pnlpanel2.Location := new System.Drawing.Point(0, 0);
  self.pnlpanel2.Name := 'pnlpanel2';
  self.pnlpanel2.Size := new System.Drawing.Size(669, 71);
  self.pnlpanel2.TabIndex := 12;
  // 
  // pictureBox1
  // 
  self.pictureBox1.Anchor := ((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.pictureBox1.Image := (resources.GetObject('pictureBox1.Image') as System.Drawing.Image);
  self.pictureBox1.Location := new System.Drawing.Point(548, 40);
  self.pictureBox1.Name := 'pictureBox1';
  self.pictureBox1.Size := new System.Drawing.Size(120, 30);
  self.pictureBox1.TabIndex := 10;
  self.pictureBox1.TabStop := false;
  // 
  // edUrl
  // 
  self.edUrl.Anchor := (((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Left) 
        or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.edUrl.Location := new System.Drawing.Point(52, 12);
  self.edUrl.Name := 'edUrl';
  self.edUrl.Size := new System.Drawing.Size(523, 20);
  self.edUrl.TabIndex := 0;
  self.edUrl.Text := 'http://www.remobjects.com';
  // 
  // rbGet
  // 
  self.rbGet.Checked := true;
  self.rbGet.Location := new System.Drawing.Point(52, 36);
  self.rbGet.Name := 'rbGet';
  self.rbGet.Size := new System.Drawing.Size(42, 20);
  self.rbGet.TabIndex := 7;
  self.rbGet.TabStop := true;
  self.rbGet.Text := 'Get';
  // 
  // label1
  // 
  self.label1.Location := new System.Drawing.Point(12, 16);
  self.label1.Name := 'label1';
  self.label1.Size := new System.Drawing.Size(32, 23);
  self.label1.TabIndex := 4;
  self.label1.Text := 'URL:';
  // 
  // rbPost
  // 
  self.rbPost.Location := new System.Drawing.Point(100, 36);
  self.rbPost.Name := 'rbPost';
  self.rbPost.Size := new System.Drawing.Size(48, 20);
  self.rbPost.TabIndex := 8;
  self.rbPost.Text := 'Post';
  // 
  // btnSubmit
  // 
  self.btnSubmit.Anchor := ((System.Windows.Forms.AnchorStyles.Top or System.Windows.Forms.AnchorStyles.Right) as System.Windows.Forms.AnchorStyles);
  self.btnSubmit.Location := new System.Drawing.Point(596, 11);
  self.btnSubmit.Name := 'btnSubmit';
  self.btnSubmit.Size := new System.Drawing.Size(72, 24);
  self.btnSubmit.TabIndex := 2;
  self.btnSubmit.Text := 'Submit';
  self.btnSubmit.Click += new System.EventHandler(@self.btnSubmit_Click);
  // 
  // cbKeepAlive
  // 
  self.cbKeepAlive.Checked := true;
  self.cbKeepAlive.CheckState := System.Windows.Forms.CheckState.Checked;
  self.cbKeepAlive.Location := new System.Drawing.Point(160, 36);
  self.cbKeepAlive.Name := 'cbKeepAlive';
  self.cbKeepAlive.Size := new System.Drawing.Size(104, 20);
  self.cbKeepAlive.TabIndex := 9;
  self.cbKeepAlive.Text := 'Keep Alive';
  // 
  // httpClient1
  // 
  self.httpClient1.ConnectionClass := nil;
  self.httpClient1.ConnectionFactory := nil;
  self.httpClient1.CustomConnectionPool := nil;
  self.httpClient1.HostAddress := nil;
  self.httpClient1.HostName := nil;
  self.httpClient1.Password := '';
  self.httpClient1.Port := 0;
  self.httpClient1.Url := nil;
  self.httpClient1.UserName := '';
  // 
  // dvParams
  // 
  self.dvParams.Table := self.tblParams;
  // 
  // dataGrid2
  // 
  self.dataGrid2.CaptionText := 'Request Content';
  self.dataGrid2.DataMember := '';
  self.dataGrid2.DataSource := self.dvParams;
  self.dataGrid2.Dock := System.Windows.Forms.DockStyle.Bottom;
  self.dataGrid2.HeaderForeColor := System.Drawing.SystemColors.ControlText;
  self.dataGrid2.Location := new System.Drawing.Point(5, 294);
  self.dataGrid2.Name := 'dataGrid2';
  self.dataGrid2.PreferredColumnWidth := 300;
  self.dataGrid2.Size := new System.Drawing.Size(651, 181);
  self.dataGrid2.TabIndex := 1;
  // 
  // splitter2
  // 
  self.splitter2.Dock := System.Windows.Forms.DockStyle.Bottom;
  self.splitter2.Location := new System.Drawing.Point(5, 289);
  self.splitter2.Name := 'splitter2';
  self.splitter2.Size := new System.Drawing.Size(651, 5);
  self.splitter2.TabIndex := 2;
  self.splitter2.TabStop := false;
  // 
  // tabPage2
  // 
  self.tabPage2.Controls.Add(self.splitter2);
  self.tabPage2.Controls.Add(self.dataGrid2);
  self.tabPage2.Controls.Add(self.dataGrid1);
  self.tabPage2.Location := new System.Drawing.Point(4, 22);
  self.tabPage2.Name := 'tabPage2';
  self.tabPage2.Padding := new System.Windows.Forms.Padding(5);
  self.tabPage2.Size := new System.Drawing.Size(661, 480);
  self.tabPage2.TabIndex := 1;
  self.tabPage2.Text := 'Parameters';
  // 
  // dataGrid1
  // 
  self.dataGrid1.CaptionText := 'Request Headers';
  self.dataGrid1.DataMember := '';
  self.dataGrid1.DataSource := self.dvHeaders;
  self.dataGrid1.Dock := System.Windows.Forms.DockStyle.Fill;
  self.dataGrid1.HeaderForeColor := System.Drawing.SystemColors.ControlText;
  self.dataGrid1.Location := new System.Drawing.Point(5, 5);
  self.dataGrid1.Name := 'dataGrid1';
  self.dataGrid1.PreferredColumnWidth := 300;
  self.dataGrid1.Size := new System.Drawing.Size(651, 470);
  self.dataGrid1.TabIndex := 0;
  // 
  // dvHeaders
  // 
  self.dvHeaders.Table := self.tblHeaders;
  // 
  // tabControl1
  // 
  self.tabControl1.Controls.Add(self.tabPage2);
  self.tabControl1.Controls.Add(self.tabPage1);
  self.tabControl1.Dock := System.Windows.Forms.DockStyle.Fill;
  self.tabControl1.Location := new System.Drawing.Point(0, 71);
  self.tabControl1.Name := 'tabControl1';
  self.tabControl1.SelectedIndex := 0;
  self.tabControl1.Size := new System.Drawing.Size(669, 506);
  self.tabControl1.TabIndex := 11;
  // 
  // MainForm
  // 
  self.ClientSize := new System.Drawing.Size(669, 577);
  self.Controls.Add(self.tabControl1);
  self.Controls.Add(self.pnlpanel2);
  self.Icon := (resources.GetObject('$this.Icon') as System.Drawing.Icon);
  self.MinimumSize := new System.Drawing.Size(685, 615);
  self.Name := 'MainForm';
  self.StartPosition := System.Windows.Forms.FormStartPosition.CenterScreen;
  self.Text := 'RemObjects Internet Pack for .NET - HTTP Spy';
  self.Load += new System.EventHandler(@self.MainForm_Load);
  (self.tblResponseHeaders as System.ComponentModel.ISupportInitialize).EndInit();
  self.tabPage1.ResumeLayout(false);
  self.tabPage1.PerformLayout();
  self.pnlpanel1.ResumeLayout(false);
  (self.dataGrid3 as System.ComponentModel.ISupportInitialize).EndInit();
  (self.dataSet1 as System.ComponentModel.ISupportInitialize).EndInit();
  (self.tblHeaders as System.ComponentModel.ISupportInitialize).EndInit();
  (self.tblParams as System.ComponentModel.ISupportInitialize).EndInit();
  self.pnlpanel2.ResumeLayout(false);
  self.pnlpanel2.PerformLayout();
  (self.pictureBox1 as System.ComponentModel.ISupportInitialize).EndInit();
  (self.dvParams as System.ComponentModel.ISupportInitialize).EndInit();
  (self.dataGrid2 as System.ComponentModel.ISupportInitialize).EndInit();
  self.tabPage2.ResumeLayout(false);
  (self.dataGrid1 as System.ComponentModel.ISupportInitialize).EndInit();
  (self.dvHeaders as System.ComponentModel.ISupportInitialize).EndInit();
  self.tabControl1.ResumeLayout(false);
  self.ResumeLayout(false);
end;
{$ENDREGION}

method MainForm.AddHeader(Name: string; Value: string);
begin
  var aRow: DataRow := tblHeaders.NewRow();
  aRow['Name'] := Name;
  aRow['Value'] := Value;
  tblHeaders.Rows.Add(aRow);
end;

method MainForm.AddResponseHeader(Name: string; Value: string);
begin
  var aRow: DataRow := tblResponseHeaders.NewRow();
  aRow['Name'] := Name;
  aRow['Value'] := Value;
  tblResponseHeaders.Rows.Add(aRow);
end;

method MainForm.btnSubmit_Click(sender: System.Object; e: System.EventArgs);
var 
  i: Integer;
  aRow: DataRow;
begin
  var lRequest: HttpClientRequest := new HttpClientRequest();
      if (rbPost.Checked) then begin
        lRequest.RequestType := RequestType.Post;
        
        var lParams: string := '';
        
        for i := 0 to tblParams.Rows.Count - 1 do begin
          aRow := tblParams.Rows[i];
          lParams := lParams + string.Format('{0}={1}#10#13', aRow['Name'].ToString(),aRow['Value'].ToString());
        end;
        lRequest.ContentString := lParams;
      end;

      lRequest.Url.Parse(edUrl.Text);    

      // set headers
      for i := 0 to tblHeaders.Rows.Count - 1 do begin
        aRow := tblHeaders.Rows[i];
        lRequest.Header.SetHeaderValue(aRow['Name'].ToString(),aRow['Value'].ToString());
      end;

      httpClient1.KeepAlive := cbKeepAlive.Checked;
      lRequest.KeepAlive := httpClient1.KeepAlive;
      
      tblResponseHeaders.Clear();
      edResult.Text := '';

      tabControl1.SelectedIndex := 1;
      Application.DoEvents();

      try
      
        var lResponse: HttpClientResponse := httpClient1.Dispatch(lRequest);
        ShowResponse(lResponse);
      except 
        On ex: HttpException do begin
        ShowResponse(ex.Response);
      end;
       On ex: Exception do begin
        _LastResultString := 'Error retrieving response: ' + ex.Message;      
        _LastResultBytes := new UnicodeEncoding().GetBytes(_LastResultString);
        _LastLength := _LastResultBytes.Length;
        SetResultText();
      end;
      end;
end;

method MainForm.ShowResponse(aResponse: HttpClientResponse);
var aRow: DataRow;
begin
  _LastResultString := aResponse.ContentString;
  _LastResultBytes := aResponse.ContentBytes;
  try
    _LastLength := aResponse.ContentLength;
  except
    _LastLength := _LastResultBytes.Length;
  end;

  AddResponseHeader(aResponse.Header.FirstHeader,'');
  for each aHeader: HttpHeader in aResponse.Header do begin
     AddResponseHeader(aHeader.Name,aHeader.Value);
     if aHeader.Name = 'Set-Cookie' then begin
        if (MessageBox.Show('Keep Cookie for future requests?', 'Internet Pack', MessageBoxButtons.YesNo) = DialogResult.Yes) then begin
          aRow := tblHeaders.NewRow();
          aRow['Name'] := 'Cookie';
          aRow['Value'] := aHeader.Value;
          tblHeaders.Rows.Add(aRow);
        end;
      end;
  end;
  SetResultText();    
end;

method MainForm.SetResultText();
var i: Integer;
begin
  Cursor := Cursors.WaitCursor;
  try  
  if _LastLength <> 0 then begin
    if (rbText.Checked) then begin
      edResult.Text := _LastResultString;
      edResult.Font := new Font('Courier New', 8.25);
    end else begin
      var lHex: string := '';
      var lChars: string := '';

      for i := 0 to _LastLength - 1 do begin
        if i mod hexWidth = 0 then begin
          if i > 0 then begin
            lHex := lHex + '| ' + lChars + System.Environment.NewLine;
            lChars := '';
          end;
          lHex := lHex +  i.ToString('X8') + ': ';
        end;
    
        lHex := lHex +  _LastResultBytes[i].ToString('X2') + ' ';
        if _LastResultBytes[i] < 32 then 
          lChars := lChars + '.'
        else 
          lChars := lChars + char(_LastResultBytes[i]);
        end;

        if (_LastLength mod hexWidth > 0) then begin
          for i := _LastLength mod hexWidth to hexWidth - 1 do begin
            lHex := lHex + '   ';
          end;
          lHex := lHex + '| ' + lChars;
        end;
          
        edResult.Text := lHex;
        edResult.Font := new Font('Courier New',8.25);
      end;
    end;
  finally
    Cursor := Cursors.Default;
  end;
end;

method MainForm.rbText_CheckedChanged(sender: System.Object; e: System.EventArgs);
begin
  SetResultText();
end;

method MainForm.MainForm_Load(sender: System.Object; e: System.EventArgs);
begin
  AddHeader('Accept',httpClient1.Accept);
  AddHeader('User-Agent',httpClient1.UserAgent);
end;

end.