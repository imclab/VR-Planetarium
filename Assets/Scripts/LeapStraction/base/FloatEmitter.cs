using UnityEngine;
using System;
using System.Collections;

namespace WidgetShowcase
{
		public class FloatEmitter : DataEmitter
		{
		
				public virtual event EventHandler<WidgetEventArg<float>> FloatEvent;
		
				private float floatValue;
		
				public float FloatValue {
						get {
								return floatValue;
						}
						set {
								if (DontEmitUnchangedValue && (value == floatValue))
										return;
								floatValue = value;
								EventHandler<WidgetEventArg<float>> handler = FloatEvent;
								if (handler != null) {
										handler (this, new WidgetEventArg<float> (value));
								}
						}
				}
		
		}
	
}
