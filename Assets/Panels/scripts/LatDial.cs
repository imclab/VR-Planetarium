using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;

namespace WidgetShowcase
{
		public class LatDial : MonoBehaviour
		{

				public Text feedback;
				public DialBase dial;

				// Use this for initialization
				void Start ()
				{
	
				}

				public float Angle {
						get {
								return  Mathf.Clamp (dial.GetAngle () / 4, -90, 90);
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (Angle > 0) {
								feedback.text = Mathf.RoundToInt (Angle) + "°N";
						} else {
								feedback.text = Mathf.RoundToInt (-Angle) + "°S";
						}
			
						if (TimeAndLocationHandler.Instance)
								TimeAndLocationHandler.Instance.Latitude = Angle;
				}
	
		}
}