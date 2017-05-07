using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public abstract class Item
{	
	public bool pickup = true;
	public int age = 0;
	public bool useTimeline = false;

	public abstract Item Copy();

	public virtual Action[] GetLocationActions(int currentTime, int locationIndex, Character player)
	{	
		return null;
	}

	public virtual Action[] GetInventoryActions(int currentTime, int inventoryIndex,  Character player)
	{		
		return null;
	}

	public virtual bool PassAction(Action action)
	{
		return true;
	}

	public virtual bool PassItem(Character character, Item otherItem, Action action)
	{
		return true;
	}

	public virtual Item RequestItem(Character character, Item otherItem, int index, Action action)
	{
		return null;
	}

	public virtual bool UseItem(Item otherItem)
	{
		return true;	
	}

	public virtual bool ToggleItem(Character character)
	{
		return true;	
	}

	public virtual bool TickLocation(Location location)
	{
		return true;
	}

	public virtual bool CheckType(Item item)
	{
		if(item == null)
			return false;
		
		return GetType() == item.GetType();
	}

	public override bool Equals (object obj)
	{		
		Item item = obj as Item;
		if(item == null)
		{
			return false;
		} 
		else 
		{
			return CheckType(item);		
		}
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		return this.GetType().ToString();
	}

	public virtual string ToShortString ()
	{
		return this.GetType().ToString();
	}
}
