using UnityEngine;
using System.Collections;
using LMLineDrawing;

public class LineTester : MonoBehaviour {
  [SerializeField]
  private Camera m_targetCamera;
  [SerializeField]
  private Transform[] m_points;
  [SerializeField]
  private Material m_lineMaterial;

	// Use this for initialization
	void Start () {
    LineObject line = LineObject.LineFactory (transformsToPoints (m_points), 1.0f, m_targetCamera, m_lineMaterial, true);
    //line.AutoTarget = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  private Vector3[] transformsToPoints(Transform[] transforms) {
    Vector3[] points = new Vector3[transforms.Length];
    for (int i=0; i<transforms.Length; i++) {
      points[i] = transforms[i].position;
    }
    return points;
  }
}
