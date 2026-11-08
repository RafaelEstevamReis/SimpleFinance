namespace DemoProject.Dialogs
{
    partial class dlgValueBox
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
            lblTitle = new System.Windows.Forms.Label();
            btnCancel = new System.Windows.Forms.Button();
            btnSave = new System.Windows.Forms.Button();
            txtMoneybox = new DemoProject.Components.MoneyBox();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new System.Drawing.Point(5, 4);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(38, 15);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "label1";
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            btnCancel.Location = new System.Drawing.Point(168, 65);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new System.Drawing.Point(41, 65);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(75, 23);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtMoneybox
            // 
            txtMoneybox.DecimalPlaces = 2;
            txtMoneybox.Location = new System.Drawing.Point(92, 34);
            txtMoneybox.MoneySign = "";
            txtMoneybox.Name = "txtMoneybox";
            txtMoneybox.Size = new System.Drawing.Size(100, 23);
            txtMoneybox.TabIndex = 0;
            txtMoneybox.Text = "0,00";
            txtMoneybox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtMoneybox.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // dlgValueBox
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(284, 97);
            Controls.Add(txtMoneybox);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(lblTitle);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Name = "dlgValueBox";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "Select Value";
            KeyUp += dlgValueBox_KeyUp;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;
        private Components.MoneyBox txtMoneybox;
    }
}