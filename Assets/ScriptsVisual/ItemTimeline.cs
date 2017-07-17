using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
    public class ItemTimeline
    {
        public int id;

        List<ItemEntry> history = new List<ItemEntry>();

        public ItemTimeline(int id)
        {
            this.id = id;
        }

        public void AddEntry(int time, Location location, Item container, Character characterCarrying)
        {
            ItemEntry entry = new ItemEntry(time);
            history.Add(entry);
        }

        public void Draw()
        {
            float fieldWidth = 150f;
            float fieldHeight = 30f;
            Vector2 posGUI = new Vector2(Screen.width - fieldWidth, 0);

            for (int i = 0, count = history.Count; i < count; i++) 
            {
                ItemEntry entry = history[i];
              //  GUI.Label(new Rect(posGUI.x , posGUI.y + fieldHeight * i, fieldWidth, fieldHeight), entry.Draw());
//                GUI.Label(new Rect(posGUI.x - (offsetX * 100), posGUI.y + fieldHeight * entry.age, fieldWidth, fieldHeight), entry.ToString());
            }
        }
    }
}
