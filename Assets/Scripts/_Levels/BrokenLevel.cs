using UnityEngine;
using System.Collections;

public class BrokenLevel : Level
{
	public Location initialGlassLocation;

	public override void Setup () 
	{			
		base.Setup();

		initialGlassLocation.initialItems.Add(new Glass(false));

	}

	public override void Load()
	{
		
	}
}
