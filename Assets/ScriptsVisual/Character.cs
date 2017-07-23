using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Character : MonoBehaviour
	{
		public GameObject visualsDefault;
		public GameObject visualsMeet;
		public GameObject visualsWait;
        public GameObject visualsWarning;

		public MeshRenderer[] colorRenderers;

		public Transform pickupPivot;

        public List<Item> inventory;
        public List<Item> initialInventory;
            
		//[HideInInspector]
		public Location initialLocation, currentLocation, previousLocation;

		public List<Action> history = new List<Action>();

		public List<Observation> observations = new List<Observation>();

		[HideInInspector]
		public Transform timeLineTrack;

		[HideInInspector]
		public CharacterTimeline timeLine;

		[HideInInspector]
		public Material primaryMaterial;

		private Observation currentObservation;
        private GameObject currentVisuals;

		[HideInInspector]
		public List<Paradox> currentParadoxes = new List<Paradox>();

        public void Setup(Location location, List<Item> inventory, Material material, Transform timeLineTrack)
		{
			initialLocation = location;
			//currentLocation = location;

			for (int i = 0, length = colorRenderers.Length; i < length; i++)
			{
				MeshRenderer renderer = colorRenderers[i];
				renderer.material = material;
			}

			this.timeLineTrack = timeLineTrack;

			primaryMaterial = material;

			CharacterTimeline timeLine = timeLineTrack.GetComponent<CharacterTimeline>();
			timeLine.Setup(material);
			this.timeLine = timeLine;

            initialInventory = new List<Item>(inventory);
            //this.inventory = new List<Item>(inventory);

//            ParticleSystem[] warningParticles = visualsWarning.GetComponentsInChildren<ParticleSystem>();
//            for (int i = 0, length = warningParticles.Length; i < length; i++)
//            {
//                warningParticles[i].startColor = material.color;
//            }
                
            currentVisuals = visualsDefault;

            Reset();
		}

		public void Reset()
		{
			currentLocation = initialLocation;
            previousLocation = initialLocation;

            inventory = new List<Item>(initialInventory);

            for (int i = 0, count = inventory.Count; i < count; i++)
            {
                Item item = inventory[i];
                item.characterCarrying = this;
            }
                
            currentParadoxes.Clear();
		}

		public void Destroy()
		{
			Destroy(timeLineTrack.gameObject);	
			Destroy(this.gameObject);
		}

//		public void MoveLocation(Location location)
//		{
//			previousLocation = currentLocation;
//			currentLocation = location;
//		}

		public int GetNextActionTime()
		{
			if (history.Count > 0)
			{
//				if (initialLocation.isTimeMachine) 
//				{
//					return GetEnterTime ();
//				}
//				else 
//				{
				Action action = history[history.Count - 1];
				return action.time + action.duration;
//				}
			}

			return 0;
		}

		//		public int GetEnterTime()
		//		{
		//			if (initialLocation.isTimeMachine)
		//			{
		//				for (int i = 0, count = history.Count; i < count; i++)
		//				{
		//					ActionEnter actionEnter = history [i] as ActionEnter;
		//
		//					if (actionEnter != null)
		//					{
		//						return actionEnter.time;
		//					}
		//				}
		//			}
		//
		//			return 0;
		//		}

		public Action GetLastAction()
		{
			if (history.Count > 0)
			{
				return history[history.Count - 1];
			}

			return null;
		}

		public int GetLastTime()
		{
			if (history.Count > 0)
			{
				Action action = history[history.Count - 1];
				return action.time + action.duration;
			}

			return 0;
		}

		public Location GetLocationAtTime(int time)
		{
			Location result = initialLocation;

			if (history.Count > 0)
			{
				for (int i = 0, count = history.Count; i < count; i++)
				{					
					ActionEnter actionEnter = history[i] as ActionEnter;

					if (actionEnter != null && time > actionEnter.time)
					{
						result = actionEnter.toLocation;
					}
				}
			}

			return result;
		}

		public float GetTrackStart(float currentTimeInterpolated)
		{
			if (initialLocation.isTimeMachine)
			{					
				for (int i = 0, count = history.Count; i < count; i++)
				{
					ActionEnter actionEnter = history[i] as ActionEnter;

					if (actionEnter != null)
					{							
						return actionEnter.time;
					}
				}
			}
			else if (history.Count > 0)
			{
				return history[0].time;
			}

			return 0;
		}

		public float GetTrackEnd(int currentTime, float currentTimeInterpolated, int timeForNextDecision, bool currentPlayer, bool draggingPlayhead)
		{
			float result = 0f;
//			bool foundAction = false;

			if (history.Count > 0)
			{
				for (int i = history.Count - 1; i >= 0; i--)
				{
					Action action = history[i];

					Location location = GetLocationAtTime(action.time + action.duration);
					ActionEnter actionEnter = action as ActionEnter;

					if (location.isTimeMachine == false || (actionEnter != null && actionEnter.toLocation.isTimeMachine))
					{
						result = action.time + action.duration;
//						foundAction = true;
						break;
					}
				}
			}

			return result;

//			if (currentPlayer)
//			{
//				if(draggingPlayhead) 
//				{
//					if(currentTimeInterpolated > result)
//						result = currentTimeInterpolated;
//				}
//				else
//				{	
//					if (timeForNextDecision > currentTimeInterpolated) 
//					{	
//						if (currentTime < timeForNextDecision) 
//						{
////							if (currentTimeInterpolated < currentTime) 
////							{
////								result = timeForNextDecision;		
////							}
//							//if(currentTime - 1 
//						}
//						else 
//						{
//							result = currentTimeInterpolated;
//						}
//
////						if (foundAction)
////						{
////							
////							Debug.Log (foundAction);
////						}
////						else 
////						{
////							result = currentTimeInterpolated;
////						}
////						if (currentTime < timeForNextDecision) 
////						{
////							result = currentTimeInterpolated;
////						}
////						else
////						{
////							result = timeForNextDecision;
////
////						}
////						else 
////						{
////							result = currentTimeInterpolated;
////						}
//					}
//					else if (currentTimeInterpolated > currentTime) 
//					{
//						result = currentTimeInterpolated;
//					}
//				}
//			}
//
//			return result;
		}

		public float GetTrackEndOld(int currentTime, float currentTimeInterpolated, bool currentPlayer, bool draggingPlayhead)
		{
			float result = 0;

			if (currentPlayer)
			{
				if (draggingPlayhead)
				{
					//trackEnd = trackStart;

					if (history.Count > 0)
					{
						bool foundAction = false;

						for (int i = history.Count - 1; i >= 0; i--)
						{
							Action action = history[i];

							Location location = GetLocationAtTime(action.time);

							if (location.isTimeMachine == false)
							{
								result = action.time + action.duration;
								foundAction = true;
								break;
							}
						}

						if (initialLocation.isTimeMachine && foundAction == false)
						{
							result = 0;
						}
						else
						{
							if (currentTimeInterpolated > result)
								result = currentTimeInterpolated;
						}
					}
					else
					{
						if (initialLocation.isTimeMachine == false)
						{
							result = currentTimeInterpolated;
						}
						else
						{
							bool foundAction = false;

							for (int i = history.Count - 1; i >= 0; i--)
							{
								Action action = history[i];

								Location location = GetLocationAtTime(action.time);

								if (location.isTimeMachine == false)
								{
									result = action.time + action.duration;
									foundAction = true;
									break;
								}
							}

							if (foundAction == false)
								result = 0;
						}

					}
				}
				else
				{
					bool foundAction = false;

					for (int i = history.Count - 1; i >= 0; i--)
					{
						Action action = history[i];

						Location location = GetLocationAtTime(action.time);

						if (location.isTimeMachine == false)
						{
							foundAction = true;
							break;
						}
					}

					if (foundAction == false)
					{
						result = 0;
					}
					else
					{
						if (currentTimeInterpolated > result)
							result = currentTimeInterpolated;
					}
				}
			}
			else
			{
				if (history.Count > 0)
				{
					for (int i = history.Count - 1; i >= 0; i--)
					{
						Action action = history[i];

						Location location = GetLocationAtTime(action.time);
						if (location.isTimeMachine == false)
						{
							result = action.time + action.duration + 1;
							break;
						}
					}
				}
			}

			return result;
		
        }

		public void SetVisuals(GameObject nextVisuals)
		{
            if (nextVisuals != currentVisuals)
            {
                currentVisuals.SetActive(false);
                nextVisuals.SetActive(true);

                currentVisuals = nextVisuals;
            }
		}

		public void UpdateCharacter(float time)
		{
            float trackStart = GetTrackStart(time);
            float trackEnd = GetTrackEnd(0, 0f, 0, false, false);

//            if (currentParadoxes.Count > 0)
//            {
//                Paradox paradox = currentParadoxes[0];
//
//                //GameObject paradoxVisuals = paradox.visuals;
//                //if (paradoxVisuals)                    
//                //   SetVisuals(paradoxVisuals);                 
//                //return;
//
//                if (time >= paradox.time)
//                {
//                    return;
//                }
//            }
            
			//Current action
			Action action = null;
			for (int i = 0, count = history.Count; i < count; i++)
			{
				Action a = history[i];
                //TODO, used to foreshadow move direction, maybe handle paradox visualsation differently
                if (time > a.time || (time == a.time && currentParadoxes.Count < 1))
                {
					action = a;
                }                
			}
				
			//Interpolate state
			Vector3 pos = currentLocation.transform.position;
            Vector3 lookDirection = Vector3.back;

			GameObject nextVisuals = visualsDefault;

			if (action != null)
			{
				float timeFraction = time - action.time;
				//TODO, mf probably needs more generalised solution here

				if (action is ActionEnter)
				{
					ActionEnter actionEnter = action as ActionEnter;	
					
					Vector3 fromPos = actionEnter.fromLocation.transform.position;
					Vector3 toPos = actionEnter.toLocation.transform.position;

					lookDirection = (toPos - fromPos).normalized;

					pos = Vector3.Lerp(fromPos, toPos, timeFraction);
				}
				else if (action is ActionWait)
				{
					ActionWait actionWait = action as ActionWait;

					if (actionWait != null && inventory.Count < 1)
					{
						nextVisuals = visualsWait;
					}
				}
			}

            if (time > trackEnd)
            {
                nextVisuals = visualsWait; //visualsWarning
                lookDirection = Vector3.back;
            }

			transform.position = pos;			
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), 15f * Time.deltaTime);

			Vector3 carryPos = pickupPivot.position;

			for (int i = 0, count = inventory.Count; i < count; i++)
			{				
				Item item = inventory[i];
				item.transform.position = carryPos;
                carryPos += Vector3.up * item.height;
			}

			if (action != null)
			{
				float timeFraction = time - action.time;

				if (action is ActionPickup)
				{
                    if (timeFraction > 0f)
                    {
                        ActionPickup actionPickup = action as ActionPickup;
//					Vector3 itemTargetPos = carryPos - Vector3.up * actionPickup.item.height;
                        if(inventory.Count > 0)
                        {
                            Item itemPickup = inventory[inventory.Count - 1];
                        //Debug.Log(item == actionPickup.currentItem);

                        //if (actionPickup.currentItem)
                        //{
                            Vector3 towardsPos = pickupPivot.position + Vector3.up * actionPickup.pickupOffset;
//                        Debug.Log(inventory.Count);
//                        Item item = inventory[inventory.Count - 1];
//                        item.transform.position = Vector3.Lerp(actionPickup.fromPos, towardsPos, timeFraction);
                            itemPickup.transform.position = Vector3.Lerp(actionPickup.fromPos, towardsPos, timeFraction);
//                        }
//                        else
//                        {
//                            Debug.LogWarning("this should probably be prevented", gameObject);
//                        }
                        }
                    }
				}
				else if (action is ActionDrop)
				{
                    if (timeFraction > 0f)
                    {
                        ActionDrop actionDrop = action as ActionDrop;

                        if (actionDrop.currentItem)
                        {
                            Vector3 fromPos = pickupPivot.transform.position + Vector3.up * actionDrop.pickupOffset;
                            actionDrop.currentItem.transform.position = Vector3.Lerp(fromPos, actionDrop.towardsPos, timeFraction);
                        }
                    }
				}
			}
                
            if (currentParadoxes.Count > 0)
			{
				GameObject paradoxVisuals = currentParadoxes[0].visuals;

				if (paradoxVisuals)
					nextVisuals = paradoxVisuals;
			}
                
            //Time line
            Vector3 localScale = timeLine.bar.localScale;
            localScale.x = trackEnd - trackStart;
            timeLine.bar.localScale = localScale;

            Vector3 localPos = timeLine.bar.localPosition;
            localPos.x = trackStart;
            timeLine.bar.localPosition = localPos;

            SetVisuals(nextVisuals);                    
		}

		public float GetPickupOffset()
		{
			if (inventory.Count > 0)
			{
				Item item = inventory[inventory.Count - 1];
				return (item.transform.position.y - pickupPivot.transform.position.y) + item.height;
			}

			return 0;
		}

//		public void UpdateCharacterOld(int currentTime, float currentTimeInterpolated, int timeForNextDecision, bool currentPlayer, bool draggingPlayhead)
//		{
//			Vector3 forward = Vector3.back;
//
//			//Movement
//			Action a;
//			float timeFraction;
//
//			if (currentTimeInterpolated > currentTime)
//			{
//				//Backwards in time
//
//				timeFraction = 1 - (currentTimeInterpolated - currentTime);
//				a = GetAction(currentTime);
//			}
//			else
//			{
//				//Forward in time
//				if (currentTime > 0)
//				{
//					a = GetAction(currentTime - 1);
//					timeFraction = currentTime - currentTimeInterpolated;
//				}
//			}
//
//			if (this == GameManager.instance.currentPlayer)
//			{
//				//Debug.Log (a + ","+history.Count+"."+this.gameObject.name);
//
//				if (history.Count > 0)
//				{
//					Debug.Log(history[0].time + "," + currentTime);
//					//Debug.Log(GameManager.instance.currentPlayer);
//				}
//			}
//
//			ActionEnter ae = a as ActionEnter;
//
//			if (ae != null)
//			{
////				Debug.Log (ae);
//
//				Vector3 fromPos = ae.fromLocation.transform.position;
//				Vector3 toPos = ae.toLocation.transform.position;
//
//				transform.position = Vector3.Lerp(fromPos, toPos, timeFraction);
//
//				if (currentTime != currentTimeInterpolated)
//				{				
//					forward = (fromPos - toPos).normalized;
//				}
//			}
//			else
//			{
//				if (currentTimeInterpolated == 0)
//				{
//					transform.position = initialLocation.transform.position;
//				}
//			}
//
//
//
//			Vector3 pos = initialLocation.transform.position;
//
//
//			if (history.Count > 0 && false)
//			{
//				//Movement
//				Location fromLocation = initialLocation;
//
//				for (int i = 0, count = history.Count; i < count; i++)
//				{
//					Action action = history[i];
//					ActionEnter actionEnter = history[i] as ActionEnter;
//
//					if (currentTimeInterpolated >= action.time + 1)
//					{	
//						if (actionEnter != null)
//						{
//							fromLocation = actionEnter.fromLocation;
//							pos = fromLocation.transform.position;
//						}
//					}
//					else
//					{						
//						if (actionEnter != null)
//						{
//							float diff = currentTimeInterpolated - actionEnter.time;
//							Vector3 fromPos = fromLocation.transform.position;
//							Vector3 toPos = actionEnter.fromLocation.transform.position;
//							pos = Vector3.Lerp(fromPos, toPos, diff);
//
//							if (currentTime != currentTimeInterpolated)
//							{				
//								forward = (toPos - fromPos).normalized;
//							}
//						}
//
//						break;
//					}
//				}
//
//				//Rotation
////				Debug.Log(Mathf.Abs(currentTime-currentTimeInterpolated));
////				if (currentPlayer == false && currentTime==currentTimeInterpolated) 
//				if (Mathf.Abs(currentTime - currentTimeInterpolated) < 0.05f)
//				{
//					Location location = GetLocationAtTime(currentTime);
//					Location nextLocation = GetLocationAtTime(currentTime + 1);
//
//					if (location != nextLocation)
//					{
//						Vector3 fromPos = location.transform.position;
//						Vector3 toPos = nextLocation.transform.position;
//						forward = (toPos - fromPos).normalized;
//					}
//					else
//					{
//						forward = Vector3.back;
//					}
//				}
//
////				if (currentPlayer == false) 
////				{
////					ActionEnter actionEnter = GetAction (currentTime + 1) as ActionEnter;
////
////					if (actionEnter != null) 
////					{
////						Vector3 fromPos = fromLocation.transform.position;
////						Vector3 toPos = actionEnter.location.transform.position;
////
////						if (currentTime != currentTimeInterpolated) 
////						{				
////							forward = (toPos - fromPos).normalized;
////						}
////					}
////				}
//			}
//
//			float trackStart = GetTrackStart(currentTimeInterpolated);
//			float trackEnd = GetTrackEnd(currentTime, currentTimeInterpolated, timeForNextDecision, currentPlayer, draggingPlayhead);
//
//			Vector3 localScale = timeLine.bar.localScale;
//			localScale.x = trackEnd - trackStart;
//			timeLine.bar.localScale = localScale;
//
//			Vector3 localPos = timeLine.bar.localPosition;
//			localPos.x = trackStart;
//			timeLine.bar.localPosition = localPos;
//
////			transform.position = pos;
//
//
//			float timeDiff = Mathf.Abs(currentTime - currentTimeInterpolated);
//
//			if (currentPlayer)
//			{
//				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), 14f * Time.deltaTime);
//			}
//			else
//			{
//				if (timeDiff < 0.5f)
//				{
//					SetVisuals(visualsDefault);
//
//					if (currentParadoxes.Count > 0)
//					{
//						Paradox paradox = currentParadoxes[0];
//
//						if (paradox.visuals)
//						{
//							SetVisuals(paradox.visuals);
//						}
//					}
//					else
//					{
//						transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), 30f * Time.deltaTime);
//
////						if (draggingPlayhead) {
////							transform.rotation = Quaternion.LookRotation (forward); 
////							//transform.forward += forward;
////						} else {
////							if (currentPlayer) {
////								transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forward), 14f * Time.deltaTime);
////							} else {
////								transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forward), 30f * Time.deltaTime);
////							}
////							//transform.forward += (forward - transform.forward) * 8f * Time.deltaTime;
////						}
//					}
//				}
//			}
//
//			if (Mathf.Abs(transform.rotation.eulerAngles.y - 180f) < 5f)
//			{
//				if (currentTime > 0)
//				{
////					visualsWait.SetActive (true);
////					visualsDefault.SetActive (false);
//				}
////				Action actionCurrent = GetAction (currentTime);
////				if (actionCurrent != null) {
////					ActionWait actionWai = actionCurrent as ActionWait;
////					if (actionWai != null) {
////						visualsWait.SetActive (true);
////						visualsDefault.SetActive (false);
////					}
////				}
////
////				if (GetLastTime () == currentTime) {
////					actionCurrent = GetAction (currentTime - 1);
////					if (actionCurrent != null) {
////						ActionWait actionWai = actionCurrent as ActionWait;
////						if (actionWai != null) {
////							visualsWait.SetActive (true);
////							visualsDefault.SetActive (false);
////						}
////					}
////				}
//			}
//		}

		public Action GetAction(int targetTime)
		{
			if (targetTime < 0)
				return null;
			
			int time = 0;

			for (int i = 0, count = history.Count; i < count; i++)
			{
				Action action = history[i];
				time += action.duration;

				if (time > targetTime)
					return action;
			}

			return null;
		}

		public void UndoActions(int currentTime)
		{	
			for (int i = history.Count - 1; i >= 0; i--)
			{				
				Action action = history[i];

				if (action.time >= currentTime)
				{
					history.RemoveAt(i);
				}
				else
				{
					break;
				}
			}

			timeLine.RemoveSymbols(currentTime);

			for (int i = observations.Count - 1; i >= 0; i--)
			{
				Observation observation = observations[i];
				     
				if (observation.time > currentTime)
				{
					observations.RemoveAt(i);
				}
			}
		}

		public int RemoveLastAction()
		{
			int lastIndex = history.Count - 1; 
			Action action = history[lastIndex];
			history.RemoveAt(lastIndex);

			return action.time;
		}

		public void CreateCurrentObservation(int currentTime, List<Character> characters)
		{
			currentObservation = MakeObservation(currentTime, characters);
		}


		public void CheckObservations(int currentTime)
		{
			for (int i = 0, count = observations.Count; i < count; i++)
			{
				Observation expectedObservation = observations[i];

				if (expectedObservation.time == currentTime)
				{
					CompareObservations(currentObservation, expectedObservation);
					break;
				}				
			}
		}

		public void CompareObservations(Observation currentObservation, Observation expectedObservation)
		{
            List<Character> currentCharacters = new List<Character>(currentObservation.characters);

			//Expected Characters
			for (int i = 0, count = expectedObservation.characters.Count; i < count; i++)
			{
				Character character = expectedObservation.characters[i];

				if (currentCharacters.Remove(character) == false)
				{	
                    Paradox paradox = new Paradox(currentObservation.time, visualsMeet, character);
					currentParadoxes.Add(paradox);	
				}			
			}

			//Unexpected Characters
			for (int i = 0, count = currentCharacters.Count; i < count; i++)
			{
                Paradox paradox = new Paradox(currentObservation.time, visualsMeet, currentCharacters[i]);
				currentParadoxes.Add(paradox);
			}

            //Expected Items
            List<ItemProfile> currentItemProfiles = new List<ItemProfile>(currentObservation.itemProfiles);

            for (int i = 0, count = expectedObservation.itemProfiles.Count; i < count; i++)
            {
                ItemProfile expectedProfile = expectedObservation.itemProfiles[i];

                if (currentItemProfiles.Remove(expectedProfile) == false)
                {
                    //Expected item not found, locate paradoxes
                    for (int s = 0, currentCount = currentItemProfiles.Count; s < currentCount; s++)
                    {
                        ItemProfile currentProfile = currentItemProfiles[s];

                        if (currentProfile.CompareSurface(expectedProfile))
                        {                        
                            for (int t = 0; t < expectedProfile.itemsInsideProfiles.Length; t++)
                            {                                
                                ItemProfile expectedInsideProfile = expectedProfile.itemsInsideProfiles[t];

                                if (currentProfile.itemsInsideProfiles.Length <= t || expectedInsideProfile != currentProfile.itemsInsideProfiles[t])
                                {
                                    Paradox paradox = new Paradox(currentObservation.time, visualsMeet, expectedInsideProfile);
                                    currentParadoxes.Add(paradox);
                                }
                            }        
                        }
                    }
                }
            }
		}

		public void AddCurrentObservation()
		{		
			observations.Add(currentObservation);
		}

		private Observation MakeObservation(int currentTime, List<Character> characters)
		{
			Location currentLocation = GetLocationAtTime(currentTime);
			
			List<Character> observedCharacters = new List<Character>();

			if (currentLocation.isTimeMachine == false)
			{
				for (int i = 0, count = characters.Count; i < count; i++)
				{
					Character character = characters[i];
				
					if (character != this)
					{
						Location location = character.GetLocationAtTime(currentTime);
					
						if (location == currentLocation)
						{
							observedCharacters.Add(character);
						}
					}
				}
			}

			return new Observation(currentTime, currentLocation, observedCharacters);
		}

	}
}
