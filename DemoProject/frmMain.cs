using Simple.Finance;
using Simple.Finance.Helpers;
using Simple.Finance.Tables;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DemoProject
{
    public partial class frmMain : Form
    {
        Manager manager;

        public frmMain()
        {
            InitializeComponent();
            manager = Program.Manager;
            manager.EventNotifier += Manager_EventNotifier;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            checkDefaultItems();

            updateMyWallets();
            updateMyCategories();
            updateMyTransactions();
            updateChart();
        }
        void checkDefaultItems()
        {
            /* Wallets */
            var wallets = manager.GetWallets().ToArray();
            if (wallets.Length == 0)
            {
                manager.CreateUpdateWallet(new Wallet()
                {
                    Id = 0,
                    Name = "Checking Account",
                    Description = "",
                    IsDeleted = false,
                });
                manager.CreateUpdateWallet(new Wallet()
                {
                    Id = 0,
                    Name = "Savings Account",
                    Description = "",
                    IsDeleted = false,
                });
            }

            /* Categories */
            var cats = manager.GetCategories().ToArray();
            if (cats.Length == 0)
            {
                manager.CreateUpdateCategory(new Category()
                {
                    Id = 0,
                    Name = "My House Expenses",
                    Description = "",
                    IsExpense = true,
                    IsDeleted = false,
                });
                manager.CreateUpdateCategory(new Category()
                {
                    Id = 0,
                    Name = "My Food Expenses",
                    Description = "",
                    IsExpense = true,
                    IsDeleted = false,
                });
                manager.CreateUpdateCategory(new Category()
                {
                    Id = 0,
                    Name = "Income",
                    Description = "",
                    IsExpense = false,
                    IsDeleted = false,
                });
            }
        }
        void Manager_EventNotifier(object? sender, ManagerNotificationEventArgs e)
        {
            // Trigger updates
            if (e.Item == ManagerNotificationEventArgs.EventNotificationItem.Wallet) updateMyWallets();
            if (e.Item == ManagerNotificationEventArgs.EventNotificationItem.Category) updateMyCategories();
            if (e.Item == ManagerNotificationEventArgs.EventNotificationItem.Transaction)
            {
                updateMyWallets();
                updateMyTransactions();
                updateChart();
            }
        }
        void updateMyWallets()
        {
            grdWallets.Rows.Clear();
            var wallets = manager.GetWallets();
            var balances = manager.GetWalletsBalance().ToDictionary(o => o.WalletId, o => o.Balance);

            foreach (var wallet in wallets)
            {
                if (wallet.IsDeleted) continue;

                if (!balances.TryGetValue(wallet.Id, out decimal balance)) balance = 0;
                var ix = grdWallets.Rows.Add(wallet.Id, wallet.Name, balance);
                grdWallets.Rows[ix].Tag = wallet;
            }
        }
        void updateMyCategories()
        {
            grdCategories.Rows.Clear();
            var categories = manager.GetCategories();

            foreach (var category in categories)
            {
                if (category.IsDeleted) continue;

                var ix = grdCategories.Rows.Add(category.Id, category.IsExpense ? "Expense" : "Income", category.Name);
                grdCategories.Rows[ix].Tag = category;
            }
        }
        void updateMyTransactions()
        {
            var wallets = manager.GetWalletsDict();
            var categories = manager.GetCategoriesDict();

            // Recent effective or changed
            grdTxRecent.Rows.Clear();

            var recentDuePaid = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddMinutes(7));
            var recentChanged = manager.GetTransactions(Manager.SearchTransactionsDate.Changed, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddMinutes(1));
            var txs = recentDuePaid.Union(recentChanged).DistinctBy(o => o.Id).OrderBy(o => o.EfectiveDate);
            foreach (var tx in txs)
            {
                string category = tx.GetCategoryName(categories);
                string wallet = tx.GetWalletName(wallets);

                int ix = grdTxRecent.Rows.Add(tx.Status, tx.EfectiveDate, category, tx.Description, tx.EfectiveValue, wallet);
                grdTxRecent.Rows[ix].Tag = tx;

                if (tx.Status == Transac.PaymentStatus.Reversed)
                {
                    grdTxRecent.Rows[ix].DefaultCellStyle.Font = new System.Drawing.Font(Font, System.Drawing.FontStyle.Strikeout);
                }
            }

            // Due
            grdTxDue.Rows.Clear();
            var dtDue = DateHelpers.MaxOf(DateTime.UtcNow.EndOfMonth(), DateTime.UtcNow.AddDays(7));
            var txsDue = manager.GetTransactions(Manager.SearchTransactionsDate.DueDate, DateTime.UtcNow.AddDays(-7), dtDue)
                                .OrderBy(o => o.DueDate);
            foreach (var tx in txsDue)
            {
                if (tx.Status == Transac.PaymentStatus.Reversed) continue;
                if (tx.Status == Transac.PaymentStatus.Paid) continue;
                int ix = grdTxDue.Rows.Add(tx.DueDate, tx.DueValue, tx.Description);
                grdTxDue.Rows[ix].Tag = tx;
            }
        }
        void updateChart()
        {
            int daysBefore = 10;
            int daysAfter = 90;

            var dateBefore = DateTime.UtcNow.AddDays(-daysBefore).EndOfDay();
            var balance = manager.GetWalletsBalance(dateBefore);

            // All transactions from that day to daysAfter
            var txs = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, dateBefore, DateTime.UtcNow.AddDays(daysAfter));
            chtAssets.Series.Clear();

            var wallets = manager.GetWallets();
            decimal max = 0;
            foreach (var wallet in wallets)
            {
                decimal[] valuesDay = new decimal[daysBefore + daysAfter];
                
                valuesDay[0] = balance.Where(o => o.WalletId == wallet.Id).Sum(o => o.Balance);

                foreach (var tx in txs)
                {
                    if (tx.WalletId != wallet.Id) continue;

                    var effDateIx = (int)(tx.EfectiveDate.Date - dateBefore.Date).TotalDays;
                    valuesDay[effDateIx] += tx.EfectiveValue;
                }

                DateTime[] arrDates = new DateTime[valuesDay.Length];
                decimal[] arrBalances = new decimal[valuesDay.Length];

                for (int i = 0; i < valuesDay.Length; i++)
                {
                    arrDates[i] = dateBefore.Date.AddDays(i);
                    if (i > 0) arrBalances[i] = arrBalances[i - 1];
                    arrBalances[i] += valuesDay[i];

                    if (max < arrBalances[i]) max = arrBalances[i];
                }

                var serie = new Series(wallet.Name);
                //serie.IsVisibleInLegend = false;
                serie.ChartType = SeriesChartType.Line;
                serie.BorderWidth = 2;
                for (int i = 0; i < valuesDay.Length; i++)
                {
                    serie.Points.AddXY(arrDates[i], arrBalances[i]);
                }
                chtAssets.Series.Add(serie);
            }

            var sToday = new Series("Today");
            sToday.IsVisibleInLegend = false;
            sToday.ChartType = SeriesChartType.RangeColumn;
            sToday.BorderWidth = 1;
            sToday["PixelPointWidth"] = "2";
            sToday.Color = Color.Black;
            sToday.Points.AddXY(DateTime.Now.Date, max);
            chtAssets.Series.Add(sToday);

            chtAssets.ChartAreas[0].AxisY.LabelStyle.Format = "N2";
            chtAssets.ChartAreas[0].AxisX.LabelStyle.Format = "dd-MMM";
            chtAssets.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chtAssets.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;
            chtAssets.ChartAreas[0].AxisX.IsMarginVisible = false;
        }

        private void grdWallets_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) => doGridClickEvent(sender as DataGridView, e);
        private void grdCategories_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) => doGridClickEvent(sender as DataGridView, e);
        private void grdWallets_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdCategories_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdTxRecent_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdTxDue_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdTxDue_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex < 0) return;

            grdTxDue.ClearSelection();
            grdTxDue.Rows[e.RowIndex].Selected = true;
            cntxDueTx.Tag = grdTxDue.Rows[e.RowIndex].Tag;

            cntxDueTx.Show(System.Windows.Forms.Cursor.Position);
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e) => editTarget(cntxEditDelete.Tag);
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) => deleteTarget(cntxEditDelete.Tag);

        private void dueTxOpenForEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editTarget(cntxDueTx.Tag);
        }
        private void DueTxReverseTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deleteTarget(cntxDueTx.Tag);
        }
        private void dueTxPayOnDueDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tx = cntxDueTx.Tag as Transac;
            if (tx == null) return;

            if (tx.Type != Transac.TransactionType.Simple)
            {
                MessageBox.Show("This type of transaction cannot be edited here");
                return;
            }

            tx.Status = Transac.PaymentStatus.Paid;
            tx.PaymentDate = tx.DueDate.Date;
            tx.PaidValue = tx.DueValue;

            manager.CreateUpdateTransaction(tx);
        }
        private void dueTxPayAsTodayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var tx = cntxDueTx.Tag as Transac;
            if (tx == null) return;

            if (tx.Type != Transac.TransactionType.Simple)
            {
                MessageBox.Show("This type of transaction cannot be edited here");
                return;
            }

            tx.Status = Transac.PaymentStatus.Paid;
            tx.PaymentDate = DateTime.Now;
            tx.PaidValue = tx.DueValue;

            manager.CreateUpdateTransaction(tx);
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            cntxNew.Show(System.Windows.Forms.Cursor.Position);
        }
        private void newWalletToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newWallet();
        }
        private void newCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newCategory();
        }
        private void singleTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newTransaction();
        }
        private void walletTransferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.dlgNewWalletTransfer.ShowDialog(manager);
        }
        private void bulkTransactionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialogs.dlgAddBulkTransactions.ShowDialog(manager, []);
        }
        private void importOFXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "OFX Files |*.ofx";
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK) return;

            var trs = Simple.Finance.Importers.TransactionImporter.FromOFX(dlg.FileName, 0, 0);
            Dialogs.dlgAddBulkTransactions.ShowDialog(manager, trs);
        }

        void doGridClickEvent(DataGridView? sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender == null) return;
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex < 0) return;

            sender.ClearSelection();
            sender.Rows[e.RowIndex].Selected = true;
            cntxEditDelete.Tag = sender.Rows[e.RowIndex].Tag;
            cntxEditDelete.Show(System.Windows.Forms.Cursor.Position);
        }
        void doGridDoubleClickEvent(DataGridView? sender, DataGridViewCellEventArgs e)
        {
            if (sender == null) return;
            if (e.RowIndex < 0) return;

            sender.ClearSelection();
            sender.Rows[e.RowIndex].Selected = true;
            var target = sender.Rows[e.RowIndex].Tag;

            editTarget(target);
        }
        private void editTarget(object? target)
        {
            if (target is Wallet w) editWallet(w);
            if (target is Category c) editCategory(c);
            if (target is Transac t) editTransaction(t);
        }
        private void deleteTarget(object? target)
        {
            if (target is Wallet w) deleteWallet(w);
            if (target is Category c) deleteCategory(c);
            if (target is Transac t)
            {
                if (t.Type != Transac.TransactionType.Simple)
                {
                    MessageBox.Show("This type of transaction cannot be reversed");
                    return;
                }
                t.Status = Transac.PaymentStatus.Reversed;
                manager.CreateUpdateTransaction(t);
            }
        }

        private void newWallet()
        {
            var w = new Wallet
            {
                Id = 0,
                Name = "New wallet",
            };
            var result = Dialogs.dlgEditWallet.ShowDialog(w);
            if (result == DialogResult.Cancel) return;

            manager.CreateUpdateWallet(w);
        }
        private void newCategory()
        {
            var c = new Category
            {
                Id = 0,
                Name = "New Category",
                IsExpense = true,
            };
            var result = Dialogs.dlgEditCategory.ShowDialog(c);
            if (result == DialogResult.Cancel) return;

            manager.CreateUpdateCategory(c);
        }
        private void newTransaction()
        {
            var t = new Transac
            {
                Id = 0,
                Created = DateTime.UtcNow,
                Changed = DateTime.UtcNow,
                DueDate = DateTime.UtcNow,
                PaymentDate = DateTime.UtcNow,
                Description = "New Transaction",
            };
            var result = Dialogs.dlgEditTransaction.ShowDialog(t, manager);
            if (result != DialogResult.OK) return;
        }

        private void editCategory(Category c)
        {
            var result = Dialogs.dlgEditCategory.ShowDialog(c);
            if (result != DialogResult.OK) return;

            manager.CreateUpdateCategory(c);
        }
        private void editWallet(Wallet w)
        {
            var result = Dialogs.dlgEditWallet.ShowDialog(w);
            if (result != DialogResult.OK) return;

            manager.CreateUpdateWallet(w);
        }
        private void editTransaction(Transac t)
        {
            if (t.Type == Transac.TransactionType.Special)
            {
                MessageBox.Show("Special transactions cannot be edited here");
            }
            else if (t.Type == Transac.TransactionType.WalletTransfer)
            {
                Dialogs.dlgUpdateWalletTransfer.ShowDialog(manager, t);
            }
            else if (t.Type == Transac.TransactionType.Simple)
            {
                Dialogs.dlgEditTransaction.ShowDialog(t, manager);
            }
            else
            {
                MessageBox.Show("This transaction cannot be edited here");
            }
        }

        private void deleteCategory(Category c)
        {
            var r = MessageBox.Show($"Delete Category: {c.Name}?", "Delete?", MessageBoxButtons.YesNo);
            if (r == DialogResult.No) return;

            c.IsDeleted = true;
            manager.CreateUpdateCategory(c);
        }
        private void deleteWallet(Wallet w)
        {
            if (grdWallets.RowCount <= 1)
            {
                MessageBox.Show("You cannot delete the last Wallet");
                return;
            }

            var r = MessageBox.Show($"Delete Wallet: {w.Name}?", "Delete?", MessageBoxButtons.YesNo);
            if (r == DialogResult.No) return;

            w.IsDeleted = true;
            manager.CreateUpdateWallet(w);
        }

        private void btnTransactionBook_Click(object sender, EventArgs e)
        {
            //frmTransactionBook.ShowDialog(manager);
            frmTransactionBook.Show(manager);
        }

        private void btnAdvSearch_Click(object sender, EventArgs e)
        {
            //frmAdvancedSearch.ShowDialog(manager);
            frmAdvancedSearch.Show(manager);
        }

    }
}
