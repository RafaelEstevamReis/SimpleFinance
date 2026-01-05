using DemoProject.Components;
using Simple.BotUtils.DI;
using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgAddBulkTransactions : DialogBase
    {
        private Manager manager;
        private Category[] categories;
        private Transac[] initTransactions = [];

        public dlgAddBulkTransactions()
        {
            manager = Injector.Get<Manager>();
            categories = manager.GetCategories().ToArray();
            InitializeComponent();
        }
        private void dlgAddBulkTransactions_Load(object sender, EventArgs e)
        {
            categories = manager.GetCategories().ToArray();
            cboWallet.DataSource = manager.GetWallets().ToArray();
            cboWallet.ValueMember = "Id";
            cboWallet.DisplayMember = "Name";

            grdData.AutoGenerateColumns = false;
            List<Types> types = [
                new Types{ IsExpense = true, Name ="Expense" },
                new Types{ IsExpense = false, Name= "Income" },
            ];
            clnType.DataSource = types;
            clnType.ValueMember = "IsExpense";
            clnType.DisplayMember = "Name";

            clnCategory.DataSource = new Category[0];
            clnCategory.ValueMember = "Id";
            clnCategory.DisplayMember = "Name";

            foreach (var tx in initTransactions)
            {
                bool isExpense = tx.DueValue < 0;
                var ix = grdData.Rows.Add(tx.EfectiveDate, isExpense, null, tx.Description, Math.Abs(tx.EfectiveValue), tx.Status == Transac.PaymentStatus.Paid);
                updateCategoryCell(ix, isExpense);
                grdData.Rows[ix].Tag = tx;
            }
        }

        private void grdData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            if (e.RowIndex < 0) return;

            if (e.ColumnIndex == clnType.Index)
            {
                var t = grdData[e.ColumnIndex, e.RowIndex].Value;
                if (t is bool bExpense)
                {
                    updateCategoryCell(e.RowIndex, bExpense);
                }
                return;
            }
        }

        private void updateCategoryCell(int rowIndex, bool isExpense)
        {
            var cboCell = grdData[clnCategory.Index, rowIndex] as DataGridViewComboBoxCell;
            if (cboCell == null) return;

            var availableCategories = categories.Where(o => o.IsExpense == isExpense).ToArray();
            cboCell.DataSource = availableCategories;
            cboCell.ValueMember = "Id";
            cboCell.DisplayMember = "Name";
            cboCell.Value = availableCategories.FirstOrDefault()?.Id;
        }

        private void grdData_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == clnDate.Index)
            {
                if (DateTime.TryParse(e.FormattedValue?.ToString(), out DateTime dt))
                {
                    grdData[e.ColumnIndex, e.RowIndex].Value = dt.Date;
                }
                else
                {
                    grdData[e.ColumnIndex, e.RowIndex].Value = DateTime.Now.Date;
                }
            }
            if (e.ColumnIndex == clnValue.Index)
            {
                if (decimal.TryParse(e.FormattedValue?.ToString(), out decimal val))
                {
                    grdData[e.ColumnIndex, e.RowIndex].Value = Math.Round(Math.Abs(val), 2);
                }
                else
                {
                    grdData[e.ColumnIndex, e.RowIndex].Value = 0M;
                }
            }

        }

        private void grdData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            //if (e.ColumnIndex == 2)
            //{
            //    e.Cancel = true;
            //    return;
            //}
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboWallet.SelectedIndex < 0)
            {
                MessageBox.Show("Select a wallet");
                return;
            }
            var wallet = (long)cboWallet.SelectedValue!;

            List<Transac> lst = [];
            foreach (DataGridViewRow row in grdData.Rows)
            {
                // is empty?
                if (row.Cells[clnDate.Index].Value == null) continue;

                DateTime date = (DateTime)row.Cells[clnDate.Index].Value;
                bool isExpense = (bool)row.Cells[clnType.Index].Value;
                long category = (long)row.Cells[clnCategory.Index].Value;
                string description = row.Cells[clnDescription.Index].Value?.ToString() ?? "";
                decimal value = (decimal)row.Cells[clnValue.Index].Value;
                bool paid = (bool)row.Cells[clnPaid.Index].Value;

                if (description.Length < 1)
                {
                    MessageBox.Show("Description must be at least 1 character long");
                    return;
                }

                var tr = new Transac
                {
                    Id = 0,
                    Created = DateTime.UtcNow,
                    Description = description,
                    CategoryId = category,
                    WalletId = wallet,
                    Type = Transac.TransactionType.Simple,
                    Status = Transac.PaymentStatus.Unpaid,
                    DueDate = date.Date,
                    DueValue = value * (isExpense ? -1 : 1),
                    PaymentDate = DateTime.UtcNow.Date,
                    PaidValue = 0,
                };
                if (paid)
                {
                    tr.Status = Transac.PaymentStatus.Paid;
                    tr.PaidValue = tr.DueValue;
                    tr.PaymentDate = tr.DueDate;
                }
                lst.Add(tr);
            }

            if (lst.Count == 0)
            {
                MessageBox.Show("Must be at least one transaction");
                return;
            }

            var result = MessageBox.Show($"Confirm creation of {lst.Count} new transactions?", "Create transactions?", MessageBoxButtons.YesNo);
            if (result != DialogResult.Yes) return;

            manager.CreateUpdateBulkTransaction(lst);
            DialogResult = DialogResult.OK;
        }


        public static DialogResult ShowDialog(IEnumerable<Transac> trs)
        {
            using var frm = new dlgAddBulkTransactions();
            frm.initTransactions = trs.ToArray();
            return frm.ShowDialog();
        }

        public record Types
        {
            public bool IsExpense { get; set; }
            public string Name { get; set; } = string.Empty;
        }
    }
}
