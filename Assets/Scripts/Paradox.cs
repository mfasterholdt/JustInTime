using UnityEngine;
using System.Collections;

public class Paradox
{
	public Action action;
	public Observation observation;
	public string message;

	public Paradox(Action action, string message)
	{
		this.action = action;
		this.message = message;
	}

	public Paradox(Observation observation, string message)
	{
		this.observation = observation;
		this.message = message;
	}
}
