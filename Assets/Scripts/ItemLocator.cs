using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemLocator
{
	public List<Item> itemInstances = new List<Item>();

	private float fieldWidth = 230;
	private float fieldHeight = 22;

	public void PassItem(List<Item> inventory)
	{
		for (int i = 0, count = inventory.Count; i < count; i++) 
		{
			itemInstances.Add(inventory[i]);	
		}
	}

	public void Draw(Vector2 posGUI)
	{
		//timeline.Sort();

		for (int i = 0, count = itemInstances.Count; i < count; i++) 
		{
			//			string watch = timeline[i].ToString();

			GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "age "+i+", time"+itemInstances[i]);
			posGUI.y += fieldHeight;
		}

		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Entries : "+itemInstances.Count);
		posGUI.y += fieldHeight;
	}
}
