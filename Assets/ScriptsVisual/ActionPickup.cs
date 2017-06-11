using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionPickup : Action
	{
		public Item item;
		public Item itemContainer;

		public Vector3 fromPos;
		public float pickupOffset;

		public ActionPickup(int time, int duration, Item item, Item itemContainer, float pickupOffset)
		{
			this.time = time;
			this.duration = duration;
			this.item = item;
			this.itemContainer = itemContainer;

			this.fromPos = item.transform.position;
			this.pickupOffset = pickupOffset;

//			Debug.Log(itemContainer.GetId().ToString() +","+item.GetId().ToString());
		}

		public override bool Perform(Character character, int currentTime)
		{
			//Find Item
			//Location currentLocation = character.GetLocationAtTime(currentTime);
			Location currentLocation = character.currentLocation;

			for (int i = 0, count = currentLocation.items.Count; i < count; i++)
			{
				Item foundContainer = currentLocation.items[i];

				if (foundContainer.Compare(itemContainer))
				{
                    Item foundItem = foundContainer.itemsInside[foundContainer.itemsInside.Count - 1];

					if (foundItem.Compare(item))
					{
						character.inventory.Add(foundItem);

						foundContainer.itemsInside.Remove(foundItem);

						foundItem.itemAround = null;
						foundItem.characterCarrying = character;
						//foundItem.transform.position = character.carryPivot.position;
					}
				}
			}

			return true;
		}
	}
}
