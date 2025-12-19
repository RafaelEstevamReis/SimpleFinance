using Simple.Finance.Tables;
using System;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    public partial class dlgEditCategory : Form
    {
        Category category;
        public dlgEditCategory()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Length < 1)
            {
                MessageBox.Show("Category name bust be longer than 1");
                return;
            }

            category.Name = txtName.Text;
            if (pnlType.Enabled) category.IsExpense = rdoExpense.Enabled;

            DialogResult = DialogResult.OK;
        }

        public static DialogResult ShowDialog(Category category)
        {
            using var frm = new dlgEditCategory();
            frm.category = category;

            if (category.IsExpense) frm.rdoExpense.Checked = true;
            else frm.rdoIncome.Checked = true;

            frm.lblId.Text = category.Id.ToString();
            frm.txtName.Text = category.Name;

            if (category.Id > 0) frm.pnlType.Enabled = false;

            return frm.ShowDialog();
        }

    }
}
