using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;

namespace WidgetShowcase
{
		public class LonDial : MonoBehaviour
		{

				public Text feedback;
				public DialBase dial;

				// Use this for initialization
				void Start ()
				{
	
				}

				public float Angle {
						get {
								return  Mathf.Clamp (dial.GetAngle () / 2, -180, 180);
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (Angle > 0) {
								feedback.text = Mathf.RoundToInt (Angle) + "°W";
						} else {
								feedback.text = Mathf.RoundToInt (-Angle) + "°E";
						}
						if (TimeAndLocationHandler.Instance)
								TimeAndLocationHandler.Instance.Longitude = Angle;
				}
	
		}
}