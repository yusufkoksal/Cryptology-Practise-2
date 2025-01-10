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
                Location = new Point(20, 230),
                Size = new Size(250, 25),
                Minimum = 1,
                Maximum = 10,
                Value = 1
            };
            columnCountInput.ValueChanged += ColumnCount_ValueChanged;

            // Sütun özellikleri paneli
            columnPanel = new FlowLayoutPanel
            {
                Location = new Point(20, 270),
                Size = new Size(460, 280),
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
                Size = new Size(440, 35),
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
            "Encrypted"
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
                CreateCrudButtons(dataGrid);
            }


            return dataGrid;
        }

        private void CreateCrudButtons(DataGridView dataGrid)
        {
            // Add Button
            Button addButton = new Button
            {
                Name = $"{dataGrid.Name}_AddBtn",
                Text = "Ekle",
                Size = new Size(80, 30),
                Location = new Point(dataGrid.Right - 270, dataGrid.Bottom - 30)
            };
            addButton.Click += (s, e) => AddRow(dataGrid);

            // Update Button
            Button updateButton = new Button
            {
                Name = $"{dataGrid.Name}_UpdateBtn",
                Text = "Güncelle",
                Size = new Size(80, 30),
                Location = new Point(dataGrid.Right - 180, dataGrid.Bottom - 30)
            };
            updateButton.Click += (s, e) => UpdateRow(dataGrid);

            // Delete Button
            Button deleteButton = new Button
            {
                Name = $"{dataGrid.Name}_DeleteBtn",
                Text = "Sil",
                Size = new Size(80, 30),
                Location = new Point(dataGrid.Right - 90, dataGrid.Bottom - 30)
            };
            deleteButton.Click += (s, e) => DeleteRow(dataGrid);

            // Butonları forma ekle
            this.Controls.Add(addButton);
            this.Controls.Add(updateButton);
            this.Controls.Add(deleteButton);

            // Butonları forma ekle
            mainForm.AddControlToForm(addButton);
            mainForm.AddControlToForm(updateButton);
            mainForm.AddControlToForm(deleteButton);
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

        public void AddRow(DataGridView dataGrid)
        {
            try
            {
                using (var entryForm = new DataEntryForm(dataGrid, mainForm))
                {
                    if (entryForm.ShowDialog() == DialogResult.OK)
                    {
                        ((Form1)mainForm).SaveGridData(dataGrid);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satır eklenirken hata oluştu: {ex.Message}");
            }
        }

        public void UpdateRow(DataGridView dataGrid)
        {
            try
            {
                if (dataGrid.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen güncellenecek satırı seçin!");
                    return;
                }

                using (var updateForm = new Form())
                {
                    updateForm.Text = "Veri Güncelleme";
                    updateForm.Size = new Size(400, (dataGrid.Columns.Count * 40) + 150);
                    updateForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                    updateForm.StartPosition = FormStartPosition.CenterScreen;
                    updateForm.MaximizeBox = false;

                    int currentY = 20;
                    Dictionary<string, TextBox> textBoxes = new Dictionary<string, TextBox>();
                    DataGridViewRow selectedRow = dataGrid.SelectedRows[0];

                    // Her sütun için Label ve TextBox oluştur
                    foreach (DataGridViewColumn column in dataGrid.Columns)
                    {
                        Label label = new Label
                        {
                            Text = column.HeaderText,
                            Location = new Point(20, currentY),
                            Size = new Size(120, 23)
                        };

                        TextBox textBox = new TextBox
                        {
                            Location = new Point(150, currentY),
                            Size = new Size(200, 23)
                        };

                        // Mevcut değeri TextBox'a yerleştir
                        string currentValue = selectedRow.Cells[column.Index].Value?.ToString() ?? "";
                        if (column.Name.Contains("Encrypted"))
                        {
                            try
                            {
                                currentValue = currentValue.Decrypt();
                            }
                            catch { /* Şifre çözme hatası durumunda boş bırak */ }
                        }
                        textBox.Text = currentValue;

                        updateForm.Controls.Add(label);
                        updateForm.Controls.Add(textBox);
                        textBoxes.Add(column.Name, textBox);

                        currentY += 35;
                    }

                    // Tarih ve kullanıcı bilgisi ekle
                    Label dateTimeLabel = new Label
                    {
                        Text = $"Tarih: 2025-01-10 13:48:28",
                        Location = new Point(20, currentY),
                        Size = new Size(330, 23)
                    };
                    updateForm.Controls.Add(dateTimeLabel);
                    currentY += 25;

                    Label userLabel = new Label
                    {
                        Text = $"Kullanıcı: yusufkoksal",
                        Location = new Point(20, currentY),
                        Size = new Size(330, 23)
                    };
                    updateForm.Controls.Add(userLabel);
                    currentY += 35;

                    // Güncelle butonu
                    Button updateButton = new Button
                    {
                        Text = "Güncelle",
                        Location = new Point(150, currentY),
                        Size = new Size(90, 30),
                        DialogResult = DialogResult.OK
                    };
                    updateButton.Click += (sender, e) =>
                    {
                        try
                        {
                            foreach (DataGridViewColumn column in dataGrid.Columns)
                            {
                                string value = textBoxes[column.Name].Text;

                                if (column.Name.Contains("Encrypted"))
                                {
                                    value = value.Encrypt1();
                                }

                                selectedRow.Cells[column.Index].Value = value;
                            }

                            ((Form1)mainForm).SaveGridData(dataGrid);
                            updateForm.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Güncelleme sırasında hata oluştu: {ex.Message}");
                        }
                    };

                    // İptal butonu
                    Button cancelButton = new Button
                    {
                        Text = "İptal",
                        Location = new Point(260, currentY),
                        Size = new Size(90, 30),
                        DialogResult = DialogResult.Cancel
                    };

                    updateForm.Controls.Add(updateButton);
                    updateForm.Controls.Add(cancelButton);

                    updateForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Satır güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public void DeleteRow(DataGridView dataGrid)
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
