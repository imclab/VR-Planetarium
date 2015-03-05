using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LMWidgets;
using Asterisms;

public class MilkyWayToggleDataBinder : DataBinderToggle {
  [SerializeField] 
  SliderDemo slider;
  
  [SerializeField]
  private FilterBehavior m_filterBehavior;
  
  override public bool GetCurrentData() {
    return m_filterBehavior.DrawMilkyWay;
  }
  
  override protected void setDataModel(bool value) { 
    m_filterBehavior.DrawMilkyWay = value;
    slider.Interactable = value;
    
  }
}