using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionPickup : Action
    {
        public Item currentItem;

        public ItemProfile itemProfile;
        public ItemProfile itemContainerProfile;

        public Vector3 fromPos;
        public float pickupOffset;

		public ActionPickup(int time, int duration, Item item, Item itemContainer, float pickupOffset)
		{
			this.time = time;
			this.duration = duration;
            this.itemProfile = item.GetProfile();
            this.itemContainerProfile = itemContainer.GetProfile();

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

                if (foundContainer.GetProfile() == itemContainerProfile)
				{                   
                    int itemCount = foundContainer.itemsInside.Count;

                    if (itemCount > 0)
                    {                        
                        Item foundItem = foundContainer.itemsInside[itemCount - 1];

                        if (foundItem.GetProfile() == itemProfile)
                        {                          
                            foundContainer.itemsInside.Remove(foundItem);
                            foundItem.itemAround = null;

                            character.inventory.Add(foundItem);
                            foundItem.characterCarrying = character;
                            currentItem = foundItem;

                            return true;
                        }
                        //Item does not match
                    }

                    //No items in container
                    break;
                }
			}

            //No container found

			return false;
		}
	}
}
