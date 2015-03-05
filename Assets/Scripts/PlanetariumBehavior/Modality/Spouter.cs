using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class Spouter : MonoBehaviour
		{
				public ParticleSystem Particles;
				public float StopEmitting = 0;

				void Start ()
				{
						if (Particles == null)
								Particles = GetComponent<ParticleSystem> ();
				}

				void Update ()
				{

						if (StopEmitting < Time.time)
								Particles.Stop ();

				}

				public void Spout (float dur = 1.0f)
				{
						Particles.Play ();
						StopEmitting = Time.time + dur;
				}
		}

	
}