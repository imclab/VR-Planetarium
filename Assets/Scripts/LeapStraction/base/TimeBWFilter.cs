using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 returns the best/worst value of a certain number of events in the history stack.
*/

namespace WidgetShowcase
{
		public class TimeBWFilter : BoolEmitter
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

				public List<bool> history = new List<bool> ();
				public TimeFilter Input;
				public mode Mode = mode.Pessimist;
				public bool WaitForFullHistory = false;

				void Awake ()
				{
						Input.IntEvent += HandleIntEvent;
				}

				void HandleIntEvent (object sender, WidgetEventArg<int> e)
				{
						history.Add (e.CurrentValue == 0);
						while (history.Count > HistoryLength)
								history.RemoveAt (0);

						if (WaitForFullHistory && (history.Count < MinValues))
								return;

						string log = "";
						bool boolResult = false;

						if (Mode == mode.Optimist) {

								foreach (bool b in history) {
										boolResult = boolResult || b;
										log += ", " + (b ? "T" : "F");
								}

						} else {
								boolResult = true;
								foreach (bool b in history) {
										boolResult = boolResult && b;
										log += ", " + (b ? "T" : "F");
								}
						}

					//	Debug.Log (string.Format ("TimeBWfilter: result = {0}, mode = {1} (history {2})", boolResult, Mode, log));
						BoolValue = boolResult;
				}
			
		}
}