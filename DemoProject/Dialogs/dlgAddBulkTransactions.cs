using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgAddBulkTransactions : Form
    {
        private Category[] categories = [];
        private Wallet[] wallets;
        private Transac[] initTransactions = [];

        public dlgAddBulkTransactions()
        {
            InitializeComponent();
        }
        private void dlgAddBulkTransactions_Load(object sender, EventArgs e)
        {
            cboWallet.DataSource = wallets;
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
            List<Transac> lst = [];
            foreach (DataGridViewRow row in grdData.Rows)
            {
                DateTime date = (DateTime)row.Cells[clnDate.Index].Value;
                Types type = (Types)row.Cells[clnType.Index].Value;
                long category = (long)row.Cells[clnCategory.Index].Value;
                string description = row.Cells[clnDescription.Index].Value?.ToString() ?? "";
                decimal value = (decimal)row.Cells[clnDescription.Index].Value;
                bool paid = (bool)row.Cells[clnPaid.Index].Value;


            }
        }


        public static DialogResult ShowDialog(Manager manager, Transac[] trs)
        {
            using var frm = new dlgAddBulkTransactions();
            frm.categories = manager.GetCategories().ToArray();
            frm.wallets = manager.GetWallets().ToArray();
            frm.initTransactions = trs;
            return frm.ShowDialog();
        }

        public record Types
        {
            public bool IsExpense { get; set; }
            public string Name { get; set; }
        }
    }
}
