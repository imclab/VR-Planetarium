using UnityEngine;
using System.Collections;

public class CompasRotator : MonoBehaviour {
  [SerializeField]
  private Transform rotationMatcher;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		float az = (TimeAndLocationHandler.Instance) ? TimeAndLocationHandler.Instance.Azimuth : 0;

    gameObject.transform.rotation = rotationMatcher.transform.rotation;

    Quaternion ySubtraction = Quaternion.Euler(0.0f, -az, 0.0f);
    transform.localRotation = transform.localRotation * ySubtraction;
	}
}
