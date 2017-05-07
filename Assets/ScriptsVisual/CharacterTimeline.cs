using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class CharacterTimeline : MonoBehaviour 
	{
		public new MeshRenderer renderer;

		public void Setup(Material material)
		{
			renderer.material = material;
		}
	}
}
