using UnityEngine;
using System.Collections;
using System;
using Leap;
using UnityEngine.UI;

namespace WidgetShowcase
{
		public enum JoystickEventType
		{
				Start,
				Changed,
				End
		}

		public struct JoystickEvent
		{
				public JoystickEventType Type;
				public Vector3 Direction; // global coordinate direction from reference  to draggable
				public Quaternion Rotation; // global coordinate rotation from reference to draggable

				public JoystickEvent (JoystickEventType type, Vector3 location, Quaternion rotation)
				{
						Type = type;
						Direction = location;
						Rotation = rotation;
				}
		}


}
