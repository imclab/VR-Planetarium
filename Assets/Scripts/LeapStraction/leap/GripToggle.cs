using UnityEngine;
using System.Collections;
using Leap;

/// <summary>
/// Grip hand to toggle associated behavior.
/// </summary>
/// <remarks>
/// GripToggle cannot be disabled while toggle is enabled.
/// </remarks>
public class GripToggle : MonoBehaviour
{
  [SerializeField]
  private WidgetShowcase.HandEmitter m_hand;
		public MonoBehaviour toggle;
		public float gripOff = 0.3f;
		public float gripOn = 0.7f;
		public Hysteresis gripState;
		bool gripStateLast;

		void Start ()
		{
				gripState = new Hysteresis ();
				if (gripOff <= gripOn) {
						gripState.increasing = true;
						gripState.persistence = gripOff;
						gripState.activation = gripOn;
				} else {
						gripState.increasing = false;
						gripState.persistence = gripOn;
						gripState.activation = gripOff;
				}
		}
	
		// Update is called once per frame
		void Update ()
		{
    if (toggle == null || m_hand.CurrentHand == null) {
      return;
    }


				Hand leap_hand = m_hand.CurrentHand.GetLeapHand ();
				gripState.position = leap_hand.GrabStrength;
				if (!gripStateLast && gripState.activated)
					// Only toggle when state changes to grip
					toggle.enabled = !toggle.enabled;
				gripStateLast = gripState.activated;
		}
}
