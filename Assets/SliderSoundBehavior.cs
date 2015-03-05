//#define DEBUG_SLIDER_AUDIO

using UnityEngine;
using WidgetShowcase;
using LMWidgets;
using System.Collections;
using System.Collections.Generic;

struct SliderUpdateData {
  public float lastValue;
  public float lastUpdateTime;
}

public class SliderSoundBehavior : MonoBehaviour {

  [SerializeField]
  private AudioSource m_audioSource;
  [SerializeField]
  private AudioLowPassFilter m_lowPassFilter;
  [SerializeField]
  private List<SliderBase> m_sliders;
  [SerializeField]
  private float m_minVelocity;
  [SerializeField]
  private float m_maxVelocity;
  [SerializeField]
  private float m_lowPassFreqAtMin;
  [SerializeField]
  private float m_lowPassFreqAtMax;
  [SerializeField]
  private float m_amplitudeAtMin;
  [SerializeField]
  private float m_amplitudeAtMax;
  [SerializeField]
  private AnimationCurve m_amplitudeCurve;
  [SerializeField]
  private AnimationCurve m_lowPassFrequencyCurve;

  private Dictionary<int, SliderUpdateData> m_previousValues; 

  private List<float> m_changedSliders;

	// Use this for initialization
	void Start () {
    m_changedSliders = new List<float>();
    m_previousValues = new Dictionary<int, SliderUpdateData>();

	  foreach(SliderBase slider in m_sliders){
//    slider.ChangeHandler += onSliderChanged;
      SliderUpdateData newData;
      newData.lastUpdateTime = Time.time;
//    newData.lastValue = slider.GetSliderPercentage();
      newData.lastValue = slider.GetSliderFraction();
      m_previousValues.Add(slider.gameObject.GetInstanceID(), newData);
    }
	}
	
	// Update is called once per frame
	void Update () {
    float highestVelocity = m_changedSliders.Count > 0 ? Mathf.Max(m_changedSliders.ToArray()) : 0.0f;

    float normalizedVelocity = Mathf.Clamp01((highestVelocity - m_minVelocity) / (m_maxVelocity - m_minVelocity));
    float lowPassFrequency = m_lowPassFreqAtMin + (m_lowPassFrequencyCurve.Evaluate(normalizedVelocity) * (m_lowPassFreqAtMax - m_lowPassFreqAtMin));
    float amplitude = m_amplitudeAtMin + (m_amplitudeCurve.Evaluate(normalizedVelocity) * (m_amplitudeAtMax - m_amplitudeAtMin));

    m_lowPassFilter.cutoffFrequency = lowPassFrequency;
    m_audioSource.volume = amplitude;

    m_changedSliders.Clear();
#if DEBUG_SLIDER_AUDIO
    Debug.Log("highestVelocity: " + highestVelocity);
    Debug.Log("normalizedVelocity: " + normalizedVelocity);
    Debug.Log("highestVelocity: " + highestVelocity);
    Debug.Log("lowPassFrequency: " + lowPassFrequency);
    Debug.Log("amplitude: " + amplitude);
    Debug.Log("");
#endif
	}

//  void onSliderChanged(object sender, WidgetEventArg<float> args) {
//    int instanceID = (sender as SliderBase).gameObject.GetInstanceID();
//    float lastValue = m_previousValues[instanceID].lastValue; 
//    float valueDiff = Mathf.Abs(args.CurrentValue - lastValue);
//    float timeDiff = Time.time - m_previousValues[instanceID].lastUpdateTime;
//    float velocity = valueDiff / timeDiff;
//
//    SliderUpdateData newData;
//    newData.lastUpdateTime = Time.time;
//    newData.lastValue = args.CurrentValue;
//    m_previousValues[instanceID] = newData;
//
//    m_changedSliders.Add(velocity);
//  }
}
