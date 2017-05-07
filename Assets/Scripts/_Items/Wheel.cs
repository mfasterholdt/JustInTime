using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class Wheel : Item
{	
	public override Item Copy ()
	{
		return new Wheel();
	}

	public override string ToString ()
	{
		return "Wheel";
	}
}
