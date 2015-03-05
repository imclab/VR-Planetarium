using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
This class keeps track of when the last time an object was touched by an object in a hand 
*/

namespace WidgetShowcase
{
	public struct HandTouchData
	{
		public int ID;
		public float LastTimeHandEntered;
		public float LastTimeHandLeft;
		
		public HandTouchData (int id, float e, float l)
		{
			ID = id;
			LastTimeHandEntered = e;
			LastTimeHandLeft = l;
		}
	}

}