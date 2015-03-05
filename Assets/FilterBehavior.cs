using UnityEngine;
using System.Collections;

public class FilterBehavior : MonoBehaviour {
  public bool DrawMilkyWay = true;
  public float MilkyWayIntensity { 
    get { return m_intensity; } 
    set {
      m_intensity = Mathf.Clamp01(value);
    }
  }

  private float m_intensity = 0.4f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
    Color current = renderer.material.color;
    float goalAlpha = 1.0f - MilkyWayIntensity;

    if ( DrawMilkyWay && current.a != goalAlpha ) {
      current.a = goalAlpha;
      renderer.material.color = current;
    }
    else if ( !DrawMilkyWay && current.a != 1.0f ) {
      current.a = 1.0f;
      renderer.material.color = current;
    }
	}
}
