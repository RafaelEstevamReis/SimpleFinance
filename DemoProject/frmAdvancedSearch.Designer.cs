namespace DemoProject
{
    partial class frmAdvancedSearch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            dtTo = new System.Windows.Forms.DateTimePicker();
            dtFrom = new System.Windows.Forms.DateTimePicker();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            cboDate = new System.Windows.Forms.ComboBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            txtDescription = new System.Windows.Forms.TextBox();
            cboReferenceType = new System.Windows.Forms.ComboBox();
            chkFilterReference = new System.Windows.Forms.CheckBox();
            label5 = new System.Windows.Forms.Label();
            cboReferenceItem = new System.Windows.Forms.ComboBox();
            chkHideReversed = new System.Windows.Forms.CheckBox();
            chkHideUnpaids = new System.Windows.Forms.CheckBox();
            chkHidePaids = new System.Windows.Forms.CheckBox();
            btnSearch = new System.Windows.Forms.Button();
            grdTransactions = new System.Windows.Forms.DataGridView();
            Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clnBalance = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnAddTransaction = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            txtTotalPaid = new DemoProject.Components.MoneyBox();
            txtTotalUnpaid = new DemoProject.Components.MoneyBox();
            label6 = new System.Windows.Forms.Label();
            txtTotalIncome = new DemoProject.Components.MoneyBox();
            label7 = new System.Windows.Forms.Label();
            txtTotalExpenses = new DemoProject.Components.MoneyBox();
            label8 = new System.Windows.Forms.Label();
            txtTotalSelected = new DemoProject.Components.MoneyBox();
            lblTotalSelected = new System.Windows.Forms.Label();
            groupBox3 = new System.Windows.Forms.GroupBox();
            chkIncludeUnpaidBalance = new System.Windows.Forms.CheckBox();
            cntxGrid = new System.Windows.Forms.ContextMenuStrip(components);
            changeDueValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            changeDueDayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            changeCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            markAsPaidAsOfTodayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            markAsPaidAsOfOriginalDueDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdTransactions).BeginInit();
            groupBox3.SuspendLayout();
            cntxGrid.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(dtTo);
            groupBox1.Controls.Add(dtFrom);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(cboDate);
            groupBox1.Location = new System.Drawing.Point(6, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(214, 112);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "By Date";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 15);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(61, 15);
            label4.TabIndex = 6;
            label4.Text = "Date Type:";
            // 
            // dtTo
            // 
            dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtTo.Location = new System.Drawing.Point(111, 76);
            dtTo.Name = "dtTo";
            dtTo.Size = new System.Drawing.Size(91, 23);
            dtTo.TabIndex = 2;
            // 
            // dtFrom
            // 
            dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtFrom.Location = new System.Drawing.Point(13, 76);
            dtFrom.Name = "dtFrom";
            dtFrom.Size = new System.Drawing.Size(91, 23);
            dtFrom.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(109, 58);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(22, 15);
            label2.TabIndex = 2;
            label2.Text = "To:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(10, 58);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(38, 15);
            label1.TabIndex = 1;
            label1.Text = "From:";
            // 
            // cboDate
            // 
            cboDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDate.FormattingEnabled = true;
            cboDate.Items.AddRange(new object[] { "Due Date", "Payment Date", "Creation Date", "Last Update", "Effective Date" });
            cboDate.Location = new System.Drawing.Point(13, 32);
            cboDate.Name = "cboDate";
            cboDate.Size = new System.Drawing.Size(189, 23);
            cboDate.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtDescription);
            groupBox2.Controls.Add(cboReferenceType);
            groupBox2.Controls.Add(chkFilterReference);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(cboReferenceItem);
            groupBox2.Location = new System.Drawing.Point(226, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(355, 112);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Of:";
            // 
            // txtDescription
            // 
            txtDescription.Location = new System.Drawing.Point(6, 76);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new System.Drawing.Size(343, 23);
            txtDescription.TabIndex = 2;
            // 
            // cboReferenceType
            // 
            cboReferenceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboReferenceType.FormattingEnabled = true;
            cboReferenceType.Items.AddRange(new object[] { "Wallet", "Category" });
            cboReferenceType.Location = new System.Drawing.Point(6, 32);
            cboReferenceType.Name = "cboReferenceType";
            cboReferenceType.Size = new System.Drawing.Size(123, 23);
            cboReferenceType.TabIndex = 0;
            cboReferenceType.SelectedIndexChanged += cboReferenceType_SelectedIndexChanged;
            // 
            // chkFilterReference
            // 
            chkFilterReference.AutoSize = true;
            chkFilterReference.Location = new System.Drawing.Point(6, 15);
            chkFilterReference.Name = "chkFilterReference";
            chkFilterReference.Size = new System.Drawing.Size(55, 19);
            chkFilterReference.TabIndex = 8;
            chkFilterReference.Text = "Filter:";
            chkFilterReference.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 59);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(120, 15);
            label5.TabIndex = 7;
            label5.Text = "Description Contains:";
            // 
            // cboReferenceItem
            // 
            cboReferenceItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboReferenceItem.FormattingEnabled = true;
            cboReferenceItem.Items.AddRange(new object[] { "Wallet", "Category" });
            cboReferenceItem.Location = new System.Drawing.Point(135, 32);
            cboReferenceItem.Name = "cboReferenceItem";
            cboReferenceItem.Size = new System.Drawing.Size(214, 23);
            cboReferenceItem.TabIndex = 1;
            // 
            // chkHideReversed
            // 
            chkHideReversed.AutoSize = true;
            chkHideReversed.Checked = true;
            chkHideReversed.CheckState = System.Windows.Forms.CheckState.Checked;
            chkHideReversed.Location = new System.Drawing.Point(185, 17);
            chkHideReversed.Name = "chkHideReversed";
            chkHideReversed.Size = new System.Drawing.Size(101, 19);
            chkHideReversed.TabIndex = 3;
            chkHideReversed.Text = "Hide Reversed";
            chkHideReversed.UseVisualStyleBackColor = true;
            // 
            // chkHideUnpaids
            // 
            chkHideUnpaids.AutoSize = true;
            chkHideUnpaids.Location = new System.Drawing.Point(88, 17);
            chkHideUnpaids.Name = "chkHideUnpaids";
            chkHideUnpaids.Size = new System.Drawing.Size(92, 19);
            chkHideUnpaids.TabIndex = 2;
            chkHideUnpaids.Text = "Hide Unpaid";
            chkHideUnpaids.UseVisualStyleBackColor = true;
            // 
            // chkHidePaids
            // 
            chkHidePaids.AutoSize = true;
            chkHidePaids.Location = new System.Drawing.Point(6, 17);
            chkHidePaids.Name = "chkHidePaids";
            chkHidePaids.Size = new System.Drawing.Size(77, 19);
            chkHidePaids.TabIndex = 1;
            chkHidePaids.Text = "Hide Paid";
            chkHidePaids.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            btnSearch.Location = new System.Drawing.Point(889, 34);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new System.Drawing.Size(75, 23);
            btnSearch.TabIndex = 3;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // grdTransactions
            // 
            grdTransactions.AllowUserToAddRows = false;
            grdTransactions.AllowUserToDeleteRows = false;
            grdTransactions.AllowUserToResizeColumns = false;
            grdTransactions.AllowUserToResizeRows = false;
            grdTransactions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grdTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdTransactions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column6, Column2, Column3, Column1, Column4, Column5, clnBalance });
            grdTransactions.Location = new System.Drawing.Point(0, 120);
            grdTransactions.Name = "grdTransactions";
            grdTransactions.ReadOnly = true;
            grdTransactions.RowHeadersVisible = false;
            grdTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grdTransactions.Size = new System.Drawing.Size(976, 398);
            grdTransactions.TabIndex = 3;
            grdTransactions.CellDoubleClick += grdTransactions_CellDoubleClick;
            grdTransactions.CellMouseClick += grdTransactions_CellMouseClick;
            grdTransactions.SelectionChanged += grdTransactions_SelectionChanged;
            // 
            // Column6
            // 
            Column6.HeaderText = "Wallet";
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            Column6.Width = 150;
            // 
            // Column2
            // 
            Column2.HeaderText = "Category";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 150;
            // 
            // Column3
            // 
            Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Column3.HeaderText = "Description";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            // 
            // Column1
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "d";
            Column1.DefaultCellStyle = dataGridViewCellStyle5;
            Column1.HeaderText = "Due Date";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 80;
            // 
            // Column4
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            Column4.DefaultCellStyle = dataGridViewCellStyle6;
            Column4.HeaderText = "Value";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // Column5
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle7.Format = "d";
            Column5.DefaultCellStyle = dataGridViewCellStyle7;
            Column5.HeaderText = "Paid Date";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 80;
            // 
            // clnBalance
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle8.Format = "N2";
            clnBalance.DefaultCellStyle = dataGridViewCellStyle8;
            clnBalance.HeaderText = "Net Amount";
            clnBalance.Name = "clnBalance";
            clnBalance.ReadOnly = true;
            // 
            // btnAddTransaction
            // 
            btnAddTransaction.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            btnAddTransaction.Location = new System.Drawing.Point(857, 524);
            btnAddTransaction.Name = "btnAddTransaction";
            btnAddTransaction.Size = new System.Drawing.Size(116, 31);
            btnAddTransaction.TabIndex = 9;
            btnAddTransaction.Text = "New Transaction";
            btnAddTransaction.UseVisualStyleBackColor = true;
            btnAddTransaction.Click += btnAddTransaction_Click;
            // 
            // label3
            // 
            label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 519);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(61, 15);
            label3.TabIndex = 6;
            label3.Text = "Total Paid:";
            // 
            // txtTotalPaid
            // 
            txtTotalPaid.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            txtTotalPaid.DecimalPlaces = 2;
            txtTotalPaid.Location = new System.Drawing.Point(6, 534);
            txtTotalPaid.MoneySign = "";
            txtTotalPaid.Name = "txtTotalPaid";
            txtTotalPaid.ReadOnly = true;
            txtTotalPaid.Size = new System.Drawing.Size(100, 23);
            txtTotalPaid.TabIndex = 4;
            txtTotalPaid.Text = "0,00";
            txtTotalPaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtTotalPaid.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // txtTotalUnpaid
            // 
            txtTotalUnpaid.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            txtTotalUnpaid.DecimalPlaces = 2;
            txtTotalUnpaid.Location = new System.Drawing.Point(112, 534);
            txtTotalUnpaid.MoneySign = "";
            txtTotalUnpaid.Name = "txtTotalUnpaid";
            txtTotalUnpaid.ReadOnly = true;
            txtTotalUnpaid.Size = new System.Drawing.Size(100, 23);
            txtTotalUnpaid.TabIndex = 5;
            txtTotalUnpaid.Text = "0,00";
            txtTotalUnpaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtTotalUnpaid.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label6
            // 
            label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(112, 519);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(76, 15);
            label6.TabIndex = 8;
            label6.Text = "Total Unpaid:";
            // 
            // txtTotalIncome
            // 
            txtTotalIncome.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            txtTotalIncome.DecimalPlaces = 2;
            txtTotalIncome.Location = new System.Drawing.Point(218, 534);
            txtTotalIncome.MoneySign = "";
            txtTotalIncome.Name = "txtTotalIncome";
            txtTotalIncome.ReadOnly = true;
            txtTotalIncome.Size = new System.Drawing.Size(100, 23);
            txtTotalIncome.TabIndex = 6;
            txtTotalIncome.Text = "0,00";
            txtTotalIncome.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtTotalIncome.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label7
            // 
            label7.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(218, 519);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(71, 15);
            label7.TabIndex = 10;
            label7.Text = "Total Icome:";
            // 
            // txtTotalExpenses
            // 
            txtTotalExpenses.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            txtTotalExpenses.DecimalPlaces = 2;
            txtTotalExpenses.Location = new System.Drawing.Point(324, 534);
            txtTotalExpenses.MoneySign = "";
            txtTotalExpenses.Name = "txtTotalExpenses";
            txtTotalExpenses.ReadOnly = true;
            txtTotalExpenses.Size = new System.Drawing.Size(100, 23);
            txtTotalExpenses.TabIndex = 7;
            txtTotalExpenses.Text = "0,00";
            txtTotalExpenses.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtTotalExpenses.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label8
            // 
            label8.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(324, 519);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(86, 15);
            label8.TabIndex = 12;
            label8.Text = "Total Expenses:";
            // 
            // txtTotalSelected
            // 
            txtTotalSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            txtTotalSelected.DecimalPlaces = 2;
            txtTotalSelected.Location = new System.Drawing.Point(430, 534);
            txtTotalSelected.MoneySign = "";
            txtTotalSelected.Name = "txtTotalSelected";
            txtTotalSelected.ReadOnly = true;
            txtTotalSelected.Size = new System.Drawing.Size(100, 23);
            txtTotalSelected.TabIndex = 8;
            txtTotalSelected.Text = "0,00";
            txtTotalSelected.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtTotalSelected.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // lblTotalSelected
            // 
            lblTotalSelected.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lblTotalSelected.AutoSize = true;
            lblTotalSelected.Location = new System.Drawing.Point(430, 519);
            lblTotalSelected.Name = "lblTotalSelected";
            lblTotalSelected.Size = new System.Drawing.Size(82, 15);
            lblTotalSelected.TabIndex = 14;
            lblTotalSelected.Text = "Total Selected:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(chkHideReversed);
            groupBox3.Controls.Add(chkIncludeUnpaidBalance);
            groupBox3.Controls.Add(chkHidePaids);
            groupBox3.Controls.Add(chkHideUnpaids);
            groupBox3.Location = new System.Drawing.Point(587, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(296, 112);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Display";
            // 
            // chkIncludeUnpaidBalance
            // 
            chkIncludeUnpaidBalance.AutoSize = true;
            chkIncludeUnpaidBalance.Location = new System.Drawing.Point(6, 37);
            chkIncludeUnpaidBalance.Name = "chkIncludeUnpaidBalance";
            chkIncludeUnpaidBalance.Size = new System.Drawing.Size(168, 19);
            chkIncludeUnpaidBalance.TabIndex = 0;
            chkIncludeUnpaidBalance.Text = "Include Unpaids in Balance";
            chkIncludeUnpaidBalance.UseVisualStyleBackColor = true;
            // 
            // cntxGrid
            // 
            cntxGrid.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { changeDueValueToolStripMenuItem, changeDueDayToolStripMenuItem, changeCategoryToolStripMenuItem, toolStripMenuItem1, markAsPaidAsOfTodayToolStripMenuItem, markAsPaidAsOfOriginalDueDateToolStripMenuItem });
            cntxGrid.Name = "cntxGrid";
            cntxGrid.Size = new System.Drawing.Size(276, 142);
            // 
            // changeDueValueToolStripMenuItem
            // 
            changeDueValueToolStripMenuItem.Name = "changeDueValueToolStripMenuItem";
            changeDueValueToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            changeDueValueToolStripMenuItem.Text = "Change Due Value";
            changeDueValueToolStripMenuItem.Click += changeDueValueToolStripMenuItem_Click;
            // 
            // changeDueDayToolStripMenuItem
            // 
            changeDueDayToolStripMenuItem.Name = "changeDueDayToolStripMenuItem";
            changeDueDayToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            changeDueDayToolStripMenuItem.Text = "Change Due Day";
            changeDueDayToolStripMenuItem.Click += changeDueDayToolStripMenuItem_Click;
            // 
            // changeCategoryToolStripMenuItem
            // 
            changeCategoryToolStripMenuItem.Name = "changeCategoryToolStripMenuItem";
            changeCategoryToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            changeCategoryToolStripMenuItem.Text = "Change Category";
            changeCategoryToolStripMenuItem.Click += changeCategoryToolStripMenuItem_Click;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(272, 6);
            // 
            // markAsPaidAsOfTodayToolStripMenuItem
            // 
            markAsPaidAsOfTodayToolStripMenuItem.Name = "markAsPaidAsOfTodayToolStripMenuItem";
            markAsPaidAsOfTodayToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            markAsPaidAsOfTodayToolStripMenuItem.Text = "Mark as Paid - As of Today";
            markAsPaidAsOfTodayToolStripMenuItem.Click += markAsPaidAsOfTodayToolStripMenuItem_Click;
            // 
            // markAsPaidAsOfOriginalDueDateToolStripMenuItem
            // 
            markAsPaidAsOfOriginalDueDateToolStripMenuItem.Name = "markAsPaidAsOfOriginalDueDateToolStripMenuItem";
            markAsPaidAsOfOriginalDueDateToolStripMenuItem.Size = new System.Drawing.Size(275, 22);
            markAsPaidAsOfOriginalDueDateToolStripMenuItem.Text = "Mark as Paid - As of Original Due Date";
            markAsPaidAsOfOriginalDueDateToolStripMenuItem.Click += markAsPaidAsOfOriginalDueDateToolStripMenuItem_Click;
            // 
            // frmAdvancedSearch
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(976, 561);
            Controls.Add(groupBox3);
            Controls.Add(txtTotalSelected);
            Controls.Add(lblTotalSelected);
            Controls.Add(txtTotalExpenses);
            Controls.Add(label8);
            Controls.Add(txtTotalIncome);
            Controls.Add(label7);
            Controls.Add(txtTotalUnpaid);
            Controls.Add(label6);
            Controls.Add(txtTotalPaid);
            Controls.Add(label3);
            Controls.Add(btnAddTransaction);
            Controls.Add(grdTransactions);
            Controls.Add(btnSearch);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            MaximizeBox = false;
            Name = "frmAdvancedSearch";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "AdvancedSearch";
            Load += frmAdvancedSearch_Load;
            Shown += frmAdvancedSearch_Shown;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grdTransactions).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            cntxGrid.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cboDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chkHideUnpaids;
        private System.Windows.Forms.CheckBox chkHidePaids;
        private System.Windows.Forms.ComboBox cboReferenceItem;
        private System.Windows.Forms.ComboBox cboReferenceType;
        private System.Windows.Forms.DataGridView grdTransactions;
        private System.Windows.Forms.CheckBox chkFilterReference;
        private System.Windows.Forms.Button btnAddTransaction;
        private System.Windows.Forms.Label label3;
        private Components.MoneyBox txtTotalPaid;
        private Components.MoneyBox txtTotalUnpaid;
        private System.Windows.Forms.Label label6;
        private Components.MoneyBox txtTotalIncome;
        private System.Windows.Forms.Label label7;
        private Components.MoneyBox txtTotalExpenses;
        private System.Windows.Forms.Label label8;
        private Components.MoneyBox txtTotalSelected;
        private System.Windows.Forms.Label lblTotalSelected;
        private System.Windows.Forms.CheckBox chkHideReversed;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkIncludeUnpaidBalance;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ContextMenuStrip cntxGrid;
        private System.Windows.Forms.ToolStripMenuItem changeDueValueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeDueDayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeCategoryToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnBalance;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem markAsPaidAsOfTodayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markAsPaidAsOfOriginalDueDateToolStripMenuItem;
    }
}