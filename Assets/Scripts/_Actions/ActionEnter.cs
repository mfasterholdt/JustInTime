using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionEnter : Action 
{
	public Location targetLocation;

	public ActionEnter(int time, Location targetLocation, int duration = 1 )
	{
		this.name = "Enter "+targetLocation.name;
		this.time = time;
		this.duration = duration;
		this.targetLocation = targetLocation;
	}

	public override bool Perform (Character character)
	{
		if(Utils.CheckCrossingCharacters(time, this, character, targetLocation) == false)
			return false;

		character.SetCurrentLocation(targetLocation);
		
		return true;	
	}	
}
