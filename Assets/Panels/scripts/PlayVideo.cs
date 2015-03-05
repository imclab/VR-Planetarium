using UnityEngine;
using System.Collections;

namespace WidgetShowcase
{
		public class PlayVideo : MonoBehaviour, IBoolEmitter
		{
				public IBoolEmitter CollissionEmitter;
				static PlayVideo PlayingVideo;
				// Use this for initialization
				public GameObject MovieGameObject = null;
				
				public string MovieName;
				

				void Start ()
				{
						if (!MovieGameObject)
								MovieGameObject = gameObject;
						MovieGameObject.renderer.enabled = false;
						if (!(MovieTexture)MovieGameObject.renderer.material.mainTexture)
								Debug.Log ("No movie in " + name);
				}

				public	void PlayMovie (bool play = true)
				{
						//	Debug.Log ("PlayMovie - setting play of " + name + " to " + (play ? "TRUE" : "FALSE"));
						if (play) {
								if (PlayingVideo != null)
										PlayingVideo.PlayMovie (false);
								renderer.enabled = true;
								MovieGameObject.renderer.material.mainTexture = Movie;
				
								Movie.Play ();
								audio.clip = Movie.audioClip;
								audio.Play ();
								PlayingVideo = this;
						} else {
								renderer.enabled = false;
								try {
										Movie.Stop ();
										audio.Stop ();
								} catch (System.NullReferenceException ex) {
								}
						}
				}
	
				// Update is called once per frame
				void Update ()
				{
	
				}

				MovieTexture Movie {
						get {
				
								if (!MovieGameObject)
										MovieGameObject = gameObject;
//								return ((MovieTexture)MovieGameObject.renderer.material.mainTexture);
				return (MovieTexture) Resources.Load("TutorialVideo/"+ MovieName, typeof(MovieTexture));
					
						}
				}


		#region IBoolEmitter implementation
				public event System.EventHandler<WidgetEventArg<bool>> BoolEvent;

				bool IsPlaying = false;

				public bool BoolValue {
						get { return IsPlaying; }
						set {
								IsPlaying = value;
								PlayMovie (value); 
								System.EventHandler<WidgetEventArg<bool>> BoolEventHandler = BoolEvent;
								if (BoolEventHandler != null)
										BoolEventHandler (this, new WidgetEventArg<bool> (value));
						}
				}
		#endregion
		}
	
}