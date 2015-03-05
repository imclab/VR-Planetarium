using UnityEngine;
using WidgetShowcase;
using Stars;
using System;
using System.Collections;

public class ZoomController : WidgetDataInputFloat {
  public override event EventHandler<WidgetEventArg<float>> DataChangedHandler; 
  
  // Returns the current system value of the data.
  public override float GetCurrentData() {
    return StarUpdater.Instance.Zoom;
  }
  
  // Set the current system value of the data.
  public override void SetCurrentData(float value) {
    StarUpdater.Instance.SetZoom(value);
    
    EventHandler<WidgetEventArg<float>> handler = DataChangedHandler;
    if ( handler != null ) {
      handler(this, new WidgetEventArg<float>(GetCurrentData()));
    }
  }
  
}
