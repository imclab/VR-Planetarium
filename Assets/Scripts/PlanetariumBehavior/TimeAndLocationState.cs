using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Time and location persisted state singleton
/// </summary>
/// <remarks>
/// This data is separated out in order to make it persistent between scenes
/// </remarks>
public class TimeAndLocationState : MonoBehaviour
{
		public static TimeAndLocationState Instance;
		public DateTime dateTime;
		public float azimuth = 0.0f;
		public float latitude = 37.78384f;
		public float longitude = -122.3921f;

		void Awake ()
		{
				if (Instance == null) {
						Instance = this;
						DontDestroyOnLoad (this.gameObject);
				} else {
						Destroy (this.gameObject);
				}
				dateTime = DateTime.UtcNow;
		}
}
