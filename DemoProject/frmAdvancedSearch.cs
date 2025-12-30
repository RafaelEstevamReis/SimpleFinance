using Simple.Finance;
using Simple.Finance.Helpers;
using Simple.Finance.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject
{
    public partial class frmAdvancedSearch : Form
    {
        private Manager manager;

        public frmAdvancedSearch()
        {
            InitializeComponent();
        }
        private void frmAdvancedSearch_Load(object sender, EventArgs e)
        {
            cboDate.SelectedIndex = 0;
            cboReferenceType.SelectedIndex = 0;
            dtFrom.Value = DateHelpers.StartOfMonth(DateTime.Now);
            dtTo.Value = DateHelpers.EndOfMonth(dtFrom.Value);
        }
        private void frmAdvancedSearch_Shown(object sender, EventArgs e)
        {
            search();
        }

        private void cboReferenceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboReferenceItem.DataSource = null;
            if (cboReferenceType.SelectedIndex == 0) cboReferenceItem.DataSource = manager.GetWallets();
            if (cboReferenceType.SelectedIndex == 1) cboReferenceItem.DataSource = manager.GetCategories();
            // Set after DataSource
            cboReferenceItem.ValueMember = "Id";
            cboReferenceItem.DisplayMember = "Name";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search();
        }
        private void search()
        {
            // Save grid positions
            var fsri = grdTransactions.FirstDisplayedScrollingRowIndex;
            var sri = -1;
            if (grdTransactions.SelectedCells.Count > 0) sri = grdTransactions.SelectedCells[0].RowIndex;

            grdTransactions.Rows.Clear();

            Manager.SearchTransactionsDate dateType = cboDate.SelectedIndex switch
            {
                0 => Manager.SearchTransactionsDate.DueDate,
                1 => Manager.SearchTransactionsDate.PaymentDate,
                2 => Manager.SearchTransactionsDate.Created,
                3 => Manager.SearchTransactionsDate.Changed,
                4 => Manager.SearchTransactionsDate.EffectiveDate,
                _ => Manager.SearchTransactionsDate.EffectiveDate,
            };
            IEnumerable<Transac> txs;
            if (chkFilterReference.Checked)
            {
                Manager.SearchTransactionsByKind kind = cboReferenceType.SelectedIndex switch
                {
                    0 => Manager.SearchTransactionsByKind.Wallet,
                    1 => Manager.SearchTransactionsByKind.Category,
                    _ => Manager.SearchTransactionsByKind.Category,
                };

                txs = manager.GetTransactionsBy(kind, (long?)cboReferenceItem.SelectedValue ?? 0, dateType, dtFrom.Value, dtTo.Value);
            }
            else
            {
                txs = manager.GetTransactions(dateType, dtFrom.Value, dtTo.Value);
            }

            if (chkHidePaids.Checked) txs = txs.Where(o => o.Status != Transac.PaymentStatus.Paid);
            if (chkHideUnpaids.Checked) txs = txs.Where(o => o.Status != Transac.PaymentStatus.Unpaid);
            if (chkHideReversed.Checked) txs = txs.Where(o => o.Status != Transac.PaymentStatus.Reversed);

            var dicCategories = manager.GetCategoriesDict();

            txs = txs.OrderBy(o => o.EfectiveValue);

            foreach (var tx in txs)
            {
                DateTime? paymentDate = null;
                decimal value = tx.DueValue;
                string add = "";

                if (tx.Status == Transac.PaymentStatus.Paid)
                {
                    paymentDate = tx.PaymentDate;
                    value = tx.PaidValue;
                    add = "[Pd] ";
                }

                int ix = grdTransactions.Rows.Add(tx.GetCategoryName(dicCategories), add + tx.Description, tx.DueDate, value, paymentDate);
                grdTransactions.Rows[ix].Tag = tx;

                if (tx.Status == Transac.PaymentStatus.Paid) grdTransactions.Rows[ix].DefaultCellStyle.BackColor = Color.LightGreen;
                else if (tx.Status == Transac.PaymentStatus.Unpaid)
                {
                    if (tx.DueDate.Date < DateTime.Now.Date)
                    {
                        grdTransactions.Rows[ix].DefaultCellStyle.BackColor = Color.MistyRose;
                    }
                }
                else if (tx.Status == Transac.PaymentStatus.Reversed)
                {
                    grdTransactions.Rows[ix].DefaultCellStyle.Font = new Font(this.Font, FontStyle.Strikeout);
                }
            }
            // Restore grid positions
            if (fsri > 0)
            {
                if (grdTransactions.Rows.Count > fsri) grdTransactions.FirstDisplayedScrollingRowIndex = fsri;
            }
            if (sri >= 0)
            {
                grdTransactions.ClearSelection();
                if (grdTransactions.Rows.Count > sri) grdTransactions.Rows[sri].Selected = true;
            }
            // Totals
            txtTotalPaid.Value = txs.Where(o => o.Status == Transac.PaymentStatus.Paid).Sum(o => o.PaidValue);
            txtTotalUnpaid.Value = txs.Where(o => o.Status == Transac.PaymentStatus.Unpaid).Sum(o => o.DueValue);
            txtTotalIncome.Value = txs.Where(o => o.DueValue > 0).Sum(o => o.EfectiveValue);
            txtTotalExpenses.Value = txs.Where(o => o.DueValue < 0).Sum(o => o.EfectiveValue);
        }

        private void grdTransactions_SelectionChanged(object sender, EventArgs e)
        {
            var selectedCellsIndex = grdTransactions.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(c => c.RowIndex)
                .Distinct();

            var trx = selectedCellsIndex.Select(ix => grdTransactions.Rows[ix].Tag as Transac)
                                        .Where(o => o != null);
            txtTotalSelected.Value = trx.Sum(o => o!.EfectiveValue);
        }
        private void grdTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var t = grdTransactions.Rows[e.RowIndex].Tag as Transac;
            editTransaction(t);
        }

        private void btnAddTransaction_Click(object sender, EventArgs e)
        {
            var t = new Transac()
            {
                Type = Transac.TransactionType.Simple,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                DueDate = DateTime.UtcNow,
                PaymentDate = DateTime.UtcNow,
            };

            editTransaction(t);
        }

        private void editTransaction(Transac t)
        {
            if (t.Type == Transac.TransactionType.Special)
            {
                MessageBox.Show("Special transactions cannot be edited here");
                return;
            }
            if (t.Type == Transac.TransactionType.WalletTransfer)
            {
                MessageBox.Show("Transfer transactions cannot be edited here");
                return;
            }

            var result = Dialogs.dlgEditTransaction.ShowDialog(t, manager);
            if (result != DialogResult.OK) return;

            //manager.CreateUpdateTransaction(t);
            search();
        }

        public static DialogResult ShowDialog(Manager manager)
        {
            using var frm = new frmAdvancedSearch();
            frm.manager = manager;
            return frm.ShowDialog();
        }

    }
}
