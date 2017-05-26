using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public struct Observation
	{
		public int time;
		public Location location;
		public List<Character> characters;

		public Observation(int currentTime, Location location, List<Character> characters)
		{
			this.time = currentTime;
			this.location = location;
			this.characters = new List<Character>(characters);
		}
	}
}