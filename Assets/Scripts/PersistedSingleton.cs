using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Persists a singleton game object when included as a component script.
/// The object is expected to be a singleton with respect to its name.
/// </summary>
/// <remarks>
/// The persisted singleton can be defined in multiple scenes.
/// The first scene entered will define the object for all
/// subsequent scenes.
/// </remarks>
public class PersistedSingleton : MonoBehaviour
{
		// Singleton distinction via object names
		static Dictionary<string,GameObject> singletons;

		// Constructor ensures single instantiation of singletons
		// IMPORTANT: This method resolves singleton conflicts,
		// and must therefore be atomic.
		PersistedSingleton ()
		{
				if (singletons == null)
						singletons = new Dictionary<string,GameObject> ();
		}

		// Called once after construction of all objects
		// IMPORTANT: This method resolves singleton conflicts,
		// and must therefore be atomic.
		void Awake ()
		{
				GameObject preempt = null;
				if (!singletons.TryGetValue (this.gameObject.name, out preempt)) {
						singletons.Add (this.gameObject.name, this.gameObject);
						DontDestroyOnLoad (this.gameObject);
				} else if (preempt != this.gameObject) {
						Destroy (this.gameObject);
				}
		}
}
