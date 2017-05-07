using UnityEngine;
using System.Collections;

public class ActionCheckAge : Action
{
	public Item item;
	public int inventoryIndex;
	public int age;
	private bool initialPerform = true;

	public ActionCheckAge(int time, Item item, int inventoryIndex)
	{
		this.time = time;
		this.item = item.Copy();
		this.inventoryIndex = inventoryIndex;
		this.age = item.age;
//
//		int currentAge = GameManager.Instance.itemTimeline.GetAgeInventory(time, item.age, item, inventoryIndex) + 1;
		//Debug.Log( (item.age+1) +" new age "+currentAge);

		this.name = "Check Age "+age;				
	}

	public override bool Perform (Character character)
	{
		if(character.inventory.Count < inventoryIndex + 1)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+" in the inventory, Paradox!");
			return false;
		}

		Item itemFound = character.inventory[inventoryIndex];

		if(itemFound == null || itemFound.Equals(item) == false)
		{
			GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+item.ToString()+" in the inventory, Paradox!");
			return false;		
		}
			
		int currentAge = GameManager.Instance.itemTimeline.GetAgeInventory(time, age, item, inventoryIndex);

		if(initialPerform)
		{
			//Debug.Log("corrected age from"+age+" to "+currentAge);
			age = currentAge;

			initialPerform = false;
		}
		else
		{
			//TODO, mff adding 1 to current time here might cause problems later
			if(currentAge != age)
			{
				Debug.Log("maybe paradox here if iteration 1");
//				GameManager.paradox = new Paradox(this, item.ToString()+" has age "+currentAge+" instead of the expected "+age+", Paradox!");
//				return false;
			}
		}
//
		return true;
	}

	//TODO, mff if paradox check is used instead of perform to check for paradoxes it should be possible to check for the ripple paradoxes only on time travel actions
	//Perhaps this will give a nice behaviour
	public override bool ParadoxCheck (int currentTime)
	{
		if(initialPerform == false)
		{
			int currentAge = GameManager.Instance.itemTimeline.GetAgeInventory(currentTime, age, item, inventoryIndex);

//			Debug.Log(currentAge+" != "+age);

			if(currentAge != age)
			{				
				return false;
			}
		}

		return true;
	}
}
