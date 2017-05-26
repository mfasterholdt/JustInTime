﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionPickup : Action 
	{
		Item item; 
		Item container;

		public ActionPickup(int time, int duration, Item item, Item container)
		{
			this.time = time;
			this.duration = 1;
			this.item = item;
			this.container = container;
		}

		public override bool Perform (Character character, int currentTime)
		{
			//Find Item
			Location currentLocation = character.GetLocationAtTime(currentTime);

			for (int i = 0, count = currentLocation.items.Count; i < count; i++) 
			{
				Item foundContainer = currentLocation.items[i];

				if(foundContainer.Compare(container))
				{
					Item foundItem = foundContainer.itemsInside[0];

					if (foundItem.Compare (item)) 
					{
						character.inventory.Add(foundItem);

						foundContainer.itemsInside.Remove (foundItem);

						foundItem.itemAround = null;
						foundItem.characterCarrying = character;

						foundItem.transform.position = character.carryPivot.position;
					}
				}
			}

			return true;
		}
	}
}
