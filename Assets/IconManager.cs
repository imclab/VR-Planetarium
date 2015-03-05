using UnityEngine;
using System.Collections;
using WidgetShowcase;

public class IconManager : MonoBehaviour {

  private delegate IEnumerator TransitionDelegate(TransitionDelegate next = null);

  [SerializeField]
  private GetRightHand m_rightHandEmmitter;
  [SerializeField]
  private RayCastEmitter m_raycastEmitter;
//  [SerializeField]
  public float IconOffsetDistance;
  [SerializeField]
  private float m_transitionTime;
  [SerializeField]
  private AnimationCurve m_inCurve;
  [SerializeField]
  private AnimationCurve m_outCurve;
  [SerializeField]
  private GameObject m_constellationIcon;
  [SerializeField]
  private GameObject m_joyballIcon;
  [SerializeField]
  private GameObject m_grabIcons;
  [SerializeField]
  private Texture2D m_grabIconOpen;
  [SerializeField]
  private Texture2D m_grabIconClosed;

  private bool m_wasAboveHorizon;

  private GameObject m_activeObject; 

	// Use this for initialization
	void Start () {
    m_activeObject = m_joyballIcon;

    if ( m_rightHandEmmitter != null ) {
      m_rightHandEmmitter.HandEvent += onHandEvent;
    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  private void onHandEvent(object sender, WidgetEventArg<HandData> e) {
    if ( !e.CurrentValue.HasHand ) { 
      if ( m_activeObject != null) { m_activeObject.renderer.enabled = false; }
      return;
    }
    else { 
      if ( m_activeObject != null) { m_activeObject.renderer.enabled = true; }
    }

    HandModel hand = e.CurrentValue.HandModel;

    transform.rotation = hand.GetPalmRotation();
    transform.position = hand.GetPalmPosition() + (transform.up * IconOffsetDistance);

    // Transition logic below here.
    if ( ModalityManager.Instance.HasActiveItem() ) { 
      if ( ModalityManager.Instance.ActiveItemName == "ARMHUD" && m_activeObject != null || ModalityManager.Instance.ActiveItemName == "Joyball" && m_activeObject != null) {
        m_activeObject = null;
        //Debug.Log("Go None");
        StopAllCoroutines();
        StartCoroutine(transitionOut(switchToNone));
      }
      return; 
    }

		bool aboveHorizon = !m_raycastEmitter.RayCheck(hand.GetPalmPosition());

    if ( aboveHorizon && !m_wasAboveHorizon || (aboveHorizon && m_activeObject == null) ) {
      if ( m_activeObject == null ) { m_activeObject = m_constellationIcon; }
      StopAllCoroutines();
      StartCoroutine(transitionOut(switchToConstellation));
    }
//    else if (!aboveHorizon && m_wasAboveHorizon || (!aboveHorizon && m_activeObject == null) ) {
    else if (!aboveHorizon && m_wasAboveHorizon )  {
        
      if ( m_activeObject == null ) { m_activeObject = null; }
      StopAllCoroutines();
//      StartCoroutine(transitionOut(switchToJoyball));
  		StartCoroutine(transitionOut(switchToNone));
//      StartCoroutine(transitionOut(switchToGrabCycle));
      
			
    }

    m_wasAboveHorizon = aboveHorizon;
  }

  private IEnumerator transitionOut(TransitionDelegate next = null) {
    Vector3 startScale = transform.localScale;
    Vector3 goalScale = new Vector3(0,0,0);
    float diff = goalScale.x - startScale.x;
    float startTime = Time.time;

    while( true ) {
      float percent = Mathf.Clamp01((Time.time - startTime) / m_transitionTime);
      if ( percent == 1.0f ) { 
        transform.localScale = goalScale;
        break;
      }

      percent = m_outCurve.Evaluate(percent);
      float newScale = startScale.x + (percent * diff);
      transform.localScale = new Vector3(newScale, newScale, newScale);

      yield return 0;
    }

    if ( next != null ) {
      StartCoroutine(next());
    }

    yield break;
  }

  private IEnumerator transitionIn(TransitionDelegate next = null) {
    if ( m_activeObject == null ) { // Nothing rendering to fade out.
      if ( next != null ) {
        StartCoroutine(next());
      }
      yield break; 
    } 

    Vector3 startScale = transform.localScale;
    Vector3 goalScale = new Vector3(1,1,1);
    float diff = goalScale.x - startScale.x;
    float startTime = Time.time;
    
    while( true ) {
      float percent = Mathf.Clamp01((Time.time - startTime) / m_transitionTime);
      
      if ( percent == 1.0f ) { 
        transform.localScale = goalScale;
        break;
      }
      
      percent = m_inCurve.Evaluate(percent);
      float newScale = startScale.x + (percent * diff);
      transform.localScale = new Vector3(newScale, newScale, newScale);
      
      yield return 0;
    }
    
    if ( next != null ) {
      StartCoroutine(next());
    }
    
    yield break;
  }

  private IEnumerator switchToConstellation(TransitionDelegate next = null) {
    if ( next != null ) {
      Debug.LogWarning("Ignoring next function");
    }

    m_activeObject = m_constellationIcon;
    m_joyballIcon.renderer.enabled = false;
    m_constellationIcon.renderer.enabled = true;
    StartCoroutine(transitionIn());
    yield break;
  }

  private IEnumerator switchToJoyball(TransitionDelegate next = null) {
    if ( next != null ) {
      Debug.LogWarning("Ignoring next function");
    }

    m_activeObject = m_joyballIcon;
    m_joyballIcon.renderer.enabled = true;
    m_constellationIcon.renderer.enabled = false;
    StartCoroutine(transitionIn());
    yield break;
  }

  private IEnumerator switchToNone(TransitionDelegate next = null) {
    if ( next != null ) {
      Debug.LogWarning("Ignoring next function");
    }

    m_activeObject = null;
    m_joyballIcon.renderer.enabled = false;
    m_constellationIcon.renderer.enabled = false;
    m_grabIcons.renderer.enabled = false;
    yield break;
  }
  
	private IEnumerator switchToGrabCycle(TransitionDelegate next = null) {
		if ( next != null ) {
			Debug.LogWarning("Ignoring next function");
		}
		m_activeObject = m_grabIcons;
    m_grabIcons.renderer.enabled = false;
    m_joyballIcon.renderer.enabled = false;
    m_constellationIcon.renderer.enabled = false;
    StartCoroutine(transitionIn());
		yield break;
	}  
  public void ToggleGrabIconCycle (bool onOff) {
  	if(onOff == true) {
      StartCoroutine(transitionIn( switchToGrabCycle));
//		m_joyballIcon.renderer.enabled = false;
//		m_constellationIcon.renderer.enabled = false;
  		m_grabIcons.renderer.enabled = true;
//		m_activeObject = m_grabIcons;
  		StartCoroutine(GrabIconCycle());
  	}
  	if(onOff == false){
//  		StopCoroutine(GrabIconCycle());
//      StopAllCoroutines();
//		m_joyballIcon.renderer.enabled = true;
//		m_constellationIcon.renderer.enabled = false;
//  		m_grabIcons.renderer.enabled = false;
//  		m_activeObject = null;
      StartCoroutine(switchToNone());
  	}
  }
  
  private IEnumerator GrabIconCycle () {
	
	while (m_activeObject == m_grabIcons){
	  	yield return new WaitForSeconds(1.0f);
	  	m_grabIcons.renderer.material.mainTexture = m_grabIconClosed;
		yield return new WaitForSeconds(.5f);
		m_grabIcons.renderer.material.mainTexture = m_grabIconOpen;	
  	}
  }
}





