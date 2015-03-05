using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WidgetShowcase
{
		public class StateList
		{

#region properties
				static Dictionary<string, StateList> lists = new Dictionary<string, StateList> ();
				public string name;
				public StateListItem[] items;
				public Dictionary<StateListItem, StateListItem[]> controlledStateChanges = new Dictionary<StateListItem, StateListItem[]> ();
		
		#endregion

#region constructors
		
				public StateList (string n, params string[] i)
				{
						name = n;
						List<StateListItem> iList = new List<StateListItem> ();
						foreach (string ii in i) {
								iList.Add (new StateListItem (ii, this));
						}
						items = iList.ToArray ();
						lists.Add (n, this);
				}

				public static StateList Create (string n, params string[] i)
				{
						return new StateList (n, i);
				}

				public static State Init (string name, params string[] states)
				{
						if (!HasList (name)) {
								Create (name, states);
						}
						return new State (name, states [0]);
				}
		#endregion

#region list management
		
				public static bool HasList (string name)
				{
						return lists.ContainsKey (name);
				}

				public static void RemoveState (string name)
				{
						if (lists.ContainsKey (name))
								lists.Remove (name);
				}
		
				public static StateList GetList (string name)
				{
						return lists [name];
				}

/// <summary>
/// Removes all predefined lists from the state registry; probably only need to do this in a test context.
/// </summary>
				public static void Clear ()
				{
						lists.Clear ();
				}
		#endregion

#region accessors
// returns a stored list of states

				public StateListItem Item (string name)
				{
						foreach (StateListItem item in items) {
								if (item.Equals (name))
										return item;
						}
						return null;
				}

				public bool Contains (string name)
				{
						return Item (name) != null;
				}

				public StateListItem First ()
				{
						return items [0];
				}

				public void Constrain (string toName, params string[] fromNames)
				{
						List<StateListItem> fromItems = new List<StateListItem> ();
						StateListItem toItem = null;
						foreach (StateListItem nameItem in items) {
								if (nameItem.Equals (toName)) {
										toItem = (nameItem);
								}
						}
						if (toItem == null)
								throw new UnityException ("Cannot find item " + toName);

						foreach (string name in fromNames) {
								foreach (StateListItem nameItem in items) {
										if (nameItem.Equals (name)) {
												fromItems.Add (nameItem);
										}
								}
						}
						controlledStateChanges.Add (toItem, fromItems.ToArray ());

				}

		#endregion
		}

}