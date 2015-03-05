using UnityEngine;
using System.Collections;
using Leap;

namespace WidgetShowcase
{
/// <summary>
/// a wrappher for hands, or absence thereof, for hand emission.
/// </summary>
		public struct HandData
		{
				public HandModel HandModel;

				public int Id {
						get {
								if (!HasHand)
										return -1; 
								return LeapHand.Id;
						}
				}

				public Hand LeapHand {
						get {
								return HasHand ? HandModel.GetLeapHand () : null;
						}
				}

				public bool HasHand {
						get {
								return HandModel != null;
						}
				}

				public HandData (HandModel hand)
				{
						HandModel = hand;
				}

				public HandData (bool HasHand)
				{ // parameter is a placeholder, as you can't have unparameterized struct constructors.
						HandModel = null;
				}
		}

}