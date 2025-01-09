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
            components = new System.ComponentModel.Container();

            // ComboBox ayarları
            comboBoxComponents = new ComboBox();
            comboBoxComponents.FormattingEnabled = true;
            comboBoxComponents.Items.AddRange(new object[] { "Button", "Label", "DataGrid" });
            comboBoxComponents.Location = new Point(20, 20);
            comboBoxComponents.Name = "comboBoxComponents";
            comboBoxComponents.Size = new Size(250, 23);
            comboBoxComponents.TabIndex = 0;
            comboBoxComponents.DropDownStyle = ComboBoxStyle.DropDownList;

            // X koordinatı için TextBox
            textBoxX = new TextBox();
            textBoxX.Location = new Point(20, 60);
            textBoxX.Name = "textBoxX";
            textBoxX.PlaceholderText = "X Koordinatı";
            textBoxX.Size = new Size(250, 23);
            textBoxX.TabIndex = 1;

            // Y koordinatı için TextBox
            textBoxY = new TextBox();
            textBoxY.Location = new Point(20, 100);
            textBoxY.Name = "textBoxY";
            textBoxY.PlaceholderText = "Y Koordinatı";
            textBoxY.Size = new Size(250, 23);
            textBoxY.TabIndex = 2;

            // Component ismi için TextBox
            componentName = new TextBox();
            componentName.Location = new Point(20, 140);
            componentName.Name = "componentName";
            componentName.PlaceholderText = "Component İsmi";
            componentName.Size = new Size(250, 23);
            componentName.TabIndex = 3;

            // Ekle butonu
            btnAdd = new Button();
            btnAdd.Location = new Point(20, 180);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(250, 30);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "Ekle";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += new EventHandler(btnAdd_Click);

            // Form ayarları
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(500, 600); // Form boyutunu büyüttük
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Name = "Form2";
            this.Text = "Component Ekle";
            this.Padding = new Padding(20);

            // Kontrolleri forma ekle
            this.Controls.AddRange(new Control[] {
        comboBoxComponents,
        textBoxX,
        textBoxY,
        componentName,
        btnAdd
    });

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}