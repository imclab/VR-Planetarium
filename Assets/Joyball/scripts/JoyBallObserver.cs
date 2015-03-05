using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WidgetShowcase
{
		public class JoyBallObserver : MonoBehaviour
		{

				public JoyBall joyBall;
				public Text HandPumpText;
				public Text HandFacingText;

				// Use this for initialization
				void Start ()
				{
	
				}
	
				// Update is called once per frame
				void Update ()
				{
						if (joyBall) {
								HandPumpText.text = string.Format ("Toggle: {0}\nHandPump: {1}", 
									joyBall.HandPump.GetComponent<Toggle> ().BoolValue.ToString (),
									joyBall.HandPump.GetComponent<StickyBool> ().BoolValue.ToString ());

								HandFacingText.text = string.Format ("Facing: {0}", 
									joyBall.PalmFacing.GetComponent<StickyBool> ().BoolValue.ToString ());
						}
				}
		}
}