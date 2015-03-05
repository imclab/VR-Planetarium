using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
This class alternates between returning true and returning false. 
When it emits is dependent on the activation mode. 
*/

namespace WidgetShowcase
{
		public class Toggle : BoolEmitter
		{

				public enum ToggleOn
				{
						OnTrue,
						OnFalse,
						OnAny
				}
				public BoolEmitter InputEmitter;
				public ToggleOn ActivationMode = ToggleOn.OnTrue;

				void Awake ()
				{
						InputEmitter.BoolEvent += HandleBoolEvent;
				}

				void HandleBoolEvent (object sender, WidgetEventArg<bool> e)
				{				
						switch (ActivationMode) {
						case ToggleOn.OnAny:
// always continue
								break;

						case ToggleOn.OnFalse:
								if (e.CurrentValue)
										return;
								break;

						case ToggleOn.OnTrue: 
								if (!e.CurrentValue)
										return;
								break;
						}

						BoolValue = !BoolValue;
// Debug.Log (string.Format ("... Toggle set to {0}", BoolValue));
				}

		}
}