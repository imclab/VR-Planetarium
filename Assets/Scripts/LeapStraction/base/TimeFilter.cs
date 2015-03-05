using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 

reflects the pace at which the input is changing; 
outputs an integer based on one of the four constants.
Given that the first event doesn't have a previous time, 
FirstEventResult is automatically output
upon receipt of the first input. 

*/

namespace WidgetShowcase
{
		public class TimeFilter : IntEmitter
		{

// the range of acceptable time between events
				public float MinSpeed = 0;
				public float MaxSpeed = 1;
				public bool ignoreUnchangedInput; 
// if true, will take  time measurement only if the input is different
// from the output; if false, will take a time input whenever the emitter sends a message

				float lastTime = -1;

				public float LastTime {
						get {
								return lastTime;
						}
						set {
								lastTime = value;
//								Debug.Log (string.Format ("TimeFilter time set to {0}", lastTime));
						}
				}

				bool lastEvent = false;
				public BoolEmitter Input;
				public int FirstEventResult = SQ_UNKNOWN; 
// in the first event do we assume we are or are not
// within the time gate, since we don't have more than one data point?
				public const int SQ_UNKNOWN = -2;
				public const int SQ_TOO_SLOW = -1;
				public const int SQ_GOOD = 0;
				public const int SQ_TOO_FAST = 1;

				void Start ()
				{
						///@TODO: register listening?
						Input.BoolEvent += HandleBoolEvent;
				}

				bool handling = false;

				void HandleBoolEvent (object sender, WidgetEventArg<bool> e)
				{
						if (handling)
								return;
						handling = true;
						if (System.Math.Abs (LastTime - -1) < 0.01f) {
								IntValue = FirstEventResult;
								//Debug.Log (string.Format ("TimeFilter Default value at start: {0}", FirstEventResult));
						} else {
								float delta = (Time.time - LastTime);
								if (delta < MinSpeed) {
										IntValue = SQ_TOO_SLOW;
								} else if (delta > MaxSpeed) {
										IntValue = SQ_TOO_FAST;
								} else
										IntValue = SQ_GOOD;
							//	Debug.Log (string.Format ("Setting TimeGate value to {0} based on delta of {1} (desired {2}..{3})", IntValue, delta, MinSpeed, MaxSpeed));
						}
						LastTime = Time.time;
						handling = false;
				}

		}

}