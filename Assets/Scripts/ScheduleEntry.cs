using UnityEngine;
using System.Collections;
using UnityEditor;

[System.Serializable]
public struct ScheduleEntry
{
	public int time;

	public enum Type{ Move, Pickup, Drop }
	public Type type;

	public Location location;

	public Item item;

	//TODO, mff schedules does currently not support actions with duration > 1
	public Action GetAction(int currentTime)
	{
		if(type == Type.Move)
		{
			return new ActionEnter(currentTime, location);
		}
		else if(type == Type.Pickup)
		{
			return new ActionPickup(currentTime, item, 0);
		}
		else if(type == Type.Drop)
		{
			return new ActionDrop(currentTime, item, 0);
		}

		return new ActionWait(currentTime, 1, location);
	}
}
