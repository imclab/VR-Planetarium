using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{

	public class JoyballZone : MonoBehaviour {

			
		[SerializeField]		
		private bool m_isRHandInZone = false;
		public bool IsRHandInZone { get { return m_isRHandInZone; } }
		private bool IsInteracting = false;
		
		private GameObject target_ = null; // Intersecting object that controls position
		
		public Material ZoneInActiveMat;
		public Material ZoneActiveMat;
		
		public float InitialDepth;
		public float CurrentHoverDepth;
		public float HoverValue;
		
		public HorizonDetector OverEarth;
		
		public Alphan ZoneAlpha;
		public Alphan ZoneOuterAlpha;
    public Alphan JoyballArrows;
		
		public JoystickActiveEmitter joystickActiveEmitter;
		
		public IconManager WristIconManager;
		
    private bool m_isJoyzoneOn = false;
		
		// Use this for initialization
		void Start () {
        JoyzoneOff ();
    }
    
		// Update is called once per frame
		void Update () {
//			if(OverEarth.IsAboveHorizon == false && IsInteracting == false && ModalityManager.Instance.ActiveItemName == ""){
				if(OverEarth.IsAboveHorizon == false && IsInteracting == false && joystickActiveEmitter.Value == false || ModalityManager.Instance.ActiveItemName == "ARMHUD"){
	        if(m_isJoyzoneOn == true){
            Debug.Log ("JoyzoneOff");
            JoyzoneOff();
            m_isJoyzoneOn = false;
          
          }
			}
//			if(OverEarth.IsAboveHorizon == false && IsInteracting == false && ModalityManager.Instance.ActiveItemName == ""){
				if(OverEarth.IsAboveHorizon == true && IsInteracting == false && joystickActiveEmitter.Value == false && ModalityManager.Instance.ActiveItemName == ""){
          if(m_isJoyzoneOn == false){
            Debug.Log ("JoyzoneOn");
            JoyzoneOn();	
            m_isJoyzoneOn = true;        
          }
			}
			if (IsInteracting && target_ == null) {
				// Fire OnExt
				IsInteracting = false;
			}
			if (target_ == null)
				return;
			
			if(IsInteracting == true){	
				CurrentHoverDepth = (target_.transform.position - transform.position).magnitude;
				HoverValue = Mathf.Clamp01(InitialDepth - CurrentHoverDepth);
//				Debug.Log ("HoverValue = " + HoverValue);
	//			ZoneAlpha.SetAlpha(HoverValue, 0.0f);
//				WristIconManager.IconOffsetDistance = Mathf.Lerp(.04f, 3.0f, .01f);
			}
		}
		
		//Show Zone if Rhand below horizon 
		
		//Hide Zone if Rhand above horizon
		
		
		private bool IsHand (Collider other)
		{
			return other.transform.parent && other.transform.parent.parent && other.transform.parent.parent.GetComponent<HandModel> ();
		}
		
		void OnTriggerEnter (Collider other)
		{
			if (target_ == null && IsHand (other) && joystickActiveEmitter.Value == false) {
				target_ = other.gameObject;

				m_isRHandInZone = true;
//				if (dialGraphics)
//					dialGraphics.HiLightDial ();
				InitialDepth = (target_.transform.position - transform.position).magnitude;
				IsInteracting = true;
				StopAllCoroutines();
				StartCoroutine(LerpIconOffset(.12f));
				WristIconManager.ToggleGrabIconCycle(true);
			}
//			gameObject.GetComponent<Renderer>().material = ZoneActiveMat;
			
		}
		
		void OnTriggerExit (Collider other)
		{
			if (other.gameObject == target_) {
				target_ = null;
				m_isRHandInZone = false;
				IsInteracting = false;
				StopAllCoroutines();
				StartCoroutine(LerpIconOffset(0.04f));
				WristIconManager.ToggleGrabIconCycle(false);
				
			}
//			gameObject.GetComponent<Renderer>().material = ZoneInActiveMat;
		}
		private IEnumerator LerpIconOffset (float destOffset) {
			Debug.Log("Lerp wrist Icon");
			float duration = 2.0f;
			float elapsedTime = 0f;
			while(elapsedTime < duration){
				WristIconManager.IconOffsetDistance = Mathf.Lerp(WristIconManager.IconOffsetDistance, destOffset, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
		}
    
    public void JoyzoneOn (){
//      m_isJoyzoneOn = true;
      ZoneAlpha.SetAlpha(0.1f, 0.5f);
      ZoneOuterAlpha.SetAlpha(0.5f, 0.5f);
      JoyballArrows.SetAlpha(0.1f, 0.5f); 
      Collider[] colliders = GetComponentsInChildren<Collider>();
      foreach(Collider c in colliders){
        c.enabled = true;
      } 
    }
    public void JoyzoneOff () {
//      m_isJoyzoneOn = false;
      ZoneAlpha.SetAlpha(0.0f, 0.5f);
      ZoneOuterAlpha.SetAlpha(0.0f, 0.5f);
      JoyballArrows.SetAlpha(0.0f, 0.5f); 
      Collider[] colliders = GetComponentsInChildren<Collider>();
      foreach(Collider c in colliders){
        c.enabled = false;
      }
    }
  }
}
