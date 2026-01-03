namespace DemoProject.Dialogs
{
    partial class dlgUpdateWalletTransfer
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
            txtValue = new DemoProject.Components.MoneyBox();
            label3 = new System.Windows.Forms.Label();
            dtDate = new System.Windows.Forms.DateTimePicker();
            label9 = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            btnCancel = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            label8 = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            rdoPaid = new System.Windows.Forms.RadioButton();
            rdoUnpaid = new System.Windows.Forms.RadioButton();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // txtValue
            // 
            txtValue.DecimalPlaces = 2;
            txtValue.Location = new System.Drawing.Point(252, 10);
            txtValue.MoneySign = "";
            txtValue.Name = "txtValue";
            txtValue.Size = new System.Drawing.Size(101, 23);
            txtValue.TabIndex = 1;
            txtValue.Text = "0,00";
            txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(213, 14);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(38, 15);
            label3.TabIndex = 46;
            label3.Text = "Value:";
            // 
            // dtDate
            // 
            dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtDate.Location = new System.Drawing.Point(48, 10);
            dtDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtDate.Name = "dtDate";
            dtDate.Size = new System.Drawing.Size(116, 23);
            dtDate.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(8, 14);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(34, 15);
            label9.TabIndex = 45;
            label9.Text = "Date:";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(8, 95);
            txtName.MaxLength = 64;
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(345, 23);
            txtName.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 76);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(130, 15);
            label1.TabIndex = 42;
            label1.Text = "Transaction Description";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(195, 123);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(93, 123);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 3;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(8, 46);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(105, 15);
            label8.TabIndex = 48;
            label8.Text = "Transaction Status:";
            // 
            // panel2
            // 
            panel2.Controls.Add(rdoPaid);
            panel2.Controls.Add(rdoUnpaid);
            panel2.Location = new System.Drawing.Point(114, 42);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(150, 26);
            panel2.TabIndex = 47;
            // 
            // rdoPaid
            // 
            rdoPaid.AutoSize = true;
            rdoPaid.Checked = true;
            rdoPaid.Location = new System.Drawing.Point(84, 3);
            rdoPaid.Name = "rdoPaid";
            rdoPaid.Size = new System.Drawing.Size(48, 19);
            rdoPaid.TabIndex = 1;
            rdoPaid.TabStop = true;
            rdoPaid.Text = "Paid";
            rdoPaid.UseVisualStyleBackColor = true;
            // 
            // rdoUnpaid
            // 
            rdoUnpaid.AutoSize = true;
            rdoUnpaid.Location = new System.Drawing.Point(6, 3);
            rdoUnpaid.Name = "rdoUnpaid";
            rdoUnpaid.Size = new System.Drawing.Size(63, 19);
            rdoUnpaid.TabIndex = 0;
            rdoUnpaid.Text = "Unpaid";
            rdoUnpaid.UseVisualStyleBackColor = true;
            // 
            // dlgUpdateWalletTransfer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(362, 153);
            Controls.Add(label8);
            Controls.Add(panel2);
            Controls.Add(txtValue);
            Controls.Add(label3);
            Controls.Add(dtDate);
            Controls.Add(label9);
            Controls.Add(txtName);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            Name = "dlgUpdateWalletTransfer";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Update Wallet Transfer";
            Load += dlgUpdateWalletTransfer_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Components.MoneyBox txtValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoPaid;
        private System.Windows.Forms.RadioButton rdoUnpaid;
    }
}