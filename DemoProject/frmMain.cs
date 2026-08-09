using DemoProject.Reports;
using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Finance.ExchangeRate;
using Simple.Finance.Helpers;
using Simple.Finance.Tables;
using Simple.Sqlite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace DemoProject
{
    public partial class frmMain : Form
    {
        private FormWindowState lastState = FormWindowState.Normal;
        private Manager manager;
        private KeyValueStorage config;
        private ExchangeRateConverter exchangeConverter;
        private Dictionary<long, Wallet> wallets = [];

        public frmMain()
        {
            InitializeComponent();
            manager = Injector.Get<Manager>();
            config = Injector.Get<KeyValueStorage>();
            exchangeConverter = Injector.Get<ExchangeRateConverter>();

            manager.EventNotifier += Manager_EventNotifier;
        }

        private async void frmMain_Load(object sender, EventArgs e)
        {
            await exchangeConverter.InitializeTables();

            clnRecentValue.FormatColumn(manager);
            clnDueTxValue.FormatColumn(manager);

            checkDefaultItems();

            updateMyWallets();
            updateMyCategories();
            updateMyTransactions();
            updateChart();

            bool isFullScreen = config.GetKey<bool>(this.Name, "isFullScreen", false);
            if (isFullScreen) this.WindowState = FormWindowState.Maximized;
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
            wallets = manager.GetWalletsDict();
            var balances = manager.GetWalletsBalance().ToDictionary(o => o.WalletId, o => o.Balance);

            foreach (var walletItem in wallets)
            {
                var wallet = walletItem.Value;
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

                if (category.IsExpense)
                {
                    grdCategories.Rows[ix].DefaultCellStyle.BackColor = Color.MistyRose;
                }
                else
                {
                    grdCategories.Rows[ix].DefaultCellStyle.BackColor = Color.LightGreen;
                }
            }
        }
        void updateMyTransactions()
        {
            wallets = manager.GetWalletsDict();
            var categories = manager.GetCategoriesDict();

            // Recent effective or changed
            grdTxRecent.Rows.Clear();

            var recentDuePaid = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(7));
            var recentChanged = manager.GetTransactions(Manager.SearchTransactionsDate.Changed, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddMinutes(1));
            var txs = recentDuePaid.Union(recentChanged).DistinctBy(o => o.Id).OrderBy(o => o.EffectiveDate);
            foreach (var tx in txs)
            {
                string category = tx.GetCategoryName(categories);
                string wallet = tx.GetWalletName(wallets);

                int ix = grdTxRecent.Rows.Add(tx.Status, tx.EffectiveDate, category, tx.Description, tx.EffectiveValue, wallet);
                grdTxRecent.Rows[ix].Tag = tx;

                if (tx.Status == Transac.PaymentStatus.Reversed)
                {
                    grdTxRecent.Rows[ix].DefaultCellStyle.Font = new Font(Font, FontStyle.Strikeout);
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

                string add = "";

                if (tx.Type == Transac.TransactionType.WalletTransfer)
                {
                    if (tx.DueValue > 0) continue;
                    add = "▶ ";
                }

                int ix = grdTxDue.Rows.Add(tx.DueDate, tx.DueValue, add + tx.Description);
                grdTxDue.Rows[ix].Tag = tx;
            }
        }
        void updateChart()
        {
            int daysBefore = 10;
            int daysAfter = config.GetKey<int>(this.Name, "daysAfter", 30);
            daysAfter = Math.Clamp(daysAfter, 10, 400);

            var dateBefore = DateTime.UtcNow.AddDays(-daysBefore).EndOfDay();
            var balance = manager.GetWalletsBalance(dateBefore);

            // All transactions from that day to daysAfter
            var txs = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, dateBefore, DateTime.UtcNow.AddDays(daysAfter));
            chtAssets.Series.Clear();

            var wallets = manager.GetWallets();
            decimal max = 0;
            foreach (var wallet in wallets)
            {
                bool showOnChart = getShowOnChartConfig(wallet.Id);
                if (!showOnChart) continue;

                decimal[] valuesDay = new decimal[daysBefore + daysAfter];
                valuesDay[0] = balance.Where(o => o.WalletId == wallet.Id).Sum(o => o.Balance);

                foreach (var tx in txs)
                {
                    if (tx.Status == Transac.PaymentStatus.Reversed) continue;
                    if (tx.WalletId != wallet.Id) continue;

                    var effDateIx = (int)(tx.EffectiveDate.Date - dateBefore.Date).TotalDays;
                    if (effDateIx >= valuesDay.Length) continue;

                    var referenceValueConverted = tx.EffectiveValue;
                    valuesDay[effDateIx] += tx.EffectiveValue;
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

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized) return;

            if (lastState == WindowState) return;
            lastState = WindowState;

            bool isFullScreen = WindowState == FormWindowState.Maximized;
            config.SetKey<bool>(this.Name, "isFullScreen", isFullScreen);
        }

        private void chtAssets_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            cntxChartPeriod.Show(System.Windows.Forms.Cursor.Position);
        }
        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item == null) return;
            int days = int.Parse(item.Text?.Replace("d", "") ?? "60");

            config.SetKey<int>(this.Name, "daysAfter", days);
            updateChart();
        }
        private void chtAssets_MouseMove(object sender, MouseEventArgs e)
        {
            var result = chtAssets.HitTest(e.Location.X, e.Location.Y);
            if (result.Series == null) return;
            if (result.PointIndex < 0) return;
            if (result.Series.Name == "Today") return;

            var point = result.Object as DataPoint;
            string name = "";
            if (point != null) name = point.AxisLabel;
            if (name == "" || result.Series.ChartType == SeriesChartType.StackedColumn) name = result.Series.Name;

            string xVal = "";
            if (result.Series.XValueType == ChartValueType.DateTime)
            {
                var dtxVal = result.Series.Points[result.PointIndex].XValue;
                var dt = DateTime.FromOADate(dtxVal);
                xVal = dt.ToShortDateString();
            }

            grpChartAssets.Text = $"Assets - {xVal} {point?.YValues[0]:#0.00} on {name} ";

        }
        private void chtAssets_MouseLeave(object sender, EventArgs e)
        {
            grpChartAssets.Text = "Assets";
        }
        private void chtAssets_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var result = chtAssets.HitTest(e.Location.X, e.Location.Y);
            if (result.Series == null) return;
            if (result.PointIndex < 0) return;

            if (result.Series.XValueType == ChartValueType.DateTime)
            {
                var dtxVal = result.Series.Points[result.PointIndex].XValue;
                var dt = DateTime.FromOADate(dtxVal);

                frmAdvancedSearch.ShowForm(dt);
            }
        }

        private void grdWallets_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView grid) return;
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex < 0) return;

            grid.ClearSelection();
            grid.Rows[e.RowIndex].Selected = true;
            cntxEditDeleteWallet.Tag = grid.Rows[e.RowIndex].Tag;
            cntxEditDeleteWallet.Show(System.Windows.Forms.Cursor.Position);
        }

        private void grdCategories_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sender is not DataGridView grid) return;
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex < 0) return;

            grid.ClearSelection();
            grid.Rows[e.RowIndex].Selected = true;
            cntxEditDeleteCategory.Tag = grid.Rows[e.RowIndex].Tag;
            cntxEditDeleteCategory.Show(System.Windows.Forms.Cursor.Position);
        }

        private void grdWallets_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdCategories_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdTxRecent_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => doGridDoubleClickEvent(sender as DataGridView, e);
        private void grdTxRecent_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                if (e.Value is not decimal dVal) return;

                var tx = grdTxRecent.Rows[e.RowIndex].Tag as Transac;
                if (tx == null) return;

                var code = tx.GetTransactionCurrencyCode(wallets);
                if (string.IsNullOrEmpty(code)) return;

                e.FormattingApplied = true;
                e.Value = CurrencyHelpers.FormatFor(dVal, code);
            }
        }

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

        private void cntxEditDeleteWallet_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cntxEditDeleteWallet.Tag is not Wallet w) return;
            showOnChartToolStripMenuItem.Checked = getShowOnChartConfig(w.Id);
        }
        private void editWalletToolStripMenuItem_Click(object sender, EventArgs e) => editTarget(cntxEditDeleteWallet.Tag);
        private void deleteWalletToolStripMenuItem_Click(object sender, EventArgs e) => deleteTarget(cntxEditDeleteWallet.Tag);
        private void showOnChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cntxEditDeleteWallet.Tag is not Wallet w) return;
            invertShowOnChartConfig(w.Id);

            updateChart();
        }

        private void editCategoryToolStripMenuItem_Click(object sender, EventArgs e) => editTarget(cntxEditDeleteCategory.Tag);
        private void deleteCatgoryToolStripMenuItem_Click(object sender, EventArgs e) => deleteTarget(cntxEditDeleteCategory.Tag);

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
            Dialogs.dlgAddBulkTransactions.ShowDialog([]);
        }
        private void importOFXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "OFX Files |*.ofx";
            var result = dlg.ShowDialog();
            if (result != DialogResult.OK) return;

            var trs = Simple.Finance.Importers.TransactionImporter.FromOFX(dlg.FileName, 0, 0, 0);
            Dialogs.dlgAddBulkTransactions.ShowDialog(trs);
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
            var result = Dialogs.dlgEditTransaction.ShowDialog(t);
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
                Dialogs.dlgUpdateWalletTransfer.ShowDialog(t);
            }
            else if (t.Type == Transac.TransactionType.Simple)
            {
                Dialogs.dlgEditTransaction.ShowDialog(t);
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

        private bool getShowOnChartConfig(long walletId)
        {
            return config.GetKey(this.Name, $"wallet_show_on_chart.{walletId}", true);
        }
        private void invertShowOnChartConfig(long walletId)
        {
            var state = getShowOnChartConfig(walletId);
            config.SetKey(this.Name, $"wallet_show_on_chart.{walletId}", !state);
        }

        private void btnTransactionBook_Click(object sender, EventArgs e)
        {
            frmTransactionBook.ShowForm();
        }
        private void btnAdvSearch_Click(object sender, EventArgs e)
        {
            frmAdvancedSearch.ShowForm();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            cntxReports.Show(System.Windows.Forms.Cursor.Position);
        }

        private void yearlySummaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> param = [];
            param["Title"] = "Yearly Summary";
            param["Format"] = "N2";

            var startOfYear = DateTime.Now.StartOfYear();
            for (int i = 0; i < 12; i++)
            {
                var currMonth = startOfYear.AddMonths(i);
                param[$"Month{i + 1:00}"] = $"{currMonth:MMM}/{currMonth:yyyy}";
            }

            var categories = manager.GetCategoriesDict();

            var txs = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, DateTime.Now.StartOfYear(), DateTime.Now.EndOfYear());

            Dictionary<string, decimal[]> dicCategories = [];
            foreach (var tx in txs)
            {
                if (tx.Status == Transac.PaymentStatus.Reversed) continue;
                if (tx.Type != Transac.TransactionType.Simple) continue;

                if (tx.Status == Transac.PaymentStatus.Unpaid)
                {
                    if (tx.DueDate.Date < DateTime.Today.Date) continue; // Já não pagou, não vai pagar
                }

                var catName = tx.GetCategoryName(categories);

                if (!dicCategories.ContainsKey(catName))
                {
                    dicCategories[catName] = new decimal[12];
                }

                dicCategories[catName][tx.EffectiveDate.Month - 1] += tx.EffectiveValue;
            }

            List<YearlySummaryModel> lst = [];
            foreach (var pair in dicCategories)
            {
                lst.Add(new YearlySummaryModel
                {
                    CategoryName = pair.Key,
                    Month01 = dicCategories[pair.Key][0],
                    Month02 = dicCategories[pair.Key][1],
                    Month03 = dicCategories[pair.Key][2],
                    Month04 = dicCategories[pair.Key][3],
                    Month05 = dicCategories[pair.Key][4],
                    Month06 = dicCategories[pair.Key][5],
                    Month07 = dicCategories[pair.Key][6],
                    Month08 = dicCategories[pair.Key][7],
                    Month09 = dicCategories[pair.Key][8],
                    Month10 = dicCategories[pair.Key][9],
                    Month11 = dicCategories[pair.Key][10],
                    Month12 = dicCategories[pair.Key][11],
                    RowTotal = dicCategories[pair.Key].Sum(),
                });
            }

            if (lst.Count == 0)
            {
                MessageBox.Show("There are not items to show");
                return;
            }

            ReportViewerForm.ShowReport("Reports/YearlySummary.rdlc", lst.ToArray(), param);
        }
    }
}
