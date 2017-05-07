using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cake : Item
{
	public int bakeAmount;
	public int bakeAmountRequried = 23;

	public enum CakeState { Unbaked, Finished, Burned};
	public CakeState state = CakeState.Unbaked;

	public Cake()
	{		
	}

	public Cake(int bakeAmount, int bakeAmountRequried, CakeState state)
	{
		this.bakeAmount = bakeAmount;
		this.bakeAmountRequried = bakeAmountRequried;
		this.state = state;
	}

	public override Item Copy ()
	{
		return new Cake(bakeAmount, bakeAmountRequried, state);
	}

	public override bool CheckType (Item item)
	{
		Cake cake = item as Cake;

		if(cake == null)
			return false;

		return GetType() == item.GetType() && state == cake.state;
	}

	public override bool Equals (object obj)
	{
		Cake item = obj as Cake;
		if(item == null)
			return false;
		return bakeAmount == item.bakeAmount && state == item.state;		
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override bool UseItem (Item otherItem)
	{
		if(otherItem is Oven)
		{
			//Baking
			bakeAmount++;

			if(bakeAmount > 40)
			{
				state = CakeState.Burned;
			}
			else if(bakeAmount >= bakeAmountRequried)
			{
				state = CakeState.Finished;
			}
			else
			{
				state = CakeState.Unbaked;
			}
		}
			
		return true;
	} 

	public override string ToString ()
	{
		string cakeText = "";

		if(state == CakeState.Unbaked)
		{
			cakeText += "Unbaked Cake";
		}
		else if(state == CakeState.Finished)
		{
			cakeText += "Finished Cake";
		}
		else if(state == CakeState.Burned)
		{
			cakeText += "Burned Cake";
		}

		cakeText += " "+bakeAmount+"/"+bakeAmountRequried;
		
		return cakeText;
	}

	public override string ToShortString ()
	{
		return ToString();
	}
}
