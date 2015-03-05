using UnityEngine;
using System.Collections;
using WidgetShowcase;

namespace LMWidgets
{

/**

This is the basis for the lat lon dials. It has the feature of using Modality/Mutex 
such that when one dial starts,
it locks out all other dials for a while.
Note - there is a certain "double record" system here with both 
modality activation and trigger tracking being used
to cover the state of being active. 

*/
		public class DialBase : MonoBehaviour, ModalitySubscriber
		{
				public float minimumAngle = 0.0f;
				public float maximumAngle = 360.0f;
				public int steps = 0;
				public bool snapToStep = false;
				private GameObject target_ = null;
				private Vector3 pivot_ = Vector3.zero;
				private float start_angle_ = 0.0f;
				private float prev_angle_ = 0.0f;
				private float curr_angle_ = 0.0f;
				public GameObject Indicator; // the object that is color changed for highlighting

				const float MAX_WAIT_FOR_TRIGGER_UPDATE = 0.5f; // the max time between validation of trigger before deactivation is forced.

				public float GetAngle ()
				{
						return curr_angle_;
				}

				public float GetPercent ()
				{
						if (minimumAngle != maximumAngle)
								return (transform.localRotation.eulerAngles.y - minimumAngle) / (maximumAngle - minimumAngle);
						else
								return 100.0f;
				}

				public float GetStepAngle ()
				{
						int step = GetStep ();
						if (steps > 0)
								return (maximumAngle - minimumAngle) * step / steps;
						else
								return 0.0f;
				}

				public int GetStep ()
				{
						return (int)(GetPercent () * steps / 100.0f + 0.5f);
				}

				private bool IsHand (Collider other)
				{
						return other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel> ();
				}

				private bool IsFingerTip (Collider other, string finger)
				{
						return (other.name == "bone3" && other.transform.parent.name == finger);
				}

				void OnTriggerEnter (Collider other)
				{
						if (target_ == null && IsHand (other)) {
								if (IsActive || (DBmodality.CanActivate (this.gameObject))) {
										Activate ();
										if (!IsActive) {
												//Debug.Log (string.Format ("Cannot activate {0}", name));
												return;
										}	
										target_ = other.gameObject;
										pivot_ = transform.InverseTransformPoint (target_.transform.position) - transform.localPosition;
										if (!transform.rigidbody.isKinematic)
												transform.rigidbody.angularVelocity = Vector3.zero;
								}
						}
				}

				void OnTriggerStay (Collider other)
				{
						if (IsTarget (other.gameObject)) {
								if (IsActive)
										LastContactWithTarget = Time.time;
						}
			
				}

				bool IsTarget (GameObject g)
				{
						return (target_ && (g == target_));
				}
		
				public static ModalityManager DBmodality = new ModalityManager ();

				void OnTriggerExit (Collider other)
				{
						if (IsTarget (other.gameObject)) {
								target_ = null;
								Deactivate ();
								float FPS_INVERSE = 1.0f / Time.deltaTime;
								float angular_velocity = (curr_angle_ - prev_angle_) * FPS_INVERSE;
								transform.rigidbody.AddRelativeTorque (new Vector3 (0.0f, 0.0f, angular_velocity));
								if (snapToStep)
										SnapToStep ();
						}
				}

				protected virtual void ApplyRotations ()
				{
						Vector3 curr_direction = transform.InverseTransformPoint (target_.transform.position) - transform.localPosition;
						transform.localRotation = Quaternion.FromToRotation (pivot_, curr_direction) * transform.localRotation;
				}

				protected virtual void ApplyConstraints ()
				{
						Vector3 rotation = transform.localRotation.eulerAngles;
						rotation.x = 0.0f;
						//rotation.y = curr_angle_ % 360;
						rotation.z = 0.0f;
						transform.localRotation = Quaternion.Euler (rotation);
				}

				protected virtual void CalculateAngles ()
				{
						prev_angle_ = (curr_angle_ > 0) ? curr_angle_ % 360 : 360.0f - (- curr_angle_) % 360;
						float delta_angle = transform.localRotation.eulerAngles.y - prev_angle_;
						if (delta_angle > 180.0f) { // Most likely moved back rather than forward
								delta_angle -= 360.0f;
						} else if (delta_angle < -180.0f) { // Most likely moved forward rather than move back
								delta_angle += 360.0f;
						}
						curr_angle_ = Mathf.Clamp (curr_angle_ + delta_angle, minimumAngle, maximumAngle);
				}

				protected virtual void SnapToStep ()
				{
						Vector3 rotation = transform.localRotation.eulerAngles;
						rotation.x = 0.0f;
						rotation.y = GetStepAngle ();
						rotation.z = 0.0f;
						transform.localRotation = Quaternion.Euler (rotation);
				}

				public virtual void Awake ()
				{
						start_angle_ = transform.localRotation.eulerAngles.y;
						if (maximumAngle < minimumAngle) {
								minimumAngle = 0.0f;
								maximumAngle = 0.0f;
						}
				}

		#region loop
		
				void Start ()
				{
						Subscribe ();
						offColor = indicatorMaterial.color;
				}

				void Update ()
				{
						if (IsActive) {
								if (Time.time - LastContactWithTarget > MAX_WAIT_FOR_TRIGGER_UPDATE) {
										//Debug.Log (string.Format ("{0} Deactivating - contact timeout", name));
										Deactivate ();
								}

								if (target_ != null) {
										//Debug.Log (string.Format ("{0} applying rotations", name));
										ApplyRotations ();
								}
								//Debug.Log (string.Format ("{0} is active", name));
						} else {
						}
						CalculateAngles (); //@TODO: are these things necessary without target_/Activation?
						ApplyConstraints ();
				}
#endregion

				Color offColor;
				Color onColor = new Color (45f / 255f, 135f / 255f, 188f / 255f);

		#region ModalitySubscriber implementation

				public void Modality (ModalityManager.ModalityMessage message)
				{
						//throw new System.NotImplementedException ();
				}

				public void Subscribe ()
				{
						DBmodality.Subscribe (this.gameObject);
				}

				public void Unsubscribe ()
				{
						DBmodality.Unsubscribe (this.gameObject);
				}

				float lastContactWithTarget_ = -1;

				public float LastContactWithTarget {
						get {
								return lastContactWithTarget_;
						}
						set {
								//Debug.Log (string.Format ("Setting LCWT of {0} to {1}", name, value));

								lastContactWithTarget_ = value;
						}
				}

				public bool Activate ()
				{
						//Debug.Log (string.Format ("{0} Activating", name));
			
						if (DBmodality.Activate (this.gameObject)) {
								LastContactWithTarget = Time.time;
								indicatorMaterial.color = onColor;
								//Debug.Log (string.Format ("{0} Activating -- SUCCESS", name));
								return true;
						} else { 
								//Debug.Log (string.Format ("{0} Activating -- FAIL", name));
								return false;
						}
				}

				public bool IsActive {
						get { 
//				Debug.Log (string.Format ("Checking activation of {0} against AIN {1}", gameObject.GetInstanceID().ToString (), DBmodality.ActiveItemName));
								return DBmodality.ActiveItemName == gameObject.GetInstanceID().ToString();
						}
				}

				public bool Deactivate ()
				{
						target_ = null;
						LastContactWithTarget = -1;
						indicatorMaterial.color = offColor;
						return  DBmodality.Deactivate (this.gameObject);
				}

				public Material indicatorMaterial {
						get {
								return Indicator.renderer.material;
						}
				}

		#endregion
		}
}

