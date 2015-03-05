using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;

public class DateTimeMonthController : WidgetDataInputInt
{
		// Fires when the data is toggled.
		public override event EventHandler<WidgetEventArg<int>> DataChangedHandler;
	
		// Returns the current toggle state of the data.
		public override int GetCurrentData ()
		{
//				Debug.Log("DateTimeMonthController says:  " + TimeAndLocationHandler.Instance.DateAndTime.Month);
//				Debug.Log("LongDateString: " + TimeAndLocationHandler.Instance.DateAndTime.ToLongDateString());
				return TimeAndLocationHandler.Instance.DateAndTime.Month;
		}
	
		// Sets the current toggle state of the data.
  public override void SetCurrentData (int value)
  {
    if (value == 0) {
			return;
		}
		DateTime newDateTime = TimeAndLocationHandler.Instance.DateAndTime;

    try {
		  newDateTime = new DateTime (newDateTime.Year, value, newDateTime.Day, newDateTime.Hour, newDateTime.Minute, newDateTime.Second);
    }
    catch (ArgumentOutOfRangeException e) {
      Debug.LogWarning("Attempting to set improper date. Ignoring.");
      return;
    }
		TimeAndLocationHandler.Instance.DateAndTime = newDateTime;

		EventHandler<WidgetEventArg<int>> handler = DataChangedHandler;
		if (handler != null) {
				handler (this, new WidgetEventArg<int> (GetCurrentData ()));
		}
	}
}