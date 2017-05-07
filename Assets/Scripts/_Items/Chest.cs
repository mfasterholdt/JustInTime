using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class Chest : Item
{	
	public override Item Copy ()
	{
		return new Chest();
	}

	public override string ToString ()
	{
		return "Chest";
	}
}
