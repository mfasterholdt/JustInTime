using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionRecover : Action
{
	private Item item;
	private Item itemContainer;
	private int itemIndex;
	private int itemContainerIndex;

	public ActionRecover(int time, string name, Item item, int itemIndex, Item itemContainer, int itemContainerIndex )
	{
		this.time = time;
		this.name = name;

		this.item = item.Copy();
		this.itemIndex = itemIndex;

		this.itemContainer = itemContainer.Copy();
		this.itemContainerIndex = itemContainerIndex;
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

		if(currentLocation.items.Count <= itemContainerIndex)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+itemContainer.ToString()+" to place "+item.ToString()+" into, Paradox!");
			return false;
		}

		Item itemContainerFound = currentLocation.items[itemContainerIndex];

		if(itemContainerFound == null || itemContainerFound.Equals(itemContainer) == false)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+itemContainer.ToString()+" to place "+item.ToString()+" into, Paradox!");
			return false;
		}

		Item itemFound = itemContainerFound.RequestItem(character, item, itemIndex, this);

		if(itemFound == null)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not recover "+item.ToString()+" from "+itemContainerFound.ToString()+", Paradox!");
			return false;	
		}

		character.inventory.Add(itemFound);
		return true;
	}

	public override bool ParadoxCheck (int currentTime)
	{
		ItemEntry entry = GameManager.Instance.itemTimeline.GetLatestFixedEntry(currentTime, itemContainer.age, itemContainerIndex);

		Box box = entry.item as Box;

		if(box != null)
		{
			if(box.GetItems()[0] == null)
				return false;
		}

		return true;
	}
}
