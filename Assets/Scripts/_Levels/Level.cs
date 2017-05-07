using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class Level : MonoBehaviour 
{
	public Location startLocation;

	private Location[] locations;

	protected List<Character> characters = new List<Character>();

	public virtual void Setup () 
	{
		if(!startLocation)
			Debug.LogError("Missing start location");
		
		locations = GetComponentsInChildren<Location>();
	}

	public virtual void Reset () 
	{	
		for (int i = 0, length = locations.Length; i < length; i++) 
		{
			locations[i].Reset();	
		}
	}

	public virtual void Load()
	{
	}

	//TODO, mff if I keep this it could probably be abstract
	public virtual Action GoalCheck(int currentTime, Character character, bool timeIsUp)
	{
		return null;	
	}

	public virtual List<Character> GetInitialCharacters()
	{
		return characters;
	}
	
 	public virtual List<Item> GetInitialInventory()
	{
		return new List<Item>();
	}

//	public virtual List<Item> GetInitialInventory ()
//	{
//		List<Item> initialInventory = new List<Item>();			
//		initialInventory.Add(new Cake(10, 10, Cake.CakeState.Finished));
//		return initialInventory;
//	}

	public virtual Action[] GetLevelActions(int currentTime, Character player)
	{
		return new Action[0];
	}

	public virtual Location[] GetLocations()
	{
		return locations;	
	}

	public virtual void MakeObservations(int currentTime, Character character)
	{
	}

	public virtual bool Tick(int currentTime, Character player)
	{	
		for (int i = 0, length = locations.Length; i < length; i++) 
		{
			if(locations[i].Tick(currentTime, player) == false)
				return false;	
		}	

		return true;
	}

	public virtual List<Location> GetTimeMachineConnections(Location timeMachine)
	{
		List<Location> timeMachineLocations = new List<Location>();

		for (int i = 0, count = locations.Length; i < count; i++) 
		{
			Location location = locations[i];

			for (int j = 0, connectedCount = location.connectedLocations.Count; j < connectedCount; j++) 
			{				
				if(location.connectedLocations[j] == timeMachine)
				{					
					timeMachineLocations.Add(location);	
					break;
				}
			}
		}

		return timeMachineLocations;
	}
}
