using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;
using LMWidgets;

public class DateTimeHourDataBinder : DataBinderDial {

  override public string GetCurrentData() {
    if (TimeAndLocationHandler.Instance == null) return "00";
    {
      return TimeAndLocationHandler.Instance.DateAndTime.Hour.ToString();
    }
  }
  
  override protected void setDataModel(string value) {
    {
      if (value == null) {
        return;
      }
      DateTime newDateTime = TimeAndLocationHandler.Instance.DateAndTime;
      
      try {
        newDateTime = new DateTime (newDateTime.Year, newDateTime.Month, newDateTime.Day, Convert.ToInt32( value), newDateTime.Minute, newDateTime.Second);
      }
      catch (ArgumentOutOfRangeException e) {
        Debug.LogWarning("Attempting to set improper date. Ignoring.");
        return;
      }
      TimeAndLocationHandler.Instance.DateAndTime = newDateTime;
      
    }
  }
  
  
}