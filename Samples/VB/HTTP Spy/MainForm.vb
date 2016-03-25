Imports RemObjects.InternetPack.Http
Imports System.Text

Public Class MainForm
    Inherits System.Windows.Forms.Form

    Private WithEvents tabControl1 As System.Windows.Forms.TabControl
    Private WithEvents dvHeaders As System.Data.DataView
    Private WithEvents dataGrid1 As System.Windows.Forms.DataGrid
    Private WithEvents tabPage2 As System.Windows.Forms.TabPage
    Private WithEvents splitter2 As System.Windows.Forms.Splitter
    Private WithEvents dataGrid2 As System.Windows.Forms.DataGrid
    Private WithEvents dvParams As System.Data.DataView
    Private WithEvents httpClient1 As RemObjects.InternetPack.Http.HttpClient
    Private WithEvents cbKeepAlive As System.Windows.Forms.CheckBox
    Private WithEvents btnSubmit As System.Windows.Forms.Button
    Private WithEvents rbPost As System.Windows.Forms.RadioButton
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents rbGet As System.Windows.Forms.RadioButton
    Private WithEvents edUrl As System.Windows.Forms.TextBox
    Private WithEvents pictureBox1 As System.Windows.Forms.PictureBox
    Private WithEvents pnlpanel2 As System.Windows.Forms.Panel
    Private WithEvents dataColumn4 As System.Data.DataColumn
    Private WithEvents dataColumn3 As System.Data.DataColumn
    Private WithEvents tblParams As System.Data.DataTable
    Private WithEvents dataColumn2 As System.Data.DataColumn
    Private WithEvents dataColumn1 As System.Data.DataColumn
    Private WithEvents tblHeaders As System.Data.DataTable
    Private WithEvents dataSet1 As System.Data.DataSet
    Private WithEvents dataGrid3 As System.Windows.Forms.DataGrid
    Private WithEvents splitter1 As System.Windows.Forms.Splitter
    Private WithEvents rbHex As System.Windows.Forms.RadioButton
    Private WithEvents pnlpanel1 As System.Windows.Forms.Panel
    Private WithEvents edResult As System.Windows.Forms.TextBox
    Private WithEvents tabPage1 As System.Windows.Forms.TabPage
    Private WithEvents rbText As System.Windows.Forms.RadioButton
    Private WithEvents dataColumn6 As System.Data.DataColumn
    Private WithEvents dataColumn5 As System.Data.DataColumn
    Private WithEvents tblResponseHeaders As System.Data.DataTable
    Private components As System.ComponentModel.IContainer

    Private _LastResultString As String
    Private _LastResultBytes As Byte()
    Private _LastLength As Integer = 0
    Private hexWidth As Integer = 16

    Public Sub New()
        MyBase.New()

        Application.EnableVisualStyles()
        InitializeComponent()
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.tblResponseHeaders = New System.Data.DataTable
        Me.dataColumn5 = New System.Data.DataColumn
        Me.dataColumn6 = New System.Data.DataColumn
        Me.rbText = New System.Windows.Forms.RadioButton
        Me.tabPage1 = New System.Windows.Forms.TabPage
        Me.edResult = New System.Windows.Forms.TextBox
        Me.pnlpanel1 = New System.Windows.Forms.Panel
        Me.rbHex = New System.Windows.Forms.RadioButton
        Me.splitter1 = New System.Windows.Forms.Splitter
        Me.dataGrid3 = New System.Windows.Forms.DataGrid
        Me.dataSet1 = New System.Data.DataSet
        Me.tblHeaders = New System.Data.DataTable
        Me.dataColumn1 = New System.Data.DataColumn
        Me.dataColumn2 = New System.Data.DataColumn
        Me.tblParams = New System.Data.DataTable
        Me.dataColumn3 = New System.Data.DataColumn
        Me.dataColumn4 = New System.Data.DataColumn
        Me.pnlpanel2 = New System.Windows.Forms.Panel
        Me.pictureBox1 = New System.Windows.Forms.PictureBox
        Me.edUrl = New System.Windows.Forms.TextBox
        Me.rbGet = New System.Windows.Forms.RadioButton
        Me.label1 = New System.Windows.Forms.Label
        Me.rbPost = New System.Windows.Forms.RadioButton
        Me.btnSubmit = New System.Windows.Forms.Button
        Me.cbKeepAlive = New System.Windows.Forms.CheckBox
        Me.httpClient1 = New RemObjects.InternetPack.Http.HttpClient
        Me.dvParams = New System.Data.DataView
        Me.dataGrid2 = New System.Windows.Forms.DataGrid
        Me.splitter2 = New System.Windows.Forms.Splitter
        Me.tabPage2 = New System.Windows.Forms.TabPage
        Me.dataGrid1 = New System.Windows.Forms.DataGrid
        Me.dvHeaders = New System.Data.DataView
        Me.tabControl1 = New System.Windows.Forms.TabControl
        CType(Me.tblResponseHeaders, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPage1.SuspendLayout()
        Me.pnlpanel1.SuspendLayout()
        CType(Me.dataGrid3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dataSet1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tblHeaders, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tblParams, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlpanel2.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dvParams, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dataGrid2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabPage2.SuspendLayout()
        CType(Me.dataGrid1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dvHeaders, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabControl1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tblResponseHeaders
        '
        Me.tblResponseHeaders.Columns.AddRange(New System.Data.DataColumn() {Me.dataColumn5, Me.dataColumn6})
        Me.tblResponseHeaders.TableName = "ResponseHeaders"
        '
        'dataColumn5
        '
        Me.dataColumn5.ColumnName = "Name"
        '
        'dataColumn6
        '
        Me.dataColumn6.ColumnName = "Value"
        '
        'rbText
        '
        Me.rbText.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.rbText.Checked = True
        Me.rbText.Location = New System.Drawing.Point(8, 4)
        Me.rbText.Name = "rbText"
        Me.rbText.Size = New System.Drawing.Size(60, 20)
        Me.rbText.TabIndex = 5
        Me.rbText.TabStop = True
        Me.rbText.Text = "Text"
        '
        'tabPage1
        '
        Me.tabPage1.Controls.Add(Me.edResult)
        Me.tabPage1.Controls.Add(Me.pnlpanel1)
        Me.tabPage1.Controls.Add(Me.splitter1)
        Me.tabPage1.Controls.Add(Me.dataGrid3)
        Me.tabPage1.Location = New System.Drawing.Point(4, 22)
        Me.tabPage1.Name = "tabPage1"
        Me.tabPage1.Padding = New System.Windows.Forms.Padding(5)
        Me.tabPage1.Size = New System.Drawing.Size(660, 480)
        Me.tabPage1.TabIndex = 0
        Me.tabPage1.Text = "Result"
        '
        'edResult
        '
        Me.edResult.Dock = System.Windows.Forms.DockStyle.Fill
        Me.edResult.Location = New System.Drawing.Point(5, 213)
        Me.edResult.Multiline = True
        Me.edResult.Name = "edResult"
        Me.edResult.ReadOnly = True
        Me.edResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.edResult.Size = New System.Drawing.Size(650, 235)
        Me.edResult.TabIndex = 4
        '
        'pnlpanel1
        '
        Me.pnlpanel1.Controls.Add(Me.rbHex)
        Me.pnlpanel1.Controls.Add(Me.rbText)
        Me.pnlpanel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlpanel1.Location = New System.Drawing.Point(5, 448)
        Me.pnlpanel1.Name = "pnlpanel1"
        Me.pnlpanel1.Size = New System.Drawing.Size(650, 27)
        Me.pnlpanel1.TabIndex = 10
        '
        'rbHex
        '
        Me.rbHex.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.rbHex.Location = New System.Drawing.Point(68, 4)
        Me.rbHex.Name = "rbHex"
        Me.rbHex.Size = New System.Drawing.Size(60, 20)
        Me.rbHex.TabIndex = 6
        Me.rbHex.Text = "Hex"
        '
        'splitter1
        '
        Me.splitter1.Dock = System.Windows.Forms.DockStyle.Top
        Me.splitter1.Location = New System.Drawing.Point(5, 208)
        Me.splitter1.Name = "splitter1"
        Me.splitter1.Size = New System.Drawing.Size(650, 5)
        Me.splitter1.TabIndex = 8
        Me.splitter1.TabStop = False
        '
        'dataGrid3
        '
        Me.dataGrid3.CaptionText = "Result Headers"
        Me.dataGrid3.DataMember = ""
        Me.dataGrid3.DataSource = Me.tblResponseHeaders
        Me.dataGrid3.Dock = System.Windows.Forms.DockStyle.Top
        Me.dataGrid3.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.dataGrid3.Location = New System.Drawing.Point(5, 5)
        Me.dataGrid3.Name = "dataGrid3"
        Me.dataGrid3.PreferredColumnWidth = 300
        Me.dataGrid3.ReadOnly = True
        Me.dataGrid3.Size = New System.Drawing.Size(650, 203)
        Me.dataGrid3.TabIndex = 7
        '
        'dataSet1
        '
        Me.dataSet1.DataSetName = "NewDataSet"
        Me.dataSet1.Locale = New System.Globalization.CultureInfo("en-US")
        Me.dataSet1.Tables.AddRange(New System.Data.DataTable() {Me.tblHeaders, Me.tblParams, Me.tblResponseHeaders})
        '
        'tblHeaders
        '
        Me.tblHeaders.Columns.AddRange(New System.Data.DataColumn() {Me.dataColumn1, Me.dataColumn2})
        Me.tblHeaders.TableName = "Headers"
        '
        'dataColumn1
        '
        Me.dataColumn1.ColumnName = "Name"
        '
        'dataColumn2
        '
        Me.dataColumn2.ColumnName = "Value"
        '
        'tblParams
        '
        Me.tblParams.Columns.AddRange(New System.Data.DataColumn() {Me.dataColumn3, Me.dataColumn4})
        Me.tblParams.TableName = "Params"
        '
        'dataColumn3
        '
        Me.dataColumn3.ColumnName = "Name"
        '
        'dataColumn4
        '
        Me.dataColumn4.ColumnName = "Value"
        '
        'pnlpanel2
        '
        Me.pnlpanel2.Controls.Add(Me.pictureBox1)
        Me.pnlpanel2.Controls.Add(Me.edUrl)
        Me.pnlpanel2.Controls.Add(Me.rbGet)
        Me.pnlpanel2.Controls.Add(Me.label1)
        Me.pnlpanel2.Controls.Add(Me.rbPost)
        Me.pnlpanel2.Controls.Add(Me.btnSubmit)
        Me.pnlpanel2.Controls.Add(Me.cbKeepAlive)
        Me.pnlpanel2.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlpanel2.Location = New System.Drawing.Point(0, 0)
        Me.pnlpanel2.Name = "pnlpanel2"
        Me.pnlpanel2.Size = New System.Drawing.Size(669, 71)
        Me.pnlpanel2.TabIndex = 12
        '
        'pictureBox1
        '
        Me.pictureBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pictureBox1.Image = CType(resources.GetObject("pictureBox1.Image"), System.Drawing.Image)
        Me.pictureBox1.Location = New System.Drawing.Point(548, 40)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(120, 30)
        Me.pictureBox1.TabIndex = 10
        Me.pictureBox1.TabStop = False
        '
        'edUrl
        '
        Me.edUrl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.edUrl.Location = New System.Drawing.Point(52, 12)
        Me.edUrl.Name = "edUrl"
        Me.edUrl.Size = New System.Drawing.Size(523, 20)
        Me.edUrl.TabIndex = 0
        Me.edUrl.Text = "http://www.remobjects.com"
        '
        'rbGet
        '
        Me.rbGet.Checked = True
        Me.rbGet.Location = New System.Drawing.Point(52, 36)
        Me.rbGet.Name = "rbGet"
        Me.rbGet.Size = New System.Drawing.Size(54, 20)
        Me.rbGet.TabIndex = 7
        Me.rbGet.TabStop = True
        Me.rbGet.Text = "Get"
        '
        'label1
        '
        Me.label1.Location = New System.Drawing.Point(12, 16)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(32, 23)
        Me.label1.TabIndex = 4
        Me.label1.Text = "URL:"
        '
        'rbPost
        '
        Me.rbPost.Location = New System.Drawing.Point(112, 36)
        Me.rbPost.Name = "rbPost"
        Me.rbPost.Size = New System.Drawing.Size(48, 20)
        Me.rbPost.TabIndex = 8
        Me.rbPost.Text = "Post"
        '
        'btnSubmit
        '
        Me.btnSubmit.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSubmit.Location = New System.Drawing.Point(596, 11)
        Me.btnSubmit.Name = "btnSubmit"
        Me.btnSubmit.Size = New System.Drawing.Size(72, 24)
        Me.btnSubmit.TabIndex = 2
        Me.btnSubmit.Text = "Submit"
        '
        'cbKeepAlive
        '
        Me.cbKeepAlive.Checked = True
        Me.cbKeepAlive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbKeepAlive.Location = New System.Drawing.Point(172, 36)
        Me.cbKeepAlive.Name = "cbKeepAlive"
        Me.cbKeepAlive.Size = New System.Drawing.Size(104, 20)
        Me.cbKeepAlive.TabIndex = 9
        Me.cbKeepAlive.Text = "Keep Alive"
        '
        'httpClient1
        '
        Me.httpClient1.ConnectionClass = Nothing
        Me.httpClient1.ConnectionFactory = Nothing
        Me.httpClient1.CustomConnectionPool = Nothing
        Me.httpClient1.HostAddress = Nothing
        Me.httpClient1.HostName = Nothing
        Me.httpClient1.Password = ""
        Me.httpClient1.Port = 0
        Me.httpClient1.UserName = ""
        '
        'dvParams
        '
        Me.dvParams.Table = Me.tblParams
        '
        'dataGrid2
        '
        Me.dataGrid2.CaptionText = "Request Content"
        Me.dataGrid2.DataMember = ""
        Me.dataGrid2.DataSource = Me.dvParams
        Me.dataGrid2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.dataGrid2.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.dataGrid2.Location = New System.Drawing.Point(5, 294)
        Me.dataGrid2.Name = "dataGrid2"
        Me.dataGrid2.PreferredColumnWidth = 300
        Me.dataGrid2.Size = New System.Drawing.Size(651, 181)
        Me.dataGrid2.TabIndex = 1
        '
        'splitter2
        '
        Me.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.splitter2.Location = New System.Drawing.Point(5, 289)
        Me.splitter2.Name = "splitter2"
        Me.splitter2.Size = New System.Drawing.Size(651, 5)
        Me.splitter2.TabIndex = 2
        Me.splitter2.TabStop = False
        '
        'tabPage2
        '
        Me.tabPage2.Controls.Add(Me.splitter2)
        Me.tabPage2.Controls.Add(Me.dataGrid2)
        Me.tabPage2.Controls.Add(Me.dataGrid1)
        Me.tabPage2.Location = New System.Drawing.Point(4, 22)
        Me.tabPage2.Name = "tabPage2"
        Me.tabPage2.Padding = New System.Windows.Forms.Padding(5)
        Me.tabPage2.Size = New System.Drawing.Size(661, 480)
        Me.tabPage2.TabIndex = 1
        Me.tabPage2.Text = "Parameters"
        '
        'dataGrid1
        '
        Me.dataGrid1.CaptionText = "Request Headers"
        Me.dataGrid1.DataMember = ""
        Me.dataGrid1.DataSource = Me.dvHeaders
        Me.dataGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText
        Me.dataGrid1.Location = New System.Drawing.Point(5, 5)
        Me.dataGrid1.Name = "dataGrid1"
        Me.dataGrid1.PreferredColumnWidth = 300
        Me.dataGrid1.Size = New System.Drawing.Size(651, 470)
        Me.dataGrid1.TabIndex = 0
        '
        'dvHeaders
        '
        Me.dvHeaders.Table = Me.tblHeaders
        '
        'tabControl1
        '
        Me.tabControl1.Controls.Add(Me.tabPage2)
        Me.tabControl1.Controls.Add(Me.tabPage1)
        Me.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabControl1.Location = New System.Drawing.Point(0, 71)
        Me.tabControl1.Name = "tabControl1"
        Me.tabControl1.SelectedIndex = 0
        Me.tabControl1.Size = New System.Drawing.Size(669, 506)
        Me.tabControl1.TabIndex = 11
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(669, 577)
        Me.Controls.Add(Me.tabControl1)
        Me.Controls.Add(Me.pnlpanel2)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(685, 615)
        Me.Name = "MainForm"
        Me.Text = "RemObjects Internet Pack for .NET - HTTP Spy"
        CType(Me.tblResponseHeaders, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPage1.ResumeLayout(False)
        Me.tabPage1.PerformLayout()
        Me.pnlpanel1.ResumeLayout(False)
        CType(Me.dataGrid3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dataSet1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tblHeaders, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tblParams, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlpanel2.ResumeLayout(False)
        Me.pnlpanel2.PerformLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dvParams, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dataGrid2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabPage2.ResumeLayout(False)
        CType(Me.dataGrid1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dvHeaders, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabControl1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Private Sub AddHeader(ByVal Name As String, ByVal Value As String)
        Dim aRow As DataRow
        aRow = tblHeaders.NewRow()
        aRow("Name") = Name
        aRow("Value") = Value
        tblHeaders.Rows.Add(aRow)
    End Sub

    Private Sub AddResponseHeader(ByVal Name As String, ByVal Value As String)
        Dim aRow As DataRow
        aRow = tblResponseHeaders.NewRow()

        aRow("Name") = Name
        aRow("Value") = Value
        tblResponseHeaders.Rows.Add(aRow)
    End Sub

    Private Sub btnSubmit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        Dim i As Integer
        Dim lRequest As HttpClientRequest
        lRequest = New HttpClientRequest()
        If (rbPost.Checked) Then
            lRequest.RequestType = RequestType.Post

            Dim lParams As String
            lParams = ""

            For i = 0 To tblParams.Rows.Count - 1
                Dim aRow As DataRow
                aRow = tblParams.Rows(i)
                lParams = lParams & String.Format("{0}={1}\r\n", aRow("Name").ToString(), aRow("Value").ToString())
            Next
            lRequest.ContentString = lParams
        End If


        lRequest.Url.Parse(edUrl.Text)

        ' set headers
        For i = 0 To tblHeaders.Rows.Count - 1
            Dim aRow As DataRow
            aRow = tblHeaders.Rows(i)
            lRequest.Header.SetHeaderValue(aRow("Name").ToString(), aRow("Value").ToString())
        Next

        httpClient1.KeepAlive = cbKeepAlive.Checked
        lRequest.KeepAlive = httpClient1.KeepAlive

        tblResponseHeaders.Clear()
        edResult.Text = ""

        tabControl1.SelectedIndex = 1
        Application.DoEvents()

        Try
            Dim lResponse As HttpClientResponse
            lResponse = httpClient1.Dispatch(lRequest)
            ShowResponse(lResponse)

        Catch ex As HttpException

            ShowResponse(ex.Response)


        Catch ex1 As Exception

            _LastResultString = "Error retrieving response: " + ex1.Message
            _LastResultBytes = New UnicodeEncoding().GetBytes(_LastResultString)
            _LastLength = _LastResultBytes.Length
            SetResultText()

        End Try
    End Sub

    Public Sub ShowResponse(ByVal aResponse As HttpClientResponse)

        _LastResultString = aResponse.ContentString
        _LastResultBytes = aResponse.ContentBytes
        Try
            _LastLength = aResponse.ContentLength
        Catch
            _LastLength = _LastResultBytes.Length
        End Try

        AddResponseHeader(aResponse.Header.FirstHeader, "")
        For Each aHeader As HttpHeader In aResponse.Header
            AddResponseHeader(aHeader.Name, aHeader.Value)
            If aHeader.Name = "Set-Cookie" Then
                If MessageBox.Show("Keep Cookie for future requests?", "Internet Pack", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then
                    Dim aRow As DataRow
                    aRow = tblHeaders.NewRow()
                    aRow("Name") = "Cookie"
                    aRow("Value") = aHeader.Value
                    tblHeaders.Rows.Add(aRow)
                End If
            End If
        Next
        SetResultText()
    End Sub

    Private Sub SetResultText()
        Cursor = Cursors.WaitCursor
        Try


            If _LastLength <> 0 Then
                If rbText.Checked Then
                    edResult.Text = _LastResultString
                    edResult.Font = New Font("Courier New", 8.25F)
                Else
                    Dim lHex As String = ""
                    Dim lChars As String = ""
                    Dim i As Integer
                    For i = 0 To _LastLength - 1
                        If i Mod hexWidth = 0 Then
                            If i > 0 Then
                                lHex += "| " + lChars + System.Environment.NewLine
                                lChars = ""
                            End If
                            lHex += i.ToString("X8") + ": "
                        End If

                        lHex = lHex & _LastResultBytes(i).ToString("X2") & " "
                        If _LastResultBytes(i) < 32 Then
                            lChars += "."
                        Else
                            lChars = lChars + Microsoft.VisualBasic.ChrW(_LastResultBytes(i))
                        End If
                    Next

                    If (_LastLength Mod hexWidth > 0) Then
                        For i = _LastLength Mod hexWidth To hexWidth - 1
                            lHex = lHex & "   "
                        Next
                        lHex += "| " + lChars
                    End If

                    edResult.Text = lHex
                    edResult.Font = New Font("Courier New", 8.25F)
                End If
            End If
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub rbText_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbText.CheckedChanged
        SetResultText()
    End Sub

    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        AddHeader("Accept", httpClient1.Accept)
        AddHeader("User-Agent", httpClient1.UserAgent)
    End Sub
End Class
