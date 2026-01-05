using DemoProject.Components;
using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgUpdateWalletTransfer : DialogBase
    {
        private Manager manager;
        private Transac oneTransaction = null!;

        public dlgUpdateWalletTransfer()
        {
            manager = Injector.Get<Manager>();
            InitializeComponent();
        }

        private void dlgUpdateWalletTransfer_Load(object sender, EventArgs e)
        {
            dtDate.Value = oneTransaction.DueDate;
            txtValue.Value = Math.Abs(oneTransaction.DueValue);
            txtName.Text = oneTransaction.Description;
            if (oneTransaction.Status == Transac.PaymentStatus.Paid) rdoPaid.Checked = true;
            else rdoUnpaid.Checked = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Description must be longer than 1");
                return;
            }

            if (txtValue.Value <= 0)
            {
                MessageBox.Show("Due Value must be greater than zero");
                return;
            }

            manager.UpdateWalletTransfer(oneTransaction.Id,
                                         txtValue.Value,
                                         dtDate.Value,
                                         txtName.Text.Trim(), 
                                         rdoPaid.Checked);

            DialogResult = DialogResult.OK;

        }
        public static DialogResult ShowDialog(Transac oneTx)
        {
            using var frm = new dlgUpdateWalletTransfer();
            frm.oneTransaction = oneTx;
            return frm.ShowDialog();
        }

    }
}
