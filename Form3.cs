using EsiCrypto3;

public class GridDesignerForm : Form
{
    private Form1 mainForm;
    private NumericUpDown columnCountInput;
    private FlowLayoutPanel columnPanel;
    private List<(TextBox Name, ComboBox Type)> columnInputs;
    private DataGridView targetDataGrid;
    private Button btnApply;

    public GridDesignerForm(Form1 mainForm, DataGridView dataGrid)
    {
        this.mainForm = mainForm;
        this.targetDataGrid = dataGrid;
        this.columnInputs = new List<(TextBox Name, ComboBox Type)>();
        InitializeComponents();
        LoadExistingColumns();
    }

    private void InitializeComponents()
    {
        this.Text = "DataGrid Düzenleyici";
        this.Size = new Size(500, 700);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MaximizeBox = false;


        Label lblColumnCount = new Label
        {
            Text = "Sütun Sayısı:",
            Location = new Point(20, 20),
            Size = new Size(100, 25)
        };

        columnCountInput = new NumericUpDown
        {
            Location = new Point(20, 45),
            Size = new Size(250, 25),
            Minimum = 1,
            Maximum = 10,
            Value = targetDataGrid.Columns.Count
        };
        columnCountInput.ValueChanged += ColumnCount_ValueChanged;


        columnPanel = new FlowLayoutPanel
        {
            Location = new Point(20, 80),
            Size = new Size(440, 520),
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(5)
        };

        btnApply = new Button
        {
            Text = "Uygula ve Kapat",
            Location = new Point(20, 610),
            Size = new Size(440, 30)
        };
        btnApply.Click += BtnApply_Click;

        this.Controls.Add(lblColumnCount);
        this.Controls.Add(columnCountInput);
        this.Controls.Add(columnPanel);
        this.Controls.Add(btnApply);
    }

    private void LoadExistingColumns()
    {
        foreach (DataGridViewColumn col in targetDataGrid.Columns)
        {
            CreateColumnInput(col.Name, GetColumnType(col));
        }
    }

    private string GetColumnType(DataGridViewColumn col)
    {
        if (col is DataGridViewCheckBoxColumn)
            return "Boolean";
        else if (col.ValueType == typeof(DateTime))
            return "Date";
        else if (col.ValueType == typeof(double))
            return "Number";
        else if (col.ValueType == typeof(decimal))
            return "Currency";
        else if (col.Name.Contains("Encrypted"))
            return "Encrypted";
        else
            return "Text";
    }

    private void CreateColumnInput(string name = "", string selectedType = "Text")
    {
        Panel inputGroup = new Panel
        {
            Size = new Size(420, 35),
            Margin = new Padding(3)
        };

        TextBox nameInput = new TextBox
        {
            Location = new Point(5, 5),
            Size = new Size(200, 25),
            Text = name,
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
        typeInput.SelectedItem = selectedType;

        inputGroup.Controls.Add(nameInput);
        inputGroup.Controls.Add(typeInput);
        columnPanel.Controls.Add(inputGroup);
        columnInputs.Add((nameInput, typeInput));
    }

    private void ColumnCount_ValueChanged(object sender, EventArgs e)
    {
        int currentCount = columnInputs.Count;
        int newCount = (int)columnCountInput.Value;

        if (newCount > currentCount)
        {
            for (int i = currentCount; i < newCount; i++)
            {
                CreateColumnInput();
            }
        }
        else if (newCount < currentCount)
        {
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

    private void BtnApply_Click(object sender, EventArgs e)
    {
        try
        {
            // Mevcut sütunları temizle
            targetDataGrid.Columns.Clear();

            // Yeni sütunları ekle
            foreach (var (NameInput, TypeInput) in columnInputs)
            {
                if (string.IsNullOrWhiteSpace(NameInput.Text))
                {
                    throw new Exception("Sütun adı boş olamaz!");
                }

                DataGridViewColumn column = CreateColumn(NameInput.Text, TypeInput.SelectedItem.ToString());
                targetDataGrid.Columns.Add(column);
            }

            // Ana formdaki verileri kaydet
            mainForm.SaveGridData(targetDataGrid);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
}