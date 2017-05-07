using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChestLevel : Level
{
	[Space(10)]
	[Header("---- Items ----")]
	public bool boxLocked;
	public bool ovenOn;

	public bool keyInBox;
	public bool keyInInventory;

	public bool letterInBox;
	public bool letterInInventory;

	public Location initialKeyLocation;
	public Location initialChestLocation;
	public Location initialFurnaceLocation;

	public override void Setup () 
	{			
		base.Setup();

		if(initialKeyLocation)
			initialKeyLocation.initialItems.Add(new Key());
		
		Item[] boxContent = new Item[10];

		if(letterInBox)
			boxContent[0] = new Paper("Letter");

		if(keyInBox)
			boxContent[1] = new Key();
				
		initialChestLocation.initialItems.Add(new Box(true, boxContent, boxLocked));

		if(initialFurnaceLocation)
			initialFurnaceLocation.initialItems.Add(new Oven(false, ovenOn, true, new Item[1]));
	}

	public override List<Item> GetInitialInventory ()
	{
		List<Item> initialInventory = new List<Item>();

		if(letterInInventory)
			initialInventory.Add(new Paper("Letter"));

		if(keyInInventory)
			initialInventory.Add(new Key());

		return initialInventory;
	}

	public override void Load()
	{
		
	}
}
