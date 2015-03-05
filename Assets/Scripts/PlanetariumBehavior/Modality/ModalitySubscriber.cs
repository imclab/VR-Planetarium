using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace WidgetShowcase
{
		public interface ModalitySubscriber
		{
				void Modality (ModalityManager.ModalityMessage message);
				// see Modality for messages
				void Subscribe ();   // should be called as start;
				void Unsubscribe (); // rarely needed -- should remove item from registry. calls Modality.Unsubscribe(name, self) || Modality.Unsubscribe(self);
				bool Activate ();    // attempt to register this object as the current modalithy calls Modality.Subscribe(name, self) || Modality.Subscribe(self);
				bool Deactivate (); // attempt to free the modality from locking into this object. 
		}

	
}