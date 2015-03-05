using UnityEngine;
using System.Collections;
using Leap;

/***
This script extracts a specific hand from the frame and emits it.
Which hand is being saught out is changeable. 
It will self destruct if the hand is invalid 
*/

namespace WidgetShowcase
{
		public class GetHandById : HandEmitter
		{
    [SerializeField]
				float KILL_DELAY = 1.5f;
				public HandController Controller;
				public IntEmitter handIdInput = null; // optional -- an input hook for changing the target hand

				long lastFrameId = -1;
				public int handId = -1;
				public State FoundState;
				public const string FOUND_STATE_NAME = "getHandById FoundState";
				public const string FOUND_STATE_NO_ID = "no id";
				public const string FOUND_STATE_NOT_FOUND = "not found";
				public const string FOUND_STATE_FOUND = "found";
				public const string FOUND_STATE_HAND_INVALID = "hand invalid";
				public bool KillIfInvalid = false;

				bool NoHandId {
						get { return handId < 0; }
				}

				// Use this for initialization
				void Start ()
				{
						FoundState = StateList.Init (FOUND_STATE_NAME, FOUND_STATE_NO_ID, FOUND_STATE_NOT_FOUND, FOUND_STATE_FOUND, FOUND_STATE_HAND_INVALID);
						FoundState.Change (!NoHandId ? FOUND_STATE_NOT_FOUND : FOUND_STATE_NO_ID);

						if (handIdInput)
								handIdInput.IntEvent += HandleIntEvent;

						FoundState.StateChangedEvent += HandleStateChangedEvent;
				}

				bool destroyed = false;

				void HandleStateChangedEvent (StateChange change)
				{
						if ((!destroyed) && (change.state == FOUND_STATE_HAND_INVALID) && KillIfInvalid) {
								Destroy (gameObject, KILL_DELAY);
								destroyed = true; 
						}
				}

				void HandleIntEvent (object sender, WidgetEventArg<int> e)
				{
						handId = e.CurrentValue;
//						Debug.Log ("GetHandById looking for hand of id " + handId);
						if (handId >= 0) FoundState.Change (FOUND_STATE_FOUND); 
// making the optimistic assumption that there is a hand of that ID In the scene and some other process found it.
				}
	
				// Update is called once per frame
				void Update ()
				{
						handFromFrame ();
				}
		
				void handFromFrame ()
				{

						if ((FoundState.state == FOUND_STATE_NO_ID) || (FoundState.state == FOUND_STATE_HAND_INVALID))
								return;
						else if (NoHandId) {
								FoundState.Change (FOUND_STATE_NO_ID);
								return;
						}

						Frame f = Controller.GetFrame ();
						if (f.Id == lastFrameId)
								return;

						if (!f.Hand (handId).IsValid) {
								FoundState.Change (FOUND_STATE_HAND_INVALID);
								return;
						}

						lastFrameId = f.Id;

						HandModel[] handsInScene = Controller.GetAllGraphicsHands ();

						HandModel oldFound = null;

						foreach (HandModel handInScene in handsInScene) {
								if ((!oldFound) && (handInScene.GetLeapHand ().Id == handId)) {
										oldFound = handInScene;
										FoundState.Change (FOUND_STATE_FOUND);
										break;
								}
						}

						if (!oldFound)
								FoundState.Change (FOUND_STATE_NOT_FOUND);

						CurrentHand = oldFound;
				}
		}
	
}
