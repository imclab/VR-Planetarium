using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WidgetShowcase
{
		public class AllEventListener : MonoBehaviour
		{
				protected bool linked = false;
				public Text TextX;
				public Text TextY;
				public Text TextZ;
				public Text TextType;
				float lastUpdateTime = 0;
				const float FADE_TIME = 0.5f;
				Image buttonImage;

				// Use this for initialization
				void Start ()
				{
						Init ();
				}

				public void Init ()
				{
						buttonImage = GetComponent<Image> ();
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (!linked) {
								Link ();
						}

						UpdateAlpha ();
				}

				protected void Link ()
				{
						if (JoyBall.Instance != null) {
								JoyBall.Instance.OnJoystickEvent += JoystickListener;
								linked = true;
						}
				}

				protected void UpdateAlpha ()
				{
						ImageAlpha = 1 - Mathf.Clamp01 ((Time.time - lastUpdateTime) / FADE_TIME);
				}

				float ImageAlpha {
						set {
								buttonImage.color = new Color (1, 1, 1, value);
						}
				}

				void JoystickListener (object sender, WidgetEventArg<JoystickEvent> e)
				{
						if (TextType) {
								TextType.text = e.CurrentValue.Type.ToString();
						}
						if (TextX)
								TextX.text = e.CurrentValue.Direction.x.ToString ();
						if (TextY)
								TextY.text = e.CurrentValue.Direction.y.ToString ();
						if (TextZ)
								TextZ.text = e.CurrentValue.Direction.z.ToString ();

						lastUpdateTime = Time.time;
				}
		}
	
}