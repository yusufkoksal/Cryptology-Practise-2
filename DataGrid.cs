using System.Text.Json.Serialization;
using Aes256;

public class DataGridData
{
    [JsonPropertyName("GridName")]
    public string GridName { get; set; }

    [JsonPropertyName("Columns")]
    public List<ColumnInfo> Columns { get; set; }

    [JsonPropertyName("Rows")]
    public List<RowData> Rows { get; set; }

    public void EncryptData()
    {
        GridName = GridName.Encrypt1();

        foreach (var column in Columns)
        {
            column.EncryptData();
        }

        foreach (var row in Rows)
        {
            row.EncryptData();
        }
    }

    public void DecryptData()
    {
        GridName = GridName.Decrypt();

        foreach (var column in Columns)
        {
            column.DecryptData();
        }

        foreach (var row in Rows)
        {
            row.DecryptData();
        }
    }
}

public class ColumnInfo
{
    [JsonPropertyName("Name")]
    public string Name { get; set; }

    [JsonPropertyName("Type")]
    public string Type { get; set; }

    public void EncryptData()
    {
        Name = Name.Encrypt1();
        Type = Type.Encrypt1();
    }

    public void DecryptData()
    {
        Name = Name.Decrypt();
        Type = Type.Decrypt();
    }
}

public class RowData
{
    [JsonPropertyName("Values")]
    public Dictionary<string, string> Values { get; set; }

    public void EncryptData()
    {
        var encryptedValues = new Dictionary<string, string>();
        foreach (var pair in Values)
        {
            encryptedValues[pair.Key.Encrypt1()] = pair.Value.Encrypt1();
        }
        Values = encryptedValues;
    }

    public void DecryptData()
    {
        var decryptedValues = new Dictionary<string, string>();
        foreach (var pair in Values)
        {
            decryptedValues[pair.Key.Decrypt()] = pair.Value.Decrypt();
        }
        Values = decryptedValues;
    }
}