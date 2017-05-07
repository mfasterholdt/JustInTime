using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionEnter : Action 
	{
		public Location location;

		public ActionEnter(int time, Location location, int duration)
		{
			this.time = time;
			this.location = location;
			this.duration = duration;
		}

		public override bool Perform (Character character)	
		{
//			character.SetCurrentLocation (location);
			return true;
		}
	}
}
