using UnityEngine;
using System.Collections;

public class ActionWait : Action
{	
	public ActionWait(int time, int duration, Location location, string name = null)
	{
		if(string.IsNullOrEmpty(name))
			this.name = "Wait "+location.name;		
		else
			this.name = name;		
		
		this.time = time;
		this.duration = duration;
	}
}
