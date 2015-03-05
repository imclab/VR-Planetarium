using UnityEngine;
using System.Collections;

public class KeyedEnabler : MonoBehaviour
{
		public GameObject target;
		public KeyCode key;
	
		// Update is called once per frame
		void Update ()
		{
				if (target == null)
						return;
				if (Input.GetKeyDown (key))
						target.SetActive (!target.activeSelf);
		}
}
