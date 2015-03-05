using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class BoolActivator : BoolEmitter
		{
				public BoolEmitter InputBoolEmitter;
				public GameObject[] targets;

				// Use this for initialization
				void Awake ()
				{
						InputBoolEmitter.BoolEvent += OnBoolChanged;
				}

				void OnBoolChanged (object sender, WidgetEventArg<bool> e)
				{
						if (targets != null) {
//								Debug.Log (string.Format ("Setting {0} active to {1}", target.name, e.CurrentValue));
								ChangeVisibility (e.CurrentValue);
						}
				}
	
				// Update is called once per frame
				void Start ()
				{
						if (EmitOnStart)
								ChangeVisibility (BoolValue);
				}

				void ChangeVisibility (bool value)
				{
						BoolValue = value;
						foreach (GameObject target in targets) {
								// Debug.Log (string.Format ("Setting visibility of {0} to {1}", target.name, BoolValue.ToString ()));
//								target.SetActive (BoolValue);
						}
				}
		}
	
}