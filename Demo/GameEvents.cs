using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class GameEvents : MonoBehaviour
    {
        public PlayerController player;
        public UIManager uiManager;

        void Start()
        {
            player.Initialize();
            uiManager.ShowStartScreen();
        }
    }
}
