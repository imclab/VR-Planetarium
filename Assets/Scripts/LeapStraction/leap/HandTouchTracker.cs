using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
This class keeps track of when the last time an object was touched by an object in a hand 
*/

namespace WidgetShowcase
{
		public class HandTouchTracker : MonoBehaviour
		{
		
				public LayerMask PartFilter; // masks which objects are considered valid tartgets. 

				public Dictionary<int, float> HandEnteredTimes = new Dictionary<int, float> ();
				public Dictionary<int, float> LastTouchTimes = new Dictionary<int, float> ();
				[SerializeField]
				float
						leaveMessageDelay = 0.25f;
		
				public float LeaveMessageDelay {
						get {
								return leaveMessageDelay;
						}
						set {
								leaveMessageDelay = Mathf.Max (0.1f, value);
						}
				}
		
				public event EventHandler<WidgetEventArg<HandTouchData>> HandleEnter;
				public event EventHandler<WidgetEventArg<HandTouchData>> HandleLeave;
	
#region loop

				void Start ()
				{
						//		HandleEnter += HandleEnterEcho;
						//		HandleLeave += HandleLeaveEcho;
				}
				// Update is called once per frame
				void Update ()
				{
						BroadcastLeaveMessages ();
				}
#endregion

				void HandleEnterEcho (object sender, WidgetEventArg<HandTouchData> e)
				{
//						Debug.Log ("Hand has entered: " + e.ToString ());
				}
		
				void HandleLeaveEcho (object sender, WidgetEventArg<HandTouchData> e)
				{
						//	Debug.Log ("Hand has left: " + e.ToString ());
				}

				void BroadcastLeaveMessages ()
				{
						float oldestTimeToKeep = Time.time - LeaveMessageDelay;
						List<int> oldIds = new List<int> ();

						foreach (int id in LastTouchTimes.Keys) {
								float time = LastTouchTimes [id];
								if (time < oldestTimeToKeep) {
										oldIds.Add (id);
								}
						}

						foreach (int id in oldIds) {
								EmitLeave (id);
								HandEnteredTimes.Remove (id);
								LastTouchTimes.Remove (id);
						}
				}

				void OnTriggerEnter (Collider coll)
				{
						OnTriggerStay (coll);
				}

				void OnTriggerStay (Collider coll)
				{
						if ((PartFilter.value & 1 << coll.gameObject.layer) == 1 << coll.gameObject.layer) {
								HandModel hm = coll.transform.gameObject.GetComponentInParent<HandModel> ();
								if (hm) {
										int id = hm.GetLeapHand ().Id;
										if (!HandEnteredTimes.ContainsKey (id)) {
												HandEnteredTimes [id] = Time.time;
												EmitEnter (id);
										}
										LastTouchTimes [id] = Time.time;
								}
						}
				}

		#region information emitter

				void EmitLeave (int id)
				{
						EventHandler<WidgetEventArg<HandTouchData>> handler = HandleLeave;
						if (handler != null) {
								handler (this, new WidgetEventArg<HandTouchData> (MakeHandTouchData (id)));
						}
				}

				HandTouchData MakeHandTouchData (int id)
				{
						return new HandTouchData (id, EntryTime (id), LastTouchTime (id));
				}

				float EntryTime (int id)
				{
						if (HandEnteredTimes.ContainsKey (id))
								return HandEnteredTimes [id];
						return 0;
				}

				float LastTouchTime (int id)
				{
						if (LastTouchTimes.ContainsKey (id))
								return LastTouchTimes [id];
						return 0;
				}

				void EmitEnter (int id)
				{
						EventHandler<WidgetEventArg<HandTouchData>> handler = HandleEnter;
						if (handler != null) {
								handler (this, new WidgetEventArg<HandTouchData> (MakeHandTouchData (id)));
						}
				}
#endregion

		}
}