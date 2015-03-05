using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WidgetShowcase
{
		public class ModalityTestOften : MonoBehaviour, ModalitySubscriber
		{
		
				public Spouter Locked;
				public Spouter Accepted;
				public Spouter Failed;
				public Spouter Open;
				public Spouter Deactivated;
				bool isActive = false;
				public float TurnOnRepeatRate = 1.5f;
				public float InitTime = 1.0f;
				public float KeepOnRate = 0.5f;
				public Text Label;
				public float TurnOffTime = -1;
				public int MaxTimes = 3;
				public int OnTimes = 0;

				public bool IsActive {
						get {
								return isActive;
						}
						set {
								isActive = value;
								Feedback (string.Format ("IsActive = {0}", value));
								ActiveLight.renderer.material.color = value ? Color.green : Color.red;
						}
				}

				public GameObject ActiveLight;
		
				// Use this for initialization
				void Start ()
				{
						Subscribe ();
						Open.Spout (0.5f);
						IsActive = false;
						InvokeRepeating ("TryToActivate", InitTime, TurnOnRepeatRate);
				}

		
				// Update is called once per frame
				void Update ()
				{
						if (OnTimes > MaxTimes)
								CancelInvoke ();
						if (TurnOffTime > 0 && TurnOffTime < Time.time)
								Deactivate ();
				}



		#region ModalitySubscriber implementation
				public void Modality (ModalityManager.ModalityMessage message)
				{
						switch (message) {
						case ModalityManager.ModalityMessage.Locked:
								Locked.Spout (0.25f);
								break;
						case ModalityManager.ModalityMessage.Accepted:
								IsActive = true;
								Feedback ("Activation Accepted");
								Accepted.Spout (0.5f);
								++OnTimes;
								break;
						case ModalityManager.ModalityMessage.Failed:
								IsActive = false;
								Failed.Spout ();
								break;
						case ModalityManager.ModalityMessage.Open:
								Open.Spout ();
								break;
						case ModalityManager.ModalityMessage.Deactivated:
								Feedback ("Deactivation accepted");
								IsActive = false;
								Deactivated.Spout (0.5f);
								break;
						default:
								throw new System.ArgumentOutOfRangeException ();
						}
				}

				public void Subscribe ()
				{
						ModalityManager.Instance.Subscribe (this.gameObject);
				}

				public bool Activate ()
				{
						Feedback ("Activating...");
						return ModalityManager.Instance.Activate (this.gameObject);
				}

				public void Unsubscribe ()
				{
						ModalityManager.Instance.Unsubscribe (this.gameObject);
				}

				public bool Deactivate ()
				{
						TurnOffTime = -1;
						Feedback ("Deactivating...");
						return ModalityManager.Instance.Deactivate (this.gameObject);
				}

				public void TryToActivate ()
				{
						if (IsActive)
								Deactivate ();
						else {
								Activate ();
								TurnOffTime = Time.time + KeepOnRate;
						}
				}

		#endregion

				void Feedback (string deactivating)
				{
						Debug.Log (string.Format ("{0}: ModalityTestOften : {1}", Time.time, deactivating));
				}
		}
	
}