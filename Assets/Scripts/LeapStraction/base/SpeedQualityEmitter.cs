using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 

reflects the pace at which the input is changing; 
if the changes are within the gates
that we define, emits true; if not emits false. 
@TODO: allow it to modify generic changes.

*/

namespace WidgetShowcase
{
		public struct SpeedQualityEmitter
		{
		
				public enum SpeedQuality
				{
						Unknown,
						TooSlow,
						TooFast,
						JustRight
				}

				public SpeedQuality Speed;

				public SpeedQualityEmitter (SpeedQuality quality)
				{
						Speed = quality;
				}
			
		}

}