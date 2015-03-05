//#define DEBUG_VIDEO

using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;
using System.Collections.Generic;
using LMWidgets;

public class VideoManager : MonoBehaviour {

  public event EventHandler MovieStartPlayingHandler; // Fires when we play the video
  public event EventHandler MovieStopPlayingHandler; // Fires when we pause the video or the video ends

  [SerializeField]
  private AudioSource m_audioPlayer;

  [SerializeField]
  private List<string> m_videoResourceNames;

  [SerializeField]
  private List<VideoToggleDataBinder> m_movieButtons;

  private string m_currentMovieName = "";

  private Dictionary<int, string> m_buttonsToMovieNames;
  private Dictionary<string, int> m_movieNamesToButtons;

  private bool isPaused = false;

	// Use this for initialization
	void Start () {
    if( m_videoResourceNames.Count <= 0 ) { throw new System.ArgumentOutOfRangeException("Must have at least one video."); }
    if( m_movieButtons.Count <= 0 ) { throw new System.ArgumentOutOfRangeException("Must have at least one button."); }
    if( m_videoResourceNames.Count != m_movieButtons.Count ) { throw new System.ArgumentOutOfRangeException("Must have same number of movie names and buttons."); }

    renderer.enabled = false;

    m_buttonsToMovieNames = new Dictionary<int, string>();
    m_movieNamesToButtons = new Dictionary<string, int>();

    for ( int i=0;i<m_videoResourceNames.Count;i++ ) {
      int buttonID = m_movieButtons[i].gameObject.GetInstanceID();
      m_movieButtons[i].SetCurrentData(false);
      m_movieButtons[i].DataChangedHandler += handleButtonPressed;
      m_buttonsToMovieNames.Add(buttonID, m_videoResourceNames[i]);
      m_movieNamesToButtons.Add(m_videoResourceNames[i], buttonID); 
    }
	}
	
	// Update is called once per frame
	void Update () {
    if ( renderer.material.mainTexture == null ) { return; }
    if ( renderer.material.mainTexture.GetType() != typeof(MovieTexture) ) { 
      Debug.LogWarning("Main texture of video plane isn't a MovieTexture.");
      return;
    }
    if ( isPaused ) { return; }

    // Dump the current movie when its done.
    if ( !(renderer.material.mainTexture as MovieTexture).isPlaying ) {
      EventHandler handler = MovieStopPlayingHandler;
      if ( handler != null ) { handler(this, new EventArgs()); }
      dumpCurrentMovie();
    }
	}

  private void handleButtonPressed(object sender,   EventArg<bool> arg) {
    int buttonId = (sender as VideoToggleDataBinder).gameObject.GetInstanceID();
    string movieName = m_buttonsToMovieNames[buttonId];
    // If we've recieved a button turning off.
    if ( arg.CurrentValue == false ) { 
      pauseCurrentMovie();
      return;
    }

    clearOtherButtons(buttonId);

    if ( m_currentMovieName == null ) { // If we don't have a current video
      loadAndPlayMovie(movieName);
    }
    else if (movieName == m_currentMovieName) { // If we have the same video
      playCurrentMovie();
    }
    else if (movieName != m_currentMovieName) { // If we have two different videos
      dumpCurrentMovie();
      loadAndPlayMovie(movieName);
    }
  }

  private void clearOtherButtons(int buttonId) {
    foreach( VideoToggleDataBinder buttonData in m_movieButtons ) {
      if ( buttonData.gameObject.GetInstanceID() == buttonId ) { continue; }
      buttonData.SetCurrentData(false);
    }
  }

  private void pauseCurrentMovie() {
    if ( renderer.material.mainTexture == null ) { return; }
    if ( renderer.material.mainTexture.GetType() != typeof(MovieTexture) ) { 
      Debug.LogWarning("main texture of video plane isn't a MovieTexture");
      return;
    }
#if DEBUG_VIDEO
    Debug.Log("Pausing current movie");
#endif
    (renderer.material.mainTexture as MovieTexture).Pause();
    m_audioPlayer.Pause();

    EventHandler handler = MovieStopPlayingHandler;
    if ( handler != null ) { handler(this, new EventArgs()); }
    isPaused = true;
  }

  private void playCurrentMovie() {
    if ( renderer.material.mainTexture == null ) { return; }
    if ( renderer.material.mainTexture.GetType() != typeof(MovieTexture) ) { 
      Debug.LogWarning("main texture of video plane isn't a MovieTexture");
      return;
    }
#if DEBUG_VIDEO
    Debug.Log("Playing current movie");
#endif

    if ( renderer.enabled != true ) { renderer.enabled = true; }

    (renderer.material.mainTexture as MovieTexture).Play();
    m_audioPlayer.Play();

    EventHandler handler = MovieStartPlayingHandler;
    if ( handler != null ) { handler(this, new EventArgs()); }
    isPaused = false;
  }

  private void dumpCurrentMovie() {
    if ( renderer.material.mainTexture == null ) { return; }
#if DEBUG_VIDEO
    Debug.Log("Dumping current movie: " + m_currentMovieName);
#endif

    pauseCurrentMovie();
    if ( renderer.enabled == true ) { renderer.enabled = false; }
    m_audioPlayer.clip = null;
    renderer.material.mainTexture = null;
    m_currentMovieName = "";
    Resources.UnloadUnusedAssets();
    isPaused = false;
  }

  private void loadAndPlayMovie(string resourceName) { 
    dumpCurrentMovie();
#if DEBUG_VIDEO
    Debug.Log("Loading new movie");
#endif
    string resourceString = "TutorialVideo/"+resourceName;
    MovieTexture newMovie = Resources.Load<MovieTexture>(resourceString);
    if ( newMovie == null ) { 
      Debug.LogError("Could not load movie at: " + resourceString); 
      return;
    }
    renderer.material.mainTexture = newMovie;
    m_audioPlayer.clip = newMovie.audioClip;
    m_currentMovieName = resourceName;
    playCurrentMovie();
  }

  void OnDestroy() {
    dumpCurrentMovie();
  }
}
