using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CustomerSetup
{
	public string name;
	public Location initialLocation;
	public Level level;
	public int goalCheckTime;
	public int giveUpTime;
	public ScheduleEntry[] schedule;

	public Customer CreateCustomer()
	{
		return CreateCustomer(new List<Item>(), null);
	}

	public Customer CreateCustomer(List<Item> inventory, Item item)
	{
		if(item != null)
		{
			for (int i = 0, length = schedule.Length; i < length; i++)
			{ 
				schedule[i].item = item.Copy();
			}
		}

		Customer customer = new Customer(initialLocation, new List<Item>(), GameManager.Instance.GetNextCharacterID(), 1, name, schedule, goalCheckTime, giveUpTime, level);
		return customer;
	}
}
