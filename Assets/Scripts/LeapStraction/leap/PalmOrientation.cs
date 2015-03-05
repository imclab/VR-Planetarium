using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{ 
		public class PalmOrientation : FloatEmitter
		{

				public HandEmitter InputEmitter;
				public GameObject upTarget;

				// Use this for initialization
				void Start ()
				{
						InputEmitter.HandEvent += HandleHandEvent;
				}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						if (e.CurrentValue.HasHand) {
								HandModel hand = e.CurrentValue.HandModel;
								Vector3 pivot = hand.GetPalmPosition ();
								Vector3 normal = pivot + hand.GetPalmNormal ();
								Vector3 axis = upTarget.transform.position;			
								FloatValue = Vector3.Dot ((normal - pivot).normalized, (axis - pivot).normalized);
						} else {
								FloatValue = -1;
						}
				}
		}
}
