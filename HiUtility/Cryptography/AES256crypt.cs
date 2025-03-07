using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace MrHihi.HiUtility.Cryptography;

public class Aes256crypt: ISymmetric
{
    public string Encrypt(string data, string key)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.GenerateIV();
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                }

                byte[] iv = aesAlg.IV;
                byte[] encrypted = msEncrypt.ToArray();

                byte[] result = new byte[iv.Length + encrypted.Length];
                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encrypted, 0, result, iv.Length, encrypted.Length);

                return Convert.ToBase64String(result);
            }
        }
    }
    public string Decrypt(string data, string key)
    {
        byte[] fullCipher = Convert.FromBase64String(data);

        using (Aes aesAlg = Aes.Create())
        {
            byte[] iv = new byte[aesAlg.BlockSize / 8];
            byte[] cipherText = new byte[fullCipher.Length - iv.Length];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipherText, 0, cipherText.Length);

            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}