using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TeddyLevel : Level
{
	[Space(10)]
	[Header("---- Items ----")]
	public Fire fire;

	[Header("---- Locations ----")]
	public Location shop;
	public Location road;
	public Location garden;
	public Location bakery;

	[Header("---- Characters ----")]
	public CustomerSetup customerSetup;

	public override void Setup ()
	{
		base.Setup ();

		TeddyBear newTeddyBear = new TeddyBear(false);
		bakery.initialItems.Add(newTeddyBear);

		bakery.initialItems.Add(fire.Copy());
	}

	public override void Load ()
	{
		characters.Clear();

		Customer customer = customerSetup.CreateCustomer();

		characters.Add(customer);
	}

	public override Action GoalCheck (int currentTime, Character character, bool timeIsUp)
	{
		TeddyBear teddyBear = Utils.FindItemOfType<TeddyBear>(character.GetCurrentLocation(), true, true, false);

		if(teddyBear != null)
		{
			GameManager.levelCompleteText = character.GetName() +" is ecstatic to see the Teddy Bear unharmed :D";
			GameManager.levelComplete = true;
		}
		else
		{
			GameManager.paradox = new Paradox(null, character.GetName() +" did not see the Teddy Bear and is inconsolable");
			GameManager.Instance.paradoxFound = true;
		}
			
		return null;
	}

	public override void MakeObservations(int currentTime, Character character)
	{
		//Observe fire in bakery from garden
		if(character.GetCurrentLocation() == garden)
		{
			List<Item> items = new List<Item>();

			for (int i = 0, count = bakery.items.Count; i < count; i++) 
			{
				Fire fire = bakery.items[i] as Fire;

				if(fire != null)
					items.Add(fire);
			} 

			Observation fireObservation = new Observation(currentTime, bakery, new List<Character>(), items, false);
			character.observations.Add(fireObservation);	
		}
	}

	public override Action[] GetLevelActions (int currentTime, Character player)
	{
		Location currentLocation = player.GetCurrentLocation();

		if(currentLocation == shop)
		{
			ActionDrive actionDrive = new ActionDrive(currentTime, 10, "Drive to the "+bakery.name, road, garden);
			return new Action[1]{actionDrive};
		}
		else if(currentLocation == garden)
		{
			ActionDrive actionDrive = new ActionDrive(currentTime, 10, "Drive to the "+shop.name, road, shop);
			return new Action[1]{actionDrive};
		}

		return new Action[0];
	}
}
