namespace ColumnCopier
{
    partial class InputComboDialogBox
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
            this.question_txt = new System.Windows.Forms.Label();
            this.cancel_btn = new System.Windows.Forms.Button();
            this.ok_btn = new System.Windows.Forms.Button();
            this.input_cmb = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // question_txt
            // 
            this.question_txt.AutoSize = true;
            this.question_txt.Location = new System.Drawing.Point(12, 9);
            this.question_txt.Name = "question_txt";
            this.question_txt.Size = new System.Drawing.Size(93, 32);
            this.question_txt.TabIndex = 7;
            this.question_txt.Text = "label1";
            // 
            // cancel_btn
            // 
            this.cancel_btn.Location = new System.Drawing.Point(18, 110);
            this.cancel_btn.Name = "cancel_btn";
            this.cancel_btn.Size = new System.Drawing.Size(237, 63);
            this.cancel_btn.TabIndex = 6;
            this.cancel_btn.Text = "Cancel";
            this.cancel_btn.UseVisualStyleBackColor = true;
            this.cancel_btn.Click += new System.EventHandler(this.cancel_btn_Click);
            // 
            // ok_btn
            // 
            this.ok_btn.Location = new System.Drawing.Point(361, 110);
            this.ok_btn.Name = "ok_btn";
            this.ok_btn.Size = new System.Drawing.Size(194, 63);
            this.ok_btn.TabIndex = 5;
            this.ok_btn.Text = "Ok";
            this.ok_btn.UseVisualStyleBackColor = true;
            this.ok_btn.Click += new System.EventHandler(this.ok_btn_Click);
            // 
            // input_cmb
            // 
            this.input_cmb.FormattingEnabled = true;
            this.input_cmb.Location = new System.Drawing.Point(18, 54);
            this.input_cmb.Name = "input_cmb";
            this.input_cmb.Size = new System.Drawing.Size(537, 39);
            this.input_cmb.TabIndex = 8;
            // 
            // InputComboDialogBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 228);
            this.Controls.Add(this.input_cmb);
            this.Controls.Add(this.question_txt);
            this.Controls.Add(this.cancel_btn);
            this.Controls.Add(this.ok_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputComboDialogBox";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " ";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label question_txt;
        private System.Windows.Forms.Button cancel_btn;
        private System.Windows.Forms.Button ok_btn;
        private System.Windows.Forms.ComboBox input_cmb;
    }
}