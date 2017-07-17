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

        [HideInInspector]
		public Location[] timeMachineConnections;

        [HideInInspector]
        public List<Item> initialInventory;

        //***TODO, post process
		void Awake () 
		{
			//Locations
			locations = GetComponentsInChildren<Location> ();

			//TimeMachine connections
			List<Location> connections = new List<Location> ();

			for (int i = 0, length = locations.Length; i < length; i++) 
			{
				Location location = locations [i];

				for (int j = 0, connectedLength = location.connectedLocations.Length; j < connectedLength; j++) 
				{
					Location connectedLocation = location.connectedLocations [j];

					if (connectedLocation.isTimeMachine) 
					{
						connections.Add(location);
					}
				}
			}

			timeMachineConnections = connections.ToArray ();

			//Items
			Item[] items = GetComponentsInChildren<Item> ();

			for (int i = 0, length = items.Length; i < length; i++) 
			{
                items[i].id = i;
			}

            //Initial Inventory
            for (int i = 0, length = transform.childCount; i < length; i++)
            {
                Transform t = transform.GetChild(i);
                Item item = t.GetComponent<Item>();

                if (item)
                    initialInventory.Add(item);
            }
		}
	}
}
