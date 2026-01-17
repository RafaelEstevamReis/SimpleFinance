using DemoProject.Components;
using Simple.Finance;
using Simple.Finance.Tables;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgNewWalletTransfer : DialogBase
    {
        private Manager manager = null!;
        private Category[] categories = [];
        private Wallet[] wallets = [];

        public dlgNewWalletTransfer()
        {
            InitializeComponent();
        }

        private void dlgNewWalletTransfer_Load(object sender, EventArgs e)
        {
            dtDate.Value = DateTime.Now.Date;

            cboSourceWallet.DataSource = wallets.ToArray();
            cboSourceWallet.DisplayMember = "Name";
            cboSourceWallet.ValueMember = "Id";

            cboDestinationWallet.DataSource = wallets.ToArray();
            cboDestinationWallet.DisplayMember = "Name";
            cboDestinationWallet.ValueMember = "Id";

            cboSourceCategory.DataSource = categories.Where(o => o.IsExpense).ToArray();
            cboSourceCategory.DisplayMember = "Name";
            cboSourceCategory.ValueMember = "Id";

            cboDestinationCategory.DataSource = categories.Where(o => !o.IsExpense).ToArray();
            cboDestinationCategory.DisplayMember = "Name";
            cboDestinationCategory.ValueMember = "Id";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboSourceWallet.SelectedIndex < 0)
            {
                MessageBox.Show("Source wallet must be selected");
                return;
            }
            if (cboSourceCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Source category must be selected");
                return;
            }
            if (cboDestinationWallet.SelectedIndex < 0)
            {
                MessageBox.Show("Destination wallet must be selected");
                return;
            }
            if (cboDestinationCategory.SelectedIndex < 0)
            {
                MessageBox.Show("Destination category must be selected");
                return;
            }
            if ((long)cboSourceWallet.SelectedValue! == (long)cboDestinationWallet.SelectedValue!)
            {
                MessageBox.Show("Source and destination wallet must be different");
                return;
            }

            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Description must be longer than 1");
                return;
            }

            if (txtValue.Value <= 0)
            {
                MessageBox.Show("Due Value must be greater than zero");
                return;
            }

            long sourceWalletId = (long)cboSourceWallet.SelectedValue;
            long destinationWalletId = (long)cboDestinationWallet.SelectedValue;

            // Check Currency mismatch
            var sourceWallet = wallets.First(x => x.Id == sourceWalletId);
            var destinationWallet = wallets.First(x => x.Id == destinationWalletId);

            if(!string.IsNullOrEmpty(sourceWallet.BaseCurrency)
               && !string.IsNullOrEmpty(destinationWallet.BaseCurrency))
            {
                if(sourceWallet.BaseCurrency != destinationWallet.BaseCurrency)
                {
                    MessageBox.Show("Source and Destination wallets must have the same BaseCurrency");
                    return;
                }
            }

            manager.CreateWalletTransfer(sourceWalletId,
                                         (long)cboSourceCategory.SelectedValue!,
                                         destinationWalletId,
                                         (long)cboDestinationCategory.SelectedValue!,
                                         txtName.Text.Trim(),
                                         txtValue.Value,
                                         dtDate.Value,
                                         rdoPaid.Checked);

            DialogResult = DialogResult.OK;
        }

        public static DialogResult ShowDialog(Manager manager)
        {
            using var frm = new dlgNewWalletTransfer();
            frm.manager = manager;
            frm.categories = manager.GetCategories().Where(o => !o.IsDeleted).ToArray();
            frm.wallets = manager.GetWallets().Where(o => !o.IsDeleted).ToArray();
            return frm.ShowDialog();
        }
    }
}
