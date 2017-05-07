using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class Fire : Item
{	
	public int fireDuration = 0;
	public enum Type{Spark, Fire, Inferno, Embers, Out }
	public Type type = Type.Fire;

	public Fire(int fireDuration, Type type)
	{
		this.pickup = false;
		this.fireDuration = fireDuration;
		this.type = type;
	}
		
	public override Item Copy ()
	{
		return new Fire(fireDuration, type);
	}

	public override Action[] GetLocationActions (int currentTime, int locationIndex, Character player)
	{
		if(type == Type.Spark || type == Type.Fire)
		{
			ActionPutOut<Fire> actionPutOut = new ActionPutOut<Fire>(currentTime, 1, "Put out Fire");
			return new Action[1]{actionPutOut};
		}

		return new Action[0];
	}

	public override bool PassAction (Action action)
	{
		if(action is ActionPutOut<Fire>)
		{
			type = Type.Out;
			fireDuration = 0;
		}

		return true;
	}

	public override bool TickLocation (Location location)
	{
		if(type == Type.Out)
			return true; 
		
		fireDuration++;

		if(fireDuration > 24)
		{
			type = Type.Out;
		}
		else if(fireDuration > 18)
		{
			type = Type.Embers;
		}
		else if(fireDuration > 16)
		{
			type = Type.Fire;
		}
		else if(fireDuration > 8)
		{
			type = Type.Inferno;
		}
		else if(fireDuration > 4)
		{			
			type = Type.Fire;
		}
		else
		{
			type = Type.Spark;
		}

		if(type == Type.Fire || type == Type.Inferno)
		{			
			TeddyBear teddyBear = Utils.FindItemOfType<TeddyBear>(location);
			if(teddyBear != null)
				teddyBear.burned = true;		
		}

		if(type == Type.Inferno)
		{
			for (int i = 0, count = location.characters.Count; i < count; i++) 
			{
				Character character = location.characters[i];
				if(character != null)
				{
					GameManager.paradox = new Paradox(null, character.GetName()+" got cooked in the "+type.ToString()+" of the "+location.name);
					return false;
				}
			}
		}

		return true;
	}

	public override bool Equals (object obj)
	{		
		Fire fire = obj as Fire;

		if(fire == null)
		{
			return false;
		} 
		else 
		{
			return type == fire.type;
		}
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		return type.ToString();
	}
}
