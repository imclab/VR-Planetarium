using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 returns the best/worst value of a certain number of events in the history stack.
*/

namespace WidgetShowcase
{
		public class BestWorstFilter : BoolEmitter
		{

				public enum mode
				{
						Optimist,
						Pessimist
				}

				public int HistoryLength = 2;

				int MinValues {
						get { 
								return Mathf.Max (2, HistoryLength);
						}
				}

				List<bool> history = new List<bool> ();
				public BoolEmitter Input;
				public mode Mode = mode.Pessimist;
				public bool WaitForFullHistory = false;

				void Awake ()
				{
						Input.BoolEvent += HandleBoolEvent;
				}

				void HandleBoolEvent (object sender, WidgetEventArg<bool> e)
				{
						history.Add (e.CurrentValue);
						while (history.Count > HistoryLength)
								history.RemoveAt (0);

						if (WaitForFullHistory && (history.Count < MinValues))
								return;

						if (Mode == mode.Optimist) {
								foreach (bool b in history) {
										if (b) {
												BoolValue = true;
												return;
										}
								}
								BoolValue = false;
						} else {
								foreach (bool b in history) {
										if (!b) {
												BoolValue = false;
												return;
										}
								}
								BoolValue = true;
						}

				}
			
		}
}