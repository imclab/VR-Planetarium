using UnityEngine;
using System.Collections;

public class DrawInBackground : MonoBehaviour {
  public int layer = 0;

	// Use this for initialization
	void Start () {
    gameObject.renderer.material.renderQueue = layer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
