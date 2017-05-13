using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Character : MonoBehaviour
	{
		public new MeshRenderer renderer;
		public GameObject visualsDefault;
		public GameObject visualsWait;

		[HideInInspector]
		public Location initialLocation;

		[HideInInspector]
		public List<Action> history = new List<Action>();

		public Transform timeLineTrack;

		[HideInInspector]
		public CharacterTimeline timeLine;

		[HideInInspector]
		public Material primaryMaterial;

		public void Setup(Location initialLocation, Material material, Transform timeLineTrack)
		{
			this.initialLocation = initialLocation;
			renderer.material = material;
			this.timeLineTrack = timeLineTrack;

			primaryMaterial = material;

			CharacterTimeline timeLine = timeLineTrack.GetComponent<CharacterTimeline> ();
			timeLine.Setup(material);
			this.timeLine = timeLine;

//			Reset ();
		}

		public void Destroy()
		{
			Destroy (timeLineTrack.gameObject);	
			Destroy (this.gameObject);
		}
//
//		public void Reset()
//		{
//
//		}
//
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
					Action action = history [history.Count - 1];
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
				return history [history.Count - 1];
			}

			return null;
		}

		public int GetLastTime()
		{
			if (history.Count > 0) 
			{
				Action action = history [history.Count - 1];
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
					ActionEnter actionEnter = history [i] as ActionEnter;

					if (actionEnter != null && time > actionEnter.time) 
					{
						result = actionEnter.location;
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
					ActionEnter actionEnter = history [i] as ActionEnter;

					if (actionEnter != null) 
					{							
						return actionEnter.time;
					}
				}
			}
			else if (history.Count > 0) 
			{
				return history [0].time;
			}

			return 0;
		}

		public float GetTrackEnd(int currentTime, float currentTimeInterpolated, int timeForNextDecision, bool currentPlayer, bool draggingPlayhead)
		{
			float result = 0f;
			bool foundAction = false;

			if (history.Count > 0) 
			{
				for (int i = history.Count - 1; i >= 0; i--) 
				{
					Action action = history[i];

					Location location = GetLocationAtTime (action.time + action.duration);
					ActionEnter actionEnter = action as ActionEnter;

					if (location.isTimeMachine == false || (actionEnter != null && actionEnter.location.isTimeMachine)) 
					{
						result = action.time + action.duration;
						foundAction = true;
						break;
					}
				}
			}

			return result;

			if (currentPlayer)
			{
				if(draggingPlayhead) 
				{
					if(currentTimeInterpolated > result)
						result = currentTimeInterpolated;
				}
				else
				{	
					if (timeForNextDecision > currentTimeInterpolated) 
					{	
						if (currentTime < timeForNextDecision) 
						{
//							if (currentTimeInterpolated < currentTime) 
//							{
//								result = timeForNextDecision;		
//							}
							//if(currentTime - 1 
						}
						else 
						{
							result = currentTimeInterpolated;
						}

//						if (foundAction)
//						{
//							
//							Debug.Log (foundAction);
//						}
//						else 
//						{
//							result = currentTimeInterpolated;
//						}
//						if (currentTime < timeForNextDecision) 
//						{
//							result = currentTimeInterpolated;
//						}
//						else
//						{
//							result = timeForNextDecision;
//
//						}
//						else 
//						{
//							result = currentTimeInterpolated;
//						}
					}
					else if (currentTimeInterpolated > currentTime) 
					{
						result = currentTimeInterpolated;
					}
				}
			}

			return result;
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

						for (int i = history.Count - 1; i >= 0; i--) {
							Action action = history [i];

							Location location = GetLocationAtTime (action.time);

							if (location.isTimeMachine == false) {
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

							for (int i = history.Count - 1; i >= 0; i--) {
								Action action = history [i];

								Location location = GetLocationAtTime (action.time);

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

					for (int i = history.Count - 1; i >= 0; i--) {
						Action action = history [i];

						Location location = GetLocationAtTime (action.time);

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

						Location location = GetLocationAtTime (action.time);
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

		public void UpdateCharacter(int currentTime, float currentTimeInterpolated, int timeForNextDecision, bool currentPlayer, bool draggingPlayhead)
		{
			Vector3 pos = initialLocation.transform.position;

			Vector3 forward = Vector3.back;

			if (history.Count > 0) 
			{
				//Movement
				Location fromLocation = initialLocation;

				for (int i = 0, count = history.Count; i < count; i++) 
				{
					Action action = history [i];
					ActionEnter actionEnter = history [i] as ActionEnter;

					if (currentTimeInterpolated >= action.time + 1) 
					{	
						if (actionEnter != null) 
						{
							fromLocation = actionEnter.location;
							pos = fromLocation.transform.position;
						}
					}
					else 
					{						
						if (actionEnter != null) 
						{
							float diff = currentTimeInterpolated - actionEnter.time;
							Vector3 fromPos = fromLocation.transform.position;
							Vector3 toPos = actionEnter.location.transform.position;
							pos = Vector3.Lerp (fromPos, toPos, diff);

							if (currentTime != currentTimeInterpolated) 
							{				
								forward = (toPos - fromPos).normalized;
							}
						}

						break;
					}
				}

				//Rotation
//				Debug.Log(Mathf.Abs(currentTime-currentTimeInterpolated));
//				if (currentPlayer == false && currentTime==currentTimeInterpolated) 
				if (Mathf.Abs(currentTime-currentTimeInterpolated) < 0.05f) 
				{
					Location location = GetLocationAtTime (currentTime);
					Location nextLocation = GetLocationAtTime (currentTime+1);

					if (location != nextLocation)
					{
						Vector3 fromPos = location.transform.position;
						Vector3 toPos = nextLocation.transform.position;
						forward = (toPos - fromPos).normalized;
					}
					else 
					{
						forward = Vector3.back;
					}
				}

//				if (currentPlayer == false) 
//				{
//					ActionEnter actionEnter = GetAction (currentTime + 1) as ActionEnter;
//
//					if (actionEnter != null) 
//					{
//						Vector3 fromPos = fromLocation.transform.position;
//						Vector3 toPos = actionEnter.location.transform.position;
//
//						if (currentTime != currentTimeInterpolated) 
//						{				
//							forward = (toPos - fromPos).normalized;
//						}
//					}
//				}
			}

			float trackStart = GetTrackStart(currentTimeInterpolated);
			float trackEnd = GetTrackEnd (currentTime, currentTimeInterpolated, timeForNextDecision, currentPlayer, draggingPlayhead);

			Vector3 localScale = timeLine.bar.localScale;
			localScale.x = trackEnd - trackStart;
			timeLine.bar.localScale = localScale;

			Vector3 localPos = timeLine.bar.localPosition;
			localPos.x = trackStart;
			timeLine.bar.localPosition = localPos;

			transform.position = pos;


			if (draggingPlayhead) 
			{
				transform.rotation = Quaternion.LookRotation (forward); 
				//transform.forward += forward;
			}
			else 
			{
				if (currentPlayer) 
				{
					transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forward), 14f * Time.deltaTime);
				}
				else 
				{
					transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (forward), 30f * Time.deltaTime);
				}
				//transform.forward += (forward - transform.forward) * 8f * Time.deltaTime;
			}
				
			visualsWait.SetActive (false);
			visualsDefault.SetActive (true);

			if (Mathf.Abs(transform.rotation.eulerAngles.y - 180f) < 5f) 
			{
				if (currentTime > 0) 
				{
//					visualsWait.SetActive (true);
//					visualsDefault.SetActive (false);
				}
//				Action actionCurrent = GetAction (currentTime);
//				if (actionCurrent != null) {
//					ActionWait actionWai = actionCurrent as ActionWait;
//					if (actionWai != null) {
//						visualsWait.SetActive (true);
//						visualsDefault.SetActive (false);
//					}
//				}
//
//				if (GetLastTime () == currentTime) {
//					actionCurrent = GetAction (currentTime - 1);
//					if (actionCurrent != null) {
//						ActionWait actionWai = actionCurrent as ActionWait;
//						if (actionWai != null) {
//							visualsWait.SetActive (true);
//							visualsDefault.SetActive (false);
//						}
//					}
//				}

			}
		}

		public Action GetAction(int targetTime)
		{
			int time = 0;

			for (int i = 0, count = history.Count; i < count; i++) 
			{
				Action action = history[i];
				time += action.duration;

				if(time > targetTime)
					return action;
			}

			return null;
		}

		public int RemoveLastAction()
		{
			int lastIndex = history.Count - 1; 
			Action action = history[lastIndex];
			history.RemoveAt(lastIndex);

			return action.time;
		}
	}
}
