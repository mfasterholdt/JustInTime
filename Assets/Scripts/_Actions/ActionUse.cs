using UnityEngine;
using System.Collections;

public class ActionUse : Action
{
	public Item item;
	public Item target;

	public ActionUse(int currentTime, string name, Item item, Item target)
	{
		this.time = currentTime;
		this.name = name;

		this.item = item.Copy();
		this.target = target.Copy();
	}

	public override bool Perform (Character character)
	{
		Location currentLocation = character.GetCurrentLocation();

		Item itemFound = Utils.FindItem(character.inventory, item);

		if(itemFound == null)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+", Paradox!");
			return false;
		}
			
		Item targetFound = Utils.FindItem(currentLocation.items, target);

		if(targetFound == null)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not "+target.ToString()+", Paradox!");
			return false;
		}

		return targetFound.UseItem(itemFound);
	}
}
