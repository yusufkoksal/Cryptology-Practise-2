namespace EsiCrypto3
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ComboBox comboBoxComponents;
        private TextBox textBoxX;
        private TextBox textBoxY;
        private Button btnAdd;

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
            this.comboBoxComponents = new ComboBox();
            this.textBoxX = new TextBox();
            this.textBoxY = new TextBox();
            this.btnAdd = new Button();
            this.SuspendLayout();
            // 
            // comboBoxComponents
            // 
            this.comboBoxComponents.FormattingEnabled = true;
            this.comboBoxComponents.Items.AddRange(new object[] {
            "Button",
            "Label",
            "DataGrid"});
            this.comboBoxComponents.Location = new System.Drawing.Point(12, 12);
            this.comboBoxComponents.Name = "comboBoxComponents";
            this.comboBoxComponents.Size = new System.Drawing.Size(121, 21);
            this.comboBoxComponents.TabIndex = 0;
            // 
            // textBoxX
            // 
            this.textBoxX.Location = new System.Drawing.Point(12, 39);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.Size = new System.Drawing.Size(100, 20);
            this.textBoxX.TabIndex = 1;
            this.textBoxX.PlaceholderText = "X";
            // 
            // textBoxY
            // 
            this.textBoxY.Location = new System.Drawing.Point(12, 65);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.Size = new System.Drawing.Size(100, 20);
            this.textBoxY.TabIndex = 2;
            this.textBoxY.PlaceholderText = "Y";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(12, 91);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Ekle";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.textBoxY);
            this.Controls.Add(this.textBoxX);
            this.Controls.Add(this.comboBoxComponents);
            this.Name = "Form2";
            this.Text = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();
        }


        #endregion
    }
}