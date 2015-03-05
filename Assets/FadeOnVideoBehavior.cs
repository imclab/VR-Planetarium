using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FadeOnVideoBehavior : MonoBehaviour {

  [SerializeField]
  private VideoManager m_videoManager;

  [SerializeField]
  private AmplitudeFadeBehavior m_fadeHandler;

  void Start() {
    if ( m_fadeHandler == null ) { throw new Exception("No fade handler"); }

    m_fadeHandler.FadeToHigh();
    m_videoManager.MovieStartPlayingHandler += onMovieStart;
    m_videoManager.MovieStopPlayingHandler += onMovieEnd;
  }

  private void onMovieStart(object sender, EventArgs arg) {
    m_fadeHandler.FadetoLow();
  }

  private void onMovieEnd(object sender, EventArgs arg) {
    m_fadeHandler.FadeToHigh();
  }
}
