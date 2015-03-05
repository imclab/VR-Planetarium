using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Leap
{
		class Spine
		{
				public List<Vector3> SpinePoints;
				public List<MeshPoint>MeshPoints = new List<MeshPoint> ();

				public Spine (List<Vector3> myPoints)
				{
						SpinePoints = myPoints;
				}

				public Spine OffsetClone (float x, float y, float z)
				{
						List<Vector3> offsetPoints = new List<Vector3> ();

						foreach (Vector3 point in SpinePoints) {
								offsetPoints.Add (point + new Vector3 (x, y, z));
						}
						return new Spine (offsetPoints);
				}

				public Spine OffsetClone (Vector3 v)
				{
						return OffsetClone (v.x, v.y, v.z);
				}

				public void AddPoints (MeshObject mo, float v)
				{
						for (int i = 0; i < SpinePoints.Count; i++) {
								Vector3 l = SpinePoints [i];
								float u = i / ((float)SpinePoints.Count);
								MeshPoints.Add (mo.AddPoint (l.x, l.y, l.z, u, v, 0, 1, 0));
						}
				}
		}

}
