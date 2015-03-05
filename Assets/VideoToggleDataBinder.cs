using UnityEngine;
using WidgetShowcase;
using LMWidgets;
using System;
using System.Collections;

public class VideoToggleDataBinder : DataBinderToggle {
  [SerializeField]
  private ButtonDemoToggle m_linkedButton;
 
  
  // Returns the current system value of the data.
  override public bool GetCurrentData() {
    return m_linkedButton.ToggleState;
  }
  
  // Set the current system value of the data.
  override protected void setDataModel(bool value) { 
    m_linkedButton.ToggleState = value;
  }
}
