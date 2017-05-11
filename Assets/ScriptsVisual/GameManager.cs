using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
		public Collider canvasCollider;
		public Button forwardButton;
		public Button backwardsButton;
		public MeshRenderer playheadRenderer;

//		public MouseEvents playheadMouseEvents;
		public Location timeMachine;
		public Material[] playerMaterials;

		private Character currentPlayer;

		private Level currentLevel;
		private int currentTime;
		private float currentTimeInterpolated;
		private int timeForNextDecision;

		private List<Character> characters = new List<Character> ();

		public enum State{None, Load, Ready, Forward, Backwards, Scrub, ScrubWait, Wait, Move, TimeTravel};
		public State state = State.None;

		public RaycastHit[] mouseHits = new RaycastHit[0];
		public bool mousePress;
		public bool mouseRelease;
		public bool mouseDown;
		public bool mousePreviousDown;

		public int timelineMax;
		public bool timeSynced;

		public float scrubWaitInterpolateSpeed;

//		public static GameManager instance;
//		private bool playheadMouseDown;
//		private bool draggingPlayhead;
//		private bool instantProgresss;

//		GameManager()
//		{
//			instance = this;
//		}

		void Start ()
		{
			SetLoadState (initialLevel);
		}
			
		void SetLoadState(Level level)
		{
			if(currentLevel)
				currentLevel.gameObject.SetActive (false);

			level.gameObject.SetActive (true);

			currentLevel = level;

			timelineMax = 23;
			CreatePlayer (level.initialLocation);

			state = State.Load;
		}

		void LoadState()
		{	
			SetReadyState ();
		}

		void SetReadyState()
		{
			state = State.Ready;
		}

		void ReadyState()
		{
			//Forward
			if (MouseDown (forwardButton.collider) && currentTime < timelineMax) 
			{				
				SetForwardState ();
				return;
			}

			//Backwards
			if (MouseDown (backwardsButton.collider) && currentTime > 0) 
			{
				SetBackwardsState ();
				return;
			}

			//Scrub
			if (MouseDown(timelineCollider)) 
			{
				SetScrubState ();
				return;
			}

			//----// Interactions //----//

			Location location = MouseDown<Location>();
			if(location)
			{
				if (EnterLocation (location)) 
				{
					SetMoveState ();
					return;
				}

				if (WaitLocation (location)) 
				{
					SetWaitState ();
					return;
				}
			}
		}

		void SetScrubState()
		{
			state = State.Scrub;
		}

		void ScrubState()
		{
			Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			float nextPlayheadPos = currentTime;

			RaycastHit hit;
			if (canvasCollider.Raycast (mouseRay, out hit, 100f)) 
			{
				Debug.DrawLine (mouseRay.origin, hit.point, Color.red);
				Vector3 hitLocalPos = timeline.transform.InverseTransformPoint (hit.point);

				nextPlayheadPos = Mathf.Clamp(hitLocalPos.x, 0, timelineMax);
			}

			if (mouseDown == false)
			{
				currentTime = Mathf.FloorToInt (nextPlayheadPos);

				timeForNextDecision = currentTime;

				float diff = Mathf.Abs(currentTime - currentTimeInterpolated);
				float interpolateSpeed = diff > 1f ? 30f : 2f;
				SetScrubWaitState (interpolateSpeed);
								
				//UndoActions ();
			}
			else 
			{
//				float dragTime = nextPlayheadPos;

				currentTimeInterpolated = Mathf.MoveTowards(currentTimeInterpolated, nextPlayheadPos, 30f * Time.deltaTime);
				currentTime = Mathf.FloorToInt(nextPlayheadPos);

				//Add wait actions to current player
				//Debug.Log(currentPlayer.GetLastTime());


			}

			int lastTime = currentPlayer.GetLastTime ();

			while (currentTimeInterpolated - 1 > lastTime) 
			{						
				Action actionWait = new ActionWait (lastTime, 1);
				AddAction (actionWait, false);
				lastTime += actionWait.duration; 
			}
		}

		void SetScrubWaitState(float interpolateSpeed)
		{
			scrubWaitInterpolateSpeed = interpolateSpeed;

			state = State.ScrubWait;				
		}

		void ScrubWaitState()
		{
			InterpolateTime (scrubWaitInterpolateSpeed);

			int lastTime = currentPlayer.GetLastTime ();

			if (currentTimeInterpolated > lastTime && currentTime > lastTime) 
			{						
				Action actionWait = new ActionWait (lastTime, 1);
				AddAction (actionWait, false);
			}

			if (timeSynced) 
			{
				SetReadyState ();
			}
		}

		void SetForwardState()
		{
			if (currentPlayer.GetAction (currentTime) == null)
			{
				Action actionWait = new ActionWait (currentTime, 1);
				AddAction (actionWait);
			}
			else 
			{
				timeForNextDecision++;
				currentTime++;
			}

			forwardButton.Toggle (true);

			state = State.Forward;
		}

		void ForwardState()
		{			
			InterpolateTime ();

			if (timeSynced) 
			{
				forwardButton.Toggle (false);

				SetReadyState ();
			}
		}

		void SetBackwardsState()
		{
			currentTime--;
			timeForNextDecision = currentTime;

			backwardsButton.Toggle (true);

			state = State.Backwards;
		}

		void BackwardsState()
		{
			InterpolateTime ();

			if (timeSynced) 
			{
				backwardsButton.Toggle (false);

				SetReadyState ();
			}
		}

		void SetWaitState()
		{
			state = State.Wait;
		}

		void WaitState()
		{
			InterpolateTime ();

			if (timeSynced) 
			{
				SetReadyState ();
			}
		}

		void SetMoveState()
		{
			state = State.Move;	
		}

		void MoveState()
		{
			InterpolateTime ();

			if (timeSynced) 
			{
				Action action = currentPlayer.GetLastAction();

				if (action != null) 
				{
					ActionEnter actionEnter = action as ActionEnter;

					if (actionEnter != null && actionEnter.location == timeMachine) 
					{
						TimeTravel ();
					}
				}

				SetReadyState ();
			}

		}

		void SetTimeTravelState()
		{
			state = State.TimeTravel;
		}

		void TimeTravelState()
		{
			
		}

		void Update()
		{
			UpdateMouseInput ();

			timeSynced = currentTimeInterpolated == currentTime; 

			switch (state) 
			{
			case State.Load:
				LoadState ();
				break;
			case State.Ready:
				ReadyState ();
				break;
			case State.Scrub:
				ScrubState ();
				break;	
			case State.ScrubWait:
				ScrubWaitState ();
				break;
			case State.Forward:
				ForwardState ();
				break;
			case State.Backwards:
				BackwardsState ();
				break;			
			case State.Wait:
				WaitState ();
				break;
			case State.Move:
				MoveState ();
				break;
			case State.TimeTravel:
				TimeTravelState ();
				break;
			}

			//----// Updates //----//

			//Playhead
			Vector3 playheadPos = playhead.localPosition;
			playheadPos.x = currentTimeInterpolated;
			playhead.localPosition = playheadPos;

			//Characters
			for (int i = 0; i < characters.Count; i++) 
			{
				Character character = characters [i];	
				character.UpdateCharacter (currentTime, currentTimeInterpolated, timeForNextDecision, character == currentPlayer, false);
			}

			//Locations
			for (int i = 0, length = currentLevel.locations.Length; i < length; i++) 
			{
				Location location = currentLevel.locations [i];
				if (location.cover) 
				{
					location.cover.SetActive (true);

					for (int j = 0, count = characters.Count; j < count; j++) 
					{
						Character character = characters [j];
						if (character.GetLocationAtTime (Mathf.RoundToInt(currentTimeInterpolated)) == location)
						{
							location.cover.SetActive (false);
							break;
						}
					}
				}
			}

			//Reload scene on Escape
			if (Input.GetKeyDown (KeyCode.Escape)) 
			{
				SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
			}

//			return;
//
//			MouseInput();
//
//			if (draggingPlayhead) 
//			{
//				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
//				float nextPlayheadPos = currentTime;
//
//				RaycastHit hit;
//				if (timelineCollider.Raycast (mouseRay, out hit, 100f)) 
//				{
//					Debug.DrawLine (mouseRay.origin, hit.point, Color.red);
//					Vector3 hitLocalPos = timeline.transform.InverseTransformPoint (hit.point);
//					nextPlayheadPos = Mathf.Clamp(hitLocalPos.x, 0, 23);
//				}
//
////				if (hits.Length > 0) 
////				{
////					for (int i = 0, length = hits.Length; i < length; i++) 
////					{
////						Transform hitTransform = hits [i].collider.gameObject.transform;
////
////						if (hitTransform == timelineCollider) 
////						{
////							draggingPlayhead = true;
////							break;
////						}	
////					}
////				}
//
////				Vector3 mousePos = Input.mousePosition;
////				mousePos.z = 15;
////				Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint (mousePos);
////				Vector3 mouseLocalPos = timeline.transform.InverseTransformPoint (mouseWorldPos);
////				mouseLocalPos.x = Mathf.Clamp(mouseLocalPos.x, 0, 23);
//
////				Debug.DrawLine (Camera.main.transform.position, mouseWorldPos);
//
//				if (Input.GetMouseButtonUp (0))
//				{
//					draggingPlayhead = false;
//
////					playheadMouseDown = false;
//
//					currentTime = Mathf.FloorToInt (nextPlayheadPos); //mouseLocalPos.x);
//
//					timeForNextDecision = currentTime;
//
//					//UndoActions ();
//				}
//				else 
//				{
//					float dragTime = nextPlayheadPos;//mouseLocalPos.x;
//
//					currentTimeInterpolated = Mathf.MoveTowards(currentTimeInterpolated, dragTime, 30f * Time.deltaTime);
//
//					currentTime = Mathf.FloorToInt(dragTime);
//				}
//
//				//Add wait actions to current player
//				if (currentTime > currentPlayer.GetLastTime()) 
//				{
//					Action actionWait = new ActionWait (currentTime - 1, 1);
//					AddAction (actionWait);
//				}
//
////				if (currentTime > timeForNextDecision) 
////				{
////					Action actionWait = new ActionWait (currentTime - 1, 1);
////					AddAction (actionWait);
////				}
//			}
//			else 
//			{
//				float diff = currentTime - currentTimeInterpolated;
//				float addTime = Mathf.Sign (diff) * 2f * Time.deltaTime; 
//
//				if (Mathf.Abs (addTime) >  Mathf.Abs (diff)) 
//				{
//					currentTimeInterpolated = currentTime;
//				}
//				else 
//				{
//					currentTimeInterpolated += addTime;
//				}
//
//				//Make Decisionsffff
//				if (timeForNextDecision > currentTime && currentTimeInterpolated == currentTime) 
//				{
//					currentTime++;
//					//Progress ();
//				}
//				else 
//				{
//					if (currentTimeInterpolated == currentTime) 
//					{
//						//UndoActions ();
//
////						Location location = currentPlayer.GetLocationAtTime (currentTime);
//						Action action = currentPlayer.GetLastAction();
//
//						if (action != null) 
//						{
//							ActionEnter actionEnter = action as ActionEnter;
//
//							if (actionEnter != null && actionEnter.location == timeMachine)   
//							{
//								TimeTravel ();
//							}
//						}
////						if (location == timeMachine) 
////						{
////							Location previousLocation = currentPlayer.GetLocationAtTime (currentTime - 2);
////							Debug.Log (previousLocation);
////
////							if(previousLocation != timeMachine)
////								TimeTravel ();
////						}
//					}
//				}
//
////				currentTimeInterpolated = Mathf.Lerp (currentTimeInterpolated, currentTime, 3f * Time.deltaTime);
//
//				//Start drag
////				if (playheadMouseDown) 
////				{
////					draggingPlayhead = true;
////				}
//			}
////
////			if (instantProgresss) 
////			{
////				//Instant progress
////				while (timeForNextDecision > currentTime) 
////				{
////					Tick ();
////				}
////
////				instantProgresss = false;
////			}
//
////			Vector3 playheadPos = playhead.localPosition;
////			playheadPos.x = currentTimeInterpolated;
////			playhead.localPosition = playheadPos;
////
////			for (int i = 0; i < characters.Count; i++) 
////			{
////				Character character = characters [i];	
////				character.UpdateCharacter (currentTime, currentTimeInterpolated, timeForNextDecision, character == currentPlayer, draggingPlayhead);
////			}
////
////
////
////			for (int i = 0, length = currentLevel.locations.Length; i < length; i++) 
////			{
////				Location location = currentLevel.locations [i];
////				if (location.cover) 
////				{
////					location.cover.SetActive (true);
////
////					for (int j = 0, count = characters.Count; j < count; j++) 
////					{
////						Character character = characters [j];
////						if (character.GetLocationAtTime (Mathf.RoundToInt(currentTimeInterpolated)) == location)
////						{
////							location.cover.SetActive (false);
////							break;
////						}
////					}
////				}
////			}
		}

		void UpdateMouseInput()
		{
			mousePreviousDown = mouseDown;

			mouseDown = Input.GetMouseButton (0);
			mousePress = mouseDown && !mousePreviousDown;
			mouseRelease = !mouseDown && mousePreviousDown;

			if (Input.GetMouseButton (0)) 
			{				
				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				mouseHits = Physics.RaycastAll (mouseRay, 100);
			}
			else 
			{
				mouseHits = new RaycastHit[0];
			}
		}

		public bool MouseDown(Collider collider)
		{
			for (int i = 0, length = mouseHits.Length; i < length; i++) 
			{
				Collider hitCollider = mouseHits [i].collider;
			
				if (collider == hitCollider)
					return true;
			}

			return false;
		}

		public T MouseDown<T>() where T : class
		{
			for (int i = 0, length = mouseHits.Length; i < length; i++) 
			{
				Collider hitCollider = mouseHits [i].collider;
				Transform hitTransform = hitCollider.gameObject.transform;

				if (hitTransform.parent)
				{
					T hitComponent = hitTransform.parent.GetComponent<T> ();

					if(hitComponent != null)
						return hitComponent;
				}
			}

			return null;
		}

		void InterpolateTime(float speed = 2f)
		{
			float diff = currentTime - currentTimeInterpolated;
			float addTime = Mathf.Sign (diff) * speed * Time.deltaTime; 

			if (Mathf.Abs (addTime) <  Mathf.Abs (diff)) 
			{
				currentTimeInterpolated += addTime;				
			}
			else
			{
				currentTimeInterpolated = currentTime;
			}
		}

//		void MouseInput()
//		{
//			if (Input.GetMouseButton (0)) 
//			{	
//				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
//				RaycastHit[] hits = Physics.RaycastAll (mouseRay, 100);
//
//				if (hits.Length > 0) 
//				{
//					for (int i = 0, length = hits.Length; i < length; i++) 
//					{
//						Collider hitCollider = hits [i].collider;
//						Transform hitTransform = hitCollider.gameObject.transform;
//
//						if (hitCollider == forwardButton.collider && currentTimeInterpolated == timeForNextDecision ) 
//						{
//							if (currentTime < 23) 
//							{
//								//int time = currentPlayer.GetNextActionTime ();
//								int time = currentTime;
//
//								if (currentPlayer.GetAction (time) == null) {
//									Action actionWait = new ActionWait (time, 1);
//									AddAction (actionWait);
//								}
//								else 
//								{
//									timeForNextDecision += 1;
//								}
//							}
//
//							forwardButton.Toggle (true);
//
//							break;
//						}
//
//						if (hitCollider == backwardsButton.collider && currentTimeInterpolated == timeForNextDecision) 
//						{
//							if (currentTime > 0) 
//							{
//								currentTime--;
//								timeForNextDecision = currentTime;
//							}
//
//							backwardsButton.Toggle (true);
//
//							break;
//						}
//
//						Location location = hitTransform.parent.GetComponent<Location> ();
//						if (location && currentTimeInterpolated == timeForNextDecision) 
//						{
//							bool entered = EnterLocation (location);
//
//							if (entered) 
//								break;
//						}
//					}
//
//					for (int i = 0, length = hits.Length; i < length; i++) 
//					{
//						Collider hitCollider = hits [i].collider;
//						Transform hitTransform = hitCollider.gameObject.transform;
//
//						Location location = hitTransform.parent.GetComponent<Location> ();
//
//						if (location && currentTimeInterpolated == timeForNextDecision) {
//							bool waited = WaitLocation (location);
//
//							if (waited)
//								break;
//						}
//					}
//				}
//			}
//			else 
//			{
//				if (currentTimeInterpolated == timeForNextDecision) 
//				{
//					if (forwardButton.value)
//						forwardButton.Toggle (false);
//
//					if (backwardsButton.value)
//						backwardsButton.Toggle (false);
//				}				
//			}
//
//			if (Input.GetMouseButtonDown (0)) 
//			{	
//				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
//				RaycastHit[] hits = Physics.RaycastAll (mouseRay, 100);
//
////				Debug.Log (hits.Length);	
//
//				if (hits.Length > 0) 
//				{
//					for (int i = 0, length = hits.Length; i < length; i++) 
//					{
//						Collider hitCollider = hits [i].collider;
//						Transform hitTransform = hitCollider.gameObject.transform;
//
////						if (hitTransform == playhead) 
////						{
////							draggingPlayhead = true;
////							break;
////						}
//
//						if (hitCollider == timelineCollider) 
//						{
//							draggingPlayhead = true;
//							break;
//						}
//
////						Location location = hitTransform.parent.GetComponent<Location> ();
////						if (location) 
////						{
////							bool entered = EnterLocation (location);
////
////							if(entered)
////								break;
////						}
//
////						if (hitCollider == waitButtonCollider) 
////						{
////							int time = currentPlayer.GetNextActionTime ();
////
////							Action actionWait = new ActionWait (time, 1);
////							AddAction (actionWait);
////
////							break;
////						}
//
////						if (hitCollider == undoButtonCollider) 
////						{
////							if (currentTime > 0) 
////							{
////								currentTime--;
////								timeForNextDecision = currentTime;
////							}
////
////							break;
////						}
//						//Debug.Log (hits [i].collider.gameObject.name);	
//					}
//
//					//Debug.DrawLine (mouseRay.origin, mouseRay.origin + mouseRay.direction * 100f);	
//				}
//
//			}
//		}

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
//			currentLevel.Reset();

//			for (int i = 0, count = characters.Count; i < count; i++) 
//				characters[i].Reset();	

			CreatePlayer (timeMachine);

			//currentTime = 0;
			//currentTimeInterpolated = 0;
			//timeForNextDecision = 0;
		}	

//		void UndoLastAction()
//		{		
//			//Remove action and observation from player
//			if (currentPlayer.history.Count > 0) 
//			{
//				//int decisionTime = currentPlayer.RemoveLastAction ();
//				//timeForNextDecision--;
//				//timeForNextDecision = decisionTime;
//
////				currentLevel.Reset();
//
//				//for (int i = 0, count = characters.Count; i < count; i++) 
//				//	characters[i].Reset();	
//				
//				//instantProgresss = true;
//				//currentTime = 0;
//			}
//		}

		public bool EnterLocation(Location location)
		{
			Location currentLocation = currentPlayer.GetLocationAtTime (currentTime);

			if (currentLocation == timeMachine && location != timeMachine)
			{				
				Action actionEnter = new ActionEnter (currentTime, timeMachine.connectedLocations [0], 1);
				AddAction (actionEnter);

				return true;
			}
			else if (location != currentLocation)
			{
				for (int i = 0, length = currentLocation.connectedLocations.Length; i < length; i++)
				{
					if (currentLocation.connectedLocations [i] == location) 
					{					
						Action actionEnter = new ActionEnter (currentTime, location, 1);
						AddAction (actionEnter);

						return true;
					}
				}
			}

			return false;
		}

		public bool WaitLocation(Location location)
		{
			Location currentLocation = currentPlayer.GetLocationAtTime (currentTime);

			if (location == currentLocation && currentTime < timelineMax)			
			{
				Action actionWait = new ActionWait (currentTime, 1);
				AddAction (actionWait);

				return true;
			}	

			return false;
		}

		public void AddAction(Action action, bool increaseCurrentTime = true)
		{
			UndoActions();

			currentPlayer.history.Add(action);

			timeForNextDecision += action.duration;

			if (increaseCurrentTime) 
			{
				currentTime += action.duration;
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

		private void CreatePlayer(Location location)
		{
			GameObject newPlayer = Instantiate (characterPrefab, currentLevel.initialLocation.transform.position, Quaternion.LookRotation(Vector3.back));
			currentPlayer = newPlayer.GetComponent<Character> ();

			GameObject newTimelineTrack = Instantiate (characterTimelinePrefab, timeline.transform.position, timeline.transform.rotation);
			newTimelineTrack.transform.parent = timeline.transform;

			Vector3 trackPosLocal = Vector3.zero;
			trackPosLocal.y = characters.Count * -0.75f;
			newTimelineTrack.transform.localPosition = trackPosLocal;

			Material material = playerMaterials [characters.Count % playerMaterials.Length];
			currentPlayer.Setup (location, material, newTimelineTrack.transform);
			playheadRenderer.material = material;

			characters.Add (currentPlayer);		
		}

//		void PlayheadMouseDown(Transform obj)
//		{
//			playheadMouseDown = true;
//		}
	}
}
