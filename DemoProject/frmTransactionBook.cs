using Simple.Finance;
using Simple.Finance.Helpers;
using Simple.Finance.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject
{
    public partial class frmTransactionBook : Form
    {
        private Manager manager;
        private Dictionary<long, string> categories;

        public frmTransactionBook()
        {
            InitializeComponent();
        }
        private void frmTransactionBook_Load(object sender, EventArgs e)
        {
            dtDate.Value = DateHelpers.StartOfMonth(DateTime.Now);
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
        void refresh()
        {
            var start = DateHelpers.StartOfMonth(dtDate.Value);
            var end = start.AddMonths(1).AddSeconds(-1);

            grdTransactions.Rows.Clear();
            var txs = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, start, end)
                             .OrderBy(o => o.EfectiveDate)
                             .ToArray();

            decimal balance = 0;
            foreach (var tx in txs)
            {
                balance += tx.EfectiveValue;
                int ix = grdTransactions.Rows.Add(tx.EfectiveDate, tx.GetCategoryName(categories), tx.Description, tx.EfectiveValue, balance);
                grdTransactions.Rows[ix].Tag = tx;
            }
        }

        public static DialogResult ShowDialog(Manager manager)
        {
            using var frm = new frmTransactionBook();
            frm.manager = manager;
            frm.categories = manager.GetCategories().ToDictionary(o => o.Id, o => o.Name);
            return frm.ShowDialog();
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
                return;
            }
            if (t.Type == Transac.TransactionType.WalletTransfer)
            {
                MessageBox.Show("Transfer transactions cannot be edited here");
                return;
            }

            var result = Dialogs.dlgEditTransaction.ShowDialog(t, manager);
            if (result != DialogResult.OK) return;

            manager.CreateUpdateTransaction(t);
            refresh();
        }
    }
}
