using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionEnter : Action 
	{
		public Location location;

		public ActionEnter(int time, int duration, Location location)
		{
			this.time = time;
			this.duration = duration;
			this.location = location;
		}

//		public override bool Perform (Character character)	
//		{
////			character.SetCurrentLocation (location);
//			return true;
//		}
	}
}
