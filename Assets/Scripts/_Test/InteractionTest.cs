using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractionTest : MonoBehaviour 
{
	public Transform player;
	public MouseEvents[] items;
	public MouseEvents[] locations;
	public MouseEvents[] tables;
	public MouseEvents oven;
	public MouseEvents ovenInside;
	public Transform ovenDoor;

	public List<Transform> inventory;
	//public Transform currentItem;
	public Transform currentLocation;

	void Start () 
	{		
		for (int i = 0, length = items.Length; i < length; i++) 
		{
			items[i].mouseDown += OnMouseDownItem;	
		}

		if(oven)
			oven.mouseDown += OnMouseDownOven;

		if(ovenInside)
			ovenInside.mouseDown += OnMouseDownOvenInside;
		
		for (int i = 0, length = locations.Length; i < length; i++) 
		{
			locations[i].mouseDown += OnMouseDownLocation;	
		}

		for (int i = 0, length = tables.Length; i < length; i++) 
		{
			tables[i].mouseDown += OnMouseDownTable;	
		}
	}

	void OnMouseDownTable(Transform table)
	{
		if(inventory.Count > 0)
		{
			Transform currentItem = inventory[inventory.Count-1];
			currentItem.parent = null;
			currentItem.position = table.position + Vector3.up * 0.75f;

			inventory.Remove(currentItem);
		}
	}

	void OnMouseDownOven(Transform oven)
	{
		Vector3 rot = ovenDoor.localRotation.eulerAngles;

		if(rot.x < 90)
			ovenDoor.Rotate(-110, 0, 0);
		else
			ovenDoor.Rotate(110, 0, 0);
	}

	void OnMouseDownOvenInside(Transform ovenInside)
	{
		if(inventory.Count > 0)
		{
			Transform currentItem = inventory[inventory.Count-1];
			currentItem.parent = null;
			currentItem.position = ovenInside.position + Vector3.back * 0.3f + Vector3.down * 0.1f;

			inventory.Remove(currentItem);
		}
	}

	void OnMouseDownItem(Transform item)
	{
		//if(currentItem == null)
		{
			item.parent = player;

			Vector3 itemPos = Vector3.zero;
			itemPos.z = -0.7f;
			itemPos.y = inventory.Count * 0.5f;

			item.localPosition = itemPos;

			inventory.Add(item);
		}
	}

	void OnMouseDownLocation(Transform location)
	{
		Vector3 pos = player.position;
		pos.x = location.position.x;
		player.position = pos;
		currentLocation = location;
	}
}
