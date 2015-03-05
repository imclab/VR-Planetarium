using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;

public class TogglePanelDataBinder : DataBinderToggle {
  [SerializeField] 
  GameObject panel;
  
  override public bool GetCurrentData() {
    if ( panel.activeSelf == true ) {
      return true;
    }
    else {
      return false;
    }
  }
  
  override protected void setDataModel(bool value) { 
    if ( value == true ) { 
      panel.SetActive(true);
    }
    else {
      panel.SetActive(false);
    }
  }
}
