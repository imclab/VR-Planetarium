using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LMWidgets;
using Leap;

namespace WidgetShowcase
{
		public class TouchMap : MonoBehaviour
		{

				public HandEmitter TouchingHand;
				public IntEmitter TargetHandId;
				const string HCS_HAS_HAND = "has hand";
				const string HCS_NOHAND = "no Hand";
				public HandTouchTracker TouchTracker;
				State HandContactState;
				Vector3 handFirstPosition;
				Vector3 CursorFirstPosition;
				public float CursorMovementLerp = 0.01f;
				public PointEmitter CursorPositionFilter;
				List<Vector3> FingerTipPositions = new List<Vector3> ();

				public Vector3 HandFirstPosition {
						get {
								return handFirstPosition;
						}
						set {
								handFirstPosition = value;
								cursorSet = true;
						}
				}

				public GameObject Cursor;
				bool cursorSet = false;
				bool handlerSet = false;
				// Use this for initialization
		#region loop
				void Start ()
				{

						if (!TouchTracker)
								TouchTracker = GetComponent<HandTouchTracker> ();

						TouchTracker.HandleEnter += OnHandEnter;
						TouchTracker.HandleLeave += OnHandLeave;

						HandContactState = StateList.Init ("TouchMap HandContactState", HCS_HAS_HAND, HCS_NOHAND);
						HandContactState.Change (HCS_NOHAND);
						HandContactState.StateChangedEvent += HandleHandContactStateChangedEvent;

						TouchingHand.HandEvent += HandleHandEvent;
						CursorFirstPosition = Cursor.transform.position;
						handlerSet = TryCPF ();
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (!handlerSet)
								handlerSet = TryCPF ();
				}
		#endregion

				bool TryCPF ()
				{
						if (handlerSet)
								return true;

						if (!CursorPositionFilter)
								return false;
						CursorPositionFilter.PointEvent += HandlePointEvent;
						return true;
				}

				void HandlePointEvent (object sender, WidgetEventArg<PointData> e)
				{
						if (Time.time < 1)
								return;
						if (e.CurrentValue.HasData) {	
								//Debug.Log ("Cursor moved to " + e.CurrentValue.Point);
								Cursor.transform.position = e.CurrentValue.Point;

						}
				}

				void HandleHandContactStateChangedEvent (StateChange change)
				{
						cursorSet = false;
						FingerTipPositions.Clear ();
				}

		#region TouchTracker response

				void OnHandLeave (object sender, WidgetEventArg<HandTouchData> e)
				{
						if (HandContactState.state == HCS_HAS_HAND) {
								if (TargetHandId.IntValue == e.CurrentValue.ID) {
										TargetHandId.IntValue = -1;
										HandContactState.Change (HCS_NOHAND);
								}
						}
				}

				void OnHandEnter (object sender, WidgetEventArg<HandTouchData> e)
				{
						if (Time.time < 1)
								return;
						if (HandContactState.state == HCS_NOHAND) {
								TargetHandId.IntValue = e.CurrentValue.ID;
								CursorFirstPosition = Cursor.transform.position;
								HandContactState.Change (HCS_HAS_HAND);
						}
				}
#endregion

				bool IsCurrentHand (HandData data)
				{
						return HandContactState.state == HCS_HAS_HAND && data.HasHand && data.LeapHand.Id == TargetHandId.IntValue;
				}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						if (IsCurrentHand (e.CurrentValue)) {
								//Vector3 palmPos = e.CurrentValue.HandModel.GetPalmPosition ();
								Vector3 fingerTipPosition = e.CurrentValue.HandModel.fingers [1].GetTipPosition ();
								if (!cursorSet) {
										HandFirstPosition = fingerTipPosition;
										CursorFirstPosition = Cursor.transform.position;
								} else {
// the cursor is moved, indirectly, by the position filter, which takes the desired input as a prompt
										//	Debug.Log ("Pushing cursor to be relative to " + e.CurrentValue.LeapHand.Id);
										Vector3 toPos = CursorFirstPosition + (fingerTipPosition - CursorFirstPosition);
										CursorPositionFilter.PointValue = new PointData (toPos);
								}

						}
				}
	
		}

}