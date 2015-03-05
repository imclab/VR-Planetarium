using UnityEngine;
using System.Collections;
using Leap;

/***
This script will emit a right hand from a controller on a repeat basis.
Once one right hand is found it will be preferred.
*/

namespace WidgetShowcase
{
		public class GetRightHand : HandEmitter
		{
    [SerializeField]
    private bool ReturnPhysicsHand;
				public FrameEmitter frameEmitter;
				public int lastHandId = -1;

				// Use this for initialization
				void Awake ()
				{
						if (frameEmitter) {
								frameEmitter.FrameEvent += HandleFrameEvent;
						}

				}

				void Start ()
				{
						if (EmitOnStart) {
								EmitHand ();
						}
				}

				void HandleFrameEvent (object sender, WidgetEventArg<FrameData> e)
				{
						HandModel oldFound = null;
			
// attempt to reconnect to the last good id
      HandModel[] handList = ReturnPhysicsHand ? e.CurrentValue.PhysicsModels : e.CurrentValue.HandModels;

      foreach (HandModel handInScene in handList) {
								if (handInScene.GetLeapHand ().Id == lastHandId) {
										oldFound = handInScene;
								}
						}
			
// on failure attempt to find a new good left hand
						if (!oldFound) {
								CurrentHand = FirstRightHand (handList);
								if (CurrentHand) {
										lastHandId = CurrentHand.GetLeapHand ().Id;
								}
						} else {
								CurrentHand = oldFound;
						}
				}

				HandModel FirstRightHand (HandModel[] handsInScene)
				{
						foreach (HandModel hand in handsInScene) {
								if (hand.GetLeapHand ().IsRight)
										return hand;
						}

						return null;
				}
		}
	
}
