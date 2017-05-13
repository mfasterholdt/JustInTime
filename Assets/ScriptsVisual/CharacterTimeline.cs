using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incteractive
{
	public class CharacterTimeline : MonoBehaviour 
	{
		public Transform bar;
		public new MeshRenderer barRenderer;

		private List<GameObject> symbols = new List<GameObject>();

		public void Setup(Material material)
		{			
			barRenderer.material = material;
		}

		public void AddSymbol(GameObject prefab, Material material, int time)
		{
			GameObject newSymbol = Instantiate (prefab);

			newSymbol.transform.parent = transform;
			newSymbol.transform.localRotation = Quaternion.identity;
			newSymbol.transform.localPosition = Vector3.right * time;

			MeshRenderer renderer = newSymbol.GetComponent<MeshRenderer> ();

			if(renderer)
				renderer.material = material;

			symbols.Add (newSymbol);
		}

		public void RemoveSymbols(int currentTime)
		{
			for (int i = symbols.Count -1; i >= 0; i--)
			{
				GameObject symbol = symbols [i];

				if (symbol.transform.localPosition.x >= currentTime) 
				{
					symbols.Remove (symbol);
					Destroy (symbol);
				}
				else 
				{
					break;
				}
			}	
		}
	}
}
