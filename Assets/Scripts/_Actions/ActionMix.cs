using UnityEngine;
using System.Collections;

public class ActionMix : Action
{
	private Item item;
	private Item newItem;

	public ActionMix(int time, int duration, string name, Item currentItem, Item newItem  )
	{
		this.time = time;
		this.duration = duration;
		this.name = name;
		this.item = currentItem.Copy();
		this.newItem = newItem.Copy();
	}

	public override bool Perform (Character player)
	{	
		player.inventory.Remove(item);
		player.inventory.Add(newItem.Copy());

		return true;
	}
}
