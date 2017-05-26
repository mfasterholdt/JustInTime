using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Paradox
	{
		public GameObject visuals;

		public Character character;
		public Transform targetPosition;

		public Paradox(GameObject visuals = null, Character character = null, Transform targetPosition = null)
		{		
			this.visuals = visuals;
			this.character = character;
			this.targetPosition = targetPosition;
		}
	}
}
