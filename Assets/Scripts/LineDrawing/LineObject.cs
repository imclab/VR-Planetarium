using UnityEngine;
using System;
using System.Collections;

namespace LMLineDrawing {
  [RequireComponent(typeof(MeshFilter))]
  public class LineObject : MonoBehaviour {

    private MeshFilter m_meshFilter;

    private bool m_autoTarget = false;
    public bool AutoTarget { 
      get { return m_autoTarget; }
      set { m_autoTarget = value; }
    }

    private float m_width;
    public float Width {
      get { return m_width; }
      set { m_width = value; } 
    }

    private bool m_continuous; 
    public bool Continuous {
      get { return m_continuous; }
      set { m_continuous = value; } 
    }

    private Camera m_targetCamera;
    public Camera TargetCamera {
      get { return m_targetCamera; }
      set { m_targetCamera = value; }
    }

    private MeshFilter _MeshFilter {
      get { 
        if (m_meshFilter == null) {
          m_meshFilter = GetComponent<MeshFilter> ();
        }
        return m_meshFilter;
      }
    }

    public Vector3[] Points {
      set { 
        UpdateMeshVerts(value);
      }
    }

    public static LineObject LineFactory(Vector3[] points, float width, Camera target, Material lineMaterial, bool continuous = false) {
      if (target == null) {
        throw new System.NullReferenceException ("Line needs a target to calulate its facing.");
      }

      GameObject line = new GameObject ();
      line.name = "line";
      line.AddComponent<MeshFilter>();
      line.AddComponent<MeshRenderer> ();
      LineObject lineObject = line.AddComponent<LineObject> ();

      lineObject.TargetCamera = target;
      lineObject.Width = width;
      lineObject.Continuous = continuous;

      Vector3 centroid = Vector3.zero;
      for (int i=0; i<points.Length; i++) { centroid+=points[i]; }
      centroid = centroid / points.Length;
      lineObject.transform.position = centroid;

      lineObject.GenerateNewMesh (points);

      line.renderer.material = lineMaterial;

      return lineObject;
    }

    private void GenerateNewMesh(Vector3[] points) {
      if (TargetCamera == null) {
        throw new System.NullReferenceException ("Line needs a target camera to calulate its facing.");
      }

      MeshData meshData = new MeshData ();
      LineGenerators.GenerateMeshDataFromPoints (ref meshData, points, TargetCamera.transform.position, transform.position, Width, Continuous, false);
      _MeshFilter.mesh = meshData.GenerateMeshFromData();
    }

    private void UpdateMeshVerts(Vector3[] points) {
      if (TargetCamera == null) {
        throw new System.NullReferenceException ("Line needs a target camera to calulate its facing.");
      }

      MeshData meshData = new MeshData ();
      LineGenerators.GenerateMeshDataFromPoints (ref meshData, points, TargetCamera.transform.position, transform.position, Width, Continuous, false);
      _MeshFilter.mesh.vertices = meshData.Verts;
    }

    void Update() {
    }
  }
}