namespace EsiCrypto3
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Form elemanlarını tanımla
        private ComboBox comboBoxComponents;
        private TextBox textBoxX;
        private TextBox textBoxY;
        private TextBox componentName;
        private Button btnAdd;
        private TextBox textBoxWidth;
        private TextBox textBoxHeight;


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
            comboBoxComponents = new ComboBox();
            textBoxX = new TextBox();
            textBoxY = new TextBox();
            componentName = new TextBox();
            btnAdd = new Button();
            textBoxWidth = new TextBox();
            textBoxHeight = new TextBox();
            SuspendLayout();
            // 
            // comboBoxComponents
            // 
            comboBoxComponents.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxComponents.FormattingEnabled = true;
            comboBoxComponents.Items.AddRange(new object[] { "Button", "Label", "DataGrid" });
            comboBoxComponents.Location = new Point(23, 31);
            comboBoxComponents.Margin = new Padding(3, 4, 3, 4);
            comboBoxComponents.Name = "comboBoxComponents";
            comboBoxComponents.Size = new Size(285, 28);
            comboBoxComponents.TabIndex = 0;
            // 
            // textBoxX
            // 
            textBoxX.Location = new Point(23, 80);
            textBoxX.Margin = new Padding(3, 4, 3, 4);
            textBoxX.Name = "textBoxX";
            textBoxX.PlaceholderText = "X Koordinatı";
            textBoxX.Size = new Size(285, 27);
            textBoxX.TabIndex = 1;
            // 
            // textBoxY
            // 
            textBoxY.Location = new Point(23, 132);
            textBoxY.Margin = new Padding(3, 4, 3, 4);
            textBoxY.Name = "textBoxY";
            textBoxY.PlaceholderText = "Y Koordinatı";
            textBoxY.Size = new Size(285, 27);
            textBoxY.TabIndex = 2;
            // 
            // componentName
            // 
            componentName.Location = new Point(23, 280);
            componentName.Margin = new Padding(3, 4, 3, 4);
            componentName.Name = "componentName";
            componentName.PlaceholderText = "Component İsmi";
            componentName.Size = new Size(285, 27);
            componentName.TabIndex = 6;
            // 
            // btnAdd
            // 
            btnAdd.Location = new Point(270, 694);
            btnAdd.Margin = new Padding(3, 4, 3, 4);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(286, 40);
            btnAdd.TabIndex = 7;
            btnAdd.Text = "Ekle";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // textBoxWidth
            // 
            textBoxWidth.Location = new Point(23, 180);
            textBoxWidth.Name = "textBoxWidth";
            textBoxWidth.PlaceholderText = "Genişlik";
            textBoxWidth.Size = new Size(285, 27);
            textBoxWidth.TabIndex = 4;
            // 
            // textBoxHeight
            // 
            textBoxHeight.Location = new Point(23, 230);
            textBoxHeight.Name = "textBoxHeight";
            textBoxHeight.PlaceholderText = "Yükseklik";
            textBoxHeight.Size = new Size(285, 27);
            textBoxHeight.TabIndex = 5;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(571, 800);
            Controls.Add(textBoxHeight);
            Controls.Add(textBoxWidth);
            Controls.Add(comboBoxComponents);
            Controls.Add(textBoxX);
            Controls.Add(textBoxY);
            Controls.Add(componentName);
            Controls.Add(btnAdd);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "Form2";
            Padding = new Padding(23, 27, 23, 27);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Component Ekle";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion


    }
}