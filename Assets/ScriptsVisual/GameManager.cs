using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Incteractive
{
	public class GameManager : MonoBehaviour
	{
		public Level initialLevel;
		public GameObject characterPrefab;

		public GameObject characterTimelinePrefab;
		public GameObject symbolEnterPrefab;

		public Transform playhead;
		public Collider canvasCollider;
		public Button forwardButton;
		public Button backwardsButton;
		public MeshRenderer playheadRenderer;

		public Location timeMachine;
		public Material[] playerMaterials;
		public Material whiteMaterial;

		public GameObject paradoxParticles;

		[Header("--- Timeline ---")]
		public GameObject timeline;
		public Collider timelineCollider;
		public MeshRenderer timelineBackground;

		private Material timelineBackgroundMaterial;

		public Character currentPlayer;

		private Level currentLevel;
		private int currentTime;
		private float currentTimeInterpolated;
		private int timeForNextDecision;

		private float blinkTimer;

		private List<Character> characters = new List<Character>();
		private List<Item> allItems = new List<Item>();

		public enum State
		{
			None,
			Load,
			Ready,
			Forward,
			Backwards,
			Scrub,
			ScrubWait,
			Action,
			TimeTravelReady,
			TimeTravel,
			Paradox}

		;

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

		void Start()
		{
			timelineBackgroundMaterial = timelineBackground.material;

			SetLoadState(initialLevel);
		}

		void SetLoadState(Level level)
		{
			if (currentLevel)
				currentLevel.gameObject.SetActive(false);

			level.gameObject.SetActive(true);

			timeMachine.connectedLocations = level.timeMachineConnections;

			currentLevel = level;

			timelineMax = 23;
			CreatePlayer(level.initialLocation);

			allItems.AddRange(level.GetComponentsInChildren<Item>().ToList());

			state = State.Load;
		}

		void LoadState()
		{	
			SetReadyState();
		}

		void SetReadyState()
		{
			state = State.Ready;
		}

		void ReadyState()
		{
			//Forward
//			if (MouseDown (forwardButton.collider) && currentTime < timelineMax) 
//			{				
//				SetForwardState ();
//				return;
//			}

			//Backwards
			if (MouseDown(backwardsButton.collider) && currentTime > 0)
			{
				SetBackwardsState();
				return;
			}

			//Scrub
			if (MouseDown(timelineCollider))
			{
				SetScrubState();
				return;
			}

			//Interactions
			bool isInteracting = Interactions();

			if (isInteracting)
			{
				SetActionState();
			}
		}

		bool Interactions()
		{
			Item item = MouseDown<Item>();
            Location location = MouseDown<Location>();
			
            //----// Items //----//
            if (item && location == currentPlayer.currentLocation)
            {
				if (item.isMovable)
				{			
					//Item	
                    if (item.itemAround)
					{						
						float pickupOffset = currentPlayer.GetPickupOffset();
                        Item itemFound = item.itemAround.itemsInside[item.itemAround.itemsInside.Count - 1];
						ActionPickup actionPickup = new ActionPickup(currentTime, 1, itemFound, item.itemAround, pickupOffset);
						AddAction(actionPickup);
						return true;
					}
					else if(item.characterCarrying) 
					{
//						Item foundContainer = currentPlayer.currentLocation.GetContainer();
//
//						if (foundContainer)
//						{
//							Item itemFound = currentPlayer.inventory[currentPlayer.inventory.Count - 1];
//
//							ActionDrop actionDrop = new ActionDrop(currentTime, 1, itemFound, foundContainer); 
//							AddAction(actionDrop);
//							return true;		
//						}
					}

				}
                else if (item.isContainer)
				{
					//Item Container
//					if (item.itemsInside.Count > 0)
//					{
//						ActionPickup actionPickup = new ActionPickup(currentTime, 1, item.itemsInside[0], item);
//						AddAction(actionPickup);
//						return true;
//					}
//					else
//					{
						if (currentPlayer.inventory.Count > 0)
						{							
							Item itemFound = currentPlayer.inventory[currentPlayer.inventory.Count - 1];
							ActionDrop actionDrop = new ActionDrop(currentTime, 1, itemFound, item); 
							AddAction(actionDrop);
							return true;
						}
//					}
				}
			}
			else
			{
				//----// Location //----//

				if (location)
				{
					if (EnterLocation(location))
						return true;

					if (WaitLocation(location))
						return true;
				}

			}
			return false;
		}

		void SetScrubState()
		{
			state = State.Scrub;
		}

		void ScrubState()
		{
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			float nextPlayheadPos = currentTime;

			RaycastHit hit;
			if (canvasCollider.Raycast(mouseRay, out hit, 100f))
			{
				Debug.DrawLine(mouseRay.origin, hit.point, Color.red);
				Vector3 hitLocalPos = timeline.transform.InverseTransformPoint(hit.point);

				nextPlayheadPos = Mathf.Clamp(hitLocalPos.x, 0, timelineMax);
			}

			if (mouseDown == false)
			{
				//currentTime = Mathf.FloorToInt (nextPlayheadPos);

				currentTime = timeForNextDecision;

//				timeForNextDecision = currentTime;

				float diff = Mathf.Abs(currentTime - currentTimeInterpolated);
				float interpolateSpeed = diff > 1f ? 20f : 2f;
				SetScrubWaitState(interpolateSpeed);
								
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

//			int lastTime = currentPlayer.GetLastTime ();
//
//			while (currentTimeInterpolated - 1 > lastTime) 
//			{						
//				Action actionWait = new ActionWait (lastTime, 1);
//				AddAction (actionWait, false);
//				lastTime += actionWait.duration; 
//			}
		}

		void SetScrubWaitState(float interpolateSpeed)
		{
			scrubWaitInterpolateSpeed = interpolateSpeed;

			state = State.ScrubWait;				
		}

		void ScrubWaitState()
		{
			InterpolateTime(scrubWaitInterpolateSpeed);

//			int lastTime = currentPlayer.GetLastTime ();

//			while (currentTimeInterpolated > lastTime && currentTime > lastTime) 
//			{						
//				Action actionWait = new ActionWait (lastTime, 1);
//				AddAction (actionWait, false);
//			}

			if (timeSynced)
			{
				SetReadyState();
			}
		}

		void SetForwardState()
		{
			if (currentPlayer.GetAction(currentTime) == null)
			{
				Action actionWait = new ActionWait(currentTime, 1);
				AddAction(actionWait);
			}
			else
			{
				timeForNextDecision++;
				currentTime++;
			}

			forwardButton.Toggle(true);

			state = State.Forward;
		}

		void ForwardState()
		{			
			InterpolateTime();

			if (timeSynced)
			{
				forwardButton.Toggle(false);

				SetReadyState();
			}
		}

		void SetBackwardsState()
		{
			currentTime--;
			timeForNextDecision = currentTime;

//			backwardsButton.Toggle (true);

			state = State.Backwards;
		}

		void BackwardsState()
		{
			InterpolateTime();

			if (timeSynced)
			{
				currentPlayer.UndoActions(currentTime); //UndoActions ();

				Location location = currentPlayer.GetLocationAtTime(currentTime);
				currentPlayer.currentLocation = location;

				if (location == timeMachine)
				{
					SetTimeTravelReadyState();
				}
				else if (MouseDown(backwardsButton.collider) && currentTime > 0)
				{
					currentTime--;
					timeForNextDecision = currentTime;
				}
				else
				{
//					backwardsButton.Toggle (false);
					SetReadyState();
				}
			}
		}

		void SetActionState()
		{
			//Perform actions
			for (int i = 0; i < characters.Count; i++)
			{
				Character character = characters[i];

				Action action = character.GetAction(currentTime);

				if (action != null)
				{
					action.Perform(character, currentTime);
				}
			}

			//Progress Time
			currentTime++;

			Location currentLocation = currentPlayer.GetLocationAtTime(currentTime);

			//Current Observations
			for (int i = 0, count = characters.Count; i < count; i++)
			{
				characters[i].CreateCurrentObservation(currentTime, characters);
			}

			//Check Observations
			for (int i = 0, count = characters.Count; i < count; i++)
			{
				Character character = characters[i];

				if (character != currentPlayer)
				{
					character.CheckObservations(currentTime);

					//Check crossing characters
					if (currentLocation.isTimeMachine == false)
					{
						if (currentLocation == character.GetLocationAtTime(currentTime - 1))
						{ 
							if (currentPlayer.GetLocationAtTime(currentTime - 1) == character.GetLocationAtTime(currentTime))
							{
								Paradox paradox = new Paradox(character.visualsMeet, currentPlayer);
								character.currentParadoxes.Add(paradox);
							}
						}
					}
				}
			}
				
			//----// Paradox Check //----//
			bool paradoxFound = false;
			for (int i = 0, count = characters.Count; i < count; i++)
			{
				Character character = characters[i];
				if (character.currentParadoxes.Count > 0)
				{
					paradoxFound = true;
					break;
				}
			}

			if (paradoxFound)
			{
				SetParadoxState();
			}
			else
			{
				currentPlayer.AddCurrentObservation();
				state = State.Action;
			}
		}

		void ActionState()
		{
			InterpolateTime();

			if (timeSynced)
			{
				if (timeForNextDecision > currentTime)
				{
					SetActionState();
				}
				else
				{
					bool isTimeTravelling = TimeTravelCheck();

					if (isTimeTravelling)
					{
						TimeTravel();
					}
					else
					{
						SetReadyState();
					}
				}
			}
		}

		bool TimeTravelCheck()
		{
			Action action = currentPlayer.GetLastAction();

			if (action != null)
			{
				ActionEnter actionEnter = action as ActionEnter;

				if (actionEnter != null && actionEnter.toLocation == timeMachine)
				{
					return true;
				}
			}

			return false;
		}

		//		void SetMoveState()
		//		{
		//			state = State.Move;
		//		}
		//
		//		void MoveState()
		//		{
		//			InterpolateTime ();
		//
		//			if (timeSynced)
		//			{
		//				Action action = currentPlayer.GetLastAction();
		//
		//				if (action != null)
		//				{
		//					ActionEnter actionEnter = action as ActionEnter;
		//
		//					if (actionEnter != null && actionEnter.location == timeMachine)
		//					{
		//						TimeTravel ();
		//						return;
		//					}
		//				}
		//
		//				SetReadyState ();
		//			}
		//
		//		}

		void SetTimeTravelReadyState()
		{
			playhead.gameObject.SetActive(false);

			state = State.TimeTravelReady;
		}

		void TimeTravelReadyState()
		{
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

			//Destroy current iteration
			if (mousePress && MouseDown(backwardsButton.collider))
			{
				characters.Remove(currentPlayer);

				currentPlayer.Destroy();
				currentPlayer = characters[characters.Count - 1];
				int lastTime = currentPlayer.GetLastTime();

				currentTime = lastTime;
				timeForNextDecision = lastTime;
				currentTimeInterpolated = lastTime;
	
				playheadRenderer.material = currentPlayer.primaryMaterial;

				playhead.gameObject.SetActive(true);
				timelineBackground.sharedMaterial = timelineBackgroundMaterial; 

				SetBackwardsState();
				//SetWaitState ();

				return;
			}

			//Press Timeline
			if (mousePress && MouseDown(timelineCollider))
			{
				SetTimeTravelState();
			}
		}

		void SetTimeTravelState()
		{
			playhead.gameObject.SetActive(true);
			timelineBackground.sharedMaterial = whiteMaterial; 

			float nextPlayheadPos = currentTime;

			RaycastHit hit;
			if (canvasCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
			{
				Vector3 hitLocalPos = timeline.transform.InverseTransformPoint(hit.point);
				nextPlayheadPos = Mathf.Clamp(hitLocalPos.x, 0, timelineMax);
			}

			int nextTime = Mathf.FloorToInt(nextPlayheadPos);

			currentTime = nextTime;
			currentTimeInterpolated = nextPlayheadPos; //nextTime;

			state = State.TimeTravel;
		}

		void TimeTravelState()
		{
			float nextPlayheadPos = currentTime;

			RaycastHit hit;
			if (canvasCollider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f))
			{
				Vector3 hitLocalPos = timeline.transform.InverseTransformPoint(hit.point);
				nextPlayheadPos = Mathf.Clamp(hitLocalPos.x, 0, timelineMax);
			}

			int nextTime = Mathf.FloorToInt(nextPlayheadPos); 

			if (mouseDown == false)
			{
				currentTime = nextTime;
				timeForNextDecision = nextTime;
				currentTimeInterpolated = nextTime;

				if (EnterLocation(timeMachine.connectedLocations[0]))
				{
					timelineBackground.sharedMaterial = timelineBackgroundMaterial; 
					SetActionState();
				}
			}
			else
			{
				//currentTime = nextTime;
				currentTime = Mathf.FloorToInt(nextPlayheadPos);

				//currentTimeInterpolated = nextTime;
				currentTimeInterpolated = Mathf.MoveTowards(currentTimeInterpolated, nextPlayheadPos, 30f * Time.deltaTime);
			}
		}

		void SetParadoxState()
		{
			
			state = State.Paradox;
		}

		void ParadoxState()
		{
			InterpolateTime();

			backwardsButton.transform.localScale = Vector3.one * (1.5f + 0.3f * Mathf.Sin(Time.time * 5f));

			if (timeSynced)
			{
				//TODO this should be done differently 
				for (int i = 0, count = characters.Count; i < count; i++)
				{
					Character character = characters[i];

					if (character.currentParadoxes.Count > 0)
					{
						Paradox paradox = character.currentParadoxes[0];

						if (paradox != null)
						{					
							paradoxParticles.gameObject.SetActive(true);
							paradoxParticles.transform.position = paradox.character.transform.position;// character.transform.position;
						}
					}
				}
					
				//Backwards
				if (MouseDown(backwardsButton.collider))
				{
					for (int i = 0, count = characters.Count; i < count; i++)
					{
						characters[i].currentParadoxes.Clear();
					}

					backwardsButton.transform.localScale = Vector3.one;

					paradoxParticles.gameObject.SetActive(false);

					SetBackwardsState();
				}
			}
		}

		void Update()
		{
			UpdateMouseInput();

			bool backwardsButtonDown = MouseDown(backwardsButton.collider) || Input.GetKey(KeyCode.Z);
			backwardsButton.Toggle(backwardsButtonDown);

			timeSynced = currentTimeInterpolated == currentTime; 

			switch (state)
			{
				case State.Load:
					LoadState();
					break;
				case State.Ready:
					ReadyState();
					break;			
				case State.Scrub:
					ScrubState();
					break;	
				case State.ScrubWait:
					ScrubWaitState();
					break;
				case State.Forward:
					ForwardState();
					break;
				case State.Backwards:
					BackwardsState();
					break;			
				case State.Action:
					ActionState();
					break;
//			case State.Move:
//				MoveState ();
//				break;
				case State.TimeTravelReady:
					TimeTravelReadyState();
					break;
				case State.TimeTravel:
					TimeTravelState();
					break;
				case State.Paradox:
					ParadoxState();
					break;
			}

			//----// Updates //----//

			//Playhead
			Vector3 playheadPos = playhead.localPosition;
			playheadPos.x = currentTimeInterpolated;
			playhead.localPosition = playheadPos;

			//-----// Simulate Entire Timeline //-----// 

			//Reset
			for (int i = 0; i < characters.Count; i++)
			{
				characters[i].Reset();
			}

			for (int i = 0; i < allItems.Count; i++)
			{
				allItems[i].Reset();
			}

			int timeInstant = 0;

			//Simulate
			while (timeInstant < currentTimeInterpolated)
			{
				timeInstant++;

				//Perform actions
				for (int i = 0; i < characters.Count; i++)
				{
					Character character = characters[i];
					//Action action = character.GetAction (timeInstant);

					Action action = null; 
					for (int k = 0, count = character.history.Count; k < count; k++)
					{
						Action a = character.history[k];
						if (timeInstant > a.time)
						{
							action = a;
						}
					}

					if (action != null)
					{
						action.Perform(character, currentTime);
					}
				}

			}	

			//-----// Show Current State //-----//
			//Items
			for (int i = 0, count = allItems.Count; i < count; i++)
			{
				Item item = allItems[i];
				item.UpdateItem();
			}

			//Characters
			for (int i = 0, count = characters.Count; i < count; i++)
			{
				Character character = characters[i];
				character.UpdateCharacter(currentTimeInterpolated);
//				character.UpdateCharacter (currentTime, currentTimeInterpolated, timeForNextDecision, character == currentPlayer, false);
			}

			//Location Covers
			for (int i = 0, length = currentLevel.locations.Length; i < length; i++)
			{
				Location location = currentLevel.locations[i];
				if (location.cover)
				{
					location.cover.SetActive(true);

					for (int j = 0, count = characters.Count; j < count; j++)
					{
						Character character = characters[j];
						if (character.GetLocationAtTime(Mathf.RoundToInt(currentTimeInterpolated)) == location)
						{
							location.cover.SetActive(false);
							break;
						}
					}
				}
			}

			//Reload scene on Escape
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		}

		void UpdateMouseInput()
		{
			mousePreviousDown = mouseDown;

			mouseDown = Input.GetMouseButton(0);
			mousePress = mouseDown && !mousePreviousDown;
			mouseRelease = !mouseDown && mousePreviousDown;

			if (Input.GetMouseButton(0))
			{				
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				mouseHits = Physics.RaycastAll(mouseRay, 100);
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
				Collider hitCollider = mouseHits[i].collider;
			
				if (collider == hitCollider)
					return true;
			}

			return false;
		}

		public T MouseDown<T>() where T : class
		{
			for (int i = 0, length = mouseHits.Length; i < length; i++)
			{
				Collider hitCollider = mouseHits[i].collider;
				Transform hitTransform = hitCollider.gameObject.transform;

				if (hitTransform.parent)
				{
					T hitComponent = hitTransform.parent.GetComponent<T>();

					if (hitComponent != null)
						return hitComponent;
				}
			}

			return null;
		}

		void InterpolateTime(float speed = 2f)
		{
			float diff = currentTime - currentTimeInterpolated;
			float addTime = Mathf.Sign(diff) * speed * Time.deltaTime; 

			if (Mathf.Abs(addTime) < Mathf.Abs(diff))
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

			CreatePlayer(timeMachine);

//			currentTime = 0;
//			currentTimeInterpolated = 0;
//			timeForNextDecision = 0;

			SetTimeTravelReadyState();
		}

		public bool EnterLocation(Location location)
		{
			Location currentLocation = currentPlayer.GetLocationAtTime(currentTime);

			if (currentLocation == timeMachine && location != timeMachine)
			{	
				Action actionEnter = new ActionEnter(currentTime, 1, timeMachine, timeMachine.connectedLocations[0]);
				AddAction(actionEnter);

				return true;
			}
			else if (location != currentLocation)
			{
				for (int i = 0, length = currentLocation.connectedLocations.Length; i < length; i++)
				{
					if (currentLocation.connectedLocations[i] == location)
					{					
						Action actionEnter = new ActionEnter(currentTime, 1, currentPlayer.currentLocation, location);
						AddAction(actionEnter);

						return true;
					}
				}
			}

			return false;
		}

		public bool WaitLocation(Location location)
		{
			Location currentLocation = currentPlayer.GetLocationAtTime(currentTime);

			if (location == currentLocation && currentTime < timelineMax)
			{
				Action actionWait = new ActionWait(currentTime, 1);
				AddAction(actionWait);

				return true;
			}	

			return false;
		}

		public void AddAction(Action action, bool increaseCurrentTime = true)
		{
			//currentPlayer.UndoActions (currentTime); //UndoActions ();

			//Add symbols
//			if (action is ActionEnter) 
//			{
////				currentPlayer.timeLine.AddSymbol (symbolEnterPrefab, currentPlayer.primaryMaterial, currentTime);
//			}

			currentPlayer.history.Add(action);

			timeForNextDecision += action.duration;

//			if (increaseCurrentTime) 
//			{
//				currentTime += action.duration;
//			}
		}

		//		void UndoActions()
		//		{
		//			currentPlayer.UndoActions (currentTime);
		//
		//			if (currentPlayer.history.Count > 0)
		//			{
		//				for (int i = currentPlayer.history.Count - 1; i >= 0; i--)
		//				{
		//					Action action = currentPlayer.history[i];
		//
		//					if (action.time >= currentTime)
		//					{
		//						currentPlayer.history.Remove (action);
		//						currentPlayer.timeLine.RemoveSymbols (currentTime);
		//					}
		//				}
		//			}
		//		}

		private void CreatePlayer(Location location)
		{
			GameObject newPlayer = Instantiate(characterPrefab, location.transform.position, Quaternion.LookRotation(Vector3.back));
			currentPlayer = newPlayer.GetComponent<Character>();

			newPlayer.name = "Player " + characters.Count;

			GameObject newTimelineTrack = Instantiate(characterTimelinePrefab, timeline.transform.position, timeline.transform.rotation);
			newTimelineTrack.transform.parent = timeline.transform;

			Vector3 trackPosLocal = Vector3.zero;
			trackPosLocal.y = characters.Count * -0.75f;
			newTimelineTrack.transform.localPosition = trackPosLocal;

			Material material = playerMaterials[characters.Count % playerMaterials.Length];
			currentPlayer.Setup(location, material, newTimelineTrack.transform);
			playheadRenderer.material = material;

			characters.Add(currentPlayer);		
		}
	}
}
