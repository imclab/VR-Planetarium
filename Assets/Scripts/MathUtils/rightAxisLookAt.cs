using UnityEngine;
using System.Collections;

/// <summary>
/// Rotates Forward basis vector towards target
/// with rotation limited to be around the right axis only.
/// </summary>
public class rightAxisLookAt : MonoBehaviour
{
		public Transform lookAt;
		public float filterRate = 1f;
		public bool IsFilterOn = false;
		public float LookAtXOffset = -.1f;
	
		
		void Start () {
			Vector3	LookAtOffsetPos = new Vector3(LookAtXOffset, lookAt.localPosition.y, lookAt.localPosition.z);
			lookAt.localPosition = LookAtOffsetPos;
		}
		
		// Update is called once per frame
		void Update ()
		{
				if (lookAt == null)
						return;

				Vector3 lookVector = lookAt.position - transform.position;
				float rightProject = Vector3.Dot (lookVector, transform.right);
				lookVector -= transform.right * rightProject;
//				Debug.DrawRay (transform.position, lookVector, Color.red);
//				Debug.DrawRay (transform.position + lookVector, lookAt.position - (transform.position + lookVector), Color.red);
//				Debug.DrawRay (transform.position, transform.forward, Color.blue);
				Quaternion lookRotate = Quaternion.FromToRotation (transform.forward, lookVector);
				float lookAngle;
				Vector3 lookAxis;
				lookRotate.ToAngleAxis (out lookAngle, out lookAxis);
				if(IsFilterOn == true){
					transform.Rotate (lookAxis, lookAngle * filterRate, Space.World);
				}
				else{
					transform.Rotate (lookAxis, lookAngle * 1.0f, Space.World);
				}
		}
}
