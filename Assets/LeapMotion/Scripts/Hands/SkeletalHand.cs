/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/
using UnityEngine;
using System.Collections;
using Leap;

// The model for our skeletal hand made out of various polyhedra.
public class SkeletalHand : HandModel
{

  protected const float PALM_CENTER_OFFSET = 0.0150f;
  public GameObject palm;
  public GameObject forearm;
  public GameObject wristJoint;
  public float verticalFilterTime = 0f; // Optimal tracking data
  public float horizontalFilterTime = 0f; // Reduced tracking data

  void Start ()
  {
    // Ignore collisions with self.
    Leap.Utils.IgnoreCollisions (gameObject, gameObject);
  }

  public override void InitHand ()
  {
    SetPositions ();
  }

  public override void UpdateHand ()
  {
    SetPositions ();
  }

  protected Vector3 GetPalmCenter ()
  {
    Vector3 offset = PALM_CENTER_OFFSET * Vector3.Scale (GetPalmDirection (), transform.localScale);
    return GetPalmPosition () - offset;
  }

  protected void SetPositions ()
  {
    for (int f = 0; f < fingers.Length; ++f) {
      if (fingers [f] != null)
        fingers [f].InitFinger ();
    }
	
    float sqrCos = hand_.Arm.Basis.zBasis.ToUnity ().z;
    sqrCos *= sqrCos;
    float filtering = sqrCos * verticalFilterTime + (1f - sqrCos) * horizontalFilterTime;
    float newWeight = Time.deltaTime / (Time.deltaTime + filtering); // Longer times assign higher weight to new data
    //Debug.Log ("newWeight(sqrCos = " + sqrCos + ", deltaTime = " + Time.deltaTime + ") = " + newWeight);

    if (palm != null) {
      palm.transform.position = GetPalmCenter ();
      palm.transform.rotation = GetPalmRotation ();
    }

    if (wristJoint != null) {
      wristJoint.transform.position = (1f - newWeight) * wristJoint.transform.position + (newWeight * GetWristPosition ());
      wristJoint.transform.rotation = Quaternion.Lerp (wristJoint.transform.rotation, GetPalmRotation (), newWeight);
    }

    if (forearm != null) {
      forearm.transform.position = (1f - newWeight) * forearm.transform.position + (newWeight * GetArmCenter ());
      forearm.transform.rotation = Quaternion.Lerp (forearm.transform.rotation, GetArmRotation (), newWeight);
    }
  }
}
