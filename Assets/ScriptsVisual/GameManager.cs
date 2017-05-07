using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class GameManager : MonoBehaviour 
	{
		public Level initialLevel;
		public GameObject characterPrefab;
		public GameObject characterTimelinePrefab;
		public GameObject timeline;
		public Transform playhead;
		public Collider timelineCollider;
		public Collider waitButtonCollider;
		public Collider undoButtonCollider;

//		public MouseEvents playheadMouseEvents;
		public Location timeMachine;
		public Material[] playerMaterials;

		private Character currentPlayer;

		private Level currentLevel;
		private int currentTime;
		private float currentTimeInterpolated;
		private int timeForNextDecision;

		private List<Character> characters = new List<Character> ();

//		public static GameManager instance;
//		private bool playheadMouseDown;
		private bool draggingPlayhead;
//		private bool instantProgresss;

//		GameManager()
//		{
//			instance = this;
//		}

		void Start ()
		{
			LoadLevel (initialLevel);

//			playheadMouseEvents.mouseDown = PlayheadMouseDown;
		}
			
		void LoadLevel(Level level)
		{
			currentLevel = level;

			CreatePlayer (currentLevel.initialLocation);				
		}

		private void CreatePlayer(Location initialLocation)
		{
			GameObject newPlayer = Instantiate (characterPrefab, currentLevel.initialLocation.transform.position, Quaternion.LookRotation(Vector3.back));
			currentPlayer = newPlayer.GetComponent<Character> ();

			Vector3 trackPos = timeline.transform.position;
			trackPos += timeline.transform.up * (characters.Count - 1) * -0.5f;

			GameObject newTimelineTrack = Instantiate (characterTimelinePrefab, trackPos, timeline.transform.rotation);
			newTimelineTrack.transform.parent = timeline.transform;

			currentPlayer.Setup (initialLocation, playerMaterials[characters.Count % playerMaterials.Length], newTimelineTrack.transform);

			characters.Add (currentPlayer);		
		}

		public bool EnterLocation(Location location)
		{
			int time = currentPlayer.GetNextActionTime ();
			Location currentLocation = currentPlayer.GetLocationAtTime (time);

			if (currentLocation == timeMachine)
			{				
				Action actionEnter = new ActionEnter (time, timeMachine.connectedLocations [0], 1);
				AddAction (actionEnter);

				return true;
			}
			else 
			{
				if (location != currentLocation) 
				{
					for (int i = 0, length = currentLocation.connectedLocations.Length; i < length; i++) 
					{
						if (currentLocation.connectedLocations [i] == location) 
						{					
							Action actionEnter = new ActionEnter (time, location, 1);
							AddAction (actionEnter);

							return true;
						}
					}
				}	
			}

			return false;
		}

		public void AddAction(Action action)
		{
			currentPlayer.history.Add(action);
			timeForNextDecision += action.duration;
		}
			
		void Update()
		{
//			Debug.Log (currentTime + " , " +currentTimeInterpolated);

			MouseInput();

			if (draggingPlayhead) 
			{
				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);

				float nextPlayheadPos = currentTime;

				RaycastHit hit;
				if (timelineCollider.Raycast (mouseRay, out hit, 100f)) 
				{
					Debug.DrawLine (mouseRay.origin, hit.point, Color.red);
					Vector3 hitLocalPos = timeline.transform.InverseTransformPoint (hit.point);
					nextPlayheadPos = Mathf.Clamp(hitLocalPos.x, 0, 23);
				}

//				if (hits.Length > 0) 
//				{
//					for (int i = 0, length = hits.Length; i < length; i++) 
//					{
//						Transform hitTransform = hits [i].collider.gameObject.transform;
//
//						if (hitTransform == timelineCollider) 
//						{
//							draggingPlayhead = true;
//							break;
//						}	
//					}
//				}

//				Vector3 mousePos = Input.mousePosition;
//				mousePos.z = 15;
//				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint (mousePos);
//				Vector3 mouseLocalPos = timeline.transform.InverseTransformPoint (mouseWorldPos);
//				mouseLocalPos.x = Mathf.Clamp(mouseLocalPos.x, 0, 23);

//				Debug.DrawLine (Camera.main.transform.position, mouseWorldPos);

				if (Input.GetMouseButtonUp (0))
				{
					draggingPlayhead = false;
//					playheadMouseDown = false;

					currentTime = Mathf.FloorToInt (nextPlayheadPos);//mouseLocalPos.x);
					timeForNextDecision = currentTime;

					UndoActions ();
				}
				else 
				{
					float dragTime = nextPlayheadPos;//mouseLocalPos.x;

					currentTimeInterpolated = Mathf.MoveTowards(currentTimeInterpolated, dragTime, 30f * Time.deltaTime);

					currentTime = Mathf.FloorToInt(dragTime);
				}

				//Add wait actions to current player
				if (currentTime > timeForNextDecision) 
				{
					Action actionWait = new ActionWait (currentTime - 1, 1);
					AddAction (actionWait);
				}
			}
			else 
			{
				float diff = currentTime - currentTimeInterpolated;
				float addTime = Mathf.Sign (diff) * 3f * Time.deltaTime; 

				if (Mathf.Abs (addTime) >  Mathf.Abs (diff)) 
				{
					currentTimeInterpolated = currentTime;
				}
				else 
				{
					currentTimeInterpolated += addTime;
				}

				//Make Decisions
				if (timeForNextDecision > currentTime) 
				{
					currentTime++;
					//Progress ();
				}
				else 
				{
					if (currentTimeInterpolated == currentTime) 
					{
						UndoActions ();

//						Location location = currentPlayer.GetLocationAtTime (currentTime);
						Action action = currentPlayer.GetLastAction();

						if (action != null) 
						{
							ActionEnter actionEnter = action as ActionEnter;

							if (actionEnter != null && actionEnter.location == timeMachine)   
							{
								TimeTravel ();
							}
						}
//						if (location == timeMachine) 
//						{
//							Location previousLocation = currentPlayer.GetLocationAtTime (currentTime - 2);
//							Debug.Log (previousLocation);
//
//							if(previousLocation != timeMachine)
//								TimeTravel ();
//						}
					}
				}

//				currentTimeInterpolated = Mathf.Lerp (currentTimeInterpolated, currentTime, 3f * Time.deltaTime);

				//Start drag
//				if (playheadMouseDown) 
//				{
//					draggingPlayhead = true;
//				}
			}
//
//			if (instantProgresss) 
//			{
//				//Instant progress
//				while (timeForNextDecision > currentTime) 
//				{
//					Tick ();
//				}
//
//				instantProgresss = false;
//			}

			Vector3 playheadPos = playhead.localPosition;
			playheadPos.x = currentTimeInterpolated;
			playhead.localPosition = playheadPos;

			for (int i = 0; i < characters.Count; i++) 
			{
				Character character = characters [i];	
				character.UpdateCharacter (currentTime, currentTimeInterpolated, character == currentPlayer, draggingPlayhead);
			}
		}

		void MouseInput()
		{
			if (Input.GetMouseButtonDown (0)) 
			{	
				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit[] hits = Physics.RaycastAll (mouseRay, 100);

//				Debug.Log (hits.Length);	

				if (hits.Length > 0) 
				{
					for (int i = 0, length = hits.Length; i < length; i++) 
					{
						Collider hitCollider = hits [i].collider;
						Transform hitTransform = hitCollider.gameObject.transform;

						if (hitTransform == playhead) 
						{
							draggingPlayhead = true;
							break;
						}

						Location location = hitTransform.parent.GetComponent<Location> ();
						if (location) 
						{
							bool entered = EnterLocation (location);

							if(entered)
								break;
						}

						if (hitCollider == waitButtonCollider) 
						{
							int time = currentPlayer.GetNextActionTime ();

							Action actionWait = new ActionWait (time, 1);
							AddAction (actionWait);

							break;
						}

						if (hitCollider == undoButtonCollider) 
						{
							if (currentTime > 0) 
							{
								currentTime--;
								timeForNextDecision = currentTime;
							}

							break;
						}
						//Debug.Log (hits [i].collider.gameObject.name);	
					}

					//Debug.DrawLine (mouseRay.origin, mouseRay.origin + mouseRay.direction * 100f);	
				}

			}
		}

//		void Progress()
//		{
//			if(timeForNextDecision > currentTime)
//			{		
//				Tick();
//			}
//		}
//
//		void Tick()
//		{
//			PerformActions ();
//
//			//currentTime++;
//
//			ActionEnter actionEnter = currentPlayer.GetAction((int)currentTime - 1) as ActionEnter;
//			if(actionEnter != null && actionEnter.location == timeMachine)
//			{
//				TimeTravel();		
//			}
//		}
//
//		bool PerformActions()
//		{
//			for (int i = 0; i < characters.Count; i++) 
//			{
//				Character player = characters[i];
//
//				for (int k = 0; k < player.history.Count; k++) 
//				{
//					Action action = player.history[k];
//					int startTime = action.time;
//					int performTime = startTime + action.duration - 1;
//
//					if(currentTime < startTime)
//					{
//						continue;
//					} 
//					else if(currentTime == performTime)
//					{
//						if(action.Perform(player) == false)
//							return false;
//
//						break;
//					}
////					else if(currentTime < performTime)
////					{
////						if(action.Prepare(player) == false)
////							return false;
////
////						break;
////					}
//					else if(startTime > currentTime)
//					{
//						break;
//					}
//				}
//			}
//
//			return true;
//		}
			
		void TimeTravel()
		{
			//currentLevel.Reset();

//			for (int i = 0, count = characters.Count; i < count; i++) 
//				characters[i].Reset();	

			CreatePlayer (timeMachine);

			currentTime = 0;
			currentTimeInterpolated = 0;
			timeForNextDecision = 0;
		}	

		void UndoLastAction()
		{		
			//Remove action and observation from player
			if (currentPlayer.history.Count > 0) 
			{
				//int decisionTime = currentPlayer.RemoveLastAction ();
				//timeForNextDecision--;
				//timeForNextDecision = decisionTime;

//				currentLevel.Reset();

				//for (int i = 0, count = characters.Count; i < count; i++) 
				//	characters[i].Reset();	
				
				//instantProgresss = true;
				//currentTime = 0;
			}
		}

		void UndoActions()
		{
			if (currentPlayer.history.Count > 0) 
			{
				for (int i = currentPlayer.history.Count - 1; i >= 0; i--) 
				{				
					Action action = currentPlayer.history[i];
//					Debug.Log (action.time + " > " + currentTime + " "+(action.time >= currentTime));

					if (action.time >= currentTime) 
					{
						currentPlayer.history.Remove (action);
					}
				}
				//Debug.Log (currentPlayer.history.Count);
				//timeForNextDecision = currentTime;

				//int decisionTime = currentPlayer.RemoveLastAction ();
				//timeForNextDecision--;
				//timeForNextDecision = decisionTime;

				//				currentLevel.Reset();

				//for (int i = 0, count = characters.Count; i < count; i++) 
				//	characters[i].Reset();	

				//instantProgresss = true;
				//currentTime = 0;
			}
		}

//		void PlayheadMouseDown(Transform obj)
//		{
//			playheadMouseDown = true;
//		}
	}
}
