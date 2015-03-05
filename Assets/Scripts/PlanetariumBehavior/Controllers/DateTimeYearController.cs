using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;

public class DateTimeYearController : WidgetDataInputInt {
	// Fires when the data is toggled.
	public override event EventHandler<WidgetEventArg<int>> DataChangedHandler;
	
	// Returns the current toggle state of the data.
	public override int GetCurrentData() {
		return TimeAndLocationHandler.Instance.DateAndTime.Year;
	}
	
	// Sets the current toggle state of the data.
	public override void SetCurrentData(int value) {
    if ( value == 0 ) { return; }
		DateTime newDateTime = TimeAndLocationHandler.Instance.DateAndTime;

    try {
		  newDateTime = new DateTime(value, newDateTime.Month, newDateTime.Day, newDateTime.Hour, newDateTime.Minute, newDateTime.Second);
    }
    catch (ArgumentOutOfRangeException e) {
      Debug.LogWarning("Attempting to set improper date. Ignoring.");
      return;
    }

		TimeAndLocationHandler.Instance.DateAndTime = newDateTime;
		
		EventHandler<WidgetEventArg<int>> handler = DataChangedHandler;
		if ( handler != null ) {
			handler(this, new WidgetEventArg<int>(GetCurrentData()));
		}
	}
}