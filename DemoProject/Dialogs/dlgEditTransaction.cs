using DemoProject.Components;
using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Finance.Helpers;
using Simple.Finance.Tables;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgEditTransaction : DialogBase
    {
        private Manager manager;
        private Category[] categories;
        private Wallet[] wallets;
        private Transac transaction = null!;

        public dlgEditTransaction()
        {
            manager = Injector.Get<Manager>();
            categories = manager.GetCategories().ToArray();
            wallets = manager.GetWallets().ToArray();
            InitializeComponent();
        }

        private void dlgEditTransaction_Load(object sender, EventArgs e)
        {
            this.Height = 413;

            lblAdvanced.Visible = transaction.Id == 0;
            rdoReversed.Visible = transaction.Id > 0;

            lblId.Text = transaction.Id.ToString();
            lblCreated.Text = transaction.Created.ToLocalTime().ToString("d");
            lblChanged.Text = transaction.Changed.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
            dtDue.Value = transaction.DueDate;
            dtPaid.Value = transaction.PaymentDate;
            txtName.Text = transaction.Description;

            if (transaction.Status == Transac.PaymentStatus.Paid) rdoPaid.Checked = true;
            else if (transaction.Status == Transac.PaymentStatus.Unpaid) rdoUnpaid.Checked = true;
            else rdoReversed.Checked = true;

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

            cboRecuringPeriod.SelectedIndex = 1;

            txtPaymentDetails.Text = transaction.PaymentDetails ?? string.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool isNew = transaction.Id == 0;

            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Transaction Description must be longer than 1");
                return;
            }

            if (txtDue.Value <= 0)
            {
                MessageBox.Show("Due Value must be greater than zero");
                return;
            }

            if (cboWallet.SelectedValue == null)
            {
                MessageBox.Show("Wallet must be selected");
                return;
            }
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Category must be selected");
                return;
            }
            if (rdoRecuringYes.Checked)
            {
                if (cboRecuringPeriod.SelectedIndex < 0)
                {
                    MessageBox.Show("A recuring period must be selected");
                    return;
                }
            }
            if (isNew && rdoReversed.Checked)
            {
                MessageBox.Show("A transaction cannot be created as Reversed");
                return;
            }

            transaction.WalletId = (long)cboWallet.SelectedValue;
            transaction.CategoryId = (long)cboCategory.SelectedValue;

            if (rdoPaid.Checked) transaction.Status = Transac.PaymentStatus.Paid;
            else if (rdoReversed.Checked) transaction.Status = Transac.PaymentStatus.Reversed;
            else transaction.Status = Transac.PaymentStatus.Unpaid;
            updatePaidChanged();

            transaction.DueDate = dtDue.Value;
            transaction.PaymentDate = dtPaid.Value.StartOfMinute();

            decimal sign = rdoExpense.Checked ? -1 : 1;
            transaction.DueValue = sign * txtDue.Value;
            transaction.PaidValue = sign * txtPaid.Value;

            transaction.Description = txtName.Text.Trim();
            transaction.PaymentDetails = txtPaymentDetails.Text.Trim();
            if (string.IsNullOrWhiteSpace(transaction.PaymentDetails)) transaction.PaymentDetails = null;

            manager.CreateUpdateTransaction(transaction); // Save
            // Recuring?
            if (isNew && rdoRecuringYes.Checked)
            {
                // Handle description
                string oldDesc = transaction.Description;
                // Adjust original
                transaction.Description = oldDesc + $" (1/{udRecuringCopies.Value + 1})";
                manager.CreateUpdateTransaction(transaction); // Save

                var txForCopy = manager.GetTransactionById(transaction.Id);
                if (txForCopy == null)
                {
                    Debug.Fail("Should not be null");
                    txForCopy = transaction;
                    txForCopy.Id = 0;
                }

                for (int i = 0; i < udRecuringCopies.Value; i++)
                {
                    txForCopy.Id = 0; // New Tr
                    txForCopy.Description = oldDesc + $" ({i + 2}/{udRecuringCopies.Value + 1})";
                    // Advance date
                    if (cboRecuringPeriod.SelectedIndex == 0)
                    {
                        txForCopy.DueDate = txForCopy.DueDate.AddDays(7);
                        txForCopy.PaymentDate = txForCopy.PaymentDate.AddDays(7);
                    }
                    else
                    {
                        txForCopy.DueDate = txForCopy.DueDate.AddMonths(1);
                        txForCopy.PaymentDate = txForCopy.PaymentDate.AddMonths(1);
                    }

                    // Save
                    manager.CreateUpdateTransaction(txForCopy); // Save
                }
            }

            DialogResult = DialogResult.OK;
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
            if (rdoPaid.Checked && transaction.Status == Transac.PaymentStatus.Unpaid) // Changed TO PAID
            {
                txtPaid.Value = txtDue.Value;
                dtPaid.Value = DateTime.Now;
            }
        }

        private void updateCategoryType()
        {
            cboCategory.DataSource = categories.Where(o => o.IsExpense == rdoExpense.Checked).ToArray();
        }

        private void updatePaidChanged()
        {
            pnlPaid.Enabled = rdoPaid.Checked;
        }

        private void lblAdvanced_Click(object sender, EventArgs e)
        {
            Height = 500;
            lblAdvanced.Visible = false;
        }
        private void rdoRecuringYes_CheckedChanged(object sender, EventArgs e)
        {
            pnlRecuring.Enabled = rdoRecuringYes.Checked;
        }

        private void lblChanged_Click(object sender, EventArgs e)
        {
            if (transaction.Id == 0)
            {
                MessageBox.Show("A new transaction do not have any logs");
                return;
            }

            dlgTransactionHistory.ShowDialog(transaction.Id);
        }

        public static DialogResult ShowDialog(Transac transaction)
        {
            using var frm = new dlgEditTransaction();
            frm.transaction = transaction;
            return frm.ShowDialog();
        }

    }
}
