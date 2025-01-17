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

            // Tüm komponentleri kaydet
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

                // Sütun bilgilerini kaydet
                foreach (DataGridViewColumn column in dataGrid.Columns)
                {
                    gridData.Columns.Add(new ColumnInfo
                    {
                        Name = column.Name,
                        Type = column.GetType().Name
                    });
                }

                // Satýr verilerini kaydet
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

                // Tüm verileri þifrele
                gridData.EncryptData();

                // Sözlüðe ekle veya güncelle
                gridDataDict[gridName] = gridData;

                // JSON'a çevir ve kaydet
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
                MessageBox.Show($"Veri kaydedilirken hata oluþtu: {ex.Message}");
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
                                // Önce grid datasýný çöz
                                gridData.DecryptData();

                                // DataGridView'i bul - artýk çözülmüþ GridName ile arama yapýyoruz
                                var dataGrid = this.Controls.OfType<DataGridView>()
                                                 .FirstOrDefault(dg => dg.Name == gridData.GridName);

                                if (dataGrid != null)
                                {
                                    // Sütunlarý ekle
                                    dataGrid.Columns.Clear();
                                    foreach (var columnInfo in gridData.Columns)
                                    {
                                        try
                                        {
                                            // columnInfo zaten çözülmüþ durumda
                                            DataGridViewColumn column = CreateColumn(columnInfo.Name, columnInfo.Type);
                                            dataGrid.Columns.Add(column);
                                        }
                                        catch (Exception columnEx)
                                        {
                                            MessageBox.Show($"Sütun yüklenirken hata: {columnEx.Message}");
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
                                                    // Value'lar zaten çözülmüþ durumda
                                                    dataGrid.Rows[rowIndex].Cells[value.Key].Value = value.Value;
                                                }
                                                catch (Exception cellEx)
                                                {
                                                    MessageBox.Show($"Hücre verisi yüklenirken hata: {cellEx.Message}");
                                                    continue;
                                                }
                                            }
                                        }
                                        catch (Exception rowEx)
                                        {
                                            MessageBox.Show($"Satýr yüklenirken hata: {rowEx.Message}");
                                            continue;
                                        }
                                    }
                                }
                            }
                            catch (Exception gridEx)
                            {
                                MessageBox.Show($"Grid verisi çözülürken hata: {gridEx.Message}");
                                continue;
                            }
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        MessageBox.Show($"JSON formatý geçersiz: {jsonEx.Message}");
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
                MessageBox.Show($"Grid verileri yüklenirken hata oluþtu: {ex.Message}");
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

                        // Verileri þifrele
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
                MessageBox.Show($"Bileþenler kaydedilirken hata oluþtu: {ex.Message}");
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
                                    if (info.Text == "Düzenle" || info.Text == "Güncelle")
                                    {
                                        control.Click += (s, e) =>
                                        {
                                            // Button isminden DataGrid ismini al
                                            string gridName = info.Name.Split('_')[0];
                                            // Form üzerinde DataGrid'i bul
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
                            MessageBox.Show($"Bileþen yüklenirken hata oluþtu: {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya okuma hatasý: {ex.Message}");
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
    }


}

public class DataEntryForm : Form
{
    private Dictionary<string, TextBox> columnTextBoxes;
    private DataGridView targetDataGrid;
    private Form1 mainForm;
    private Label dateTimeLabel;
    private Label userLabel;
    private DataGridViewRow existingRow;
    private bool isUpdate;


    public DataEntryForm(DataGridView dataGrid, Form1 mainForm)
        : this(dataGrid, mainForm, null)
    {
    }


    public DataEntryForm(DataGridView dataGrid, Form1 mainForm, DataGridViewRow row)
    {
        this.targetDataGrid = dataGrid;
        this.mainForm = mainForm;
        this.existingRow = row;
        this.isUpdate = (row != null);
        columnTextBoxes = new Dictionary<string, TextBox>();
        InitializeForm();
    }

    private void InitializeForm()
    {
        this.Text = isUpdate ? "Veri Güncelleme" : "Veri Giriþi";
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
                Name = column.Name + "_TextBox",
                Location = new Point(150, currentY),
                Size = new Size(200, 23)
            };

            // Eðer güncelleme ise mevcut deðerleri TextBox'lara yerleþtir
            if (isUpdate)
            {
                string currentValue = existingRow.Cells[column.Index].Value?.ToString() ?? "";
                if (column.Name.Contains("Encrypted"))
                {
                    try
                    {
                        currentValue = currentValue.Decrypt();
                    }
                    catch
                    {

                    }
                }
                textBox.Text = currentValue;
            }

            this.Controls.Add(label);
            this.Controls.Add(textBox);
            columnTextBoxes.Add(column.Name, textBox);

            currentY += 35;
        }


        dateTimeLabel = new Label
        {
            Text = $"Tarih: {DateTime.Now}",
            Location = new Point(20, currentY),
            Size = new Size(330, 23)
        };
        this.Controls.Add(dateTimeLabel);
        currentY += 25;

        userLabel = new Label
        {
            Text = $"Kullanýcý: yusufkoksal",
            Location = new Point(20, currentY),
            Size = new Size(330, 23)
        };
        this.Controls.Add(userLabel);
        currentY += 35;


        Button saveButton = new Button
        {
            Text = isUpdate ? "Güncelle" : "Kaydet",
            Location = new Point(150, currentY),
            Size = new Size(90, 30),
            DialogResult = DialogResult.OK
        };
        saveButton.Click += SaveButton_Click;

        // Ýptal butonu
        Button cancelButton = new Button
        {
            Text = "Ýptal",
            Location = new Point(260, currentY),
            Size = new Size(90, 30),
            DialogResult = DialogResult.Cancel
        };

        this.Controls.Add(saveButton);
        this.Controls.Add(cancelButton);
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (isUpdate)
            {

                foreach (DataGridViewColumn column in targetDataGrid.Columns)
                {
                    string value = columnTextBoxes[column.Name].Text;

                    if (column.Name.Contains("Encrypted"))
                    {
                        value = value.Encrypt1();
                    }

                    existingRow.Cells[column.Index].Value = value;
                }
            }
            else
            {

                int newRowIndex = targetDataGrid.Rows.Add();
                DataGridViewRow newRow = targetDataGrid.Rows[newRowIndex];

                foreach (DataGridViewColumn column in targetDataGrid.Columns)
                {
                    string value = columnTextBoxes[column.Name].Text;

                    if (column.Name.Contains("Encrypted"))
                    {
                        value = value.Encrypt1();
                    }

                    newRow.Cells[column.Index].Value = value;
                }
            }

            mainForm.SaveGridData(targetDataGrid);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Veri {(isUpdate ? "güncellenirken" : "kaydedilirken")} hata oluþtu: {ex.Message}");
        }
    }
}