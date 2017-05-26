using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Item : MonoBehaviour 
	{
		public bool isMovable;
		public bool isContainer;

		[HideInInspector]
		public List<Item> itemsInside = new List<Item>();

		[HideInInspector]
		public Item itemAround;

		[HideInInspector]
		public Character characterCarrying;

		private int id;

		void Awake()
		{
			for (int i = 0, count = transform.childCount; i < count; i++) 
			{
				Item item = transform.GetChild (i).GetComponent<Item> ();	

				if (item) 
				{
					itemsInside.Add (item);
					item.itemAround = this;
				}
			}
		}

		public bool Compare(Item item)
		{
			if (this.id == item.id) 
			{
				return true;
			}

			return false;
		}

		public void SetId(int id)
		{
			this.id = id;
		}
	}
}
