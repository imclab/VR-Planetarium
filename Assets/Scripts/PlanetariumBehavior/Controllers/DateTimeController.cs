using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;

public class DateTimeController : WidgetDataInputDateTime {
	// Fires when the data is toggled.
	public override event EventHandler<WidgetEventArg<DateTime>> DataChangedHandler;
	
	// Returns the current toggle state of the data.
	public override DateTime GetCurrentData() {
		return TimeAndLocationHandler.Instance.DateAndTime;
	}
	
	// Sets the current toggle state of the data.
	public override void SetCurrentData(DateTime value) {
		TimeAndLocationHandler.Instance.DateAndTime = value;
		
		EventHandler<WidgetEventArg<DateTime>> handler = DataChangedHandler;
		if ( handler != null ) {
			handler(this, new WidgetEventArg<DateTime>(GetCurrentData()));
		}
	}
}