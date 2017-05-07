using UnityEngine;
using System.Collections;

public class ActionDrive : Action
{
	Location targetLocation;
	Location roadLocation;
	Item vehicle;

	public ActionDrive(int time, int driveDuration, string name, Location roadLocation, Location targetLocation, Item vehicle = null)
	{
		this.time = time;
		this.duration = driveDuration;
		this.name = name;

		this.targetLocation = targetLocation;
		this.roadLocation = roadLocation;
		this.vehicle = vehicle;
	}

	public override bool Prepare (Character character)
	{
		Location currentLocation = character.GetCurrentLocation();

		if(currentLocation != roadLocation)
		{
			//Move Vehicle
			if(vehicle != null)
			{
				Item itemInstance = currentLocation.items.Find(x => x.Equals(vehicle));

				if(itemInstance == null || !currentLocation.items.Remove(itemInstance))
				{
					GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+vehicle.ToString()+" in the "+currentLocation.name+", Paradox!");
					return false;
				}

				roadLocation.items.Add(itemInstance.Copy());
			}

			//Move character
			if(Utils.CheckCrossingCharacters(time, this, character,roadLocation) == false)
				return false;
			
			character.SetCurrentLocation(roadLocation);

			return true;
		}
		
		return true;
	}

	public override bool Perform (Character character)
	{
		Location currentLocation = character.GetCurrentLocation();

		//Move Vehicle
		if(vehicle != null)
		{
			Item itemInstance = currentLocation.items.Find(x => x.Equals(vehicle));

			if(itemInstance == null || !currentLocation.items.Remove(itemInstance))
			{
				GameManager.paradox = new Paradox(this, character.GetName()+" could not find "+vehicle.ToString()+" in the "+currentLocation.name+", Paradox!");
				return false;
			}

			targetLocation.items.Add(itemInstance.Copy());
		}

		//Move Character
		if(Utils.CheckCrossingCharacters(time, this, character,targetLocation) == false)
			return false;
		
		character.SetCurrentLocation(targetLocation);

		return true;
	}
}
