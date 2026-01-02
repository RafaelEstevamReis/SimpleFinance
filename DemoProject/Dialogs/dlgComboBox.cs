using System;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgComboBox : Form
    {
        public dlgComboBox()
        {
            InitializeComponent();
        }

        private void dlgComboBox_Load(object sender, EventArgs e)
        {
            if (cboBox.Items.Count > 0) cboBox.SelectedIndex = 0;
        }
        private void dlgComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnSave.PerformClick();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboBox.SelectedIndex < 0)
            {
                MessageBox.Show("You must select a value");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        public static DialogResult ShowDialog(string caption, object[] items, out long outputValue)
            => ShowDialog(caption, caption, items, "Id", "Name", out outputValue);
        public static DialogResult ShowDialog(string caption, string title, object[] items, string valueMember, string displayMember, out long outputValue)
        {
            outputValue = -1;

            using var frm = new dlgComboBox();
            frm.Text = caption;
            frm.lblTitle.Text = title;

            frm.cboBox.DataSource = items;
            frm.cboBox.DisplayMember = displayMember;
            frm.cboBox.ValueMember = valueMember;

            var result = frm.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (frm.cboBox.SelectedValue is string) outputValue = frm.cboBox.SelectedIndex;
                else outputValue = Convert.ToInt64(frm.cboBox.SelectedValue);
            }
            return result;
        }

    }
}
