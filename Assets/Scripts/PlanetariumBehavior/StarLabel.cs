using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Stars;

public class StarLabel : MonoBehaviour
{		
  [SerializeField]
  private Transform m_centerTransform;

  private bool m_updatedStarReference = false;
  public GameObject StarReference { set { m_starReference = value.GetComponent<StarBehavior>(); } get { return m_starReference.gameObject; } }
  private StarBehavior m_starReference;

	public Text LabelComp;
	const int ExtraSpaces = 2;

	// Use this for initialization
	void Start ()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
    if (!m_updatedStarReference) {
      m_starReference.LabelReference = gameObject;
      m_updatedStarReference = true;
    }

    //UpdateRepresentation();

    transform.LookAt (m_centerTransform.position, m_centerTransform.up);
	}

  public void UpdateRepresentation() {
//    Debug.Log("Label Update Rep");
    float scaleFactor = 0.002f;
    float distanceFactor = StarUpdater.Instance.CalculateInverseScaleFactor(transform.position, 0.25f);
    float inverseParentScale = 1.0f;
    if ( transform.parent != null ) {
      inverseParentScale = 1.0f / transform.parent.lossyScale.x;
    }
    scaleFactor *= distanceFactor * inverseParentScale;
    transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
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
