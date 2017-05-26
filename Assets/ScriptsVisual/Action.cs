using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public abstract class Action
	{
		public int time;
		public int duration = 1;

		public virtual bool Perform(Character character, int currentTime)
		{
			return true;
		}
	}
}
