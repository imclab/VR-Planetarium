using UnityEngine;
using System.Collections;
using WidgetShowcase;

/// <summary>
/// When enabled, hand will pull in first PullToHold object with which it is aligned.
/// </summary>
public class PullObject : MonoBehaviour, ModalitySubscriber
{
  [SerializeField]
  private Vector3 holdPositionOffset;
  [SerializeField]
  private HandEmitter m_hand;
  public Camera m_leftCam;
  //public HandModel m_handModel;
  public Transform view;
  //public Transform cast;
  [HideInInspector]
  public Transform Hold = null;
  public int layerSelect = 0;
  PullToHold pulled;

  private bool attemptingToReaquire = false;


  // ***
  // MODALITY IMPLEMENTATION
  // ***

  // see Modality for messages
  public void Modality (ModalityManager.ModalityMessage message) {
    if ( ModalityManager.Instance == null ) { return; }
    // This implementation is going to be synchronous for now. No need to respond to messages.
  }

  // should be called as start;
  public void Subscribe () {
    if ( ModalityManager.Instance == null ) { return; }
    ModalityManager.Instance.Subscribe (this.gameObject);
    //Debug.Log("Subscribe: " + gameObject.GetInstanceID());
  }

  // rarely needed -- should remove item from registry. calls Modality.Unsubscribe(name, self) || Modality.Unsubscribe(self);
  public void Unsubscribe () {
    if ( ModalityManager.Instance == null ) { return; }
    ModalityManager.Instance.Unsubscribe (this.gameObject);
//    Debug.Log("Unsubscribe: PullObject " + gameObject.GetInstanceID());
  }

  // attempt to register this object as the current modalithy calls Modality.Subscribe(name, self) || Modality.Subscribe(self);
  public bool Activate () {
    if ( ModalityManager.Instance == null ) { 
      Debug.LogWarning("No Modality Manager Instance.");
      return true; 
    }
    bool result = ModalityManager.Instance.Activate (this.gameObject);
//    Debug.Log("Activation: Pull Object " + result);
    return result;
  }

  // attempt to free the modality from locking into this object. 
  public bool Deactivate () {
    if ( ModalityManager.Instance == null ) { return true; }
    StartCoroutine(WaitForDropModality());
    return true;
  }

  private IEnumerator WaitForDropModality(float time = 0.5f) {
    float endTime = Time.time + time;
    
    while(Time.time < endTime) {
      yield return 0;
    }

    ModalityManager.Instance.Deactivate (this.gameObject);
    yield break;
  }

  // ***
  // END MODALITY IMPLEMENTATION
  // **

  void Awake() {
    if ( ModalityManager.Instance != null ) { 
      Subscribe();
    }
  }

  void OnDestroy() {
    if ( ModalityManager.Instance != null ) { 
      Unsubscribe();
    }
  }

  // Identify pulled object using view of hand
  void Update ()
  {
    if (view == null) {
      return;
    }

    if ( m_hand.CurrentHand == null ) { 
      if ( pulled != null ) {
        if ( !attemptingToReaquire ) {
            StartCoroutine(reaquireHand(2.0f));
        }
      }
      return;
    }

    if (pulled != null) {
      return;
    }



    if ( Hold == null && m_hand.CurrentHand != null ) { 
      updateHold();
      //GrabberHand grabberHand = m_hand.CurrentHand.gameObject.GetComponent<GrabberHand>();
      //if ( grabberHand == null ) { return; }
      //Hold = grabberHand.hold;
    }


    Vector2 screenPoint = m_leftCam.WorldToScreenPoint(m_hand.CurrentHand.GetPalmPosition());
    Ray throughHand = m_leftCam.ScreenPointToRay(screenPoint);
    
    //Debug.DrawRay(m_leftCam.transform.position, m_leftCam.ScreenPointToRay(screenPoint).direction * 1000.0f);
    
    RaycastHit castInfo;
    bool hit = Physics.Raycast(throughHand, out castInfo, float.PositiveInfinity, LayerMask.GetMask("AsterismColliders"));
    
    if(hit) {

      if ( !Activate() ) { 
        this.enabled = false;
        return; 
      } 

      AsterismReference asterismRef = castInfo.transform.GetComponent<AsterismReference>();
      if ( asterismRef != null ) {
        Asterisms.Asterism hitAsterism = asterismRef.Asterism;

        PullToHold intersect = hitAsterism.mover.GetComponent<PullToHold>();

        if ( intersect.cameraPoint == null ) { intersect.cameraPoint = m_leftCam.transform; }

        if (intersect != null) {
          SelectStars(hitAsterism.mover);
          Acquire (intersect);
        }
      }
    }
  }

  private void SelectStars(GameObject jewelcase) {
    bool updatedAsterism = false;

    foreach( Stars.StarBehavior starBehavior in jewelcase.transform.GetComponentsInChildren<Stars.StarBehavior>() ) {
      if (updatedAsterism == false) {
        Stars.StarData starData = starBehavior.GetStarData();

        if ( starData != null ) {
          int index = starData.AsterismIndex;
          Asterisms.Asterism asterism = Asterisms.AsterismParser.AsterismData[index];
          //asterism.lineArt.SetWidth(3.0f);
          updatedAsterism = true;
        }
      }

      starBehavior.IsSelected = true;
    }
  }

  private void DeselectStars(GameObject jewelcase) {
    bool updatedAsterism = false;

    foreach( Stars.StarBehavior starBehavior in jewelcase.transform.GetComponentsInChildren<Stars.StarBehavior>() ) {
      if (updatedAsterism == false) {
        Stars.StarData starData = starBehavior.GetStarData();
        
        if ( starData != null ) {
          int index = starData.AsterismIndex;
          Asterisms.Asterism asterism = Asterisms.AsterismParser.AsterismData[index];
          //asterism.lineArt.SetWidth(1.0f);
          updatedAsterism = true;
        }
      }

      starBehavior.IsSelected = false;
    }
  }

  private IEnumerator reaquireHand(float time = 0.25f) {
    attemptingToReaquire = true;
    float startTime = Time.time;
   
    while(Time.time - startTime < time) { 
      if ( m_hand.CurrentHand != null ) {
        updateHold();
        attemptingToReaquire = false;
        yield break;
      }

      yield return 0;
    }

    doDetatch();
    attemptingToReaquire = false;
    yield break;
  }

  void OnDisable ()
  {
    doDetatch();
  }

  void doDetatch() {
    if (pulled != null)
      DeselectStars(pulled.gameObject);
    Release();
    Deactivate();
  }

  void Release ()
  {
    if (pulled == null)
        return;
    //NOTE: pulled reference is implicitly made null if pulled is destroyed
    pulled.holder = null;
    pulled = null;
    Hold = null;
  }

  void Acquire (PullToHold intersect)
  {
    if (intersect.holder != null) {
        intersect.holder.enabled = false;
    }

    if ( Hold == null ) { 
      throw new System.Exception("Attempting to aquire hold without holder.");
    }

    pulled = intersect;

    if (intersect == null) { return; }

    //NOTE: hold reference is implicitly made null if hold is destroyed
    pulled.holder = this;
    Hold.transform.rotation = intersect.gameObject.transform.rotation;

    if ( intersect.asterismKey != -1 && intersect.asterismKey < Asterisms.AsterismParser.AsterismData.Length ) {
      float scaleFactor = Asterisms.AsterismParser.AsterismData[intersect.asterismKey].scaleFactor;
      Hold.transform.localScale = new Vector3(0.007f,0.007f,0.007f) * scaleFactor;
    }

  }

  void updateHold() {
    if ( m_hand.CurrentHand == null ) { 
      throw new System.Exception("No current hand");
    }

    GrabberHand grabberHand = m_hand.CurrentHand.gameObject.GetComponent<GrabberHand>();
    if ( grabberHand == null ) { 
      throw new System.Exception("No GrabberHand component on hand: " + m_hand.CurrentHand.gameObject.name);
    }
    Hold = grabberHand.hold;
  }
}
