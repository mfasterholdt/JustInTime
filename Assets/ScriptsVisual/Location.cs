using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Location : MonoBehaviour
	{
		public bool isTimeMachine;
		public GameObject cover;
		public Location[] connectedLocations;

		public List<Item> items = new List<Item>();

		void Awake()
		{
			for (int i = 0, count = transform.childCount; i < count; i++) 
			{
				Item item = transform.GetChild (i).GetComponent<Item> ();	

				if (item) 
					items.Add (item);
			}
		}
	}
}
