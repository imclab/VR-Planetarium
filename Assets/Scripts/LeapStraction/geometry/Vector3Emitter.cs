using UnityEngine;
using System;
using System.Collections;

namespace WidgetShowcase
{
		public class Vector3Emitter : DataEmitter
		{

				public virtual event EventHandler<WidgetEventArg<Vector3>> Vector3Event;

				public Vector3 vector3Value;

				public Vector3 Vector3Value {
						get {
								return vector3Value;
						}
						set {
								if (DontEmitUnchangedValue && vector3Value == value)
										return;
								Vector3Value = value;
								EventHandler<WidgetEventArg<Vector3>> handler = Vector3Event;
								if (handler != null) {
										handler (this, new WidgetEventArg<Vector3> (value));
								}
						}
				}

		}

}