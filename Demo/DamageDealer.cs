using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class DamageDealer : MonoBehaviour
    {
        public int damageAmount = 10;

        public void DealDamage()
        {
            Logger.LogEvent("Dealt " + damageAmount + " damage.");
        }
    }
}
