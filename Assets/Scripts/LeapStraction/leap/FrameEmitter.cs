using UnityEngine;
using System;
using System.Collections;
using Leap;


/**
 This is a simple pipe to translate a controller into an emitter.
 It by default, emits on Update. 
 Note, if you set both UseFixedUpdtate and UseUpdate to true,
only one of them will emit any given frame --- probably fixed Update.

note like msot DataEmitters should, the FrameEmitter is reactive to the 
DontEmitUnchangedValue field -- each frame is only emitted once;
also there is some possibility that some frames may not be emitted 
as they fall between FixedUpdate and especially Update.
*/

namespace WidgetShowcase
{
		public class FrameEmitter: DataEmitter
		{
				public virtual event EventHandler<WidgetEventArg<FrameData>> FrameEvent;

				public bool UseUpdate = true;
				public bool UseFixedUpdate = false;
				public HandController Controller;
				public FrameData frameValue;

				public FrameData FrameValue {
						get {
								return frameValue;
						}
						set {
								if (DontEmitUnchangedValue && (value == frameValue))
										return;
								frameValue = value;
								EventHandler<WidgetEventArg<FrameData>> handler = FrameEvent;
								if (handler != null) {
										handler (this, new WidgetEventArg<FrameData> (value));
								}
						}
				}

				long lastId = -1;

				void Update ()
				{
						if (UseUpdate)
								CheckFrames ();
				}

				void FixedUpdate ()
				{
						if (UseFixedUpdate)
								CheckFrames ();
				}

				void CheckFrames ()
				{
						Frame frame = Controller.GetFrame ();
						if (frame.Id != lastId) {
								FrameValue = new FrameData (frame, Controller);
								lastId = frame.Id;
						}
				}
		}

	
}