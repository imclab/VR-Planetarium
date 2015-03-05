using UnityEngine;
using System.Collections;

/// <summary>
/// When enabled, hand will pull in first PullToHold object with which it is aligned.
/// </summary>
public class PullObject : MonoBehaviour
{
		public Transform view;
		public Transform cast;
		public Transform hold;
		public int layerSelect = 0;
		PullToHold pulled;
	
		// Identify pulled object using view of hand
		void Update ()
		{
				if (view == null ||
						cast == null ||
						hold == null)
						return;

				if (pulled != null) {
					return;
				}
		
				int layerMask = 1 << layerSelect; //Select a single layer
				RaycastHit hit;
				Ray ray = new Ray (view.position, cast.position - view.position);
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, layerMask)) {
						Debug.DrawRay (ray.origin, hit.point - ray.origin, Color.red);
						PullToHold intersect = hit.collider.gameObject.GetComponent<PullToHold> ();
						if (intersect != null) {
								Acquire (intersect);
						}
				} else {
						Debug.DrawRay (ray.origin, ray.direction * 100f, Color.white);
				}
		}

		void OnDisable ()
		{
				Release ();
		}

		void Release ()
		{
				if (pulled == null)
						return;
				//NOTE: pulled reference is implicitly made null if pulled is destroyed
				pulled.holder = null;
				pulled = null;
		}

		void Acquire (PullToHold intersect)
		{
				if (intersect.holder != null) {
						intersect.holder.enabled = false;
				}

				pulled = intersect;
				if (intersect == null)
						return;
				//NOTE: hold reference is implicitly made null if hold is destroyed
				pulled.holder = this;
		}
}
