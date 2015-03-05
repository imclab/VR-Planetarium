using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WidgetShowcase
{
		public class NewPanelCursor : PointEmitter
		{

				public PointConstrainer TouchPanel;
				public GameObject BottomCorner;
				public GameObject TopCorner;
				Vector3 Extent;
				Vector3 TopPosition;
				Vector3 BottomPosition;
				public Text LatLabel;
				public Text LonLabel;
				const string DEG = "°";
		
				string LonString (float longitude)
				{
						return Mathf.RoundToInt (Mathf.Abs (longitude)) + DEG + (longitude >= 0 ? "E" : "W");
				}
		
				string LatString (float latitude)
				{
						return Mathf.RoundToInt (Mathf.Abs (latitude)) + DEG + (latitude >= 0 ? "N" : "S");
				}		
		
				// Use this for initialization
				void Start ()
				{
						BottomPosition = BottomCorner.transform.localPosition;
						TopPosition = TopCorner.transform.localPosition;
						if (System.Math.Abs (BottomPosition.z - TopPosition.z) < 0.001f)
								TopPosition.z += 0.01f;
						Extent = (TopPosition - BottomPosition);
				}
	
				// Update is called once per frame
				void Update ()
				{
						ComputeRelativePosition ();
				}

				void ComputeRelativePosition ()
				{

						Vector3 local = transform.localPosition;
						local -= BottomPosition;
						local.x /= Extent.x;
						local.y /= Extent.y;
						local.z /= Extent.z;

						//Debug.Log (string.Format ("(x 10) Local: {0}, top: {1}, bottom: {2}, XYZ: {3}", transform.localPosition * 10, TopPosition * 10, BottomPosition * 10, local * 10));
						// NOTE: In order to avoid coordinate singularities, set all acoordinates simultaneously.
						Vector3 AziLatLon = TimeAndLocationHandler.Instance.AziLatLon;
						AziLatLon [0] = 0f;
						AziLatLon [1] = Mathf.Clamp ((local.y - 0.5f) * 180f, -90f, 90f);
						AziLatLon [2] = Mathf.Clamp ((local.x - 0.5f) * 360f, -180f, 180f);
						TimeAndLocationHandler.Instance.AziLatLon = AziLatLon;

						if (LatLabel)
							LatLabel.text = LatString (AziLatLon [1]);
						if (LonLabel)
							LonLabel.text = LonString (AziLatLon [2]);
		}
	}
	
}