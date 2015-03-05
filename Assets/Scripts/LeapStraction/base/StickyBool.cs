using UnityEngine;
using System.Collections;

/**
This class handles Hysterisis behavor, but does not accept an on value below an off value. 
*/

namespace WidgetShowcase
{
		public class StickyBool: BoolEmitter
		{

#region float input
				public FloatEmitter FloatValueEmitter;
		
				void HandleFloatEvent (object sender, WidgetEventArg<float> e)
				{
						if (e.CurrentValue > OnValue)
								currentRegion.Change (Top);
						else if (e.CurrentValue < OffValue) {
								currentRegion.Change (Bottom);
						} else {
								currentRegion.Change (Middle);
						}
				}
#endregion

		#region range
				public float OnValue = 0.7f;
				public float OffValue = 0.3f;
		
				void InitRange ()
				{
						float min = Mathf.Min (OnValue, OffValue);
						float max = Mathf.Max (OnValue, OffValue);
						OnValue = max;
						OffValue = min;
				}
#endregion

		#region state
				public State currentRegion;
				const string Top = "top";
				const string Middle = "middle";
				const string Bottom = "bottom";
				const string Unknown = "unknown";
		
				void InitState ()
				{
						currentRegion = StateList.Init ("StickyBool", Unknown, Top, Middle, Bottom);
						currentRegion.StateChangedEvent += HandleStateChangedEvent;
				}
		
				void HandleStateChangedEvent (StateChange change)
				{
						if (change.unchanged)
								return;

						switch (change.state) {
						case Top:
								BoolValue = true;
								break;
				
						case Bottom: 
								BoolValue = false;
								break;

						case Middle: 
// ignored 
								break;
						default:
								Debug.Log ("!!! unknown state " + change.state);
								break;
						}
			
						// Debug.Log (string.Format ("==== StickyBool state: {0}, boolValue {1}", change.state, BoolValue));
				}

#endregion

				void Awake ()
				{
						InitRange ();
						FloatValueEmitter.FloatEvent += HandleFloatEvent;
						InitState ();
				}

				protected override void Start ()
				{
						base.Start ();
				}
		}
}
