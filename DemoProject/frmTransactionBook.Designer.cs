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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            dtDate = new System.Windows.Forms.DateTimePicker();
            btnNext = new System.Windows.Forms.Button();
            btnPrev = new System.Windows.Forms.Button();
            grdTransactions = new System.Windows.Forms.DataGridView();
            btnNewTr = new System.Windows.Forms.Button();
            cntxBtn = new System.Windows.Forms.ContextMenuStrip(components);
            newTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            newWalletTransferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            cboWallet = new System.Windows.Forms.ComboBox();
            label1 = new System.Windows.Forms.Label();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clnNetAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)grdTransactions).BeginInit();
            cntxBtn.SuspendLayout();
            SuspendLayout();
            // 
            // dtDate
            // 
            dtDate.CustomFormat = "MMMM/yyyy";
            dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtDate.Location = new System.Drawing.Point(293, 10);
            dtDate.Name = "dtDate";
            dtDate.Size = new System.Drawing.Size(141, 23);
            dtDate.TabIndex = 0;
            dtDate.ValueChanged += dtDate_ValueChanged;
            // 
            // btnNext
            // 
            btnNext.Location = new System.Drawing.Point(440, 10);
            btnNext.Name = "btnNext";
            btnNext.Size = new System.Drawing.Size(30, 23);
            btnNext.TabIndex = 1;
            btnNext.Text = ">";
            btnNext.UseVisualStyleBackColor = true;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Location = new System.Drawing.Point(257, 10);
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
            grdTransactions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column1, Column2, Column3, clnValue, clnNetAmount });
            grdTransactions.Location = new System.Drawing.Point(0, 41);
            grdTransactions.Name = "grdTransactions";
            grdTransactions.ReadOnly = true;
            grdTransactions.RowHeadersVisible = false;
            grdTransactions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grdTransactions.Size = new System.Drawing.Size(724, 490);
            grdTransactions.TabIndex = 3;
            grdTransactions.CellDoubleClick += grdTransactions_CellDoubleClick;
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
            cntxBtn.Size = new System.Drawing.Size(181, 70);
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
            // cboWallet
            // 
            cboWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboWallet.FormattingEnabled = true;
            cboWallet.Location = new System.Drawing.Point(50, 10);
            cboWallet.Name = "cboWallet";
            cboWallet.Size = new System.Drawing.Size(137, 23);
            cboWallet.TabIndex = 6;
            cboWallet.SelectedValueChanged += cboWallet_SelectedValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 14);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(43, 15);
            label1.TabIndex = 5;
            label1.Text = "Wallet:";
            // 
            // Column1
            // 
            dataGridViewCellStyle4.Format = "dd/MM HH:mm";
            Column1.DefaultCellStyle = dataGridViewCellStyle4;
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
            // clnValue
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.Format = "N2";
            clnValue.DefaultCellStyle = dataGridViewCellStyle5;
            clnValue.HeaderText = "Value";
            clnValue.Name = "clnValue";
            clnValue.ReadOnly = true;
            // 
            // clnNetAmount
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.Format = "N2";
            clnNetAmount.DefaultCellStyle = dataGridViewCellStyle6;
            clnNetAmount.HeaderText = "Net Amount";
            clnNetAmount.Name = "clnNetAmount";
            clnNetAmount.ReadOnly = true;
            // 
            // frmTransactionBook
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(724, 531);
            Controls.Add(cboWallet);
            Controls.Add(label1);
            Controls.Add(btnNewTr);
            Controls.Add(grdTransactions);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(dtDate);
            MaximizeBox = false;
            MinimumSize = new System.Drawing.Size(700, 500);
            Name = "frmTransactionBook";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Transaction Book";
            Load += frmTransactionBook_Load;
            ((System.ComponentModel.ISupportInitialize)grdTransactions).EndInit();
            cntxBtn.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.DataGridView grdTransactions;
        private System.Windows.Forms.Button btnNewTr;
        private System.Windows.Forms.ContextMenuStrip cntxBtn;
        private System.Windows.Forms.ToolStripMenuItem newTransactionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newWalletTransferToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboWallet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnNetAmount;
    }
}