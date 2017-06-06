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
    public partial class InputComboDialogBox : Form
    {
        public InputComboDialogBox()
        {
            InitializeComponent();
        }

        private void ok_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancel_btn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public string InputText
        {
            get { return input_cmb.Items[input_cmb.SelectedIndex].ToString(); }
        }

        public int InputSelectedItem
        {
            get { return input_cmb.SelectedIndex; }
            set { input_cmb.SelectedIndex = value; }
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

        public void SetInputListItems(List<string> items)
        {
            input_cmb.Items.Clear();
            for (var i = 0; i < items.Count; i++)
                input_cmb.Items.Add(items[i]);
        }
    }
}
