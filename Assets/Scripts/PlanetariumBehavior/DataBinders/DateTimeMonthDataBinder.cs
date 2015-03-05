using UnityEngine;
using WidgetShowcase;
using System;
using System.Collections;
using LMWidgets;
using System.Globalization;


public class DateTimeMonthDataBinder : DataBinderDial {

  override public string GetCurrentData() {
    return TimeAndLocationHandler.Instance.DateAndTime.ToString("MMMM");
  }
  
  override protected void setDataModel(string value) {
    {
      if (value == null) {
        return;
      }
      
      int month = DateTime.ParseExact(value, "MMMM", CultureInfo.CurrentCulture ).Month;
      DateTime newDateTime = TimeAndLocationHandler.Instance.DateAndTime;
      
      
      try {
//        Debug.Log("Month DataBinder SetCurrentData(value) = " + value);
          newDateTime = new DateTime (newDateTime.Year, month, newDateTime.Day, newDateTime.Hour, newDateTime.Minute, newDateTime.Second);
      }
      catch (ArgumentOutOfRangeException e) {
        Debug.LogWarning("Attempting to set improper date. Ignoring.");
        return;
      }
      TimeAndLocationHandler.Instance.DateAndTime = newDateTime;
      
    }
  }
}
