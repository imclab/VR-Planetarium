using UnityEngine;
using System.Collections;
using System;
using Leap;
using UnityEngine.UI;

namespace WidgetShowcase
{

/**
returns the relative offset of one object from another, relative to a camera. 
Note - a considerable amount of functionality has been exported to the controller gameObject. 
Hands, for instance, are injected via input classes.
In point of fact EVERYTHING done here could be expressed in DataEmitters; I left the code here 
for economy, but in another pass, I'd move all this stuff into individual nodes. 
*/

		/// <summary>
		/// Motion & Rotation control relative to an initial state
		/// </summary>
		/// <remarks>
		/// A requirement for using this system is that both reference and draggable control objects
		/// are transformed with the user. Otherwise motion will be exponential in all directions.
		/// </remarks>
		public class JoyBall : MonoBehaviour
		{
				public Text feedback;

#region Inputs
				public BoolEmitter VisibilityInput;
				public HandEmitter HandInput;
#endregion
		
		#region internal compoentns
				public JoyBallPalmFacing PalmFacing;
				public JoyBallHandPump HandPump;
#endregion

		#region interaction

				//bool RefInitialized = false;
				//float lastSightedHand = -1;

				public virtual event EventHandler<WidgetEventArg<JoystickEvent>> OnJoystickEvent;

				void InitActivationListener ()
				{
						VisibilityInput.BoolEvent += HandleBoolEvent;
				}

				void InitHandListener ()
				{
						HandInput.HandEvent += HandleActivationFromInput;
				}

				void HandleActivationFromInput (object sender, WidgetEventArg<HandData> e)
				{
						if (e.CurrentValue.HasHand) {
								//lastSightedHand = Time.time;
								UpdateJoystickPosition (e.CurrentValue.HandModel);
						} 
				}

				void HandleBoolEvent (object sender, WidgetEventArg<bool> e)
				{
	
						activeness.Change (e.CurrentValue ? Activeness_Active : Activeness_Inactive);
				}

				void Echo (JoystickEventType type, Vector3 direction, Quaternion rotation)
				{
						if (OnJoystickEvent != null) {
								OnJoystickEvent (this, new WidgetEventArg<JoystickEvent> (new JoystickEvent (type, direction, rotation)));
						}
				}

				public static JoyBall Instance = null;

/**
Broadcasts the actionable properties of the joystick, expressed as a direction and rotation in world coordinates
*/
				void UpdateJoystickPosition (HandModel handModel)
				{
						if (handModel == null ||
								activeness.state != Activeness_Active)
								return;

						Vector3 direction = Draggable.transform.position - Reference.transform.position;
						Quaternion rotation = Quaternion.identity;
						if (rotationRegisterded) {
								rotation = Draggable.transform.rotation * Quaternion.Inverse (Reference.transform.rotation);
						}
						Echo (JoystickEventType.Changed, direction, rotation);
				}
#endregion

		#region loop
				// Use this for initialization
				void Start ()
				{
						if (Instance != null) {
								throw new UnityException ("THERE CAN BE ONLY ONE!");
						}
						Instance = this;

						InitState ();
						InitObjectAlpha ();
						InitActivationListener ();
						InitHandListener ();
						
				}

				void InitState ()
				{
						activeness = StateList.Init ("JB Activeness", Activeness_Inactive, Activeness_Active);
						gesture = StateList.Init ("JB Gesture", Gesture_NoHand, Gesture_Fist, Gesture_Open);
			
						activeness.StateChangedEvent += ActivenessChanged;
						gesture.StateChangedEvent += GestureChanged;
				}

				void ToggleActive ()
				{
						switch (activeness.state) {
						case Activeness_Active:
								activeness.Change (Activeness_Inactive);
								break;
						case Activeness_Inactive:
								activeness.Change (Activeness_Active);

								break;
						}
				}

#endregion

		#region visual objects
				public GameObject Draggable;
				public GameObject Reference;
				public Alphan outerAlphan;
				public Alphan innerAlphan;
		        public GameObject JoyballArrows;
				//public GameObject SpawnPoint;

				void InitObjectAlpha ()
				{
						//ReferenceAlpha.SetAlpha (0, 0.1f);
//							innerAlphan.SetAlpha (0, 1.0f);
				}

//				Alphan ReferenceAlpha {
//						get {
//								return Reference.GetComponent<Alphan> ();
//						}
//				}
        Alphan JoyballArrowsAlpha {
          get {
            return JoyballArrows.GetComponent<Alphan> ();
          }
        }
    #endregion
    
    #region gesture
				public State gesture;
				const string Gesture_Open = "openHand";
				const string Gesture_Fist = "fist";
				const string Gesture_NoHand = "noHand";

				void GestureChanged (StateChange change)
				{
						if (change.unchanged)
								return;
				}

#endregion

		#region openness
				const float MIN_ANGLE = 0.1f;
				const float MAX_ANGLE = -0.2f;
				public State activeness;
				public const string Activeness_Inactive = "closed";
				public const string Activeness_Active = "open";

				// responds to state change of Activeness; sets the alpha of ref objects. 
				void ActivenessChanged (StateChange change)
				{
						if (change.unchanged)
								return;

						if (change.state == Activeness_Active) {
								//Reference.transform.rotation = Draggable.transform.rotation;
								Debug.Log ("JoyBall's ActivenessChanged to: " + change.state);
								innerAlphan.SetAlpha (1.0f, .1f);
								outerAlphan.SetAlpha (1.0f, .1f);
                JoyballArrowsAlpha.SetAlpha(.5f, 0.25f);
        
        rotationRegisterded = false;
								StartCoroutine (UpdateReferenceRotation (0.5f));
								Echo (JoystickEventType.Start, Vector3.zero, Quaternion.identity);
								//This my need to be an Object in Hand method
								GetComponent<ObjectInHand>().OnActivate();

								// Allow player to look around without moving the reference
								GetComponent<AlignHalo>().enabled = false;
						} else {
									innerAlphan.SetAlpha (.1f, 0.25f);
									outerAlphan.SetAlpha (.1f, 0.25f);
                  JoyballArrowsAlpha.SetAlpha(.1f, 0.25f);
        
        //	ReferenceAlpha.SetAlpha (0, 0.1f);
								Echo (JoystickEventType.End, Vector3.zero, Quaternion.identity);
								//RefInitialized = false;
								
								//This may need to go in ObjectInHand
								GetComponent<ObjectInHand>().OnDeactivate();

								// Align JoyBall with field of view
								GetComponent<AlignHalo>().enabled = true;
						}
				}
		#endregion

				private bool rotationRegisterded = false;

				private IEnumerator UpdateReferenceRotation (float seconds)
				{
						float endTime = Time.time + seconds;
						while (Time.time < endTime) {
//        Debug.Log ("Not yet time.");
								yield return 0;
						}

						rotationRegisterded = true;
						//Reference.transform.rotation = Draggable.transform.rotation;

						yield break;
				}


		}
		

}
