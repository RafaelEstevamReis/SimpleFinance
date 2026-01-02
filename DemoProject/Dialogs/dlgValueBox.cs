using DemoProject.Components;
using System;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgValueBox : DialogBase
    {
        private Func<decimal, bool>? validationFunc;

        public dlgValueBox()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = true;
            if (validationFunc != null)
            {
                isValid = validationFunc(txtMoneybox.Value);
            }

            if (!isValid)
            {
                MessageBox.Show("The value is invalid");
                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void dlgValueBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnSave.PerformClick();
            }
        }

        public static DialogResult ShowDialog(string caption, decimal inputValue, out decimal outputValue)
            => ShowDialog(caption, caption, 2, inputValue, null, out outputValue);
        public static DialogResult ShowDialog(string caption, int decimalPlaces, decimal inputValue, out decimal outputValue)
            => ShowDialog(caption, caption, decimalPlaces, inputValue, null, out outputValue);
        public static DialogResult ShowDialog(string caption, string title, int decimalPlaces, decimal inputValue, Func<decimal, bool>? isValid, out decimal outputValue)
        {
            outputValue = inputValue;

            using var frm = new dlgValueBox();
            frm.validationFunc = isValid;
            frm.Text = caption;
            frm.lblTitle.Text = title;
            frm.txtMoneybox.DecimalPlaces = decimalPlaces;
            frm.txtMoneybox.Value = inputValue;
            var result = frm.ShowDialog();

            if (result == DialogResult.OK)
            {
                outputValue = frm.txtMoneybox.Value;
            }
            return result;
        }

    }
}
