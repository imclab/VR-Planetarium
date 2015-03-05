using UnityEngine;
using System.Collections;
using Stars;

public class KeyboardControls : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  if ( Input.GetKey(KeyCode.LeftControl ) ) {
      if( Input.GetKeyDown(KeyCode.Semicolon) ) {     StarUpdater.Instance.IncrementZoom(); }
      if( Input.GetKeyDown(KeyCode.Period) ) {        StarUpdater.Instance.DecrimentZoom(); }

      if( Input.GetKeyDown(KeyCode.Quote) ) {         Asterisms.AsterismDrawer.TurnOnAsterisms(); }
      if( Input.GetKeyDown(KeyCode.Slash) ) {         Asterisms.AsterismDrawer.TurnOffAsterisms(); }

      //if( Input.GetKeyDown(KeyCode.LeftBracket) ) {   StarUpdater.Instance.TurnOnLabels(); }
      //if( Input.GetKeyDown(KeyCode.RightBracket) ) {  StarUpdater.Instance.TurnOffLabels(); }

      if( Input.GetKeyDown(KeyCode.O) ) {             StarUpdater.Instance.IncrementSaturationLevel(); }
      if( Input.GetKeyDown(KeyCode.K) ) {             StarUpdater.Instance.DecrimentSaturationLevel(); }

      if( Input.GetKeyDown(KeyCode.P) ) {             StarUpdater.Instance.IncrementLuminanceFilter(); }
      if( Input.GetKeyDown(KeyCode.L) ) {             StarUpdater.Instance.DecrimentLuminanceFilter(); }

      if( Input.GetKey(KeyCode.W) ) { TimeAndLocationHandler.IncrementLatitude(); }
      if( Input.GetKey(KeyCode.S) ) { TimeAndLocationHandler.DecrimentLatitude(); }
      if( Input.GetKey(KeyCode.A) ) { TimeAndLocationHandler.IncrementLongitude(); }
      if( Input.GetKey(KeyCode.D) ) { TimeAndLocationHandler.DecrimentLongitude(); }

      if( Input.GetKeyDown(KeyCode.Z) ) { TimeAndLocationHandler.IncrementDay(); }
      if( Input.GetKeyDown(KeyCode.X) ) { TimeAndLocationHandler.DecrimentDay(); }
      if( Input.GetKeyDown(KeyCode.C) ) { TimeAndLocationHandler.IncrementHour(); }
      if( Input.GetKeyDown(KeyCode.V) ) { TimeAndLocationHandler.DecrimentHour(); }
    }
  }
}
