namespace DemoProject.Dialogs
{
    partial class dlgEditTransaction
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
            txtName = new System.Windows.Forms.TextBox();
            lblId = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            lblCreated = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            lblChanged = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            panel1 = new System.Windows.Forms.Panel();
            rdoIncome = new System.Windows.Forms.RadioButton();
            rdoExpense = new System.Windows.Forms.RadioButton();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            panel2 = new System.Windows.Forms.Panel();
            rdoReversed = new System.Windows.Forms.RadioButton();
            rdoPaid = new System.Windows.Forms.RadioButton();
            rdoUnpaid = new System.Windows.Forms.RadioButton();
            label9 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            dtDue = new System.Windows.Forms.DateTimePicker();
            dtPaid = new System.Windows.Forms.DateTimePicker();
            label11 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            cboWallet = new System.Windows.Forms.ComboBox();
            cboCategory = new System.Windows.Forms.ComboBox();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            txtDue = new DemoProject.Components.MoneyBox();
            txtPaid = new DemoProject.Components.MoneyBox();
            pnlPaid = new System.Windows.Forms.Panel();
            lblAdvanced = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            udRecuringCopies = new System.Windows.Forms.NumericUpDown();
            label14 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            cboRecuringPeriod = new System.Windows.Forms.ComboBox();
            rdoRecuringYes = new System.Windows.Forms.RadioButton();
            rdoRecuringNo = new System.Windows.Forms.RadioButton();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            pnlPaid.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)udRecuringCopies).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(195, 283);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(93, 283);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 10;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(9, 250);
            txtName.MaxLength = 64;
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(345, 23);
            txtName.TabIndex = 9;
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new System.Drawing.Point(31, 10);
            lblId.Name = "lblId";
            lblId.Size = new System.Drawing.Size(12, 15);
            lblId.TabIndex = 8;
            lblId.Text = "-";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(9, 10);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(20, 15);
            label2.TabIndex = 7;
            label2.Text = "Id:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 231);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(130, 15);
            label1.TabIndex = 6;
            label1.Text = "Transaction Description";
            // 
            // lblCreated
            // 
            lblCreated.AutoSize = true;
            lblCreated.Location = new System.Drawing.Point(124, 10);
            lblCreated.Name = "lblCreated";
            lblCreated.Size = new System.Drawing.Size(12, 15);
            lblCreated.TabIndex = 0;
            lblCreated.Text = "-";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(74, 10);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(51, 15);
            label4.TabIndex = 12;
            label4.Text = "Created:";
            // 
            // lblChanged
            // 
            lblChanged.AutoSize = true;
            lblChanged.Location = new System.Drawing.Point(251, 10);
            lblChanged.Name = "lblChanged";
            lblChanged.Size = new System.Drawing.Size(12, 15);
            lblChanged.TabIndex = 1;
            lblChanged.Text = "-";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(193, 10);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(58, 15);
            label6.TabIndex = 14;
            label6.Text = "Changed:";
            // 
            // panel1
            // 
            panel1.Controls.Add(rdoIncome);
            panel1.Controls.Add(rdoExpense);
            panel1.Location = new System.Drawing.Point(121, 64);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(161, 27);
            panel1.TabIndex = 3;
            // 
            // rdoIncome
            // 
            rdoIncome.AutoSize = true;
            rdoIncome.Location = new System.Drawing.Point(86, 3);
            rdoIncome.Name = "rdoIncome";
            rdoIncome.Size = new System.Drawing.Size(65, 19);
            rdoIncome.TabIndex = 1;
            rdoIncome.TabStop = true;
            rdoIncome.Text = "Income";
            rdoIncome.UseVisualStyleBackColor = true;
            rdoIncome.CheckedChanged += rdoIncome_CheckedChanged;
            // 
            // rdoExpense
            // 
            rdoExpense.AutoSize = true;
            rdoExpense.Location = new System.Drawing.Point(3, 3);
            rdoExpense.Name = "rdoExpense";
            rdoExpense.Size = new System.Drawing.Size(68, 19);
            rdoExpense.TabIndex = 0;
            rdoExpense.TabStop = true;
            rdoExpense.Text = "Expense";
            rdoExpense.UseVisualStyleBackColor = true;
            rdoExpense.CheckedChanged += rdoExpense_CheckedChanged;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(9, 69);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(97, 15);
            label7.TabIndex = 17;
            label7.Text = "Transaction Type:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(9, 135);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(105, 15);
            label8.TabIndex = 19;
            label8.Text = "Transaction Status:";
            // 
            // panel2
            // 
            panel2.Controls.Add(rdoReversed);
            panel2.Controls.Add(rdoPaid);
            panel2.Controls.Add(rdoUnpaid);
            panel2.Location = new System.Drawing.Point(121, 130);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(234, 27);
            panel2.TabIndex = 5;
            // 
            // rdoReversed
            // 
            rdoReversed.AutoSize = true;
            rdoReversed.Location = new System.Drawing.Point(140, 3);
            rdoReversed.Name = "rdoReversed";
            rdoReversed.Size = new System.Drawing.Size(72, 19);
            rdoReversed.TabIndex = 2;
            rdoReversed.TabStop = true;
            rdoReversed.Text = "Reversed";
            rdoReversed.UseVisualStyleBackColor = true;
            // 
            // rdoPaid
            // 
            rdoPaid.AutoSize = true;
            rdoPaid.Location = new System.Drawing.Point(77, 3);
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
            rdoUnpaid.Location = new System.Drawing.Point(3, 3);
            rdoUnpaid.Name = "rdoUnpaid";
            rdoUnpaid.Size = new System.Drawing.Size(63, 19);
            rdoUnpaid.TabIndex = 0;
            rdoUnpaid.TabStop = true;
            rdoUnpaid.Text = "Unpaid";
            rdoUnpaid.UseVisualStyleBackColor = true;
            rdoUnpaid.CheckedChanged += rdoUnpaid_CheckedChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(9, 168);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(58, 15);
            label9.TabIndex = 20;
            label9.Text = "Due Date:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(3, 7);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(48, 15);
            label10.TabIndex = 21;
            label10.Text = "Paid At:";
            // 
            // dtDue
            // 
            dtDue.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            dtDue.Location = new System.Drawing.Point(73, 164);
            dtDue.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtDue.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtDue.Name = "dtDue";
            dtDue.Size = new System.Drawing.Size(95, 23);
            dtDue.TabIndex = 6;
            // 
            // dtPaid
            // 
            dtPaid.CustomFormat = "dd/MM/yyyy HH:mm";
            dtPaid.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            dtPaid.Location = new System.Drawing.Point(64, 3);
            dtPaid.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtPaid.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtPaid.Name = "dtPaid";
            dtPaid.Size = new System.Drawing.Size(132, 23);
            dtPaid.TabIndex = 0;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(9, 36);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(43, 15);
            label11.TabIndex = 24;
            label11.Text = "Wallet:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(9, 102);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(58, 15);
            label12.TabIndex = 25;
            label12.Text = "Cateogry:";
            // 
            // cboWallet
            // 
            cboWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboWallet.FormattingEnabled = true;
            cboWallet.Location = new System.Drawing.Point(76, 33);
            cboWallet.Name = "cboWallet";
            cboWallet.Size = new System.Drawing.Size(279, 23);
            cboWallet.TabIndex = 2;
            // 
            // cboCategory
            // 
            cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboCategory.FormattingEnabled = true;
            cboCategory.Location = new System.Drawing.Point(76, 99);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new System.Drawing.Size(279, 23);
            cboCategory.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(211, 168);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(62, 15);
            label3.TabIndex = 28;
            label3.Text = "Due Value:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(202, 7);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(64, 15);
            label5.TabIndex = 29;
            label5.Text = "Paid Value:";
            // 
            // txtDue
            // 
            txtDue.DecimalPlaces = 2;
            txtDue.Location = new System.Drawing.Point(274, 164);
            txtDue.MoneySign = "";
            txtDue.Name = "txtDue";
            txtDue.Size = new System.Drawing.Size(80, 23);
            txtDue.TabIndex = 7;
            txtDue.Text = "0,00";
            txtDue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtDue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // txtPaid
            // 
            txtPaid.DecimalPlaces = 2;
            txtPaid.Location = new System.Drawing.Point(265, 3);
            txtPaid.MoneySign = "";
            txtPaid.Name = "txtPaid";
            txtPaid.Size = new System.Drawing.Size(81, 23);
            txtPaid.TabIndex = 1;
            txtPaid.Text = "0,00";
            txtPaid.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtPaid.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // pnlPaid
            // 
            pnlPaid.Controls.Add(label10);
            pnlPaid.Controls.Add(txtPaid);
            pnlPaid.Controls.Add(dtPaid);
            pnlPaid.Controls.Add(label5);
            pnlPaid.Location = new System.Drawing.Point(9, 193);
            pnlPaid.Name = "pnlPaid";
            pnlPaid.Size = new System.Drawing.Size(349, 30);
            pnlPaid.TabIndex = 8;
            // 
            // lblAdvanced
            // 
            lblAdvanced.AutoSize = true;
            lblAdvanced.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            lblAdvanced.Location = new System.Drawing.Point(1, 300);
            lblAdvanced.Name = "lblAdvanced";
            lblAdvanced.Size = new System.Drawing.Size(75, 17);
            lblAdvanced.TabIndex = 33;
            lblAdvanced.Text = "Advanced ▼";
            lblAdvanced.Click += lblAdvanced_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(udRecuringCopies);
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(label13);
            groupBox1.Controls.Add(cboRecuringPeriod);
            groupBox1.Controls.Add(rdoRecuringYes);
            groupBox1.Controls.Add(rdoRecuringNo);
            groupBox1.Location = new System.Drawing.Point(9, 317);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(345, 83);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Create as Recuring";
            // 
            // udRecuringCopies
            // 
            udRecuringCopies.Location = new System.Drawing.Point(233, 47);
            udRecuringCopies.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
            udRecuringCopies.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            udRecuringCopies.Name = "udRecuringCopies";
            udRecuringCopies.Size = new System.Drawing.Size(44, 23);
            udRecuringCopies.TabIndex = 3;
            udRecuringCopies.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(183, 50);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(46, 15);
            label14.TabIndex = 36;
            label14.Text = "Copies:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(9, 50);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(34, 15);
            label13.TabIndex = 35;
            label13.Text = "Type:";
            // 
            // cboRecuringPeriod
            // 
            cboRecuringPeriod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboRecuringPeriod.FormattingEnabled = true;
            cboRecuringPeriod.Items.AddRange(new object[] { "Weekly", "Monthly" });
            cboRecuringPeriod.Location = new System.Drawing.Point(49, 47);
            cboRecuringPeriod.Name = "cboRecuringPeriod";
            cboRecuringPeriod.Size = new System.Drawing.Size(121, 23);
            cboRecuringPeriod.TabIndex = 2;
            // 
            // rdoRecuringYes
            // 
            rdoRecuringYes.AutoSize = true;
            rdoRecuringYes.Location = new System.Drawing.Point(121, 19);
            rdoRecuringYes.Name = "rdoRecuringYes";
            rdoRecuringYes.Size = new System.Drawing.Size(96, 19);
            rdoRecuringYes.TabIndex = 1;
            rdoRecuringYes.Text = "Create copies";
            rdoRecuringYes.UseVisualStyleBackColor = true;
            // 
            // rdoRecuringNo
            // 
            rdoRecuringNo.AutoSize = true;
            rdoRecuringNo.Checked = true;
            rdoRecuringNo.Location = new System.Drawing.Point(8, 19);
            rdoRecuringNo.Name = "rdoRecuringNo";
            rdoRecuringNo.Size = new System.Drawing.Size(100, 19);
            rdoRecuringNo.TabIndex = 0;
            rdoRecuringNo.TabStop = true;
            rdoRecuringNo.Text = "One time only";
            rdoRecuringNo.UseVisualStyleBackColor = true;
            // 
            // dlgEditTransaction
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(362, 405);
            Controls.Add(lblAdvanced);
            Controls.Add(groupBox1);
            Controls.Add(pnlPaid);
            Controls.Add(txtDue);
            Controls.Add(label3);
            Controls.Add(cboCategory);
            Controls.Add(cboWallet);
            Controls.Add(label12);
            Controls.Add(label11);
            Controls.Add(dtDue);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(panel2);
            Controls.Add(label7);
            Controls.Add(panel1);
            Controls.Add(lblChanged);
            Controls.Add(label6);
            Controls.Add(lblCreated);
            Controls.Add(label4);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtName);
            Controls.Add(lblId);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "dlgEditTransaction";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Edit Transaction";
            Load += dlgEditTransaction_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            pnlPaid.ResumeLayout(false);
            pnlPaid.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)udRecuringCopies).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCreated;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblChanged;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rdoExpense;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rdoIncome;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rdoPaid;
        private System.Windows.Forms.RadioButton rdoUnpaid;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dtDue;
        private System.Windows.Forms.DateTimePicker dtPaid;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ComboBox cboWallet;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private Components.MoneyBox txtDue;
        private Components.MoneyBox txtPaid;
        private System.Windows.Forms.Panel pnlPaid;
        private System.Windows.Forms.Label lblAdvanced;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoRecuringNo;
        private System.Windows.Forms.RadioButton rdoRecuringYes;
        private System.Windows.Forms.ComboBox cboRecuringPeriod;
        private System.Windows.Forms.NumericUpDown udRecuringCopies;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton rdoReversed;
    }
}