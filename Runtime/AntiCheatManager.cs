using UnityEngine;
using System.IO;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Manages the Anti-Cheat system: enables/disables, logging, and ensures AntiCheatBehaviour exists.
    /// </summary>
    public static class AntiCheatManager
    {
        public static bool AntiCheatEnabled { get; private set; } = false;
        private static readonly string logPath = Path.Combine(Application.persistentDataPath, "anti_cheat_log.txt");

        /// <summary>
        /// Enable or disable the anti-cheat system.
        /// Automatically adds AntiCheatBehaviour to the scene if enabled.
        /// </summary>
        public static void SetAntiCheatEnabled(bool enabled)
        {
            AntiCheatEnabled = enabled;

            if (enabled && Application.isPlaying)
            {
                if (Object.FindFirstObjectByType<AntiCheatBehaviour>() == null)
                {
                    GameObject antiCheatGO = new GameObject("AntiCheatManager");
                    antiCheatGO.AddComponent<AntiCheatBehaviour>();
                    Object.DontDestroyOnLoad(antiCheatGO);
                }
            }
        }

        /// <summary>
        /// Logs a message to the persistent log file and Unity console.
        /// </summary>
        public static void LogEvent(string message)
        {
            Debug.Log(message);

            try
            {
                File.AppendAllText(logPath, $"[{System.DateTime.Now}] {message}\n");
            }
            catch
            {
                // Fail silently if logging fails
            }
        }
    }
}
