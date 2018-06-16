namespace ColumnCopier
{
    partial class OutputMultiColumnCopyWizard
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
            this.ok_btn = new System.Windows.Forms.Button();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.seperatorOption_cmb = new System.Windows.Forms.ComboBox();
            this.seperatorItem_txt = new System.Windows.Forms.TextBox();
            this.seperatorItemPost_txt = new System.Windows.Forms.TextBox();
            this.seperatorItemPre_txt = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lineSeperatorOption_cmb = new System.Windows.Forms.ComboBox();
            this.lineSeperatorItem_txt = new System.Windows.Forms.TextBox();
            this.availableColumns_lst = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.deselect_btn = new System.Windows.Forms.Button();
            this.select_btn = new System.Windows.Forms.Button();
            this.selectedColumns_lst = new System.Windows.Forms.ListBox();
            this.columnDown_btn = new System.Windows.Forms.Button();
            this.columnUp_btn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.currentColumnText_txt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.currentSelectedColumn_txt = new System.Windows.Forms.TextBox();
            this.includeColumnHeaders_cxb = new System.Windows.Forms.CheckBox();
            this.groupBox5.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ok_btn
            // 
            this.ok_btn.Location = new System.Drawing.Point(813, 908);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(237, 63);
            this.ok_btn.TabIndex = 1;
            this.ok_btn.Text = "Ok";
            this.ok_btn.UseVisualStyleBackColor = true;
            this.ok_btn.Click += new System.EventHandler(this.ok_btn_Click);
            // 
            // cancel_btn
            // 
            this.cancel_btn.Location = new System.Drawing.Point(375, 908);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(237, 63);
            this.cancel_btn.TabIndex = 2;
            this.cancel_btn.Text = "Cancel";
            this.cancel_btn.UseVisualStyleBackColor = true;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label15);
            this.groupBox5.Controls.Add(this.seperatorOption_cmb);
            this.groupBox5.Controls.Add(this.seperatorItem_txt);
            this.groupBox5.Controls.Add(this.seperatorItemPost_txt);
            this.groupBox5.Controls.Add(this.seperatorItemPre_txt);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(218, 741);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Size = new System.Drawing.Size(389, 162);
            this.groupBox5.TabIndex = 45;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Column Separators";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(8, 107);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(213, 32);
            this.label15.TabIndex = 48;
            this.label15.Text = "Column Option:";
            // 
            // seperatorOption_cmb
            // 
            this.seperatorOption_cmb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seperatorOption_cmb.FormattingEnabled = true;
            this.seperatorOption_cmb.Items.AddRange(new object[] {
            ",",
            "",
            "\",\"",
            "( , )",
            "(\' \', \' \')",
            "(\" \", \" \")",
            ";"});
            this.seperatorOption_cmb.Location = new System.Drawing.Point(264, 100);
            this.seperatorOption_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.seperatorOption_cmb.Name = "seperatorOption_cmb";
            this.seperatorOption_cmb.Size = new System.Drawing.Size(119, 39);
            this.seperatorOption_cmb.TabIndex = 47;
            this.seperatorOption_cmb.Text = ",";
            this.seperatorOption_cmb.SelectedIndexChanged += new System.EventHandler(this.seperatorOption_cmb_SelectedIndexChanged);
            // 
            // seperatorItem_txt
            // 
            this.seperatorItem_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seperatorItem_txt.Location = new System.Drawing.Point(157, 48);
            this.seperatorItem_txt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.seperatorItem_txt.Name = "seperatorItem_txt";
            this.seperatorItem_txt.Size = new System.Drawing.Size(81, 38);
            this.seperatorItem_txt.TabIndex = 44;
            this.seperatorItem_txt.Text = ", ";
            // 
            // seperatorItemPost_txt
            // 
            this.seperatorItemPost_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seperatorItemPost_txt.Location = new System.Drawing.Point(299, 48);
            this.seperatorItemPost_txt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.seperatorItemPost_txt.Name = "seperatorItemPost_txt";
            this.seperatorItemPost_txt.Size = new System.Drawing.Size(81, 38);
            this.seperatorItemPost_txt.TabIndex = 46;
            // 
            // seperatorItemPre_txt
            // 
            this.seperatorItemPre_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seperatorItemPre_txt.Location = new System.Drawing.Point(16, 48);
            this.seperatorItemPre_txt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.seperatorItemPre_txt.Name = "seperatorItemPre_txt";
            this.seperatorItemPre_txt.Size = new System.Drawing.Size(81, 38);
            this.seperatorItemPre_txt.TabIndex = 45;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lineSeperatorOption_cmb);
            this.groupBox1.Controls.Add(this.lineSeperatorItem_txt);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(813, 741);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(389, 162);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Line Separators";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 32);
            this.label1.TabIndex = 48;
            this.label1.Text = "Line Option:";
            // 
            // lineSeperatorOption_cmb
            // 
            this.lineSeperatorOption_cmb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineSeperatorOption_cmb.FormattingEnabled = true;
            this.lineSeperatorOption_cmb.Items.AddRange(new object[] {
            "\\n",
            ""});
            this.lineSeperatorOption_cmb.Location = new System.Drawing.Point(264, 100);
            this.lineSeperatorOption_cmb.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lineSeperatorOption_cmb.Name = "lineSeperatorOption_cmb";
            this.lineSeperatorOption_cmb.Size = new System.Drawing.Size(119, 39);
            this.lineSeperatorOption_cmb.TabIndex = 47;
            this.lineSeperatorOption_cmb.Text = "\\n";
            this.lineSeperatorOption_cmb.SelectedIndexChanged += new System.EventHandler(this.lineSeperatorOption_cmb_SelectedIndexChanged);
            // 
            // lineSeperatorItem_txt
            // 
            this.lineSeperatorItem_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lineSeperatorItem_txt.Location = new System.Drawing.Point(157, 48);
            this.lineSeperatorItem_txt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lineSeperatorItem_txt.Name = "lineSeperatorItem_txt";
            this.lineSeperatorItem_txt.Size = new System.Drawing.Size(81, 38);
            this.lineSeperatorItem_txt.TabIndex = 44;
            this.lineSeperatorItem_txt.Text = "\\n";
            // 
            // availableColumns_lst
            // 
            this.availableColumns_lst.FormattingEnabled = true;
            this.availableColumns_lst.ItemHeight = 31;
            this.availableColumns_lst.Location = new System.Drawing.Point(12, 47);
            this.availableColumns_lst.Name = "availableColumns_lst";
            this.availableColumns_lst.Size = new System.Drawing.Size(595, 345);
            this.availableColumns_lst.TabIndex = 47;
            this.availableColumns_lst.SelectedIndexChanged += new System.EventHandler(this.availableColumns_lst_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(260, 32);
            this.label2.TabIndex = 49;
            this.label2.Text = "Available Columns:";
            // 
            // deselect_btn
            // 
            this.deselect_btn.Location = new System.Drawing.Point(613, 101);
            this.deselect_btn.Name = "deselect_btn";
            this.deselect_btn.Size = new System.Drawing.Size(194, 52);
            this.deselect_btn.TabIndex = 50;
            this.deselect_btn.Text = "<--";
            this.deselect_btn.UseVisualStyleBackColor = true;
            this.deselect_btn.Click += new System.EventHandler(this.deselect_btn_Click);
            // 
            // select_btn
            // 
            this.select_btn.Location = new System.Drawing.Point(613, 159);
            this.select_btn.Name = "select_btn";
            this.select_btn.Size = new System.Drawing.Size(194, 52);
            this.select_btn.TabIndex = 51;
            this.select_btn.Text = "-->";
            this.select_btn.UseVisualStyleBackColor = true;
            this.select_btn.Click += new System.EventHandler(this.select_btn_Click);
            // 
            // selectedColumns_lst
            // 
            this.selectedColumns_lst.FormattingEnabled = true;
            this.selectedColumns_lst.ItemHeight = 31;
            this.selectedColumns_lst.Location = new System.Drawing.Point(813, 47);
            this.selectedColumns_lst.Name = "selectedColumns_lst";
            this.selectedColumns_lst.Size = new System.Drawing.Size(595, 345);
            this.selectedColumns_lst.TabIndex = 52;
            this.selectedColumns_lst.SelectedIndexChanged += new System.EventHandler(this.selectedColumns_lst_SelectedIndexChanged);
            // 
            // columnDown_btn
            // 
            this.columnDown_btn.Location = new System.Drawing.Point(1414, 159);
            this.columnDown_btn.Name = "columnDown_btn";
            this.columnDown_btn.Size = new System.Drawing.Size(194, 52);
            this.columnDown_btn.TabIndex = 54;
            this.columnDown_btn.Text = "\\/";
            this.columnDown_btn.UseVisualStyleBackColor = true;
            this.columnDown_btn.Click += new System.EventHandler(this.columnDown_btn_Click);
            // 
            // columnUp_btn
            // 
            this.columnUp_btn.Location = new System.Drawing.Point(1414, 101);
            this.columnUp_btn.Name = "columnUp_btn";
            this.columnUp_btn.Size = new System.Drawing.Size(194, 52);
            this.columnUp_btn.TabIndex = 53;
            this.columnUp_btn.Text = "/\\";
            this.columnUp_btn.UseVisualStyleBackColor = true;
            this.columnUp_btn.Click += new System.EventHandler(this.columnUp_btn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(807, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(254, 32);
            this.label3.TabIndex = 55;
            this.label3.Text = "Selected Columns:";
            // 
            // currentColumnText_txt
            // 
            this.currentColumnText_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentColumnText_txt.Location = new System.Drawing.Point(12, 429);
            this.currentColumnText_txt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.currentColumnText_txt.Multiline = true;
            this.currentColumnText_txt.Name = "currentColumnText_txt";
            this.currentColumnText_txt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.currentColumnText_txt.Size = new System.Drawing.Size(595, 308);
            this.currentColumnText_txt.TabIndex = 56;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 395);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(348, 32);
            this.label4.TabIndex = 57;
            this.label4.Text = "Selected Column Preview:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(813, 395);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(348, 32);
            this.label5.TabIndex = 59;
            this.label5.Text = "Selected Column Preview:";
            // 
            // currentSelectedColumn_txt
            // 
            this.currentSelectedColumn_txt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.1F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentSelectedColumn_txt.Location = new System.Drawing.Point(813, 429);
            this.currentSelectedColumn_txt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.currentSelectedColumn_txt.Multiline = true;
            this.currentSelectedColumn_txt.Name = "currentSelectedColumn_txt";
            this.currentSelectedColumn_txt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.currentSelectedColumn_txt.Size = new System.Drawing.Size(595, 308);
            this.currentSelectedColumn_txt.TabIndex = 58;
            // 
            // includeColumnHeaders_cxb
            // 
            this.includeColumnHeaders_cxb.AutoSize = true;
            this.includeColumnHeaders_cxb.Location = new System.Drawing.Point(1239, 758);
            this.includeColumnHeaders_cxb.Name = "includeColumnHeaders_cxb";
            this.includeColumnHeaders_cxb.Size = new System.Drawing.Size(364, 36);
            this.includeColumnHeaders_cxb.TabIndex = 60;
            this.includeColumnHeaders_cxb.Text = "Include Column Headers";
            this.includeColumnHeaders_cxb.UseVisualStyleBackColor = true;
            // 
            // OutputMultiColumnCopyWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1615, 982);
            this.Controls.Add(this.includeColumnHeaders_cxb);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.currentSelectedColumn_txt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.currentColumnText_txt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.columnDown_btn);
            this.Controls.Add(this.columnUp_btn);
            this.Controls.Add(this.selectedColumns_lst);
            this.Controls.Add(this.select_btn);
            this.Controls.Add(this.deselect_btn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.availableColumns_lst);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.ok_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OutputMultiColumnCopyWizard";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Copy Multiple Column Wizard";
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.Button cancel_btn;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox seperatorOption_cmb;
        private System.Windows.Forms.TextBox seperatorItem_txt;
        private System.Windows.Forms.TextBox seperatorItemPost_txt;
        private System.Windows.Forms.TextBox seperatorItemPre_txt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox lineSeperatorOption_cmb;
        private System.Windows.Forms.TextBox lineSeperatorItem_txt;
        private System.Windows.Forms.ListBox availableColumns_lst;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button deselect_btn;
        private System.Windows.Forms.Button select_btn;
        private System.Windows.Forms.ListBox selectedColumns_lst;
        private System.Windows.Forms.Button columnDown_btn;
        private System.Windows.Forms.Button columnUp_btn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox currentColumnText_txt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox currentSelectedColumn_txt;
        private System.Windows.Forms.CheckBox includeColumnHeaders_cxb;
    }
}