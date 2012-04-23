using System;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RemObjects.InternetPack.Http;

namespace HTTPSpy
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class MainForm : System.Windows.Forms.Form
    {
        private System.Windows.Forms.TextBox edUrl;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox edResult;
        private NameValueCollection Headers = new NameValueCollection();
        private NameValueCollection Content = new NameValueCollection();
        private System.Windows.Forms.RadioButton rbText;
        private System.Windows.Forms.RadioButton rbHex;

        private string _LastResultString;
        private byte[] _LastResultBytes;
        private int _LastLength = 0;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Data.DataSet dataSet1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Windows.Forms.DataGrid dataGrid1;
        private System.Data.DataTable tblHeaders;
        private System.Data.DataTable tblParams;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Windows.Forms.DataGrid dataGrid2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rbPost;
        private System.Windows.Forms.RadioButton rbGet;
        private System.Data.DataView dvHeaders;
        private System.Data.DataView dvParams;

        private int hexWidth = 16;
        private System.Windows.Forms.DataGrid dataGrid3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Data.DataTable tblResponseHeaders;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private RemObjects.InternetPack.Http.HttpClient httpClient1;
        private System.Windows.Forms.CheckBox cbKeepAlive;
        private System.Windows.Forms.Panel pnlpanel2;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel pnlpanel1;
        private System.Windows.Forms.Button btnSubmit;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.edUrl = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.dataGrid2 = new System.Windows.Forms.DataGrid();
            this.dvParams = new System.Data.DataView();
            this.tblParams = new System.Data.DataTable();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.dvHeaders = new System.Data.DataView();
            this.tblHeaders = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.edResult = new System.Windows.Forms.TextBox();
            this.pnlpanel1 = new System.Windows.Forms.Panel();
            this.rbHex = new System.Windows.Forms.RadioButton();
            this.rbText = new System.Windows.Forms.RadioButton();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.dataGrid3 = new System.Windows.Forms.DataGrid();
            this.tblResponseHeaders = new System.Data.DataTable();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataSet1 = new System.Data.DataSet();
            this.label1 = new System.Windows.Forms.Label();
            this.rbPost = new System.Windows.Forms.RadioButton();
            this.rbGet = new System.Windows.Forms.RadioButton();
            this.httpClient1 = new RemObjects.InternetPack.Http.HttpClient();
            this.cbKeepAlive = new System.Windows.Forms.CheckBox();
            this.pnlpanel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dvParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dvHeaders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblHeaders)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.pnlpanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblResponseHeaders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            this.pnlpanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // edUrl
            // 
            this.edUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.edUrl.Location = new System.Drawing.Point(52, 12);
            this.edUrl.Name = "edUrl";
            this.edUrl.Size = new System.Drawing.Size(513, 20);
            this.edUrl.TabIndex = 0;
            this.edUrl.Text = "http://www.remobjects.com";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSubmit.Location = new System.Drawing.Point(586, 11);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(72, 24);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(5, 76);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(659, 501);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.splitter2);
            this.tabPage2.Controls.Add(this.dataGrid2);
            this.tabPage2.Controls.Add(this.dataGrid1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(5);
            this.tabPage2.Size = new System.Drawing.Size(651, 475);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Parameters";
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(5, 284);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(641, 5);
            this.splitter2.TabIndex = 2;
            this.splitter2.TabStop = false;
            // 
            // dataGrid2
            // 
            this.dataGrid2.CaptionText = "Request Content";
            this.dataGrid2.DataMember = "";
            this.dataGrid2.DataSource = this.dvParams;
            this.dataGrid2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGrid2.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid2.Location = new System.Drawing.Point(5, 289);
            this.dataGrid2.Name = "dataGrid2";
            this.dataGrid2.PreferredColumnWidth = 300;
            this.dataGrid2.Size = new System.Drawing.Size(641, 181);
            this.dataGrid2.TabIndex = 1;
            // 
            // dvParams
            // 
            this.dvParams.Table = this.tblParams;
            // 
            // tblParams
            // 
            this.tblParams.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn3,
            this.dataColumn4});
            this.tblParams.TableName = "Params";
            // 
            // dataColumn3
            // 
            this.dataColumn3.ColumnName = "Name";
            // 
            // dataColumn4
            // 
            this.dataColumn4.ColumnName = "Value";
            // 
            // dataGrid1
            // 
            this.dataGrid1.CaptionText = "Request Headers";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.DataSource = this.dvHeaders;
            this.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(5, 5);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.PreferredColumnWidth = 300;
            this.dataGrid1.Size = new System.Drawing.Size(641, 465);
            this.dataGrid1.TabIndex = 0;
            // 
            // dvHeaders
            // 
            this.dvHeaders.Table = this.tblHeaders;
            // 
            // tblHeaders
            // 
            this.tblHeaders.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2});
            this.tblHeaders.TableName = "Headers";
            // 
            // dataColumn1
            // 
            this.dataColumn1.ColumnName = "Name";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "Value";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.edResult);
            this.tabPage1.Controls.Add(this.pnlpanel1);
            this.tabPage1.Controls.Add(this.splitter1);
            this.tabPage1.Controls.Add(this.dataGrid3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(5);
            this.tabPage1.Size = new System.Drawing.Size(650, 470);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Result";
            // 
            // edResult
            // 
            this.edResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.edResult.Location = new System.Drawing.Point(5, 213);
            this.edResult.Multiline = true;
            this.edResult.Name = "edResult";
            this.edResult.ReadOnly = true;
            this.edResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.edResult.Size = new System.Drawing.Size(640, 225);
            this.edResult.TabIndex = 4;
            // 
            // pnlpanel1
            // 
            this.pnlpanel1.Controls.Add(this.rbHex);
            this.pnlpanel1.Controls.Add(this.rbText);
            this.pnlpanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlpanel1.Location = new System.Drawing.Point(5, 438);
            this.pnlpanel1.Name = "pnlpanel1";
            this.pnlpanel1.Size = new System.Drawing.Size(640, 27);
            this.pnlpanel1.TabIndex = 10;
            // 
            // rbHex
            // 
            this.rbHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbHex.Location = new System.Drawing.Point(68, 4);
            this.rbHex.Name = "rbHex";
            this.rbHex.Size = new System.Drawing.Size(60, 20);
            this.rbHex.TabIndex = 6;
            this.rbHex.Text = "Hex";
            // 
            // rbText
            // 
            this.rbText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rbText.Checked = true;
            this.rbText.Location = new System.Drawing.Point(8, 4);
            this.rbText.Name = "rbText";
            this.rbText.Size = new System.Drawing.Size(60, 20);
            this.rbText.TabIndex = 5;
            this.rbText.TabStop = true;
            this.rbText.Text = "Text";
            this.rbText.CheckedChanged += new System.EventHandler(this.rbText_CheckedChanged);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(5, 208);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(640, 5);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // dataGrid3
            // 
            this.dataGrid3.CaptionText = "Result Headers";
            this.dataGrid3.DataMember = "";
            this.dataGrid3.DataSource = this.tblResponseHeaders;
            this.dataGrid3.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGrid3.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid3.Location = new System.Drawing.Point(5, 5);
            this.dataGrid3.Name = "dataGrid3";
            this.dataGrid3.PreferredColumnWidth = 300;
            this.dataGrid3.ReadOnly = true;
            this.dataGrid3.Size = new System.Drawing.Size(640, 203);
            this.dataGrid3.TabIndex = 7;
            // 
            // tblResponseHeaders
            // 
            this.tblResponseHeaders.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn5,
            this.dataColumn6});
            this.tblResponseHeaders.TableName = "ResponseHeaders";
            // 
            // dataColumn5
            // 
            this.dataColumn5.ColumnName = "Name";
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "Value";
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Locale = new System.Globalization.CultureInfo("en-US");
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.tblHeaders,
            this.tblParams,
            this.tblResponseHeaders});
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "URL:";
            // 
            // rbPost
            // 
            this.rbPost.Location = new System.Drawing.Point(100, 36);
            this.rbPost.Name = "rbPost";
            this.rbPost.Size = new System.Drawing.Size(48, 20);
            this.rbPost.TabIndex = 8;
            this.rbPost.Text = "Post";
            // 
            // rbGet
            // 
            this.rbGet.Checked = true;
            this.rbGet.Location = new System.Drawing.Point(52, 36);
            this.rbGet.Name = "rbGet";
            this.rbGet.Size = new System.Drawing.Size(40, 20);
            this.rbGet.TabIndex = 7;
            this.rbGet.TabStop = true;
            this.rbGet.Text = "Get";
            // 
            // httpClient1
            // 
            this.httpClient1.ConnectionClass = null;
            this.httpClient1.ConnectionFactory = null;
            this.httpClient1.CustomConnectionPool = null;
            this.httpClient1.HostAddress = null;
            this.httpClient1.HostName = null;
            this.httpClient1.Password = "";
            this.httpClient1.Port = 0;
            this.httpClient1.Url = null;
            this.httpClient1.UserName = "";
            // 
            // cbKeepAlive
            // 
            this.cbKeepAlive.Checked = true;
            this.cbKeepAlive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbKeepAlive.Location = new System.Drawing.Point(160, 36);
            this.cbKeepAlive.Name = "cbKeepAlive";
            this.cbKeepAlive.Size = new System.Drawing.Size(104, 20);
            this.cbKeepAlive.TabIndex = 9;
            this.cbKeepAlive.Text = "Keep Alive";
            // 
            // pnlpanel2
            // 
            this.pnlpanel2.Controls.Add(this.pictureBox1);
            this.pnlpanel2.Controls.Add(this.edUrl);
            this.pnlpanel2.Controls.Add(this.rbGet);
            this.pnlpanel2.Controls.Add(this.label1);
            this.pnlpanel2.Controls.Add(this.rbPost);
            this.pnlpanel2.Controls.Add(this.btnSubmit);
            this.pnlpanel2.Controls.Add(this.cbKeepAlive);
            this.pnlpanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlpanel2.Location = new System.Drawing.Point(5, 5);
            this.pnlpanel2.Name = "pnlpanel2";
            this.pnlpanel2.Size = new System.Drawing.Size(659, 71);
            this.pnlpanel2.TabIndex = 10;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(538, 40);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 30);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(669, 582);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pnlpanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(685, 620);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemObjects Internet Pack for .NET - HTTP Spy";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dvParams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblParams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dvHeaders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblHeaders)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.pnlpanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tblResponseHeaders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            this.pnlpanel2.ResumeLayout(false);
            this.pnlpanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.Run(new MainForm());
        }

        private void AddHeader(string Name, string Value)
        {
            DataRow aRow = tblHeaders.NewRow();
            aRow["Name"] = Name;
            aRow["Value"] = Value;
            tblHeaders.Rows.Add(aRow);
        }

        private void AddResponseHeader(string Name, string Value)
        {
            DataRow aRow = tblResponseHeaders.NewRow();
            aRow["Name"] = Name;
            aRow["Value"] = Value;
            tblResponseHeaders.Rows.Add(aRow);
        }

        private void btnSubmit_Click(object sender, System.EventArgs e)
        {
            HttpClientRequest lRequest = new HttpClientRequest();
            if (rbPost.Checked) // don't set .Get, it's the default
            {
                lRequest.RequestType = RequestType.Post;

                string lParams = "";
                for (int i = 0; i < tblParams.Rows.Count; i++)
                {
                    DataRow aRow = tblParams.Rows[i];

                    lParams += string.Format("{0}={1}\r\n", aRow["Name"].ToString(), aRow["Value"].ToString());
                }
                lRequest.ContentString = lParams;
            }

            lRequest.Url.Parse(edUrl.Text);

            // set headers
            for (int i = 0; i < tblHeaders.Rows.Count; i++)
            {
                DataRow aRow = tblHeaders.Rows[i];
                lRequest.Header.SetHeaderValue(aRow["Name"].ToString(), aRow["Value"].ToString());
            }

            httpClient1.KeepAlive = cbKeepAlive.Checked;
            lRequest.KeepAlive = httpClient1.KeepAlive;

            tblResponseHeaders.Clear();
            edResult.Text = "";

            tabControl1.SelectedIndex = 1;
            Application.DoEvents();

            try
            {
                HttpClientResponse lResponse = httpClient1.Dispatch(lRequest);
                ShowResponse(lResponse);
            }
            catch (HttpException ex)
            {
                ShowResponse(ex.Response);
            }
            catch (Exception ex)
            {
                _LastResultString = "Error retrieving response: " + ex.Message;
                _LastResultBytes = new UnicodeEncoding().GetBytes(_LastResultString);
                _LastLength = _LastResultBytes.Length;
                SetResultText();
            }
        }

        public void ShowResponse(HttpClientResponse aResponse)
        {
            _LastResultString = aResponse.ContentString;
            _LastResultBytes = aResponse.ContentBytes;
            try
            {
                _LastLength = aResponse.ContentLength;
            }
            catch
            {
                _LastLength = _LastResultBytes.Length;
            }

            AddResponseHeader(aResponse.Header.FirstHeader, "");
            foreach (HttpHeader aHeader in aResponse.Header)
            {
                AddResponseHeader(aHeader.Name, aHeader.Value);
                if (aHeader.Name == "Set-Cookie")
                {
                    if (MessageBox.Show("Keep Cookie for future requests?", "Internet Pack", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        DataRow aRow = tblHeaders.NewRow();
                        aRow["Name"] = "Cookie";
                        aRow["Value"] = aHeader.Value;
                        tblHeaders.Rows.Add(aRow);
                    }
                }
            }

            SetResultText();
        }

        private void SetResultText()
        {
            if (_LastLength != 0)
            {
                if (rbText.Checked)
                {
                    edResult.Text = _LastResultString;
                    edResult.Font = new Font("Courier New", (float)8.25);
                }
                else
                {
                    string lHex = "";
                    string lChars = "";

                    for (int i = 0; i < _LastLength; i++)
                    {
                        if (i % hexWidth == 0)
                        {
                            if (i > 0)
                            {
                                lHex += "| " + lChars + "\r\n";
                                lChars = "";
                            }
                            lHex += i.ToString("X8") + ": ";
                        }

                        lHex += _LastResultBytes[i].ToString("X2") + ' ';
                        if (_LastResultBytes[i] < 32)
                            lChars += ".";
                        else
                            lChars += (char)(_LastResultBytes[i]);
                    }

                    if (_LastLength % hexWidth > 0)
                    {
                        for (int i = _LastLength % hexWidth; i < hexWidth; i++)
                        {
                            lHex += "   ";
                        }
                        lHex += "| " + lChars;
                    }

                    edResult.Text = lHex;
                    edResult.Font = new Font("Courier New", (float)8.25);
                }
            }
        }

        private void rbText_CheckedChanged(object sender, System.EventArgs e)
        {
            SetResultText();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            AddHeader("Accept", httpClient1.Accept);
            AddHeader("User-Agent", httpClient1.UserAgent);
        }
    }
}