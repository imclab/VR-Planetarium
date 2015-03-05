using UnityEngine;
using WidgetShowcase;
using System;

// Interface to define an object that can be a data provider to a widget.
public abstract class WidgetDataInput<T> : MonoBehaviour {
  // Fires when the data is updated with the most recent data as the payload
  abstract public event EventHandler<WidgetEventArg<T>> DataChangedHandler;

  // Returns the current system value of the data.
  abstract public T GetCurrentData();

  // Set the current system value of the data.
  abstract public void SetCurrentData(T value);
}

// Non generic hacks so we can show these serialized in the Unity Editor
public abstract class WidgetDataInputBool : WidgetDataInput<bool> {};
public abstract class WidgetDataInputFloat : WidgetDataInput<float> {};
public abstract class WidgetDataInputVector3 : WidgetDataInput<Vector3> {};
public abstract class WidgetDataInputDateTime : WidgetDataInput<DateTime> {};
public abstract class WidgetDataInputInt : WidgetDataInput<int> {};
