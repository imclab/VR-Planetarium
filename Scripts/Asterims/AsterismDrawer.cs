using UnityEngine;
using Vectrosity;
using System.Collections;
using Stars;

namespace Asterisms {
  public class AsterismDrawer {
    private static Color m_asterismColor = new Color(255.0f,255.0f,255.0f,0.25f);
    private static Camera m_rightCamera;
    private static Camera m_leftCamera;
    private static float m_lastAsterismOpacity = 0.25f;
    private static Material m_lineMat;

    public static Camera LeftCamera { set { m_leftCamera = value; } }
    public static Camera RightCamera { set { m_rightCamera = value; } }

    public static void GetAsterismLines(ref StarData[] starData) {
      for(int i=0; i<AsterismParser.AsterismData.Length; i++) {
        Asterism asterism = AsterismParser.AsterismData[i];
        Vector3[] positionArray = new Vector3[asterism.HD_ids.Length];
        for(int j=0; j<asterism.HD_ids.Length;j++) {
          uint hdid = asterism.HD_ids[j];
          if(StarParser.HD_idToIndex != null && StarParser.HD_idToIndex.ContainsKey(hdid)) {
            StarData star = starData[StarParser.HD_idToIndex[hdid]];
            GameObject starObj = star.GameObjectRepresentation;
            if ( starObj != null ) {
              positionArray[j] = starObj.transform.position;
            }
            else {
              Debug.LogError("Ummm...this star is missing a game object. Skipping the asterism: " + asterism.name);
            }
          }
          else {
            Debug.LogError("Ummm...this star is missing. Skipping the asterism: " + asterism.name);
            continue;
          }
        }

        if ( asterism.lineArt == null ) {
          if ( m_lineMat == null ) {
            m_lineMat = Resources.Load("Lines/LineMat") as Material;
          }

          VectorLine asterismLines = new VectorLine(asterism.name + "_line", positionArray, m_lineMat, 1.0f, LineType.Discrete);
          asterism.lineArt = asterismLines;
        }
        else {
          asterism.lineArt.points3 = positionArray;
        }
        AsterismParser.AsterismData[i] = asterism;
      }

      setAsterismColor(m_asterismColor);
    }

    private static void setAsterismColor(Color color) {
      foreach(Asterism asterism in AsterismParser.AsterismData) {
        if(asterism.lineArt == null ) { continue; }
        asterism.lineArt.SetColor(color);
      }
    }

    public static void SetAsterismOpacity(float opacity) {
      opacity = Mathf.Clamp01(opacity);
      m_lastAsterismOpacity = m_asterismColor.a;
      m_asterismColor.a = opacity;
      setAsterismColor(m_asterismColor);
    }

    public static void TurnOnAsterisms() { SetAsterismOpacity(m_lastAsterismOpacity); }
    public static void TurnOffAsterisms() { SetAsterismOpacity(0.0f); }

    public static void DrawAsterisms() {
      if ( m_leftCamera == null || m_rightCamera == null ) { 
        Debug.LogError("No Draw Camera Defined.");
        return;
      }

      if ( AsterismParser.AsterismData == null ) { return; }

      VectorLine.SetCamera3D(m_leftCamera);
      foreach(Asterism asterism in AsterismParser.AsterismData) {
        if ( asterism.lineArt == null ) { continue; }
        asterism.lineArt.Draw3D();
      }
      /*
      VectorLine.SetCamera3D(m_rightCamera);
      foreach(Asterism asterism in AsterismParser.AsterismData) {
        if ( asterism.lineArt == null ) { continue; }
        asterism.lineArt.Draw3D();
      }*/
    }
  }
}
