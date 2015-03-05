using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarkPassage : MonoBehaviour
{
		public Color onColor = new Color (210f / 255f, 38f / 255f, 75f / 255f);
		public Color offColor = new Color (4f / 255f, 92f / 255f, 204f / 255f);
		public GameObject toggleMark;
		public bool stateOn = false;
		Dictionary<GameObject, Vector3> enterPoints = new Dictionary<GameObject, Vector3> ();

		// Use this for initialization
		void Start ()
		{
				colorMark ();
		}
	
		// Called when object enters
		void OnTriggerEnter (Collider other)
		{
				enterPoints.Add (other.gameObject, other.transform.position - transform.position);
		}

		void OnTriggerExit (Collider other)
		{
				Vector3 entered;
				if (enterPoints.TryGetValue (other.gameObject, out entered)) {
						enterPoints.Remove (other.gameObject);
						if (enterPoints.Count == 0) {
								// Last object in chain passed through
								Vector3 passage = other.transform.position - transform.position;
								passage -= entered;
								if (Vector3.Dot (passage, transform.forward) > 0f) {
										// Moving in the right direction -> mark passage
										stateOn = ! stateOn;
										colorMark ();
								}
						}
				}
		}

		void colorMark ()
		{
				if (toggleMark != null &&
						toggleMark.renderer != null &&
						toggleMark.renderer.material != null) {
						if (stateOn) {
								toggleMark.renderer.material.color = onColor;
						} else {
								toggleMark.renderer.material.color = offColor;
						}
				}
		}
}
