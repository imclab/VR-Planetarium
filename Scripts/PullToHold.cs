using UnityEngine;
using System.Collections;

/// <summary>
/// Linearly interpolates between hold and rest transforms.
/// </summary>
public class PullToHold : MonoBehaviour
{
		public Transform rest;
		public float maxSpeed = 0; // Maximum movement speed - also determines rotation & scale interpolations

		[HideInInspector]
		public PullObject
				holder = null; // holder is set by PullObject
		[HideInInspector]
		public bool 
				isHeld = false;
		[HideInInspector]
		public bool 
				atRest = false;
	
		// FixedUpdate is called with invariant delta-time on GameObjects with RigidBody component
		void FixedUpdate ()
		{
				if (maxSpeed < Mathf.Epsilon)
						return;
				bool toHold = false;
				Transform globalTarget = rest;
				if (holder != null) {
						toHold = true;
						globalTarget = holder.hold;
				}
				if (globalTarget == null) {
						return;
				}

				// Linear interpolation between pull & rest
				Vector3 displace = (globalTarget.position - transform.position);
				float deltaSpace = Time.deltaTime * maxSpeed;
				if (deltaSpace > displace.magnitude) {
						if (toHold) {
								isHeld = true;
						} else {
								atRest = true;
						}
						transform.position = globalTarget.position;
						transform.rotation = globalTarget.rotation;
						transform.localScale = globalTarget.localScale;
						return;
				} else {
						isHeld = false;
						atRest = false;
				}
		
				float scale = deltaSpace / displace.magnitude;
				transform.position += displace * scale;
				transform.rotation = Quaternion.Lerp (transform.rotation, globalTarget.rotation, scale);
				transform.localScale *= (1f - scale);
				transform.localScale += scale * globalTarget.localScale;
		}
}
