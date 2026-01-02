namespace DemoProject
{
    partial class frmTransactionBook
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            dtDate = new System.Windows.Forms.DateTimePicker();
            btnNext = new System.Windows.Forms.Button();
            btnPrev = new System.Windows.Forms.Button();
            grdTransactions = new System.Windows.Forms.DataGridView();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            btnNewTr = new System.Windows.Forms.Button();
            cntxBtn = new System.Windows.Forms.ContextMenuStrip(components);
            newTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newWalletTransferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)grdTransactions).BeginInit();
            cntxBtn.SuspendLayout();
            SuspendLayout();
            // 
            // dtDate
            // 
            dtDate.CustomFormat = "MMMM/yyyy";
            dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtDate.Location = new System.Drawing.Point(293, 12);
            dtDate.Name = "dtDate";
            dtDate.Size = new System.Drawing.Size(141, 23);
            dtDate.TabIndex = 0;
            dtDate.ValueChanged += dtDate_ValueChanged;
            // 
            // btnNext
            // 
            btnNext.Location = new System.Drawing.Point(440, 12);
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(30, 23);
            btnNext.TabIndex = 1;
            btnNext.Text = ">";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new System.Drawing.Point(257, 12);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new System.Drawing.Size(30, 23);
            btnPrev.TabIndex = 2;
            btnPrev.Text = "<";
            btnPrev.UseVisualStyleBackColor = true;
            btnPrev.Click += btnPrev_Click;
            // 
            // grdTransactions
            // 
            grdTransactions.AllowUserToAddRows = false;
            grdTransactions.AllowUserToDeleteRows = false;
            grdTransactions.AllowUserToResizeColumns = false;
            grdTransactions.AllowUserToResizeRows = false;
            grdTransactions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grdTransactions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdTransactions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column1, Column2, Column3, Column4, Column5 });
            grdTransactions.Location = new System.Drawing.Point(0, 41);
            grdTransactions.Name = "grdTransactions";
            grdTransactions.ReadOnly = true;
            grdTransactions.RowHeadersVisible = false;
            grdTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grdTransactions.Size = new System.Drawing.Size(726, 486);
            grdTransactions.TabIndex = 3;
            grdTransactions.CellDoubleClick += grdTransactions_CellDoubleClick;
            // 
            // Column1
            // 
            dataGridViewCellStyle1.Format = "dd/MM HH:mm";
            Column1.DefaultCellStyle = dataGridViewCellStyle1;
            Column1.HeaderText = "Date";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 80;
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
            dataGridViewCellStyle3.Format = "N2";
            Column5.DefaultCellStyle = dataGridViewCellStyle3;
            Column5.HeaderText = "Balance";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            // 
            // btnNewTr
            // 
            btnNewTr.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            btnNewTr.Location = new System.Drawing.Point(690, 7);
            btnNewTr.Name = "btnNewTr";
            btnNewTr.Size = new System.Drawing.Size(32, 28);
            btnNewTr.TabIndex = 4;
            btnNewTr.Text = "*";
            btnNewTr.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            btnNewTr.UseVisualStyleBackColor = true;
            btnNewTr.Click += btnNewTr_Click;
            // 
            // cntxBtn
            // 
            cntxBtn.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { newTransactionToolStripMenuItem, newWalletTransferToolStripMenuItem });
            cntxBtn.Name = "cntxBtn";
            cntxBtn.Size = new System.Drawing.Size(179, 48);
            // 
            // newTransactionToolStripMenuItem
            // 
            newTransactionToolStripMenuItem.Name = "newTransactionToolStripMenuItem";
            newTransactionToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            newTransactionToolStripMenuItem.Text = "New Transaction";
            newTransactionToolStripMenuItem.Click += newTransactionToolStripMenuItem_Click;
            // 
            // newWalletTransferToolStripMenuItem
            // 
            newWalletTransferToolStripMenuItem.Name = "newWalletTransferToolStripMenuItem";
            newWalletTransferToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            newWalletTransferToolStripMenuItem.Text = "New Wallet Transfer";
            newWalletTransferToolStripMenuItem.Click += newWalletTransferToolStripMenuItem_Click;
            // 
            // frmTransactionBook
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(726, 527);
            Controls.Add(btnNewTr);
            Controls.Add(grdTransactions);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(dtDate);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmTransactionBook";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Transaction Book";
            Load += frmTransactionBook_Load;
            ((System.ComponentModel.ISupportInitialize)grdTransactions).EndInit();
            cntxBtn.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.DataGridView grdTransactions;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.Button btnNewTr;
        private System.Windows.Forms.ContextMenuStrip cntxBtn;
        private System.Windows.Forms.ToolStripMenuItem newTransactionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newWalletTransferToolStripMenuItem;
    }
}