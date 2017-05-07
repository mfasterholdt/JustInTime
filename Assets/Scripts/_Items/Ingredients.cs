using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class Ingredients : Item
{	
	public int mixDuration;
	public Cake cake;

	public Ingredients()
	{		
	}

	public Ingredients(int mixDuration, Cake cake)
	{
		this.mixDuration = mixDuration;
		this.cake = cake.Copy() as Cake;
	}

	public override Item Copy ()
	{
		return new Ingredients(mixDuration, cake);
	}

	public override Action[] GetInventoryActions (int currentTime, int locationIndex, Character player)
	{
		ActionMix actionMix = new ActionMix(currentTime, mixDuration, "Mix Ingredients", this, cake);
		return new Action[]{ actionMix };
	}

	public override string ToString ()
	{
		return "Ingredients";
	}
}
