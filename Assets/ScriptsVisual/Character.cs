using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class Character : MonoBehaviour
	{
		public Location initialLocation;


		public List<Action> history = new List<Action>();
		public Transform timeLineTrack;
		public new MeshRenderer renderer;

		public void Setup(Location initialLocation, Material material, Transform timeLineTrack)
		{
			this.initialLocation = initialLocation;
			renderer.material = material;
			this.timeLineTrack = timeLineTrack;

			CharacterTimeline characterTimeline = timeLineTrack.GetComponent<CharacterTimeline> ();
			characterTimeline.Setup(material);

//			Reset ();
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


		public Location GetLocationAtTime(int time)
		{
			Location result = initialLocation;

			if (history.Count > 0) 
			{
				for (int i = 0, count = history.Count; i < count; i++) 
				{					
					ActionEnter actionEnter = history [i] as ActionEnter;

					if (actionEnter != null && time >= actionEnter.time) 
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

		public float GetTrackEnd(int currentTime, float currentTimeInterpolated, bool currentPlayer, bool draggingPlayhead)
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

		public void UpdateCharacter(int currentTime, float currentTimeInterpolated, bool currentPlayer, bool draggingPlayhead)
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

					if (currentTimeInterpolated > action.time + 1) 
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
								forward = toPos - fromPos;
							}
						}

						break;
					}
				}

			}

			float trackStart = GetTrackStart(currentTimeInterpolated);

//			float trackEnd = 0;
			float trackEnd = GetTrackEnd (currentTime, currentTimeInterpolated, currentPlayer, draggingPlayhead);

			//Timeline Tracker
//			if (initialLocation.isTimeMachine) 
//			{
//				for (int i = 0, count = history.Count; i < count; i++)
//				{
//					ActionEnter actionEnter = history [i] as ActionEnter;
//					
//					if (actionEnter != null)
//					{
//						trackStart = actionEnter.time;
//						break;
//					}
//				}
//			}
//			else 
//			{
//				trackStart = history [0].time;
//			}

//			trackEnd = 0;

//			if (currentPlayer) 
//			{
//				if (draggingPlayhead) 
//				{
//					//trackEnd = trackStart;
//
//					if (history.Count > 0)
//					{
//						bool foundAction = false;
//
//						for (int i = history.Count - 1; i >= 0; i--) {
//							Action action = history [i];
//
//							Location location = GetLocationAtTime (action.time);
//
//							if (location.isTimeMachine == false) {
//								trackEnd = action.time + action.duration;
//								foundAction = true;
//								break;
//							}
//						}
//
//						if (initialLocation.isTimeMachine && foundAction == false)  
//						{
//							trackEnd = 0;
//						}
//						else 
//						{
//							if (currentTimeInterpolated > trackEnd)
//								trackEnd = currentTimeInterpolated;
//						}
//					}
//					else 
//					{
//						if (initialLocation.isTimeMachine == false) 
//						{
//							trackEnd = currentTimeInterpolated;
//						} 
//						else 
//						{
//							bool foundAction = false;
//
//							for (int i = history.Count - 1; i >= 0; i--) {
//								Action action = history [i];
//
//								Location location = GetLocationAtTime (action.time);
//
//								if (location.isTimeMachine == false) 
//								{
//									trackEnd = action.time + action.duration;
//									foundAction = true;
//									break;
//								}
//							}
//
//							if (foundAction == false)
//								trackEnd = 0;
//						}
//
//					}
//				}
//				else 
//				{
//					bool foundAction = false;
//
//					for (int i = history.Count - 1; i >= 0; i--) {
//						Action action = history [i];
//
//						Location location = GetLocationAtTime (action.time);
//
//						if (location.isTimeMachine == false) 
//						{
//							foundAction = true;
//							break;
//						}
//					}
//
//					if (foundAction == false) 
//					{
//						trackEnd = 0;
//					}
//					else 
//					{
//						if (currentTimeInterpolated > trackEnd)
//							trackEnd = currentTimeInterpolated;
//					}
//				}
//			}
//			else 
//			{
//				if (history.Count > 0) 
//				{
//					for (int i = history.Count - 1; i >= 0; i--) 
//					{
//						Action action = history[i];
//
//						Location location = GetLocationAtTime (action.time);
//						if (location.isTimeMachine == false) 
//						{
//							trackEnd = action.time + action.duration + 1;
//							break;
//						}
//					}
//				}
//			}
//
//			if (currentPlayer)
//				Debug.Log (trackStart + " , " +trackEnd);
			
//			if (currentPlayer) 
//			{
//				if (initialLocation.isTimeMachine) 
//				{
//						
//				}
//				else 
//				{
//					
//				}
//
////				if (history.Count > 0) 
////				{
//////				if (currentPlayer && !draggingPlayhead) 
//////				{
//////					if (trackEnd > currentTimeInterpolated)
//////						trackEnd = currentTimeInterpolated;
//////				}
//////				else 
//////				{
////					Action lastAction = history [history.Count - 1];
////					trackEnd = lastAction.time + lastAction.duration;
//////				}
////				}
////
////				if (currentPlayer && currentTimeInterpolated > trackEnd) 
////				{
////					trackEnd = currentTimeInterpolated;
////				}
//
//			}
//			else 
//			{
//				Action lastAction = history [history.Count - 1];
//				trackEnd = lastAction.time + lastAction.duration;
//			}

			Vector3 localScale = timeLineTrack.localScale;


			localScale.x = trackEnd - trackStart;
			timeLineTrack.localScale = localScale;

			Vector3 localPos = timeLineTrack.localPosition;
			localPos.x = trackStart;
			timeLineTrack.localPosition = localPos;

			transform.position = pos;
			transform.forward = forward;
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
