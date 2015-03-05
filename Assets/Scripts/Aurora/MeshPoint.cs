using UnityEngine;
using System.Collections;

namespace Leap
{
		public	struct MeshPoint
		{
				public Vector3 Loc;
				public int Index;
				public Vector2 UV;
				public Vector3 Normal;
		
		public MeshPoint (
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

		public MeshPoint (Vector3 loc, Vector2 uv, int index, Vector3 normal)
				{
						Loc = loc;
						UV = uv;
						Normal = normal;
						Index = index;
				}

		public static MeshPoint Null {
			get { return new MeshPoint (0, 0, 0, 0, 0, 0, 0, 0, -1);
								;}
				}
		}

}