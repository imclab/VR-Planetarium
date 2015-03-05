using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelCursor : MonoBehaviour
{
		public const float Y_EXTENT = 0.18f;
		public const float X_EXTENT = -0.36f;

#region main loop

		// Use this for initialization
		void Start ()
		{
	
		}
	
		float maxlat = 0;
		float minlat = 0;
		float maxlon = 0;
		float minlon = 0;

		// Update is called once per frame
		void Update ()
		{
				Vector3 p = rectTransform.localPosition;

				float y = Y_EXTENT * (latitude - 90) / ((float)90);

				float x = X_EXTENT * (longitude) / ((float)180);
				p.y = y;
				p.x = x;
		
//				Debug.Log (string.Format ("lat: {0}. lon: {1}; x: {2}, y: {3}", Mathf.Round (latitude), Mathf.Round (longitude), x, y)); 
				rectTransform.localPosition = Vector3.Lerp (rectTransform.localPosition, p, 0.05f);

				maxlat = Mathf.Max (latitude, maxlat);
				minlat = Mathf.Min (latitude, minlat);
				maxlon = Mathf.Max (maxlon, longitude);
				minlon = Mathf.Min (minlon, longitude);
			//	Debug.Log (string.Format ("Lat from {0} .. {1}. Lon from {2} to {3}.", minlat, maxlat, minlon, maxlon)); 
		}

#endregion

		RectTransform rectTransform {
				get { return GetComponent<RectTransform> (); }
		}

		float latitude {
				get { 
						if (!TimeAndLocationHandler.Instance)
								return 0;
						return TimeAndLocationHandler.Instance.Latitude;
				}
		}

		float longitude {
				get {
						if (!TimeAndLocationHandler.Instance)
								return 0; 
						return TimeAndLocationHandler.Instance.Longitude;
				}
		} 
}
