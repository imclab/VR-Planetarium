using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/**
 a test scaffold to observe handpumping behavior
*/

namespace WidgetShowcase
{
		public class HandPumpListener : MonoBehaviour
		{
				public HandEmitter HandEmitter;
				public StickyBool PumpedEmitter;
				public HandToGripStrength Strength;
				public Text HandEmitterFeedback;
				public Text FistPumpFeedback;
	public Text StrengthFeedback;
				int fistPumpedTrue = 0;
				int fistPumpedFalse = 0;

				// Use this for initialization
				void Awake ()
				{
						HandEmitter.HandEvent += HandleHandEvent;
						PumpedEmitter.BoolEvent += HandlePumpEvent;
						Strength.FloatEvent += HandleStrengthEvent;
				}

		void HandleStrengthEvent (object sender, WidgetEventArg<float> e)
		{
			StrengthFeedback.text = Mathf.RoundToInt(100 * e.CurrentValue).ToString();
		}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						HandEmitterFeedback.text = e.CurrentValue.HasHand ? string.Format ("Hand ID " + e.CurrentValue.Id) : "-- no hand --";
				}

				void HandlePumpEvent (object sender, WidgetEventArg<bool> e)
				{
						if (e.CurrentValue)
								++fistPumpedTrue;
						else
								++fistPumpedFalse;
						FistPumpFeedback.text = string.Format ("Fist Pumped: {0} ({1} true, {2} false)", 
							e.CurrentValue ? "T" : "F", fistPumpedTrue, fistPumpedFalse);
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}
		}

}