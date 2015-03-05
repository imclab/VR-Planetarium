/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

// A Leap Motion hand script that set's the alpha
// of the hand based on the hand's self confidence value.
public class ConfidenceTransparency : MonoBehaviour {
  public float timeLag = 0f;
  public Smoother smoothedConfidence;
  private List<float> initialAlphas;

  void Start() {
    smoothedConfidence = new Smoother (1f, timeLag);
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    initialAlphas = new List<float> ();
    for (int i = 0; i < renderers.Length; ++i) {
      initialAlphas.Add (renderers[i].material.color.a);
    }
    Update ();
  }

  void Update() {
    Hand leap_hand = GetComponent<HandModel>().GetLeapHand();
    if (leap_hand == null)
      return;
	smoothedConfidence.Update (leap_hand.Confidence, Time.deltaTime);
	float confidence = smoothedConfidence.state;
//		Debug.Log ("Confidence = " + leap_hand.Confidence + " -> Smoothed Confidence = " + smoothedConfidence.state);
    if (leap_hand != null) {
      Renderer[] renderers = GetComponentsInChildren<Renderer>();
      for (int i = 0; i < renderers.Length && i < initialAlphas.Count; ++i) {
        SetRendererAlpha(renderers[i], confidence * initialAlphas[i]);
	  }
    }
  }

  protected void SetRendererAlpha(Renderer render, float alpha) {
    Color new_color = render.material.color;
    new_color.a = alpha;
    render.material.color = new_color;
  }
}
