using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
Filters/dectorates a bool emitter 
to only emit true or false. 
Note, the Always setting makes this class redundant -- 
only iincluded for completeness
*/

namespace WidgetShowcase
{
		public enum BoolWhen
		{
				IfFalse,
				IfTrue,
				Always
		}

		public class FIlterBoolEmitter : BoolEmitter
		{
				public BoolEmitter InputEmitter;

				public BoolWhen CopyWhen = BoolWhen.IfTrue;
		
				void Start ()
				{
						InputEmitter.BoolEvent += Handler;
						LastValue = BoolValue;
				}
		
				bool LastValue;

				void Handler (object sender, WidgetEventArg<bool> e)
				{
						if (DontEmitUnchangedValue && (LastValue == e.CurrentValue))
								return;
						switch (CopyWhen) {
						case BoolWhen.IfFalse:
								if (!e.CurrentValue)
										BoolValue = e.CurrentValue;
								break;

						case BoolWhen.IfTrue:
								if (e.CurrentValue)
										BoolValue = e.CurrentValue;
								break;

						case BoolWhen.Always:
								BoolValue = e.CurrentValue;
								break;
						default:
								throw new System.ArgumentOutOfRangeException ();
						}
				}
		
		}
	
}
