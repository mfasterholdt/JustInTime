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
        public int time;

        public Paradox(int time, GameObject visuals, Character character)
		{		
            this.time = time;
			this.visuals = visuals;
			this.character = character;			
		}

        public Paradox(int time, GameObject visuals, Vector3 position)
        {       
            this.time = time;
            this.visuals = visuals;
            this.position = position;
        }
	}
}
