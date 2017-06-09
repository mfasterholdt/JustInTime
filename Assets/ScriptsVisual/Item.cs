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

		[HideInInspector]
		public Item initialItemAround;

		[HideInInspector]
		public Character initialCharacterCarrying;

		private int id;

		void Awake()
		{
			for (int i = 0, count = transform.childCount; i < count; i++) 
			{
				Item item = transform.GetChild (i).GetComponent<Item> ();	

				if (item) 
				{
					Debug.Log (this + " with " + item +" inside");
					itemsInside.Add (item);
					item.itemAround = this;
					item.initialItemAround = this;
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

		public void UpdateItem()
		{
			if (itemAround) 
			{
				transform.position = itemAround.transform.position;
			}
//			else if(characterCarrying)
//			{
//				transform.position = characterCarrying.carryPivot.position;
//			}
		}

		public void Reset()
		{
			if (initialItemAround) 
			{
				initialItemAround.itemsInside.Add (this);
				transform.position = initialItemAround.transform.position;

				itemAround = initialItemAround;
			}
		}

		public override string ToString ()
		{
			return string.Format (gameObject.name);
		}
	}
}
