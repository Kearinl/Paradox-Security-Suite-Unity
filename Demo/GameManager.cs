using UnityEngine;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class GameManager : MonoBehaviour
    {
        public PlayerController player;
        public UIManager uiManager;

        void Start()
        {
            Logger.LogEvent("Game Started");
            player.Initialize();
            uiManager.ShowStartScreen();
        }
    }
}
