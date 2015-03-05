using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace WidgetShowcase
{
		public class Alphan : MonoBehaviour
		{
				public GameObject alphaTarget;
				public Text feedback;
				//float originalScale;
	

				// Use this for initialization
				void Start ()
				{
						if (alphaTarget == null)
								alphaTarget = this.gameObject;
						//originalScale = alphaTarget.transform.localScale.x;
						GetLastAlpha ();
				}
	
				// Update is called once per frame
				void Update ()
				{
					GetLastAlpha();
				
					if(targetAlpha != lastAlpha){
						AdjustAlpha ();
					}
				}

				void GetLastAlpha ()
				{
						lastAlpha = alphaTarget.gameObject.renderer.material.color.a;
				}

				public float lastAlpha = 0;
				public float targetAlpha = 0;
				private float startTime = 0;
				public float targetTime = 0;

				public void SetAlpha (float newTargetAlpha, float Delay = 0)
				{
						GetLastAlpha ();
						targetTime = Time.time + Mathf.Max (0, Delay);
						targetAlpha = newTargetAlpha;
						startTime = Time.time;
				}

				void AdjustAlpha ()
				{
						//if (targetTime >= Time.time + 0.1f) {
								if(lastAlpha > .02 && alphaTarget.renderer.enabled == false){
									alphaTarget.renderer.enabled = true;
								}
								float lerp = (Time.time - startTime) / (targetTime - startTime);
								float alpha = Mathf.Lerp (lastAlpha, targetAlpha, lerp);
								Color newColor = alphaTarget.renderer.material.color;
								if (alpha < 0.01f)
										alpha = 0;
								newColor.a = alpha;
								if (feedback)
										feedback.text = (string.Format ("LERP: {0}, alpha {1}", lerp, alpha));
								alphaTarget.renderer.material.color = newColor;
								//	alphaTarget.transform.localScale = Vector3.one * Mathf.Max (0.1f, alpha) * originalScale;
								if(lastAlpha < .02){
									alphaTarget.renderer.enabled = false;
								}
					//	}
				}
		}
}