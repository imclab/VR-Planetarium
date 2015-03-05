using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

namespace WidgetShowcase
{
		public class HAM : MonoBehaviour
		{

				public HandController Controller;
				Dictionary<int, GameObject> AurorasForHand = new Dictionary<int, GameObject> ();

				// Use this for initialization
				void Start ()
				{
	
				}
	
				// Update is called once per frame
				void Update ()
				{
						Frame f = Controller.GetFrame ();

						List<int> killIds = new List<int> ();

						foreach (int hid in AurorasForHand.Keys) {
								GameObject g = AurorasForHand [hid];
								if (!g)
										return;
								GetHandById gh = g.GetComponent<GetHandById> ();
								if ((gh != null) && (gh.FoundState != null))
								if (gh.FoundState.state == GetHandById.FOUND_STATE_HAND_INVALID) {
										killIds.Add (hid);
								}
						}
			
						foreach (int i in killIds)
								AurorasForHand.Remove (i);

						foreach (Hand h in f.Hands) {
								if (!AurorasForHand.ContainsKey (h.Id)) {
										AurorasForHand [h.Id] = NewAurora (h.Id);
//										Debug.Log ("Creating Aurora for hand Id " + h.Id);
								}
						}

				}

				GameObject NewAurora (int id)
				{
						GameObject ah = (GameObject)Instantiate ((GameObject)Resources.Load ("Aurora/Aurora"));
						//Debug.Log ("Made aurora from game object " + ah.name);
						ah.transform.parent = transform;
						ah.transform.localPosition = Vector3.zero;
						ah.transform.localRotation = Quaternion.identity;
						ah.transform.localScale = Vector3.one;

						GetHandById gh = ah.GetComponent<GetHandById> ();
						gh.Controller = Controller;
						gh.handId = id;
						gh.KillIfInvalid = true;

						return ah;
				}
		}
	
}