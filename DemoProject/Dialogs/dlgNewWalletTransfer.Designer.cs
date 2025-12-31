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
            cboSourceCategory = new System.Windows.Forms.ComboBox();
            cboSourceWallet = new System.Windows.Forms.ComboBox();
            label12 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            dtDate = new System.Windows.Forms.DateTimePicker();
            label9 = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            label1 = new System.Windows.Forms.Label();
            cboDestinationCategory = new System.Windows.Forms.ComboBox();
            cboDestinationWallet = new System.Windows.Forms.ComboBox();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(195, 300);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 13;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(93, 300);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 12;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtValue
            // 
            txtValue.DecimalPlaces = 2;
            txtValue.Location = new System.Drawing.Point(252, 201);
            txtValue.MoneySign = "";
            txtValue.Name = "txtValue";
            txtValue.Size = new System.Drawing.Size(101, 23);
            txtValue.TabIndex = 33;
            txtValue.Text = "0,00";
            txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            txtValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(213, 205);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(38, 15);
            label3.TabIndex = 38;
            label3.Text = "Value:";
            // 
            // cboSourceCategory
            // 
            cboSourceCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboSourceCategory.FormattingEnabled = true;
            cboSourceCategory.Location = new System.Drawing.Point(134, 62);
            cboSourceCategory.Name = "cboSourceCategory";
            cboSourceCategory.Size = new System.Drawing.Size(219, 23);
            cboSourceCategory.TabIndex = 30;
            // 
            // cboSourceWallet
            // 
            cboSourceWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboSourceWallet.FormattingEnabled = true;
            cboSourceWallet.Location = new System.Drawing.Point(134, 33);
            cboSourceWallet.Name = "cboSourceWallet";
            cboSourceWallet.Size = new System.Drawing.Size(219, 23);
            cboSourceWallet.TabIndex = 29;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(7, 65);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(97, 15);
            label12.TabIndex = 37;
            label12.Text = "Source Cateogry:";
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
            dtDate.Location = new System.Drawing.Point(48, 201);
            dtDate.MaxDate = new System.DateTime(2100, 12, 31, 0, 0, 0, 0);
            dtDate.MinDate = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            dtDate.Name = "dtDate";
            dtDate.Size = new System.Drawing.Size(116, 23);
            dtDate.TabIndex = 31;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(8, 205);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(34, 15);
            label9.TabIndex = 35;
            label9.Text = "Date:";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(8, 264);
            txtName.MaxLength = 64;
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(345, 23);
            txtName.TabIndex = 34;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 245);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(130, 15);
            label1.TabIndex = 32;
            label1.Text = "Transaction Description";
            // 
            // cboDestinationCategory
            // 
            cboDestinationCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDestinationCategory.FormattingEnabled = true;
            cboDestinationCategory.Location = new System.Drawing.Point(134, 148);
            cboDestinationCategory.Name = "cboDestinationCategory";
            cboDestinationCategory.Size = new System.Drawing.Size(219, 23);
            cboDestinationCategory.TabIndex = 40;
            // 
            // cboDestinationWallet
            // 
            cboDestinationWallet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cboDestinationWallet.FormattingEnabled = true;
            cboDestinationWallet.Location = new System.Drawing.Point(134, 119);
            cboDestinationWallet.Name = "cboDestinationWallet";
            cboDestinationWallet.Size = new System.Drawing.Size(219, 23);
            cboDestinationWallet.TabIndex = 39;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(7, 151);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(121, 15);
            label2.TabIndex = 42;
            label2.Text = "Destination Cateogry:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 122);
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
            label6.Location = new System.Drawing.Point(7, 98);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(126, 15);
            label6.TabIndex = 44;
            label6.Text = "Destination (inbound):";
            // 
            // dlgNewWalletTransfer
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(362, 333);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(cboDestinationCategory);
            Controls.Add(cboDestinationWallet);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(txtValue);
            Controls.Add(label3);
            Controls.Add(cboSourceCategory);
            Controls.Add(cboSourceWallet);
            Controls.Add(label12);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private Components.MoneyBox txtValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboSourceCategory;
        private System.Windows.Forms.ComboBox cboSourceWallet;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtDate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboDestinationCategory;
        private System.Windows.Forms.ComboBox cboDestinationWallet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}