using UnityEngine;
using System.Collections;

public class OrientationSwitch : MonoBehaviour
{
		public MonoBehaviour switched;
		public MonoBehaviour toggle;
		public Transform view;
		public Transform cast;
		public float minAngle = 0f; //degrees
		public float maxAngle = 180f; //degrees
		float minCos;
		float maxCos;
		Hysteresis orientationState;

		// Use this for initialization
		void Start ()
		{
				minCos = -Mathf.Cos (minAngle * Mathf.Deg2Rad);
				maxCos = -Mathf.Cos (maxAngle * Mathf.Deg2Rad);
				orientationState = new Hysteresis ();
		
				orientationState.increasing = minCos < maxCos;
				orientationState.persistence = minCos;
				orientationState.activation = maxCos;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (switched == null ||
					view == null ||
					cast == null)
						return;

				if (toggle &&
					toggle.enabled)
						return;

				orientationState.position = -Vector3.Dot (cast.up, (cast.position - view.position).normalized);
				if (switched.enabled != orientationState.activated)
						switched.enabled = orientationState.activated;
		}
}
