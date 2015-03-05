using UnityEngine;
using System.Collections;
using System;

namespace WidgetShowcase
{

		/// <summary>
		/// A record of a change from one state to another
		/// </summary>
		public struct StateChange
		{

				public StateListItem fromState;
				public StateListItem toState;
				public StateList list;
				public bool allowed;

				public string state { get { return toState.name; } }

				public bool unchanged { get { return fromState.name == toState.name; } }

				public  StateChange (StateListItem fromS, StateListItem toS, StateList li, bool a)
				{
						if (fromS.name == "" || toS.name == "") throw new Exception("cannot change to empty states");
						fromState = fromS;
						toState = toS;
						list = li;
						allowed = a;
				}

				public override string ToString ()
				{
						return string.Format (" [change]  {0} ... {1}: {2}{3}", 
											  fromState.name,
											  toState.name, 
											  (allowed ? "" : " (not allowed)"),
											  (unchanged ? " (unchanged)" : "")
						                     );
				}

		}

}