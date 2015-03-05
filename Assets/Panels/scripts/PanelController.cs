using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LMWidgets;

namespace WidgetShowcase
{
		public class PanelController : MonoBehaviour
		{
				const string PANEL_CONTROLLER = "panel controller";
				public ButtonBase MyStartButton;
				public Animator myMecanim;
				public string loadSceneName = "Main";
/**
The panels try to grab modality until it has achieved it at least once.
*/

        // Use this for initialization
        void Start ()
        {
          if (MyStartButton) {
            MyStartButton.StartHandler += StartHandler;
          }
        }
	
				void Awake ()
				{
				}

				// Update is called once per frame
				void Update ()
				{
				}
		
				void StartHandler (object sender, EventArg<bool> e)
				{
						AnimateClose ();
				}

				void AnimateClose ()
				{
						myMecanim.Play("PanelMecanim");
				}

				public void OnAnimationFinish ()
				{
						Application.LoadLevelAsync (loadSceneName);
				}
		}
}