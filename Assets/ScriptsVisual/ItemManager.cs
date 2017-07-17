using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
    public class ItemManager
    {
        private List<ItemTimeline> itemTimelines = new List<ItemTimeline>();

        private List<Item> allItems;

        public ItemManager(List<Item> items)
        {
            for (int i = 0, count = items.Count; i < count; i++)
            {
                Item item = items[i];

                if (item && item.isMovable)
                {
                    ItemTimeline itemTimeline = new ItemTimeline(item.id);
                    itemTimelines.Add(itemTimeline);
                }
            }

            allItems = items;
        }

        public void AddEntries(int time)
        {
            for (int i = 0, count = allItems.Count; i < count; i++)
            {
                Item item = allItems[i];

                for (int s = 0, timelineCount = itemTimelines.Count; s < timelineCount; s++) 
                {
                    ItemTimeline timeline = itemTimelines[s];

                    if (timeline.id == item.id)
                    {
                        timeline.AddEntry(time, null, null, null);
                    }
                }
            }
        }

        public void Draw()
        {
            for (int s = 0, timelineCount = itemTimelines.Count; s < timelineCount; s++)
            {
                ItemTimeline timeline = itemTimelines[s];
                timeline.Draw();
            }
        }
    }
}
