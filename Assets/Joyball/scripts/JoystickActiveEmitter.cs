using UnityEngine;
using System.Collections;

/**
 * This class only emits the handPump value if the facing handler is true; further, setting
 * facing handler to true sets handPumpToggle to false;
 * note that modality and the boolean value of this class are now synchronized;
 * the only way to activate the joyball is achieving modality, 
 * and once activated the only way 
 * to close the joyball is to close modality. 
*/

namespace WidgetShowcase
{
  public class JoystickActiveEmitter : BoolEmitter, ModalitySubscriber
  {
    public FrameEmitter frameEmitter;
    public BoolEmitter HandFacingInput;
    public BoolEmitter HandPumpToggle;
    public HorizonDetector OverEarth;
    public JoyballZone InTheZone;
    public TimeFilter TimeGate;
    const string JOYBALL = "Joyball";
    State handCountState;
    const string JAX_NO_HANDS = "no hands";
    const string JAX_HANDS = "hands";
    
    #region loop
    void Start ()
    {
      handCountState = StateList.Init ("JAE handCountState", JAX_NO_HANDS, JAX_HANDS);
      HandFacingInput.BoolEvent += FacingHandler;
      HandPumpToggle.BoolEvent += HandlePumpToggle;
      Subscribe ();
      frameEmitter.FrameEvent += FrameEventHandler;
      handCountState.StateChangedEvent += HandCountStateChangeHandler;
    }
    
    void Update ()
    {
      
    }
    
    #endregion
    
    bool IsOverEarth ()
    {
      return OverEarth.IsAboveHorizon;
    }
    
    bool IsInTheZone () {
    	return InTheZone.IsRHandInZone;
    }
    
    bool IsPointedAwayFromUser ()
    {
      return (!HandFacingInput) || (!HandFacingInput.BoolValue);
    }
    
    bool IsPumpingFastEnough ()
    {
      return  c < 2 || TimeGate.IntValue == TimeFilter.SQ_GOOD;
    }
    
    int c = 0;
    
    void HandlePumpToggle (object sender, WidgetEventArg<bool> e)
    {
//      Debug.Log (string.Format ("_____ HAND PUMP TOGGLEL {0} _____", ++c));
      
  //    Debug.Log (string.Format ("HandPumpToggle Listener: IsOver = {0}, AwayFromUser = {1}, PumpFastEnough: {2}, Pumptoggle: {3}",
    //                            IsOverEarth (),
      //                          IsPointedAwayFromUser (),
        //                        IsPumpingFastEnough (),
          //                      e.CurrentValue));

      if (IsOverEarth () && IsInTheZone() && IsPointedAwayFromUser () && Value != true) {
        Activate ();
      } else {
        if (!IsOverEarth ()) {
          //Debug.Log ("NOT OVER EARTH");
        }
        if (!IsPointedAwayFromUser ()) {
          //Debug.Log ("NOT POINTED IN THE RIGHT DIRECTION");
        }
        if (!IsPumpingFastEnough ()) {
          //Debug.Log ("PUMPING TOO SLOW");
        }
        if(!IsInTheZone()){
		  //Debug.Log ("Not in the Zone");
	    }
		Deactivate ();
      }
    }
    
    void FacingHandler (object sender, WidgetEventArg<bool> e)
    {
      if (BoolValue && (e.CurrentValue == true)) {

        HandPumpToggle.BoolValue = false;
      }
    }

    private IEnumerator WaitForDropModality(float time = 0.1f) {
      float endTime = Time.time + time;

      while(Time.time < endTime) {
        yield return 0;
      }
      //Debug.Log("Drop Modality: " + Time.time);
      ModalityManager.Instance.Deactivate (JOYBALL);
      yield break;
    }
    
    void FrameEventHandler (object sender, WidgetEventArg<FrameData> e)
    {
      if (e.CurrentValue.CurrentFrame.Hands.Count > 0) {
        handCountState.Change (JAX_HANDS);
      } else {
        handCountState.Change (JAX_NO_HANDS);
      }
    }
    
    float lastHandTime = 0;
    public float MaxTimeWithoutHands = 0.1f;
    
    void HandCountStateChangeHandler (StateChange change)
    {
      if (change.unchanged) {
        if ( change.state == JAX_NO_HANDS && Time.time - lastHandTime > MaxTimeWithoutHands && BoolValue == true) { 
          Deactivate();
        }
        else if ( change.state == JAX_HANDS ) {
          lastHandTime = Time.time;
        }
        return;
      }
      switch (change.state) {
      case JAX_HANDS:
        lastHandTime = Time.time;
        break;
        
      case JAX_NO_HANDS:
        // do nothing
        break;
        
      default:
        Debug.Log ("Bad hand state");
        break;
      }
    }
    
    #region ModalitySubscriber implementation
    
    public void Modality (ModalityManager.ModalityMessage message)
    {
      //throw new System.NotImplementedException ();
    }
    
    public void Subscribe ()
    {
      ModalityManager.Instance.Subscribe (JOYBALL, this.gameObject);
    }
    
    public void Unsubscribe ()
    {
      ModalityManager.Instance.Unsubscribe (JOYBALL);
    }
    
    public bool Activate ()
    {
      //Debug.Log("Activate");
      //Debug.Log("Modality active item: " + ModalityManager.Instance.ActiveItemName);
      bool a = ModalityManager.Instance.Activate (JOYBALL);
      BoolValue = a;
      //Debug.Log (string.Format ("Activation of Joyball: {0}, current owner of activation is {1}", (a ? "True" : "False"), ModalityManager.Instance.ActiveItemName));
      return a;
    }
    
    public bool Deactivate ()
    {
      BoolValue = false;
//      Debug.Log("Deactivate: " + Time.time);
      StartCoroutine(WaitForDropModality());
      return true;
    }
    
    #endregion
  }
  
}