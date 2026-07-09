namespace DemoProject.Dialogs
{
    partial class dlgNewWalletTransfer
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
            btnCancel = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            txtValue = new DemoProject.Components.MoneyBox();
            label3 = new System.Windows.Forms.Label();
            cboSourceWallet = new System.Windows.Forms.ComboBox();
            label11 = new System.Windows.Forms.Label();
            dtDate = new System.Windows.Forms.DateTimePicker();
            label9 = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            cboDestinationWallet = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            rdoPaid = new System.Windows.Forms.RadioButton();
            rdoUnpaid = new System.Windows.Forms.RadioButton();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(195, 237);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(93, 237);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 7;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtValue
            // 
            txtValue.DecimalPlaces = 2;
            txtValue.Location = new System.Drawing.Point(252, 149);
            txtValue.MoneySign = "";
            txtValue.Name = "txtValue";
            txtValue.Size = new System.Drawing.Size(101, 23);
            txtValue.TabIndex = 5;
            txtValue.Text = "0,00";
            txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(213, 153);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(38, 15);
            label3.TabIndex = 38;
            label3.Text = "Value:";
            // 
            // cboSourceWallet
            // 
            cboSourceWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboSourceWallet.FormattingEnabled = true;
            cboSourceWallet.Location = new System.Drawing.Point(134, 33);
            cboSourceWallet.Name = "cboSourceWallet";
            cboSourceWallet.Size = new System.Drawing.Size(219, 23);
            cboSourceWallet.TabIndex = 0;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(7, 36);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(82, 15);
            label11.TabIndex = 36;
            label11.Text = "Source Wallet:";
            // 
            // dtDate
            // 
            dtDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtDate.Location = new System.Drawing.Point(48, 149);
            dtDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtDate.Name = "dtDate";
            dtDate.Size = new System.Drawing.Size(116, 23);
            dtDate.TabIndex = 4;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(8, 153);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(34, 15);
            label9.TabIndex = 35;
            label9.Text = "Date:";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(8, 201);
            txtName.MaxLength = 64;
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(345, 23);
            txtName.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 182);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(130, 15);
            label1.TabIndex = 32;
            label1.Text = "Transaction Description";
            // 
            // cboDestinationWallet
            // 
            cboDestinationWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDestinationWallet.FormattingEnabled = true;
            cboDestinationWallet.Location = new System.Drawing.Point(134, 86);
            cboDestinationWallet.Name = "cboDestinationWallet";
            cboDestinationWallet.Size = new System.Drawing.Size(219, 23);
            cboDestinationWallet.TabIndex = 2;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 89);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(106, 15);
            label4.TabIndex = 41;
            label4.Text = "Destination Wallet:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(7, 10);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(110, 15);
            label5.TabIndex = 43;
            label5.Text = "Source (outbound):";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(7, 65);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(126, 15);
            label6.TabIndex = 44;
            label6.Text = "Destination (inbound):";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(8, 120);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(105, 15);
            label8.TabIndex = 46;
            label8.Text = "Transaction Status:";
            // 
            // panel2
            // 
            panel2.Controls.Add(rdoPaid);
            panel2.Controls.Add(rdoUnpaid);
            panel2.Location = new System.Drawing.Point(128, 115);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(150, 27);
            panel2.TabIndex = 45;
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
            // dlgNewWalletTransfer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(362, 269);
            Controls.Add(label8);
            Controls.Add(panel2);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(cboDestinationWallet);
            Controls.Add(label4);
            Controls.Add(txtValue);
            Controls.Add(label3);
            Controls.Add(cboSourceWallet);
            Controls.Add(label11);
            Controls.Add(dtDate);
            Controls.Add(label9);
            Controls.Add(txtName);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "dlgNewWalletTransfer";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "New Wallet Transfer";
            Load += dlgNewWalletTransfer_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private Components.MoneyBox txtValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboSourceWallet;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDestinationWallet;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoPaid;
        private System.Windows.Forms.RadioButton rdoUnpaid;
    }
}