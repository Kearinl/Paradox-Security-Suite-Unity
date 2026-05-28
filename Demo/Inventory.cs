using UnityEngine;
using System.Collections.Generic;

namespace HappyDayzGames.ParadoxSecuritySuite
{
    public class Inventory : MonoBehaviour
    {
        private List<string> items = new List<string>();

        public void AddItem(string item)
        {
            items.Add(item);
        }

        public void ClearItems()
        {
            items.Clear();
        }
    }
}
