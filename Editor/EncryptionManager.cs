using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Handles file encryption using AES or XOR for supported text-based assets.
    /// </summary>
    public static class EncryptionManager
    {
        private static readonly string aesKey = "MyStrongKey123456";    // Must be 16 ASCII characters
        private static readonly string aesIV = "InitializationVe";     // Must be 16 ASCII characters
        private static readonly string xorKey = "SimpleKey";

        /// <summary>
        /// Encrypts supported text-based files in the Assets folder using AES or XOR based on settings.
        /// </summary>
        public static void EncryptFiles()
        {
            var settings = ObfuscatorSettings.Instance;
            string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.EndsWith(".json") || file.EndsWith(".txt") || file.EndsWith(".xml"))
                {
                    string content = File.ReadAllText(file);

                    string encrypted = settings.useAES
                        ? EncryptAES(content)
                        : EncryptXOR(content);

                    File.WriteAllText(file, encrypted);
                    Debug.Log($"🔐 Encrypted {file} with {(settings.useAES ? "AES" : "XOR")}");
                    LogManager.LogAction($"Encrypted {file}");
                }
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Encrypts a string using XOR with the default key.
        /// </summary>
        public static string EncryptXOR(string input)
        {
            return EncryptXOR(input, xorKey);
        }

        /// <summary>
        /// Encrypts a string using XOR with a specified key.
        /// </summary>
        public static string EncryptXOR(string input, string key)
        {
            var output = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
                output.Append((char)(input[i] ^ key[i % key.Length]));
            return output.ToString();
        }

        /// <summary>
        /// Encrypts a string using AES with the default key and IV.
        /// </summary>
        public static string EncryptAES(string plainText)
        {
            return EncryptAES(plainText, aesKey, aesIV);
        }

        /// <summary>
        /// Encrypts a string using AES with a specified key and IV.
        /// </summary>
        public static string EncryptAES(string plainText, string key, string iv)
        {
            byte[] keyBytes = new byte[16];
            byte[] ivBytes = new byte[16];

            Array.Copy(Encoding.UTF8.GetBytes(key), keyBytes, Math.Min(keyBytes.Length, key.Length));
            Array.Copy(Encoding.UTF8.GetBytes(iv), ivBytes, Math.Min(ivBytes.Length, iv.Length));

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            using Aes aes = Aes.Create();
            aes.Key = keyBytes;
            aes.IV = ivBytes;
            aes.Mode = CipherMode.CBC;

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputBytes, 0, inputBytes.Length);
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypts an AES-encrypted string with a specified key and IV.
        /// </summary>
        public static string DecryptAES(string encrypted, string key, string iv)
        {
            byte[] keyBytes = new byte[16];
            byte[] ivBytes = new byte[16];

            Array.Copy(Encoding.UTF8.GetBytes(key), keyBytes, Math.Min(keyBytes.Length, key.Length));
            Array.Copy(Encoding.UTF8.GetBytes(iv), ivBytes, Math.Min(ivBytes.Length, iv.Length));

            byte[] encryptedBytes = Convert.FromBase64String(encrypted);

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
