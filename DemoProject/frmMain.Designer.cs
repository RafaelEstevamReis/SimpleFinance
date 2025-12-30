using System.Drawing;
using System.Windows.Forms;

namespace DemoProject
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle8 = new DataGridViewCellStyle();
            groupBox1 = new GroupBox();
            grdWallets = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            btnAddWallet = new Button();
            groupBox2 = new GroupBox();
            grdCategories = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            btnAddCategory = new Button();
            groupBox3 = new GroupBox();
            grdTxRecent = new DataGridView();
            Column5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            Column9 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            btnAddTransaction = new Button();
            groupBox4 = new GroupBox();
            grdTxDue = new DataGridView();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            cntxEditDelete = new ContextMenuStrip(components);
            editToolStripMenuItem = new ToolStripMenuItem();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            btnTransactionBook = new Button();
            btnAdvSearch = new Button();
            cntxDueTx = new ContextMenuStrip(components);
            markAsPaidToolStripMenuItem = new ToolStripMenuItem();
            dueTxPayOnDueDateToolStripMenuItem = new ToolStripMenuItem();
            dueTxPayAsTodayToolStripMenuItem = new ToolStripMenuItem();
            dueTxOpenForEditToolStripMenuItem = new ToolStripMenuItem();
            dueTxReverseTransactionToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdWallets).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdCategories).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdTxRecent).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdTxDue).BeginInit();
            cntxEditDelete.SuspendLayout();
            cntxDueTx.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(grdWallets);
            groupBox1.Location = new Point(5, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(367, 231);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "My Wallets";
            // 
            // grdWallets
            // 
            grdWallets.AllowUserToAddRows = false;
            grdWallets.AllowUserToDeleteRows = false;
            grdWallets.AllowUserToResizeColumns = false;
            grdWallets.AllowUserToResizeRows = false;
            grdWallets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdWallets.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3 });
            grdWallets.Location = new Point(7, 22);
            grdWallets.Name = "grdWallets";
            grdWallets.ReadOnly = true;
            grdWallets.RowHeadersVisible = false;
            grdWallets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdWallets.Size = new Size(354, 203);
            grdWallets.TabIndex = 0;
            grdWallets.CellDoubleClick += grdWallets_CellDoubleClick;
            grdWallets.CellMouseClick += grdWallets_CellMouseClick;
            // 
            // Column1
            // 
            Column1.HeaderText = "Id";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 50;
            // 
            // Column2
            // 
            Column2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column2.HeaderText = "Name";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // Column3
            // 
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            Column3.DefaultCellStyle = dataGridViewCellStyle5;
            Column3.HeaderText = "Balance";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 75;
            // 
            // btnAddWallet
            // 
            btnAddWallet.Location = new Point(5, 498);
            btnAddWallet.Name = "btnAddWallet";
            btnAddWallet.Size = new Size(92, 31);
            btnAddWallet.TabIndex = 1;
            btnAddWallet.Text = "New Wallet";
            btnAddWallet.UseVisualStyleBackColor = true;
            btnAddWallet.Click += btnAddWallet_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(grdCategories);
            groupBox2.Location = new Point(378, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(561, 231);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "My Categories";
            // 
            // grdCategories
            // 
            grdCategories.AllowUserToAddRows = false;
            grdCategories.AllowUserToDeleteRows = false;
            grdCategories.AllowUserToResizeColumns = false;
            grdCategories.AllowUserToResizeRows = false;
            grdCategories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdCategories.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, Column4, dataGridViewTextBoxColumn2 });
            grdCategories.Location = new Point(6, 22);
            grdCategories.Name = "grdCategories";
            grdCategories.ReadOnly = true;
            grdCategories.RowHeadersVisible = false;
            grdCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdCategories.Size = new Size(549, 203);
            grdCategories.TabIndex = 1;
            grdCategories.CellDoubleClick += grdCategories_CellDoubleClick;
            grdCategories.CellMouseClick += grdCategories_CellMouseClick;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "Id";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 50;
            // 
            // Column4
            // 
            Column4.HeaderText = "Type";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 150;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn2.HeaderText = "Name";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // btnAddCategory
            // 
            btnAddCategory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddCategory.Location = new Point(103, 498);
            btnAddCategory.Name = "btnAddCategory";
            btnAddCategory.Size = new Size(92, 31);
            btnAddCategory.TabIndex = 2;
            btnAddCategory.Text = "New Category";
            btnAddCategory.UseVisualStyleBackColor = true;
            btnAddCategory.Click += btnAddCategory_Click;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox3.Controls.Add(grdTxRecent);
            groupBox3.Location = new Point(5, 237);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(645, 257);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "Recent Transactions";
            // 
            // grdTxRecent
            // 
            grdTxRecent.AllowUserToAddRows = false;
            grdTxRecent.AllowUserToDeleteRows = false;
            grdTxRecent.AllowUserToResizeColumns = false;
            grdTxRecent.AllowUserToResizeRows = false;
            grdTxRecent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdTxRecent.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdTxRecent.Columns.AddRange(new DataGridViewColumn[] { Column5, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, Column8, Column9, Column7 });
            grdTxRecent.Location = new Point(6, 22);
            grdTxRecent.Name = "grdTxRecent";
            grdTxRecent.ReadOnly = true;
            grdTxRecent.RowHeadersVisible = false;
            grdTxRecent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdTxRecent.Size = new Size(633, 229);
            grdTxRecent.TabIndex = 2;
            grdTxRecent.CellDoubleClick += grdTxRecent_CellDoubleClick;
            // 
            // Column5
            // 
            Column5.HeaderText = "Status";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 70;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle6.Format = "d";
            dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewTextBoxColumn4.HeaderText = "Eff. Date";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            dataGridViewTextBoxColumn4.Width = 80;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.HeaderText = "Catgory";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // Column8
            // 
            Column8.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column8.HeaderText = "Description";
            Column8.MinimumWidth = 250;
            Column8.Name = "Column8";
            Column8.ReadOnly = true;
            // 
            // Column9
            // 
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            Column9.DefaultCellStyle = dataGridViewCellStyle7;
            Column9.HeaderText = "Value";
            Column9.Name = "Column9";
            Column9.ReadOnly = true;
            Column9.Width = 75;
            // 
            // Column7
            // 
            Column7.HeaderText = "Wallet";
            Column7.Name = "Column7";
            Column7.ReadOnly = true;
            // 
            // btnAddTransaction
            // 
            btnAddTransaction.Location = new Point(201, 498);
            btnAddTransaction.Name = "btnAddTransaction";
            btnAddTransaction.Size = new Size(116, 31);
            btnAddTransaction.TabIndex = 3;
            btnAddTransaction.Text = "New Transaction";
            btnAddTransaction.UseVisualStyleBackColor = true;
            btnAddTransaction.Click += btnAddTransaction_Click;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox4.Controls.Add(grdTxDue);
            groupBox4.Location = new Point(656, 237);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(283, 257);
            groupBox4.TabIndex = 2;
            groupBox4.TabStop = false;
            groupBox4.Text = "Due Transactions";
            // 
            // grdTxDue
            // 
            grdTxDue.AllowUserToAddRows = false;
            grdTxDue.AllowUserToDeleteRows = false;
            grdTxDue.AllowUserToResizeColumns = false;
            grdTxDue.AllowUserToResizeRows = false;
            grdTxDue.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grdTxDue.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdTxDue.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn9 });
            grdTxDue.Location = new Point(6, 22);
            grdTxDue.Name = "grdTxDue";
            grdTxDue.ReadOnly = true;
            grdTxDue.RowHeadersVisible = false;
            grdTxDue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdTxDue.Size = new Size(271, 229);
            grdTxDue.TabIndex = 3;
            grdTxDue.CellDoubleClick += grdTxDue_CellDoubleClick;
            grdTxDue.CellMouseClick += grdTxDue_CellMouseClick;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle8.Format = "d";
            dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle8;
            dataGridViewTextBoxColumn6.HeaderText = "Due Date";
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            dataGridViewTextBoxColumn6.Width = 80;
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn9.HeaderText = "Description";
            dataGridViewTextBoxColumn9.MinimumWidth = 250;
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            dataGridViewTextBoxColumn9.ReadOnly = true;
            // 
            // cntxEditDelete
            // 
            cntxEditDelete.Items.AddRange(new ToolStripItem[] { editToolStripMenuItem, deleteToolStripMenuItem });
            cntxEditDelete.Name = "cntxEditDelete";
            cntxEditDelete.Size = new Size(145, 48);
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(144, 22);
            editToolStripMenuItem.Text = "Open for Edit";
            editToolStripMenuItem.Click += editToolStripMenuItem_Click;
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(144, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // btnTransactionBook
            // 
            btnTransactionBook.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnTransactionBook.Location = new Point(829, 498);
            btnTransactionBook.Name = "btnTransactionBook";
            btnTransactionBook.Size = new Size(110, 31);
            btnTransactionBook.TabIndex = 3;
            btnTransactionBook.Text = "Transaction Book";
            btnTransactionBook.UseVisualStyleBackColor = true;
            btnTransactionBook.Click += btnTransactionBook_Click;
            // 
            // btnAdvSearch
            // 
            btnAdvSearch.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnAdvSearch.Location = new Point(713, 498);
            btnAdvSearch.Name = "btnAdvSearch";
            btnAdvSearch.Size = new Size(110, 31);
            btnAdvSearch.TabIndex = 4;
            btnAdvSearch.Text = "Advanced Search";
            btnAdvSearch.UseVisualStyleBackColor = true;
            btnAdvSearch.Click += btnAdvSearch_Click;
            // 
            // cntxDueTx
            // 
            cntxDueTx.Items.AddRange(new ToolStripItem[] { markAsPaidToolStripMenuItem, dueTxOpenForEditToolStripMenuItem, dueTxReverseTransactionToolStripMenuItem });
            cntxDueTx.Name = "cntxDueTx";
            cntxDueTx.Size = new Size(181, 92);
            // 
            // markAsPaidToolStripMenuItem
            // 
            markAsPaidToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dueTxPayOnDueDateToolStripMenuItem, dueTxPayAsTodayToolStripMenuItem });
            markAsPaidToolStripMenuItem.Name = "markAsPaidToolStripMenuItem";
            markAsPaidToolStripMenuItem.Size = new Size(180, 22);
            markAsPaidToolStripMenuItem.Text = "Mark as Paid";
            // 
            // dueTxPayOnDueDateToolStripMenuItem
            // 
            dueTxPayOnDueDateToolStripMenuItem.Name = "dueTxPayOnDueDateToolStripMenuItem";
            dueTxPayOnDueDateToolStripMenuItem.Size = new Size(180, 22);
            dueTxPayOnDueDateToolStripMenuItem.Text = "On Due Date";
            dueTxPayOnDueDateToolStripMenuItem.Click += dueTxPayOnDueDateToolStripMenuItem_Click;
            // 
            // dueTxPayAsTodayToolStripMenuItem
            // 
            dueTxPayAsTodayToolStripMenuItem.Name = "dueTxPayAsTodayToolStripMenuItem";
            dueTxPayAsTodayToolStripMenuItem.Size = new Size(180, 22);
            dueTxPayAsTodayToolStripMenuItem.Text = "As Today";
            dueTxPayAsTodayToolStripMenuItem.Click += dueTxPayAsTodayToolStripMenuItem_Click;
            // 
            // dueTxOpenForEditToolStripMenuItem
            // 
            dueTxOpenForEditToolStripMenuItem.Name = "dueTxOpenForEditToolStripMenuItem";
            dueTxOpenForEditToolStripMenuItem.Size = new Size(180, 22);
            dueTxOpenForEditToolStripMenuItem.Text = "Open for Edit";
            dueTxOpenForEditToolStripMenuItem.Click += dueTxOpenForEditToolStripMenuItem_Click;
            // 
            // dueTxReverseTransactionToolStripMenuItem
            // 
            dueTxReverseTransactionToolStripMenuItem.Name = "dueTxReverseTransactionToolStripMenuItem";
            dueTxReverseTransactionToolStripMenuItem.Size = new Size(180, 22);
            dueTxReverseTransactionToolStripMenuItem.Text = "Reverse Transaction";
            dueTxReverseTransactionToolStripMenuItem.Click += DueTxReverseTransactionToolStripMenuItem_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(949, 538);
            Controls.Add(btnAdvSearch);
            Controls.Add(btnAddTransaction);
            Controls.Add(btnAddCategory);
            Controls.Add(btnAddWallet);
            Controls.Add(btnTransactionBook);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            MaximizeBox = false;
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Simple.Finance Demo";
            Load += frmMain_Load;
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdWallets).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdCategories).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdTxRecent).EndInit();
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grdTxDue).EndInit();
            cntxEditDelete.ResumeLayout(false);
            cntxDueTx.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private DataGridView grdWallets;
        private DataGridView grdCategories;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private ContextMenuStrip cntxEditDelete;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private Button btnAddWallet;
        private Button btnAddCategory;
        private Button btnAddTransaction;
        private DataGridView grdTxRecent;
        private DataGridView grdTxDue;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private Button btnTransactionBook;
        private Button btnAdvSearch;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn Column8;
        private DataGridViewTextBoxColumn Column9;
        private DataGridViewTextBoxColumn Column7;
        private ContextMenuStrip cntxDueTx;
        private ToolStripMenuItem markAsPaidToolStripMenuItem;
        private ToolStripMenuItem dueTxPayOnDueDateToolStripMenuItem;
        private ToolStripMenuItem dueTxPayAsTodayToolStripMenuItem;
        private ToolStripMenuItem dueTxOpenForEditToolStripMenuItem;
        private ToolStripMenuItem dueTxReverseTransactionToolStripMenuItem;
    }
}
