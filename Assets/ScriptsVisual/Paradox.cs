using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Paradox
	{
        public int time;
        public GameObject visuals;

        public Character character;

        public ItemProfile itemProfile;

        public enum ParadoxType {Item, Character}
        public ParadoxType type;

        public Paradox(int time, GameObject visuals, Character character)
		{		
            this.time = time;
			this.visuals = visuals;
			this.character = character;	
            type = ParadoxType.Character;
		}

        public Paradox(int time, GameObject visuals, ItemProfile itemProfile)
        {       
            this.time = time;
            this.visuals = visuals;
            this.itemProfile = itemProfile;

            type = ParadoxType.Item;
        }

        public override string ToString()
        {
            if (type == ParadoxType.Character)
            {
                return "Paradox " + time +" "+character;
            }
            else
            {
                return "Paradox " + time + " item";
            }
        }
	}
}
