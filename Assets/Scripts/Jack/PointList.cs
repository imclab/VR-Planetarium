using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Jack
{
		public class PointList
		{
				public List<Point> Points = new List<Point> ();
				public PointList lastList;
				public PointList nextList;
				public Point capPoint;
				public float height;
				Beanstalk host;
				public enum ListType
				{
						Cap,
						Ring,
						Other
				}

				public ListType type = ListType.Other;
		
		#region constructors
				public	PointList (Beanstalk host_)
				{
						host = host_;
						height = 0;
						lastList = null;
						nextList = null;
				}
		
				public PointList (Beanstalk host_, float height_, PointList last)
				{
						host = host_;
						height = height_;
						nextList = null;
						if (last != null)
								last.nextList = this;
						lastList = last;
						type = ListType.Ring;
				}
		
				public PointList (Beanstalk host_, float height_)
				{
						host = host_;
						height = height_;
						lastList = null;
						nextList = null;
						type = ListType.Cap;
				}
		#endregion
		
				public Vector3 Center { get { return new Vector3 (0, height, 0); } }

				void MakePoints ()
				{
						float v = Mathf.Clamp01 (host.CapUVsize + (height * host.BandUVsize) / host.Height);
						Vector3 n;

						for (int arc = 0; arc < host.RadialPoints; ++arc) {
								float radAngle = (2 * Mathf.PI) * arc / ((float)(host.RadialPoints - 1));
								float u = Mathf.Clamp01 (radAngle / host.RadialPoints);
								float x = Mathf.Cos (radAngle) * host.Radius;
								float z = Mathf.Sin (radAngle) * host.Radius;
								
								n = new Vector3 (x, 0, z).normalized;
								Debug.Log (string.Format ("Normalized Point {0}: {1} ({2}, {3}, {4})", arc, n, x, 0, z));
								Point pt = host.AddPoint (x, height, z, u, v, n.x, n.y, n.z);
								Points.Add (pt);
						}

						if (type == ListType.Cap) {
								n = height > 0 ? Vector3.up : Vector3.down;
								v = height > 0 ? 1 : 0;
								float u = 0;
								capPoint = host.AddPoint (0, height, 0, u, v, n.x, n.y, n.z);
						}
				}
		
				public Point Add (Point p)
				{
						Points.Add (p);
						return p;
				}
		
				public int Count { get { return Points.Count; } }
		
				public void Clear ()
				{
						Points.Clear ();
				}
		
				public Point Get (int i)
				{
						return Points [i];
				}

				void MakePolygons ()
				{
						switch (type) {
						case ListType.Cap: 

								for (int i = 0; i < Points.Count - 1; i++) {
										Point rimPoint = Points [i];
										Point nextRimPoint = Points [i + 1];
										host.AddTriangle (rimPoint, capPoint, nextRimPoint);
								}

								break;

						case ListType.Ring: 
								if (lastList != null) {
										Debug.Log ("Making Ring");
										if (lastList.Points.Count == Points.Count)
												for (int i = 0; i < (Points.Count - 1); i++) {
														//Point rimPoint = Points [i];
														//Point nextRimPoint = Points [i + 1];
														Point ll = Points [i];
														Point lr = Points [i + 1];
														Point ul = lastList.Points [i];
														Point ur = lastList.Points [i + 1];
														host.AddTriangle (ll, lr, ur);
														host.AddTriangle (ul, ll, ur);
												}
										else {
												Debug.Log ("Not making points for ring -- wrong count");
										}
								} else {
										Debug.Log ("not making points for ring -- no lastList");
								}

								break;

						default:
								Debug.Log ("Not making polys for ring -- no type set");
								break;
								;
						}

				}
		
				public static PointList MakeCap (Beanstalk host_, float height_)
				{
						PointList pl = new PointList (host_, height_);
						pl.type = ListType.Cap;
						pl.MakePoints ();
						pl.MakePolygons ();
						return pl;
				}
		
				public static PointList MakeRing (Beanstalk host_, float height, PointList lastRing = null)
				{
						PointList pl = new PointList (host_, height, lastRing);
						pl.MakePoints ();
						pl.MakePolygons ();
						return pl;
				}
		}
}