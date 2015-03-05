using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;
using Asterisms;

public class StarNameToggleDataBinder : DataBinderToggle {
  [SerializeField] 
  SliderDemo slider;
  
  override public bool GetCurrentData() {
    return Stars.StarUpdater.Instance.LabelDrawToggle;
  }
  
  override protected void setDataModel(bool value) { 
    Stars.StarUpdater.Instance.SetLabelDrawing(value);
    slider.Interactable = value;
    
  }
}