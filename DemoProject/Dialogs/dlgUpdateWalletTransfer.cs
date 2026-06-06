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
            var wallets = manager.GetWalletsDict();
            dtDueDate.Value = oneTransaction.DueDate;
            dtPaidDate.Value = oneTransaction.PaymentDate;
            txtDueValue.Value = Math.Abs(oneTransaction.DueValue);
            txtPaidValue.Value = Math.Abs(oneTransaction.PaidValue);
            txtName.Text = oneTransaction.Description;
            txtPaymentDetails.Text = oneTransaction.PaymentDetails;

            if (oneTransaction.Status == Transac.PaymentStatus.Paid) rdoPaid.Checked = true;
            else if (oneTransaction.Status == Transac.PaymentStatus.Unpaid) rdoUnpaid.Checked = true;
            else rdoReversed.Checked = true;

            var pair = manager.GetTransferPair(oneTransaction);
            lblSourceWallet.Text = pair.soruce.GetWalletName(wallets);
            lblDestinationWallet.Text = pair.destination.GetWalletName(wallets);
        }

        private void rdoUnpaid_CheckedChanged(object sender, EventArgs e)
        {
            updatePaidChanged();
        }
        private void rdoPaid_CheckedChanged(object sender, EventArgs e)
        {
            updatePaidChanged();
        }

        private void rdoReversed_CheckedChanged(object sender, EventArgs e)
        {
            updatePaidChanged();
        }

        private void updatePaidChanged()
        {
            pnlPaid.Enabled = rdoPaid.Checked;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Description must be longer than 1");
                return;
            }

            if (txtDueValue.Value <= 0)
            {
                MessageBox.Show("Due Value must be greater than zero");
                return;
            }

            var status = rdoPaid.Checked ? Transac.PaymentStatus.Paid
                                                    : rdoUnpaid.Checked ? Transac.PaymentStatus.Unpaid
                                                        : Transac.PaymentStatus.Reversed;

            var paymentDetails = txtPaymentDetails.TextLength > 0 ? txtPaymentDetails.Text : null;
            manager.UpdateWalletTransfer(oneTransaction.Id,
                                         txtDueValue.Value,
                                         txtPaidValue.Value,
                                         dtDueDate.Value,
                                         dtPaidDate.Value,
                                         txtName.Text.Trim(),
                                         paymentDetails,
                                         status);

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
