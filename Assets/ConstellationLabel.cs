using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Asterisms;
using Stars;

public class ConstellationLabel : MonoBehaviour {

  [SerializeField]
  private Transform m_centerTransform;

  public Text LabelComp;
  
  // Use this for initialization
  void Start ()
  {
    
  }
  
  // Update is called once per frame
  void Update ()
  {
    transform.LookAt (m_centerTransform.position, m_centerTransform.up);
    UpdateRepresentation();
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
      label = value;
      LabelComp.text = label;
    }
  }
}
