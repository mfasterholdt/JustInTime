//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//[System.Serializable]
//public class Customer : Character
//{	
//	public Customer(Location initialLocation, List<Item> items, int id, int iteration, string name) : base(initialLocation, items, id, iteration, name)
//	{		
//	}
//
//	public override void Tick (int currentTime)
//	{
//		if(currentTime == timeForNextDecision)
//		{
//
//			//if(currentTime > 3)
////			{
////				ActionMix actionMix = new ActionMix(currentTime, 8, "Mix Ingredients", inventory[0], new Cake());
////				AddAction(actionMix);
////			}
////			else
////			{
////				ActionWait actionWait = new ActionWait(currentTime, 1, currentLocation);
////				AddAction(actionWait);
////			}
//
//			ActionWait actionWait = new ActionWait(currentTime, 1, currentLocation, "Waiting for cake in "+currentLocation.name);
//			AddAction(actionWait);
//		}
//	}
//
//}