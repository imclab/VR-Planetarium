using UnityEngine;
using WidgetShowcase;
using Stars;
using System;
using System.Collections;

public class MilkyWayIntensityController : WidgetDataInputFloat {
  [SerializeField]
  private FilterBehavior m_filterBehavior;

  public override event EventHandler<WidgetEventArg<float>> DataChangedHandler; 
  
  // Returns the current system value of the data.
  public override float GetCurrentData() {
    return m_filterBehavior.MilkyWayIntensity;
  }
  
  // Set the current system value of the data.
  public override void SetCurrentData(float value) {
    m_filterBehavior.MilkyWayIntensity = value;
    
    EventHandler<WidgetEventArg<float>> handler = DataChangedHandler;
    if ( handler != null ) {
      handler(this, new WidgetEventArg<float>(GetCurrentData()));
    }
  }
  
}