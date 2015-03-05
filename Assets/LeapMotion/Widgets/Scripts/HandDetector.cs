using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
  [RequireComponent(typeof(BoxCollider))]
  public class HandDetector : MonoBehaviour
  {
    [HideInInspector]
    public GameObject
      target = null;

    private bool IsHand (Collider other)
    {
      return other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel> ();
    }

    public void ResetTarget ()
    {
      target = null;
    }

    void OnTriggerEnter (Collider other)
    {
      if (target != null)
        return;

      if (IsHand (other)) {
        target = other.gameObject;
      }
    }

	void OnTriggerExit (Collider other) {
      if (target == other.gameObject) {
        target = null;
      }
    }

    void OnDisable ()
    {
      target = null;
    }

    void Awake ()
    {
      GetComponent<BoxCollider> ().isTrigger = true;
    }
  }
}

