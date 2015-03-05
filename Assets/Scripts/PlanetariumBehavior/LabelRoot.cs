using UnityEngine;
using System.Collections;

public class LabelRoot : MonoBehaviour
{
/**
 this folderol allows us to assign children to the global instance 
regardless of whether or not its been initialized to a scene item.
*/
		static GameObject labelRootGameObject;

		public static GameObject LabelRootGameObject {
				get {
						if (!labelRootGameObject)
								labelRootGameObject = new GameObject ();
						return labelRootGameObject;
				}
		set {
			if (!labelRootGameObject)
				labelRootGameObject = new GameObject ();
						for (int i = 0; i < labelRootGameObject.transform.childCount; ++i) {
								GameObject g = labelRootGameObject.transform.GetChild (i).gameObject;
								g.transform.SetParent (value.transform, true);
						}
						labelRootGameObject = value;
				}
		}

		// Use this for initialization
		void Start ()
		{

		}
	
		bool init = false;
		// Update is called once per frame
		void Update ()
		{
		
				if (!init) {
						LabelRootGameObject = this.gameObject;
						init = true;
				}
		}

		public static void ParentToMe (GameObject go)
		{
				go.transform.SetParent (LabelRootGameObject.transform, true);
		}
}
