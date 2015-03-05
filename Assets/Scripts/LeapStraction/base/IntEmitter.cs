using UnityEngine;
using System;
using System.Collections;

namespace WidgetShowcase
{
		public class IntEmitter : DataEmitter, IIntEmitter
		{
		
				public virtual event EventHandler<WidgetEventArg<int>> IntEvent;
		
				private int intValue;
		
				public int IntValue {
						get {
								return intValue;
						}
						set {
								if (DontEmitUnchangedValue && (value == intValue))
										return;
								intValue = value;
								EventHandler<WidgetEventArg<int>> handler = IntEvent;
								if (handler != null) {
										handler (this, new WidgetEventArg<int> (value));
								}
						}
				}
		
		}
	
}
