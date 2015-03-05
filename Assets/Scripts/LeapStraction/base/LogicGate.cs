using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
Detects two rapid changes in a bool value in a short time period
ending with positive value; intended to track "Fist pumps"
note - this class only outputs true.
*/

namespace WidgetShowcase
{
		public enum LogicGateType
		{
				And,
				Or, 
				Merge
		}

		public class LogicGate : BoolEmitter
		{

				public List<BoolEmitter> InputEmitters = new List<BoolEmitter> ();
				public LogicGateType LogicType = LogicGateType.And;

				void Start ()
				{
						foreach (BoolEmitter bem in InputEmitters) {
								bem.BoolEvent += Handler;
						}
				}

/**
 * note - we are polling the current state of all inputs, so the value of the changed one
* is not (specifically) used.
*/
				void Handler (object sender, WidgetEventArg<bool> e)
				{
						switch (LogicType) {
						case LogicGateType.And:
								PollAnd ();
								break;
						case LogicGateType.Or:
								PollOr ();
								break;
						case LogicGateType.Merge:
								BoolValue = e.CurrentValue;
								break;
						default:
								throw new System.ArgumentOutOfRangeException ();
						}
				}

				void PollOr ()
				{
						foreach (BoolEmitter bem in InputEmitters) {
								if (bem.BoolValue) {
										BoolValue = true;
										return;
								}
						}
						BoolValue = false;
				}
		
				void PollAnd ()
				{
						foreach (BoolEmitter bem in InputEmitters) {
								if (!bem.BoolValue) {					
										BoolValue = false;
										return;
								}
						}
						BoolValue = true;
				}
		}

}
