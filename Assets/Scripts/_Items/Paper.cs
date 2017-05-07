using UnityEngine;
using System.Collections;

public class Paper : Item
{
	string name; 

	public Paper(string name)
	{
		this.name = name;
	}

	public override Item Copy ()
	{
		return new Paper(name);
	}

	public override bool UseItem (Item otherItem)
	{
		if(otherItem is Oven)
		{
			Oven oven = otherItem as Oven;
			oven.RemoveItem(this);	
		}

		return true;
	}
	public override string ToString ()
	{
		return name;
	}
}
