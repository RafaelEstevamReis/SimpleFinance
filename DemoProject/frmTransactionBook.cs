using Simple.Finance;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            dtDate.Value = StartOfMonth(DateTime.Now);
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
            var start = StartOfMonth(dtDate.Value);
            var end = start.AddMonths(1).AddSeconds(-1);

            grdTransactions.Rows.Clear();
            var txs = manager.GetTransactions(Manager.SearchTransactionsDate.EffectiveDate, start, end)
                             .OrderBy(o => o.EfectiveDate)
                             .ToArray();

            decimal balance = 0;
            foreach (var tx in txs)
            {
                balance += tx.EfectiveValue;
                grdTransactions.Rows.Add(tx.EfectiveDate, tx.GetCategoryName(categories), tx.Description, tx.EfectiveValue, balance);
            }
        }
        private static DateTime StartOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1, 0, 0, 0, dt.Kind);
        }

        public static DialogResult ShowDialog(Manager manager)
        {
            using var frm = new frmTransactionBook();
            frm.manager = manager;
            frm.categories = manager.GetCategories().ToDictionary(o => o.Id, o => o.Name);
            return frm.ShowDialog();
        }


    }
}
