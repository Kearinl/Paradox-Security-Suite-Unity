using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class EnemyAI : MonoBehaviour
    {
        public Health health;

        void Start()
        {
            Logger.LogEvent("Enemy spawned");
            health.SetStartingHealth(50);
        }
    }
}
