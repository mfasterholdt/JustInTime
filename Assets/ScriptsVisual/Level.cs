using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Level : MonoBehaviour 
	{
		public Location initialLocation;

		[HideInInspector]
		public Location[] locations;

		void Awake () 
		{
			locations = GetComponentsInChildren<Location> ();
		}
		
		// Update is called once per frame
		void Update () 
		{
			
		}
	}
}
