using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Location : MonoBehaviour
{	
	public Vector2 placement;
	public List<Location> connectedLocations;
	public List<Item> items = new List<Item>();

	public List<Item> initialItems = new List<Item>();

	[HideInInspector]
	public List<Character>characters = new List<Character>();

	public void Reset()
	{
		characters.Clear();

		items = Utils.CopyItemList(initialItems);
	}

	public virtual bool Tick(int currentTime, Character player)
	{
		for (int i = 0, count = items.Count; i < count; i++) 
		{
			if(items[i].TickLocation(this) == false)
				return false;	
		}

		return true;
	}

}
