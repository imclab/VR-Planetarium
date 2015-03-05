using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Leap;
using UnityEngine.UI;
using LMWidgets;

namespace WidgetShowcase
{
	public class ArmHUDMGR : MonoBehaviour, ModalitySubscriber {
	
		//ArmHUD States
		public const string STATE_NAME_ARMHUD = "ArmHUD Manager State";
		public const string ARMHUDSTATE_START = "Start";
		public const string ARMHUDSTATE_NOLEFTHAND = "NOLEFTHAND";
		public const string ARMHUDSTATE_STATUS_ETC = "Status screen visible";
		public const string ARMHUDSTATE_SETTINGS_ONLY = "Settings screen Visible";
		public const string ARMHUDSTATE_2HANDSEDIT = "2nd Hand Ready";
		public const string ARMHUDSTATE_PANELVISIBLE = "Panel Visible";
		public const string ARMHUDSTATE_DROPPED = "Waiting for hand to return";
		public State ArmHUDState;
		
		private const string ARMHUD = "ARMHUD";
		
		public float DropDelay = 2.0f;
		public Transform DroppedLerpTarget;
		public AnimationCurve DroppedLerpCurve;
		
		public bool ArmHUDisOpen = false;
		public GameObject GraphicsPlane;
		public Texture2D StatusETC;
		public Texture2D SettingsETC;
		public GameObject StatusCanvas;
		public GameObject SettingsCanvas;
		
		public HandController handController;
//		public Animation m_animation;
//		public Animation eventsAnimation;
		public Animator ArmHUDAnimator;
		public Animator ArmHUDeventsAnimator;
		public GameObject ArmHUDgeom;
		public Transform ArmHUDbaseLookAtGRP;
//		public Transform LookTarget;
		public Transform Redirect;
		public rightAxisLookAt RightAxisLookAt;
		public string forearmName = "forearm";
		
		public GameObject PanelButtonsPanel;
		public float PanelRotationYOffset = 0.0f;
		[SerializeField]
		private List<ButtonDemoToggle> m_panelButtons;
		[SerializeField]
		private List<GameObject> m_widgetPanels;
    public Dictionary<ButtonDemoToggle, TogglePanelDataBinder> PanelDataBinderDictionary = new Dictionary<ButtonDemoToggle, TogglePanelDataBinder>();
//    public Dictionary<ButtonDemoToggle, GameObject> PanelDataBinderDictionary = new Dictionary<ButtonDemoToggle, GameObject>();
    
		
		[SerializeField]
		private List<ButtonDemoToggle> m_SliderToggleButtons;
		[SerializeField]
		private List<SliderDemo> SlidersWithToggle;
		public Dictionary<ButtonDemoToggle, SliderDemo> ToggleSliderlDictionary = new Dictionary<ButtonDemoToggle, SliderDemo>();
    
    [SerializeField]
    private List<TogglePanelDataBinder> m_TogglePanelDataBinders;
		
		public TimeAndLocationHandler timeAndLocationHandler;
		public Text SettingsPanelDateTimeText;
		public Text StatusPanelDateTimeText;
		public Text StatusPanelTimeText;
		
		public AsterismBrightnessDataBinder asterismBrightnessDataBinder;
		public Text AsterismnBrightnessText;
    public StarNameOpacityDataBinder starNameBrightnessDataBinder;
		public Text StarNameBrightnessText;
		public MilkyWayIntensityDataBinder milkyWayIntensityDataBinder;
		public Text MilkyWayIntensityText;
		public LuminenceFilterDataBinder luminanceFilterDataBinder;
		public Text LuminanceFilterText;
		public StarSaturationDataBinder starSaturationDataBinder;
		public Text StarSaturationText;
    public DepthControlDataBinder depthControlDataBinder;
		public Text DepthControlDataBinderText;
		
		void Awake () {
			WristGraphicsOff();
		}
		
		// Use this for initialization
		void Start () {
//      CloseAllPanels ();
      RightAxisLookAt = ArmHUDbaseLookAtGRP.GetComponentInChildren<rightAxisLookAt>();//Make this better
			InitArmHUDState ();
			ArmHUDAnimator.StopPlayback();
			ArmHUDeventsAnimator.StopPlayback();
			WristGraphicsOff();
			
			for (int numItem = 0; numItem < m_panelButtons.Count; numItem++){
				PanelDataBinderDictionary.Add(m_panelButtons[numItem], m_TogglePanelDataBinders[numItem]);
				m_panelButtons[numItem].StartHandler += OnPanelButtonPressed;
			}
			for (int numItem = 0; numItem < m_SliderToggleButtons.Count; numItem++){
				ToggleSliderlDictionary.Add(m_SliderToggleButtons[numItem], SlidersWithToggle[numItem]);
//				m_SliderToggleButtons[numItem].StartHandler += OnSliderToggleButtonPressed;
			}
			PanelButtonsPanel.transform.Rotate(0f, PanelRotationYOffset, 0f);
			
			Subscribe();
			UpdateArmHUDGUIvalues();
		}
		
		// Update is called once per frame
		void Update () {
			HandModel model0;
			HandModel model1;
			
			if(handController.GetAllGraphicsHands ().Length == 1){
				model0 = handController.GetAllGraphicsHands()[0];
				if(model0.GetLeapHand().IsLeft == true){
					AlignArmHUD(model0);
				}
				else if(ArmHUDState.state == ARMHUDSTATE_PANELVISIBLE) {
				}
				else{
					if(ArmHUDisOpen == true){
						ArmHUDState.Change(ARMHUDSTATE_DROPPED);
					}
				}
			}
			else if(handController.GetAllGraphicsHands().Length > 1){
				// FIXME: This should either persist based on ID
				// or should require a single left hand.
				model0 = handController.GetAllGraphicsHands()[0];
				model1 = handController.GetAllGraphicsHands()[1];
				if(model0.GetLeapHand().IsLeft &&
				   model1.GetLeapHand().IsRight){
					AlignArmHUD(model0);
				}
				else if(model0.GetLeapHand().IsRight &&
				        model1.GetLeapHand().IsLeft){
					AlignArmHUD(model1);
				}
				else{
					if(ArmHUDisOpen == true){
						ArmHUDState.Change(ARMHUDSTATE_DROPPED);
					}				}
			}
			
			else {
				if(ArmHUDisOpen == true){
					ArmHUDState.Change(ARMHUDSTATE_DROPPED);
				}			}
			if(ArmHUDState.state == ARMHUDSTATE_PANELVISIBLE){
				UpdateArmHUDGUIvalues();
			}
			
			
		}
		
		void AlignArmHUD (HandModel handModel){
			ArmHUDgeom.SetActive(true);
			Transform forearm = handModel.transform.Find (forearmName);
			if (forearm != null) {
				transform.position = forearm.position;
				transform.rotation = forearm.rotation;
			} else {
				transform.position = handModel.GetArmCenter();
				transform.rotation = handModel.GetArmRotation();
			}
			if(ArmHUDisOpen == false){//Change this to state transition from ARMHUDSTATE_NOLEFTHAND -> ARMHUDSTATE_STATUS_ETC
				if(Activate()){
					ArmHUDAnimator.Play("Take 001_Opening");
					ArmHUDeventsAnimator.Play("WristGraphicsEvents");
					ArmHUDisOpen = true;
				}
				else{
					ArmHUDState.Change (ARMHUDSTATE_STATUS_ETC);
					return;
				}
			}
//			ArmHUDbaseLookAtGRP.LookAt(LookTarget);
			Vector3 localEulerAngles = ArmHUDbaseLookAtGRP.localEulerAngles;
			if(ArmHUDisOpen == true){
				if(localEulerAngles.x < 340.0f && localEulerAngles.x > 280.0f){
					ArmHUDState.Change (ARMHUDSTATE_STATUS_ETC);
				}
				else if(localEulerAngles.x >26.0f && localEulerAngles.x < 80.0f && ArmHUDState.state != ARMHUDSTATE_PANELVISIBLE)
				{
					ArmHUDState.Change (ARMHUDSTATE_SETTINGS_ONLY);
				}
			}
//			localEulerAngles.y = 90f;
//			localEulerAngles.z = 0f;
//			ArmHUDbaseLookAtGRP.localEulerAngles = localEulerAngles;
		}
		
		public void InitArmHUDState ()
		{
	//		handController = GameObject.FindObjectOfType<HandController> () as HandController;
	//		LookAtTarget = GameObject.Find ("OVRCameraRig") as GameObject;
			
			
			if (!StateList.HasList (STATE_NAME_ARMHUD))
				InitArmHUDStateList ();
			ArmHUDState = new State (STATE_NAME_ARMHUD);
			ArmHUDState.StateChangedEvent += OnArmHUDStateChange;
			ArmHUDState.Change (ARMHUDSTATE_NOLEFTHAND);
		}
		void InitArmHUDStateList ()
		{
			if (!StateList.HasList (STATE_NAME_ARMHUD))
				StateList.Create (
					STATE_NAME_ARMHUD,
					ARMHUDSTATE_START,
					ARMHUDSTATE_NOLEFTHAND,
					ARMHUDSTATE_STATUS_ETC,
					ARMHUDSTATE_SETTINGS_ONLY,
					ARMHUDSTATE_2HANDSEDIT,
					ARMHUDSTATE_PANELVISIBLE,
					ARMHUDSTATE_DROPPED
					);
			
		}
		
		void OnArmHUDStateChange (StateChange change)
		{
			if (change.unchanged || (!change.allowed)) {
//				Debug.Log ("ignoring change " + change.ToString ());
				return;
			}
			//Debug.Log ("ArmHUDstate.Change: " + change.ToString ());
			
			switch (change.toState.name) {
			case ARMHUDSTATE_NOLEFTHAND: 
				RightAxisLookAt.IsFilterOn = false;
				CloseArmHUD ();
				Deactivate();
				StopCoroutine("droppedLerp");
				break;
				
			case ARMHUDSTATE_STATUS_ETC:
//				ChangeGraphicTo(StatusETC);
				CloseAllPanels ();
				PanelButtonsPanel.SetActive(false);
				RightAxisLookAt.IsFilterOn = false;
				StatusCanvas.SetActive(true);
				SettingsCanvas.SetActive(false);
				DropDelay = .5f;
				StopCoroutine("droppedLerp");
				break;
				
			case ARMHUDSTATE_SETTINGS_ONLY: 
//				ChangeGraphicTo(SettingsETC);
				PanelButtonsPanel.SetActive(true);
				RightAxisLookAt.IsFilterOn = true;
				StatusCanvas.SetActive(false);
				SettingsCanvas.SetActive(true);
				DropDelay = 1.0f;
				StopCoroutine("droppedLerp");
				break;
			case ARMHUDSTATE_PANELVISIBLE:
				RightAxisLookAt.IsFilterOn = true;
				DropDelay = 3.0f;
				StopCoroutine("droppedLerp");
				break;
			case ARMHUDSTATE_DROPPED:
				StartCoroutine(droppedCounter(DropDelay));
				StartCoroutine("droppedLerp", DropDelay);
				break;
			}
			

		}
		
		void CloseArmHUD (){
//			Debug.Log ("CloseArmHUD");
			ArmHUDAnimator.StopPlayback();
			ArmHUDeventsAnimator.StopPlayback();
			ArmHUDeventsAnimator.Play ("WristGraphicsOff");
//			WristGraphicsOff();
			ArmHUDisOpen = false;
			ArmHUDgeom.SetActive(false);
//			CloseAllPanels ();
		}

		
		public void ChangeGraphicTo (Texture2D texture){
//			GraphicsPlane.renderer.material.mainTexture = texture;

			StatusCanvas.SetActive(!StatusCanvas.activeSelf);
			SettingsCanvas.SetActive(!SettingsCanvas.activeSelf);
			
		}
		
		//needed to separate these to be able to call them from the Animation clip timeline
		public void WristGraphicsOn (){
			GraphicsPlane.renderer.enabled = true;
		}
		public void WristGraphicsOff (){
			GraphicsPlane.renderer.enabled = false;
		}
		
		
		private void OnPanelButtonPressed (object sender, LMWidgets.EventArg<bool> args){
			  ChangeActivePanel(sender as ButtonDemoToggle);
		}
		private void OnSliderToggleButtonPressed (object sender, LMWidgets.EventArg<bool> args){
			//Debug.Log("ButtonPressed: " + args.CurrentValue);
//			ToggleSlider(sender as ButtonDemoToggle);
		}
	
		public void ChangeActivePanel (ButtonDemoToggle clickedButton) {
//      Debug.Log (clickedButton.transform.parent.name + ".ChangeActivePanel() - 1");
			ArmHUDState.Change(ARMHUDSTATE_PANELVISIBLE);
			foreach(ButtonDemoToggle button in m_panelButtons){
				if(button == clickedButton){
					TogglePanelDataBinder panelDataBinder = PanelDataBinderDictionary[button].GetComponent<TogglePanelDataBinder>();
          if(panelDataBinder.GetCurrentData() == false){
						ArmHUDState.Change(ARMHUDSTATE_SETTINGS_ONLY);
//            panelDataBinder.SetCurrentData(true);
//					} else {
//            panelDataBinder.SetCurrentData(true);
          }
				}
				else {
            TogglePanelDataBinder panelDataBinder = PanelDataBinderDictionary[button].GetComponent<TogglePanelDataBinder>();
            panelDataBinder.SetCurrentData(false);
				}				
			}
		}
		public void ToggleSlider (ButtonDemoToggle clickedButton) {
			//Debug.Log ("Toggle clickedButton: " + PathName.Path (clickedButton.gameObject));
			SliderDemo sliderToToggle = ToggleSliderlDictionary[clickedButton];
			//toggle the slider	
			sliderToToggle.gameObject.SetActive( !sliderToToggle.gameObject.activeSelf);				
 		}				
	
		public void CloseAllPanels () {
			foreach(ButtonDemoToggle button in m_panelButtons){
        TogglePanelDataBinder panelDataBinder = PanelDataBinderDictionary[button].GetComponent<TogglePanelDataBinder>();
//				panel.SetActive(false);
//      find a replacement for this
        panelDataBinder.SetCurrentData(false);
      }	
		}

		#region ModalitySubscriber implementation
		
		public void Modality (ModalityManager.ModalityMessage message)
		{
			//throw new System.NotImplementedException ();
		}
		
		public void Subscribe ()
		{
			ModalityManager.Instance.Subscribe (ARMHUD, this.gameObject);
		}
		
		public void Unsubscribe ()
		{
			ModalityManager.Instance.Unsubscribe (ARMHUD);
		}
		
		public bool Activate ()
		{
			bool a = ModalityManager.Instance.Activate (ARMHUD);
//			Debug.Log (string.Format ("Activation of ARMHUD: {0}, current owner of activation is {1}", (a ? "True" : "False"),
//			                          ModalityManager.Instance.ActiveItemName));
			return a;
		}
		
		public bool Deactivate ()
		{
			if (ModalityManager.Instance.ActiveItemName == ARMHUD)
				return ModalityManager.Instance.Deactivate (ARMHUD);
			else
				return false;
		}
		
		#endregion
		
		IEnumerator droppedCounter(float dropDelay)
		{	
			yield return new WaitForSeconds(dropDelay);				
			if(ArmHUDState.state == ARMHUDSTATE_DROPPED){
				ArmHUDState.Change(ARMHUDSTATE_NOLEFTHAND);
 			}
		}
		IEnumerator droppedLerp(float dropDelay) {
			float timer = 0.0f;
//			Quaternion newRotation = Quaternion.Euler(90f, 0f, 0f);
			
			while (timer <= dropDelay) {
				transform.position = Vector3.Lerp (transform.position, DroppedLerpTarget.position, DroppedLerpCurve.Evaluate(timer/dropDelay));
//				transform.localRotation = Quaternion.Lerp(transform.localRotation, newRotation, DroppedLerpCurve.Evaluate(timer/dropDelay));
				timer += Time.deltaTime;
				yield return null;
			}
		}
		public void UpdateArmHUDGUIvalues(){
			SettingsPanelDateTimeText.text = timeAndLocationHandler.DateAndTime.ToLongDateString();
			StatusPanelDateTimeText.text = timeAndLocationHandler.DateAndTime.ToLongDateString();
			StatusPanelTimeText.text = timeAndLocationHandler.DateAndTime.ToShortTimeString() + " GMT";
			AsterismnBrightnessText.text = Convert.ToInt32( (asterismBrightnessDataBinder.GetCurrentData() * 100f)).ToString() + "%";
			StarNameBrightnessText.text = Convert.ToInt32( (starNameBrightnessDataBinder.GetCurrentData() * 100f)).ToString() + "%";
			MilkyWayIntensityText.text = Convert.ToInt32( (milkyWayIntensityDataBinder.GetCurrentData() * 100f)).ToString() + "%";
			LuminanceFilterText.text = Convert.ToInt32( (luminanceFilterDataBinder.GetCurrentData() * 100f)).ToString() + "%";
			StarSaturationText.text = Convert.ToInt32( (starSaturationDataBinder.GetCurrentData() * 100f)).ToString() + "%";
			DepthControlDataBinderText.text = Convert.ToInt32( (depthControlDataBinder.GetCurrentData() * 100f)).ToString() + "%";
		}
		
	}
	
	
}




