using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Customer : Character
{	
	private ScheduleEntry[] schedule;
	private Level level;
	private int goalCheckTime;
	private int loseTime;

	public Customer(Location initialLocation, List<Item> items, int id, int iteration, string name, ScheduleEntry[] schedule, int goalCheckTime, int loseTime, Level level) : base(initialLocation, items, id, iteration, name)
	{
		this.schedule = schedule;
		this.level = level;

		this.goalCheckTime = goalCheckTime;
		this.loseTime = loseTime;
	}
		
	public override void Tick (int currentTime)
	{
		if(currentTime == timeForNextDecision)
		{
			Action action = null;

			if(currentTime >= goalCheckTime)
			{
				action = level.GoalCheck(currentTime, this, currentTime >= loseTime);
			}		

			//No goal related action follow schedule
			if(action == null)
			{
				for (int i = 0, length = schedule.Length; i < length; i++) 
				{
					ScheduleEntry entry = schedule[i];	

					if(entry.time == currentTime)
					{
						action = entry.GetAction(currentTime);
						break;
					}
				}
			}

			//No schedule wait
			if(action == null)
			{
				action = new ActionWait(currentTime, 1, currentLocation);
			}
				
			AddAction(action);			
		}
	}
}