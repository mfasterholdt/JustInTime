using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CakeLevel : Level 
{
	[Space(10)]
	[Header("---- Items ----")]
	public Oven oven;
	public Ingredients ingredients;

	[Space(20)]
	public Location ingredientsLocation;

	[Space(10)]
	public Location[] ovenLocations;

	[Header("---- Locations ----")]
	public Location garden;

	[Header("---- Characters ----")]
	public CustomerSetup customerSetup;


	public override void Setup () 
	{			
		base.Setup();

		ingredientsLocation.initialItems.Add(ingredients.Copy());

		for (int i = 0, length = ovenLocations.Length; i < length; i++) 
			ovenLocations[i].initialItems.Add(oven.Copy());
	}

	public override void Load()
	{
		characters.Clear();

		Customer customer = customerSetup.CreateCustomer();
		characters.Add(customer);
	}

	public override Action GoalCheck (int currentTime, Character character, bool timeIsUp)
	{
		if(timeIsUp)
		{
			GameManager.paradox = new Paradox(null, character.GetName() +" never received cake and is inconsolable");
			GameManager.Instance.paradoxFound = true;
		}

		return null;
	}
		
	public override Action[] GetLevelActions(int currentTime, Character player)
	{
		Location currentLocation = player.GetCurrentLocation();
		List<Action> levelActions = new List<Action>();

		if(currentLocation == garden)
		{
			List<Item> inventory = player.inventory;
			for (int i = 0, count = inventory.Count; i < count; i++) 
			{
				Cake cake = inventory[i] as Cake;

				if(cake != null && cake.state == Cake.CakeState.Finished)
				{
					ActionDeliver actionDeliver = new ActionDeliver(currentTime, cake);
					levelActions.Add(actionDeliver);
				}
			}
		}

		return levelActions.ToArray();
	}
}