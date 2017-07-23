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
                Transform t = transform.GetChild(i);

                if (t.gameObject.activeInHierarchy)
                {
                    Item item = t.GetComponent<Item>();	

                    if (item && item.gameObject.activeInHierarchy)
                    { 
                        items.Add(item);
                    }
                }
			}
		}

		public override string ToString ()
		{
			return gameObject.name;
		}

		public Item GetContainer()
		{
			Item foundContainer;

			for (int i = 0, count = items.Count; i < count; i++)
			{
				Item item = items[i];

				if (item.isContainer && item.itemsInside.Count < 1)
				{
					return item;
				}
			}

			return null;
		}
	}
}
