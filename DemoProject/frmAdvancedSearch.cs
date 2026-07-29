using Simple.BotUtils.DI;
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
        private readonly Manager manager;

        public frmAdvancedSearch()
        {
            manager = Injector.Get<Manager>();
            InitializeComponent();
        }
        private void frmAdvancedSearch_Load(object sender, EventArgs e)
        {
            cboReferenceType.SelectedIndex = 0;

            this.GetConfig(cboDate, 4);
            this.GetConfig(dtFrom, DateHelpers.StartOfMonth(DateTime.Now));
            this.GetConfig(dtTo, DateHelpers.EndOfMonth(DateTime.Now));

            this.GetConfig(chkHidePaids, false);
            this.GetConfig(chkHideUnpaids, false);
            this.GetConfig(chkHideReversed, false);
            this.GetConfig(chkIncludeUnpaidBalance, false);
            this.GetConfig(chkIncludeTransfersInTotals, false);

            clnValue.FormatColumn(manager);
            chkFilterReference_CheckedChanged(new(), EventArgs.Empty);
        }
        private void frmAdvancedSearch_Shown(object sender, EventArgs e)
        {
            search();
        }

        private void cboReferenceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboReferenceItem.DataSource = null;
            if (cboReferenceType.SelectedIndex == 0)
            {
                cboReferenceItem.DataSource = manager.GetWallets();
            }
            if (cboReferenceType.SelectedIndex == 1)
            {
                cboReferenceItem.DataSource = manager
                    .GetCategories()
                    .OrderBy(o => o.IsExpense)
                    .ThenBy(o => o.Name)
                    .ToList();
            }
            // Set after DataSource
            cboReferenceItem.ValueMember = "Id";
            cboReferenceItem.DisplayMember = "Name";
        }

        private void chkFilterReference_CheckedChanged(object sender, EventArgs e)
        {
            cboReferenceType.Enabled = cboReferenceItem.Enabled = chkFilterReference.Checked;
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            search();
        }
        private void search()
        {
            // save configs
            this.SaveConfig(cboDate, o => o.SelectedIndex);
            this.SaveConfig(dtFrom);
            this.SaveConfig(dtTo);

            this.SaveConfig(chkHidePaids);
            this.SaveConfig(chkHideUnpaids);
            this.SaveConfig(chkHideReversed);
            this.SaveConfig(chkIncludeUnpaidBalance);
            this.SaveConfig(chkIncludeTransfersInTotals);

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
            clnWallet.Visible = true;
            clnCategory.Visible = true;
            if (chkFilterReference.Checked)
            {
                Manager.SearchTransactionsByKind kind;
                if (cboReferenceType.SelectedIndex == 0)
                {
                    kind = Manager.SearchTransactionsByKind.Wallet;
                    clnWallet.Visible = false;
                }
                else if (cboReferenceType.SelectedIndex == 1)
                {
                    kind = Manager.SearchTransactionsByKind.Category;
                    clnCategory.Visible = false;
                }
                else
                {
                    kind = Manager.SearchTransactionsByKind.Category;
                }

                txs = manager.GetTransactionsBy(kind, (long?)cboReferenceItem.SelectedValue ?? 0, dateType, dtFrom.Value, dtTo.Value);
            }
            else
            {
                txs = manager.GetTransactions(dateType, dtFrom.Value, dtTo.Value);
            }

            if (chkHidePaids.Checked) txs = txs.Where(o => o.Status != Transac.PaymentStatus.Paid);
            if (chkHideUnpaids.Checked) txs = txs.Where(o => o.Status != Transac.PaymentStatus.Unpaid);
            if (chkHideReversed.Checked) txs = txs.Where(o => o.Status != Transac.PaymentStatus.Reversed);
            if (txtDescription.TextLength > 0) txs = txs.Where(o => o.Description.Contains(txtDescription.Text, StringComparison.InvariantCultureIgnoreCase));

            var dicWallets = manager.GetWalletsDict();
            var dicCategories = manager.GetCategoriesDict();

            txs = txs.OrderBy(o => o.EffectiveDate.Date)
                     .ThenByDescending(o => o.Status)
                     .ThenBy(o => o.Type)
                     .ThenByDescending(o => o.DueValue)
                     ;

            //decimal balance = 0;
            var balances = new Dictionary<string, decimal>();

            foreach (var tx in txs)
            {
                DateTime? paymentDate = null;
                string add = "";

                if (tx.Type == Transac.TransactionType.Simple)
                {
                    if (tx.Status == Transac.PaymentStatus.Paid) add = "[Pd] ";
                }
                else if (tx.Type == Transac.TransactionType.WalletTransfer)
                {
                    if (tx.DueValue > 0) add = "▶ ";
                    else add = "◀ ";
                }

                var currencyCode = tx.GetTransactionCurrencyCode(dicWallets) ?? "";
                if (!balances.ContainsKey(currencyCode)) balances[currencyCode] = 0;

                if (tx.Status == Transac.PaymentStatus.Paid)
                {
                    paymentDate = tx.PaymentDate;
                    balances[currencyCode] += tx.PaidValue;
                }
                else if (tx.Status == Transac.PaymentStatus.Unpaid && chkIncludeUnpaidBalance.Checked)
                {
                    balances[currencyCode] += tx.DueValue;
                }

                int ix = grdTransactions.Rows.Add(tx.GetWalletName(dicWallets),
                                                  tx.GetCategoryName(dicCategories),
                                                  add + tx.Description,
                                                  tx.DueDate,
                                                  tx.EffectiveValue,
                                                  paymentDate,
                                                  balances[currencyCode]);
                grdTransactions.Rows[ix].Tag = tx;

                if (tx.Status == Transac.PaymentStatus.Paid) grdTransactions.Rows[ix].DefaultCellStyle.BackColor = Color.PaleGreen;
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

                if (balances[currencyCode] < 0)
                {
                    grdTransactions[clnBalance.Index, ix].Style.ForeColor = Color.DarkRed;
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
            txs = txs.Where(o => o.Status != Transac.PaymentStatus.Reversed); // Remove reversed from stats
            if (!chkIncludeTransfersInTotals.Checked) txs = txs.Where(o => o.Type == Transac.TransactionType.Simple);

            txtTotalPaid.SumMoneyBoxFor(manager, txs.Where(o => o.Status == Transac.PaymentStatus.Paid), o => o.PaidValue);
            txtTotalUnpaid.SumMoneyBoxFor(manager, txs.Where(o => o.Status == Transac.PaymentStatus.Unpaid), o => o.DueValue);
            txtTotalIncome.SumMoneyBoxFor(manager, txs.Where(o => o.DueValue > 0), o => o.EffectiveValue);
            txtTotalExpenses.SumMoneyBoxFor(manager, txs.Where(o => o.DueValue < 0), o => -o.EffectiveValue);

            txtTotalNet.Value = txtTotalIncome.Value - txtTotalExpenses.Value;
        }

        private void grdTransactions_SelectionChanged(object sender, EventArgs e)
        {
            var trx = getSelectedTransactions().Where(o => o.Status != Transac.PaymentStatus.Reversed).ToArray();

            txtTotalSelected.SumMoneyBoxFor(manager, trx);
            lblTotalSelected.Text = $"Total Selected: ({trx.Length})";
        }
        private void grdTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var t = grdTransactions.Rows[e.RowIndex].Tag as Transac;
            editTransaction(t!);
        }
        private void grdTransactions_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex < 0) return;

            // If the row is not selected, unselect all and reselect this ne
            if (!grdTransactions.Rows[e.RowIndex].Selected)
            {
                grdTransactions.ClearSelection();
                grdTransactions.Rows[e.RowIndex].Selected = true;
            }

            cntxGrid.Show(Cursor.Position);
        }
        private Transac[] getSelectedTransactions()
        {
            var selectedCellsIndex = grdTransactions.SelectedCells
                .Cast<DataGridViewCell>()
                .Select(c => c.RowIndex)
                .Distinct();

            var trx = selectedCellsIndex.Select(ix => grdTransactions.Rows[ix].Tag)
                                        .OfType<Transac>();
            return trx.ToArray();
        }
        private void btnAddTransaction_Click(object sender, EventArgs e)
        {
            cntxNew.Show(Cursor.Position);
        }

        private void singleTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Transac()
            {
                Type = Transac.TransactionType.Simple,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                DueDate = DateTime.Now,
                PaymentDate = DateTime.Now,
            };

            if (chkFilterReference.Checked)
            {
                if (cboReferenceType.SelectedIndex == 0)
                {
                    t.WalletId = (long?)cboReferenceItem.SelectedValue ?? 0;
                }
                else if (cboReferenceType.SelectedIndex == 1)
                {
                    t.CategoryId = (long?)cboReferenceItem.SelectedValue ?? 0;
                }
            }

            editTransaction(t);
        }
        private void walletTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.dlgNewWalletTransfer.ShowDialog(manager);
            search();
        }
        private void bulkTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.dlgAddBulkTransactions.ShowDialog([]);
            search();
        }
        private void importOFXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "OFX Files |*.ofx";
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK) return;

            var trs = Simple.Finance.Importers.TransactionImporter.FromOFX(dlg.FileName, 0, 0);
            Dialogs.dlgAddBulkTransactions.ShowDialog(trs);
            search();
        }

        private void editTransaction(Transac t)
        {
            if (t.Type == Transac.TransactionType.Special)
            {
                MessageBox.Show("Special transactions cannot be edited here");
            }
            else if (t.Type == Transac.TransactionType.WalletTransfer)
            {
                Dialogs.dlgUpdateWalletTransfer.ShowDialog(t);
                search();
            }
            else if (t.Type == Transac.TransactionType.Simple)
            {
                Dialogs.dlgEditTransaction.ShowDialog(t);
                search();
            }
            else
            {
                MessageBox.Show("This transaction cannot be edited here");
            }
        }

        private void changeDueValueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = getSelectedTransactions();
            if (selected.Length == 0)
            {
                MessageBox.Show("No transactions selected");
                return;
            }
            if (selected.Any(o => o.Type != Transac.TransactionType.Simple))
            {
                MessageBox.Show("Only SIMPLE transactions can be cloned");
                return;
            }

            var result = Dialogs.dlgValueBox.ShowDialog("New Due Value", 2, 0, out decimal newValue);
            if (result != DialogResult.OK) return;

            foreach (var tx in selected)
            {
                tx.DueValue = newValue * Math.Sign(tx.DueValue);

                if (tx.Status == Transac.PaymentStatus.Paid)
                {
                    tx.PaidValue = tx.DueValue;
                }
            }

            manager.CreateUpdateBulkTransaction(selected);
            search();
        }
        private void changeDueDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = getSelectedTransactions();
            if (selected.Length == 0)
            {
                MessageBox.Show("No transactions selected");
                return;
            }
            if (selected.Any(o => o.Type != Transac.TransactionType.Simple))
            {
                MessageBox.Show("Only SIMPLE transactions can be cloned");
                return;
            }

            var result = Dialogs.dlgValueBox.ShowDialog("New Due Day", "New Due Day", 0, 0, o => o >= 1 && o <= 31, out decimal newValue);
            if (result != DialogResult.OK) return;

            foreach (var tx in selected)
            {
                var newDate = tx.DueDate.StartOfMonth().AddDays((int)newValue - 1);
                if (newDate <= tx.DueDate.EndOfMonth()) tx.DueDate = newDate;
                else tx.DueDate = tx.DueDate.EndOfMonth();
            }

            manager.CreateUpdateBulkTransaction(selected);
            search();
        }
        private void changeCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = getSelectedTransactions();
            if (selected.Length == 0)
            {
                MessageBox.Show("No transactions selected");
                return;
            }

            var sign = Math.Sign(selected[0].DueValue);
            if (selected.Any(k => Math.Sign(k.DueValue) != sign))
            {
                MessageBox.Show("All transactions must be either 'Expense' or 'Income'");
                return;
            }
            if (selected.Any(o => o.Type != Transac.TransactionType.Simple))
            {
                MessageBox.Show("Only SIMPLE transactions can be cloned");
                return;
            }

            var categories = manager.GetCategories()
                                    .Where(o => sign > 0 ? !o.IsExpense : o.IsExpense)
                                    .ToArray()
                                    ;
            var result = Dialogs.dlgComboBox.ShowDialog("New Category", categories, out long newValue);
            if (result != DialogResult.OK) return;

            foreach (var tx in selected)
            {
                tx.CategoryId = newValue;
            }

            manager.CreateUpdateBulkTransaction(selected);
            search();
        }

        private void reverseTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = getSelectedTransactions();
            if (selected.Length == 0)
            {
                MessageBox.Show("No transactions selected");
                return;
            }

            if (selected.Any(o => o.Type != Transac.TransactionType.Simple))
            {
                MessageBox.Show("Only SIMPLE transactions can be reversed");
                return;
            }

            var result = MessageBox.Show($"Reverse all {selected.Length} selected transactions?", "Reverse?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }

            foreach (var tx in selected)
            {
                tx.Status = Transac.PaymentStatus.Reversed;
            }

            manager.CreateUpdateBulkTransaction(selected);
            search();
        }

        private void markAsPaidAsOfTodayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            markAsPaid(asToday: true);
        }
        private void markAsPaidAsOfOriginalDueDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            markAsPaid(asToday: false);
        }
        void markAsPaid(bool asToday)
        {
            var selected = getSelectedTransactions();
            if (selected.Length == 0)
            {
                MessageBox.Show("No transactions selected");
                return;
            }

            if (selected.Any(o => o.Status == Transac.PaymentStatus.Paid))
            {
                MessageBox.Show("Paid transactions cannot be paid");
                return;
            }
            if (selected.Any(o => o.Status == Transac.PaymentStatus.Reversed))
            {
                MessageBox.Show("Reversed transactions cannot be paid");
                return;
            }
            if (selected.Any(o => o.Type != Transac.TransactionType.Simple))
            {
                MessageBox.Show("Only SIMPLE transactions can be paid");
                return;
            }

            foreach (var tx in selected)
            {
                tx.Status = Transac.PaymentStatus.Paid;
                if (asToday)
                {
                    tx.PaymentDate = DateTime.Now;
                    tx.PaidValue = tx.DueValue;
                }
                else
                {
                    tx.PaymentDate = tx.DueDate.Date;
                    tx.PaidValue = tx.DueValue;
                }
            }

            manager.CreateUpdateBulkTransaction(selected);
            search();
        }
        private void cloneTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = getSelectedTransactions();
            if (selected.Length == 0)
            {
                MessageBox.Show("No transactions selected");
                return;
            }
            if (selected.Length != 1)
            {
                MessageBox.Show("Select only one transaction to clone");
                return;
            }

            var tx = selected[0];

            if (tx.Type != Transac.TransactionType.Simple)
            {
                MessageBox.Show("Only SIMPLE transactions can be cloned");
                return;
            }

            tx.Id = 0; // new
            tx.Created = DateTime.UtcNow;
            tx.Changed = DateTime.UtcNow;
            tx.Description += " (clone)";

            editTransaction(tx);

            search();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            cntxReports.Show(Cursor.Position);
        }
        private void categoriesOverviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param["Title"] = "Categories Overview";
            //var items = new[] { new { Description = "Widget 6000", Price = 104.99m, Qty = 1 }, new { Description = "Gizmo MAX", Price = 1.41m, Qty = 25 } };
            var categories = manager.GetCategoriesDict();
            var allRecentTx = grdTransactions.Rows.Cast<DataGridViewRow>().Select(o => o.Tag as Transac).ToArray();
            var items = allRecentTx.Select(o => new Reports.CategoriesOverviewModel
            {
                CategoryName = o.GetCategoryName(categories),
                Description = o.Description,
                Date = o.EffectiveDate,
                Value = o.EffectiveValue,
            }).ToArray();

            if (items.Length == 0)
            {
                MessageBox.Show("There are not items to show");
                return;
            }

            Reports.ReportViewerForm.ShowReport("Reports/CategoriesOverview.rdlc", items, param);
        }

        public static void ShowForm()
        {
            var frm = new frmAdvancedSearch();
            frm.Show();
            frm.FormClosed += (s, e) => frm.Dispose();
        }
        public static void ShowForm(DateTime toDate)
        {
            var frm = new frmAdvancedSearch();
            frm.Show();
            frm.FormClosed += (s, e) => frm.Dispose();

            frm.cboDate.SelectedIndex = 4; // Effective
            frm.dtFrom.Value = toDate.StartOfDay();
            frm.dtTo.Value = toDate.EndOfDay();
            frm.btnSearch.PerformClick();
        }

    }
}
