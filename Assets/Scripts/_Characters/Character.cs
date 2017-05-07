using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//TODO, mff consider making a copy structure like with items
//This might be required to probably check items of observed players?

[System.Serializable]
public class Character
{	
	public List<Action> history = new List<Action>();

	//Timeline as it happened during this iteration, could make more sense to have seperate from character
	public List<ItemEntry> timeline = new List<ItemEntry>();

	//TODO, mff consider using hash table with time as key
	//this might also be possible for history and other lists
	//how does hash tables deal with adding and removing, max allocation?
	public List<Observation> observations = new List<Observation>();
	public Location initialLocation;

	public List<Item> inventory;

	//TODO, mff this is not used for player, so it seems a little miss placed here
	public int timeForNextDecision = 0;

	protected int id;
	protected int iteration;
	protected string name;

	public List<Item> initialInventory;
	protected Location currentLocation;

	public Character()
	{
	}

	public Character(Location initialLocation, List<Item> items, int id, int iteration, string name)
	{	
		initialInventory = Utils.CopyItemList(items);
		inventory = Utils.CopyItemList(initialInventory);

		this.id = id;
		this.iteration = iteration;
		this.name = name;
		this.initialLocation = initialLocation;

		SetCurrentLocation(initialLocation);
	}

	public void Reset()
	{
		SetCurrentLocation(initialLocation);
		inventory = Utils.CopyItemList(initialInventory);
	}

	public virtual void Tick(int currentTime)
	{
	}
		
	//TODO, mff consider what is happening in GameManager, Character and Observation, in regards to this check
	public virtual bool CheckObservation(int currentTime)
	{
		int observationCount = observations.Count;

//		if(observationCount <= 0)
//			return true;
		
		for (int i = 0; i < observationCount; i++) 
		{
			Observation observation = observations[i];  

			if(observation.time == currentTime)
			{
				if(observation.CheckObservation(this) == false)
					return false;
				
//				//----// Characters //----//
//				Location observationLocation = observation.location;
//				List<Character> currentCharacters = new List<Character>(observationLocation.characters);
//				for (int j = 0, count = observation.characters.Count; j < count; j++)
//				{
//					Character expectedCharacter = observation.characters[j];
//
//					if(currentCharacters.Remove(expectedCharacter) == false)
//					{
//						GameManager.paradox = new Paradox(observation, this.GetName()+" did not see "+expectedCharacter.GetName()+" in the "+observationLocation.name+", Paradox!");
//						return false;
//					}
//				}
//
//				int unexpectedCharactersCount = currentCharacters.Count;
//
//				if(unexpectedCharactersCount > 0)
//				{
//					string paradoxMessage = this.GetName()+" did not expect to see ";
//
//					for (int j = 0; j < unexpectedCharactersCount; j++) 
//					{	
//						if(j > 0)
//							paradoxMessage += " and ";
//
//						paradoxMessage += currentCharacters[j].GetName();	
//					}
//
//					paradoxMessage += " in the "+observationLocation.name+", Paradox!";
//
//					GameManager.paradox = new Paradox(observation, paradoxMessage);
//
//					return false;
//				}
//
//				//----// Items //----//
//				List<Item> currentItems = new List<Item>(observationLocation.items);
//
//				for (int j = 0, count = observation.items.Count; j < count; j++) 
//				{
//					Item expectedItem = observation.items[j];
//					if(currentItems.Remove(expectedItem) == false)
//					{				
//						GameManager.paradox = new Paradox(observation, this.GetName()+" did not see "+expectedItem+" in the "+observationLocation.name+", Paradox!");
//						return false;
//					}	
//				}
//
//				int unexpectedItemsCount = currentItems.Count; 
//				if(unexpectedItemsCount > 0)
//				{
//					string paradoxMessage = this.GetName()+" saw ";
//
//					for (int j = 0; j < unexpectedItemsCount; j++) 
//					{	
//						if(j > 0)
//							paradoxMessage += " and ";
//
//						paradoxMessage += currentItems[j].ToString();	
//					}
//
//					paradoxMessage += " in the "+observationLocation.name+", Paradox!";
//
//					GameManager.paradox = new Paradox(observation, paradoxMessage);
//					return false;
//				}
			}	
		}

		return true;	
	}

	public virtual void MakeObservations(int currentTime, Level currrentLevel)
	{
		int observationCount = observations.Count;

		if(observationCount == 0 || currentTime > observations[observationCount - 1].time)
		{	
			//Make new observations

			//Current location
			Observation newObservation = new Observation(currentTime, currentLocation, currentLocation.characters, currentLocation.items, true);
			observations.Add(newObservation);

			//Level specific observations
			currrentLevel.MakeObservations(currentTime, this);
		}
	}

	public bool CheckIdentity(Character otherCharacter)
	{
		return id == otherCharacter.GetID();
	}

	public void SetCurrentLocation(Location newLocation)
	{
		if(currentLocation != null)
			currentLocation.characters.Remove(this);
		
		currentLocation = newLocation;
		
		if(newLocation != null)
			newLocation.characters.Add(this);
	}

	public Location GetCurrentLocation()
	{
		return currentLocation;
	}

	public void AddAction(Action action)
	{
		history.Add(action);
		timeForNextDecision += action.duration;	
	}

	public Action GetAction(int targetTime)
	{
		int time = 0;
		for (int i = 0, count = history.Count; i < count; i++) 
		{
			Action action = history[i];
			time += action.duration;

			if(time > targetTime)
				return action;
		}

		return null;
	}

	//TODO, mff consider if we can remove a whole range of actions in one go, maybe wait until refactoring lists
	public int RemoveAction()
	{
		int lastIndex = history.Count - 1; 
		Action action = history[lastIndex];
		history.RemoveAt(lastIndex);

		int decisionTime = action.time;
		timeForNextDecision = decisionTime;

		return decisionTime;
	}

	public int GetID()
	{
		return id;
	}

	public int GetIteration()
	{
		return iteration;
	}

	public string GetName(bool includeIteration = true)
	{
		if(includeIteration)
			return name+" "+iteration;
		else
			return name;
	}
}