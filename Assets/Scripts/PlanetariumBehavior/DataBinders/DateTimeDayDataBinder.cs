using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;
using LMWidgets;

public class DateTimeDayDataBinder : DataBinderDial {
  

  override public string GetCurrentData() {
    if (TimeAndLocationHandler.Instance == null) return "1";
    {
      return TimeAndLocationHandler.Instance.DateAndTime.Day.ToString();
    }
  }
  
  override protected void setDataModel(string value) {
    {
      if (value == null) {
        return;
      }
      DateTime newDateTime = TimeAndLocationHandler.Instance.DateAndTime;
      
      try {
                newDateTime = new DateTime (newDateTime.Year, newDateTime.Month, Convert.ToInt32( value), newDateTime.Hour, newDateTime.Minute, newDateTime.Second);
      }
      catch (ArgumentOutOfRangeException e) {
        Debug.LogWarning("Attempting to set improper date. Ignoring.");
        return;
      }
      TimeAndLocationHandler.Instance.DateAndTime = newDateTime;
      
    }
  }
  

}

