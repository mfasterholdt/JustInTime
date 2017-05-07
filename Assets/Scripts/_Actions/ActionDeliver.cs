using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionDeliver : Action
{
	private Item item;

	public ActionDeliver(int currentTime, Item item)
	{
		this.time = currentTime;
		this.name = "Deliver "+item.GetType();
		this.item = item;
	}

	public override bool Perform (Character player)
	{
		int ticks = GameManager.Instance.GetMaximumTime();
		string levelCompleteText = item.GetType() +" delivered in "+ ticks +" ticks";
		GameManager.levelCompleteText = levelCompleteText;

		GameManager.levelComplete = true;

		return true;
	}
}
