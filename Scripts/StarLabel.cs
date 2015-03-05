using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StarLabel : MonoBehaviour
{		
		public Text LabelComp;
		const int ExtraSpaces = 2;

		// Use this for initialization
		void Start ()
		{
				LabelRoot.ParentToMe (this.gameObject);
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		string label;

		public string Label {
				get {
						return label;
				}
				set {
						for (int i = 0; i < ExtraSpaces; ++i)
								value = " " + value;
						label = value;
						LabelComp.text = label;
				}
		}
}
