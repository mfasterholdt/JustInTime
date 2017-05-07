//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//[System.Serializable]
//public class Driver : Character
//{	
//	private ScheduleEntry[] schedule;
//	private int deliveryTime;
//
//	public Driver(Location initialLocation, List<Item> items, int id, int iteration, string name, int deliveryTime, ScheduleEntry[] schedule) : base(initialLocation, items, id, iteration, name)
//	{	
//		this.schedule = schedule;
//		this.deliveryTime = deliveryTime;
//	}
//
//	public override void Tick (int currentTime)
//	{
//		if(currentTime == timeForNextDecision)
//		{
//			if(currentTime == deliveryTime)
//			{
//				if(CakeCheck())
//				{
//					GameManager.levelCompleteText = GetName() +" eats the cake with much glee :D";
//					GameManager.levelComplete = true;
//				}
//				else
//				{
//					GameManager.paradox = new Paradox(null, GetName() +" was not satisfied with the content of the box and is inconsolable");
//					GameManager.Instance.paradoxFound = true;
//				}
//			}
//
//			Action action = new ActionWait(currentTime, 1, currentLocation);
//
//			for (int i = 0, length = schedule.Length; i < length; i++) 
//			{
//				ScheduleEntry entry = schedule[i];	
//
//				if(entry.time == currentTime)
//				{
//					action = entry.GetAction(currentTime);					
//				}
//			}
//
//			AddAction(action);			
//		}
//	}
//
//	public bool CakeCheck()
//	{
//		if(inventory.Count > 0)
//		{
//			Box box = inventory[0] as Box;
//			if(box != null)
//			{
//				Item[] items = box.GetItems();
//				if(items.Length > 0)
//				{
//					Cake cake = items[0] as Cake;
//
//					if(cake != null && cake.state == Cake.CakeState.Finished)
//						return true;
//				}
//			}
//		}
//		return false;
//	}
//}