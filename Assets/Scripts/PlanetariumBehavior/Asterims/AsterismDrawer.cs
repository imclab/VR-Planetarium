using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Stars;
using LMLineDrawing;

namespace Asterisms {
  public class AsterismDrawer {
    private static Color m_asterismColor = new Color(1.0f,1.0f,1.0f, 0.3f);
    private static float m_asterismOpacity = 0.3f;
    private static Color m_borderColor = new Color(1.0f,1.0f,1.0f, 0.0f);
    private static Color m_borderSelectedColor = new Color(1.0f,1.0f,1.0f, 0.5f);
    private static Camera m_rightCamera;
    private static Camera m_leftCamera;
    private static Material m_lineMat;
    private static bool m_drawAsterisms = true;
    private static GameObject m_asterismLineRoot = null;

    public static Camera LeftCamera { 
      set { m_leftCamera = value; } 
      get { return m_leftCamera; }
    }
    public static Camera RightCamera { 
      set { m_rightCamera = value; } 
      get { return m_leftCamera; }
    }

    public static bool DrawToggle { get { return m_drawAsterisms; } set { m_drawAsterisms = value; } }

    public static void GetAsterismLines(ref StarData[] starData) {
      bool initilized = true;
      for(int i=0; i<AsterismParser.AsterismData.Length; i++) {
        if ( !initilized ) { break; }
        Asterism asterism = AsterismParser.AsterismData[i];
        Vector3[] positionArray = new Vector3[asterism.HD_ids.Length];
        for(int j=0; j<asterism.HD_ids.Length;j++) {
          uint hdid = asterism.HD_ids[j];

          try {
            StarData star = starData[StarParser.HD_idToIndex[hdid]];
            GameObject starObj = star.GameObjectRepresentation;
            if ( starObj != null ) {

              if ( starObj.transform.position == Vector3.zero ) { 
                initilized = false;
                break; 
              }

              positionArray[j] = starObj.transform.position;

            }
            else {
              Debug.LogWarning("Ummm...this star is missing a game object. Skipping the asterism: " + asterism.name);
            }
          }
          catch (KeyNotFoundException e) {
            Debug.Log(e.Message);
            Debug.LogWarning("Ummm...this star is missing: " + hdid + ". Skipping the asterism: " + asterism.name);
            continue;
          }
        }

        if ( !initilized ) { return; }

        if ( asterism.lineArt == null ) {
          if ( m_lineMat == null ) {
            m_lineMat = Resources.Load("Lines/LineMat") as Material;
          }

          if ( m_asterismLineRoot == null ) { m_asterismLineRoot = new GameObject("AsterismLineRoot"); }

          LineObject asterismLines = LineObject.LineFactory(positionArray, 3.0f, m_leftCamera, m_lineMat);

          asterismLines.gameObject.name = asterism.name + "_line";

          asterismLines.transform.parent = m_asterismLineRoot.transform;

          asterism.lineArt = asterismLines;
        }
        else {
          asterism.lineArt.Points = positionArray;
        }
        AsterismParser.AsterismData[i] = asterism;
      }

      setAsterismColor(m_asterismColor);
    }

    public static void DisableAllAsterisms() {
		try {    
		  for(int i=0; i<AsterismParser.AsterismData.Length;i++) {
						try  {AsterismParser.AsterismData[i].IsSelected = false;} catch {}
		        	
		      }
		}
      catch (System.NullReferenceException nex){ Debug.LogWarning("Null reference: " + nex.Message); }
    }

    public static void UpdateAsterismBorders() {
      foreach ( Asterism asterism in AsterismParser.AsterismData ) {
        if ( asterism.borderArt == null ) { 
          continue; 
        }

        Color currentColor = asterism.borderArt.renderer.material.color;
        Color goalColor = asterism.IsSelected ? m_borderSelectedColor : m_borderColor;
        Vector4 diff = (currentColor - goalColor);
        float mag = diff.magnitude;

        if ( mag < 0.01 ) {
          continue;
        }

        Color color = Color.Lerp(currentColor, goalColor, 0.5f);

        asterism.borderArt.renderer.material.color = color;
      }

      DisableAllAsterisms();
    }

    private static void setAsterismColor(Color color) {
      foreach(Asterism asterism in AsterismParser.AsterismData) {
        if(asterism.lineArt == null ) { continue; }
        Color current = asterism.lineArt.renderer.material.color;
        Vector4 diff = current - color;
        if ( diff.magnitude > 0.01f ) { 
          asterism.lineArt.renderer.material.color = color;
        }
      }
    }

    public static void SetAsterismOpacity(float opacity) {
      opacity = Mathf.Clamp01(opacity);
      m_asterismOpacity = opacity;
      //Debug.Log("Set Opacity | DrawToggle: " + m_drawAsterisms + " | m_asterismOpacity: " + m_asterismOpacity);
      m_asterismColor.a = m_drawAsterisms ? m_asterismOpacity : 0.0f;
      setAsterismColor(m_asterismColor);
    }

    public static void TurnOnAsterisms() { 
      DrawToggle = true;
      SetAsterismOpacity(Brightness); 
    }

    public static void TurnOffAsterisms() {
      DrawToggle = false;
      SetAsterismOpacity(Brightness); 
    }

    public static float Brightness { get { return m_asterismOpacity; } }

//    public static void DrawAsterisms(bool cullOffCameraAsterisms = true) {
//      if ( m_leftCamera == null || m_rightCamera == null ) { 
//        Debug.LogError("No Draw Camera Defined.");
//        return;
//      }
//
//      if ( AsterismParser.AsterismData == null ) { return; }
//
//      //VectorLine.SetCamera3D(m_leftCamera);
//      foreach(Asterism asterism in AsterismParser.AsterismData) {
//        //if ( asterism.lineArt == null ) { continue; }
//        if ( asterism.mover != null && m_leftCamera != null ) {
//          if ( asterism.mover.transform.position == asterism.root.transform.position && cullOffCameraAsterisms) {
//            Vector2 screenPoint = m_leftCamera.WorldToScreenPoint(asterism.mover.transform.position);
//            if ( screenPoint.x < 0 || screenPoint.x > m_leftCamera.pixelWidth || screenPoint.y < 0 || screenPoint.y > m_leftCamera.pixelHeight ) { continue; }
//          }
//        }
//        try {
//          asterism.lineArt.Draw3D();
//        }
//        catch (UnityException e) {
//          Debug.LogError("error: " + e.Message);
//        }
//      }
//    }
  }
}
