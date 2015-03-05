using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class FloatToOpacity : MonoBehaviour
		{

				public FloatEmitter InputEmitter;
				public GameObject Target;

				// Use this for initialization
				void Start ()
				{
						InputEmitter.FloatEvent += HandleFloatEvent;
				}

				void HandleFloatEvent (object sender, WidgetEventArg<float> e)
				{
						if (Target && Target.renderer && Target.renderer.material) {
								Color c = Target.renderer.material.color;
								c.a = Mathf.Clamp01 (e.CurrentValue);
								Target.renderer.material.color = c;
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}
		}
}
