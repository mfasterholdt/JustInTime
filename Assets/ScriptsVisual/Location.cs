using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Location : MonoBehaviour
	{
//		public GameObject trigger;

		public bool isTimeMachine;
		public Location[] connectedLocations;
		public GameObject cover;

//		void Awake () 
//		{
//			if (trigger) 
//			{
//				MouseEvents mouseEvents = trigger.GetComponent<MouseEvents> ();
//				mouseEvents.mouseDown = LocationMouseDown;	
//			}
//		}
//
//		void Update () 
//		{
//			
//		}
//
//		void LocationMouseDown(Transform transform)
//		{
//			GameManager.instance.EnterLocation (this);
//		}
	}
}
