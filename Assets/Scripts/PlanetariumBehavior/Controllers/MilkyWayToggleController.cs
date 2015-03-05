using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;

public class MilkyWayToggleController : WidgetDataInputBool {
  [SerializeField]
  private FilterBehavior m_filterBehavior;

  // Fires when the data is toggled.
  public override event EventHandler<WidgetEventArg<bool>> DataChangedHandler;
  
  // Returns the current toggle state of the data.
  public override bool GetCurrentData() {
    return m_filterBehavior.DrawMilkyWay;
  }
  
  // Sets the current toggle state of the data.
  public override void SetCurrentData(bool value) {
    m_filterBehavior.DrawMilkyWay = value;
    
    EventHandler<WidgetEventArg<bool>> handler = DataChangedHandler;
    if ( handler != null ) {
      handler(this, new WidgetEventArg<bool>(GetCurrentData()));
    }
  }
}