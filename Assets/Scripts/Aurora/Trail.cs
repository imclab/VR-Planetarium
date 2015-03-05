using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Leap
{
		public class Trail : MeshObject
		{
				public float AURORA_HEIGHT = 0.1f;
				List<Vector3> SpinePoints = new List<Vector3> ();
				public HandController HandController;
				public int VCOUNT = 3;
				//VectorLine trail = null;
				//VectorLine trail2 = null;
				public Camera MyCamera;
				public float SCALE_RATIO = 3.0f;
				public int MAX_POINTS = 4;
				public float MIN_Y = 0.2f;
				GameObject go;
				public ParticleSystem AuroraParticles;

				// Use this for initialization
				void Start ()
				{
						go = new GameObject ();
						go.transform.SetParent (MyCamera.transform);
						//VectorLine.SetCamera3D (MyCamera);
						//VectorLine.SetLineParameters (Color.white, null, 2.0f, 0, 1, LineType.Continuous, Joins.None);
				}
	
				// Update is called once per frame
				void Update ()
				{
						AddHandPoints ();
				}

				void AddHandPoints ()
				{
						Frame frame = HandController.GetFrame ();
						if (frame.Hands.Count > 0) {
								HandModel handModel = RightmostHand ();
								if (handModel != null) {
										AddHandPoints (handModel);
										ReMesh ();
								} else {
										Debug.Log ("No RM hand");
								}
						}
			
				}

				Vector3 Ceiling (Vector3 v)
				{
						go.transform.position = v;
						Vector3 lp = go.transform.localPosition;
						lp.y = Mathf.Max (MIN_Y, lp.y);
			
						go.transform.localPosition = lp;
						return go.transform.position;
				}

				void AddHandPoints (HandModel handModel)
				{
						Vector3 v = handModel.fingers [1].GetTipPosition (); // will be world position

						v -= MyCamera.transform.position;
						v *= SCALE_RATIO;
						v += MyCamera.transform.position;

						SpinePoints.Add (Ceiling (v));
						while (SpinePoints.Count > MAX_POINTS)
								SpinePoints.RemoveAt (0);
				}

				HandModel RightmostHand ()
				{
						HandModel rightHand = null;
						foreach (HandModel hand in HandController.GetAllGraphicsHands()) {
								if (rightHand != null) {
										if (hand.GetLeapHand ().IsRight)
												rightHand = hand;
								} else {
										rightHand = hand;
								}
						}
						return rightHand;
				}

				void ReMesh ()
				{
						Clear ();
						Spine spine = new Spine (SpinePoints);
						spine.AddPoints (this, 0);
						for (int i = 0; i < VCOUNT; ++i) {
								Spine spine2 = spine.OffsetClone (AURORA_HEIGHT * MyCamera.transform.up);
								spine2.AddPoints (this, 1);
								Lathe (spine, spine2);
								spine = spine2;
						}
				}

				void MakeQuad (MeshPoint lastMeshPoint, MeshPoint MeshPoint, MeshPoint lastTopMeshPoint, MeshPoint topMeshPoint)
				{
			AddTriangle (MeshPoint, lastMeshPoint, lastTopMeshPoint);
			AddTriangle (lastTopMeshPoint, topMeshPoint, MeshPoint);

			AddTriangle (lastTopMeshPoint, lastMeshPoint, MeshPoint);
			AddTriangle (MeshPoint, topMeshPoint, lastTopMeshPoint);

				}

				void Lathe (Spine spine, Spine spineTop)
				{
						if (spine.SpinePoints.Count != spineTop.SpinePoints.Count) 
								throw new UnityException (string.Format ("Unequal number of points between spine 1 ({0}) and spine 2 ({1})",
				                                         spine.SpinePoints.Count, spineTop.SpinePoints.Count));

						MeshPoint lastSpinePoint = MeshPoint.Null;
						MeshPoint lastTopSpinePoint = MeshPoint.Null;

						for (int i = 0; i < spine.MeshPoints.Count; ++i) {

								MeshPoint spinePoint = spine.MeshPoints [i];
								MeshPoint topSpinePoint = spineTop.MeshPoints [i];

								if (i > 0) {
										MakeQuad (lastSpinePoint, spinePoint, lastTopSpinePoint, topSpinePoint);
								}
								lastSpinePoint = spinePoint;
								lastTopSpinePoint = topSpinePoint;
						}

						MakeMesh ();

						Debug.Log ("Making Trails");

						//SpineToVectorLine ("trail 1", ref trail, spine.SpinePoints.ToArray (), Color.white);
						//SpineToVectorLine ("trail 2", ref trail2, spineTop.SpinePoints.ToArray (), Color.red);
				}
      /*
				void SpineToVectorLine (string spineName, ref VectorLine line, Vector3[] points, Color color)
				{
						if (line != null)
								VectorLine.Destroy (ref line);

						if ((points == null) || (points.Length <= 2)) {
								Debug.Log ("too few points");
								return;
						}

						if (points.Length > MAX_POINTS) {
								List<Vector3> newPoints = new List<Vector3> ();
								for (int i = points.Length - MAX_POINTS; i < points.Length; ++i) {
										newPoints.Add (points [i]);
								}
								points = newPoints.ToArray ();
						}
 
						Debug.Log ("   Points: " + PointStr (points));
						line = VectorLine.MakeLine (spineName, points, color);
						
						line.Draw3D ();
				}*/
		
				string PointStr (Vector3[] points)
				{
						string s = "";
						foreach (Vector3 p in points) {
								s += PointStr (p) + ", ";

						}
						return s;
				}

				string PointStr (Vector3 point)
				{
						return string.Format (" ({0}, {1}, {2})", Mathf.RoundToInt (point.x), Mathf.RoundToInt (point.y), Mathf.RoundToInt (point.z));
				}
		}
}
