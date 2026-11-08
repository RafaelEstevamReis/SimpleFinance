namespace DemoProject.Dialogs
{
    partial class dlgTransactionHistory
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
            grdDados = new System.Windows.Forms.DataGridView();
            Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)grdDados).BeginInit();
            SuspendLayout();
            // 
            // grdDados
            // 
            grdDados.AllowUserToAddRows = false;
            grdDados.AllowUserToDeleteRows = false;
            grdDados.AllowUserToResizeColumns = false;
            grdDados.AllowUserToResizeRows = false;
            grdDados.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdDados.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { Column5, Column1, Column2, Column3, Column4 });
            grdDados.Dock = System.Windows.Forms.DockStyle.Fill;
            grdDados.Location = new System.Drawing.Point(0, 0);
            grdDados.Name = "grdDados";
            grdDados.ReadOnly = true;
            grdDados.RowHeadersVisible = false;
            grdDados.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            grdDados.Size = new System.Drawing.Size(800, 450);
            grdDados.TabIndex = 0;
            // 
            // Column5
            // 
            Column5.HeaderText = "#";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column5.Width = 50;
            // 
            // Column1
            // 
            Column1.HeaderText = "Date/Time";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column1.Width = 120;
            // 
            // Column2
            // 
            Column2.HeaderText = "Column";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            Column2.Width = 150;
            // 
            // Column3
            // 
            Column3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Column3.HeaderText = "Old Value";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column4
            // 
            Column4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            Column4.HeaderText = "New Value";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dlgTransactionHistory
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(grdDados);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "dlgTransactionHistory";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Transaction History";
            Load += dlgTransactionHistory_Load;
            ((System.ComponentModel.ISupportInitialize)grdDados).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView grdDados;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
    }
}