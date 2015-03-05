using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class RayCastThroughPointEmitter : RayCastEmitter
		{

				public PointEmitter targetEmitter;

				// Use this for initialization
				void Start ()
				{
						targetEmitter.PointEvent += HandlePointEvent;
				}

				void HandlePointEvent (object sender, WidgetEventArg<PointData> e)
				{
						if (e.CurrentValue.HasData) {
								BoolValue = RayCheck (e.CurrentValue.Point);
						}
				}

				void Update ()
				{

				}
		}

}