using UnityEngine;
using System.Collections;

public class OrientationSwitch : MonoBehaviour
{
  [SerializeField]
  private WidgetShowcase.RayCastEmitter m_raycastEmmitter;

		public MonoBehaviour switched;
		public MonoBehaviour toggle;
		public Transform view;
    public WidgetShowcase.HandEmitter Hand;
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
        Hand == null ||
        Hand.CurrentHand == null) {
      return;
    }

		if (toggle &&
        toggle.enabled) {
			return;
    }

    //Don't allow constellation grab is below the horizon.
    if ( m_raycastEmmitter!= null && m_raycastEmmitter.RayCheck(Hand.CurrentHand.GetPalmPosition()) ) {
      if (switched.enabled != false)
        switched.enabled = false;
      return;
    }

    orientationState.position = -Vector3.Dot (-Hand.CurrentHand.GetPalmNormal(), (Hand.CurrentHand.GetPalmPosition() - view.position).normalized);

		if (switched.enabled != orientationState.activated)
			switched.enabled = orientationState.activated;
  }
}
