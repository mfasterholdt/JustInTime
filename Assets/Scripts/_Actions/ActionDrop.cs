using UnityEngine;
using System.Collections;

public class ActionDrop : Action
{
	private Item item;
	private int inventoryIndex;

	public ActionDrop(int time, Item item, int inventoryIndex)
	{
		this.name = "Drop "+item.ToString();
		this.time = time;
		this.item = item.Copy();
		this.inventoryIndex = inventoryIndex;
	}

	public override bool Perform (Character character)
	{		
		Location currentLocation = character.GetCurrentLocation();

//		Item itemFound = Utils.FindItem(character.inventory, item);
//
//		if(itemFound == null || character.inventory.Remove(item) == false)
//		{
//			GameManager.paradox = new Paradox(this, character.GetName()+" could not drop "+item.ToString()+" in the "+currentLocation.name+", Paradox!");
//			return false;			
//		}

		if(character.inventory.Count < inventoryIndex + 1)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+" in the inventory, Paradox!");
			return false;
		}

		Item itemFound = character.inventory[inventoryIndex];

		if(itemFound == null || itemFound.Equals(item) == false)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not drop "+item.ToString()+" in the "+currentLocation.name+", Paradox!");
			return false;		
		}

		character.inventory.RemoveAt(inventoryIndex);
		currentLocation.items.Add(itemFound);

		return true;
	}
}
