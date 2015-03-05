using System.Collections;
using UnityEngine;

namespace Stars {

  public class StarData 
  {
    private uint m_id;
    private uint m_hdId;
    private string m_name;
    private float m_rightAscention;
    private float m_declination;
    private float m_distance;
    private float m_magnitude;
    private float m_absMagnitude;
    private float m_colorIndex;
    private GameObject m_gameObject;
    private StarLabel m_label;
    private Vector3 m_worldPosition;

    private float m_normRA;
    private float m_normDec;
    private float m_normDist;
    private float m_normMag;
    private float m_normAbsMag;
    private float m_normColorIndex;

    public float NormalizedRightAscention { get { return m_normRA; } set { m_normRA = value; } }
    public float NormalizedDeclination { get { return m_normDec; } set { m_normDec = value; } }
    public float NormalisedDistance { get { return m_normDist; } set { m_normDist = value; } }
    public float NormalizedMagnitude { get { return m_normMag; } set { m_normMag = value; } }
    public float NormalizedAbsoluteMagnitude { get { return m_normAbsMag; } set { m_normAbsMag = value; } }
    public float NormalizedColorIndex { get { return m_normColorIndex; } set { m_normColorIndex = value; } }

  	public uint Id { get { return m_id; } }
    public uint HD_id { get { return m_hdId; } }
  	public string Name { get { return m_name; } }
  	public float RightAscention { get { return m_rightAscention; } }
  	public float Declination { get { return m_declination; } }
  	public float Distance { get { return m_distance; } }
  	public float Magnitude { get { return m_magnitude; } }
  	public float AbsMagnitude { get { return m_absMagnitude; } }
  	public float ColorIndex { get { return m_colorIndex; } }
  	public string Label { get { if (m_name == null) return ""; return m_name; }}
    public GameObject GameObjectRepresentation { get { return m_gameObject; } set { m_gameObject = value; } }
    public StarLabel LabelObject { get { return m_label; } set { m_label = value; } }
    public Vector3 WorldPosition { 
      get { 
        if ( m_worldPositionSet ) { return m_worldPosition; } else { return Vector3.zero; }
      } set { 
        m_worldPositionSet = true;
        m_worldPosition = value;
      } 
    }

    private bool m_worldPositionSet = false;

  	public StarData(uint id, uint hdId, string name, float ra, float dec, float dist, float mag, float absMag, float colorIndex) {
  		m_id = id;
      m_hdId = hdId;
  		m_name = name;
  		m_rightAscention = ra;
  		m_declination = dec;
  		m_distance = dist;
  		m_magnitude = mag;
  		m_absMagnitude = absMag;
  		m_colorIndex = colorIndex;

      m_normRA = Mathf.Clamp01(RightAscention / 24.0f);
      m_normDec = Mathf.Clamp01( (Declination +90.0f) / 180.0f );
      m_normDist = Mathf.Clamp01 ((Distance - StarParser.MinInputDist) / (StarParser.MaxInputDist - StarParser.MinInputDist));
      m_normMag = OneMinusNorm( m_magnitude , StarParser.MinInputMag, StarParser.MaxInputMag, StarParser.MagnitudeCurve );
      m_normAbsMag = OneMinusNorm(m_absMagnitude, StarParser.MinInputAbsMag, StarParser.MaxInputAbsMag, StarParser.MagnitudeCurve);
      m_normColorIndex = Norm(m_colorIndex , StarParser.MinInputColorIndex, StarParser.MaxInputColorIndex );

  	}

    float Norm (float value, float min, float max)
    {
      return Mathf.Clamp01((value - min)/(max - min));
    }
    
    float Norm (float value, float min, float max, AnimationCurve curve)
    {
      return curve.Evaluate(Norm(value, min, max));
    }
    
    float OneMinusNorm (float value, float min, float max, AnimationCurve curve){
      return curve.Evaluate( OneMinusNorm(value, min, max));
    }
    
    float OneMinusNorm (float value, float min, float max)
    {
      return 1.0f - Norm(value, min, max);
    }    
  }
}