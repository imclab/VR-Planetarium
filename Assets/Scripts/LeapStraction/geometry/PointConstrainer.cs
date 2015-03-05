using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class PointConstrainer : PointEmitter
		{

				public GameObject Constrainer; // the frame of reference in which the point resides
				// must be a game object with a box collider;
				public GameObject BottomLeftPoint;
				public GameObject TopRightPoint;
				public float MinLocalX = 0;
				public float MaxLocalX = 0;
				public float MinLocalY = 0;
				public float MaxLocalY = 0;
				public float MinLocalZ = 0;
				public float MaxLocalZ = 0;

				public override PointData PointValue {
						get { return pointValue; }
						set {
								if (value.HasData) {
										pointValue = ConstrainPointData (value);
								} else {
										pointValue = value;
								}
								EmitValue (pointValue);
						}
				}

				void Start ()
				{
						if (!Constrainer)
								Constrainer = gameObject;
						if (BottomLeftPoint) {
								MinLocalX = BottomLeftPoint.transform.localPosition.x;
								MinLocalY = BottomLeftPoint.transform.localPosition.y;
								MinLocalZ = BottomLeftPoint.transform.localPosition.z;
						}
						if (TopRightPoint) {
								MaxLocalY = TopRightPoint.transform.localPosition.x;
								MaxLocalY = TopRightPoint.transform.localPosition.y;
								MaxLocalZ = TopRightPoint.transform.localPosition.z;
						}
				}

/* 
 value is a world space point
*/
				PointData ConstrainPointData (PointData value)
				{
						if (!value.HasData)
								return value;
						Vector3 localValue = Constrainer.transform.InverseTransformPoint (value.Point);
						//	BoxCollider coll = (BoxCollider)Constrainer.collider;
						//	Bounds b = coll.bounds;
						//	Debug.Log (string.Format ("Constraining point {0} (local {1}) to {2} .. {3}", value.Point, localValue, b.min, b.max));
						localValue.x = Mathf.Clamp (localValue.x, MinLocalX, MaxLocalX);
						localValue.y = Mathf.Clamp (localValue.y, MinLocalY, MaxLocalY);
						localValue.z = Mathf.Clamp (localValue.z, MinLocalZ, MaxLocalZ);
						Vector3 valueOut = Constrainer.transform.TransformPoint (localValue);
						//Debug.Log (string.Format ("... result:{0} (global {1})", localValue, valueOut));

						return new PointData (valueOut);
				}
		}
	
}