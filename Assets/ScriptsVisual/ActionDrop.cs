using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionDrop : Action 
	{
		public Item item; 
		public Item itemContainer;

		public float pickupOffset;
		public Vector3 towardsPos;

		public ActionDrop(int time, int duration, Item item, Item itemContainer)
		{
			this.time = time;
			this.duration = duration;
			this.item = item;
			this.itemContainer = itemContainer;

			pickupOffset = item.transform.position.y - item.characterCarrying.pickupPivot.position.y;
            towardsPos = itemContainer.GetDropPosition();
		}

		public override bool Perform (Character character, int currentTime)
		{
			Location currentLocation = character.currentLocation;


			for (int i = 0, count = currentLocation.items.Count; i < count; i++) 
			{
				Item foundContainer = currentLocation.items[i];

				if(foundContainer.Compare(itemContainer) && character.inventory.Count > 0)
				{
					Item foundItem = character.inventory [character.inventory.Count -1];
					//Item foundItem = foundContainer.itemsInside[0];

					if (foundItem.Compare (item)) 
					{
						
						character.inventory.Remove(foundItem);

						foundContainer.itemsInside.Add (foundItem);

						foundItem.itemAround = foundContainer;
						foundItem.characterCarrying = null;

						//foundItem.transform.position = character.carryPivot.position;
					}
				}
			}

			return true;
		}
	}
}
