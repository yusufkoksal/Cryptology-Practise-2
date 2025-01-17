namespace EsiCrypto3
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnEdit = new Button();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            SuspendLayout();
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(722, 45);
            btnEdit.Margin = new Padding(4, 5, 4, 5);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(100, 35);
            btnEdit.TabIndex = 0;
            btnEdit.Text = "Düzenle";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            // 
            // button1
            // 
            button1.Location = new Point(722, 105);
            button1.Name = "button1";
            button1.Size = new Size(100, 36);
            button1.TabIndex = 1;
            button1.Text = "Reset";
            button1.UseVisualStyleBackColor = true;
            button1.Click += resetButtonClick;
            // 
            // button2
            // 
            button2.Location = new Point(722, 167);
            button2.Name = "button2";
            button2.Size = new Size(100, 36);
            button2.TabIndex = 2;
            button2.Text = "Ekle";
            button2.UseVisualStyleBackColor = true;
            button2.Click += addButton;
            // 
            // button3
            // 
            button3.Location = new Point(722, 231);
            button3.Name = "button3";
            button3.Size = new Size(100, 36);
            button3.TabIndex = 3;
            button3.Text = "Sil";
            button3.UseVisualStyleBackColor = true;
            button3.Click += deleteButton;
            // 
            // button4
            // 
            button4.Location = new Point(722, 297);
            button4.Name = "button4";
            button4.Size = new Size(100, 36);
            button4.TabIndex = 4;
            button4.Text = "Update";
            button4.UseVisualStyleBackColor = true;
            button4.Click += updateButton;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(872, 610);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(btnEdit);
            Margin = new Padding(4, 5, 4, 5);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnEdit;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
    }
}
