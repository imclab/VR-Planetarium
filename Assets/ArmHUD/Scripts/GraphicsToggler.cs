using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LMWidgets;

namespace WidgetShowcase
{

	public class GraphicsToggler : MonoBehaviour {
	
    public DataBinderToggle m_ToggleController;
		
		public Texture2D OnTexture;
		public Texture2D OffTexture;
		private RawImage iconImage;
			
		
		void Awake () {
			iconImage = GetComponent<RawImage>();
		}
	
		// Use this for initialization
		void Start () {
			if (m_ToggleController != null) {
				m_ToggleController.DataChangedHandler += OnToggleChanged;
				bool currentBool = m_ToggleController.GetCurrentData ();
				if (currentBool == true) {
					iconImage.texture = OnTexture;
				} else {
					iconImage.texture = OffTexture;
				}
			}
		}
		
		// Update is called once per frame
		void Update () {
		}
		
		private void OnToggleChanged (object sender, EventArg<bool> args)
		{
			//Debug.Log ("OnToggleChanged");
			if (args.CurrentValue == true) {
				iconImage.texture = OnTexture;
			} else {
				iconImage.texture = OffTexture;
			}
		}
	}
}
