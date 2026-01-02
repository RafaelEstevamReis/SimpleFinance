namespace DemoProject.Dialogs
{
    partial class dlgAddBulkTransactions
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
            grdData = new System.Windows.Forms.DataGridView();
            label1 = new System.Windows.Forms.Label();
            cboWallet = new System.Windows.Forms.ComboBox();
            btnSave = new System.Windows.Forms.Button();
            clnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clnType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            clnCategory = new System.Windows.Forms.DataGridViewComboBoxColumn();
            clnDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            clnPaid = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)grdData).BeginInit();
            SuspendLayout();
            // 
            // grdData
            // 
            grdData.AllowUserToResizeColumns = false;
            grdData.AllowUserToResizeRows = false;
            grdData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            grdData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { clnDate, clnType, clnCategory, clnDescription, clnValue, clnPaid });
            grdData.Location = new System.Drawing.Point(0, 37);
            grdData.Name = "grdData";
            grdData.RowHeadersWidth = 21;
            grdData.Size = new System.Drawing.Size(800, 416);
            grdData.TabIndex = 0;
            grdData.CellValidating += grdData_CellValidating;
            grdData.CellValueChanged += grdData_CellValueChanged;
            grdData.DataError += grdData_DataError;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(7, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(43, 15);
            label1.TabIndex = 1;
            label1.Text = "Wallet:";
            // 
            // cboWallet
            // 
            cboWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboWallet.FormattingEnabled = true;
            cboWallet.Location = new System.Drawing.Point(56, 6);
            cboWallet.Name = "cboWallet";
            cboWallet.Size = new System.Drawing.Size(137, 23);
            cboWallet.TabIndex = 2;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(662, 459);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(131, 23);
            btnSave.TabIndex = 9;
            btnSave.Text = "Create Transactions";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // clnDate
            // 
            dataGridViewCellStyle1.Format = "d";
            clnDate.DefaultCellStyle = dataGridViewCellStyle1;
            clnDate.HeaderText = "Date";
            clnDate.Name = "clnDate";
            clnDate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            clnDate.Width = 80;
            // 
            // clnType
            // 
            clnType.HeaderText = "Type";
            clnType.Name = "clnType";
            clnType.Width = 90;
            // 
            // clnCategory
            // 
            clnCategory.HeaderText = "Category";
            clnCategory.Name = "clnCategory";
            clnCategory.Width = 150;
            // 
            // clnDescription
            // 
            clnDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            clnDescription.HeaderText = "Description";
            clnDescription.Name = "clnDescription";
            clnDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clnValue
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.Format = "N2";
            clnValue.DefaultCellStyle = dataGridViewCellStyle2;
            clnValue.HeaderText = "Value";
            clnValue.Name = "clnValue";
            clnValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // clnPaid
            // 
            clnPaid.HeaderText = "Paid";
            clnPaid.Name = "clnPaid";
            clnPaid.Width = 50;
            // 
            // dlgAddBulkTransactions
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 490);
            Controls.Add(btnSave);
            Controls.Add(cboWallet);
            Controls.Add(label1);
            Controls.Add(grdData);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "dlgAddBulkTransactions";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Add Bulk Transactions";
            Load += dlgAddBulkTransactions_Load;
            ((System.ComponentModel.ISupportInitialize)grdData).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView grdData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboWallet;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnType;
        private System.Windows.Forms.DataGridViewComboBoxColumn clnCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn clnValue;
        private System.Windows.Forms.DataGridViewCheckBoxColumn clnPaid;
    }
}