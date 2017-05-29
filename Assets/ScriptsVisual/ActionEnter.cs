using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionEnter : Action 
	{
		public Location fromLocation;
		public Location toLocation;

		public ActionEnter(int time, int duration, Location fromLocation, Location toLocation)
		{
			this.time = time;
			this.duration = duration;
			this.fromLocation = fromLocation;
			this.toLocation = toLocation;
		}

		public override bool Perform (Character character, int currentTime)	
		{
			character.MoveLocation (toLocation);

			return true;
		}

		public override string ToString ()
		{
			return fromLocation+"  ->  "+ toLocation + "  [ActionEnter]";
		}
	}
}
