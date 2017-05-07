using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Car : Item 
{
	public int capacity = 4;

	private Item[] itemsInside;

	public Car(bool pickup, Item[] itemsInside)
	{
		this.pickup = pickup;
		this.itemsInside = Utils.CopyItemArray(itemsInside);
		this.capacity = itemsInside.Length;
	}

	public override Item Copy ()
	{	
		if(itemsInside == null)
			return new Car(pickup, new Item[capacity]);
		else
			return new Car(pickup, itemsInside);
	}

	public override Action[] GetLocationActions(int currentTime, int locationIndex, Character player)
	{		
		List<Action> actions = new List<Action>();

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
				ActionRecover actionRecover = new ActionRecover(currentTime, "Detach "+item.ToString()+" from Car", item, i, this, locationIndex ); 
				actions.Add(actionRecover);
			}
		}

		if(freeSpace)
		{
			for(int i = 0, count = player.inventory.Count; i < count; i++)
			{
				Wheel wheel = player.inventory[i] as Wheel;

				if(wheel != null)
				{
					ActionPlace actionPlace = new ActionPlace(currentTime, "Attach "+wheel.ToString()+" to Car", wheel, i, this, locationIndex ); 
					actions.Add(actionPlace);
				}
			}
		}

		return actions.ToArray();
	}

	public Item[] GetItems()
	{
		return itemsInside;
	}

	public override bool PassItem(Character character, Item otherItem, Action action)
	{		
		if(action.GetType() == typeof(ActionPlace))
		{
			for (int i = 0, length = itemsInside.Length; i < length; i++) 
			{
				if(itemsInside[i] == null)
				{
					itemsInside[i] = otherItem.Copy();
					return true;
				}
			}
		}

		return false;
	}

	public override Item RequestItem (Character character, Item otherItem, int index, Action action)
	{
		if(action.GetType() == typeof(ActionRecover))
		{
			return Utils.FindAndRemoveItem(itemsInside, otherItem);

//			for (int i = 0, length = itemsInside.Length; i < length; i++) 
//			{
////				Item itemFound = Utils.FindItem(otherItem);
//
//				if(itemsInside[i] != null && itemsInside[i].Equals(otherItem))
//				{
//					itemsInside[i] = null;
//					return null;
//				}
//			}
		}

		return null;
	}

	public override bool Equals (object obj)
	{		
		Car item = obj as Car;
		if(item == null)
		{
			return false;
		} 
		else 
		{
			//TODO, mff items inside should be compared here
			return true;
		}
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		int wheelCount = 0;

		for (int i = 0, length = itemsInside.Length; i < length; i++) 
		{
			if(itemsInside[i] != null)
				wheelCount++;
		}

		if(wheelCount == 0)
		{
			return "Car without wheels";
		}
		else if(wheelCount == 1)
		{
			return "Car with 1 wheel";
		}
		else
		{			
			return "Car with "+wheelCount+" wheels";
		}
	}
}
