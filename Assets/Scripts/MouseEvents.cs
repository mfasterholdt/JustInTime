using UnityEngine;
using System.Collections;

public class MouseEvents : MonoBehaviour 
{
	public delegate void MouseEvent(Transform obj);
	public MouseEvent mouseDown; 

	void OnMouseDown()
	{
		if(mouseDown != null)
			mouseDown(transform);
	}
}
