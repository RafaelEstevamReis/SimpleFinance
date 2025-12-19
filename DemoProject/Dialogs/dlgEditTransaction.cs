using Simple.Finance.Tables;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgEditTransaction : Form
    {
        private Transac transaction;
        private Category[] categories;
        private Wallet[] wallets;

        public dlgEditTransaction()
        {
            InitializeComponent();
        }

        private void dlgEditTransaction_Load(object sender, EventArgs e)
        {
            lblId.Text = transaction.Id.ToString();
            lblCreated.Text = transaction.Created.ToLocalTime().ToString("d");
            lblChanged.Text = transaction.Changed.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            dtDue.Value = transaction.DueDate;
            dtPaid.Value = transaction.PaymentDate;
            txtName.Text = transaction.Description;

            if (transaction.Status == Transac.PaymentStatus.Paid) rdoPaid.Checked = true;
            else rdoUnpaid.Checked = true;

            if (transaction.DueValue > 0) rdoIncome.Checked = true;
            else rdoExpense.Checked = true;

            txtDue.Value = Math.Abs(transaction.DueValue);
            txtPaid.Value = Math.Abs(transaction.PaidValue);

            cboCategory.DataSource = categories.Where(o => o.IsExpense == rdoExpense.Checked).ToArray();
            cboCategory.DisplayMember = "Name";
            cboCategory.ValueMember = "Id";
            cboCategory.SelectedValue = transaction.CategoryId;

            cboWallet.DataSource = wallets;
            cboWallet.DisplayMember = "Name";
            cboWallet.ValueMember = "Id";
            cboWallet.SelectedValue = transaction.WalletId;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Transaction Description bust be longer than 1");
                return;
            }

            if(txtDue.Value <= 0)
            {
                MessageBox.Show("Due Value bust be longer than 1");
                return;
            }

            if (cboWallet.SelectedValue == null)
            {
                MessageBox.Show("Wallet bust be selected");
                return;
            }
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Category bust be selected");
                return;
            }

            transaction.WalletId = (long)cboWallet.SelectedValue;
            transaction.CategoryId = (long)cboCategory.SelectedValue;

            transaction.Status = rdoPaid.Checked ? Transac.PaymentStatus.Paid : Transac.PaymentStatus.Unpaid;
            transaction.DueDate = dtDue.Value;
            transaction.PaymentDate = dtPaid.Value;

            decimal sign = rdoExpense.Checked ? -1 : 1;
            transaction.DueValue = sign * txtDue.Value;
            transaction.PaidValue = sign * txtPaid.Value;

            transaction.Description = txtName.Text;

            DialogResult = DialogResult.OK;
        }

        public static DialogResult ShowDialog(Transac transaction, Simple.Finance.Manager manager)
        {
            using var frm = new dlgEditTransaction();
            frm.transaction = transaction;
            frm.categories = manager.GetCategories().Where(o => !o.IsDeleted).ToArray();
            frm.wallets = manager.GetWallets().Where(o => !o.IsDeleted).ToArray();
            return frm.ShowDialog();
        }

        private void rdoExpense_CheckedChanged(object sender, EventArgs e)
        {
            updateCategoryType();
        }
        private void rdoIncome_CheckedChanged(object sender, EventArgs e)
        {
            updateCategoryType();
        }

        private void rdoUnpaid_CheckedChanged(object sender, EventArgs e)
        {
            updatePaidChanged();
        }
        private void rdoPaid_CheckedChanged(object sender, EventArgs e)
        {
            updatePaidChanged();
        }

        private void updateCategoryType()
        {
            cboCategory.DataSource = categories.Where(o => o.IsExpense == rdoExpense.Checked).ToArray();
        }

        private void updatePaidChanged()
        {
            pnlPaid.Enabled = rdoPaid.Checked;
        }

    }
}
