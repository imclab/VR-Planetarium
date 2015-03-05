using System.Collections;

/// <summary>
/// Implements a two-state system with hysteresis.
/// An update via a point above the activation threshold yields an activated state.
/// An update below the persistence threshold yields an enervated state.
/// The state cannot change in the closed interval between persistence and activation thresholds.
/// </summary>
/// <remarks>
/// The Hysteresis can be visualized as a function equal to 1 above the activation threshold,
/// equal to 0 below the persistence threshold, and equal to a linear interpolation between.
/// Given a configured Hysteresis as follows:
///  Hysteresis instance(persistance, activation);
/// this function can be evaluated for all points as follows:
///  instance(point).interpolated();
/// In order to for Hysteresis to apply a condition it must be the case that
///  ((persistence <= activation) && increasing) ||
///  ((persistence >= activation) && !increasing)
/// In the absence of conditions the state cannot change due to updates.
/// </remarks>
public class Hysteresis
{
		// DEFAULT: Inactive with interpolated = 0
		protected bool m_activated = false;
		protected float m_position = 0f;
		// Configuration
		public bool increasing = true;
		public float persistence = 0f;
		public float activation = 0f;

		/// <remarks>
		/// Changing the position will modify activated status ONLY
		/// if the configuration is in a consistent state
		/// </remarks>
		public float position {
				get {
						return m_position;
				}
				set {
						if (consistent) {
								if ((value > activation && increasing) ||
									(value < activation && !increasing)) {
										m_activated = true;
								}
								if ((value < persistence && increasing) ||
									(value > persistence && !increasing)) {
										m_activated = false;
								}
						}
						m_position = value;
				}
		}

		/// <remarks>
		/// Changing activated state will modify m_position if necessary.
		/// Because the hysteresis interval is closed, there exist closest consistent states.
		/// </remarks>
		public bool activated {
				get {
						return m_activated;
				}
				set {
						if (consistent) {
								if (value) {
										if ((m_position < persistence && increasing) ||
											(m_position > persistence && !increasing)) {
												// Set position to closest possible consistent state
												m_position = persistence;
										}
								} else {
										if ((m_position > activation && increasing) ||
											(m_position < activation && !increasing)) {
												// Set position to closest possible inactive
												m_position = activation;
										}
								}
						}
						m_activated = value;
				}
		}

		public bool consistent {
				get {
						return 
						(persistence <= activation && increasing) || 
						(persistence >= activation && !increasing);
				}
		}

		public float interpolated {
				get {
						if ((persistence >= activation && increasing) || 
							(persistence <= activation && !increasing)) {
								// Interpolation is not possible:
								// either persistence == activation, or consistent == false
								return m_activated ? 1f : 0f;
						}
						if ((m_position >= activation && increasing) ||
							(m_position <= activation && !increasing)) {
								return 1f;
						}
						if ((m_position <= persistence && increasing) ||
							(m_position >= persistence && !increasing)) {
								return 0f;
						}
						return (m_position - persistence) / (activation - persistence);
				}
		}
}
