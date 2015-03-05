using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;
using Asterisms;

public class AsterismToggleDataBinder : DataBinderToggle {
  [SerializeField] 
  SliderDemo slider;
  
  override public bool GetCurrentData() {
    return AsterismDrawer.DrawToggle;
  }
  
  override protected void setDataModel(bool value) { 
    if ( value == false )  { 
      AsterismDrawer.TurnOffAsterisms(); 
      slider.Interactable = false;
      
      
    } else { 
      AsterismDrawer.TurnOnAsterisms(); 
      slider.Interactable = true;
      
    }
  }
}

