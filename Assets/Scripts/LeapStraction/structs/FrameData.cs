using UnityEngine;
using System.Collections;
using Leap;

public struct FrameData
{

		public Frame CurrentFrame;
		public HandController Controller;

		public FrameData (Frame frame, HandController controller)
		{

				CurrentFrame = frame;
				Controller = controller;

		}


	public HandModel[] HandModels {
		get {
			return Controller.GetAllGraphicsHands();
		}
	}

  public HandModel[] PhysicsModels {
    get {
      return Controller.GetAllPhysicsHands();
    }
  }
	
	#region equals
		public bool Equals (FrameData p)
		{
				return p.CurrentFrame.Id == CurrentFrame.Id;
		}
	
		public static bool operator == (FrameData c1, FrameData c2)
		{
				return c1.Equals (c2);
		}
	
		public static bool operator != (FrameData c1, FrameData c2)
		{
				return !c1.Equals (c2);
		}
	#endregion

}
