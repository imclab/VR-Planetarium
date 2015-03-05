using UnityEngine;
using System.Collections;


namespace Stars {
  public class StarBehavior : MonoBehaviour {

    public float BaseScaleFactor = 1.0f;
    public float SelectedScaleFactor = 2.0f;
    public bool IsSelected = false;
    public GameObject LabelReference { 
      get { 
        if ( m_labelReference != null ) {
          return m_labelReference.gameObject; 
        }
        else {
          return null;
        }
      }
      set { m_labelReference = value.GetComponent<StarLabel>(); }
    }
    public StarLabel m_labelReference = null; 
  	
  	// Update is called once per frame
  	void Update () {
      if ( true ) { 
        UpdateRepresentation();
      }
  	}

    public void UpdateRepresentation() {
      float scaleFactor = IsSelected ? SelectedScaleFactor : BaseScaleFactor;
      float distanceFactor = StarUpdater.Instance.CalculateInverseScaleFactor(transform.position);
      float inverseParentScale = 1.0f;
      if ( transform.parent != null ) {
        inverseParentScale = 1.0f / transform.parent.localScale.x;
      }
      scaleFactor *= distanceFactor * inverseParentScale;
      if ( transform.localScale.x != scaleFactor ) {
        transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
      }

      if ( LabelReference != null ) {
        m_labelReference.UpdateRepresentation();
      }
    }

    public StarData GetStarData() {
      uint hdID = 0;
      bool parsed = uint.TryParse(gameObject.name, out hdID);
      if  (!parsed) { return null; }
      int index = StarParser.HD_idToIndex[hdID];
      return StarParser.Instance.Stars[index];
    }
  }
}
