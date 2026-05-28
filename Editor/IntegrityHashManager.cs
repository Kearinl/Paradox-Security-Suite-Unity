using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Handles SHA-256 hash generation and integrity validation for project files.
    /// </summary>
    public static class IntegrityHashManager
    {
        private static readonly string path = Path.Combine(Application.dataPath, "ParadoxSecuritySuite/Data/integrity_hash.json");
        private static Dictionary<string, string> hashMap = new();

        [System.Serializable]
        private class HashEntry
        {
            public string path;
            public string hash;
        }

        [System.Serializable]
        private class Wrapper
        {
            public List<HashEntry> entries = new List<HashEntry>();
        }

        /// <summary>
        /// Generates SHA-256 hashes for the given asset paths and saves them to integrity_hash.json.
        /// </summary>
        public static void GenerateHashes(string[] assetPaths)
        {
            hashMap.Clear();
            Wrapper wrapper = new Wrapper();

            Debug.Log("🔍 Starting hash generation...");

            foreach (string assetPath in assetPaths)
            {
                if (!File.Exists(assetPath)) continue;

                string normalizedPath = assetPath.Replace("\\", "/");
                string content = File.ReadAllText(assetPath);
                string hash = ComputeSHA256(content);

                hashMap[normalizedPath] = hash;
                wrapper.entries.Add(new HashEntry { path = normalizedPath, hash = hash });

                Debug.Log($"✅ Hashed: {normalizedPath} → {hash}");
            }

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(path, json);

            Debug.Log($"📄 integrity_hash.json saved with {wrapper.entries.Count} file(s).");
        }

        /// <summary>
        /// Validates the integrity of tracked files against their saved hashes.
        /// </summary>
        public static bool Validate()
        {
            Load();
            Debug.Log("🔎 Validating integrity of tracked files...");

            foreach (var kvp in hashMap)
            {
                if (!File.Exists(kvp.Key))
                {
                    Debug.LogWarning($"⚠️ Missing file: {kvp.Key}");
                    return false;
                }

                string content = File.ReadAllText(kvp.Key);
                string currentHash = ComputeSHA256(content);

                if (currentHash != kvp.Value)
                {
                    Debug.LogWarning($"❌ Hash mismatch: {kvp.Key}");
                    Debug.LogWarning($"    Expected: {kvp.Value}");
                    Debug.LogWarning($"    Found:    {currentHash}");
                    return false;
                }
                else
                {
                    Debug.Log($"✅ Match: {kvp.Key}");
                }
            }

            Debug.Log("🎉 Integrity validation passed!");
            return true;
        }

        /// <summary>
        /// Loads the saved hashes from integrity_hash.json.
        /// </summary>
        private static void Load()
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("⚠️ No integrity hash file found. Skipping load.");
                hashMap = new();
                return;
            }

            string json = File.ReadAllText(path);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);

            hashMap = new();
            foreach (var entry in wrapper.entries)
            {
                hashMap[entry.path] = entry.hash;
            }

            Debug.Log($"📦 Loaded {hashMap.Count} hash entries from integrity_hash.json.");
        }

        /// <summary>
        /// Computes the SHA-256 hash of a string.
        /// </summary>
        private static string ComputeSHA256(string input)
        {
            using var sha = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha.ComputeHash(bytes);
            return System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
