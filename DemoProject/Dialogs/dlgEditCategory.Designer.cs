using System.Drawing;
using System.Windows.Forms;

namespace DemoProject.Dialogs
{
    partial class dlgEditCategory
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
            button2 = new Button();
            btnSave = new Button();
            txtName = new TextBox();
            lblId = new Label();
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            rdoExpense = new RadioButton();
            rdoIncome = new RadioButton();
            pnlType = new Panel();
            pnlType.SuspendLayout();
            SuspendLayout();
            // 
            // button2
            // 
            button2.DialogResult = DialogResult.Cancel;
            button2.Location = new Point(217, 116);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 11;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(90, 116);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 23);
            btnSave.TabIndex = 10;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtName
            // 
            txtName.Location = new Point(12, 77);
            txtName.MaxLength = 32;
            txtName.Name = "txtName";
            txtName.Size = new Size(358, 23);
            txtName.TabIndex = 9;
            // 
            // lblId
            // 
            lblId.AutoSize = true;
            lblId.Location = new Point(85, 9);
            lblId.Name = "lblId";
            lblId.Size = new Size(12, 15);
            lblId.TabIndex = 8;
            lblId.Text = "-";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(71, 15);
            label2.TabIndex = 7;
            label2.Text = "Category Id:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 59);
            label1.Name = "label1";
            label1.Size = new Size(93, 15);
            label1.TabIndex = 6;
            label1.Text = "Category Name:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 34);
            label3.Name = "label3";
            label3.Size = new Size(85, 15);
            label3.TabIndex = 12;
            label3.Text = "Category Type:";
            // 
            // rdoExpense
            // 
            rdoExpense.AutoSize = true;
            rdoExpense.Location = new Point(3, 3);
            rdoExpense.Name = "rdoExpense";
            rdoExpense.Size = new Size(68, 19);
            rdoExpense.TabIndex = 13;
            rdoExpense.TabStop = true;
            rdoExpense.Text = "Expense";
            rdoExpense.UseVisualStyleBackColor = true;
            // 
            // rdoIncome
            // 
            rdoIncome.AutoSize = true;
            rdoIncome.Location = new Point(77, 3);
            rdoIncome.Name = "rdoIncome";
            rdoIncome.Size = new Size(65, 19);
            rdoIncome.TabIndex = 14;
            rdoIncome.TabStop = true;
            rdoIncome.Text = "Income";
            rdoIncome.UseVisualStyleBackColor = true;
            // 
            // pnlType
            // 
            pnlType.Controls.Add(rdoExpense);
            pnlType.Controls.Add(rdoIncome);
            pnlType.Location = new Point(103, 32);
            pnlType.Name = "pnlType";
            pnlType.Size = new Size(171, 26);
            pnlType.TabIndex = 15;
            // 
            // dlgEditCategory
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(383, 151);
            Controls.Add(pnlType);
            Controls.Add(label3);
            Controls.Add(button2);
            Controls.Add(btnSave);
            Controls.Add(txtName);
            Controls.Add(lblId);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "dlgEditCategory";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Edit Category";
            pnlType.ResumeLayout(false);
            pnlType.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button2;
        private Button btnSave;
        private TextBox txtName;
        private Label lblId;
        private Label label2;
        private Label label1;
        private Label label3;
        private RadioButton rdoExpense;
        private RadioButton rdoIncome;
        private Panel pnlType;
    }
}