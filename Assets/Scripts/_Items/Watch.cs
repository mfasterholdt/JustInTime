using UnityEngine;
using System.Collections;

public class Watch : Item 
{
	public Watch(int age = 0)
	{
		this.age = age;
		this.useTimeline = true;
	}

	public override Item Copy ()
	{
		return new Watch(age);
	}

//	public override bool Equals (object obj)
//	{
//		Watch item = obj as Watch;
//		if(item == null)
//		{
//			return false;
//		} 
//		else 
//		{
//			return age == item.age;
//		}
//	}

	public override string ToString ()
	{
		return "Watch "+age;
	}

	public override string ToShortString ()
	{
		return ToString();
	}
}
