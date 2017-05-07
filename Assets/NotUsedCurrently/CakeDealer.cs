//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class CakeDealer : Character
//{
//	private ScheduleEntry[] schedule;
//
//	public CakeDealer(Location initialLocation, List<Item> items, int id, int iteration, string name, ScheduleEntry[] schedule) : base(initialLocation, items, id, iteration, name)
//	{	
//		this.schedule = schedule;
//	}
//
//	public override void Tick (int currentTime)
//	{
//		if(currentTime == timeForNextDecision)
//		{
//			Action action = new ActionWait(currentTime, 1, currentLocation);
//
//			Cake cake = FindCake();
//
//			if(cake != null)
//			{
//				GameManager.levelCompleteText = GetName() +" is pleased with the delivery :D";
//				GameManager.levelComplete = true;
//
//				action = new ActionPickup(currentTime, cake);
//			}
//			else
//			{
//				for (int i = 0, length = schedule.Length; i < length; i++) 
//				{
//					ScheduleEntry entry = schedule[i];	
//
//					if(entry.time == currentTime)
//					{
//						if(entry.type == ScheduleEntry.Type.Pickup)
//						{
//							GameManager.paradox = new Paradox(null, GetName() +" gave up waiting for his delivery and is inconsolable");
//							GameManager.Instance.paradoxFound = true;
//						}
//						else
//						{
//							action = entry.GetAction(currentTime);
//						}
//					}
//				}
//			}
//
//			AddAction(action);			
//		}
//	}
//
//	public Cake FindCake()
//	{
//		for (int i = 0, count = currentLocation.items.Count; i < count; i++) 
//		{
//			Cake cake = currentLocation.items[i] as Cake;
//			if(cake != null)
//				return cake;
//		}
//
//		return null;
//
////		currentLocation.items.Find(x => x.Equals(Cake)
////		return inventory.Count > 0 && inventory[0] is Cake;
//	}
//}
