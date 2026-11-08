using System.Drawing;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    partial class dlgEditWallet
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
            label1 = new Label();
            label2 = new Label();
            lblId = new Label();
            txtName = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            txtCurrencyCode = new TextBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 37);
            label1.Name = "label1";
            label1.Size = new Size(78, 15);
            label1.TabIndex = 0;
            label1.Text = "Wallet Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(56, 15);
            label2.TabIndex = 1;
            label2.Text = "Wallet Id:";
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(74, 9);
            lblId.Name = "lblId";
            lblId.Size = new Size(12, 15);
            lblId.TabIndex = 0;
            lblId.Text = "-";
            // 
            // txtName
            // 
            txtName.Location = new Point(12, 55);
            txtName.MaxLength = 32;
            txtName.Name = "txtName";
            txtName.Size = new Size(358, 23);
            txtName.TabIndex = 1;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(91, 127);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(218, 127);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // txtCurrencyCode
            // 
            txtCurrencyCode.Location = new Point(143, 87);
            txtCurrencyCode.MaxLength = 3;
            txtCurrencyCode.Name = "txtCurrencyCode";
            txtCurrencyCode.Size = new Size(64, 23);
            txtCurrencyCode.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 90);
            label3.Name = "label3";
            label3.Size = new Size(125, 15);
            label3.TabIndex = 4;
            label3.Text = "Wallet Currency Code:";
            // 
            // dlgEditWallet
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(385, 160);
            Controls.Add(txtCurrencyCode);
            Controls.Add(label3);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtName);
            Controls.Add(lblId);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "dlgEditWallet";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Edit Wallet";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label lblId;
        private TextBox txtName;
        private Button btnSave;
        private Button btnCancel;
        private TextBox txtCurrencyCode;
        private Label label3;
    }
}