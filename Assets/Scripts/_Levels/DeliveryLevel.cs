using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeliveryLevel : Level 
{	
	[Space(10)]
	[Header("---- Items ----")]
	public Location boxLocation;
	private Item box;

	[Space(10)]
	public Ingredients ingredients;
	public Location ingredientsLocation;

	[Space(10)]
	public Oven oven;
	public Location[] ovenLocations;

	[Header("---- Characters ----")]
	public CustomerSetup customerSetup;
	public string driverName;
	public Location driverInitialLocation;
	public ScheduleEntry[] driverSchedule;

	public override void Setup () 
	{			
		base.Setup();

		box = new Box(true, new Item[1], false);

		boxLocation.initialItems.Add(box.Copy());
		ingredientsLocation.initialItems.Add(ingredients.Copy());

		for (int i = 0, length = ovenLocations.Length; i < length; i++) 
			ovenLocations[i].initialItems.Add(oven.Copy());
	}

	public override void Load()
	{
		characters.Clear();

		Customer customer = customerSetup.CreateCustomer(new List<Item>(), box.Copy());

		characters.Add(customer);
	}

	public override Action GoalCheck (int currentTime, Character character, bool timeIsUp)
	{
		if(CakeCheck(character))
		{
			GameManager.levelCompleteText = character.GetName() +" eats the cake with much glee :D";
			GameManager.levelComplete = true;	
		}
		else
		{
			GameManager.paradox = new Paradox(null, character.GetName() +" was not satisfied with the content of the box and is inconsolable");
			GameManager.Instance.paradoxFound = true;
		}

		return null;
	}

	public bool CakeCheck(Character character)
	{
		if(character.inventory.Count > 0)
		{
			Box box = character.inventory[0] as Box;
			if(box != null)
			{
				Item[] items = box.GetItems();
				if(items.Length > 0)
				{
					Cake cake = items[0] as Cake;

					if(cake != null && cake.state == Cake.CakeState.Finished)
						return true;
				}
			}
		}

		return false;
	}
}
