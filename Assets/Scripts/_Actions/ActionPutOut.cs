using UnityEngine;
using System.Collections;

public class ActionPutOut<T> : Action where T : Item
{	
	public ActionPutOut (int time, int duration, string name)
	{
		this.name = name;				
		this.time = time;
		this.duration = duration;
	}

	//TODO, mff consider if a general structure can be reused across actions instead of making new once
	//There seem to be certain similarities between them
	//Generic as well?
	public override bool Perform (Character character)
	{
		Location location = character.GetCurrentLocation();

		if(location)
		{
			Item item = Utils.FindItemOfType<T>(location);
			if(item != null)
				item.PassAction(this);
		}

		return true;
	}
}
