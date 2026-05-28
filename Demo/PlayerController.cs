using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class PlayerController : MonoBehaviour
    {
        public Inventory inventory;
        public Health health;

        public void Initialize()
        {
            Debug.Log("Player Initialized");
            health.SetStartingHealth(100);
            inventory.ClearItems();
        }
    }
}
