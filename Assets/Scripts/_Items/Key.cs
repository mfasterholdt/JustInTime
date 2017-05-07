using UnityEngine;
using System.Collections;

using System.Collections.Generic;

[System.Serializable]
public class Key : Item
{	
	public override Item Copy ()
	{
		return new Key();
	}

	public override string ToString ()
	{
		return "Key";
	}
}
