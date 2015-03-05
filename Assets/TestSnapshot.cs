using UnityEngine;
using System.Collections;

public class TestSnapshot : MonoBehaviour {
	
	public RenderTexture StatusRenderTex;

	// Use this for initialization
	void Start () {
		renderer.material.mainTexture = GetRTPixels(StatusRenderTex);
	
	}
	
	Texture2D GetRTPixels(RenderTexture rt) {
		RenderTexture currentActiveRT = RenderTexture.active;
		RenderTexture.active = rt;
		Texture2D tex = new Texture2D(rt.width, rt.height);
		tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		RenderTexture.active = currentActiveRT;
		return tex;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
