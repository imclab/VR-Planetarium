/******************************************************************************\
* Copyright (C) Leap Motion, Inc. 2011-2014.                                   *
* Leap Motion proprietary. Licensed under Apache 2.0                           *
* Available at http://www.apache.org/licenses/LICENSE-2.0.html                 *
\******************************************************************************/

using UnityEngine;
using System.Collections;
using Leap;

// A Leap Motion hand script that set's the alpha
// of the hand based on the hand's self confidence value.
public class HmdFading : MonoBehaviour {

  private Vector CENTER = new Vector(0, 180, 0);
  private const float ALPHA_CONSTANT = 0.00375f;
  private Material material;

  void Start() {
    material = new Material(Shader.Find("Transparent/Diffuse"));
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    
    for (int i = 0; i < renderers.Length; ++i)
      renderers[i].material = material;
  }

  void SetRendererAlpha(Renderer render, float alpha) {
    Color new_color = render.material.color;
    new_color.a = alpha;
    render.material.color = new_color;
  }

  void Update() {
    Hand leap_hand = GetComponent<HandModel>().GetLeapHand();

    if (leap_hand != null) {
      float dist_squared = (leap_hand.PalmPosition - CENTER).MagnitudeSquared / 1000000.0f;
      float alpha = Mathf.Min(leap_hand.Confidence,
                              ALPHA_CONSTANT / (dist_squared * dist_squared));

      Renderer[] renders = GetComponentsInChildren<Renderer>();
      foreach (Renderer render in renders)
        SetRendererAlpha(render, alpha);
    }
  }
}
