using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace WidgetShowcase
{
		public class ModalityEcho : MonoBehaviour
		{
		
				// Use this for initialization
				void Start ()
				{
			
				}
		
				// Update is called once per frame
				void Update ()
				{
						if (ModalityManager.Instance != null)
								GetComponent<Text> ().text = ModalityManager.Instance.ActiveItemName;
				}
		}
	
}