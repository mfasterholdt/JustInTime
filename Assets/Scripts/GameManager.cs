 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour 
{
	public Level initialLevel;	
	private Level[] levels;

	public Location timeMachine;

	[HideInInspector]
	public Level currentLevel;

	[HideInInspector]
	public int currentTime;

	//TODO, mff consider a paradox list to see everything that went wrong instead of just the first instance
	//Might be funny to see the full blurp of madness
	public static Paradox paradox;
	public static string levelCompleteText;
	public static bool levelComplete;

	//TODO, mff consider adding gamestate enum to handle play, (instantProgress), paradox, levelComplete, timeTravel
//	public enum GameState { Loading, Wait, Decision, TimeTravel, Lose, Win }
//	public GameState gameState = GameState.Loading;

	[HideInInspector]
	public bool timeTravelNow;

	private int timeForNextDecision;

	[HideInInspector]
	public Character currentPlayer;

	[HideInInspector]
	public List<Character> characters = new List<Character>();

	private float fieldWidth = 230;
	private float fieldWidthWorld = 130;
	private float fieldHeight = 22;
	private Vector2 posGUI;

	private bool levelLoaded;
	private bool stylesLoaded;

	public static GameManager Instance;
	private bool instantProgresss;

	//TODO, mff maybe the paradox things should be generalised to all halt/lose conditions, perhaps together with game state
	//TODO, mff find a nice way to distinquish distinguish between fail and paradox
	[HideInInspector]
	public bool paradoxFound;

	private GUIStyle styleHistoryEntry;
	private GUIStyle styleParadox;
	private float tickTimer;
	private const float tickDelay = 0.35f;
	private const int ticksPerHour = 24;

	private int nextCharacterID;

	public ItemTimeline itemTimeline = new ItemTimeline();
//	private ItemLocator itemLocator = new ItemLocator();

	public GameManager()
	{
		GameManager.Instance = this;
	}

	public int GetNextCharacterID()
	{
		return nextCharacterID++;
	}

	void Start()
	{
		levels = GetComponentsInChildren<Level>(false);

		itemTimeline.characters = characters;

		for (int i = 0, length = levels.Length; i < length; i++) 
			levels[i].Setup();	

		if(initialLevel)
			LoadLevel(initialLevel);
		else
			LoadLevel(levels[0]);
	}
		
	void LoadLevel(Level level)
	{
//		gameState == GameState.Loading;
		levelLoaded = false;

		currentTime = 0;
		timeForNextDecision = 0;
		currentLevel = level;

		timeMachine.connectedLocations = currentLevel.GetTimeMachineConnections(timeMachine);
		characters.Clear();

		currentLevel.Reset();
		currentLevel.Load();

		List<Character> initialCharacters = currentLevel.GetInitialCharacters();
		for (int i = 0, count = initialCharacters.Count; i < count; i++) 
		{
			characters.Add(initialCharacters[i]);	
		}

		currentPlayer = new Character(currentLevel.startLocation, currentLevel.GetInitialInventory(), GetNextCharacterID(), 1, "Frankie");
		characters.Add(currentPlayer);	

		//Item time line load and repopulate
		itemTimeline.Load();

		itemTimeline.PassItems(currentLevel.startLocation.items);

		for (int i = 1, count = characters.Count; i < count; i++) 
		{
			Character character = characters[i];
			itemTimeline.PassItems(character.inventory);
		}	

		instantProgresss = true;
		levelLoaded = true;
		levelComplete = false;
		paradoxFound = false;

		Progress();
	}

	void Update()
	{			
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			LoadLevel(currentLevel);
			return;
//			SceneManager.LoadScene( SceneManager.GetActiveScene().name );
		}	

		if(Input.GetKeyDown(KeyCode.Z))
		{
			UndoLastAction();
		}

		if(Input.GetKeyDown(KeyCode.R))
		{
			Ripple();
		}

		for ( int i = 1, length = levels.Length; i <= length; i++ )
		{
			if ( Input.GetKeyDown( i.ToString() ) )
			{
				LoadLevel(levels[i-1]);	
				return;
			}
		}

		if(paradoxFound || levelComplete)
			return;
		
		if(currentTime == timeForNextDecision && Input.GetKeyDown(KeyCode.Space))
		{			
			ActionWait actionWait = new ActionWait(currentTime, 1, currentPlayer.GetCurrentLocation() );
			AddAction(actionWait);			 
		}

		Progress();
	}

	void Progress()
	{
		if(instantProgresss)
		{
			//Instant progress

			while(timeForNextDecision > currentTime )
			{
				Tick();
			}

			instantProgresss = false;
		}
		else
		{
			//Normal progress
			if(timeForNextDecision > currentTime )
			{
				if(tickTimer > 0)
				{
					tickTimer -= Time.deltaTime;
				}
				else
				{				
					Tick();	

					if(timeForNextDecision > currentTime && !paradoxFound)
						tickTimer = tickDelay;
				}	
			}
		}
	}

	void UndoLastAction()
	{				
		//Wrapping to previous iteration
		if(currentPlayer.history.Count == 0)
		{
			int currentIteration = currentPlayer.GetIteration();

			if(currentIteration > 1)
			{
				characters.Remove(currentPlayer);
				currentPlayer.SetCurrentLocation(null);

				for (int i = 0, count = characters.Count; i < count; i++) 
				{
					Character character = characters[i];	
					if(currentPlayer.CheckIdentity(character) && character.GetIteration() == currentIteration - 1)
					{
						currentPlayer = character;
						break;
					}					
				}
			}
			else
			{
				return;
			}
		}

		int lastIndex = currentPlayer.history.Count - 1;

		if(lastIndex >= 0)
		{
			//Remove latest player action
//			Action latestAction = currentPlayer.history[lastIndex];
//			int decisionTime = latestAction.time;
//			timeForNextDecision = decisionTime;
//			currentPlayer.history.RemoveAt(lastIndex);

			//Remove action and observation from player
			int decisionTime = currentPlayer.RemoveAction();
			timeForNextDecision = decisionTime;

			for (int j = currentPlayer.observations.Count - 1; j >= 0; j--) 
			{
				if(decisionTime <= currentPlayer.observations[j].time)
					currentPlayer.observations.RemoveAt(currentPlayer.observations.Count - 1);						
			}

			//Find highest player time
			int highestTime = decisionTime;

			for (int i = 0, characterCount = characters.Count; i < characterCount; i++) 
			{
				Character character = characters[i];

				if(character.CheckIdentity(currentPlayer))
				{
					int historyCount = character.history.Count;

					if(historyCount > 0)
					{
						Action action = character.history[historyCount - 1];

						int characterHighestTime = action.time + action.duration;

						if(characterHighestTime > highestTime)
							highestTime = characterHighestTime;
					}
				}
			}

			for (int i = 0, characterCount = characters.Count; i < characterCount; i++) 
			{
				Character character = characters[i];	

				if(!character.CheckIdentity(currentPlayer))
				{
					//Remove npc actions
					for (int j = character.history.Count - 1; j >= 0; j--) 
					{
						Action action = character.history[j];

						if(highestTime <= action.time)
							character.RemoveAction();					
					}

					//Remove npc observations
					for (int j = character.observations.Count - 1; j >= 0; j--) 
					{
						Observation observation = character.observations[j];

						if(highestTime < observation.time)
							character.observations.RemoveAt(character.observations.Count - 1);						
					}
				}
			}

			//Reset level
			currentLevel.Reset();

			for (int i = 0, count = characters.Count; i < count; i++) 
				characters[i].Reset();	
			
			itemTimeline.itemInstances.Clear();
			currentPlayer.timeline.Clear();
//			itemTimeline.timeline.Clear();

			itemTimeline.PassItems(currentLevel.startLocation.items);

			for (int i = 1, count = characters.Count; i < count; i++) 
			{
				Character character = characters[i];
				itemTimeline.PassItems(character.inventory);
			}	

			currentTime = 0;
			instantProgresss = true;
			paradoxFound = false;
			levelComplete = false;

//			watchList.Clear();
		}
	}

	void Tick()
	{		
		if(paradoxFound)
			return;
		
		//----// Character ticks //----// NPC actions for this tick added
		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			characters[i].Tick(currentTime);
		}

		//----// Perform and Verify actions //----//
		if(PerformActions() == false)
		{
			paradoxFound = true;
			return;
		}

		if(VerifyActions() == false)
		{
			paradoxFound = true;
			return;
		}

		//Time moves
		currentTime++;

		//Level Tick, Oven baking, Fire growing etc.
		if(currentLevel.Tick(currentTime, currentPlayer) == false)
		{
			paradoxFound = true;
			return;
		}

		//Check previous observations
		if(CheckObservations() == false)
		{
			paradoxFound = true;
			return;
		}

		//Make new observations
		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			Character character = characters[i];
			Location currentLocation = character.GetCurrentLocation();
			if(currentLocation != null && currentLocation != timeMachine)
			{
				character.MakeObservations(currentTime, currentLevel);
			}
		}					 

		//Add item timeline entries
		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			Character character = characters[i];
			Action currentAction = character.GetAction(currentTime - 1);

			if(character.GetCurrentLocation() != timeMachine || (currentAction != null && currentAction.GetType() == typeof(ActionEnter)))
			{
				for (int j = 0, itemCount = character.inventory.Count; j < itemCount; j++)
				{					
					Item item = character.inventory[j];
					if(item != null && item.useTimeline)
					{
						item.age++;
						itemTimeline.AddEntry(currentTime, item, currentPlayer, item.age, j, -1, character.GetCurrentLocation());
						//						watchList.Add(watch.age);
					}
				}
			}
		}

		Location[] locations = currentLevel.GetLocations();
		for (int i = 0, length = locations.Length; i < length; i++) 
		{
			Location location = locations[i];
			for (int j = 0, itemCount = location.items.Count; j < itemCount; j++) 
			{
				Item item = location.items[j];
				if(item != null && item.useTimeline)
				{
					item.age++;
					itemTimeline.AddEntry(currentTime, item, currentPlayer, item.age, -1, j, location);
					//					watchList.Add(watch.age);

				}
			}
		}


		//Time travel

		for (int i = 0, characterCount = characters.Count; i < characterCount; i++) 
		{
			Character character = characters[i];

			if(character != currentPlayer)
			{
				Action action = character.GetAction(currentTime-1);
				if(action != null)
				{
					ActionEnter actionEnter = action as ActionEnter;
					if(actionEnter != null && actionEnter.targetLocation == timeMachine)
					{
//						Debug.Log("Reached time tRavel of not currentPlayer");	
//						character.SetCurrentLocation(null);

						//TODO, ripple removed for now
						//Character nextCharacter = characters[i+1];
						//nextCharacter.initialInventory = Utils.CopyItemList(character.inventory);
					}
				}
			}
		}	

		{
			//ActionEnter actionEnter = currentPlayer.history[currentPlayer.history.Count-1] as ActionEnter;
			ActionEnter actionEnter = currentPlayer.GetAction(currentTime-1) as ActionEnter;
			if(actionEnter != null && actionEnter.targetLocation == timeMachine)
			{
				TimeTravel();		
			}
		}
	}

	private bool CheckObservations()
	{
		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			if(characters[i].CheckObservation(currentTime) == false)
			{
				return false;
			}
		}

		return true;
	}

	bool PerformActions()
	{
		for (int i = 0; i < characters.Count; i++) 
		{
			Character player = characters[i];

			for (int s = 0; s < player.history.Count; s++) 
			{
				Action action = player.history[s];
				int startTime = action.time;
				int performTime = startTime + action.duration - 1;

				if(currentTime < startTime)
				{
					continue;
				} 
				else if(currentTime == performTime)
				{
					if(action.Perform(player) == false)
						return false;

					break;
				}
				else if(currentTime < performTime)
				{
					if(action.Prepare(player) == false)
						return false;

					break;
				}
				else if(startTime > currentTime)
				{
					break;
				}
			}
		}

		return true;
	}

	bool VerifyActions()
	{
		for (int i = 0; i < characters.Count; i++) 
		{
			Character player = characters[i];

			for (int s = 0; s < player.history.Count; s++) 
			{
				Action action = player.history[s];
				int startTime = action.time;
				int performTime = startTime + action.duration - 1;

				if(currentTime < startTime)
				{
					continue;
				} 
				else if(currentTime == performTime)
				{
					if(action.VerifyPerform(player) == false)
						return false;
					
					break;
				}
				else if(currentTime < performTime)
				{
					if(action.VerifyPrepare(player) == false)
						return false;
					
					break;
				}
				else if(startTime > currentTime)
				{
					break;
				}
			}
		}

		return true;
	}

	public void Ripple()
	{
		itemTimeline.Ripple(currentTime);
		//TODO, will return to ripples later
		//Reset level
//		currentLevel.Reset();
//
//		for (int j = 0, count = characters.Count; j < count; j++) 
//			characters[j].Reset();	
//
//		currentTime = 0;
//		instantProgresss = true;
//		paradoxFound = false;
//		levelComplete = false;
//
//		watchList.Clear();	
	}

	public int GetMaximumTime()
	{
		int maximumTime = 0;

		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			Character player = characters[i];
			Action lastAction = player.history[player.history.Count - 1];
			maximumTime = Mathf.Max(maximumTime, lastAction.time + lastAction.duration);
		}	

		return maximumTime;
	}

	public void AddAction(Action action)
	{
		currentPlayer.history.Add(action);
		timeForNextDecision += action.duration;
	}

	public void TimeTravel()
	{
		itemTimeline.TimeTravel(currentTime);

		currentLevel.Reset();

		Character newPlayer = new Character(timeMachine, currentPlayer.inventory, currentPlayer.GetID(), currentPlayer.GetIteration() + 1, currentPlayer.GetName(false));

		for (int i = 0, count = characters.Count; i < count; i++) 
			characters[i].Reset();	

		characters.Add(newPlayer);
		currentPlayer = newPlayer;

		currentTime = 0;
		timeForNextDecision = 0;

		itemTimeline.itemInstances.Clear();

		itemTimeline.PassItems(currentLevel.startLocation.items);

		for (int i = 1, count = characters.Count; i < count; i++) 
		{
			Character character = characters[i];
			itemTimeline.PassItems(character.inventory);
		}	

		timeTravelNow = false;
	}

	void OnGUI()
	{	
		if(!levelLoaded)
			return;

		if(!stylesLoaded)
			LoadStyles();

		//Intro Header
	 	posGUI = new Vector2(30, 30);

		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Level : "+currentLevel.name);
		posGUI.y += fieldHeight;

		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Time : "+currentTime);
		posGUI.y += fieldHeight;

		Location currentLocation = currentPlayer.GetCurrentLocation();
		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Location : "+ currentLocation.name);		
		posGUI.y += fieldHeight;
		posGUI.y += 20;

		GUI.enabled = currentTime == timeForNextDecision && !paradoxFound && !levelComplete;
		DrawLocation(currentLocation);

		posGUI.y = 220;
		DrawActions();

		posGUI.y = 400;
		DrawInventory(currentLocation);

		GUI.enabled = true;

		posGUI = new Vector2(300, 20);
		DrawHistory();

		posGUI = new Vector2(1500, 20);
		posGUI.x = Screen.width - fieldWidthWorld - 50;

		DrawWorld();

		DrawParadox();

		DrawLevelComplete();

		//Gather clock age
//		GUI.contentColor = Color.red;

		posGUI = new Vector3(Screen.width - 400, 20);
		posGUI.y += fieldHeight;

//		watchList.Sort();
//
//		for (int i = 0, watchCount = watchList.Count; i < watchCount; i++) 
//		{
//			string watch = watchList[i].ToString();
//
//			GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), watch);
//			posGUI.y += fieldHeight;
//		}
//
//		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Entries : "+watchList.Count);
//		posGUI.y += fieldHeight;

		itemTimeline.Draw(currentTime, posGUI);
		//itemLocator.Draw(posGUI);

//		GUI.contentColor = Color.white;
	}

//	public List<int> watchList = new List<int>();

	void LoadStyles()
	{		
		Texture2D backgroundTexture = Texture2D.blackTexture;

		Color[] pixels = backgroundTexture.GetPixels();
		Color c = Color.black;
		c.a = 0.2f;
		for (int i = 0, length = pixels.Length; i < length; i++) 
			pixels[i] = c;

		backgroundTexture.SetPixels(pixels);
		backgroundTexture.Apply();

		styleHistoryEntry = new GUIStyle(GUI.skin.label);
		styleHistoryEntry.normal.background = backgroundTexture;
		styleHistoryEntry.fontSize = 11;
		styleHistoryEntry.alignment = TextAnchor.MiddleLeft;

		styleParadox = new GUIStyle(GUI.skin.label);
		styleParadox.normal.background = backgroundTexture;
		styleParadox.alignment = TextAnchor.MiddleCenter;

		stylesLoaded = true;
	}

	void DrawLocation(Location currentLocation)
	{	
		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Enter : ");
		posGUI.y += fieldHeight;

		for (int i = 0; i < currentLocation.connectedLocations.Count; i++) 
		{
			Location connectedLocation = currentLocation.connectedLocations[i];

			if(GUI.Button(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), connectedLocation.name))
			{			
				ActionEnter actionEnter = new ActionEnter(currentTime, connectedLocation);
				AddAction(actionEnter);
			}

			posGUI.y += fieldHeight;
		}
	
		posGUI.y += fieldHeight;
	}


	void DrawActions()
	{
		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Actions : ");
		posGUI.y += fieldHeight;

		Location currentLocation = currentPlayer.GetCurrentLocation();

		if(GUI.Button(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Wait"))
		{			
			ActionWait actionWait = new ActionWait(currentTime, 1, currentLocation);
			AddAction(actionWait);
		}

		posGUI.y += fieldHeight;


		if(currentLocation != timeMachine)
		{
			for (int j = 0; j < currentLocation.items.Count; j++) 
			{
				Item item = currentLocation.items[j];
				if(item.pickup)
				{
					if(GUI.Button(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Pickup "+item.ToShortString()))
					{				
						ActionPickup actionPickup = new ActionPickup(currentTime, item, j);
						AddAction(actionPickup);
					}

					posGUI.y += fieldHeight;
				}
			}

			//Inventory actions
			for (int i = 0, count = currentPlayer.inventory.Count; i < count; i++) 
			{
				Item item = currentPlayer.inventory[i];
				Action[] itemActions = item.GetInventoryActions(currentTime, i, currentPlayer);

				if(itemActions != null)
				{
					for (int j = 0, length = itemActions.Length; j < length; j++) 
					{
						Action action = itemActions[j];
						if(GUI.Button(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), action.name))
						{				
							AddAction(action);
						}
						posGUI.y += fieldHeight;
					}
				}
			}

			//Location items actions 
			for (int i = 0, count = currentLocation.items.Count; i < count; i++) 
			{
				Item item = currentLocation.items[i];
				Action[] itemActions = item.GetLocationActions(currentTime, i, currentPlayer);

				if(itemActions != null)
				{					
					for (int j = 0, length = itemActions.Length; j < length; j++) 
					{
						Action action = itemActions[j];
						if(GUI.Button(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), action.name))
						{				
							AddAction(action);
						}
						posGUI.y += fieldHeight;
					}
				}
			}
		}

		Action[] levelActions = currentLevel.GetLevelActions(currentTime, currentPlayer);
		for (int j = 0, length = levelActions.Length; j < length; j++) 
		{
			Action action = levelActions[j];
			if(GUI.Button(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), action.name))
			{				
				AddAction(action);
			}

			posGUI.y += fieldHeight;
		}
	}

	void DrawInventory(Location currentLocation)
	{
		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "Items : ");
		posGUI.y += fieldHeight;

		List<Item> inventory = currentPlayer.inventory;

		for (int i = 0; i < inventory.Count; i++) 
		{
			Item item = inventory[i];
			GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), item.ToShortString());

			if(currentLocation != timeMachine)
			{
				if(GUI.Button(new Rect(posGUI.x + 140, posGUI.y, 60, fieldHeight), "Drop"))
				{
					ActionDrop actionDrop = new ActionDrop(currentTime, item, i);
					AddAction(actionDrop);
				}

				if(item.useTimeline)
				{
					if(GUI.Button(new Rect(posGUI.x + 80, posGUI.y, 60, fieldHeight), "Age"))
					{
						ActionCheckAge actionCheckAge = new ActionCheckAge(currentTime, item, i);
						AddAction(actionCheckAge);
					}
				}
			}

			posGUI.y += fieldHeight;
		}
	}

	void DrawWorld()
	{		
		Location[] locations = currentLevel.GetLocations();

		for (int i = 0, length = locations.Length; i < length; i++) 
		{
			DrawWorldLocation(locations[i]);
		}

		DrawWorldLocation(timeMachine);
	}

	void DrawWorldLocation(Location location)
	{
		Vector2 pos = posGUI;
		pos.x -= location.placement.x * fieldWidthWorld; 
		pos.y += location.placement.y * fieldHeight;

		GUI.Label(new Rect(pos.x, pos.y, fieldWidth, fieldHeight), location.name);
		pos.y += fieldHeight;
		pos.x += 15;

		for (int s = 0, count = location.items.Count; s < count; s++) 
		{
			Item item = location.items[s];
			GUI.Label(new Rect(pos.x, pos.y, fieldWidth, fieldHeight), item.ToString());
			pos.y += fieldHeight;
		}

		for (int s = 0, count = location.characters.Count; s < count; s++) 
		{
			Character player = location.characters[s];

			GUI.Label(new Rect(pos.x, pos.y, fieldWidth, fieldHeight), player.GetName());
			pos.y += fieldHeight;

			for (int t = 0; t < player.inventory.Count; t++) 
			{
				GUI.Label(new Rect(pos.x + 15, pos.y, fieldWidth, fieldHeight), player.inventory[t].ToString());
				pos.y += fieldHeight;
			}
		}
	}

	void DrawHistory()
	{	
		GUI.Label(new Rect(posGUI.x, posGUI.y, fieldWidth, fieldHeight), "History : ");
		posGUI.y += fieldHeight;

		float historyEntryWidth = 200;

		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);

		for (int i = 0, count = characters.Count; i < count; i++) 
		{
			Character character = characters[i];

			for (int j = 0, historyCount = character.history.Count; j < historyCount; j++) 
			{				
				Action action = character.history[j];

				bool entryHasPassed = action.time + action.duration - 1 < currentTime;

				if(paradoxFound && action == GameManager.paradox.action)
					GUI.contentColor = Color.yellow;
				else if(action.ParadoxCheck(currentTime) == false)
					GUI.contentColor = Color.magenta;
				else if(entryHasPassed)
					GUI.contentColor = Color.green;
				else if(action.time < currentTime)
					GUI.contentColor = Color.cyan;
				else
					GUI.contentColor = Color.red;
				
				float entryHeight = action.duration * fieldHeight;
				entryHeight -= 4;

				string actionText = action.ToString();

				float posY = posGUI.y + action.time * fieldHeight;
				GUI.Label(new Rect(posGUI.x + 20 + i * 205, posY, historyEntryWidth, entryHeight), actionText, styleHistoryEntry);
			}

			//Observations
			GUI.contentColor = Color.blue;
			for (int j = 0, observationCount = character.observations.Count; j < observationCount; j++) 
			{
				Observation observation = character.observations[j];

				float obsPosX =  posGUI.x - 20 + (i + 1)* 205;
				float obsPosY = posGUI.y + (observation.time - 1) * fieldHeight - 1;

				Rect obsRect = new Rect(obsPosX, obsPosY, 30, fieldHeight);

				if(obsRect.Contains(mousePos))
				{
					GUI.contentColor = Color.white;

					float obsGUIX = Screen.width - 200;
					float obsWidth = 250;

					for (int s = 0; s < observationCount; s++) 
					{
						Observation showObservation = character.observations[s];
						if(showObservation.time == observation.time)
						{
							float obsGUIY = Screen.height - 300;
							GUI.Label(new Rect(obsGUIX, obsGUIY, obsWidth, fieldHeight), "// "+showObservation.location.name + (showObservation.checkUnexpected ? "": " [ Addivtive ]"), styleHistoryEntry);							
							
							obsGUIY += fieldHeight;

							int characterCount = showObservation.characters.Count;
							if(characterCount > 0)
							{
								GUI.Label(new Rect(obsGUIX, obsGUIY, obsWidth, fieldHeight), "Characters", styleHistoryEntry);
								obsGUIY += fieldHeight;
								for (int k = 0; k < characterCount; k++) 
								{
									GUI.Label(new Rect(obsGUIX, obsGUIY, obsWidth, fieldHeight), "\t"+showObservation.characters[k].GetName(), styleHistoryEntry);
									obsGUIY += fieldHeight;
								}
							}

							int itemCount = showObservation.items.Count;
							if(itemCount > 0)
							{
								GUI.Label(new Rect(obsGUIX, obsGUIY, obsWidth, fieldHeight), "Items", styleHistoryEntry);
								obsGUIY += fieldHeight;
								for (int k = 0; k < itemCount; k++) 
								{
									GUI.Label(new Rect(obsGUIX, obsGUIY, obsWidth, fieldHeight), "\t"+showObservation.items[k].ToString(), styleHistoryEntry);
									obsGUIY += fieldHeight;
								}
							}

							obsGUIX -= obsWidth + 10;
						}
					}			
				}					
				else
				{
					GUI.contentColor = Color.blue;
				}
				
				GUI.Label(obsRect, "Obs");
			}
		}

		for (int i = 0; i <= 40; i++) 
		{
			GUI.contentColor = currentTime > i ? Color.green : Color.red;
			GUI.Label(new Rect(posGUI.x, posGUI.y + i * fieldHeight - 1, fieldWidth, fieldHeight), i.ToString());
		}

		GUI.contentColor = Color.white;
	}

	void DrawParadox()
	{
		if(paradoxFound)
		{
			float width = 400;
			float height = 200;
			float x = (Screen.width - width) / 1.3f;
			float y = (Screen.height - height) / 1.3f;
			GUI.Label(new Rect(x, y, width, height), GameManager.paradox.message, styleParadox);

			x += (width - (width * 0.5f)) / 2f;

			if(GUI.Button(new Rect(x, y + height - (fieldHeight * 2f), width * 0.5f, fieldHeight), "Undo last action"))
			{			
				UndoLastAction();
			}
		}
	}

	void DrawLevelComplete()
	{
		if(levelComplete)
		{
			float width = 400;
			float height = 200;
			float x = (Screen.width - width) / 1.3f;
			float y = (Screen.height - height) / 1.3f;

			GUI.Label(new Rect(x, y, width, height), GameManager.levelCompleteText, styleParadox);		
		}
	}
}
