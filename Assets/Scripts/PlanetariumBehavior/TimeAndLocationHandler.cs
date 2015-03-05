using UnityEngine;
using System;
using System.Collections;

public class TimeAndLocationHandler : MonoBehaviour
{
		public static TimeAndLocationHandler Instance;
		[SerializeField]
		float
				m_latLongStep;
		[SerializeField] 
		GameObject
				m_orbiterRoot;
		[SerializeField]
		GameObject
				m_earth;
		[SerializeField]
		GameObject
				m_viewer;
		[SerializeField]
		float
				m_earthRadius = 45.0f;
		public float timeScale = 1f;
		// Persisted Data
		private DateTime m_dateTime;

		public DateTime DateAndTime { get { return m_dateTime; } set { m_dateTime = value; } }

		public float Azimuth {
				get { return AziLatLon [0]; }
				set { 
						Vector3 all = AziLatLon;
						all [0] = value;
						AziLatLon = all;
				}
		}

//public float Latitude = 0;
//public float Longitude = 0;


		public float Latitude { 
				get { return AziLatLon [1]; }
				set { 
						Vector3 all = AziLatLon;
						all [1] = value;
						AziLatLon = all;
				}
		}

		public float Longitude {
				get { return AziLatLon [2]; }
				set { 
						Vector3 all = AziLatLon;
						all [2] = value;
						AziLatLon = all;
				}
		} 

		/// <summary>
		/// Positions & orients the viewer & earth by Azimuth, Latitude & Longitude.
		/// This method moves heaven & earth to make the viewer the center of the universe!
		/// </summary>
		/// <remarks>
		/// The earth "up" is assumed to be the north pole, and the earth "right"
		/// is assumed to be the origin of latitude & longitude coordinates.
		/// </remarks>
		public Vector3 AziLatLon { 
				get {
						if (!m_earth)
								return Vector3.one;
						Vector3 worldFrom = m_viewer ? (m_viewer.transform.position - m_earth.transform.position) : Vector3.zero;
						float earthRight = Vector3.Dot (worldFrom, m_earth.transform.right);
						float earthUp = Vector3.Dot (worldFrom, m_earth.transform.up);
						float earthForward = Vector3.Dot (worldFrom, m_earth.transform.forward);

						float equatorial = Mathf.Sqrt (earthRight * earthRight + earthForward * earthForward);
						float latitude = Mathf.Atan2 (earthUp, equatorial) * Mathf.Rad2Deg;
						float longitude = 0f;
						float azimuth = 0f;
						if (equatorial > 1e-3) {
								longitude = Mathf.Atan2 (earthForward, earthRight) * Mathf.Rad2Deg;

								Vector3 zeroAziRight = Vector3.Cross (worldFrom, m_earth.transform.up).normalized;
								Debug.DrawRay (m_viewer.transform.position, zeroAziRight, Color.red);
								Vector3 zeroAziForward = -Vector3.Cross (worldFrom, zeroAziRight).normalized;
								Debug.DrawRay (m_viewer.transform.position, zeroAziForward, Color.blue);
								azimuth = Mathf.Atan2 (
									Vector3.Dot (m_viewer.transform.forward, zeroAziRight), 
									Vector3.Dot (m_viewer.transform.forward, zeroAziForward)
								) * Mathf.Rad2Deg;
						} else {
								// Define longitude = azimuth = view from pole
								// with zero being the view of a geodesic with longitude = 0
								// WARNING: There will be coordinate discontinuities when crossing a pole!
								Debug.Log ("Computing Azimuth & Longitude at Pole!");
								azimuth = Mathf.Atan2 (
									Vector3.Dot (m_earth.transform.forward, m_viewer.transform.forward),
									Vector3.Dot (-m_earth.transform.right, m_viewer.transform.forward)
								) * Mathf.Rad2Deg;
								longitude = azimuth;
						}

						return new Vector3 (azimuth, latitude, longitude);
				}
				set {
						if (!m_viewer)
								return;
						// Initialize
						m_viewer.transform.position = m_earth.transform.position + m_earth.transform.right * m_earthRadius;
						m_viewer.transform.localRotation = Quaternion.Euler (270f, 270f, 0f); //Looking North from the Gulf of Guinea
      
						// Use Azimuth Longitude & Latitude to position & orient viewer
						m_viewer.transform.RotateAround (m_earth.transform.position, m_earth.transform.right, value [0]);
						m_viewer.transform.RotateAround (m_earth.transform.position, m_earth.transform.forward, value [1]);
						m_viewer.transform.RotateAround (m_earth.transform.position, m_earth.transform.up, -value [2]);
      
						// Place viewer at center
						Vector3 displace = m_viewer.transform.position;
						m_viewer.transform.position = Vector3.zero;
						m_earth.transform.position -= displace;

						// Record state
						// NOTE: This cannot be an OnDestroy call since referenced objects might be absent
						TimeAndLocationState state = TimeAndLocationState.Instance;
						if (state) {
								state.dateTime = m_dateTime;
								state.azimuth = value [0];
								state.latitude = value [1];
								state.longitude = value [2];
						}
				}
		}
  
		public static void IncrementLongitude ()
		{
				if (Instance)
						Instance.Longitude += Instance.m_latLongStep * Time.deltaTime;
		}

		public static void DecrimentLongitude ()
		{
				if (Instance)
						Instance.Longitude -= Instance.m_latLongStep * Time.deltaTime;
		}

		public static void IncrementLatitude ()
		{
				if (Instance)
						Instance.Latitude += Instance.m_latLongStep * Time.deltaTime;
		}

		public static void DecrimentLatitude ()
		{
				if (Instance)
						Instance.Latitude -= Instance.m_latLongStep * Time.deltaTime;
		}

		public static void IncrementDay ()
		{
				if (Instance)
						Instance.DateAndTime = Instance.DateAndTime.AddDays (1.0f);
		}

		public static void DecrimentDay ()
		{
				if (Instance)
						Instance.DateAndTime = Instance.DateAndTime.AddDays (-1.0f);
		}

		public static void IncrementHour ()
		{
				if (Instance)
						Instance.DateAndTime = Instance.DateAndTime.AddHours (1.0f);
		}

		public static void DecrimentHour ()
		{
				if (Instance)
						Instance.DateAndTime = Instance.DateAndTime.AddHours (-1.0f);
		}
 
		void Awake ()
		{
				if (Instance == null) {
						Instance = this;
				} else {
						Debug.Log ("Attempting to create more than once TimeAndLocationHandlerInstance");
				}
		}

		// Use this for initialization
		void Start ()
		{
				TimeAndLocationState initialState = TimeAndLocationState.Instance;
				if (initialState) {
						m_dateTime = initialState.dateTime;
						AziLatLon = new Vector3 (initialState.azimuth, initialState.latitude, initialState.longitude);
				} else {
						m_dateTime = DateTime.UtcNow;
						AziLatLon = new Vector3 (90f, 0f, 0f);
				}
		}

		// FixedUpdate is called at invariant time intervals
		void FixedUpdate ()
		{
				if (!m_orbiterRoot)
						return;
				m_dateTime = m_dateTime.AddSeconds (Time.deltaTime * timeScale);

				float normSidereal = getSidereal () * 360f / 24f;
				m_orbiterRoot.transform.localRotation = Quaternion.Euler (0.0f, normSidereal, 0.0f);
		}

		public float getSidereal ()
		{
				return (float)calcGreenwichMeanSiderealTime (DateAndTime);
		}

		private int calcDayNumber (DateTime dateAndTime)
		{
				DateTime yearStart = new DateTime (dateAndTime.Year, 1, 1, 0, 0, 0);
				TimeSpan diff = dateAndTime - yearStart;
				return diff.Days;
		}

		private double calcHours (DateTime dateAndTime)
		{
				DateTime dayStart = new DateTime (dateAndTime.Year, dateAndTime.Month, dateAndTime.Day, 0, 0, 0);
				TimeSpan diff = dateAndTime - dayStart;
				return diff.TotalHours;
		}

		private double calcGreenwichMeanSiderealTime (DateTime greenwichMeanDateAndTime)
		{
				// Constant calculation for Greenwich Mean Sidereal Time 
				// http://www.astro.umd.edu/~jph/GST_eqn.pdf
				double GST = 6.5988098 + 0.0657098244 * calcDayNumber (greenwichMeanDateAndTime) + 1.00273791 * calcHours (greenwichMeanDateAndTime);
				return GST % 24.0f; // reduce result into 24 hour range.
		}
}
