using UnityEngine;
using WidgetShowcase;
using System.Collections;
using Stars;

public class SliderControls : MonoBehaviour
{
	
		public Transform Earth;
		[SerializeField]
		private AnimationCurve
				directionCurve;
		[SerializeField]
		private AnimationCurve
		rotationCurve;
		[SerializeField]
		private float
				m_maxDisplace = 0.5f; // Meters
		[SerializeField]
		private float
				m_maxSpeed = 20f; // Meters per Second
		[SerializeField]
		private float
				m_maxRotate = 945f; // Degrees
		[SerializeField]
		private float
				m_maxSpin = 360f; // Degrees per Second
		private Vector3 m_speed = Vector3.zero; // Orbital momentum relative to user camera
		private float m_spin = 0f; // Angular momentum about the user camera up axis
		private bool m_isMoving = false;

		// Use this for initialization
		void Start ()
		{
		}

		bool joyBallRegistered;

		void RegisterJoyBall ()
		{
				if (JoyBall.Instance != null) {
						JoyBall.Instance.OnJoystickEvent += onJoyStickEvent;
						joyBallRegistered = true;
				}
		}

		void onJoyStickEvent (object sender, WidgetEventArg<JoystickEvent> e)
		{
				switch (e.CurrentValue.Type) {
				case JoystickEventType.Start:
						m_isMoving = true;
						break;
				case JoystickEventType.Changed:
						m_speed = normalizeJoystickDirection (e.CurrentValue.Direction);
						m_spin = normalizeJoystickRotation (e.CurrentValue.Rotation);
						break;
				case JoystickEventType.End:
						m_isMoving = false;
						break;
				default:
						Debug.LogError ("UNKNOWN JoystickEventType " + e.CurrentValue);
						break;
				}
		}

	#region EarthOrbit
		// NOTE: This region and the TimeAndLocationHandler AziLatLon properties should be unified
		void FixedUpdate ()
		{
				if (!joyBallRegistered) {
						RegisterJoyBall ();
				} else if (m_isMoving &&
						   Earth != null) {
						//Debug.Log ("BEFORE AziLatLon = " + TimeAndLocationHandler.Instance.AziLatLon);
						Vector3 worldFrom = transform.position - Earth.position;
						Vector3 worldTo = worldFrom + (m_speed * Time.deltaTime);
						float angle = 0f;
						Vector3 axis = transform.transform.up;
						Quaternion.FromToRotation (worldFrom, worldTo).ToAngleAxis (out angle, out axis);
						transform.transform.RotateAround (Earth.position, axis, angle);
						transform.transform.RotateAround (transform.transform.position, transform.transform.up, m_spin * Time.deltaTime);
			
						Vector3 recenter = transform.position;
						transform.position = Vector3.zero;
						Earth.position -= recenter;
						//Debug.Log ("AFTER AziLatLon = " + TimeAndLocationHandler.Instance.AziLatLon);
				}
		}
	
	#endregion
	
	
	#region JoyBallNormalizers

		float applyCurvesToRotation (float f)
		{
				bool isNeg = f < 0;
				return rotationCurve.Evaluate (Mathf.Abs (f)) * (isNeg ? -1 : 1);
		}


		/// <returns>Global coordinate displacement vector defined by Joystick</returns>
		// NOTE: This region should be be the front-most level of the JoyBall
		// NOTE: Output is a 3 vector with magnitude less than or equal to max speed.
		private Vector3 normalizeJoystickDirection (Vector3 input)
		{
				// Project displacement to tangent plane
				Vector3 tangent = input;
				tangent -= transform.up * Vector3.Dot (input, transform.up);
				// Apply gain curve to displacement
				float displace = tangent.magnitude;
				float output = directionCurve.Evaluate (displace / m_maxDisplace);
				output *= m_maxSpeed;
				return tangent.normalized * output;
		}
	
		/// <returns>Angle (degrees) of rotation about Up axis in local coordinates</returns>
		private float normalizeJoystickRotation (Quaternion input)
		{
				// Project rotation to be in tangent plane
				float angle;
				Vector3 axis;
				input.ToAngleAxis (out angle, out axis);
				float pivot = Vector3.Dot ((axis * angle), transform.up);
				// Map angle to [-180,180] range
				if (pivot > 180f) {
						pivot -= 360f;
				}
				if (pivot < -180f) {
						pivot += 360f;
				}
				// Apply gain curve to pivoting
				float output = rotationCurve.Evaluate (Mathf.Abs (pivot) / m_maxRotate) * Mathf.Sign (pivot);
				output *= m_maxSpin;
				return output;
		}
	#endregion

		private void onStarNameSliderChange (object sender, WidgetEventArg<float> args)
		{
				StarUpdater.Instance.SetLabelOpacity (args.CurrentValue);
		}
  
		private void onMilkyWayChange (object sender, WidgetEventArg<float> args)
		{
    
		}
  
		private void onStarBrightnesChange (object sender, WidgetEventArg<float> args)
		{
				StarUpdater.Instance.SetMinLuminance (args.CurrentValue);
		}
  
		private void onDepthChange (object sender, WidgetEventArg<float> args)
		{
				StarUpdater.Instance.SetZoom (args.CurrentValue);
		}

		private void onSatChange (object sender, WidgetEventArg<float> args)
		{
				StarUpdater.Instance.SetStarSaturation (args.CurrentValue);
		}

		private void onAsterismChange (object sender, WidgetEventArg<float> args)
		{
				Asterisms.AsterismDrawer.SetAsterismOpacity (args.CurrentValue);
		}
}
;