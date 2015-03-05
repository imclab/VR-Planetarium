using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;

public class AsterismBrightnessDataBinder : DataBinderSlider {
  
  override public float GetCurrentData() {
    return Asterisms.AsterismDrawer.Brightness;
  }
  
  override protected void setDataModel(float value) {
    Asterisms.AsterismDrawer.SetAsterismOpacity(value);
    
  }
}
