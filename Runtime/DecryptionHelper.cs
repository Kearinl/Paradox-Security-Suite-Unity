using System.IO;
using System.Text;
using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Provides XOR-based decryption for files and strings.
    /// </summary>
    public static class DecryptionHelper
    {
        private static readonly string xorKey = "SimpleKey";

        /// <summary>
        /// Decrypts the contents of a file using XOR decryption.
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
            return DecryptXOR(encrypted, xorKey);
        }

        /// <summary>
        /// Decrypts a string using XOR with the provided key.
        /// </summary>
        /// <param name="input">The encrypted string.</param>
        /// <param name="key">The key to use for XOR decryption.</param>
        /// <returns>Decrypted string.</returns>
        public static string DecryptXOR(string input, string key)
        {
            var output = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                output.Append((char)(input[i] ^ key[i % key.Length]));
            }
            return output.ToString();
        }
    }
}
