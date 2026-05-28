using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class Weapon : MonoBehaviour
    {
        public DamageDealer damageDealer;

        public void Fire()
        {
            Debug.Log("Firing weapon");
            damageDealer.DealDamage();
        }
    }
}
