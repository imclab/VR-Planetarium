using UnityEngine;
using System.Collections;
using WidgetShowcase;

/**
This is an emitter that emits a truth case when a ray 
from centerObject to centerObject 
to intersect a target of type mask.

This class will emit a value once per frame; to reduce noise, set 
DontEmitUnchangedValue to true;
*/

namespace WidgetShowcase
{
		public class RayCastEmitter : BoolEmitter
		{
				public float MaxDistance = 10000.0f;
				public GameObject centerObject = null;
				public GameObject targetObject = null;
				public LayerMask mask;

				[HideInInspector]
				public GameObject lastHit = null; 

	#region loop

				void Update ()
				{
						if ((centerObject != null) && (targetObject != null) && (centerObject.activeSelf) && (targetObject.activeSelf)) {
								Debug.Log ("Testing RayCast");
								BoolValue = RayCheck ();
						} else {
								lastHit = null;
								BoolValue = false;
						}
				}

 #endregion

				Vector3 center {
						get { return centerObject.transform.position; }
				}

				Vector3 targetPosition {
						get { return targetObject.transform.position; }
				}

				Vector3 direction {
						get { return (targetPosition - center).normalized; }
				}
		
				public bool RayCheck ()
				{
						return RayCheck (targetPosition);
				}
		
				public bool RayCheck (Vector3 tp)
				{
						Ray ray = new Ray (center, (tp - center).normalized);
			
						RaycastHit hitInfo;
			//Debug.DrawRay(center, tp * MaxDistance, new Color(1.0f, 0.0f,0.0f));
					//	Debug.Log (string.Format ("Testing raytrace between {0} with vector through {1}", center, tp));
			
			if (Physics.Raycast (ray, out hitInfo, MaxDistance, LayerMask.GetMask("Earth"))) {
							//	Debug.Log ("Hit test successful -- result " + hitInfo.transform.name);
								lastHit = hitInfo.transform.gameObject;
								return true;
						} else {
							//	Debug.Log ("Hit Test failed");
								lastHit = null;
								return false;
						}
			
				}
		
				public bool RayCheck (Vector3 center, Vector3 targetPosition)
				{
						Ray ray = new Ray (center, (targetPosition - center).normalized);
			
						RaycastHit hitInfo;
			
						if (Physics.Raycast (ray, out hitInfo, MaxDistance, mask)) {
							//	Debug.Log ("Hit test successful -- result " + hitInfo.transform.name);
								lastHit = hitInfo.transform.gameObject;
								return true;
						} else {
							//	Debug.Log ("Hit Test failed");
								lastHit = null;
								return false;
						}
			
				}

		}

}