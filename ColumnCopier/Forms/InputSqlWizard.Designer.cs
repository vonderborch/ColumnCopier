namespace ColumnCopier
{
    partial class InputSqlWizard
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
            this.query_txt = new System.Windows.Forms.TextBox();
            this.ok_btn = new System.Windows.Forms.Button();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.question_txt = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.conn_txt = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.providerSqlServer_rbn = new System.Windows.Forms.RadioButton();
            this.providerPostgreSql_rbn = new System.Windows.Forms.RadioButton();
            this.providerMySql_rbn = new System.Windows.Forms.RadioButton();
            this.providerNone_rbn = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // query_txt
            // 
            this.query_txt.Location = new System.Drawing.Point(182, 205);
            this.query_txt.Multiline = true;
            this.query_txt.Name = "query_txt";
            this.query_txt.Size = new System.Drawing.Size(1082, 616);
            this.query_txt.TabIndex = 0;
            // 
            // ok_btn
            // 
            this.ok_btn.Location = new System.Drawing.Point(1070, 827);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(194, 63);
            this.ok_btn.TabIndex = 1;
            this.ok_btn.Text = "Ok";
            this.ok_btn.UseVisualStyleBackColor = true;
            this.ok_btn.Click += new System.EventHandler(this.ok_btn_Click);
            // 
            // cancel_btn
            // 
            this.cancel_btn.Location = new System.Drawing.Point(827, 827);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(237, 63);
            this.cancel_btn.TabIndex = 2;
            this.cancel_btn.Text = "Cancel";
            this.cancel_btn.UseVisualStyleBackColor = true;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // question_txt
            // 
            this.question_txt.AutoSize = true;
            this.question_txt.Location = new System.Drawing.Point(12, 205);
            this.question_txt.Name = "question_txt";
            this.question_txt.Size = new System.Drawing.Size(164, 32);
            this.question_txt.TabIndex = 3;
            this.question_txt.Text = "SQL Query:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(462, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 32);
            this.label1.TabIndex = 5;
            this.label1.Text = "Connection String:";
            // 
            // conn_txt
            // 
            this.conn_txt.Location = new System.Drawing.Point(718, 9);
            this.conn_txt.Multiline = true;
            this.conn_txt.Name = "conn_txt";
            this.conn_txt.Size = new System.Drawing.Size(546, 190);
            this.conn_txt.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.providerSqlServer_rbn);
            this.panel1.Controls.Add(this.providerPostgreSql_rbn);
            this.panel1.Controls.Add(this.providerMySql_rbn);
            this.panel1.Controls.Add(this.providerNone_rbn);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(438, 180);
            this.panel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(193, 32);
            this.label2.TabIndex = 7;
            this.label2.Text = "SQL Provider:";
            // 
            // providerSqlServer_rbn
            // 
            this.providerSqlServer_rbn.AutoSize = true;
            this.providerSqlServer_rbn.Location = new System.Drawing.Point(212, 134);
            this.providerSqlServer_rbn.Name = "providerSqlServer_rbn";
            this.providerSqlServer_rbn.Size = new System.Drawing.Size(199, 36);
            this.providerSqlServer_rbn.TabIndex = 3;
            this.providerSqlServer_rbn.Text = "SQL Server";
            this.providerSqlServer_rbn.UseVisualStyleBackColor = true;
            // 
            // providerPostgreSql_rbn
            // 
            this.providerPostgreSql_rbn.AutoSize = true;
            this.providerPostgreSql_rbn.Location = new System.Drawing.Point(212, 92);
            this.providerPostgreSql_rbn.Name = "providerPostgreSql_rbn";
            this.providerPostgreSql_rbn.Size = new System.Drawing.Size(207, 36);
            this.providerPostgreSql_rbn.TabIndex = 2;
            this.providerPostgreSql_rbn.Text = "PostgreSQL";
            this.providerPostgreSql_rbn.UseVisualStyleBackColor = true;
            // 
            // providerMySql_rbn
            // 
            this.providerMySql_rbn.AutoSize = true;
            this.providerMySql_rbn.Location = new System.Drawing.Point(212, 50);
            this.providerMySql_rbn.Name = "providerMySql_rbn";
            this.providerMySql_rbn.Size = new System.Drawing.Size(131, 36);
            this.providerMySql_rbn.TabIndex = 1;
            this.providerMySql_rbn.Text = "MySql";
            this.providerMySql_rbn.UseVisualStyleBackColor = true;
            // 
            // providerNone_rbn
            // 
            this.providerNone_rbn.AutoSize = true;
            this.providerNone_rbn.Checked = true;
            this.providerNone_rbn.Location = new System.Drawing.Point(212, 8);
            this.providerNone_rbn.Name = "providerNone_rbn";
            this.providerNone_rbn.Size = new System.Drawing.Size(120, 36);
            this.providerNone_rbn.TabIndex = 0;
            this.providerNone_rbn.TabStop = true;
            this.providerNone_rbn.Text = "None";
            this.providerNone_rbn.UseVisualStyleBackColor = true;
            // 
            // InputSqlWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1281, 902);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.conn_txt);
            this.Controls.Add(this.question_txt);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.ok_btn);
            this.Controls.Add(this.query_txt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputSqlWizard";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SQL Input Wizard";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox query_txt;
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.Button cancel_btn;
        private System.Windows.Forms.Label question_txt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox conn_txt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton providerSqlServer_rbn;
        private System.Windows.Forms.RadioButton providerPostgreSql_rbn;
        private System.Windows.Forms.RadioButton providerMySql_rbn;
        private System.Windows.Forms.RadioButton providerNone_rbn;
        private System.Windows.Forms.Label label2;
    }
}