using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Utils
{
	public static List<Item> CopyItemList(List<Item> items)
	{
		int itemCount = items.Count;

		List<Item> newItems = new List<Item>(itemCount);

		for (int i = 0; i < itemCount; i++) 
		{
			newItems.Add(items[i].Copy());
		}

		return newItems;
	}

	public static Item[] CopyItemArray(Item[] items)
	{
		int itemLength = items.Length;

		Item[] newItems = new Item[itemLength];

		for (int i = 0; i < itemLength; i++) 
		{
			Item itemToCopy = items[i];

			if(itemToCopy != null)
				newItems[i] = itemToCopy.Copy();
		}

		return newItems;
	}

	public static Item FindItem(List<Item> itemList, Item itemFind)
	{
		for (int i = 0, count = itemList.Count ; i < count; i++) 
		{
			Item item = itemList[i];

			if(item != null && item.Equals(itemFind))
				return item;
		}

		return null;
	}

	public static Item FindAndRemoveItem(Item[] itemArray, Item itemFind)
	{
		for (int i = 0, length = itemArray.Length; i < length; i++) 
		{
			Item item = itemArray[i];

			if(item != null && item.Equals(itemFind))
			{
				itemArray[i] = null;
				return item;
			}
		}

		return null;
	}

	public static T FindItemOfType<T>(List<Item> itemList) where T : class
	{
		for (int i = 0, count = itemList.Count ; i < count; i++) 
		{
			T item = itemList[i] as T;

			if(item != null)
				return item;
		}

		return null;
	}

	public static T FindItemOfType<T>(Location location) where T : class
	{
		for (int i = 0, count = location.items.Count; i < count; i++) 
		{
			T item = location.items[i] as T;

			if(item != null)
				return item;
		}

		return null;
	}

	public static T FindItemOfType<T>(Location location, bool checkLocation, bool checkInventories, bool checkInsideBoxes) where T : class
	{
		if(checkLocation)
		{
			for (int i = 0, count = location.items.Count; i < count; i++) 
			{
				T item = location.items[i] as T;

				if(item != null)
					return item;
			}
		}

		if(checkInventories)
		{
			for (int i = 0, characterCount = location.characters.Count; i < characterCount; i++) 
			{
				Character character = location.characters[i];

				for (int j = 0, count = character.inventory.Count; j < count; j++) 
				{
					T item = character.inventory[j] as T;

					if(item != null)
						return item;
				}
			}
		}

		//TODO, mff add box check here

		return null;
	}
	public static bool CheckCrossingCharacters(int time, Action action, Character character, Location targetLocation)
	{
		for (int i = 0, count = targetLocation.characters.Count; i < count; i++)
		{
			Character otherCharacter = targetLocation.characters[i];

			if(otherCharacter.CheckIdentity(character))
			{	
				Action nextAction = otherCharacter.GetAction(time);

				if(nextAction != null)
				{
					ActionEnter nextActionEnter = nextAction as ActionEnter;

					if(nextActionEnter != null && nextActionEnter.targetLocation == character.GetCurrentLocation())
					{
						GameManager.paradox = new Paradox(action, character.GetName()+" met "+otherCharacter.GetName()+" between "+character.GetCurrentLocation().name+" and "+targetLocation.name+", Paradox!");
						return false;	
					}
				}
			}	
		}

		return true;
	}
}
