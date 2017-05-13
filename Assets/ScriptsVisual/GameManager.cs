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
		public Transform playhead;
		public Collider canvasCollider;
		public Button forwardButton;
		public Button backwardsButton;
		public MeshRenderer playheadRenderer;

		public Location timeMachine;
		public Material[] playerMaterials;
		public Material whiteMaterial;

		[Header("--- Timeline ---")]
		public GameObject timeline;
		public Collider timelineCollider;
		public MeshRenderer timelineBackground;

		private Material timelineBackgroundMaterial;

		private Character currentPlayer;

		private Level currentLevel;
		private int currentTime;
		private float currentTimeInterpolated;
		private int timeForNextDecision;

		private float blinkTimer;

		private List<Character> characters = new List<Character> ();

		public enum State{None, Load, Ready, Forward, Backwards, Scrub, ScrubWait, Wait, Move, TimeTravel};
		public State state = State.None;

		public RaycastHit[] mouseHits = new RaycastHit[0];
		public bool mousePress;
		public bool mouseRelease;
		public bool mouseDown;
		public bool mousePreviousDown;

		private bool waitForMouseRelease;

		public int timelineMax;
		public bool timeSynced;

		public float scrubWaitInterpolateSpeed;

		public static GameManager instance;

		GameManager()
		{
			instance = this;
		}

		void Start ()
		{
			timelineBackgroundMaterial = timelineBackground.material;

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

			while (currentTimeInterpolated > lastTime && currentTime > lastTime) 
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
						return;
					}
				}

				SetReadyState ();
			}

		}

		void SetTimeTravelState()
		{
			playhead.gameObject.SetActive (false);

			state = State.TimeTravel;
		}


		void TimeTravelState()
		{
//			if (waitForMouseRelease) 
//			{
//				if (mouseRelease) 
//				{
//					timelineBackground.sharedMaterial = timelineBackgroundMaterial; 
//					waitForMouseRelease = false;
//
//					SetReadyState ();	
//				}
//			}
//			else
//			{ 
				if (blinkTimer > 0f) 
				{
					blinkTimer -= Time.deltaTime;
				}
				else
				{
					if (timelineBackground.sharedMaterial == whiteMaterial)
					{
						timelineBackground.sharedMaterial = timelineBackgroundMaterial; 
						blinkTimer = 0.4f;
					}
					else 
					{
						timelineBackground.sharedMaterial = whiteMaterial; 
						blinkTimer = 0.15f;
					}
				}

				if (mousePress && MouseDown (timelineCollider)) 
				{
					Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);

					RaycastHit hit;

					if (canvasCollider.Raycast (mouseRay, out hit, 100f)) 
					{
						Debug.DrawLine (mouseRay.origin, hit.point, Color.red);
						Vector3 hitLocalPos = timeline.transform.InverseTransformPoint (hit.point);

						float nextPlayheadPos = Mathf.Clamp (hitLocalPos.x, 0, timelineMax);
						int nextTime = Mathf.FloorToInt (nextPlayheadPos);

						currentTime = nextTime;
						currentTimeInterpolated = nextTime;
						timeForNextDecision = nextTime;

						playhead.gameObject.SetActive (true);

						timelineBackground.sharedMaterial = timelineBackgroundMaterial; 
						blinkTimer = 0;

						SetReadyState ();	

//						waitForMouseRelease = true;
					}
				}
//			}	
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
			
		void TimeTravel()
		{
//			currentLevel.Reset();

//			for (int i = 0, count = characters.Count; i < count; i++) 
//				characters[i].Reset();	

			CreatePlayer (timeMachine);

//			currentTime = 0;
//			currentTimeInterpolated = 0;
//			timeForNextDecision = 0;

			SetTimeTravelState();
		}	

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

					if (action.time >= currentTime) 
					{
						currentPlayer.history.Remove (action);
					}
				}
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
	}
}
