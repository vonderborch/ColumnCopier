namespace ColumnCopier
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.components = new System.ComponentModel.Container();
            this.paste_btn = new System.Windows.Forms.Button();
            this.pasteCopy_btn = new System.Windows.Forms.Button();
            this.copy_btn = new System.Windows.Forms.Button();
            this.copyLine_btn = new System.Windows.Forms.Button();
            this.line_txt = new System.Windows.Forms.TextBox();
            this.column_txt = new System.Windows.Forms.TextBox();
            this.column_cmb = new System.Windows.Forms.ComboBox();
            this.history_cmb = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.history_txt = new System.Windows.Forms.TextBox();
            this.clearHistory_btn = new System.Windows.Forms.Button();
            this.replaceText_txt = new System.Windows.Forms.TextBox();
            this.copyReplace_btn = new System.Windows.Forms.Button();
            this.replaceComma_btn = new System.Windows.Forms.Button();
            this.columnNum_txt = new System.Windows.Forms.Label();
            this.rowNum_txt = new System.Windows.Forms.Label();
            this.currentColumn_txt = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.defaultColumn_txt = new System.Windows.Forms.TextBox();
            this.defaultColumnText_txt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.threshold_txt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.text_rbtn = new System.Windows.Forms.RadioButton();
            this.number_rbtn = new System.Windows.Forms.RadioButton();
            this.isTop_cbx = new System.Windows.Forms.CheckBox();
            this.saveSettings_btn = new System.Windows.Forms.Button();
            this.loadSettings_btn = new System.Windows.Forms.Button();
            this.saveAsNew_btn = new System.Windows.Forms.Button();
            this.header_cxb = new System.Windows.Forms.CheckBox();
            this.removeBlanks_cxb = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.replaceTextPost_txt = new System.Windows.Forms.TextBox();
            this.replaceTextPre_txt = new System.Windows.Forms.TextBox();
            this.replaceQuotedComma_btn = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status_txt = new System.Windows.Forms.ToolStripStatusLabel();
            this.help_btn = new System.Windows.Forms.Button();
            this.replaceSemiColon_btn = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.export_btn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // paste_btn
            // 
            this.paste_btn.AutoSize = true;
            this.paste_btn.Location = new System.Drawing.Point(21, 17);
            this.paste_btn.Name = "paste_btn";
            this.paste_btn.Size = new System.Drawing.Size(392, 58);
            this.paste_btn.TabIndex = 0;
            this.paste_btn.Text = "Paste";
            this.toolTip.SetToolTip(this.paste_btn, "Pastes data from the clipboard, creating a new request.");
            this.paste_btn.UseVisualStyleBackColor = true;
            this.paste_btn.Click += new System.EventHandler(this.paste_btn_Click);
            // 
            // pasteCopy_btn
            // 
            this.pasteCopy_btn.AutoSize = true;
            this.pasteCopy_btn.Location = new System.Drawing.Point(21, 81);
            this.pasteCopy_btn.Name = "pasteCopy_btn";
            this.pasteCopy_btn.Size = new System.Drawing.Size(392, 58);
            this.pasteCopy_btn.TabIndex = 1;
            this.pasteCopy_btn.Text = "Paste and Copy";
            this.toolTip.SetToolTip(this.pasteCopy_btn, "Pastes data from the clipboard, creating a new request, and then copies the defau" +
        "lt selected column into the clipboard.\r\n");
            this.pasteCopy_btn.UseVisualStyleBackColor = true;
            this.pasteCopy_btn.Click += new System.EventHandler(this.pasteCopy_btn_Click);
            // 
            // copy_btn
            // 
            this.copy_btn.AutoSize = true;
            this.copy_btn.Location = new System.Drawing.Point(21, 145);
            this.copy_btn.Name = "copy_btn";
            this.copy_btn.Size = new System.Drawing.Size(394, 58);
            this.copy_btn.TabIndex = 2;
            this.copy_btn.Text = "Copy Column";
            this.toolTip.SetToolTip(this.copy_btn, "Copies the currently selected column into the clipboard.");
            this.copy_btn.UseVisualStyleBackColor = true;
            this.copy_btn.Click += new System.EventHandler(this.copy_btn_Click);
            // 
            // copyLine_btn
            // 
            this.copyLine_btn.AutoSize = true;
            this.copyLine_btn.Location = new System.Drawing.Point(21, 209);
            this.copyLine_btn.Name = "copyLine_btn";
            this.copyLine_btn.Size = new System.Drawing.Size(304, 58);
            this.copyLine_btn.TabIndex = 3;
            this.copyLine_btn.Text = "Copy Line";
            this.toolTip.SetToolTip(this.copyLine_btn, "Copies the line in the column selected by the line number text.");
            this.copyLine_btn.UseVisualStyleBackColor = true;
            this.copyLine_btn.Click += new System.EventHandler(this.copyLine_btn_Click);
            // 
            // line_txt
            // 
            this.line_txt.Location = new System.Drawing.Point(331, 220);
            this.line_txt.Name = "line_txt";
            this.line_txt.Size = new System.Drawing.Size(82, 38);
            this.line_txt.TabIndex = 4;
            this.line_txt.Text = "0";
            this.toolTip.SetToolTip(this.line_txt, "The line to copy.");
            this.line_txt.TextChanged += new System.EventHandler(this.line_txt_TextChanged);
            // 
            // column_txt
            // 
            this.column_txt.Location = new System.Drawing.Point(421, 62);
            this.column_txt.Multiline = true;
            this.column_txt.Name = "column_txt";
            this.column_txt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.column_txt.Size = new System.Drawing.Size(630, 571);
            this.column_txt.TabIndex = 5;
            this.toolTip.SetToolTip(this.column_txt, "The currently selected column.");
            // 
            // column_cmb
            // 
            this.column_cmb.FormattingEnabled = true;
            this.column_cmb.Location = new System.Drawing.Point(421, 17);
            this.column_cmb.Name = "column_cmb";
            this.column_cmb.Size = new System.Drawing.Size(630, 39);
            this.column_cmb.TabIndex = 6;
            this.toolTip.SetToolTip(this.column_cmb, "A list of all columns in the current request.");
            this.column_cmb.SelectedIndexChanged += new System.EventHandler(this.column_cmb_SelectedIndexChanged);
            // 
            // history_cmb
            // 
            this.history_cmb.FormattingEnabled = true;
            this.history_cmb.Location = new System.Drawing.Point(22, 542);
            this.history_cmb.Name = "history_cmb";
            this.history_cmb.Size = new System.Drawing.Size(379, 39);
            this.history_cmb.TabIndex = 7;
            this.toolTip.SetToolTip(this.history_cmb, "A list of previous requests.");
            this.history_cmb.SelectedIndexChanged += new System.EventHandler(this.history_cmb_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 598);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 32);
            this.label1.TabIndex = 8;
            this.label1.Text = "Max History";
            // 
            // history_txt
            // 
            this.history_txt.Location = new System.Drawing.Point(193, 595);
            this.history_txt.Name = "history_txt";
            this.history_txt.Size = new System.Drawing.Size(208, 38);
            this.history_txt.TabIndex = 9;
            this.history_txt.Text = "10";
            this.toolTip.SetToolTip(this.history_txt, "The maximum number of previously stored requests that are saved.");
            // 
            // clearHistory_btn
            // 
            this.clearHistory_btn.AutoSize = true;
            this.clearHistory_btn.Location = new System.Drawing.Point(21, 644);
            this.clearHistory_btn.Name = "clearHistory_btn";
            this.clearHistory_btn.Size = new System.Drawing.Size(392, 58);
            this.clearHistory_btn.TabIndex = 10;
            this.clearHistory_btn.Text = "Clear History";
            this.toolTip.SetToolTip(this.clearHistory_btn, "Clears all previously saved requests.");
            this.clearHistory_btn.UseVisualStyleBackColor = true;
            this.clearHistory_btn.Click += new System.EventHandler(this.clearHistory_btn_Click);
            // 
            // replaceText_txt
            // 
            this.replaceText_txt.Location = new System.Drawing.Point(79, 273);
            this.replaceText_txt.Name = "replaceText_txt";
            this.replaceText_txt.Size = new System.Drawing.Size(82, 38);
            this.replaceText_txt.TabIndex = 12;
            this.replaceText_txt.Text = ", ";
            this.toolTip.SetToolTip(this.replaceText_txt, "The text that replaces new line characters in the selected column when copying an" +
        "d replacing.");
            // 
            // copyReplace_btn
            // 
            this.copyReplace_btn.AutoSize = true;
            this.copyReplace_btn.Location = new System.Drawing.Point(79, 380);
            this.copyReplace_btn.Name = "copyReplace_btn";
            this.copyReplace_btn.Size = new System.Drawing.Size(304, 58);
            this.copyReplace_btn.TabIndex = 11;
            this.copyReplace_btn.Text = "Copy and Replace";
            this.toolTip.SetToolTip(this.copyReplace_btn, "Copies the selected columns and replaces the new line characters with the desired" +
        " characters in their place.");
            this.copyReplace_btn.UseVisualStyleBackColor = true;
            this.copyReplace_btn.Click += new System.EventHandler(this.copyReplace_btn_Click);
            // 
            // replaceComma_btn
            // 
            this.replaceComma_btn.AutoSize = true;
            this.replaceComma_btn.Location = new System.Drawing.Point(79, 317);
            this.replaceComma_btn.Name = "replaceComma_btn";
            this.replaceComma_btn.Size = new System.Drawing.Size(84, 57);
            this.replaceComma_btn.TabIndex = 13;
            this.replaceComma_btn.Text = ", ";
            this.toolTip.SetToolTip(this.replaceComma_btn, "Changes the replace text to a [, ] character, with no characters at the beginning" +
        " or end of the resulting string.");
            this.replaceComma_btn.UseVisualStyleBackColor = true;
            this.replaceComma_btn.Click += new System.EventHandler(this.replaceComma_btn_Click);
            // 
            // columnNum_txt
            // 
            this.columnNum_txt.AutoSize = true;
            this.columnNum_txt.Location = new System.Drawing.Point(1058, 23);
            this.columnNum_txt.Name = "columnNum_txt";
            this.columnNum_txt.Size = new System.Drawing.Size(189, 32);
            this.columnNum_txt.TabIndex = 14;
            this.columnNum_txt.Text = "# of Columns:";
            this.toolTip.SetToolTip(this.columnNum_txt, "The number of columns in the current request.");
            // 
            // rowNum_txt
            // 
            this.rowNum_txt.AutoSize = true;
            this.rowNum_txt.Location = new System.Drawing.Point(1058, 107);
            this.rowNum_txt.Name = "rowNum_txt";
            this.rowNum_txt.Size = new System.Drawing.Size(147, 32);
            this.rowNum_txt.TabIndex = 15;
            this.rowNum_txt.Text = "# of Rows:";
            this.toolTip.SetToolTip(this.rowNum_txt, "The number of rows in the currently selected column of the current request.");
            // 
            // currentColumn_txt
            // 
            this.currentColumn_txt.AutoSize = true;
            this.currentColumn_txt.Location = new System.Drawing.Point(1058, 65);
            this.currentColumn_txt.Name = "currentColumn_txt";
            this.currentColumn_txt.Size = new System.Drawing.Size(245, 32);
            this.currentColumn_txt.TabIndex = 16;
            this.currentColumn_txt.Text = "Current Column #:";
            this.toolTip.SetToolTip(this.currentColumn_txt, "The current column number in the current request.");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1058, 247);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(326, 32);
            this.label2.TabIndex = 17;
            this.label2.Text = "Default Column Number:";
            this.toolTip.SetToolTip(this.label2, "The default column number to switch to when a request is loaded.");
            // 
            // defaultColumn_txt
            // 
            this.defaultColumn_txt.Location = new System.Drawing.Point(1064, 286);
            this.defaultColumn_txt.Name = "defaultColumn_txt";
            this.defaultColumn_txt.Size = new System.Drawing.Size(320, 38);
            this.defaultColumn_txt.TabIndex = 18;
            this.defaultColumn_txt.Text = "0";
            this.toolTip.SetToolTip(this.defaultColumn_txt, "The default column number to switch to when a request is loaded.");
            // 
            // defaultColumnText_txt
            // 
            this.defaultColumnText_txt.Location = new System.Drawing.Point(1064, 380);
            this.defaultColumnText_txt.Name = "defaultColumnText_txt";
            this.defaultColumnText_txt.Size = new System.Drawing.Size(320, 38);
            this.defaultColumnText_txt.TabIndex = 20;
            this.toolTip.SetToolTip(this.defaultColumnText_txt, "The default name of the column to switch to when a request is loaded.");
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1058, 341);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(301, 32);
            this.label3.TabIndex = 19;
            this.label3.Text = "Default Column Name:";
            this.toolTip.SetToolTip(this.label3, "The default name of the column to switch to when a request is loaded.");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.threshold_txt);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.text_rbtn);
            this.panel1.Controls.Add(this.number_rbtn);
            this.panel1.Location = new System.Drawing.Point(1064, 438);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(326, 147);
            this.panel1.TabIndex = 21;
            // 
            // threshold_txt
            // 
            this.threshold_txt.Location = new System.Drawing.Point(148, 81);
            this.threshold_txt.Name = "threshold_txt";
            this.threshold_txt.Size = new System.Drawing.Size(155, 38);
            this.threshold_txt.TabIndex = 3;
            this.threshold_txt.Text = "5";
            this.toolTip.SetToolTip(this.threshold_txt, "The maximum different characters threshold for a default column name match.");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 4);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 32);
            this.label4.TabIndex = 2;
            this.label4.Text = "Default Priority";
            // 
            // text_rbtn
            // 
            this.text_rbtn.AutoSize = true;
            this.text_rbtn.Location = new System.Drawing.Point(14, 83);
            this.text_rbtn.Name = "text_rbtn";
            this.text_rbtn.Size = new System.Drawing.Size(127, 36);
            this.text_rbtn.TabIndex = 1;
            this.text_rbtn.Text = "Name";
            this.toolTip.SetToolTip(this.text_rbtn, "Use the default column name by default when loading a request, falling back on th" +
        "e default number if no column of the requested name exists, and then falling bac" +
        "k on the first column.");
            this.text_rbtn.UseVisualStyleBackColor = true;
            this.text_rbtn.CheckedChanged += new System.EventHandler(this.text_rbtn_CheckedChanged);
            // 
            // number_rbtn
            // 
            this.number_rbtn.AutoSize = true;
            this.number_rbtn.Checked = true;
            this.number_rbtn.Location = new System.Drawing.Point(14, 40);
            this.number_rbtn.Name = "number_rbtn";
            this.number_rbtn.Size = new System.Drawing.Size(152, 36);
            this.number_rbtn.TabIndex = 0;
            this.number_rbtn.TabStop = true;
            this.number_rbtn.Text = "Number";
            this.toolTip.SetToolTip(this.number_rbtn, "Use the default column number by default when loading a request, falling back on " +
        "the default name if no column of the requested number exists, and then falling b" +
        "ack on the first column.");
            this.number_rbtn.UseVisualStyleBackColor = true;
            this.number_rbtn.CheckedChanged += new System.EventHandler(this.number_rbtn_CheckedChanged);
            // 
            // isTop_cbx
            // 
            this.isTop_cbx.AutoSize = true;
            this.isTop_cbx.Location = new System.Drawing.Point(1064, 590);
            this.isTop_cbx.Name = "isTop_cbx";
            this.isTop_cbx.Size = new System.Drawing.Size(219, 36);
            this.isTop_cbx.TabIndex = 22;
            this.isTop_cbx.Text = "Show on Top";
            this.toolTip.SetToolTip(this.isTop_cbx, "Whether this program should be shown on top of all others or not.");
            this.isTop_cbx.UseVisualStyleBackColor = true;
            this.isTop_cbx.CheckedChanged += new System.EventHandler(this.isTop_cbx_CheckedChanged);
            // 
            // saveSettings_btn
            // 
            this.saveSettings_btn.AutoSize = true;
            this.saveSettings_btn.Location = new System.Drawing.Point(441, 644);
            this.saveSettings_btn.Name = "saveSettings_btn";
            this.saveSettings_btn.Size = new System.Drawing.Size(252, 58);
            this.saveSettings_btn.TabIndex = 23;
            this.saveSettings_btn.Text = "Save State";
            this.toolTip.SetToolTip(this.saveSettings_btn, "Saves the current state of the program.");
            this.saveSettings_btn.UseVisualStyleBackColor = true;
            this.saveSettings_btn.Click += new System.EventHandler(this.saveSettings_btn_Click);
            // 
            // loadSettings_btn
            // 
            this.loadSettings_btn.AutoSize = true;
            this.loadSettings_btn.Location = new System.Drawing.Point(1000, 644);
            this.loadSettings_btn.Name = "loadSettings_btn";
            this.loadSettings_btn.Size = new System.Drawing.Size(241, 58);
            this.loadSettings_btn.TabIndex = 24;
            this.loadSettings_btn.Text = "Load State";
            this.toolTip.SetToolTip(this.loadSettings_btn, "Load a new program state from another save file.");
            this.loadSettings_btn.UseVisualStyleBackColor = true;
            this.loadSettings_btn.Click += new System.EventHandler(this.loadSettings_btn_Click);
            // 
            // saveAsNew_btn
            // 
            this.saveAsNew_btn.AutoSize = true;
            this.saveAsNew_btn.Location = new System.Drawing.Point(699, 644);
            this.saveAsNew_btn.Name = "saveAsNew_btn";
            this.saveAsNew_btn.Size = new System.Drawing.Size(295, 58);
            this.saveAsNew_btn.TabIndex = 25;
            this.saveAsNew_btn.Text = "Save As New State";
            this.toolTip.SetToolTip(this.saveAsNew_btn, "Save the current state of the program to a new file.");
            this.saveAsNew_btn.UseVisualStyleBackColor = true;
            this.saveAsNew_btn.Click += new System.EventHandler(this.saveAsNew_btn_Click);
            // 
            // header_cxb
            // 
            this.header_cxb.AutoSize = true;
            this.header_cxb.Checked = true;
            this.header_cxb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.header_cxb.Location = new System.Drawing.Point(1064, 196);
            this.header_cxb.Name = "header_cxb";
            this.header_cxb.Size = new System.Drawing.Size(284, 36);
            this.header_cxb.TabIndex = 26;
            this.header_cxb.Text = "Data Has Headers";
            this.toolTip.SetToolTip(this.header_cxb, "Whether the new data has a header row or not.");
            this.header_cxb.UseVisualStyleBackColor = true;
            this.header_cxb.CheckedChanged += new System.EventHandler(this.header_cxb_CheckedChanged);
            // 
            // removeBlanks_cxb
            // 
            this.removeBlanks_cxb.AutoSize = true;
            this.removeBlanks_cxb.Checked = true;
            this.removeBlanks_cxb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.removeBlanks_cxb.Location = new System.Drawing.Point(1064, 154);
            this.removeBlanks_cxb.Name = "removeBlanks_cxb";
            this.removeBlanks_cxb.Size = new System.Drawing.Size(313, 36);
            this.removeBlanks_cxb.TabIndex = 27;
            this.removeBlanks_cxb.Text = "Remove Blank Lines";
            this.toolTip.SetToolTip(this.removeBlanks_cxb, "Whether to remove blank lines from a column when processing new requests.");
            this.removeBlanks_cxb.UseVisualStyleBackColor = true;
            this.removeBlanks_cxb.CheckedChanged += new System.EventHandler(this.removeBlanks_cxb_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.export_btn);
            this.panel2.Controls.Add(this.replaceTextPost_txt);
            this.panel2.Controls.Add(this.replaceTextPre_txt);
            this.panel2.Controls.Add(this.replaceQuotedComma_btn);
            this.panel2.Controls.Add(this.statusStrip1);
            this.panel2.Controls.Add(this.help_btn);
            this.panel2.Controls.Add(this.replaceSemiColon_btn);
            this.panel2.Controls.Add(this.paste_btn);
            this.panel2.Controls.Add(this.removeBlanks_cxb);
            this.panel2.Controls.Add(this.pasteCopy_btn);
            this.panel2.Controls.Add(this.header_cxb);
            this.panel2.Controls.Add(this.copy_btn);
            this.panel2.Controls.Add(this.saveAsNew_btn);
            this.panel2.Controls.Add(this.copyLine_btn);
            this.panel2.Controls.Add(this.loadSettings_btn);
            this.panel2.Controls.Add(this.line_txt);
            this.panel2.Controls.Add(this.saveSettings_btn);
            this.panel2.Controls.Add(this.column_txt);
            this.panel2.Controls.Add(this.isTop_cbx);
            this.panel2.Controls.Add(this.column_cmb);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.history_cmb);
            this.panel2.Controls.Add(this.defaultColumnText_txt);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.history_txt);
            this.panel2.Controls.Add(this.defaultColumn_txt);
            this.panel2.Controls.Add(this.clearHistory_btn);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.copyReplace_btn);
            this.panel2.Controls.Add(this.currentColumn_txt);
            this.panel2.Controls.Add(this.replaceText_txt);
            this.panel2.Controls.Add(this.rowNum_txt);
            this.panel2.Controls.Add(this.replaceComma_btn);
            this.panel2.Controls.Add(this.columnNum_txt);
            this.panel2.Location = new System.Drawing.Point(2, 1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1454, 771);
            this.panel2.TabIndex = 28;
            // 
            // replaceTextPost_txt
            // 
            this.replaceTextPost_txt.Location = new System.Drawing.Point(301, 273);
            this.replaceTextPost_txt.Name = "replaceTextPost_txt";
            this.replaceTextPost_txt.Size = new System.Drawing.Size(82, 38);
            this.replaceTextPost_txt.TabIndex = 33;
            this.toolTip.SetToolTip(this.replaceTextPost_txt, "The text that ends the resulting line when copying and replacing.");
            // 
            // replaceTextPre_txt
            // 
            this.replaceTextPre_txt.Location = new System.Drawing.Point(189, 273);
            this.replaceTextPre_txt.Name = "replaceTextPre_txt";
            this.replaceTextPre_txt.Size = new System.Drawing.Size(82, 38);
            this.replaceTextPre_txt.TabIndex = 32;
            this.toolTip.SetToolTip(this.replaceTextPre_txt, "The text that begins the resulting line when copying and replacing.");
            // 
            // replaceQuotedComma_btn
            // 
            this.replaceQuotedComma_btn.AutoSize = true;
            this.replaceQuotedComma_btn.Location = new System.Drawing.Point(189, 317);
            this.replaceQuotedComma_btn.Name = "replaceQuotedComma_btn";
            this.replaceQuotedComma_btn.Size = new System.Drawing.Size(84, 57);
            this.replaceQuotedComma_btn.TabIndex = 31;
            this.replaceQuotedComma_btn.Text = "\",\" ";
            this.toolTip.SetToolTip(this.replaceQuotedComma_btn, "Changes the replace text to a [\", \"] character, with a \" at the beginning and end" +
        " of the resulting string.");
            this.replaceQuotedComma_btn.UseVisualStyleBackColor = true;
            this.replaceQuotedComma_btn.Click += new System.EventHandler(this.replaceQuotedComma_btn_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status_txt});
            this.statusStrip1.Location = new System.Drawing.Point(0, 725);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1454, 46);
            this.statusStrip1.TabIndex = 30;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status_txt
            // 
            this.status_txt.Name = "status_txt";
            this.status_txt.Size = new System.Drawing.Size(122, 41);
            this.status_txt.Text = "             ";
            // 
            // help_btn
            // 
            this.help_btn.Location = new System.Drawing.Point(1247, 644);
            this.help_btn.Name = "help_btn";
            this.help_btn.Size = new System.Drawing.Size(196, 58);
            this.help_btn.TabIndex = 29;
            this.help_btn.Text = "Help/About";
            this.toolTip.SetToolTip(this.help_btn, "Bring up the help/about window.");
            this.help_btn.UseVisualStyleBackColor = true;
            this.help_btn.Click += new System.EventHandler(this.help_btn_Click);
            // 
            // replaceSemiColon_btn
            // 
            this.replaceSemiColon_btn.AutoSize = true;
            this.replaceSemiColon_btn.Location = new System.Drawing.Point(299, 317);
            this.replaceSemiColon_btn.Name = "replaceSemiColon_btn";
            this.replaceSemiColon_btn.Size = new System.Drawing.Size(84, 57);
            this.replaceSemiColon_btn.TabIndex = 28;
            this.replaceSemiColon_btn.Text = ";";
            this.toolTip.SetToolTip(this.replaceSemiColon_btn, "Changes the replace text to a [;] character, with no characters at the beginning " +
        "or end of the resulting string.");
            this.replaceSemiColon_btn.UseVisualStyleBackColor = true;
            this.replaceSemiColon_btn.Click += new System.EventHandler(this.replaceSemiColon_btn_Click);
            // 
            // export_btn
            // 
            this.export_btn.AutoSize = true;
            this.export_btn.Location = new System.Drawing.Point(22, 444);
            this.export_btn.Name = "export_btn";
            this.export_btn.Size = new System.Drawing.Size(391, 58);
            this.export_btn.TabIndex = 34;
            this.export_btn.Text = "Export Current Request";
            this.toolTip.SetToolTip(this.export_btn, "Exports the current request to a tab and line seperated format.");
            this.export_btn.UseVisualStyleBackColor = true;
            this.export_btn.Click += new System.EventHandler(this.export_btn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(240F, 240F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1457, 773);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Column Copier";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button paste_btn;
        private System.Windows.Forms.Button pasteCopy_btn;
        private System.Windows.Forms.Button copy_btn;
        private System.Windows.Forms.Button copyLine_btn;
        private System.Windows.Forms.TextBox line_txt;
        private System.Windows.Forms.TextBox column_txt;
        private System.Windows.Forms.ComboBox column_cmb;
        private System.Windows.Forms.ComboBox history_cmb;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox history_txt;
        private System.Windows.Forms.Button clearHistory_btn;
        private System.Windows.Forms.TextBox replaceText_txt;
        private System.Windows.Forms.Button copyReplace_btn;
        private System.Windows.Forms.Button replaceComma_btn;
        private System.Windows.Forms.Label columnNum_txt;
        private System.Windows.Forms.Label rowNum_txt;
        private System.Windows.Forms.Label currentColumn_txt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox defaultColumn_txt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox defaultColumnText_txt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton text_rbtn;
        private System.Windows.Forms.RadioButton number_rbtn;
        private System.Windows.Forms.CheckBox isTop_cbx;
        private System.Windows.Forms.Button saveSettings_btn;
        private System.Windows.Forms.Button saveAsNew_btn;
        private System.Windows.Forms.Button loadSettings_btn;
        private System.Windows.Forms.CheckBox header_cxb;
        private System.Windows.Forms.TextBox threshold_txt;
        private System.Windows.Forms.CheckBox removeBlanks_cxb;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button replaceSemiColon_btn;
        private System.Windows.Forms.Button help_btn;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status_txt;
        private System.Windows.Forms.Button replaceQuotedComma_btn;
        private System.Windows.Forms.TextBox replaceTextPost_txt;
        private System.Windows.Forms.TextBox replaceTextPre_txt;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button export_btn;
    }
}

