using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PreserveChildren : MonoBehaviour
{
		[HideInInspector]
		public List<GameObject>
				preserved;

		// Use this for initialization
		void Awake ()
		{
				preserved = new List<GameObject> ();
		}
	
		// Update is called once per frame
		void OnDestroy ()
		{
				foreach (GameObject child in preserved) {
						child.transform.parent = null;
				}
		}
}
