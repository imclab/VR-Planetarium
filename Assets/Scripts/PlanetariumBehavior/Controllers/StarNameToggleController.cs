using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;

public class StarNameToggleController : WidgetDataInputBool {
  // Fires when the data is toggled.
  public override event EventHandler<WidgetEventArg<bool>> DataChangedHandler;
  
  // Returns the current toggle state of the data.
  public override bool GetCurrentData() {
    return Stars.StarUpdater.Instance.LabelDrawToggle;
  }
  
  // Sets the current toggle state of the data.
  public override void SetCurrentData(bool value) {
    Stars.StarUpdater.Instance.SetLabelDrawing(value);
    
    EventHandler<WidgetEventArg<bool>> handler = DataChangedHandler;
    if ( handler != null ) {
      handler(this, new WidgetEventArg<bool>(GetCurrentData()));
    }
  }
}