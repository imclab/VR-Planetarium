using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WidgetShowcase;
using LMWidgets;

public class ButtonClickListener : MonoBehaviour
{

  [SerializeField]
  private ButtonBase[] m_buttons;
  [SerializeField]
  private DialGraphics[] m_dials;
  [SerializeField]
  private AudioSource m_audio_pressed;
  [SerializeField]
  private AudioSource m_audio_released;
  [SerializeField]
  private AudioSource m_dialTick;

  private Dictionary<int, int> m_dialPrevValues;

  // Use this for initialization
//  void Start () {
//    m_dialPrevValues = new Dictionary<int, int>();
//
//    foreach(ButtonBase button in m_buttons) {
//      button.StartHandler += onButtonStart;
//      button.EndHandler += onButtonEnd;
//    }
//
//    foreach(DialGraphics dial in m_dials) {
//      dial.StartHandler += onDialStart;
//      dial.EndHandler += onDialEnd;
//      dial.ChangeHandler += onDialChanged;
//      m_dialPrevValues.Add(dial.gameObject.GetInstanceID(), dial.CurrentDialInt);
//    }
//  }
//
//  private void onButtonStart(object sender, WidgetEventArg<bool> arg) {
//    if ( m_audio_pressed != null ) {
//      m_audio_pressed.Stop();
//      m_audio_pressed.Play();
//    }
//  }
//
//
//    private void onButtonEnd (object sender, WidgetEventArg<bool> arg)
//    {
//        if (m_audio_released != null) {
//            m_audio_pressed.Stop ();
//            m_audio_released.Play ();
//        }
//    }
//
//    private void onDialStart (object sender, WidgetEventArg<int> arg)
//    {
//        if (m_audio_pressed != null) {
//            m_audio_pressed.Stop ();
//            m_audio_pressed.Play ();
//        }
//    }
//
//  private void onDialEnd(object sender, WidgetEventArg<int> arg) {
//    if ( m_audio_released != null ) {
//      m_audio_pressed.Stop();
//      m_audio_released.Play();
//    }
//  }
//
//  private void onDialChanged(object sender, WidgetEventArg<int> arg) {
//    int instanceId = (sender as DialGraphics).gameObject.GetInstanceID();
//    if ( m_dialPrevValues[instanceId] != arg.CurrentValue ) {
//      m_dialTick.Stop();
//      m_dialTick.Play();
//    }
//    m_dialPrevValues[instanceId] = arg.CurrentValue;
//  }

}
