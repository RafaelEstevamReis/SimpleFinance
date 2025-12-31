using DemoProject.Components;
using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgTransactionHistory : DialogBase
    {
        private TableLogRegistry[] logs = [];

        public dlgTransactionHistory()
        {
            InitializeComponent();
        }

        private void dlgTransactionHistory_Load(object sender, EventArgs e)
        {
            long lastId = -1;
            foreach (var ev in logs)
            {
                if (ev.LogId == lastId)
                {
                    grdDados.Rows.Add(null, null, ev.FieldName, ev.OldValue, ev.NewValue);
                }
                else
                {
                    grdDados.Rows.Add(ev.LogId, ev.Event, ev.FieldName, ev.OldValue, ev.NewValue);
                }
                lastId = ev.LogId;
            }
        }

        public static DialogResult ShowDialog(Manager manager, long tableId)
        {
            using var dlg = new dlgTransactionHistory();
            dlg.logs = manager.GetLogs<Transac>(tableId).ToArray();
            return dlg.ShowDialog();
        }
    }
}
