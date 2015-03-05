using UnityEngine;
using System;
using System.Collections;

namespace WidgetShowcase
{
		public class CollissionEmitter : DataEmitter, IBoolEmitter
		{

#region loop
				void Start ()
				{
						DontEmitUnchangedValue = true;
				}
#endregion

				public GameObjectValidator TargetValidator = null;

				void OnCollisionEnter (Collision collision)
				{
						if (TargetValidator != null && !TargetValidator.Test (collision.gameObject))
								return;

						BoolValue = true;
				}

				public virtual event EventHandler<WidgetEventArg<bool>> BoolEvent;

				public bool boolValue;

				public bool BoolValue {
						get {
								return boolValue;
						}
						set {
								if (DontEmitUnchangedValue && boolValue == value)
										return;
								boolValue = value;
								EventHandler<WidgetEventArg<bool>> handler = BoolEvent;
								if (handler != null) {
										handler (this, new WidgetEventArg<bool> (value));
								}
						}
				}

		}

}