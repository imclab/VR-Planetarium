using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/**
Note - MM is NOT a Mono derivation - does not need to be instantiated.
there is a single instance as as convenience but local modalities can be created as well. 
*/

namespace WidgetShowcase
{
		public class ModalityManager
		{
				static ModalityManager instance = new ModalityManager ();

				public static ModalityManager Instance {
						get {
								return instance;
						}
				}

				Dictionary<string, GameObject> subscribers = new Dictionary<string, GameObject> ();
				string activated = "";

				public string ActiveItemName {
						get {
								return activated;
						}
						set {
								if (value == null)
										value = "";
								//Debug.Log (string.Format ("  -----Activation string set to {0}", value));
								activated = value;
						}
				}

				public  ModalityManager ()
				{

				}

				public void Subscribe (string itemname, GameObject target)
				{
						if (!subscribers.ContainsKey (itemname))
								subscribers.Add (itemname, target);
				}

				public void Subscribe (GameObject target)
				{
						Subscribe (target.GetInstanceID ().ToString (), target);
				}
		
				public void Unsubscribe (GameObject target)
				{
						if (target == null)
								throw new ArgumentNullException ("target");
						Unsubscribe (target.GetInstanceID ().ToString ());
				}
		
				public void Unsubscribe (string itemName)
				{
						if (subscribers.ContainsKey (itemName)) {
								if (ActiveItemName == itemName)
										Deactivate (itemName);
								subscribers.Remove (itemName);
						}
				}

/**
 * Attempt to activate an item. 
 * If it fails, "Failed Activation" will be broadcast back to the target. 
 * If it fails (or there is no registered target by the given name) returns false.
 * else, returns true. 
*/

				public enum ModalityMessage
				{
						Locked, // tells subscribers that another item has locked modality
						Accepted, // tells subscribers that their activation attempt was successful
						Deactivated, // feedback that your deactivation signal was successful.
						Failed, // tells subscribers who attempt to lock modality that their attempt failed
						Open // tells items that the modality can accept subscriptions
				}

				public bool HasActiveItem ()
				{
						if (ActiveItemName != null && ActiveItemName != "") {
								return true;
						}
						return false;
				}

				public bool CanActivate (GameObject target)
				{
						return CanActivate (target.GetInstanceID ().ToString ());
				}

				public bool CanActivate (string itemName)
				{
						if (!subscribers.ContainsKey (itemName)) {
								return false;
						}

						if ((ActiveItemName != "") && (ActiveItemName != null)) {
								return false;
						}

						return true;
				}

				public bool Activate (GameObject target)
				{
						return Activate (target.GetInstanceID ().ToString ());
				}

				public bool Activate (string itemName)
				{
						if (!subscribers.ContainsKey (itemName)) {
//								Debug.Log (string.Format ("WE DON'T HAVE A {0}; cannot activate", itemName));
								return false;
						}

						if ((ActiveItemName != "") && (ActiveItemName != null)) {
								//Debug.Log ("Activation Block!");
								Call (itemName, ModalityMessage.Failed);
								return false;
						}

						ActiveItemName = itemName;

						foreach (string subKey in subscribers.Keys) {
								if (subKey != itemName) {
//										Debug.Log ("activation is deactivating " + subKey);
										Call (subKey, ModalityMessage.Locked);
								}
						}
						Call (itemName, ModalityMessage.Accepted);

						return true;
				}
		
				public bool Deactivate (GameObject target)
				{
						return Deactivate (target.GetInstanceID ().ToString ());
				}
	
				public bool Deactivate (string itemName)
				{
						if ((!subscribers.ContainsKey (itemName)) || (ActiveItemName != itemName)) {
								return false;
						}
						Call (itemName, ModalityMessage.Deactivated);

						ActiveItemName = "";
						return true;
				}

				void Call (string itemName, ModalityMessage command)
				{
						if (subscribers [itemName])
								subscribers [itemName].BroadcastMessage ("Modality", command, SendMessageOptions.DontRequireReceiver);
				}
		}
	
}