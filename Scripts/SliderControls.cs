using UnityEngine;
using VRWidgets;
using System.Collections;
using Stars;

public class SliderControls : MonoBehaviour {

	[SerializeField]
	private SliderDemo m_zoomSlider;

	// Use this for initialization
	void Start () {
		m_zoomSlider.SliderChangedHandler += onSliderChanged;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void onSliderChanged(object sender, WidgetEventArg<float> args) {
		StarUpdater.Instance.SetZoom(args.CurrentValue);
	}
}
