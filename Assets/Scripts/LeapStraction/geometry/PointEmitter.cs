using UnityEngine;
using System;
using System.Collections;

/**
This is an emitter that emits Vector3 data. It is encased in Points to handle the eventuality
that the provider cannot provide data; because structs cannot themselves be null, the null
possibility is handled by the PointData's HasData boolean.
*/

namespace WidgetShowcase
{
		public class PointEmitter : DataEmitter
		{

				public virtual event EventHandler<WidgetEventArg<PointData>> PointEvent;
		
				public PointData pointValue;

				public virtual PointData PointValue {
						get {
								return pointValue;
						}
						set {
								if (DontEmitUnchangedValue && pointValue == (value))
										return;
								pointValue = value;
								EmitValue (pointValue);
						}
				}

				protected void EmitValue (PointData pointValue)
				{
						EventHandler<WidgetEventArg<PointData>> handler = PointEvent;
						if (handler != null) {
								handler (this, new WidgetEventArg<PointData> (pointValue));
						}
				}
		}

}