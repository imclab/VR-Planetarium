using UnityEngine;
using System.Collections;


public class EventSubscriberExample : MonoBehaviour {

	[SerializeField]
	private ButtonOrreryToggle m_button1;
	
	[SerializeField]
	private SliderDemo m_slider;

	// Use this for initialization
	void Start () {
		m_button1.buttonPressedHandler += OnButtonPressed;
		m_button1.buttonReleasedHandler += OnButtonReleased;
		m_slider.SliderPressedHandler += OnSliderPressed;
		m_slider.SliderReleasedHandler += OnSliderReleased;
		m_slider.SliderChangedHandler += OnSliderChanged;
	}
	
	private void OnButtonPressed (object sender, VRWidgets.WidgetEventArg<bool> args){
		Debug.Log("ButtonPressed: " + args.CurrentValue);
	}
	private void OnButtonReleased (object sender, VRWidgets.WidgetEventArg<bool> args){
		Debug.Log("ButtonReleased: " + args.CurrentValue);
	}
	
	private void OnSliderPressed (object sender, VRWidgets.WidgetEventArg<float> args){
		Debug.Log ("SliderPressed: " + args.CurrentValue);
	}
	
	private void OnSliderReleased (object sender, VRWidgets.WidgetEventArg<float> args){
		Debug.Log ("SliderReleased: " + args.CurrentValue);
	}
	
	private void OnSliderChanged (object sender, VRWidgets.WidgetEventArg<float> args){
		Debug.Log ("SliderChanged: " + args.CurrentValue);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
