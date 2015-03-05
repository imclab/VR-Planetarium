//#define DRAW_NORMALS

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Jack
{
		public class Beanstalk : MonoBehaviour
		{
			
				List<Point> Points = new List<Point> ();
				List<int> triangles = new List<int> ();
				List<PointList> PointLists = new List<PointList> ();
				public GameObject NormalPointer;
				float height = 0.5f;
				public List<GameObject> Pointers = new List<GameObject> ();
				List<GameObject> SpinePoints = new List<GameObject> ();
				public GameObject SpinePlaceholder;
				const float BEND_EXP = 1.5f;
				const float BEND_SCALE = 0.25f;

				public float Height {
						get {
								return height;
						}
						set {
								height = value;
								CreateStalk ();
						}
				}

				float radius = 1;

				public float Radius {
						get {
								return radius;
						}
						set {
								radius = value;
								CreateStalk ();
						}
				}

				int radialPoints = 12;

				public int RadialPoints {
						get {
								return radialPoints;
						}
						set {
								//	radialPoints = value;
								//CreateStalk ();
						}
				}

				int rowCount = 8;

				public int RowCount {
						get {
								return rowCount;
						}
						set {
								rowCount = value;
						}
				}

				public float CapUVsize {
						get {
								return (RadialPoints + 1) / ((float)(RadialPoints + 2));
						}
				}

				public float BandUVsize { get { return 1f - (2f * CapUVsize); } }

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
		
				public PointList lowerCap;
				public PointList upperCap;

				// Use this for initialization
				void Start ()
				{
						CreateStalk ();
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (Mathf.Floor (Time.time) > RadialPoints) {
								RadialPoints = Mathf.FloorToInt (Time.time);
								Debug.Log ("Updating point count");
						}
						if (Mathf.Floor (Time.time * 3) / 5 > Height) {
								Height = Mathf.Floor (Time.time * 3) / 5;
						}
				}

				void CreateSpine ()
				{
						/*	foreach (GameObject g in SpinePoints) {
								if (g)
										Destroy (g);
						}
						SpinePoints.Clear (); */
						GameObject lastG = null;
						for (int i = 0; i < PointLists.Count; i++) {
								PointList p = PointLists [i];
								GameObject g;
								if (SpinePoints.Count <= i) {
										g = (GameObject)Instantiate (SpinePlaceholder);
										SpinePoints.Add (g);
								} else {
										g = SpinePoints [i];
								}
								g.transform.position = new Vector3 (Mathf.Pow (i, BEND_EXP) * BEND_SCALE, p.height, 0);
								if (lastG) {
									//	lastG.transform.LookAt (g.transform, Vector3.forward);
										//lastG.transform.RotateAround (lastG.transform.position, Vector3.forward, -90);
								}
								lastG = g;
						}
				}

				void SpineToPoints ()
				{
						for (int i = 1; i < PointLists.Count; i++) {
								PointList pl = PointLists [i];	
								GameObject spineGO = SpinePoints [i];	
								for (int j = 0; j < pl.Points.Count; j++) {
										Point p = pl.Points [j];
										GameObject pointAlias = new GameObject ();
										pointAlias.transform.parent = spineGO.transform;
										pointAlias.transform.localPosition = new Vector3 (p.Loc.x, 0, p.Loc.z);
										p.Loc = pointAlias.transform.position;
										p.Normal = (p.Loc - spineGO.transform.position).normalized;
										Points [p.Index] = p;
										Destroy (pointAlias);

										/*		Vector3 offset = p.Loc - pl.Center;
										GameObject normalGO = new GameObject ();
										normalGO.transform.parent = pointGO.transform;
										normalGO.transform.localPosition = p.Normal;
										// change point based on game objects
										p.Normal = (normalGO.transform.position - pointGO.transform.position).normalized;
										p.Loc = pointGO.transform.position; */
								}
								
						}
				}

				void EraseData ()
				{
						foreach (GameObject g in Pointers) {
								if (g)
										Destroy (g);
						}
						Pointers.Clear ();
			
						Points.Clear ();
						foreach (PointList pl in PointLists) {
								pl.Clear ();
						}
						PointLists.Clear ();
						triangles.Clear ();
				}

				void CreateStalk ()
				{
						EraseData ();

						PointList lastRing = null;

						for (int j = 0; j <= RowCount; ++j) {
								Debug.Log (string.Format ("making ring at {0} with last {1}", j, lastRing));
								float ringHeight = this.Height * j / RowCount;
								PointList pl = PointList.MakeRing (this, ringHeight, lastRing);
								PointLists.Add (pl);
								lastRing = pl;
						}

						CreateSpine ();
						SpineToPoints ();
 
						Mesh stalk;
						GetComponent<MeshFilter> ().mesh.Clear ();
						stalk = GetComponent<MeshFilter> ().mesh;


						stalk.vertices = Vertices;
						stalk.triangles = triangles.ToArray ();
						stalk.normals = Normals;
						stalk.uv = UVs;

#if DRAW_NORMALS
								foreach (Point p in Points) {
										GameObject n = (GameObject)Instantiate (NormalPointer);
										p.AlignGameObject (n);
										Pointers.Add (n);
								}
#endif
				}

				public Point AddPoint (float x, float y, float z, float u, float v, float nx, float ny, float nz)
				{
						int index = Points.Count;
						Point newPoint = new Point (x, y, z, u, v, nx, ny, nz, index);
						Points.Add (newPoint);
						return newPoint;
				}

				public void AddTriangle (Point a, Point b, Point c)
				{
						triangles.Add (a.Index);
						triangles.Add (b.Index);
						triangles.Add (c.Index);
				}
		
				Vector3 NormalFromPts (Vector3 a, Vector3 b, Vector3 c)
				{
						return Vector3.Normalize (Vector3.Cross (b - a, c - a));
				}

				Vector3 NormalFromPts (Point a, Point b, Point c)
				{
						return NormalFromPts (a.Loc, b.Loc, c.Loc);
				}

				Vector3[] Normals {
						get {
								Vector3[] nl = new Vector3[Points.Count];
								for (int i = 0; i < Points.Count; i++) {
										Point p = Points [i];
										nl [i] = p.Normal;
								}

								return nl;
						}
				}
		}
}