using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Manages the whitelist of files and folders that should be ignored by the Obfuscator Tool.
    /// </summary>
    public static class WhitelistManager
    {
        private static readonly string WhitelistPath = Path.Combine(Application.dataPath, "ParadoxSecuritySuite/Data/whitelist.json");

        /// <summary>
        /// Loads whitelist entries from JSON. Each entry can be a filename or a folder path (relative to Assets/).
        /// </summary>
        public static HashSet<string> Load()
        {
            if (!File.Exists(WhitelistPath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WhitelistPath));
                File.WriteAllText(WhitelistPath, JsonUtility.ToJson(new Wrapper(), true));
                return new HashSet<string>();
            }

            string json = File.ReadAllText(WhitelistPath);
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            return wrapper?.entries != null ? new HashSet<string>(wrapper.entries) : new HashSet<string>();
        }

        /// <summary>
        /// Checks if a full path is whitelisted by filename or folder (relative to Assets/).
        /// </summary>
        public static bool IsWhitelisted(string fullPath)
        {
            var whitelist = Load();
            string fileName = Path.GetFileName(fullPath).ToLowerInvariant();
            string assetPath = "Assets/" + fullPath.Replace(Application.dataPath, "").Replace("\\", "/").TrimStart('/');
            assetPath = assetPath.ToLowerInvariant();

            foreach (var entry in whitelist)
            {
                string normalizedEntry = entry.Replace("\\", "/").TrimEnd('/').ToLowerInvariant();

                // Folder match: entry is a folder
                if (normalizedEntry.EndsWith("/") && assetPath.StartsWith(normalizedEntry))
                    return true;

                // Folder match without trailing slash
                if (!normalizedEntry.EndsWith("/") &&
                    Directory.Exists(Path.Combine(Application.dataPath, normalizedEntry)) &&
                    assetPath.StartsWith(normalizedEntry + "/"))
                    return true;

                // File match: entry is filename
                if (fileName == normalizedEntry)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds a new entry to the whitelist.
        /// </summary>
        public static void Add(string entry)
        {
            var current = Load();
            if (current.Add(entry))
                Save(current);
        }

        /// <summary>
        /// Saves the whitelist to the JSON file.
        /// </summary>
        public static void Save(HashSet<string> entries)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(WhitelistPath));
            string json = JsonUtility.ToJson(new Wrapper { entries = new List<string>(entries) }, true);
            File.WriteAllText(WhitelistPath, json);
        }

        [System.Serializable]
        private class Wrapper
        {
            public List<string> entries = new List<string>();
        }
    }
}
