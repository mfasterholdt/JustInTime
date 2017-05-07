using UnityEngine;
using System.Collections;

public class ActionToggle : Action
{
	public Item item;

	public ActionToggle(int currentTime, string name, Item item)
	{
		this.time = currentTime;
		this.name = name;
		this.item = item.Copy();
	}

	public override bool Perform (Character character)
	{
		Location currentLocation = character.GetCurrentLocation();

		Item itemFound = Utils.FindItem(currentLocation.items, item);
		if(itemFound == null)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not toggle "+item.ToString()+", Paradox!");
			return false;
		}

		return itemFound.ToggleItem(character);
	}
}
