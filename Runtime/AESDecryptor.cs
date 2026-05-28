using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Provides AES decryption for files and strings. Must use the same key and IV as used in EncryptionManager.
    /// </summary>
    public static class AESDecryptor
    {
        private static readonly string aesKey = "MyStrongKey123456"; // Must be 16 ASCII characters
        private static readonly string aesIV = "InitializationVe";   // Must be 16 ASCII characters

        /// <summary>
        /// Decrypts the contents of a file using AES.
        /// </summary>
        /// <param name="path">The full file path.</param>
        /// <returns>Decrypted string content, or null if file not found.</returns>
        public static string DecryptFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.LogError($"File not found: {path}");
                return null;
            }

            string encrypted = File.ReadAllText(path);
            return Decrypt(encrypted, aesKey, aesIV);
        }

        /// <summary>
        /// Decrypts a base64-encoded AES string.
        /// </summary>
        /// <param name="encryptedText">The encrypted text in Base64 format.</param>
        /// <param name="key">AES key (must match encryption key).</param>
        /// <param name="iv">AES IV (must match encryption IV).</param>
        /// <returns>Decrypted plain text.</returns>
        public static string Decrypt(string encryptedText, string key, string iv)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            byte[] encryptedBytes = System.Convert.FromBase64String(encryptedText);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;

            using MemoryStream ms = new(encryptedBytes);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new(cs);
            return sr.ReadToEnd();
        }
    }
}
