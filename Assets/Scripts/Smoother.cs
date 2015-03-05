using System.Collections;

/// <summary>
/// Applies an exponential smoothing filter determined by the
/// timeLag of the smoothed result.
/// </summary>
/// <remarks>
/// The behavior of the filter is independent of the frame rate.
/// Lower frame rates will have less filtering applied.
/// </remarks>
public class Smoother
{
		public float state;
		public float timeLag;

		public Smoother (float init, float set_timeLag = 0f)
		{
				state = init;
				timeLag = set_timeLag;
		}

		// Update is called once per frame
		public void Update (float next, float deltaTime)
		{
				float nextWeight = 1f;
				if (timeLag > 0f)
						nextWeight = deltaTime / (deltaTime + timeLag);
				state *= (1f - nextWeight);
				state += nextWeight * next;
		}
}
