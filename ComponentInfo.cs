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

        [JsonPropertyName("X")]
        public int X { get; set; }

        [JsonPropertyName("Y")]
        public int Y { get; set; }

        [JsonPropertyName("IsDataGrid")]
        public bool IsDataGrid { get; set; }

        [JsonPropertyName("Columns")]
        public List<string> Columns { get; set; }


        public void EncryptData()
        {
            try
            {
                if (!string.IsNullOrEmpty(Type)) Type = Type.Encrypt1();
                if (!string.IsNullOrEmpty(Name)) Name = Name.Encrypt1();
                if (!string.IsNullOrEmpty(Text)) Text = Text.Encrypt1();
                if (!string.IsNullOrEmpty(IsDataGrid)) Columns = IsDataGrid.Encrypt1();
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
                if (!string.IsNullOrEmpty(Type)) Type = Type.Decrypt();
                if (!string.IsNullOrEmpty(Name)) Name = Name.Decrypt();
                if (!string.IsNullOrEmpty(Text)) Text = Text.Decrypt();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Çözme hatası: {ex.Message}");
            }
        }
    }

}

