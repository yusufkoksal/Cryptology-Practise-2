using System.Security.Cryptography;
using System.Text;

namespace Aes256
{
    public static class AesEncryption
    {
        private static readonly byte[] key = Convert.FromBase64String("M3p5ipEyNf27iJ7hjTeE2GE9X129gYBXiby/ZNz871c=");
        private static readonly byte[] iv = Encoding.UTF8.GetBytes("1122334455667788");


        public static string Encrypt1(this string plaintext)
        {
            using (Aes aesAlg = Aes.Create())
            {

                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedBytes;


                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sr = new StreamWriter(csEncrypt))
                        {
                            sr.Write(plaintext);
                        }
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }


                return Convert.ToBase64String(encryptedBytes);
            }
        }


        public static string Decrypt(this string ciphertext)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                byte[] cipherTextByte = Convert.FromBase64String(ciphertext);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream ms = new MemoryStream(cipherTextByte))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }

                }
            }
        }
    }
}
