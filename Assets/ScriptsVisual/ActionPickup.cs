using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionPickup : Action
    {
//        public Item currentItem;

        public ItemProfile itemProfile;
        public ItemProfile itemContainerProfile;
        public Location location;

        public Vector3 fromPos;
        public float pickupOffset;

		public ActionPickup(int time, int duration, Item item, Item itemContainer, Location location, float pickupOffset)
		{
			this.time = time;
			this.duration = duration;
            this.itemProfile = item.GetProfile();
            this.itemContainerProfile = itemContainer.GetProfile();

			this.fromPos = item.transform.position;
            this.location = location;
			this.pickupOffset = pickupOffset;

//			Debug.Log(itemContainer.GetId().ToString() +","+item.GetId().ToString());
		}

        public Item FindContainer()
        {
            for (int i = 0, count = location.items.Count; i < count; i++)
            {
                Item container = location.items[i];
                ItemProfile containerProfile = container.GetProfile();

                if (itemContainerProfile.CompareSurface(containerProfile))
                {
                    return container;
                }
            }

            return null;
        }

        public Item FindItem(Item container)
        {
            if (container)
            {
                int itemCount = container.itemsInside.Count;

                if (itemCount > 0)
                {                        
                    Item item = container.itemsInside[itemCount - 1];
                    //Debug.Log(item.GetProfile(false) + "     =    "+itemProfile);
                    if (item.GetProfile() == itemProfile)
                    {                          
                        return item;
                    }
                }
            }

            return null;
        }

//        public bool Check()
//        {
//            Item container = FindContainer();
//
//            if (container)
//            {
//                Item item = FindItem(container);
//
//                if (item)
//                {
//                    return true;
//                }
//            }
//
//            return false;
//        }

		public override bool Perform(Character character)
		{
			//Find Item
			//Location currentLocation = character.GetLocationAtTime(currentTime);
			Location currentLocation = character.currentLocation;

            Item container = FindContainer();

            if (container)
            {
                Item item = FindItem(container);

//                if (item == null)
//                {
//                    //Make shadow item
//                    //TODO, how will this work with nested items?
//                    GameObject warning = GameObject.Instantiate(GameManager.instance.itemWarningPrefab, itemProfile.position, Quaternion.identity);                    
//                    character.currentWarnings.Add(warning);
//                    //item = warning.GetComponent<Item>();
//                }
                if (item)
                {
                    container.itemsInside.Remove(item);
                    item.itemAround = null;

                    character.inventory.Add(item);
                    item.characterCarrying = character;

                    return true;
                }
            }

//			for (int i = 0, count = currentLocation.items.Count; i < count; i++)
//			{
//                Item foundContainer = currentLocation.items[i];
//
//                if (foundContainer.GetProfile() == itemContainerProfile)
//				{                   
//                    int itemCount = foundContainer.itemsInside.Count;
//
//                    if (itemCount > 0)
//                    {                        
//                        Item foundItem = foundContainer.itemsInside[itemCount - 1];
//
//                        if (foundItem.GetProfile() == itemProfile)
//                        {                          
//                            foundContainer.itemsInside.Remove(foundItem);
//                            foundItem.itemAround = null;
//
//                            character.inventory.Add(foundItem);
//                            foundItem.characterCarrying = character;
////                            currentItem = foundItem;
//
//                            return true;
//                        }
//                        //Item does not match
//                    }
//
//                    //No items in container
//                    break;
//                }
//			}

            //No container found
			return false;
		}
	}
}
