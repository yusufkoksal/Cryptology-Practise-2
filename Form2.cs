using Aes256;

namespace EsiCrypto3
{
    public partial class Form2 : Form
    {
        private Form1 mainForm;
        private NumericUpDown columnCountInput;
        private FlowLayoutPanel columnPanel;
        private List<(TextBox Name, ComboBox Type)> columnInputs;

        public Form2(Form1 form1)
        {
            InitializeComponent();
            mainForm = form1;
            columnInputs = new List<(TextBox Name, ComboBox Type)>();
            InitializeDataGridComponents();
        }

        private void InitializeDataGridComponents()
        {
            // DataGrid seçeneğini ComboBox'a ekle (eğer yoksa)
            if (!comboBoxComponents.Items.Contains("DataGrid"))
            {
                comboBoxComponents.Items.Add("DataGrid");
            }

            // ComboBox seçim olayını dinle
            comboBoxComponents.SelectedIndexChanged += ComboBoxComponents_SelectedIndexChanged;
        }

        private void ComboBoxComponents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxComponents.SelectedItem?.ToString() == "DataGrid")
            {
                ShowDataGridOptions();
            }
            else
            {
                HideDataGridOptions();
            }
        }

        private void ShowDataGridOptions()
        {
            // Sütun sayısı seçici
            columnCountInput = new NumericUpDown
            {
                Location = new Point(20, 230), // btnAdd'den sonra
                Size = new Size(250, 25),
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };
            columnCountInput.ValueChanged += ColumnCount_ValueChanged;

            // Sütun özellikleri paneli
            columnPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 270), // columnCountInput'tan sonra
                Size = new Size(460, 280), // Form genişliğine uygun
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            this.Controls.Add(columnCountInput);
            this.Controls.Add(columnPanel);

            // İlk sütun input'unu oluştur
            CreateColumnInput();
        }
        private void HideDataGridOptions()
        {
            if (columnCountInput != null)
            {
                this.Controls.Remove(columnCountInput);
                columnCountInput.Dispose();
                columnCountInput = null;
            }

            if (columnPanel != null)
            {
                this.Controls.Remove(columnPanel);
                columnPanel.Dispose();
                columnPanel = null;
            }

            columnInputs.Clear();
        }

        private void CreateColumnInput()
        {
            Panel inputGroup = new Panel
            {
                Size = new Size(440, 35), // Panel genişliğine uygun
                Margin = new Padding(3)
            };

            TextBox nameInput = new TextBox
            {
                Location = new Point(5, 5),
                Size = new Size(200, 25),
                PlaceholderText = $"Sütun {columnInputs.Count + 1} Adı"
            };

            ComboBox typeInput = new ComboBox
            {
                Location = new Point(nameInput.Right + 10, 5),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            typeInput.Items.AddRange(new string[]
            {
            "Text",
            "Number",
            "Date",
            "Boolean",
            "Currency",
            "Encrypted" // AES256 şifrelenmiş metin için
            });
            typeInput.SelectedIndex = 0;

            inputGroup.Controls.Add(nameInput);
            inputGroup.Controls.Add(typeInput);
            columnPanel.Controls.Add(inputGroup);
            columnInputs.Add((nameInput, typeInput));
        }
        private void ColumnCount_ValueChanged(object sender, EventArgs e)
        {
            int currentCount = columnInputs.Count;
            int newCount = (int)columnCountInput.Value; if (newCount > currentCount)
            {
                // Yeni sütun inputları ekle
                for (int i = currentCount; i < newCount; i++)
                {
                    CreateColumnInput();
                }
            }
            else if (newCount < currentCount)
            {
                // Fazla sütun inputlarını kaldır
                for (int i = currentCount - 1; i >= newCount; i--)
                {
                    RemoveColumnInput(i);
                }
            }
        }

        private void RemoveColumnInput(int index)
        {
            if (index < columnPanel.Controls.Count)
            {
                columnPanel.Controls.RemoveAt(index);
                columnInputs.RemoveAt(index);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedComponent = comboBoxComponents.SelectedItem?.ToString();
                if (string.IsNullOrEmpty(selectedComponent))
                {
                    MessageBox.Show("Lütfen bir bileşen türü seçin.");
                    return;
                }

                if (!int.TryParse(textBoxX.Text, out int x) || !int.TryParse(textBoxY.Text, out int y))
                {
                    MessageBox.Show("Geçerli X ve Y koordinatları girin.");
                    return;
                }

                string name = componentName.Text;
                if (string.IsNullOrEmpty(name))
                {
                    MessageBox.Show("Lütfen bileşen adı girin.");
                    return;
                }

                Control control = null;
                switch (selectedComponent)
                {
                    case "Button":
                        control = new Button();
                        control.Text = name;
                        break;
                    case "Label":
                        control = new Label();
                        control.Text = name;
                        break;
                    case "DataGrid":
                        if (columnInputs.Count == 0)
                        {
                            MessageBox.Show("Lütfen en az bir sütun ekleyin!");
                            return;
                        }
                        control = CreateDataGridView(name);
                        break;
                }

                if (control != null)
                {
                    control.Name = name;
                    control.Location = new Point(x, y);
                    mainForm.AddControlToForm(control);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata oluştu: {ex.Message}");
            }
        }

        private DataGridView CreateDataGridView(string name)
        {
            DataGridView dataGrid = new DataGridView
            {
                Name = name,
                Size = new Size(600, 200),
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Sütunları ekle
            foreach (var (NameInput, TypeInput) in columnInputs)
            {
                if (string.IsNullOrWhiteSpace(NameInput.Text))
                {
                    throw new Exception("Sütun adı boş olamaz!");
                }

                DataGridViewColumn column = CreateColumn(NameInput.Text, TypeInput.SelectedItem.ToString());
                dataGrid.Columns.Add(column);
            }

            // CRUD butonlarını oluştur ve yerleştir
            Button addButton = new Button
            {
                Name = $"{name}_AddBtn",
                Text = "Ekle",
                Size = new Size(80, 30),
                Location = new Point(dataGrid.Right - 270, dataGrid.Bottom - 30)
            };

            Button updateButton = new Button
            {
                Name = $"{name}_UpdateBtn",
                Text = "Güncelle",
                Size = new Size(80, 30),
                Location = new Point(dataGrid.Right - 180, dataGrid.Bottom - 30)
            };

            Button deleteButton = new Button
            {
                Name = $"{name}_DeleteBtn",
                Text = "Sil",
                Size = new Size(80, 30),
                Location = new Point(dataGrid.Right - 90, dataGrid.Bottom - 30)
            };

            // Butonların click eventlerini ekle
            addButton.Click += (s, e) => AddRow(dataGrid);
            updateButton.Click += (s, e) => UpdateRow(dataGrid);
            deleteButton.Click += (s, e) => DeleteRow(dataGrid);

            // Butonları forma ekle
            mainForm.AddControlToForm(addButton);
            mainForm.AddControlToForm(updateButton);
            mainForm.AddControlToForm(deleteButton);

            return dataGrid;
        }

        private DataGridViewColumn CreateColumn(string name, string type)
        {
            DataGridViewColumn column;
            switch (type)
            {
                case "Number":
                    column = new DataGridViewTextBoxColumn
                    {
                        ValueType = typeof(double)
                    };
                    break;
                case "Date":
                    column = new DataGridViewTextBoxColumn
                    {
                        ValueType = typeof(DateTime)
                    };
                    break;
                case "Boolean":
                    column = new DataGridViewCheckBoxColumn();
                    break;
                case "Currency":
                    column = new DataGridViewTextBoxColumn
                    {
                        ValueType = typeof(decimal),
                        DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" }
                    };
                    break;
                case "Encrypted":
                    column = new DataGridViewTextBoxColumn
                    {
                        ValueType = typeof(string)
                    };
                    break;
                default: // Text
                    column = new DataGridViewTextBoxColumn();
                    break;
            }

            column.Name = name;
            column.HeaderText = name;
            return column;
        }

        private void AddRow(DataGridView dataGrid)
        {
            try
            {
                int rowIndex = dataGrid.Rows.Add();
                DataGridViewRow row = dataGrid.Rows[rowIndex];

                foreach (DataGridViewColumn column in dataGrid.Columns)
                {
                    string value = Microsoft.VisualBasic.Interaction.InputBox(
                        $"'{column.HeaderText}' için değer girin:",
                        "Veri Girişi",
                        "");

                    if (column.ValueType == typeof(string) && column.Name.Contains("Encrypted"))
                    {
                        value = value.Encrypt1(); // AES256 şifreleme
                    }

                    row.Cells[column.Index].Value = value;
                    ((Form1)mainForm).SaveGridData(dataGrid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satır eklenirken hata oluştu: {ex.Message}");
            }
        }

        private void UpdateRow(DataGridView dataGrid)
        {
            try
            {
                if (dataGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen güncellenecek satırı seçin!");
                    return;
                }

                DataGridViewRow row = dataGrid.SelectedRows[0];
                foreach (DataGridViewColumn column in dataGrid.Columns)
                {
                    string currentValue = row.Cells[column.Index].Value?.ToString() ?? "";
                    if (column.Name.Contains("Encrypted"))
                    {
                        currentValue = currentValue.Decrypt(); // Şifrelenmiş değeri göstermek için çöz
                    }

                    string value = Microsoft.VisualBasic.Interaction.InputBox(
                        $"'{column.HeaderText}' için yeni değer girin:",
                        "Veri Güncelleme",
                        currentValue);

                    if (column.Name.Contains("Encrypted"))
                    {
                        value = value.Encrypt1(); // Yeni değeri şifrele
                    }

                    row.Cells[column.Index].Value = value;
                    ((Form1)mainForm).SaveGridData(dataGrid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satır güncellenirken hata oluştu: {ex.Message}");
            }
        }

        private void DeleteRow(DataGridView dataGrid)
        {
            try
            {
                if (dataGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen silinecek satırı seçin!");
                    return;
                }

                if (MessageBox.Show("Seçili satırı silmek istediğinize emin misiniz?",
                                  "Silme Onayı",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dataGrid.Rows.RemoveAt(dataGrid.SelectedRows[0].Index);
                    ((Form1)mainForm).SaveGridData(dataGrid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satır silinirken hata oluştu: {ex.Message}");
            }
        }

    }
}
