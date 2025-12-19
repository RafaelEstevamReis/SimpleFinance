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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            dtTo = new System.Windows.Forms.DateTimePicker();
            dtFrom = new System.Windows.Forms.DateTimePicker();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            cboDate = new System.Windows.Forms.ComboBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            cboReferenceType = new System.Windows.Forms.ComboBox();
            chkFilterReference = new System.Windows.Forms.CheckBox();
            label5 = new System.Windows.Forms.Label();
            chkHideUnpaids = new System.Windows.Forms.CheckBox();
            chkHidePaids = new System.Windows.Forms.CheckBox();
            cboReferenceItem = new System.Windows.Forms.ComboBox();
            btnSearch = new System.Windows.Forms.Button();
            grdTransactions = new System.Windows.Forms.DataGridView();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnAddTransaction = new System.Windows.Forms.Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdTransactions).BeginInit();
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
            dtTo.TabIndex = 4;
            // 
            // dtFrom
            // 
            dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtFrom.Location = new System.Drawing.Point(13, 76);
            dtFrom.Name = "dtFrom";
            dtFrom.Size = new System.Drawing.Size(91, 23);
            dtFrom.TabIndex = 3;
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
            groupBox2.Controls.Add(cboReferenceType);
            groupBox2.Controls.Add(chkFilterReference);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(chkHideUnpaids);
            groupBox2.Controls.Add(chkHidePaids);
            groupBox2.Controls.Add(cboReferenceItem);
            groupBox2.Location = new System.Drawing.Point(226, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(355, 112);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Of:";
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
            label5.Size = new System.Drawing.Size(62, 15);
            label5.TabIndex = 7;
            label5.Text = "Additional";
            // 
            // chkHideUnpaids
            // 
            chkHideUnpaids.AutoSize = true;
            chkHideUnpaids.Location = new System.Drawing.Point(99, 77);
            chkHideUnpaids.Name = "chkHideUnpaids";
            chkHideUnpaids.Size = new System.Drawing.Size(97, 19);
            chkHideUnpaids.TabIndex = 7;
            chkHideUnpaids.Text = "Hide Unpaids";
            chkHideUnpaids.UseVisualStyleBackColor = true;
            // 
            // chkHidePaids
            // 
            chkHidePaids.AutoSize = true;
            chkHidePaids.Location = new System.Drawing.Point(11, 77);
            chkHidePaids.Name = "chkHidePaids";
            chkHidePaids.Size = new System.Drawing.Size(82, 19);
            chkHidePaids.TabIndex = 6;
            chkHidePaids.Text = "Hide Paids";
            chkHidePaids.UseVisualStyleBackColor = true;
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
            // btnSearch
            // 
            btnSearch.Location = new System.Drawing.Point(728, 34);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new System.Drawing.Size(75, 23);
            btnSearch.TabIndex = 2;
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
            grdTransactions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column2, Column3, Column1, Column4, Column5 });
            grdTransactions.Location = new System.Drawing.Point(0, 120);
            grdTransactions.Name = "grdTransactions";
            grdTransactions.ReadOnly = true;
            grdTransactions.RowHeadersVisible = false;
            grdTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grdTransactions.Size = new System.Drawing.Size(815, 368);
            grdTransactions.TabIndex = 4;
            grdTransactions.CellDoubleClick += grdTransactions_CellDoubleClick;
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle1.Format = "d";
            Column1.DefaultCellStyle = dataGridViewCellStyle1;
            Column1.HeaderText = "Due Date";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 80;
            // 
            // Column4
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            Column4.DefaultCellStyle = dataGridViewCellStyle2;
            Column4.HeaderText = "Value";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // Column5
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.Format = "d";
            Column5.DefaultCellStyle = dataGridViewCellStyle3;
            Column5.HeaderText = "Paid Date";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 80;
            // 
            // btnAddTransaction
            // 
            btnAddTransaction.Location = new System.Drawing.Point(696, 492);
            btnAddTransaction.Name = "btnAddTransaction";
            btnAddTransaction.Size = new System.Drawing.Size(116, 31);
            btnAddTransaction.TabIndex = 5;
            btnAddTransaction.Text = "New Transaction";
            btnAddTransaction.UseVisualStyleBackColor = true;
            btnAddTransaction.Click += btnAddTransaction_Click;
            // 
            // frmAdvancedSearch
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(815, 527);
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
            ResumeLayout(false);
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
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.CheckBox chkFilterReference;
        private System.Windows.Forms.Button btnAddTransaction;
    }
}