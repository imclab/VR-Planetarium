using UnityEngine;
using WidgetShowcase;
using Asterisms;
using System;
using System.Collections;

public class AsterismToggleController : WidgetDataInputBool {
  // Fires when the data is toggled.
  public override event EventHandler<WidgetEventArg<bool>> DataChangedHandler;

  // Returns the current toggle state of the data.
  public override bool GetCurrentData() {
    return AsterismDrawer.DrawToggle;
  }
  
  // Sets the current toggle state of the data.
  public override void SetCurrentData(bool value) {
    if ( value == false )  { AsterismDrawer.TurnOffAsterisms(); } else { AsterismDrawer.TurnOnAsterisms(); }

    EventHandler<WidgetEventArg<bool>> handler = DataChangedHandler;
    if ( handler != null ) {
      handler(this, new WidgetEventArg<bool>(GetCurrentData()));
    }
  }
}
