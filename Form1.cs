using System.Text.Json;

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
                            Y = control.Location.Y
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
                            // �ifrelenmi� verileri ��z
                            info.DecryptData();
                            Control control = null;

                            switch (info.Type)
                            {
                                case "Button":
                                    control = new Button();
                                    break;
                                case "Label":
                                    control = new Label();
                                    break;
                                case "DataGridView":
                                    control = new DataGridView();
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
                            continue; // Hatal� komponenti atla, di�erlerine devam et
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya okuma hatas�: {ex.Message}");
            }
        }

        private void resetButtonClick(object sender, EventArgs e)
        {
            if (File.Exists(COMPONENTS_FILE))
            {
                File.WriteAllText(COMPONENTS_FILE, "[]");
            }
        }
    }


}

