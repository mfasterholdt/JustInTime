using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Paradox
	{
		public GameObject visuals;

		public Character character;
		public Vector3 position;

        public Paradox(GameObject visuals, Character character)
		{		
			this.visuals = visuals;
			this.character = character;			
		}

        public Paradox(GameObject visuals, Vector3 position)
        {       
            this.visuals = visuals;
            this.position = position;
        }
	}
}
