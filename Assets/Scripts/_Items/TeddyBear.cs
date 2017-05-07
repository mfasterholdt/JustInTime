using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class TeddyBear : Item
{	
	public bool burned = false;

	public TeddyBear(bool burned)
	{
		this.burned = burned;
	}

	public override Item Copy ()
	{
		return new TeddyBear(burned);
	}
		
	public override bool Equals (object obj)
	{		
		TeddyBear teddyBear = obj as TeddyBear;

		if(teddyBear == null)
		{
			return false;
		} 
		else 
		{
			return this.burned == teddyBear.burned;		
		}
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override string ToString ()
	{
		if(burned)
			return "Burned Teddy Bear";
		else
			return "Teddy Bear";
	}

	public override string ToShortString ()
	{
		return ToString();
	}
}
