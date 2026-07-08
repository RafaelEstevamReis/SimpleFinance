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
    public partial class frmTransactionBook : Form
    {
        private readonly Manager manager;
        private Dictionary<long, Category> categories;

        public frmTransactionBook()
        {
            manager = Injector.Get<Manager>();
            categories = manager.GetCategoriesDict();
            InitializeComponent();
        }
        private void frmTransactionBook_Load(object sender, EventArgs e)
        {
            dtDate.Value = DateTime.Now;
            var allWallets = new Wallet()
            {
                Id = 0,
                Name = "[All Wallets]"
            };

            cboWallet.DataSource = manager.GetWallets().Union([allWallets]).OrderBy(o => o.Id).ToList();
            cboWallet.ValueMember = "Id";
            cboWallet.DisplayMember = "Name";

            clnValue.FormatColumn(manager);
        }
        private void btnPrev_Click(object sender, EventArgs e)
        {
            dtDate.Value = dtDate.Value.AddMonths(-1);
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            dtDate.Value = dtDate.Value.AddMonths(1);
        }
        private void dtDate_ValueChanged(object sender, EventArgs e)
        {
            refresh();
        }
        private void cboWallet_SelectedValueChanged(object sender, EventArgs e)
        {
            refresh();
        }
        void refresh()
        {
            var start = DateHelpers.StartOfMonth(dtDate.Value);
            var end = start.AddMonths(1).AddSeconds(-1);

            grdTransactions.Rows.Clear();
            IEnumerable<Transac> txs = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, start, end);

            decimal balance = 0;
            clnNetAmount.HeaderText = "Net Amount";
            if (cboWallet.SelectedValue is long walletId && walletId > 0)
            {
                txs = txs.Where(o => o.WalletId == walletId);

                var walletBalance = manager.GetWalletsBalance(start).ToDictionary(o => o.WalletId, o => o.Balance);
                walletBalance.TryGetValue(walletId, out balance);
                clnNetAmount.HeaderText = "Balance";
            }

            txs = txs.OrderBy(o => o.EfectiveDate.Date)
                     .ThenByDescending(o => (int)o.Status)
                     .ThenByDescending(o => o.EfectiveValue)
                     .ToArray();

            foreach (var tx in txs)
            {
                if (tx.Status == Transac.PaymentStatus.Reversed) continue;
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

                balance += tx.EfectiveValue;
                int ix = grdTransactions.Rows.Add(tx.EfectiveDate, tx.GetCategoryName(categories), add + tx.Description, tx.EfectiveValue, balance);
                grdTransactions.Rows[ix].Tag = tx;

                if (tx.Status == Transac.PaymentStatus.Paid) grdTransactions.Rows[ix].DefaultCellStyle.BackColor = Color.PaleGreen;
                else if (tx.Status == Transac.PaymentStatus.Unpaid)
                {
                    if (tx.DueDate.Date < DateTime.Now.Date)
                    {
                        grdTransactions.Rows[ix].DefaultCellStyle.BackColor = Color.MistyRose;
                    }
                }
            }
        }

        private void grdTransactions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender == null) return;
            if (e.RowIndex < 0) return;

            grdTransactions.ClearSelection();
            grdTransactions.Rows[e.RowIndex].Selected = true;
            var target = grdTransactions.Rows[e.RowIndex].Tag as Transac;
            if (target == null) return;
            editTransaction(target);
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
                refresh();
            }
            else if (t.Type == Transac.TransactionType.Simple)
            {
                Dialogs.dlgEditTransaction.ShowDialog(t);
                refresh();
            }
            else
            {
                MessageBox.Show("This transaction cannot be edited here");
            }
        }

        private void btnNewTr_Click(object sender, EventArgs e)
        {
            cntxBtn.Show(Cursor.Position);
        }

        private void newTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new Transac
            {
                Id = 0,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                WalletId = (long?)cboWallet.SelectedValue ?? 0,
                DueDate = dtDate.Value.Date,
                PaymentDate = dtDate.Value.Date,
                Description = "New Transaction",
            };
            Dialogs.dlgEditTransaction.ShowDialog(t);
            refresh();
        }
        private void newWalletTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.dlgNewWalletTransfer.ShowDialog(manager);
            refresh();
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
            refresh();
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

            refresh();
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

        public static void ShowForm()
        {
            var frm = new frmTransactionBook();
            frm.Show();
            frm.FormClosed += (s, e) => frm.Dispose();
        }

    }
}
