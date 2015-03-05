using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class ObjectInHand : DataEmitter
		{

				public 	HandEmitter InputHandEmitter;
				public GameObject target;
				public GameObject JoyBallDraggable;
				public Vector3  DraggableStartPos;
				public Vector3 JoyballStartPos;
//				public GameObject JoyballCenter;
				
				// Use this for initialization
				void Start ()
				{
//						DraggableStartPos = transform.localPosition;
						
						if (target == null)
								target = this.gameObject;

				}
				
				public void OnActivate() {
//					JoyballStartPos = transform.localPosition;
					Debug.Log ("JoyballStartPos = " + JoyballStartPos);
					
					InputHandEmitter.HandEvent += HandleHandEvent;
				}
				
				public void OnDeactivate() {
					Debug.Log ("ObjectInHand.OnDeactivate()");
					InputHandEmitter.HandEvent -= HandleHandEvent;
					StartCoroutine(LerpToJoyZone());
				}

				void HandleHandEvent (object sender, WidgetEventArg<HandData> e)
				{
						if (e.CurrentValue.HasHand) {
								if (target) {
										target.transform.position = e.CurrentValue.HandModel.GetPalmPosition ();
										target.transform.rotation = e.CurrentValue.HandModel.GetPalmRotation ();
								}
						} // note - not handling noo-hand case - other script should do that. 
				}
				
				private IEnumerator LerpToJoyZone (){
					float lerpLength = 2.0f;
//					DraggableStartPos = JoyBallDraggable.transform.localPosition;
//					yield return new WaitForSeconds(1.0f);
					Debug.Log ("LerpToJoyZone");
					float elapsedTime = 0f;
//					Vector3 currentAngle = JoyBallDraggable.transform.eulerAngles;
					while (elapsedTime < lerpLength)
					{
							Vector3 currentAngle = JoyBallDraggable.transform.localEulerAngles;
							
							JoyBallDraggable.transform.localPosition = Vector3.Lerp(JoyBallDraggable.transform.localPosition, new Vector3 (0f, 0f, 0f), (elapsedTime / lerpLength));
//							JoyBallDraggable.transform.localRotation = Quaternion.Lerp(JoyBallDraggable.transform.localRotation, new Quaternion (0f, 0f, 0f, 0f), (elapsedTime / lerpLength));
							currentAngle = new Vector3(
								Mathf.LerpAngle(currentAngle.x, 0f, elapsedTime / lerpLength),
								Mathf.LerpAngle(currentAngle.y, 0f, elapsedTime / lerpLength),
								Mathf.LerpAngle(currentAngle.z, 0f, elapsedTime / lerpLength));
							JoyBallDraggable.transform.localEulerAngles = currentAngle;
							elapsedTime += Time.deltaTime;
							yield return new WaitForEndOfFrame();
						}
					
//					transform.localPosition = JoyballStartPos;
//					JoyBallDraggable.transform.localPosition = new Vector3 (0f, 0f, 0f);
//					JoyBallDraggable.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
//					JoyballCenter.transform.localPosition = new Vector3 (0f, 0f, 0f);
//					JoyballCenter.transform.localEulerAngles = new Vector3 (0f, 0f, 0f);
			
				}
		}
}
