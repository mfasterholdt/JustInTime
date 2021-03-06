﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public struct Observation
	{
		public int time;
		public Location location;
        public List<ItemProfile> itemProfiles;
		public List<Character> characters;

		public Observation(int currentTime, Location location, List<Character> characters)
		{
			this.time = currentTime;
			this.location = location;
			this.characters = new List<Character>(characters);

            List<ItemProfile> itemProfiles = new List<ItemProfile>(location.items.Count);

            for (int i = 0, count = location.items.Count; i < count; i++)
            {
                itemProfiles.Add(location.items[i].GetProfile());
            }

            this.itemProfiles = itemProfiles;
		}

        public override string ToString()
        {
            string s = time+"  "+location.gameObject.name;

            for (int i = 0, count = itemProfiles.Count; i < count; i++)
            {
                s += itemProfiles[i]+"  ";
            }

            for (int i = 0, count = characters.Count; i < count; i++)
            {
                s += characters[i].name+"  ";
            }

            return s;
        }
	}
}