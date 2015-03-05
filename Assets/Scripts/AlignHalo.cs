using UnityEngine;
using System.Collections;

/// <summary>
/// Aligns position with viewer center eye direction,
/// while maintaining height and tilt of viewer body.
/// </summary>
/// <remarks>
/// The reference will be placed at the specified position
/// relative to the transform parent, qualified as follows:
/// (1) The viewer horizontal displacement is an added.
/// (2) The view direction is applied as a
/// rotation around the viewer body.
/// </remarks>
public class AlignHalo : MonoBehaviour {
	public Transform Viewer;
	public Vector3 ReferencePosition;

	public int dbgStep = 0;
	void Update () {
		if (transform.parent == null ||
		    Viewer == null) {
			return;
		}
		if (dbgStep == 0) return;

		transform.localRotation = Quaternion.identity;
		transform.localPosition = ReferencePosition;
		
		if (dbgStep == 1) return;
		
		// Compute the local rotation: horizontal line through viewer up & forward
		Vector3 look = Viewer.forward;
		look -= transform.parent.up * Vector3.Dot (transform.parent.up, look);
		if (look.magnitude < 1e-3) {
			return;
		}
		Vector3 axis = transform.parent.up;
		float angle = 0f;
		Quaternion.FromToRotation (transform.parent.forward, look).ToAngleAxis (out angle, out axis);
		transform.RotateAround(transform.parent.position, axis, angle);

		
		if (dbgStep == 2) return;
		
		// Compute the local displacement
		Vector3 CenterEyeDisplace = transform.parent.InverseTransformPoint (Viewer.position);
		CenterEyeDisplace.y = 0f;
		transform.localPosition += CenterEyeDisplace;
	}
}
