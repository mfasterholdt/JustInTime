using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour 
{
	public GameObject released;
	public GameObject pressed;
	public Collider collider;

	public bool value;

	void Awake()
	{
		Toggle (false);
	}

	public void Toggle(bool value)
	{
		pressed.SetActive(value);
		released.SetActive(!value);

		this.value = value;
	}
}
