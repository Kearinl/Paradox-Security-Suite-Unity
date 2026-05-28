using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Handles logging of obfuscation and encryption actions.
    /// </summary>
    public static class LogManager
    {
        private static readonly string logPath = Path.Combine(Application.dataPath, "ParadoxSecuritySuite/Data/obfuscation_log.txt");

        /// <summary>
        /// Logs an action to the console and appends it to the obfuscation log file if logging is enabled.
        /// </summary>
        public static void LogAction(string message)
        {
            var settings = ObfuscatorSettings.Instance;
            if (!settings.enableLogging) return;

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string entry = $"[{timestamp}] {message}";

            Debug.Log("📝 " + entry);

            if (!settings.dryRunMode)
            {
                File.AppendAllText(logPath, entry + Environment.NewLine);
            }
        }

        /// <summary>
        /// Clears the obfuscation log file.
        /// </summary>
        public static void ClearLog()
        {
            if (File.Exists(logPath))
            {
                File.WriteAllText(logPath, string.Empty);
                Debug.Log("🧼 Log file cleared.");
            }
        }

        /// <summary>
        /// Loads a set of file names that have already been obfuscated or encrypted based on the log file.
        /// </summary>
        public static HashSet<string> LoadObfuscatedFiles()
        {
            var result = new HashSet<string>();

            if (!File.Exists(logPath)) return result;

            var lines = File.ReadAllLines(logPath);
            foreach (var line in lines)
            {
                if (line.Contains("Renamed class:") || line.Contains("Encrypted:"))
                {
                    var parts = line.Split(new[] { "→", "Encrypted:" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        string target = parts[^1].Trim();
                        if (!string.IsNullOrEmpty(target))
                        {
                            string fileName = Path.GetFileName(target);
                            result.Add(fileName);
                        }
                    }
                }
            }

            return result;
        }
    }
}
