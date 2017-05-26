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

			Item[] items = GetComponentsInChildren<Item> ();

			for (int i = 0, length = items.Length; i < length; i++) 
			{
				items [i].SetId(i);	
			}
		}
	}
}
