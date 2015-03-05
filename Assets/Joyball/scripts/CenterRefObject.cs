using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class CenterRefObject : MonoBehaviour
		{
				public HandEmitter InputHand;
				public BoolEmitter InputActive;
				public GameObject ReferenceCenter;
				HandData data = new HandData(null);

				// Use this for initialization
				void Start ()
				{
						InputHand.HandEvent += HandleHandEvent;
						InputActive.BoolEvent += ActivationHandler;
				}

				void ActivationHandler (object sender, WidgetEventArg<bool> e)
				{
						if (e.CurrentValue) {
								ReferenceCenter.SetActive (true);
								if (data.HasHand)
										ReferenceCenter.transform.position = data.HandModel.GetPalmPosition ();;
										ReferenceCenter.transform.rotation = data.HandModel.GetPalmRotation ();
						} else {
								ReferenceCenter.transform.localPosition = new Vector3 (0f, 0f, 0f);
								ReferenceCenter.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
								ReferenceCenter.SetActive (false);
						}
				}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						if (e.CurrentValue.HasHand)
								data = e.CurrentValue;
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}
		}
	
}