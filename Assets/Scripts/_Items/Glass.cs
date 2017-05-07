using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class Glass : Item
{	
	bool isBroken;

	public Glass(bool isBroken)
	{
		this.isBroken = isBroken;
		this.pickup = false;
	}

	public override Item Copy ()
	{
		return new Glass(isBroken);
	}

	public override string ToString ()
	{
		return "Glass";
	}
}
