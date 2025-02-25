using System.Globalization;
using System.Text.Json;
using Aes256;
using EsiCrypto3;

namespace EsiCrypto3
{
    public partial class Form1 : Form
    {
        private const string COMPONENTS_FILE = @"C:\Users\ysf20\source\repos\EsiCrypto3\EsiCrypto3\components.json";
        private const string GRID_DATA_FILE = @"C:\Users\ysf20\source\repos\EsiCrypto3\EsiCrypto3\griddata.json";
        private Dictionary<string, DataGridData> gridDataDict = new Dictionary<string, DataGridData>();
        private Form2 form2;

        public Form1()
        {
            InitializeComponent();
            form2 = new Form2(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadComponentsFromFile();
            LoadGridData();
        }

        public void AddControlToForm(Control control)
        {
            this.Controls.Add(control);

            if (control is DataGridView dataGrid)
            {
                // DataGrid verilerini kaydet
                SaveGridData(dataGrid);

                // DataGrid'in FormClosing eventinde verileri kaydet
                this.FormClosing += (s, e) => SaveGridData(dataGrid);
            }

            // T�m komponentleri kaydet
            SaveComponentsToFile();
        }

        public void SaveGridData(DataGridView dataGrid)
        {
            try
            {
                string gridName = dataGrid.Name;

                var gridData = new DataGridData
                {
                    GridName = gridName,
                    Columns = new List<ColumnInfo>(),
                    Rows = new List<RowData>()
                };

                // S�tun bilgilerini kaydet
                foreach (DataGridViewColumn column in dataGrid.Columns)
                {
                    gridData.Columns.Add(new ColumnInfo
                    {
                        Name = column.Name,
                        Type = column.GetType().Name
                    });
                }

                // Sat�r verilerini kaydet
                foreach (DataGridViewRow row in dataGrid.Rows)
                {
                    if (row.IsNewRow) continue;

                    var rowData = new RowData
                    {
                        Values = new Dictionary<string, string>()
                    };

                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        string value = cell.Value?.ToString() ?? "";
                        rowData.Values[dataGrid.Columns[cell.ColumnIndex].Name] = value;
                    }

                    gridData.Rows.Add(rowData);
                }

                // T�m verileri �ifrele
                gridData.EncryptData();

                // S�zl��e ekle veya g�ncelle
                gridDataDict[gridName] = gridData;

                // JSON'a �evir ve kaydet
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(gridDataDict, options);
                File.WriteAllText(GRID_DATA_FILE, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Veri kaydedilirken hata olu�tu: {ex.Message}");
            }
        }

        private void LoadGridData()
        {
            try
            {
                if (File.Exists(GRID_DATA_FILE))
                {
                    string json = File.ReadAllText(GRID_DATA_FILE);

                    if (string.IsNullOrEmpty(json))
                    {
                        gridDataDict = new Dictionary<string, DataGridData>();
                        return;
                    }

                    try
                    {
                        gridDataDict = JsonSerializer.Deserialize<Dictionary<string, DataGridData>>(json);

                        foreach (var gridData in gridDataDict.Values)
                        {
                            try
                            {
                                // �nce grid datas�n� ��z
                                gridData.DecryptData();

                                // DataGridView'i bul - art�k ��z�lm�� GridName ile arama yap�yoruz
                                var dataGrid = this.Controls.OfType<DataGridView>()
                                                 .FirstOrDefault(dg => dg.Name == gridData.GridName);

                                if (dataGrid != null)
                                {
                                    // S�tunlar� ekle
                                    dataGrid.Columns.Clear();
                                    foreach (var columnInfo in gridData.Columns)
                                    {
                                        try
                                        {
                                            // columnInfo zaten ��z�lm�� durumda
                                            DataGridViewColumn column = CreateColumn(columnInfo.Name, columnInfo.Type);
                                            dataGrid.Columns.Add(column);
                                        }
                                        catch (Exception columnEx)
                                        {
                                            MessageBox.Show($"S�tun y�klenirken hata: {columnEx.Message}");
                                            continue;
                                        }
                                    }

                                    // Verileri ekle
                                    foreach (var rowData in gridData.Rows)
                                    {
                                        try
                                        {
                                            int rowIndex = dataGrid.Rows.Add();
                                            foreach (var value in rowData.Values)
                                            {
                                                try
                                                {
                                                    // Value'lar zaten ��z�lm�� durumda
                                                    dataGrid.Rows[rowIndex].Cells[value.Key].Value = value.Value;
                                                }
                                                catch (Exception cellEx)
                                                {
                                                    MessageBox.Show($"H�cre verisi y�klenirken hata: {cellEx.Message}");
                                                    continue;
                                                }
                                            }
                                        }
                                        catch (Exception rowEx)
                                        {
                                            MessageBox.Show($"Sat�r y�klenirken hata: {rowEx.Message}");
                                            continue;
                                        }
                                    }
                                }
                            }
                            catch (Exception gridEx)
                            {
                                MessageBox.Show($"Grid verisi ��z�l�rken hata: {gridEx.Message}");
                                continue;
                            }
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        MessageBox.Show($"JSON format� ge�ersiz: {jsonEx.Message}");
                        gridDataDict = new Dictionary<string, DataGridData>();
                    }
                }
                else
                {
                    gridDataDict = new Dictionary<string, DataGridData>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Grid verileri y�klenirken hata olu�tu: {ex.Message}");
                gridDataDict = new Dictionary<string, DataGridData>();
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

        private void SaveComponentsToFile()
        {
            try
            {
                List<ComponentInfo> components = new List<ComponentInfo>();

                foreach (Control control in this.Controls)
                {
                    if (control is Button || control is Label || control is DataGridView)
                    {
                        ComponentInfo info = new ComponentInfo
                        {
                            Type = control.GetType().Name,
                            Name = control.Name,
                            Text = control.Text,
                            X = control.Location.X,
                            Y = control.Location.Y,
                            IsDataGrid = control is DataGridView,
                            Columns = control is DataGridView ?
                            ((DataGridView)control).Columns.Cast<DataGridViewColumn>()
                            .Select(c => c.Name).ToList() :
                            null
                        };

                        // Verileri �ifrele
                        info.EncryptData();
                        components.Add(info);
                    }
                }

                string json = JsonSerializer.Serialize(components, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(COMPONENTS_FILE, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bile�enler kaydedilirken hata olu�tu: {ex.Message}");
            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(this);
            form2.Show();
        }



        private void LoadComponentsFromFile()
        {
            try
            {
                if (File.Exists(COMPONENTS_FILE))
                {
                    string json = File.ReadAllText(COMPONENTS_FILE);
                    List<ComponentInfo> components = JsonSerializer.Deserialize<List<ComponentInfo>>(json);

                    foreach (var info in components)
                    {
                        try
                        {
                            info.DecryptData();
                            Control control = null;

                            switch (info.Type)
                            {
                                case "Button":
                                    control = new Button();
                                    if (info.Text == "D�zenle" || info.Text == "G�ncelle")
                                    {
                                        control.Click += (s, e) =>
                                        {

                                            string gridName = info.Name.Split('_')[0];

                                            var dataGrid = Controls.OfType<DataGridView>()
                                                .FirstOrDefault(dg => dg.Name == gridName);
                                            if (dataGrid != null)
                                            {
                                                form2.UpdateRow(dataGrid);
                                            }
                                        };
                                    }
                                    else if (info.Text == "Sil")
                                    {
                                        control.Click += (s, e) =>
                                        {
                                            string gridName = info.Name.Split('_')[0];
                                            var dataGrid = Controls.OfType<DataGridView>()
                                                .FirstOrDefault(dg => dg.Name == gridName);
                                            if (dataGrid != null)
                                            {
                                                form2.DeleteRow(dataGrid);
                                            }
                                        };
                                    }
                                    else if (info.Text == "Ekle")
                                    {
                                        control.Click += (s, e) =>
                                        {
                                            string gridName = info.Name.Split('_')[0];
                                            var dataGrid = Controls.OfType<DataGridView>()
                                                .FirstOrDefault(dg => dg.Name == gridName);
                                            if (dataGrid != null)
                                            {
                                                form2.AddRow(dataGrid);
                                            }
                                        };
                                    }
                                    break;

                                case "Label":
                                    control = new Label();
                                    break;
                                case "DataGridView":
                                    var dataGrid = new DataGridView
                                    {
                                        Name = info.Name,
                                        Location = new Point(info.X, info.Y),
                                        Size = new Size(600, 300),
                                        AllowUserToAddRows = true,
                                        AllowUserToDeleteRows = true,
                                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                                    };
                                    control = dataGrid;


                                    break;
                            }

                            if (control != null)
                            {
                                control.Name = info.Name;
                                control.Text = info.Text;
                                control.Location = new Point(info.X, info.Y);
                                this.Controls.Add(control);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Bile�en y�klenirken hata olu�tu: {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya okuma hatas�: {ex.Message}");
            }
        }



        public Form2 Form2Instance
        {
            get
            {
                if (form2 == null)
                {
                    form2 = new Form2(this);
                }
                return form2;
            }
        }


        private void resetButtonClick(object sender, EventArgs e)
        {
            if (File.Exists(COMPONENTS_FILE))
            {
                File.WriteAllText(COMPONENTS_FILE, "[]");
            }
        }

        private void addButton(object sender, EventArgs e)
        {
            var dataGrid = this.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dataGrid != null)
            {
                form2.AddRow(dataGrid);
            }
        }

        private void deleteButton(object sender, EventArgs e)
        {
            var dataGrid = this.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dataGrid != null)
            {
                form2.DeleteRow(dataGrid);
            }
        }

        private void updateButton(object sender, EventArgs e)
        {
            var dataGrid = this.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dataGrid != null)
            {
                form2.UpdateRow(dataGrid);
            }
        }

        private void btnEditDataGrid_Click(object sender, EventArgs e)
        {
            var dataGrid = this.Controls.OfType<DataGridView>().FirstOrDefault();
            if (dataGrid != null)
            {
                ShowGridDesigner(dataGrid);
            }
        }

        private void ShowGridDesigner(DataGridView dataGrid)
        {
            using (var designerForm = new GridDesignerForm(this, dataGrid))
            {
                designerForm.ShowDialog();
            }
        }
    }


}

public class DataEntryForm : Form
{
    private Dictionary<string, TextBox> textBoxes;
    private DataGridView targetDataGrid;
    private Form1 mainForm;

    public DataEntryForm(DataGridView dataGrid, Form1 form1)
    {
        targetDataGrid = dataGrid;
        mainForm = form1;
        textBoxes = new Dictionary<string, TextBox>();
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Veri Giri�i";
        this.Size = new Size(400, (targetDataGrid.Columns.Count * 40) + 150);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MaximizeBox = false;

        int currentY = 20;


        foreach (DataGridViewColumn column in targetDataGrid.Columns)
        {
            Label label = new Label
            {
                Text = column.HeaderText,
                Location = new Point(20, currentY),
                Size = new Size(120, 23)
            };

            TextBox textBox = new TextBox
            {
                Name = column.Name,
                Location = new Point(150, currentY),
                Size = new Size(200, 23),
                Tag = column.ValueType
            };


            string tipUyarisi = GetDataTypeHint(column);
            textBox.PlaceholderText = tipUyarisi;


            textBox.Leave += TextBox_Leave;

            this.Controls.Add(label);
            this.Controls.Add(textBox);
            textBoxes.Add(column.Name, textBox);

            currentY += 35;
        }


        Label dateTimeLabel = new Label
        {
            Text = $"Tarih: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
            Location = new Point(20, currentY),
            Size = new Size(330, 23)
        };
        this.Controls.Add(dateTimeLabel);
        currentY += 25;

        Label userLabel = new Label
        {
            Text = $"Kullan�c�: yusufkoksal",
            Location = new Point(20, currentY),
            Size = new Size(330, 23)
        };
        this.Controls.Add(userLabel);
        currentY += 35;

        Button saveButton = new Button
        {
            Text = "Kaydet",
            Location = new Point(150, currentY),
            Size = new Size(90, 30)
        };
        saveButton.Click += SaveButton_Click;


        Button cancelButton = new Button
        {
            Text = "�ptal",
            Location = new Point(260, currentY),
            Size = new Size(90, 30),
            DialogResult = DialogResult.Cancel
        };

        this.Controls.Add(saveButton);
        this.Controls.Add(cancelButton);
    }

    private string GetDataTypeHint(DataGridViewColumn column)
    {
        if (column is DataGridViewCheckBoxColumn)
            return "true/false";
        else if (column.ValueType == typeof(DateTime))
            return "YYYY-MM-DD";
        else if (column.ValueType == typeof(double))
            return "Say�sal de�er (�rn: 123.45)";
        else if (column.ValueType == typeof(decimal))
            return "Para birimi (�rn: 123.45)";
        else
            return "";
    }

    private void TextBox_Leave(object sender, EventArgs e)
    {
        TextBox textBox = (TextBox)sender;
        string value = textBox.Text.Trim();

        if (string.IsNullOrEmpty(value))
            return;

        try
        {
            ValidateDataTypeInput(textBox.Name, value);
            textBox.BackColor = SystemColors.Window;
        }
        catch (Exception ex)
        {
            textBox.BackColor = Color.LightPink;
            MessageBox.Show(ex.Message, "Veri Tipi Hatas�", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            textBox.Focus();
        }
    }

    private void ValidateDataTypeInput(string columnName, string value)
    {
        DataGridViewColumn column = targetDataGrid.Columns[columnName];

        if (column is DataGridViewCheckBoxColumn)
        {
            if (!bool.TryParse(value.ToLower(), out _))
                throw new Exception("Bu alan i�in 'true' veya 'false' de�eri giriniz.");
        }
        else if (column.ValueType == typeof(DateTime))
        {
            if (!DateTime.TryParse(value, out _))
                throw new Exception("Ge�erli bir tarih giriniz (YYYY-MM-DD).");
        }
        else if (column.ValueType == typeof(double))
        {
            if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                throw new Exception("Ge�erli bir say�sal de�er giriniz.");
        }
        else if (column.ValueType == typeof(decimal))
        {
            if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                throw new Exception("Ge�erli bir para birimi de�eri giriniz.");
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {

            foreach (var kvp in textBoxes)
            {
                string columnName = kvp.Key;
                string value = kvp.Value.Text.Trim();

                if (!string.IsNullOrEmpty(value))
                {
                    ValidateDataTypeInput(columnName, value);
                }
            }


            int rowIndex = targetDataGrid.Rows.Add();
            foreach (DataGridViewColumn column in targetDataGrid.Columns)
            {
                string value = textBoxes[column.Name].Text;

                if (column.Name.Contains("Encrypted"))
                {
                    value = value.Encrypt1();
                }
                else if (column is DataGridViewCheckBoxColumn)
                {
                    value = bool.Parse(value.ToLower()).ToString();
                }

                targetDataGrid.Rows[rowIndex].Cells[column.Index].Value = value;
            }

            mainForm.SaveGridData(targetDataGrid);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Veri kaydedilirken hata olu�tu: {ex.Message}",
                          "Hata",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
        }
    }
}