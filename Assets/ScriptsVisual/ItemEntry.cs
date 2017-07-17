using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
    public class ItemEntry
    {
        int time;

        Location location;

        ItemProfile container;

        Character characterCarrying;

        public ItemEntry(int time)
        {
            this.time = time;
        }

        public string Draw()
        {
            return time.ToString();
        }
    }
}
