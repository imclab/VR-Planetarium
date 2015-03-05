using UnityEngine;
using System.Collections;

public class QualityControls : MonoBehaviour {
	public int targetFrameRate = -1; // -1 indicates maximum possible speed
	public float actualFrameRate = 0f;

	// Use this for initialization
	void Awake () {
		if (QualitySettings.vSyncCount != 0) {
			Debug.Log("vSync will override target frame rate");
			return;
		}
		Application.targetFrameRate = targetFrameRate;
	}
	
	// Update is called once per frame
	void Update () {
		actualFrameRate = 1f / Time.deltaTime;
	}
}
