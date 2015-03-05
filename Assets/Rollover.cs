using UnityEngine;
using System.Collections;

public class Rollover : MonoBehaviour {

  [SerializeField]
  private Camera m_leftCam;
  [SerializeField]
  private GripToggle m_gripToggle;
  [SerializeField]
  private WidgetShowcase.HandEmitter m_hand;

  void OnDisable() {
    Asterisms.AsterismDrawer.DisableAllAsterisms();
  }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
    if ( m_hand.CurrentHand == null ) { 
      Asterisms.AsterismDrawer.DisableAllAsterisms();
      return;
    }

    //Asterisms.AsterismDrawer.DisableAllAsterisms();
	  if ( m_gripToggle.enabled && WidgetShowcase.ModalityManager.Instance.CanActivate(gameObject) ) {
      Vector2 screenPoint = m_leftCam.WorldToScreenPoint(m_hand.CurrentHand.GetPalmPosition());
      Ray throughHand = m_leftCam.ScreenPointToRay(screenPoint);
      
      //Debug.DrawRay(m_leftCam.transform.position, m_leftCam.ScreenPointToRay(screenPoint).direction * 1000.0f);
      
      RaycastHit castInfo;
      bool hit = Physics.Raycast(throughHand, out castInfo, float.PositiveInfinity, LayerMask.GetMask("AsterismColliders"));
      
      if(hit) {
        AsterismReference asterismRef = castInfo.transform.GetComponent<AsterismReference>();
        if ( asterismRef != null ) {
          Asterisms.AsterismParser.AsterismData[asterismRef.index].IsSelected = true;
        }
      }
    }
	}
}
