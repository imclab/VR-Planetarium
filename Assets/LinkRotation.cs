using UnityEngine;
using System.Collections;

public class LinkRotation : MonoBehaviour
{

		public GameObject SourceGameObject;

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
				transform.rotation = SourceGameObject.transform.rotation;
		}
}
