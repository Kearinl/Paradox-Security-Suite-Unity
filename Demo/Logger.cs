using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public static class Logger
    {
        public static void LogEvent(string message)
        {
            Debug.Log("[LogEvent] " + message);
        }
    }
}
