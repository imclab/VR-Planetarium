using UnityEngine;
using System.Collections;

public class Highlighter : MonoBehaviour {

  [SerializeField]
  private Light earthLight;
  [SerializeField]
  private float minIntensity = 1.0f;
  [SerializeField]
  private float maxIntensity = 2.0f;
  [SerializeField]
  private float maxHighlight;
  [SerializeField]
  private float minHighlight;
  [SerializeField]
  private float transitionSeconds;
  [SerializeField]
  private AnimationCurve transitionCurve;
  [SerializeField]
  private WidgetShowcase.HandEmitter handEmitter;
  [SerializeField]
  private WidgetShowcase.RayCastEmitter raycastEmitter;

  public static Highlighter Instance;
	
	[SerializeField]
  private bool m_isActive;

  private float m_startAlpha;
  private float m_startIntensity;
  private float m_startTime;

  public bool Active { 
    get { return m_isActive; } 
    set { 
      if ( m_isActive == value ) { return; }
      m_isActive = value; 
      m_startAlpha = renderer.material.color.a;
      m_startIntensity = earthLight.intensity;
      m_startTime = Time.time;
      //Debug.Log("Switch to: " + m_isActive);
    } 
  }

  void Awake() {
    if ( Instance == null ) {
    }
    else {
      Debug.LogWarning("Attempting to create multiple Highlighter Instances.");
    }
  }

	// Use this for initialization
	void Start () {
	  if ( handEmitter != null ) {
      handEmitter.HandEvent += onHandEvent;
    }
	}

  private void onHandEvent(object sender, WidgetShowcase.WidgetEventArg<WidgetShowcase.HandData> e) {
    if ( WidgetShowcase.ModalityManager.Instance.HasActiveItem() ) { 
      if ( WidgetShowcase.ModalityManager.Instance.ActiveItemName == "ARMHUD" ) {
        Active = false;
      }
      return; 
    }

    if ( e.CurrentValue.HasHand && raycastEmitter != null) {
      if ( raycastEmitter.RayCheck(e.CurrentValue.HandModel.GetPalmPosition()) ) {
        Active = true;
      }
      else {
        Active = false;
      }
    }
    else {
      Active = false;
    }
  }
  	
	// Update is called once per frame
	void Update () {

    if ( Input.GetKeyUp(KeyCode.V) ) {
      Active = !Active;
    }

    float newHighlight = 0.0f;
    float newIntensity = 0.0f;
    float goalHighlight = m_isActive ? maxHighlight : minHighlight;
    float goalIntensity = m_isActive ? maxIntensity : minIntensity;

    if ( Time.time - m_startTime >= transitionSeconds ) { 
      newHighlight = goalHighlight;
      newIntensity = goalIntensity;
    }
    else {
      float percent = (Time.time - m_startTime) / (transitionSeconds);
      percent = transitionCurve.Evaluate(percent);
      newHighlight = m_startAlpha + (percent * (goalHighlight - m_startAlpha));
      newIntensity = m_startIntensity + (percent * (goalIntensity - m_startIntensity));
    }

    if ( goalHighlight != renderer.material.color.a ) {
      Color temp = renderer.material.color;
      temp.a = newHighlight;
      renderer.material.color = temp;
    }

    if ( goalIntensity != earthLight.intensity ) {
      earthLight.intensity = newIntensity;
    }
	}
}
