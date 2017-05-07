using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionPlace : Action
{
	private Item item;
	private Item itemContainer;
	private int inventoryIndex;
	private int itemContainerIndex;

	public ActionPlace(int time, string name, Item item, int inventoryIndex, Item itemContainer, int itemContainerIndex)
	{
		this.time = time;
		this.name = name;

		this.item = item.Copy();
		this.inventoryIndex = inventoryIndex;

		this.itemContainer = itemContainer.Copy();
		this.itemContainerIndex =itemContainerIndex;
	}

	public override bool Perform (Character character)
	{	
		Location currentLocation = character.GetCurrentLocation();

//		Item itemContainerFound = Utils.FindItem(currentLocation.items, itemContainer);
//
//		if(itemContainerFound == null)
//		{
//			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+itemContainer.ToString()+" to place "+item.ToString()+" into, Paradox!");
//			return false;
//		}
//
		if(currentLocation.items.Count <= itemContainerIndex)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+itemContainer.ToString()+" to place "+item.ToString()+" into,, Paradox!");
			return false;
		}

		Item itemContainerFound = currentLocation.items[itemContainerIndex];

		if(itemContainerFound == null || itemContainerFound.Equals(itemContainer) == false)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+itemContainer.ToString()+" to place "+item.ToString()+" into, Paradox!");
			return false;
		}

		Item itemFound = character.inventory[inventoryIndex];

		if(itemFound == null || itemFound.Equals(item) == false)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+" in inventory, Paradox!");
			return false;
		}

//		Item itemFound = Utils.FindItem(character.inventory, item);
//
//		if(itemFound == null)
//		{
//			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+" in inventory, Paradox!");
//			return false;
//		}

		character.inventory.RemoveAt(inventoryIndex);
	
		if(itemContainerFound.PassItem(character, itemFound, this) == false)
		{		
			GameManager.paradox = new Paradox(this, character.GetName()+" could not place "+itemFound.ToString()+" inside "+itemContainer.ToString()+", Paradox!");
			return false;
		}

		return true;
	}
}
