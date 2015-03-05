using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Leap
{
		public abstract class MeshObject : MonoBehaviour
		{
			
				protected List<MeshPoint> Points = new List<MeshPoint> ();
				protected List<int> Triangles = new List<int> ();

				Vector3[] Vertices {
						get {
								Vector3[] vl = new Vector3[Points.Count];
								for (int i = 0; i < Points.Count; ++i)
										vl [i] = Points [i].Loc;
								return vl;
						}
				}
		
				Vector2[] UVs {
						get {
								Vector2[] uvl = new Vector2[Points.Count];
								for (int i = 0; i < Points.Count; ++i)
										uvl [i] = Points [i].UV;
								return uvl;
						}
				}

				public void Clear ()
				{
						Points.Clear ();
						Triangles.Clear ();
				}

				public MeshPoint AddPoint (float x, float y, float z, float u, float v, float nx, float ny, float nz)
				{
						int index = Points.Count;
						MeshPoint newPoint = new MeshPoint (x, y, z, u, v, nx, ny, nz, index);
						Points.Add (newPoint);
						return newPoint;
				}

				public void AddTriangle (MeshPoint a, MeshPoint b, MeshPoint c)
				{
						Triangles.Add (a.Index);
						Triangles.Add (b.Index);
						Triangles.Add (c.Index);
				}
		
				protected Vector3 NormalFromPts (Vector3 a, Vector3 b, Vector3 c)
				{
						return Vector3.Normalize (Vector3.Cross (b - a, c - a));
				}

				protected Vector3 NormalFromPts (MeshPoint a, MeshPoint b, MeshPoint c)
				{
						return NormalFromPts (a.Loc, b.Loc, c.Loc);
				}

				Vector3[] Normals {
						get {
								Vector3[] nl = new Vector3[Points.Count];
								for (int i = 0; i < Points.Count; i++) {
										MeshPoint p = Points [i];
										nl [i] = p.Normal;
								}

								return nl;
						}
				}

				int[] TrianglesArray {
						get { return Triangles.ToArray (); }
				}

				protected void MakeMesh ()
				{
						MeshFilter meshFilter = GetComponent<MeshFilter> ();

						if (meshFilter == null)
								throw new UnityException ("MeshObject must have MeshFilter");
						Mesh myMesh = meshFilter.sharedMesh;
						if (myMesh == null) {
								meshFilter.mesh = new Mesh ();
								myMesh = meshFilter.sharedMesh;
						}
						myMesh.Clear ();
						myMesh.vertices = Vertices;
						Debug.Log ("Vertices: " + Vertices.ToString ());
						Debug.Log ("TRIANGLES: " + TrianglesArray.ToString ());
						myMesh.triangles = TrianglesArray;
						myMesh.normals = Normals;
						myMesh.uv = UVs;
				}
		}
}