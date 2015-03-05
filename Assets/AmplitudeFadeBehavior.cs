using UnityEngine;
using System;
using System.Collections;

public class AmplitudeFadeBehavior : MonoBehaviour {
  [SerializeField]
  private float m_fadeTime;
  [SerializeField]
  private float m_fadeLowLevel;
  [SerializeField]
  private float m_fadeHighLevel;
  [SerializeField]
  private AnimationCurve m_fadeUpCurve;
  [SerializeField]
  private AnimationCurve m_fadeDownCurve;
  [SerializeField]
  private AudioSource m_audioLoop;

  private Coroutine m_currentFadeRoutine = null;

	// Use this for initialization
	void Start () {
	  if ( m_audioLoop == null ) { 
      throw new Exception("No AudioLoop to attenuate.");
    }
	}

  public void FadetoLow() {
    stopCurrentFade();
    m_currentFadeRoutine = StartCoroutine(FadeToGoal(m_fadeLowLevel, m_fadeTime, m_fadeDownCurve));
  }

  public void FadeToHigh() {
    stopCurrentFade();
    m_currentFadeRoutine = StartCoroutine(FadeToGoal(m_fadeHighLevel, m_fadeTime, m_fadeUpCurve));
  }

  private void stopCurrentFade() {
    if ( m_currentFadeRoutine != null ) { 
      try { 
        StopCoroutine(m_currentFadeRoutine); 
      }
      catch ( MissingReferenceException e ) { 
        Debug.Log("Lost the fade coroutine: " + e.Message);
      }
      m_currentFadeRoutine = null;
    }
  }

  private IEnumerator FadeToGoal(float goal, float seconds = 0.25f, AnimationCurve filter = null) {
    float startValue = m_audioLoop.volume;
    float diff = goal - startValue;
    float startTime = Time.time;
    float percent = 0.0f; // What percentage of the way through the fade are we.

    while(percent < 1.0f) {
      percent = Mathf.Clamp01((Time.time - startTime) / seconds);
      float filtered = percent;
      if ( filter != null ) { filtered = filter.Evaluate(filtered); }
      float newVolume = startValue + (filtered * diff);
      m_audioLoop.volume = newVolume;
      yield return 0;
    }

    yield break;
  }
}
