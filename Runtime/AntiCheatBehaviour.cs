using UnityEngine;
using System.Diagnostics;
using System.Linq;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    /// <summary>
    /// Monitors for known cheat processes running on the system.
    /// </summary>
    public class AntiCheatBehaviour : MonoBehaviour
    {
        private readonly string[] cheatProcesses = new string[]
        {
            "cheatengine", "artmoney", "scylla", "ollydbg", "x64dbg", "ida", "gamehack", "processhacker"
        };

        private float checkInterval = 5f; // seconds
        private float timer = 0f;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (!AntiCheatManager.AntiCheatEnabled)
                return;

            timer += Time.unscaledDeltaTime;
            if (timer >= checkInterval)
            {
                timer = 0f;
                RunChecks();
            }
        }

        private void RunChecks()
        {
            CheckCheatProcesses();
        }

        private void CheckCheatProcesses()
        {
            try
            {
                var running = Process.GetProcesses();
                foreach (var proc in running)
                {
                    string procName = proc.ProcessName.ToLower();
                    if (cheatProcesses.Any(p => procName.Contains(p)))
                    {
                        string message = $"⚠ Known cheat process detected: {proc.ProcessName}";
                        HandleCheatDetected(message);
                        return;
                    }
                }
            }
            catch (System.Exception e)
            {
                AntiCheatManager.LogEvent("Error checking processes: " + e.Message);
            }
        }

        private void HandleCheatDetected(string message)
        {
            AntiCheatManager.LogEvent(message);

#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayDialog("Cheat Detected", message, "OK");
#else
            // Optionally show a UI popup in build here if desired
            Application.Quit();
#endif
        }
    }
}
