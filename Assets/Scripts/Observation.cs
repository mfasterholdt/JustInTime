using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct Observation 
{
	public int time;
	public Location location;
	public List<Character> characters;
	public List<Item> items;
	public bool checkUnexpected;

	public Observation(int currentTime, Location location, List<Character> characters, List<Item> items, bool checkUnexpected)
	{
		this.time = currentTime;
		this.location = location;
		this.characters = new List<Character>(characters);
		this.items = Utils.CopyItemList(items);
		this.checkUnexpected = checkUnexpected;
	}

	public bool CheckObservation(Character character)
	{
		//----// Characters //----//
		List<Character> currentCharacters = new List<Character>(location.characters);
		for (int j = 0, count = characters.Count; j < count; j++)
		{
			Character expectedCharacter = characters[j];
			if(currentCharacters.Remove(expectedCharacter) == false)
			{
				GameManager.paradox = new Paradox(this, character.GetName()+" did not see "+expectedCharacter.GetName()+" in the "+location.name+", Paradox!");
				return false;
			}
		}

		if(checkUnexpected)
		{
			int unexpectedCharactersCount = currentCharacters.Count;
			if(unexpectedCharactersCount > 0)
			{
				string paradoxMessage = character.GetName()+" did not expect to see ";

				for (int j = 0; j < unexpectedCharactersCount; j++) 
				{	
					if(j > 0)
						paradoxMessage += " and ";

					paradoxMessage += currentCharacters[j].GetName();	
				}

				paradoxMessage += " in the "+location.name+", Paradox!";

				GameManager.paradox = new Paradox(this, paradoxMessage);

				return false;
			}
		}

		//----// Items //----//
		List<Item> currentItems = new List<Item>(location.items);

		for (int j = 0, count = items.Count; j < count; j++) 
		{
			Item expectedItem = items[j];
			if(currentItems.Remove(expectedItem) == false)
			{				
				GameManager.paradox = new Paradox(this, character.GetName()+" did not see "+expectedItem+" in the "+location.name+", Paradox!");
				return false;
			}	
		}

		if(checkUnexpected)
		{
			int unexpectedItemsCount = currentItems.Count; 
			if(unexpectedItemsCount > 0)
			{
				string paradoxMessage = character.GetName()+" saw ";

				for (int j = 0; j < unexpectedItemsCount; j++) 
				{	
					if(j > 0)
						paradoxMessage += " and ";

					paradoxMessage += currentItems[j].ToString();	
				}

				paradoxMessage += " in the "+location.name+", Paradox!";

				GameManager.paradox = new Paradox(this, paradoxMessage);
				return false;
			}
		}
		return true;
	}
}
