using UnityEngine;
using System.Collections;
using WidgetShowcase;

public class HorizonDetector : MonoBehaviour {

  [SerializeField]
  private HandEmitter m_rightHandEmitter;
  [SerializeField]
  private Transform m_rayRoot;

  private bool m_isAboveHorizon = false;
  public bool IsAboveHorizon { get { return m_isAboveHorizon; } }

	// Use this for initialization
	void Start () {
    m_rightHandEmitter.HandEvent += onHandEvent;
	}

  private void onHandEvent(object sender, WidgetEventArg<WidgetShowcase.HandData> e) {
    if ( !e.CurrentValue.HasHand ) { 
      m_isAboveHorizon = false; 
      return;
    }
    m_isAboveHorizon = HorizonCheck(e.CurrentValue.HandModel);
  }

  public bool HorizonCheck(HandModel hand) {
    Ray ray = new Ray(m_rayRoot.position, (hand.GetPalmPosition() - m_rayRoot.position).normalized);
    Debug.DrawRay(m_rayRoot.position, ray.direction * 10000.0f, new Color(1.0f, 0.0f,0.0f));
    RaycastHit hitInfo;
    LayerMask mask = LayerMask.GetMask("Earth");
    bool hit = Physics.Raycast(ray, out hitInfo, 10000.0f, mask);
    if ( hit ) {
      return true;
    }
    else {
      return false;
    }
  }
}
