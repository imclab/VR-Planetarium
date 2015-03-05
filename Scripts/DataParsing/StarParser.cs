//#define DEBUG

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Stars {
  public class StarLoadedEventArgs : EventArgs
  {
  	private StarData[] m_stars;

  	public StarData[] Stars { get { return m_stars; } }

  	public StarLoadedEventArgs (StarData[] stars)
  	{
  		m_stars = stars;
  	}
  }

  public class StarParser : MonoBehaviour
  {
    private static Dictionary<uint, int> m_HD_idToIndex = new Dictionary<uint, int>();
    public static Dictionary<uint, int> HD_idToIndex { get { return m_HD_idToIndex; } } 

    private  static float MinStarDist = float.MaxValue;
    private  static float MaxStarDist = float.MinValue;
    private  static float MinStarMag = float.MaxValue;
    private  static float MaxStarMag = float.MinValue;
    private  static float MinAbsStarMag = float.MaxValue;
    private  static float MaxAbsStarMag = float.MinValue;
    private  static float MinStarColorIndex = float.MinValue;
    private  static float MaxStarColorIndex = float.MinValue;

    [SerializeField]
    float			minInputDist = 1.3f;
    [SerializeField]
    float			maxInputDist = 10000;
    [SerializeField]
    float			minInputMag = -1.44f;
    [SerializeField]
    float			maxInputMag = 10;
    [SerializeField]
    float			minInputAbsMag = -11.06f;
    [SerializeField]
    float			maxInputAbsMag = 8.7f;
    [SerializeField]
    float		  minInputColorIndex = -2;
    [SerializeField]
    float		  maxInputColorIndex = 4;
    [SerializeField]
    AnimationCurve  magnitudeCurve;
    [SerializeField]
    AnimationCurve  distanceCurve;
    [SerializeField]
    TextAsset m_starDataCSV;
    
    public static StarParser Instance;
    
    private StarData[] m_stars;

    public static float MinInputDist { get { if (Instance) return Instance.minInputDist; else return -1.44f; }}
    public static float MaxInputDist { get { if (Instance) return Instance.maxInputDist; else return 10;}}
    public static float MinInputMag { get { if (Instance) return Instance.minInputMag; else return -1.44f;}}
    public static float MaxInputMag { get { if (Instance) return Instance.maxInputMag; else return 10;}}
    public static float MinInputAbsMag { get { if (Instance) return Instance.minInputAbsMag; else return -11.06f;}}
    public static float MaxInputAbsMag { get { if (Instance) return Instance.maxInputAbsMag; else return 8.7f;}}
    public static float MinInputColorIndex { get { if (Instance) return Instance.minInputColorIndex; else return -2;}}
    public static float MaxInputColorIndex { get { if (Instance) return Instance.maxInputColorIndex; else return 4;}}
    public static AnimationCurve MagnitudeCurve { get { if (Instance) return Instance.magnitudeCurve; else return null; }}
    public static AnimationCurve DistanceCurve { get { if (Instance) return Instance.distanceCurve; else return null; }}

    public event EventHandler<StarLoadedEventArgs> StarsLoadedHandler;

    void Awake ()
    {
    		if (Instance == null) {
    				Instance = this;
    		} else {
    				throw new System.Exception ("Attempting to create multiple StarParser instances.");
    		}
    }

    // Use this for initialization
    void Start ()
    {
    		string[] lines = parseCSVToLines (m_starDataCSV.text);
        Asterisms.AsterismParser.LoadAsterismData();
    		StartCoroutine (parseCSVLinesToStars (lines));
    }

    private string[] parseCSVToLines (string csv)
    {
    		return csv.Split ('\n');
    }

/*  
Index    STAR DATA
====  =================
0  	  StarID,
1  	  HIP,
2  	  HD,
3  	  HR,
4  	  Gliese,
5  	  BayerFlamsteed,
6  	  ProperName,
7  	  RA,
8  	  Dec,
9  	  Distance,
10 	  PMRA,
11 	  PMDec,
12    RV,
13 	  Mag,
14 	  AbsMag,
15 	  Spectrum,
16	  ColorIndex,
17	  X,
18	  Y,
19	  Z,
20	  VX,
21	  VY,
22	  VZ
*/

  	//uint id, string name, float ra, float dec, float dist, float mag
  	private IEnumerator parseCSVLinesToStars (string[] lines)
  	{
  	  float lastTimeCheck = Time.time;
  		float lastFrameTime = Time.time;

  #if DEBUG
  		Debug.Log ("lines length: " + lines.Length);
  #endif

  		m_stars = new StarData[lines.Length - 2];
  		int count = 0;

  		foreach (string line in lines) {
    		string[] lineData = line.Split (',');
    		if (lineData.Length != 23) {
    				continue;
    		} //ingnore incomplete lines
    		if (lineData [0] == "StarID") { 
  #if DEBUG
    				Debug.Log ("Ignoring first line");
  #endif
    				continue;
    		} 

    		uint id;
        uint hdId;
    		string starName = lineData [6];
    		float ra;
    		float dec;
    		float dist;
    		float mag;
    		float absMag;
    		float colorIndex;

    		// Attempt parsing
    		bool idParsed = uint.TryParse (lineData [0], out id);
        uint.TryParse (lineData [2], out hdId); // We don't care if it parses or not.
    		bool raParsed = float.TryParse (lineData [7], out ra);
    		bool decParsed = float.TryParse (lineData [8], out dec);
    		bool distParsed = float.TryParse (lineData [9], out dist);
    		bool magParsed = float.TryParse (lineData [13], out mag);
    		bool absMagParsed = float.TryParse (lineData [14], out absMag);
    		float.TryParse (lineData [16], out colorIndex); // We don't really care if it parses or not

    		// Check if parsing successful
    		if (!idParsed ||
    				!raParsed ||
    				!decParsed ||
    				!distParsed ||
    				!magParsed ||
    				!absMagParsed) {
    				Debug.Log ("missing key data: " + line);
    				continue;
    		}


    		if ( mag > 8.0f ) { continue; } // Keep to human eye limiting magnitude
    		if (id == 0) {
    				continue;
    		} // Skip sol

    		m_stars [count] = new StarData (id, hdId, starName, ra, dec, dist, mag, absMag, colorIndex);

        if ( hdId != 0) {
          if ( !m_HD_idToIndex.ContainsKey(hdId) ) {
            m_HD_idToIndex.Add(hdId, count);
          }
          else {
            Debug.LogWarning("already parsed this HD_ID: " + hdId);
          }
        }

  			count++;

  			if (dist < MinStarDist) { 
  				MinStarDist = dist;
  			} else if (dist > MaxStarDist) {
  				MaxStarDist = dist;
  			}

  			if (mag < MinStarMag) { 
  				MinStarMag = mag;
  			} else if (mag > MaxStarMag) {
  				MaxStarMag = mag;
  			}

  			if (absMag < MinAbsStarMag) { 
  				MinAbsStarMag = absMag;
  			} else if (absMag > MaxAbsStarMag) {
  				MaxAbsStarMag = absMag;
  			}

  			if (colorIndex < MinStarColorIndex) { 
  				MinStarColorIndex = colorIndex;
  			} else if (colorIndex > MaxStarColorIndex) {
  				MaxStarColorIndex = colorIndex;
  			}

  			if (Time.time - lastFrameTime >= 0.25) {
  				lastFrameTime = Time.time;
  				yield return true;
  			}
  		}

  		Array.Resize<StarData> (ref m_stars, count);

  #if DEBUG
  		
  		Debug.Log ("minDist: " + MinStarDist);
  		Debug.Log ("maxDist: " + MaxStarDist);
  		Debug.Log ("minMag: " + MinStarMag);
  		Debug.Log ("maxMag: " + MaxStarMag);
  		Debug.Log ("minAbsMag: " + MinAbsStarMag);
  		Debug.Log ("maxAbsMag: " + MaxAbsStarMag);
  		Debug.Log ("minColorIndex: " + MinStarColorIndex);
  		Debug.Log ("maxColorIndex: " + MaxStarColorIndex);

  #endif

  		onStarsLoaded ();
  		yield break;
  	}

  	private void onStarsLoaded ()
  	{
  #if DEBUG
      Debug.Log ("Star loading complete.");
  #endif
  		EventHandler<StarLoadedEventArgs> handler = StarsLoadedHandler;

  		if (handler != null) {
  			handler(this, new StarLoadedEventArgs(m_stars));
  		}
  	}   
  }
}