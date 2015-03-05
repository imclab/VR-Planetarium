using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class HandToPalmPoint : PointEmitter
		{
				public HandEmitter HandEmitter;

				// Use this for initialization
				void Start ()
				{
						HandEmitter.HandEvent += HandleHandEvent;
				}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						if (e.CurrentValue.HasHand) {
								PointValue = new PointData (e.CurrentValue.HandModel.GetPalmPosition ());
						} else {
								pointValue = new PointData (false);
						}
				}
		}
}