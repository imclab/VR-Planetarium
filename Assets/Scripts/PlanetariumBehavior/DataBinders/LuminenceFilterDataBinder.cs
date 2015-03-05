using UnityEngine;
using WidgetShowcase;
using Stars;
using System;
using System.Collections;
using LMWidgets;

public class LuminenceFilterDataBinder : DataBinderSlider {
  public GameObject Skyglow;
  
  private float m_skyglowBaseAlpha;
  
  
  void Start() {
    if ( Skyglow ) {
      m_skyglowBaseAlpha = Skyglow.renderer.material.color.a;
    }
  }
  
  // Returns the current system value of the data.
  override public float GetCurrentData() {
    return 1.0f - StarUpdater.Instance.MinLuminance;
  }
  
  // Set the current system value of the data.
  override protected void setDataModel(float value) {
    StarUpdater.Instance.SetMinLuminance(1.0f - value);
    
    if ( Skyglow ) {
      float newSkyglowAlpha = m_skyglowBaseAlpha + ((1.0f - value) * (1.0f - m_skyglowBaseAlpha));
      Color temp = Skyglow.renderer.material.color;
      temp.a = newSkyglowAlpha;
      Skyglow.renderer.material.color = temp;
    }
  }
}
