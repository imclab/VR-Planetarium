using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{ 
		public class HandToGripStrength : FloatEmitter
		{

				public HandEmitter InputEmitter;
				// Use this for initialization
				void Start ()
				{
						InputEmitter.HandEvent += HandleHandEvent;
				}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						if (e.CurrentValue.HasHand) {
							//	Debug.Log (string.Format ("Grip Strength = {0}", e.CurrentValue.LeapHand.GrabStrength));
								FloatValue = e.CurrentValue.LeapHand.GrabStrength;
						}
				}
		}
}