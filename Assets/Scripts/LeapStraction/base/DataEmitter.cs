using UnityEngine;
using System;
using System.Collections;

/**
 * this root class for emitters is actually a good root class for anything
you want to inline document in a way that is visible to the user. 
*/

namespace WidgetShowcase
{
		public abstract class DataEmitter : MonoBehaviour
		{
				public string Notes; // public documentation
				public bool DontEmitUnchangedValue = false; // only emit changes; must be implemented in local classes
				public bool EmitOnStart = false; // emit value on startup. NOTE: this is not implemented across most classes!
		}


}