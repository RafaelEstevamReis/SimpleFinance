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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle7 = new DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            groupBox1 = new GroupBox();
            grdWallets = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            groupBox2 = new GroupBox();
            grdCategories = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            groupBox3 = new GroupBox();
            grdTxRecent = new DataGridView();
            Column5 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            Column8 = new DataGridViewTextBoxColumn();
            Column9 = new DataGridViewTextBoxColumn();
            Column7 = new DataGridViewTextBoxColumn();
            groupBox4 = new GroupBox();
            grdTxDue = new DataGridView();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
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
            btnAddNew = new Button();
            cntxNew = new ContextMenuStrip(components);
            newWalletToolStripMenuItem = new ToolStripMenuItem();
            newCategoryToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripSeparator();
            singleTransactionToolStripMenuItem = new ToolStripMenuItem();
            walletTransferToolStripMenuItem = new ToolStripMenuItem();
            bulkTransactionsToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripSeparator();
            importOFXToolStripMenuItem = new ToolStripMenuItem();
            grpChartAssets = new GroupBox();
            chtAssets = new System.Windows.Forms.DataVisualization.Charting.Chart();
            cntxChartPeriod = new ContextMenuStrip(components);
            dToolStripMenuItem = new ToolStripMenuItem();
            dToolStripMenuItem1 = new ToolStripMenuItem();
            dToolStripMenuItem2 = new ToolStripMenuItem();
            dToolStripMenuItem3 = new ToolStripMenuItem();
            dToolStripMenuItem4 = new ToolStripMenuItem();
            dToolStripMenuItem5 = new ToolStripMenuItem();
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
            cntxNew.SuspendLayout();
            grpChartAssets.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chtAssets).BeginInit();
            cntxChartPeriod.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox1.Controls.Add(grdWallets);
            groupBox1.Location = new Point(656, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(318, 125);
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
            grdWallets.Dock = DockStyle.Fill;
            grdWallets.Location = new Point(3, 19);
            grdWallets.Name = "grdWallets";
            grdWallets.ReadOnly = true;
            grdWallets.RowHeadersVisible = false;
            grdWallets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdWallets.Size = new Size(312, 103);
            grdWallets.TabIndex = 0;
            grdWallets.CellDoubleClick += grdWallets_CellDoubleClick;
            grdWallets.CellMouseClick += grdWallets_CellMouseClick;
            // 
            // Column1
            // 
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleRight;
            Column1.DefaultCellStyle = dataGridViewCellStyle1;
            Column1.HeaderText = "Id";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 30;
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
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            Column3.DefaultCellStyle = dataGridViewCellStyle2;
            Column3.HeaderText = "Balance";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 75;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            groupBox2.Controls.Add(grdCategories);
            groupBox2.Location = new Point(656, 134);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(318, 159);
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
            grdCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdCategories.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, Column4, dataGridViewTextBoxColumn2 });
            grdCategories.Dock = DockStyle.Fill;
            grdCategories.Location = new Point(3, 19);
            grdCategories.Name = "grdCategories";
            grdCategories.ReadOnly = true;
            grdCategories.RowHeadersVisible = false;
            grdCategories.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdCategories.Size = new Size(312, 137);
            grdCategories.TabIndex = 1;
            grdCategories.CellDoubleClick += grdCategories_CellDoubleClick;
            grdCategories.CellMouseClick += grdCategories_CellMouseClick;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle3;
            dataGridViewTextBoxColumn1.HeaderText = "Id";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Width = 30;
            // 
            // Column4
            // 
            Column4.HeaderText = "Type";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 70;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewTextBoxColumn2.HeaderText = "Name";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(grdTxRecent);
            groupBox3.Location = new Point(5, 328);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(645, 240);
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
            grdTxRecent.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdTxRecent.Columns.AddRange(new DataGridViewColumn[] { Column5, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, Column8, Column9, Column7 });
            grdTxRecent.Dock = DockStyle.Fill;
            grdTxRecent.Location = new Point(3, 19);
            grdTxRecent.Name = "grdTxRecent";
            grdTxRecent.ReadOnly = true;
            grdTxRecent.RowHeadersVisible = false;
            grdTxRecent.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdTxRecent.Size = new Size(639, 218);
            grdTxRecent.TabIndex = 2;
            grdTxRecent.CellDoubleClick += grdTxRecent_CellDoubleClick;
            // 
            // Column5
            // 
            Column5.HeaderText = "Status";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 60;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle4.Format = "d";
            dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle4;
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
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            Column9.DefaultCellStyle = dataGridViewCellStyle5;
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
            // groupBox4
            // 
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            groupBox4.Controls.Add(grdTxDue);
            groupBox4.Location = new Point(656, 296);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(318, 272);
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
            grdTxDue.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdTxDue.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn6, Column6, dataGridViewTextBoxColumn9 });
            grdTxDue.Dock = DockStyle.Fill;
            grdTxDue.Location = new Point(3, 19);
            grdTxDue.Name = "grdTxDue";
            grdTxDue.ReadOnly = true;
            grdTxDue.RowHeadersVisible = false;
            grdTxDue.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grdTxDue.Size = new Size(312, 250);
            grdTxDue.TabIndex = 3;
            grdTxDue.CellDoubleClick += grdTxDue_CellDoubleClick;
            grdTxDue.CellMouseClick += grdTxDue_CellMouseClick;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewCellStyle6.Format = "dd/MM";
            dataGridViewTextBoxColumn6.DefaultCellStyle = dataGridViewCellStyle6;
            dataGridViewTextBoxColumn6.HeaderText = "Due";
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            dataGridViewTextBoxColumn6.Width = 50;
            // 
            // Column6
            // 
            dataGridViewCellStyle7.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "N2";
            Column6.DefaultCellStyle = dataGridViewCellStyle7;
            Column6.HeaderText = "Value";
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            Column6.Width = 80;
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
            btnTransactionBook.Location = new Point(864, 571);
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
            btnAdvSearch.Location = new Point(748, 571);
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
            cntxDueTx.Size = new Size(178, 70);
            // 
            // markAsPaidToolStripMenuItem
            // 
            markAsPaidToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { dueTxPayOnDueDateToolStripMenuItem, dueTxPayAsTodayToolStripMenuItem });
            markAsPaidToolStripMenuItem.Name = "markAsPaidToolStripMenuItem";
            markAsPaidToolStripMenuItem.Size = new Size(177, 22);
            markAsPaidToolStripMenuItem.Text = "Mark as Paid";
            // 
            // dueTxPayOnDueDateToolStripMenuItem
            // 
            dueTxPayOnDueDateToolStripMenuItem.Name = "dueTxPayOnDueDateToolStripMenuItem";
            dueTxPayOnDueDateToolStripMenuItem.Size = new Size(141, 22);
            dueTxPayOnDueDateToolStripMenuItem.Text = "On Due Date";
            dueTxPayOnDueDateToolStripMenuItem.Click += dueTxPayOnDueDateToolStripMenuItem_Click;
            // 
            // dueTxPayAsTodayToolStripMenuItem
            // 
            dueTxPayAsTodayToolStripMenuItem.Name = "dueTxPayAsTodayToolStripMenuItem";
            dueTxPayAsTodayToolStripMenuItem.Size = new Size(141, 22);
            dueTxPayAsTodayToolStripMenuItem.Text = "As Today";
            dueTxPayAsTodayToolStripMenuItem.Click += dueTxPayAsTodayToolStripMenuItem_Click;
            // 
            // dueTxOpenForEditToolStripMenuItem
            // 
            dueTxOpenForEditToolStripMenuItem.Name = "dueTxOpenForEditToolStripMenuItem";
            dueTxOpenForEditToolStripMenuItem.Size = new Size(177, 22);
            dueTxOpenForEditToolStripMenuItem.Text = "Open for Edit";
            dueTxOpenForEditToolStripMenuItem.Click += dueTxOpenForEditToolStripMenuItem_Click;
            // 
            // dueTxReverseTransactionToolStripMenuItem
            // 
            dueTxReverseTransactionToolStripMenuItem.Name = "dueTxReverseTransactionToolStripMenuItem";
            dueTxReverseTransactionToolStripMenuItem.Size = new Size(177, 22);
            dueTxReverseTransactionToolStripMenuItem.Text = "Reverse Transaction";
            dueTxReverseTransactionToolStripMenuItem.Click += DueTxReverseTransactionToolStripMenuItem_Click;
            // 
            // btnAddNew
            // 
            btnAddNew.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAddNew.Location = new Point(5, 571);
            btnAddNew.Name = "btnAddNew";
            btnAddNew.Size = new Size(92, 31);
            btnAddNew.TabIndex = 7;
            btnAddNew.Text = "Add New ▼";
            btnAddNew.UseVisualStyleBackColor = true;
            btnAddNew.Click += btnAddNew_Click;
            // 
            // cntxNew
            // 
            cntxNew.Items.AddRange(new ToolStripItem[] { newWalletToolStripMenuItem, newCategoryToolStripMenuItem, toolStripMenuItem1, singleTransactionToolStripMenuItem, walletTransferToolStripMenuItem, bulkTransactionsToolStripMenuItem, toolStripMenuItem2, importOFXToolStripMenuItem });
            cntxNew.Name = "btnNew";
            cntxNew.Size = new Size(197, 148);
            // 
            // newWalletToolStripMenuItem
            // 
            newWalletToolStripMenuItem.Name = "newWalletToolStripMenuItem";
            newWalletToolStripMenuItem.Size = new Size(196, 22);
            newWalletToolStripMenuItem.Text = "New Wallet";
            newWalletToolStripMenuItem.Click += newWalletToolStripMenuItem_Click;
            // 
            // newCategoryToolStripMenuItem
            // 
            newCategoryToolStripMenuItem.Name = "newCategoryToolStripMenuItem";
            newCategoryToolStripMenuItem.Size = new Size(196, 22);
            newCategoryToolStripMenuItem.Text = "New Category";
            newCategoryToolStripMenuItem.Click += newCategoryToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(193, 6);
            // 
            // singleTransactionToolStripMenuItem
            // 
            singleTransactionToolStripMenuItem.Name = "singleTransactionToolStripMenuItem";
            singleTransactionToolStripMenuItem.Size = new Size(196, 22);
            singleTransactionToolStripMenuItem.Text = "New Single Transaction";
            singleTransactionToolStripMenuItem.Click += singleTransactionToolStripMenuItem_Click;
            // 
            // walletTransferToolStripMenuItem
            // 
            walletTransferToolStripMenuItem.Name = "walletTransferToolStripMenuItem";
            walletTransferToolStripMenuItem.Size = new Size(196, 22);
            walletTransferToolStripMenuItem.Text = "New Wallet Transfer";
            walletTransferToolStripMenuItem.Click += walletTransferToolStripMenuItem_Click;
            // 
            // bulkTransactionsToolStripMenuItem
            // 
            bulkTransactionsToolStripMenuItem.Name = "bulkTransactionsToolStripMenuItem";
            bulkTransactionsToolStripMenuItem.Size = new Size(196, 22);
            bulkTransactionsToolStripMenuItem.Text = "New Bulk Transactions";
            bulkTransactionsToolStripMenuItem.Click += bulkTransactionsToolStripMenuItem_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(193, 6);
            // 
            // importOFXToolStripMenuItem
            // 
            importOFXToolStripMenuItem.Name = "importOFXToolStripMenuItem";
            importOFXToolStripMenuItem.Size = new Size(196, 22);
            importOFXToolStripMenuItem.Text = "Import OFX";
            importOFXToolStripMenuItem.Click += importOFXToolStripMenuItem_Click;
            // 
            // grpChartAssets
            // 
            grpChartAssets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            grpChartAssets.Controls.Add(chtAssets);
            grpChartAssets.Location = new Point(5, 6);
            grpChartAssets.Name = "grpChartAssets";
            grpChartAssets.Size = new Size(645, 319);
            grpChartAssets.TabIndex = 8;
            grpChartAssets.TabStop = false;
            grpChartAssets.Text = "Assets";
            // 
            // chtAssets
            // 
            chtAssets.BackColor = SystemColors.Control;
            chartArea1.AxisX2.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.Name = "ChartArea1";
            chtAssets.ChartAreas.Add(chartArea1);
            chtAssets.Dock = DockStyle.Fill;
            legend1.BackColor = SystemColors.Control;
            legend1.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend1.Name = "Legend1";
            chtAssets.Legends.Add(legend1);
            chtAssets.Location = new Point(3, 19);
            chtAssets.Name = "chtAssets";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chtAssets.Series.Add(series1);
            chtAssets.Size = new Size(639, 297);
            chtAssets.TabIndex = 0;
            chtAssets.Text = "chart1";
            chtAssets.MouseClick += chtAssets_MouseClick;
            chtAssets.MouseDoubleClick += chtAssets_MouseDoubleClick;
            chtAssets.MouseLeave += chtAssets_MouseLeave;
            chtAssets.MouseMove += chtAssets_MouseMove;
            // 
            // cntxChartPeriod
            // 
            cntxChartPeriod.Items.AddRange(new ToolStripItem[] { dToolStripMenuItem, dToolStripMenuItem1, dToolStripMenuItem2, dToolStripMenuItem3, dToolStripMenuItem4, dToolStripMenuItem5 });
            cntxChartPeriod.Name = "cntxChartPeriod";
            cntxChartPeriod.Size = new Size(100, 136);
            // 
            // dToolStripMenuItem
            // 
            dToolStripMenuItem.Name = "dToolStripMenuItem";
            dToolStripMenuItem.Size = new Size(99, 22);
            dToolStripMenuItem.Text = "30d";
            dToolStripMenuItem.Click += dToolStripMenuItem_Click;
            // 
            // dToolStripMenuItem1
            // 
            dToolStripMenuItem1.Name = "dToolStripMenuItem1";
            dToolStripMenuItem1.Size = new Size(99, 22);
            dToolStripMenuItem1.Text = "60d";
            dToolStripMenuItem1.Click += dToolStripMenuItem_Click;
            // 
            // dToolStripMenuItem2
            // 
            dToolStripMenuItem2.Name = "dToolStripMenuItem2";
            dToolStripMenuItem2.Size = new Size(99, 22);
            dToolStripMenuItem2.Text = "90d";
            dToolStripMenuItem2.Click += dToolStripMenuItem_Click;
            // 
            // dToolStripMenuItem3
            // 
            dToolStripMenuItem3.Name = "dToolStripMenuItem3";
            dToolStripMenuItem3.Size = new Size(99, 22);
            dToolStripMenuItem3.Text = "120d";
            dToolStripMenuItem3.Click += dToolStripMenuItem_Click;
            // 
            // dToolStripMenuItem4
            // 
            dToolStripMenuItem4.Name = "dToolStripMenuItem4";
            dToolStripMenuItem4.Size = new Size(99, 22);
            dToolStripMenuItem4.Text = "180d";
            dToolStripMenuItem4.Click += dToolStripMenuItem_Click;
            // 
            // dToolStripMenuItem5
            // 
            dToolStripMenuItem5.Name = "dToolStripMenuItem5";
            dToolStripMenuItem5.Size = new Size(99, 22);
            dToolStripMenuItem5.Text = "365d";
            dToolStripMenuItem5.Click += dToolStripMenuItem_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 611);
            Controls.Add(grpChartAssets);
            Controls.Add(btnAddNew);
            Controls.Add(btnAdvSearch);
            Controls.Add(btnTransactionBook);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            MinimumSize = new Size(970, 520);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Simple.Finance Demo";
            Load += frmMain_Load;
            Resize += frmMain_Resize;
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
            cntxNew.ResumeLayout(false);
            grpChartAssets.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)chtAssets).EndInit();
            cntxChartPeriod.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private DataGridView grdWallets;
        private DataGridView grdCategories;
        private ContextMenuStrip cntxEditDelete;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private DataGridView grdTxRecent;
        private DataGridView grdTxDue;
        private Button btnTransactionBook;
        private Button btnAdvSearch;
        private ContextMenuStrip cntxDueTx;
        private ToolStripMenuItem markAsPaidToolStripMenuItem;
        private ToolStripMenuItem dueTxPayOnDueDateToolStripMenuItem;
        private ToolStripMenuItem dueTxPayAsTodayToolStripMenuItem;
        private ToolStripMenuItem dueTxOpenForEditToolStripMenuItem;
        private ToolStripMenuItem dueTxReverseTransactionToolStripMenuItem;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private Button btnAddNew;
        private ContextMenuStrip cntxNew;
        private ToolStripMenuItem newWalletToolStripMenuItem;
        private ToolStripMenuItem newCategoryToolStripMenuItem;
        private ToolStripMenuItem singleTransactionToolStripMenuItem;
        private ToolStripMenuItem walletTransferToolStripMenuItem;
        private ToolStripMenuItem bulkTransactionsToolStripMenuItem;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem importOFXToolStripMenuItem;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DataGridViewTextBoxColumn Column8;
        private DataGridViewTextBoxColumn Column9;
        private DataGridViewTextBoxColumn Column7;
        private GroupBox grpChartAssets;
        private System.Windows.Forms.DataVisualization.Charting.Chart chtAssets;
        private ContextMenuStrip cntxChartPeriod;
        private ToolStripMenuItem dToolStripMenuItem;
        private ToolStripMenuItem dToolStripMenuItem1;
        private ToolStripMenuItem dToolStripMenuItem2;
        private ToolStripMenuItem dToolStripMenuItem3;
        private ToolStripMenuItem dToolStripMenuItem4;
        private ToolStripMenuItem dToolStripMenuItem5;
    }
}
