

using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour {
	[SerializeField]
	private float sensitivity;

	private Vector3 lastMousePosition;

	// Use this for initialization
	void Start () {
		lastMousePosition = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 delta = Input.mousePosition - lastMousePosition;
		Vector3 velocity = delta / Time.deltaTime;

		if ( Input.GetMouseButton(0) ) {
			Quaternion toRotate = Quaternion.Euler(-1 * velocity.y * sensitivity, velocity.x * sensitivity, 0.0f);
			Quaternion currentRotation = transform.localRotation;
			currentRotation = currentRotation * toRotate;
			transform.localRotation = currentRotation;
		}

		lastMousePosition = Input.mousePosition;
	}	
}
