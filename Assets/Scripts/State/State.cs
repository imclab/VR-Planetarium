using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WidgetShowcase
{
		public class State
		{

				public StateList list;
				public StateListItem myState;

				public string state { get { return myState == null ? "" : myState.name; } set { Change (value); } }

				public State (string name)
				{
						list = StateList.GetList (name);
						myState = list.First ();
				}

				public State (string name, string itemName)
				{
						list = StateList.GetList (name);
						myState = list.Item (itemName);
				}
		
				public bool Change (string name, bool force)
				{
						if (force) {
								if (list.Contains (name)) {
										myState = list.Item (name);
										return true;
								} else {
										return false;
								}
						} else {
								return	Change (name);
						}
				}

				public bool Change (string name)
				{
						foreach (StateListItem item in list.items) {
								if (item.Equals (name)) {

// if there are controls over a given StateItem, enforce them
										if (list.controlledStateChanges.ContainsKey (item)) {
						
//												Debug.Log ("--- enforcing change limits for changing to " + item.ToString ());
												foreach (StateListItem allowedItem in list.controlledStateChanges[item]) {
														//	Debug.Log ("... can change from state " + allowedItem.ToString ());
														if (allowedItem.Equals (myState)) {
																//Debug.Log (".... and that is what we are trying to do!");
																StateChanged (myState, item);
																myState = item;
																return true;					
														}
												}
												StateChanged (myState, item, false);
												return false;
										}

										StateChanged (myState, item);
										myState = item;
										return true;
								}

						}
						return false;
				}
		
		#region Events

				public delegate void StateChangedDelegate (StateChange change);
		
				/// <summary>An event that gets fired </summary>
				public event StateChangedDelegate StateChangedEvent;

				public void StateChanged (StateListItem fromItem, StateListItem toItem, bool allowed = true)
				{
						if (StateChangedEvent != null) // fire the event
								StateChangedEvent (new StateChange (fromItem, toItem, list, allowed));
				}

		#endregion
		
		}
}