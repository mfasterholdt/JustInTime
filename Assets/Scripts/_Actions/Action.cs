using UnityEngine;
using System.Collections;

//public delegate bool ActionEvent(Character player, Action action);

public abstract class Action
{
	public string name;
	public int time;
	public int duration = 1;

	//TODO, mff consider having a default constructor for all actions with mandatory paramters, start with AcitonWait
//	protected Action(string name, int time, int duration = 1)
//	{
//		this.name = name;
//		this.time = time;
//		this.duration = duration;
//	}

//	public virtual bool CheckPrepare(Character player)
//	{
//		return true;
//	}

	public virtual bool ParadoxCheck(int currentTime)
	{
		return true;	
	}

	public virtual bool Prepare(Character character)
	{
		return true;
	}

	public virtual bool VerifyPrepare(Character character)
	{
		return true;
	}

//	public virtual bool CheckPerform(Character player)
//	{
//		return true;
//	}

	public virtual bool Perform(Character character)
	{
		return true;
	}

	public virtual bool VerifyPerform(Character character)
	{
		return true;
	}

	public override string ToString ()
	{
		return name;
	}
		
}
