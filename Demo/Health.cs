using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class Health : MonoBehaviour
    {
        private int health;

        public void SetStartingHealth(int value)
        {
            health = value;
            Debug.Log("Health set to " + value);
        }
    }
}
