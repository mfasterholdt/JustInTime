using UnityEngine;
using System.Collections;

public class RipplesLevel : Level
{
	public Location initialBoxLocation;
	public Location initialWatchLocation;

	public override void Setup () 
	{			
		base.Setup();

		if(initialWatchLocation)
		{
			initialWatchLocation.initialItems.Add(new Watch());
		}
		if(initialBoxLocation)
		{
			Paper paper = new Paper("Letter");

			Box box = new Box(true, new Item[1]{ paper }, false);

			initialBoxLocation.initialItems.Add(box);
		}
	}

	public override void Load()
	{
		
	}
}
