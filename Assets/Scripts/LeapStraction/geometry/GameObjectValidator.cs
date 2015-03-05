using UnityEngine;
using System;
using System.Collections;

namespace WidgetShowcase
{
		public abstract class GameObjectValidator : MonoBehaviour
		{
				public abstract bool Test (GameObject obj);
		}

}