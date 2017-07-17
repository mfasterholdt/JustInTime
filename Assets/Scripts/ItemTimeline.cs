using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public struct ItemEntry
{
	public int time;
	public int age;
	public Item item;
	public bool fixedItem;

	public int inventoryIndex;
	public int locationIndex;
	public Location location;

	public ItemEntry(int time, int age, Item item, int inventoryIndex, int locationIndex, Location location)
	{
		this.time = time;
		this.age = age;

		//TODO, mff fix this nonsense
		this.fixedItem = false;

		if(item is Box)
		{
			Item previousItem = GameManager.Instance.itemTimeline.GetItemInside(time - 1, age - 1);

			if(previousItem == null)
			{
				this.fixedItem = true;
			}
			else
			{
				if(previousItem.Equals(item) == false)
				{
					this.fixedItem = true;
				}
				else
				{
					
					Box otherBox = previousItem as Box;
					Box box = item as Box;
					//if(box)
					//{
					Item item1 = box.GetItems()[0];
					Item item2 = otherBox.GetItems()[0];
					if((item1 == null && item2 != null) || (item1 != null && item2 == null))
					{					
						this.fixedItem = true;
					}
					//}
		//			this.fixedItem = previousItem == null || ;
				}
			}
		}

		this.item = item.Copy();
		this.inventoryIndex = inventoryIndex;
		this.locationIndex = locationIndex;
		this.location = location;
	}

	public override string ToString ()
	{
		if(fixedItem)
			return  " time "+time+", age "+age+", "+item.ToString();
		else
			return  " time "+time+", age "+age;
	}
}

public class ItemTimeline
{
	//TODO, mff probably not used anymore
	public List<Item> itemInstances = new List<Item>();

	public List<Character> characters;
	//public List<ItemEntry> mergedTimeline;

	private float fieldWidth = 230;
	private float fieldHeight = 22;

	public void Ripple(int currentTime)
	{
		Debug.Log("Ripple");
	}

	public void Load()
	{
		itemInstances.Clear();

		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			characters[i].timeline.Clear();	
		}
	}

//	public void AddEntry(int currentTime, Character currentPlayer)
//	{
//		for (int i = 0, count = itemInstances.Count; i < count; i++) 
//		{
//			Item item = itemInstances[i];
//
//			if(ItemInsideTimeMachine(currentTime, item) == false)
//			{
//				ItemEntry entry = new ItemEntry(currentTime, item.age);	
//				currentPlayer.timeline.Add(entry);
//			}
//		}
//
//		currentPlayer.timeline = currentPlayer.timeline.OrderBy(x => x.age).ToList();
//	}

	public void AddEntry(int currentTime, Item item, Character currentPlayer, int age, int inventoryIndex, int locationIndex, Location location)
	{		
		ItemEntry entry = new ItemEntry(currentTime, age, item, inventoryIndex, locationIndex, location);
		currentPlayer.timeline.Add(entry);		
	}
		
	public bool ItemInsideTimeMachine(int currentTime, Item item)
	{
		for (int i = 0, count = GameManager.Instance.characters.Count; i < count; i++) 
		{
			Character character = GameManager.Instance.characters[i];

			bool insideTimeMachine = character.GetCurrentLocation() == GameManager.Instance.timeMachine;
			if(insideTimeMachine)
			{
				Action action = character.GetAction(currentTime);

				if(action != null)
				{
					ActionEnter actionEnter = action as ActionEnter;

					if(actionEnter != null)
						insideTimeMachine = false;
				}
			}

			if(insideTimeMachine)
			{
				for (int j = 0, itemCount = character.inventory.Count; j < itemCount; j++) 
				{					
					if(character.inventory[j] == item)
						return true;
				}
			}
		}

		return false;
	}

	public void PassItems(List<Item> inventory)
	{
		for (int i = 0, count = inventory.Count; i < count; i++) 
		{
			itemInstances.Add(inventory[i]);	
		}
	}

	//TODO, mff really not of this is needed any more?
	public void TimeTravel(int currentTime)
	{

//		for (int i = 0, count = previousTimeline.Count; i < count; i++) 
//		{
//			ItemEntry itemEntry = previousTimeline[i];
//
//			if(itemEntry.time >= currentTime)
//			{
//				timeline.Add(itemEntry);
//			}
//		}
//
//		timeline = timeline.OrderBy(x => x.age).ToList();
//		previousTimeline = new List<ItemEntry>(timeline);
//
//		timeline.Clear();
	}

	public ItemEntry GetLatestFixedEntry(int currentTime, int age, int locationIndex)
	{
		List<ItemEntry> mergedTimeline = GetMergedTimeline(currentTime);

		bool foundCurrentEntry = false;

		for (int i = mergedTimeline.Count - 1; i >= 0; i--) 
		{
			ItemEntry entry = mergedTimeline[i];

			if(foundCurrentEntry == false && entry.age == age && entry.locationIndex == locationIndex)
			{
				foundCurrentEntry = true;
			}

			if(foundCurrentEntry && entry.fixedItem)
			{
				return entry;
			}
		}

		//TODO, this can be fixed if class instead of struct
		Debug.LogWarning("this should probably not happen");

		return mergedTimeline[0];
	}

	public Item GetItemInside(int currentTime, int age)
	{
		List<ItemEntry> mergedTimeline = GetMergedTimeline(currentTime);

		for (int i = 0, count = mergedTimeline.Count; i < count; i++) 
		{
			ItemEntry entry = mergedTimeline[i];

			if(entry.age == age)
			{
				return entry.item;
			}
		}

		return null;
	}

	public int GetAgeInventory(int currentTime, int age, Item item, int inventoryIndex)
	{
		List<ItemEntry> mergedTimeline = GetMergedTimeline(currentTime);

		for (int i = 0, count = mergedTimeline.Count; i < count; i++) 
		{
			ItemEntry entry = mergedTimeline[i];
			//Debug.Log(entry.time+" = "+(currentTime - 1));
			//Debug.Log(entry.inventoryIndex+" = "+inventoryIndex);

//			if(entry.time == currentTime - 1 && entry.inventoryIndex == inventoryIndex)
//			{
//				return i + 1;
//			}
			//TODO this comparison automatically use next entry in line if correct age is not found 
			if(entry.age >= age && entry.inventoryIndex == inventoryIndex)
			{
				return i + 1;
			}	
		}

		Debug.LogWarning(item.ToString() +" with age "+item.age +" was not found in timeline");

		return 0;
	}

	public List<ItemEntry> GetMergedTimeline(int currentTime)
	{
		List<ItemEntry> newMergedTimeline = new List<ItemEntry>(characters.Last().timeline);

		int currentMergeTime = currentTime;
		int nextMergeTime = currentTime;

		for (int i = characters.Count - 2; i >= 0; i--) 
		{
			Character character = characters[i];

			for (int j = 0, count = character.timeline.Count; j < count; j++) 
			{
				ItemEntry entry = character.timeline[j];
				int entryTime = entry.time;

				if(entryTime > currentMergeTime)
				{
					newMergedTimeline.Add(entry);

					if(entryTime > nextMergeTime)
						nextMergeTime = entryTime;
				}
			}

			currentMergeTime = nextMergeTime;
		}

		return newMergedTimeline.OrderBy(x => x.age).ToList();
	}

	public void Draw(int currentTime, Vector2 posGUI)
	{
		//return;

		int characterCount = characters.Count;

		int currentMergeTime = currentTime;
		int nextMergeTime = currentTime;

		for (int i = characterCount - 1; i >= 0; i--) 
		{		
			Character character = characters[i];
			int offsetX = characterCount - i;

			for (int j = 0, itemCount = character.timeline.Count; j < itemCount; j++) 
			{
				ItemEntry entry = character.timeline[j];
				if(i == characterCount - 1)
				{
					GUI.contentColor = Color.green;
				}
				else
				{
					if(entry.time > currentMergeTime)
					{
						GUI.contentColor = Color.green;

						if(entry.time > nextMergeTime)
							nextMergeTime = entry.time;
					}
					else
					{
						GUI.contentColor = Color.red;
					}
				}

				GUI.Label(new Rect(posGUI.x - (offsetX * 100), posGUI.y + fieldHeight * entry.age, fieldWidth, fieldHeight), entry.ToString());
			}

			currentMergeTime = nextMergeTime;
		}


		Color white = Color.white;
		white.a = 0.5f;
		GUI.contentColor = white;

		List<ItemEntry> mergedTimeline = GetMergedTimeline(currentTime);

//		List<ItemEntry> mergedTimeline = new List<ItemEntry>(characters.Last().timeline);

//		currentMergeTime = currentTime;
//		nextMergeTime = currentTime;
//
//		for (int i = characterCount - 2; i >= 0; i--) 
//		{
//			Character character = characters[i];
//
//			for (int j = 0, count = character.timeline.Count; j < count; j++) 
//			{
//				ItemEntry entry = character.timeline[j];
//				int entryTime = entry.time;
//
//				if(entryTime >= currentMergeTime)
//				{
//					mergedTimeline.Add(entry);
//
//					if(entryTime >= nextMergeTime)
//						nextMergeTime = entryTime + 1;
//				}
//			}
//
//			currentMergeTime = nextMergeTime;
//		}
			
		//mergedTimeline = mergedTimeline.OrderBy(x => x.age).ToList();

		for (int i = 0, count = mergedTimeline.Count; i < count; i++) 
		{
			ItemEntry entry = mergedTimeline[i];
			int ageFinal = i + 1;
			GUI.Label(new Rect(posGUI.x, posGUI.y + fieldHeight * ageFinal, fieldWidth, fieldHeight), entry.ToString()+", final "+ageFinal);
		}

		return;
	}
}
