using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Oven : Item 
{
	public int capacity = 1;

	private Item[] itemsInside;

	public bool isBurning = true;
	public bool toggleAllowed = false;

	public Oven(bool pickup = false, bool isBurning = true, bool toggle = false, Item[] itemsInside = null)
	{
		this.pickup = pickup;
		this.itemsInside = Utils.CopyItemArray(itemsInside);
		this.capacity = itemsInside.Length;
		this.isBurning = isBurning;
		this.toggleAllowed = toggle;
	}

	public override Item Copy ()
	{	
		if(itemsInside == null)
			return new Oven(pickup, isBurning, toggleAllowed, new Item[capacity]);
		else
			return new Oven(pickup, isBurning, toggleAllowed, itemsInside);
	}

	public override Action[] GetLocationActions(int currentTime, int locationIndex, Character player)
	{		
		List<Action> actions = new List<Action>();

		if(toggleAllowed)
		{
			string toggleText = isBurning ? "Turn Oven Off" : "Turn Oven On";
			ActionToggle actionToggle = new ActionToggle(currentTime, toggleText, this);
			actions.Add(actionToggle);
		}

		bool freeSpace = false;
		for (int i = 0, length = itemsInside.Length; i < length; i++) 
		{
			Item item = itemsInside[i];
			if(item == null)
			{
				freeSpace = true;
			}
			else
			{
				ActionRecover actionRecover = new ActionRecover(currentTime, "Take "+item.ToShortString()+" from Oven", item, i, this, locationIndex ); 
				actions.Add(actionRecover);
			}
		}

		if(freeSpace)
		{
			for(int i = 0, count = player.inventory.Count; i < count; i++)
			{
				Item item = player.inventory[i];
				//TODO, maybe allow the oven to filter somehow, this currently allows placing ingredients in oven
//				if(item.GetType() == typeof(Cake))
				{
					ActionPlace actionPlace = new ActionPlace(currentTime, "Place "+item.ToShortString()+" in Oven", item, i, this, locationIndex ); 
					actions.Add(actionPlace);
				}
			}
		}

		return actions.ToArray();
	}

	public override bool PassItem(Character character, Item otherItem, Action action)
	{		
		if(action.GetType() == typeof(ActionPlace))
		{
			return AddItem(otherItem);
		}

		return true;
	}
		
	public override Item RequestItem(Character character, Item otherItem, int index, Action action)
	{
		if(action.GetType() == typeof(ActionRecover))
		{
			return RemoveItem(otherItem);
		}

		return null;
	}

	public bool AddItem(Item otherItem)
	{
		for (int i = 0, length = itemsInside.Length; i < length; i++) 
		{
			if(itemsInside[i] == null)
			{
				itemsInside[i] = otherItem.Copy();
				return true;
			}
		}

		return false;
	}

	public Item RemoveItem(Item otherItem)
	{
		return Utils.FindAndRemoveItem(itemsInside, otherItem);

//		for (int i = 0, length = itemsInside.Length; i < length; i++) 
//		{
//			Item itemFound = itemsInside[i];
//
//			if(itemFound != null && itemFound.Equals(otherItem))
//			{
//				itemsInside[i] = null;
//				return itemFound;
//			}
//		}
//
//		return null;
	}

	public override bool ToggleItem (Character character)
	{
		isBurning = !isBurning;

		return true;
	}

	public override bool TickLocation (Location location)
	{
		if(isBurning)
		{
			for (int i = 0, length = itemsInside.Length; i < length; i++) 
			{
				Item item = itemsInside[i];

				if(item != null)
				{
					if(item.UseItem(this) == false)
						return false;
				}
			}
		}

		return true;
	}

	public override bool Equals (object obj)
	{		
		Oven oven = obj as Oven;
		if(oven == null)
		{
			return false;
		} 
		else 
		{
			return isBurning == oven.isBurning;
		}
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		string itemText = "Oven";

		int itemLength =itemsInside.Length;
		itemText += " [ ";	
			
		for (int i = 0; i < itemLength; i++) 
		{
			Item item = itemsInside[i];

			if(item != null)
			{
				if(i != 0)
					itemText += ", ";	
				
				itemText += item.ToString();
			}
		}

		itemText += " ]";	

		if(toggleAllowed)
			itemText += isBurning ? " On" : " Off";

		return itemText;
	}
}
