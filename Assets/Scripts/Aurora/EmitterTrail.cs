//#define DEBUG
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

namespace WidgetShowcase
{
		public class EmitterTrail : MonoBehaviour
		{
				[SerializeField]
				private float
						m_maxEmmissionRate = 200.0f;
				[SerializeField]
				private float
						m_minEmmissionRate = 80.0f;
				[SerializeField]
				private float
						m_minHandSpeedForEmmission;
				[SerializeField]
				private float
						m_handSpeedForMaxEmmission;
				public float AURORA_HEIGHT = 0.1f;
				List<Vector3> SpinePoints = new List<Vector3> ();
				public HandEmitter HandInputEmitter;
				public int VCOUNT = 3;
				//VectorLine trail = null;
				//VectorLine trail2 = null;
				public Camera MyCamera;
				public float SCALE_RATIO = 3.0f;
				public int MAX_POINTS = 4;
				public float MIN_Y = 0.2f;
				GameObject go;
				public  GameObject Emitter;
				const float MIN_DISTANCE = 0.02f;
    float startTime = 0;

    [SerializeField]
    float rampTime = 1.0f;

				// Use this for initialization
				void Start ()
				{
						go = new GameObject ();
						if (!MyCamera) {
								MyCamera = GetComponentInParent<Camera> ();
						}
						go.transform.SetParent (MyCamera.transform);
						//VectorLine.SetCamera3D (MyCamera);
						//VectorLine.SetLineParameters (Color.white, null, 2.0f, 0, 1, LineType.Continuous, Joins.None);
						HandInputEmitter.HandEvent += HandleHandEvent;
				}

    Vector3 lastNoYPosition = Vector3.zero;
    float lastVelocity = 0.0f;



				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						SetEmission (e.CurrentValue.HasHand);

						if (!e.CurrentValue.HasHand) {
          
								return;
						}
       
						AddHandPoints (e.CurrentValue.HandModel);

						// Set the current emmission rate based on hand speed.
						//Vector3 velocity = e.CurrentValue.HandModel.GetLeapHand ().PalmVelocity.ToUnityScaled ();
      float smoothingFactor = Mathf.Clamp01(0.1f);
      Vector3 position = transform.localPosition;
      Vector3 noYPosition = new Vector3(position.x, 0.0f, position.z);
      float velocity = (lastNoYPosition - noYPosition).magnitude * Time.deltaTime;
      float velocitySmoothed = (lastVelocity * (1 - smoothingFactor)) + (velocity * smoothingFactor);
      lastVelocity = velocitySmoothed;
      lastNoYPosition = noYPosition;
            
      float normSpeed = Mathf.Clamp01((velocitySmoothed - m_minHandSpeedForEmmission) / (m_handSpeedForMaxEmmission - m_minHandSpeedForEmmission));
//      Debug.Log("velocitySmoothed: " + (velocitySmoothed * 100));
      //Debug.Log("normSpeed: " + normSpeed);

						if (normSpeed > 0) { 
								setEmmissionRateFromPercentage (Mathf.Clamp01 (normSpeed));
						} else {
								SetEmission (false);
						}
				}

    void OnEnable() {
      startTime = Time.time;
    }

				private void setEmmissionRateFromPercentage (float percent)
				{
						Mathf.Clamp01 (percent);
						float rate = m_minEmmissionRate + (percent * (m_maxEmmissionRate - m_minEmmissionRate));
      float timerPercent = Mathf.Clamp01((Time.time - startTime) / rampTime);
      rate *= timerPercent;
						Emitter.GetComponentInChildren<ParticleSystem> ().emissionRate = rate;
				}

				void ScaleLocalPosition ()
				{
						Vector3 local = transform.localPosition;
						local *= SCALE_RATIO;
						local.y = Mathf.Max (MIN_Y, local.y);
						local.x *= 2;
						transform.localPosition = local;
				}
  
				// Update is called once per frame
				void Update ()
				{
						Vector3 currentPoint = RandomSpinePoint ();
						transform.position = currentPoint;
						ScaleLocalPosition ();
						//SpineToVectorLine ("trail", ref trail, SpinePoints.ToArray (), Color.red);

      if ( HandInputEmitter.CurrentHand == null ) {
        SetEmission(false);
        setEmmissionRateFromPercentage(0.0f);
      }
				}

				void SetEmission (bool b)
				{
						Emitter.GetComponentInChildren<ParticleSystem> ().enableEmission = b;
				}

				int counter = 0;

				Vector3 RandomSpinePoint ()
				{
						if (SpinePoints.Count < 1)
								return Vector3.zero;
						counter += Random.Range (1, 4);
						counter %= SpinePoints.Count;
						return SpinePoints [counter];
				}
        /*
				void SpineToVectorLine (string spineName, ref VectorLine line, Vector3[] points, Color color)
				{
						if (line != null)
								VectorLine.Destroy (ref line);
      
						if ((points == null) || (points.Length <= 2)) {
								//  Debug.Log ("too few points");
								return;
						}
      
						if (points.Length > MAX_POINTS) {
								List<Vector3> newPoints = new List<Vector3> ();
								for (int i = points.Length - MAX_POINTS; i < points.Length; ++i) {
										newPoints.Add (points [i]);
								}
								points = newPoints.ToArray ();
						}
      
						line = VectorLine.MakeLine (spineName, points, color);

#if DEBUG
      line.Draw3D ();
#endif
				}*/

				Vector3 LastPoint {
						get { return SpinePoints.Count > 0 ? SpinePoints [SpinePoints.Count - 1] : Vector3.zero; }
				}

				bool IsFarEnoughAway (Vector3 v)
				{
						return (v - LastPoint).magnitude > MIN_DISTANCE;
				}

				void AddHandPoints (HandModel handModel)
				{
						Vector3 v = handModel.fingers [1].GetTipPosition (); // (index finger) -- will be world position
						if (SpinePoints.Count < 1 || IsFarEnoughAway (v))
								SpinePoints.Add (v);
						while (SpinePoints.Count > MAX_POINTS)
								SpinePoints.RemoveAt (0);
				}
		}
}
