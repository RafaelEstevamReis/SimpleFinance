using Simple.Finance.Tables;
using System;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgEditWallet : Form
    {
        Wallet wallet;
        public dlgEditWallet()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Wallet name bust be longer than 1");
                return;
            }

            wallet.Name = txtName.Text;
            DialogResult = DialogResult.OK;
        }

        public static DialogResult ShowDialog(Wallet wallet)
        {
            using var frm = new dlgEditWallet();
            frm.wallet = wallet;
            frm.lblId.Text = wallet.Id.ToString();
            frm.txtName.Text = wallet.Name;

            return frm.ShowDialog();
        }

    }
}
