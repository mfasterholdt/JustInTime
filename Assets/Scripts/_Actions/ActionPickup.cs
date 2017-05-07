using UnityEngine;
using System.Collections;

public class ActionPickup : Action
{
	private Item item;
	private int locationItemIndex;

	public ActionPickup(int time, Item item, int locationItemIndex)
	{
		this.name = "Pickup "+item.ToString();
		this.time = time;
		this.duration = 1;
		this.item = item.Copy();
		this.locationItemIndex = locationItemIndex;
	}

	public override bool Perform (Character character)
	{	
		Location currentLocation = character.GetCurrentLocation();

//		Item itemFound = Utils.FindItem(currentLocation.items, item);
//
//		if(itemFound == null || currentLocation.items.Remove(itemFound) == false)
//		{
//			GameManager.paradox = new Paradox(this, character.GetName()+" could not pickup "+item.ToString()+" in the "+currentLocation.name+", Paradox!");
//			return false;
//		}

		if(currentLocation.items.Count < locationItemIndex + 1)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+" in the "+currentLocation.name+", Paradox!");
			return false;
		}

		//TODO, mff this currently does not support two characters picking up objects from the same location on the same tick
		Item itemFound = currentLocation.items[locationItemIndex];

		if(itemFound == null || itemFound.Equals(item) == false)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not pickup "+item.ToString()+" in the "+currentLocation.name+", Paradox!");
			return false;
		}

		currentLocation.items.RemoveAt(locationItemIndex);
		character.inventory.Add(itemFound);

		return true;
	}
}
