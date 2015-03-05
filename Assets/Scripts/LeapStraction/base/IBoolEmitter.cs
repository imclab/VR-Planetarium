using UnityEngine;
using System;
using System.Collections;

namespace WidgetShowcase
{
		public interface IBoolEmitter
		{
				bool BoolValue { get; set; }

				event EventHandler<WidgetEventArg<bool>> BoolEvent;
		}

}