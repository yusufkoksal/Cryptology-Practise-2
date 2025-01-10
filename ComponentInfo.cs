using System.Text.Json.Serialization;
using Aes256;

namespace EsiCrypto3
{
    public class ComponentInfo
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Text")]
        public string Text { get; set; }

        // X, Y ve IsDataGrid için şifrelenmiş değerleri tutacak propertyler
        [JsonPropertyName("X")]
        public string EncryptedX { get; set; }

        [JsonPropertyName("Y")]
        public string EncryptedY { get; set; }


        [JsonPropertyName("IsDataGrid")]
        public string EncryptedIsDataGrid { get; set; }

        [JsonPropertyName("Columns")]
        public List<string> Columns { get; set; }

        // Gerçek değerleri tutacak private alanlar
        private int _x;
        private int _y;
        private bool _isDataGrid;

        // X, Y ve IsDataGrid için public propertyler
        [JsonIgnore]
        public int X
        {
            get => _x;
            set
            {
                _x = value;
                EncryptedX = value.ToString();
            }
        }

        [JsonIgnore]
        public int Y
        {
            get => _y;
            set
            {
                _y = value;
                EncryptedY = value.ToString();
            }
        }

        [JsonIgnore]
        public bool IsDataGrid
        {
            get => _isDataGrid;
            set
            {
                _isDataGrid = value;
                EncryptedIsDataGrid = value.ToString();
            }
        }

        public void EncryptData()
        {
            try
            {
                if (!string.IsNullOrEmpty(Type))
                    Type = Type.Encrypt1();
                if (!string.IsNullOrEmpty(Name))
                    Name = Name.Encrypt1();
                if (!string.IsNullOrEmpty(Text))
                    Text = Text.Encrypt1();

                // X, Y ve IsDataGrid değerlerini şifrele
                EncryptedX = X.ToString().Encrypt1();
                EncryptedY = Y.ToString().Encrypt1();
                EncryptedIsDataGrid = IsDataGrid.ToString().Encrypt1();

                // Columns listesini şifrele
                if (Columns != null)
                {
                    Columns = Columns.Select(c => !string.IsNullOrEmpty(c) ? c.Encrypt1() : c).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Şifreleme hatası: {ex.Message}");
            }
        }

        public void DecryptData()
        {
            try
            {
                if (!string.IsNullOrEmpty(Type))
                    Type = Type.Decrypt();
                if (!string.IsNullOrEmpty(Name))
                    Name = Name.Decrypt();
                if (!string.IsNullOrEmpty(Text))
                    Text = Text.Decrypt();

                // X, Y ve IsDataGrid değerlerini çöz
                if (!string.IsNullOrEmpty(EncryptedX))
                {
                    string decryptedX = EncryptedX.Decrypt();
                    _x = int.Parse(decryptedX);
                }

                if (!string.IsNullOrEmpty(EncryptedY))
                {
                    string decryptedY = EncryptedY.Decrypt();
                    _y = int.Parse(decryptedY);
                }

                if (!string.IsNullOrEmpty(EncryptedIsDataGrid))
                {
                    string decryptedIsDataGrid = EncryptedIsDataGrid.Decrypt();
                    _isDataGrid = bool.Parse(decryptedIsDataGrid);
                }

                // Columns listesini çöz
                if (Columns != null)
                {
                    Columns = Columns.Select(c => !string.IsNullOrEmpty(c) ? c.Decrypt() : c).ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Çözme hatası: {ex.Message}");
            }
        }
    }
}