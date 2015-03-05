using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;
using WidgetShowcase;
using Stars;


public class StarNameOpacityDataBinder : DataBinderSlider {

    override public float GetCurrentData() {
      return StarUpdater.Instance.LabelOpacity;
    }
    
  override protected void setDataModel(float value) {
    StarUpdater.Instance.SetLabelOpacity(value);
    
    }
  }
