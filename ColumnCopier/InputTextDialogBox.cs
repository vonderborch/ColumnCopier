using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColumnCopier
{
    public partial class InputTextDialogBox : Form
    {
        public InputTextDialogBox()
        {
            InitializeComponent();
        }

        public string InputText
        {
            get { return input_txt.Text; }
            set { input_txt.Text = value; }
        }

        public string QuestionText
        {
            get { return this.Text; }
            set
            {
                this.Text = value;
                question_txt.Text = value;
            }
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
