using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class ActionWait : Action
	{
		public ActionWait(int time, int duration)
		{
			this.time = time;
			this.duration = duration;
		}
	}
}
