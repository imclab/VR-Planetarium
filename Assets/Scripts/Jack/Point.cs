using UnityEngine;
using System.Collections;

namespace Jack
{
		public	struct Point
		{
				public Vector3 Loc;
				public int Index;
				public Vector2 UV;
				public Vector3 Normal;
				const float ARROW_LENGTH = 1.5f;
		
				public Point (
					float x, float y, float z, 
					float u, float v,
					 float nx, float ny, float nz,
					 int index)
				{
						Loc = new Vector3 (x, y, z);
						UV = new Vector2 (u, v);
						Index = index;
						Normal = new Vector3 (nx, ny, nz);
				}

				public void AlignGameObject (GameObject n)
				{
					/*	n.transform.position = Loc;
						if (n.GetComponent<PointedNornal> ()) {
								PointedNornal pn = n.GetComponent<PointedNornal> ();
								pn.tip.transform.position = Loc + Normal * ARROW_LENGTH;
								pn.arrow.transform.LookAt (pn.tip.transform);
						} */
				}
		}

}