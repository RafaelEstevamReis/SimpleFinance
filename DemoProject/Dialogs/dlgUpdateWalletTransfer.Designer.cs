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
            txtDueValue = new DemoProject.Components.MoneyBox();
            label3 = new System.Windows.Forms.Label();
            dtDueDate = new System.Windows.Forms.DateTimePicker();
            label9 = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            btnCancel = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            label8 = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            rdoReversed = new System.Windows.Forms.RadioButton();
            rdoPaid = new System.Windows.Forms.RadioButton();
            rdoUnpaid = new System.Windows.Forms.RadioButton();
            label4 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            lblDestinationWallet = new System.Windows.Forms.Label();
            lblSourceWallet = new System.Windows.Forms.Label();
            dtPaidDate = new System.Windows.Forms.DateTimePicker();
            label2 = new System.Windows.Forms.Label();
            txtPaymentDetails = new System.Windows.Forms.TextBox();
            label15 = new System.Windows.Forms.Label();
            txtPaidValue = new DemoProject.Components.MoneyBox();
            label5 = new System.Windows.Forms.Label();
            pnlPaid = new System.Windows.Forms.Panel();
            panel2.SuspendLayout();
            pnlPaid.SuspendLayout();
            SuspendLayout();
            // 
            // txtDueValue
            // 
            txtDueValue.DecimalPlaces = 2;
            txtDueValue.Location = new System.Drawing.Point(252, 88);
            txtDueValue.MoneySign = "";
            txtDueValue.Name = "txtDueValue";
            txtDueValue.Size = new System.Drawing.Size(101, 23);
            txtDueValue.TabIndex = 1;
            txtDueValue.Text = "0,00";
            txtDueValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtDueValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(189, 92);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(62, 15);
            label3.TabIndex = 46;
            label3.Text = "Due Value:";
            // 
            // dtDueDate
            // 
            dtDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtDueDate.Location = new System.Drawing.Point(72, 88);
            dtDueDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtDueDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtDueDate.Name = "dtDueDate";
            dtDueDate.Size = new System.Drawing.Size(116, 23);
            dtDueDate.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(8, 92);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(58, 15);
            label9.TabIndex = 45;
            label9.Text = "Due Date:";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(8, 170);
            txtName.MaxLength = 64;
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(345, 23);
            txtName.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 151);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(130, 15);
            label1.TabIndex = 42;
            label1.Text = "Transaction Description";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(195, 278);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(93, 278);
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
            label8.Location = new System.Drawing.Point(8, 61);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(105, 15);
            label8.TabIndex = 48;
            label8.Text = "Transaction Status:";
            // 
            // panel2
            // 
            panel2.Controls.Add(rdoReversed);
            panel2.Controls.Add(rdoPaid);
            panel2.Controls.Add(rdoUnpaid);
            panel2.Location = new System.Drawing.Point(114, 57);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(239, 26);
            panel2.TabIndex = 47;
            // 
            // rdoReversed
            // 
            rdoReversed.AutoSize = true;
            rdoReversed.Location = new System.Drawing.Point(149, 3);
            rdoReversed.Name = "rdoReversed";
            rdoReversed.Size = new System.Drawing.Size(72, 19);
            rdoReversed.TabIndex = 2;
            rdoReversed.Text = "Reversed";
            rdoReversed.UseVisualStyleBackColor = true;
            rdoReversed.CheckedChanged += rdoReversed_CheckedChanged;
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
            rdoPaid.CheckedChanged += rdoPaid_CheckedChanged;
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
            rdoUnpaid.CheckedChanged += rdoUnpaid_CheckedChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(8, 30);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(106, 15);
            label4.TabIndex = 50;
            label4.Text = "Destination Wallet:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(8, 8);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(82, 15);
            label11.TabIndex = 49;
            label11.Text = "Source Wallet:";
            // 
            // lblDestinationWallet
            // 
            lblDestinationWallet.AutoSize = true;
            lblDestinationWallet.Location = new System.Drawing.Point(120, 30);
            lblDestinationWallet.Name = "lblDestinationWallet";
            lblDestinationWallet.Size = new System.Drawing.Size(12, 15);
            lblDestinationWallet.TabIndex = 52;
            lblDestinationWallet.Text = "-";
            // 
            // lblSourceWallet
            // 
            lblSourceWallet.AutoSize = true;
            lblSourceWallet.Location = new System.Drawing.Point(120, 8);
            lblSourceWallet.Name = "lblSourceWallet";
            lblSourceWallet.Size = new System.Drawing.Size(12, 15);
            lblSourceWallet.TabIndex = 51;
            lblSourceWallet.Text = "-";
            // 
            // dtPaidDate
            // 
            dtPaidDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtPaidDate.Location = new System.Drawing.Point(71, 2);
            dtPaidDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtPaidDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtPaidDate.Name = "dtPaidDate";
            dtPaidDate.Size = new System.Drawing.Size(116, 23);
            dtPaidDate.TabIndex = 53;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 6);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(48, 15);
            label2.TabIndex = 54;
            label2.Text = "Paid At:";
            // 
            // txtPaymentDetails
            // 
            txtPaymentDetails.Location = new System.Drawing.Point(8, 215);
            txtPaymentDetails.MaxLength = 512;
            txtPaymentDetails.Multiline = true;
            txtPaymentDetails.Name = "txtPaymentDetails";
            txtPaymentDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            txtPaymentDetails.Size = new System.Drawing.Size(345, 57);
            txtPaymentDetails.TabIndex = 56;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(8, 198);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(92, 15);
            label15.TabIndex = 55;
            label15.Text = "Payment Details";
            // 
            // txtPaidValue
            // 
            txtPaidValue.DecimalPlaces = 2;
            txtPaidValue.Location = new System.Drawing.Point(251, 2);
            txtPaidValue.MoneySign = "";
            txtPaidValue.Name = "txtPaidValue";
            txtPaidValue.Size = new System.Drawing.Size(101, 23);
            txtPaidValue.TabIndex = 57;
            txtPaidValue.Text = "0,00";
            txtPaidValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtPaidValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(188, 6);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(64, 15);
            label5.TabIndex = 58;
            label5.Text = "Paid Value:";
            // 
            // pnlPaid
            // 
            pnlPaid.Controls.Add(label2);
            pnlPaid.Controls.Add(txtPaidValue);
            pnlPaid.Controls.Add(dtPaidDate);
            pnlPaid.Controls.Add(label5);
            pnlPaid.Location = new System.Drawing.Point(1, 112);
            pnlPaid.Name = "pnlPaid";
            pnlPaid.Size = new System.Drawing.Size(358, 27);
            pnlPaid.TabIndex = 59;
            // 
            // dlgUpdateWalletTransfer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(362, 312);
            Controls.Add(pnlPaid);
            Controls.Add(txtPaymentDetails);
            Controls.Add(label15);
            Controls.Add(lblDestinationWallet);
            Controls.Add(lblSourceWallet);
            Controls.Add(label4);
            Controls.Add(label11);
            Controls.Add(label8);
            Controls.Add(panel2);
            Controls.Add(txtDueValue);
            Controls.Add(label3);
            Controls.Add(dtDueDate);
            Controls.Add(label9);
            Controls.Add(txtName);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "dlgUpdateWalletTransfer";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Update Wallet Transfer";
            Load += dlgUpdateWalletTransfer_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            pnlPaid.ResumeLayout(false);
            pnlPaid.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Components.MoneyBox txtDueValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtDueDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoPaid;
        private System.Windows.Forms.RadioButton rdoUnpaid;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblDestinationWallet;
        private System.Windows.Forms.Label lblSourceWallet;
        private System.Windows.Forms.RadioButton rdoReversed;
        private System.Windows.Forms.DateTimePicker dtPaidDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPaymentDetails;
        private System.Windows.Forms.Label label15;
        private Components.MoneyBox txtPaidValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlPaid;
    }
}