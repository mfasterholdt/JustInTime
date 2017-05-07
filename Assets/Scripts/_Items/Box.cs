using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Box : Item 
{
	public int capacity = 1;

	private Item[] itemsInside;
	private bool locked;

	public Box(bool pickup, Item[] itemsInside, bool locked, int age = 0)
	{
		this.pickup = pickup;
		this.itemsInside = Utils.CopyItemArray(itemsInside);
		this.capacity = itemsInside.Length;
		this.locked = locked;
		this.age = age;
		this.useTimeline = true;
	}

	public override Item Copy ()
	{	
		if(itemsInside == null)
			return new Box(pickup, new Item[capacity], locked, age);
		else
			return new Box(pickup, itemsInside, locked, age);
	}

	public override Action[] GetLocationActions(int currentTime, int locationIndex, Character character)
	{		
		List<Action> actions = new List<Action>();
		Key key = Utils.FindItemOfType<Key>(character.inventory);

		if(locked)
		{
			if(key != null)
			{
				ActionUse actionUse = new ActionUse(currentTime, "Unlock "+this.GetType(), key, this); 
				actions.Add(actionUse);
			}
		}
		else
		{
			if(key != null)
			{
				ActionUse actionUse = new ActionUse(currentTime, "Lock "+this.GetType(), key, this); 
				actions.Add(actionUse);
			}

			//TODO, this is a good start for handling box content through timeline
			if(GameManager.Instance.itemTimeline.characters[0].timeline.Count > 0)
			{
				ItemEntry itemEntry = GameManager.Instance.itemTimeline.GetLatestFixedEntry(currentTime, age, locationIndex);

				Box previousBox = itemEntry.item as Box;

				if(previousBox != null)
				{
					//Debug.Log(previousBox.ToString() +", "+locationIndex);
				}
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
					ActionRecover actionRecover = new ActionRecover(currentTime, "Take "+item.ToString()+" from Box "+age, item, i, this, locationIndex ); 
					actions.Add(actionRecover);
				}
			}

			if(freeSpace)
			{
				for(int i = 0, count = character.inventory.Count; i < count; i++)
				{
					Item item = character.inventory[i];

					if(item is Box == false)
					{
						ActionPlace actionPlace = new ActionPlace(currentTime, "Place "+item.ToString()+" in Box "+age, item, i, this, locationIndex ); 
						actions.Add(actionPlace);
					}
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
			if(locked)
			{
				GameManager.paradox = new Paradox(action, "Could not place "+otherItem.ToString()+", "+this.ToString()+" is Locked, Paradox!");
				return false;
			}

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

	public override Item RequestItem (Character character, Item otherItem, int itemIndex, Action action)
	{
		if(action.GetType() == typeof(ActionRecover))
		{
			if(locked)
			{
				GameManager.paradox = new Paradox(action, "Could not recover "+otherItem.ToString()+", "+this.ToString()+" is Locked, Paradox!");
				return null;
			}

			Item item = itemsInside[itemIndex];

			if(item != null && item.Equals(otherItem))
			{
				itemsInside[itemIndex] = null;
				return item;
			}

			//return Utils.FindAndRemoveItem(itemsInside, otherItem);

//			for (int i = 0, length = itemsInside.Length; i < length; i++) 
//			{
//				Item itemFound = itemsInside[i];
//
//				if(itemFound != null && itemFound.Equals(otherItem))
//				{
//					itemsInside[i] = null;
//					return itemFound;
//				}
//			}
		}

		return null;
	}

	public override bool UseItem (Item otherItem)
	{
		if(otherItem is Key)
		{
			locked = !locked;
		}
		else if(otherItem is Oven)
		{
			Oven oven = otherItem as Oven;

			Item itemRemoved = oven.RemoveItem(this);

			if(itemRemoved != null)
			{
				for (int j = 0, length = itemsInside.Length; j < length; j++) 
				{
					Item item = itemsInside[j];

					if(item != null && item is Key)
					{
						oven.AddItem(item);
					}						
				}
			}	
		}

		return true;
	}

	public override bool Equals (object obj)
	{		
		Box item = obj as Box;
		if(item == null)
		{
			return false;
		} 
		else 
		{
			return true;
		}
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		string itemText = "Box";

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

		if(locked)
			itemText += " Locked";
		
		itemText += " "+age;

		return itemText;
	}

	public override string ToShortString ()
	{
		return "Box "+age;
	}
}
