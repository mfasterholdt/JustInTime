//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class ActionSchedule : Action
//{
//	private List<Action> actions;
//
//	public ActionSchedule(int time, string name, Location initialLocation, ScheduleEntry[] schedule)
//	{
//		this.time = time;
//		this.name = name;
//
//		//duration
//		ScheduleEntry lastEntry = schedule[schedule.Length-1];
//		this.duration = lastEntry.time + lastEntry.GetAction(time).duration;
//
//		//Create actions
//		int scheduleTime = 0;
//
//		actions = new List<Action>();
//		Location scheduleLocation = initialLocation;
//		int entryIndex = 0;
//
//		ScheduleEntry entry = schedule[0];
//
//		while(scheduleTime < duration)
//		{
//			if(entry.time == scheduleTime)
//			{
//				Action newAction = entry.GetAction(time + scheduleTime);
//				actions.Add(newAction);
//
////				Debug.Log("added "+newAction.name+" at "+(time + scheduleTime));
//
//				scheduleLocation = entry.location;
//
//				entryIndex++;
//
//				if(entryIndex < schedule.Length)
//					entry = schedule[entryIndex];			
//
//				scheduleTime += newAction.duration;
//			}
//			else
//			{
////				Debug.Log("added wait at "+(time + scheduleTime));
//				Action newAction = new ActionWait(time + scheduleTime, 1, scheduleLocation);
//				actions.Add(newAction);
//
//				scheduleTime++;
//			}
//		}
//	}
//
//	public override bool Prepare (Character player)
//	{	
//		return true;
//	}
//
//	public override bool Perform (Character player)
//	{
//		Debug.Log("perform schedule");
//		return true;
//	}
//}
