using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Item : MonoBehaviour 
	{
		public bool isMovable;
		public bool isContainer;
        public bool showContent = true;
		public float height = 0.8f;

		//[HideInInspector]
		public List<Item> itemsInside = new List<Item>();

		//[HideInInspector]
		public Item itemAround;

		[HideInInspector]
		public Character characterCarrying;

		public List<Item> initialItemsInside = new List<Item> ();

		[HideInInspector]
		public Character initialCharacterCarrying;

		public int id;

		void Awake()
		{
			for (int i = 0, count = transform.childCount; i < count; i++) 
			{
				Item item = transform.GetChild (i).GetComponent<Item> ();	

                if (item && item.gameObject.activeInHierarchy) 
				{
					itemsInside.Add (item);
					item.itemAround = this;
				}
			}

			initialItemsInside = new List<Item>(itemsInside);
		}

        public Item Copy()
        {
            GameObject obj = Instantiate(gameObject);

            Item newItem = obj.GetComponent<Item>();
            newItem.id = id;

            return newItem;
        }

        public ItemProfile GetProfile(bool includeContent = true)
        {
            ItemProfile profile =  new ItemProfile();

            profile.id = id;

            if (showContent && includeContent)
            {
                profile.showContent = true;

                ItemProfile[] itemsInsideProfiles = new ItemProfile[itemsInside.Count];

                for (int i = 0, count = itemsInside.Count; i < count; i++)
                {
                    itemsInsideProfiles[i] = itemsInside[i].GetProfile(includeContent);   
                }

                profile.itemsInsideProfiles = itemsInsideProfiles;
            }
            else
            {
                profile.showContent = false;
            }

            profile.position = transform.position;

            return profile;
        }

//		public bool Compare(Item item)
//		{
//			if (this.id == item.id) 
//			{
//				return true;
//			}
//
//			return false;
//		}

//		public void SetId(int newId)
//		{
//			id = newId;
//		}
//
//		public int GetId()
//		{
//			return id;
//		}

		public void UpdateItem()
		{
			Vector3 pos = transform.position;

			for (int i = 0, count = itemsInside.Count; i < count; i++)
			{
				Item item = itemsInside[i];
				item.transform.position = pos;
				pos += Vector3.up * item.height;
			}
			
//			if (itemAround) 
//			{
//				transform.position = itemAround.transform.position;
//			}
//			else if(characterCarrying)
//			{
//				transform.position = characterCarrying.carryPivot.position;
//			}
		}

		public void Reset()
		{
			for (int i = 0, count = initialItemsInside.Count; i < count; i++) 
			{
				Item item = initialItemsInside [i];	
				item.itemAround = this;
			}

			itemsInside = new List<Item>(initialItemsInside);
		}

        public Vector3 GetDropPosition()
        {
            if (itemsInside.Count > 0)
            {
                Item item = itemsInside[itemsInside.Count - 1];
                return item.transform.position + Vector3.up * item.height;
            }
            else
            {
                return transform.position;
            }
        }

		public override string ToString ()
		{
			return string.Format (gameObject.name);
		}
	}
}
