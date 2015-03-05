using UnityEngine;
using WidgetShowcase;
using Stars;
using System;
using System.Collections;
using LMWidgets;

public class MilkyWayIntensityDataBinder : DataBinderSlider {
  [SerializeField]
  private FilterBehavior m_filterBehavior;

  override public float GetCurrentData() {
    return m_filterBehavior.MilkyWayIntensity;
  }
  
  override protected void setDataModel(float value) {
    m_filterBehavior.MilkyWayIntensity = value;
    
  }
}
