using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionDrop : Action 
	{
        public Item currentItem;
		public ItemProfile itemProfile; 
        public ItemProfile itemContainerProfile;

		public float pickupOffset;
		public Vector3 towardsPos;

		public ActionDrop(int time, int duration, Item item, Item itemContainer)
		{
			this.time = time;
			this.duration = duration;
            this.itemProfile = item.GetProfile(false);
            this.itemContainerProfile = itemContainer.GetProfile(false);

			pickupOffset = item.transform.position.y - item.characterCarrying.pickupPivot.position.y;
            towardsPos = itemContainer.GetDropPosition();
		}

		public override bool Perform (Character character)
		{
			Location currentLocation = character.currentLocation;
           
			for (int i = 0, count = currentLocation.items.Count; i < count; i++) 
			{
                Item foundContainer = currentLocation.items[i];

                if(foundContainer.GetProfile(false) == itemContainerProfile && character.inventory.Count > 0)
				{
                    int lastIndex = character.inventory.Count - 1;

                    Item foundItem = character.inventory[lastIndex];
					//Item foundItem = foundContainer.itemsInside[0];

                    if (foundItem.GetProfile(false) == itemProfile) 
					{						
                        character.inventory.RemoveAt(lastIndex);

						foundContainer.itemsInside.Add(foundItem);

						foundItem.itemAround = foundContainer;
						foundItem.characterCarrying = null;

                        currentItem = foundItem;
						//foundItem.transform.position = character.carryPivot.position;
					}
				}
			}

			return true;
		}
	}
}
