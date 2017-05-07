using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TireChangerLevel : Level 
{	
	[Space(10)]
	[Header("---- Items ----")]
	public Wheel wheel;
	public int initialWheels = 1;

	private Cake cake = new Cake(5, 5, Cake.CakeState.Finished);

	[Header("---- Locations ----")]
	public Location parkingLot;
	public Location road;
	public Location crossRoad;
	public Location hidden;

	[Header("---- Characters ----")]
	public CustomerSetup customerSetup;

	public override void Setup () 
	{			
		base.Setup();

		Item[] itemsInsideCar = new Item[4];

		for (int i = 0; i < initialWheels; i++) 
		{
			itemsInsideCar[i] = wheel.Copy();	
		}

		Car newCar = new Car(false, itemsInsideCar);
		parkingLot.initialItems.Add(newCar);
	}

	public override void Load()
	{
		characters.Clear();

		Customer customer = customerSetup.CreateCustomer();
		characters.Add(customer);
	}

	public override Action GoalCheck (int currentTime, Character character, bool timeIsUp)
	{
		Location currentLocation = character.GetCurrentLocation();	
	
		Cake cakeFound = Utils.FindItemOfType<Cake>(currentLocation);

		if(cakeFound != null)
		{
			GameManager.levelCompleteText = character.GetName() +" is pleased with the delivery :D";
			GameManager.levelComplete = true;

			return new ActionPickup(currentTime, cakeFound, 0);
		}
		else
		{
			GameManager.paradox = new Paradox(null, character.GetName() +" did not find his delivery and is inconsolable");
			GameManager.Instance.paradoxFound = true;
		}

		return null;
	}

	public override List<Item> GetInitialInventory ()
	{
		return new List<Item>(){cake.Copy()};
	}

	public override Action[] GetLevelActions (int currentTime, Character currentPlayer)
	{
		Location currentLocation = currentPlayer.GetCurrentLocation();
		List<Action> actions = new List<Action>();

		for (int i = 0, count = currentLocation.items.Count; i < count; i++) 
		{
			Car car = currentLocation.items[i] as Car;

			if(car != null)
			{
				int wheelCount = 0;
				Item[] itemsInside = car.GetItems();
				for (int j = 0, length = itemsInside.Length; j < length; j++) 
				{
					if(itemsInside[j] != null)
						wheelCount++;
				}

				if(wheelCount >= itemsInside.Length)
				{
					Location targetLocation = null;

					if(currentLocation == parkingLot)
						targetLocation = crossRoad;
					else if(currentLocation == crossRoad)
						targetLocation = parkingLot;
				
					if(targetLocation != null)
					{
						ActionDrive actionDrive = new ActionDrive(currentTime, 4, "Drive to the "+targetLocation.name, road, targetLocation, car); 
						actions.Add(actionDrive);
					}
				}
			}
		}

		return actions.ToArray();
	}
}
