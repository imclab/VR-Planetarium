using UnityEngine;
using Asterisms;
using System;
using System.Collections;
using System.Collections.Generic;

/*
 * Star Updater
 * 
 * Purpose: 
 *  - Hand the initial generation of the particle and game object stars.
 *  - Manage the positonal layout and updating of the stars.
 *  - Be the handler for incloming requests to change the layout of the stars.
 *  - Manage the calls to updating the drawing of the asterisms.
 * 
 * Notes of interest: 
 * - onStarsLoaded is the critical "start point" for all real execution here.
 * - Depends on StarParser for data and event calls.
 */

namespace Stars {
  public class StarUpdater : MonoBehaviour {

    public Transform playerController;

    public GameObject StarLabelPrototype;
    public GameObject ConstellationLabelPrototype;

    public static StarUpdater Instance;

    public event EventHandler FirstDrawCompleteHandler;
    private bool m_firstDrawDone = false;

    // Getters that may be useful for UI and other scaling needes.
    public static Vector2 MinDistRange { get { if (Instance) return Instance.minDistRange; else return new Vector2(0,0); }}
    public static Vector2 MaxDistRange { get { if (Instance) return Instance.maxDistRange; else return new Vector2(0,0); }}
    public static Vector2 MaxScaleRange { get { if (Instance) return Instance.maxScaleRange; else return new Vector2(0,0); }}
    public static Vector2 MinScaleRange { get { if (Instance) return Instance.minScaleRange; else return new Vector2(0,0); }}

    /*
     * EDITOR SERIALIZED VALUES
     */
    [SerializeField]
    GameObject m_polarAligner;
    [SerializeField]
    GameObject m_starPrefab; // The game object to spawn for interactive stars
    [SerializeField]
    Camera    m_cameraLeft; // The camera to use for vectrosity rendering
    [SerializeField]
    Camera    m_cameraRight;
    [SerializeField]
    float zoom = 0.5f;
    [SerializeField]
    float zoomStep = 0.05f;
    [SerializeField]
    Vector2 minZoom = new Vector2(10.0f, 1000.0f);
    [SerializeField]
    Vector2 maxZoom = new Vector2(80.0f, 500.0f);
    [SerializeField]
    float     distStep = 2.0f;
    [SerializeField]
    Vector2   minDistRange = new Vector2(10.0f, 500.0f);
    [SerializeField]
    float     minDist = 21.12f;
    [SerializeField]
    Vector2   maxDistRange = new Vector2(500.0f, 1000.0f);
    [SerializeField]
    float     maxDist = 1000;
    //[SerializeField]
    //float     scaleStep = 0.1f;
    [SerializeField]
    Vector2   minScaleRange = new Vector2(0.05f, 0.4f);
    [SerializeField]
    float    minScale = 0.3f;
    [SerializeField]
    Vector2   maxScaleRange = new Vector2(0.4f, 1.5f);
    [SerializeField]
    float    maxScale = 0.4f;
    [SerializeField]
    float saturationStep = 0.1f;
    [SerializeField]
    float    saturationLevel = 0.3f;
    [SerializeField]
    Color     blueColorSaturated = new Color (114f, 262f, 255f, 1.0f);
    [SerializeField]
    Color     redColorSaturated = new Color (255f, 208f, 169f, 1.0f);
    [SerializeField]
    float     minLuminosityMod = 0.4f;
    [SerializeField]
    float     maxLuminosityMod = 1.0f;
    [SerializeField]
    float     luminanceFilterStep = 0.1f;
    [SerializeField]
    float     minLuminance = 0.0f;
    [SerializeField]
    float LABEL_DISTANCE = 300f;
    [SerializeField]
    float labelOpacityStep = 25.5f;
    [SerializeField]
    float labelOpacity = 0.5f;
    [SerializeField]
    bool drawLabels = true;
    [SerializeField]
    AnimationCurve luminanceCurve;

    /*
     * END EDITOR SERIALIZED VALUES
     */

    /*
     * STAR DATA AND DRAWING STRUCTURES
     */

    // Star Field Particle Objects
    //private ParticleSystem m_starSystem;
    //private ParticleSystem.Particle[] starParticles;

    
    private GameObject[] m_backgroundStarObjects;
    
    [SerializeField]
    private Material m_starMat;
    
    private int m_batchSize = 16250;

    // Vectrosity points
    //VectorPoints m_starPoints;

    // Abstract star data to be recieved from the star parser
    StarData[] m_stars;

    // Reference to the static StarParser instance
    private StarParser m_starParser;

    // Reference to all the star labels in the scene
    public HashSet<StarLabel> m_starLabels;
    public HashSet<ConstellationLabel> m_constellationLabels;

    private bool m_isPolarAligned = false;
		public GameObject iAmPolaris;
    /*
     * END STAR DATA AND DRAWING STRUCTURES
     */

    /*
     * ASYNC UPDATER VARS
     */

    // State for async updater
    private bool isUpdating = false;

    // State history to determine if an update is needed
    float last_minDist;
    float last_maxDist;
    float last_minScale;
    float last_maxScale;
    private float last_saturationLevel;
    private Color last_blueSaturatedColor;
    private Color last_redSaturatedColor;
    private float last_minLuminance;
    private float last_minLuminosityMod;
    private float last_maxLuminosityMod;
    private AnimationCurve last_magnitudeCurve;
    private AnimationCurve last_distanceCurve;
    private AnimationCurve last_luminanceCurve;

    // State flags marking what manners of update are needed
    // Flags are marked true if data has changed since last update
    private bool m_distDirty = true;
    private bool m_scaleDirty = true;
    private bool m_colorDirty = true;
    private bool m_luminosityDirty = true;

    /*
     * END ASYNC UPDATER VARS
     */


    /*
     * LAYOUT VALUE DIRECT SETTERS
     */

      // As it ins't guarnteed the sent value is the final value
      // these functions return the value that the field was 
      // actually set to.

    // Set the "Star Zoom" or "Depth-y-ness" of the scene
    public float SetZoom(float value) {
      value = Mathf.Clamp01(value);
      zoom = value;
      Vector2 newMinAndMaxDist = Vector2.Lerp(minZoom, maxZoom, zoom);
      SetMinDist(newMinAndMaxDist.x);
      SetMaxDist(newMinAndMaxDist.y);
      return zoom;
    }

    // Set the min-bound distance from origin for the stars in the scene
    public float SetMinDist(float value) {
      value = Mathf.Clamp(value, minDistRange.x, minDistRange.y);
      minDist = value;
      return minDist;
    }

    // Set the max-bound distance from origin for the stars in the scene
    public float SetMaxDist(float value) {
      value = Mathf.Clamp(value, maxDistRange.x, maxDistRange.y);
      maxDist = value;
      return maxDist;
    }

    // Set the min-bound scale for the stars in the scene
    public float SetMinScale(float value) {
      value = Mathf.Clamp(value, minScaleRange.x, minScaleRange.y);
      minScale = value;
      return minScale;
    }

    // Set the max-bound scale for the stars in the scene
    public float SetMaxScale(float value) {
      value = Mathf.Clamp(value, maxScaleRange.x, maxScaleRange.y);
      maxScale = value;
      return maxScale;
    }

    public float SetLabelOpacity(float opacity) {
      float drawOpacity = (LabelDrawToggle) ? opacity : 0.0f;
      foreach(StarLabel label in m_starLabels) {
        Color temp = label.LabelComp.color;
        temp.a = drawOpacity;
        label.LabelComp.color = temp;       
      }
      foreach(ConstellationLabel label in m_constellationLabels) {
        Color temp = label.LabelComp.color;
        temp.a = drawOpacity * 0.8f;
        label.LabelComp.color = temp;       
      }

      labelOpacity = opacity;
      return opacity;
    }

    public float SetStarSaturation(float value) {
      value = Mathf.Clamp01(value);
      saturationLevel = value;
      return saturationLevel;
    }

    public float SetMinLuminance(float value) {
      value = Mathf.Clamp01(value);
      minLuminance = value;
      return minLuminance;
    }

    public bool SetLabelDrawing(bool value) { 
      drawLabels = value;
      SetLabelOpacity(LabelOpacity);
      return value;
    }

    /*
     * END LAYOUT VALUE DIRECT SETTERS
     */

    /*
     * LAYOUT VALUE GETTERS
     */

    public float Zoom { get { return zoom; } }
    public float Saturation { get { return saturationLevel; } }
    public float LabelOpacity { get { return labelOpacity; } }
    public bool LabelDrawToggle { get { return drawLabels; } } 
    public float MinLuminance { get { return minLuminance; } } 
    public float MilkyWayIntensity { get { return 0.0f; } } 

    /*
     * END LAYOUT VALUE GETTERS
     */

    /*
     * LAYOUT VALUE INCREMENTERS
     */
    public float IncrementZoom() { return SetZoom(zoom + zoomStep); }
    public float DecrimentZoom() { return SetZoom(zoom - zoomStep); }
    public float IncrementMinDist() { return SetMinDist(minDist + distStep); }
    public float DecrimentMinDist() { return SetMinDist(minDist - distStep); }
    public float IncrementMaxDist() { return SetMaxDist(maxDist + distStep); }
    public float DecrimentMaxDist() { return SetMaxDist(maxDist - distStep); }
    public float IncrementMinScale() { return SetMinScale(minScale + distStep); }
    public float DecrimentMinScale() { return SetMinScale(minScale - distStep); }
    public float IncrementMaxScale() { return SetMaxScale(maxScale + distStep); }
    public float DecrimentMaxScale() { return SetMaxScale(maxScale - distStep); }
    public float IncrementSaturationLevel() { return SetStarSaturation(saturationLevel + saturationStep); }
    public float DecrimentSaturationLevel() { return SetStarSaturation(saturationLevel - saturationStep); }
    public float IncrementLabelOpacity() { return SetLabelOpacity(labelOpacity + labelOpacityStep); }
    public float DecrimentLabelOpacity() { return SetLabelOpacity(labelOpacity - labelOpacityStep); }
    public float IncrementSaturation() { return SetStarSaturation(saturationLevel + saturationStep); }
    public float DecrimentSaturation() { return SetStarSaturation(saturationLevel - saturationStep); }
    public float IncrementLuminanceFilter() { return SetMinLuminance(minLuminance + luminanceFilterStep); }
    public float DecrimentLuminanceFilter() { return SetMinLuminance(minLuminance - luminanceFilterStep); }
    /*
     * END LAYOUT VALUE INCREMENTERS
     */

    /*
     * LABEL VISIBILITY MODIFIERS
     */
    //public void TurnOnLabels() { SetLabelOpacity(0.0f); }
    //public void TurnOffLabels() { SetLabelOpacity(1.0f); }

    /*
     * END LABEL VISIBILITY MODIFIERS
     */


    // Setup a static reference to this instance
    // if it is the first created instance. Otherwise log an error.
    void Awake ()
    {
      if (Instance == null) {
        Instance = this;
      } else {
        throw new System.Exception ("Attempting to create multiple StarUpdater instances.");
      }
    }

    // Use this for initialization
    void Start () {
      Application.targetFrameRate = 120;
      //m_starSystem = gameObject.GetComponent<ParticleSystem> ();
      m_starParser = StarParser.Instance;
      m_starLabels = new HashSet<StarLabel>();
      m_constellationLabels = new HashSet<ConstellationLabel>();
      SetZoom(zoom);
      AsterismDrawer.LeftCamera = m_cameraLeft;
      AsterismDrawer.RightCamera = m_cameraRight;
      m_starParser.StarsLoadedHandler += onStarsLoaded;
    }


    // Grab parsed data from the StarParser and do the requisite setup.
    private void onStarsLoaded(object sender, StarLoadedEventArgs args) {
      m_stars = args.Stars;
      generateStars();  
    }

    // Setup the particles and objects to hold the stars. Don't set their positions.
    private void generateStars() {
      // Game Object Stars
      foreach(StarData sData in m_stars) {
        if( AsterismParser.StarsInAsterisms.Contains(sData.HD_id) ) {
          GameObject newStar = GameObject.Instantiate(m_starPrefab, Vector3.zero, Quaternion.identity) as GameObject;
          newStar.AddComponent<StarBehavior>();
          sData.GameObjectRepresentation = newStar;
        }
      }

      // Background stars
      generateStarGeometry();
      /*
      for(int i=0; i<m_stars.Length; i++) { 
        setStarColor(i, new Color32(
          (byte)UnityEngine.Random.Range(0,255),
          (byte)UnityEngine.Random.Range(0,255),
          (byte)UnityEngine.Random.Range(0,255),
          (byte)UnityEngine.Random.Range(0,255)));
      }*/
    }

    private void generateStarGeometry() {


      m_backgroundStarObjects = new GameObject[(int)Mathf.Ceil(m_stars.Length / (float)m_batchSize)];

      for ( int i=0; i<m_backgroundStarObjects.Length; i++ ) {
        m_backgroundStarObjects[i] = new GameObject("BackgroundStarGeometry" + i);
        m_backgroundStarObjects[i].AddComponent<MeshFilter>();
        m_backgroundStarObjects[i].AddComponent<MeshRenderer>();
        m_backgroundStarObjects[i].renderer.material = m_starMat;
      }

      CombineInstance[] meshes = new CombineInstance[m_batchSize];

      // Calculate all the meshes for various stars
      for( int i=0; i<m_stars.Length;i++ ) {

        // Save to mesh
        if ( i != 0 && i % m_batchSize == 0 ) {
          int gameObjectIndex = (int)Mathf.Floor((i-1)/m_batchSize);
          saveMeshArrayToObj(gameObjectIndex, meshes);
          meshes = new CombineInstance[Mathf.Min(m_stars.Length - i, m_batchSize)];
        }
        Vector3 position = starPositionFromNormalizedCoords(m_stars[i].NormalizedRightAscention, m_stars[i].NormalizedDeclination, m_stars[i].NormalisedDistance);
        float width = 0.005f;
        float scaleDistanceAdjustment = CalculateInverseScaleFactor(position);
        width *= scaleDistanceAdjustment;
        meshes[i % m_batchSize].mesh = quadFromPositionAndWidth(position, width);
      }

      saveMeshArrayToObj(m_backgroundStarObjects.Length - 1, meshes);
    }

    private void saveMeshArrayToObj(int gameObjectIndex, CombineInstance[] meshes) {
      //Debug.Log("Mesh Combine: Mesh count: " + meshes.Length + " | goIndex: " + gameObjectIndex);
      
      // Assign combined mesh to BackgroundStars GameObject.
      MeshFilter backgroundStarMesh = m_backgroundStarObjects[gameObjectIndex].transform.GetComponent<MeshFilter>();
      backgroundStarMesh.mesh.CombineMeshes(meshes, true, false);
      
      // Destroy the old meshes
      for (int j = 0; j < meshes.Length; j++) {
        Destroy (meshes[j].mesh);
      }
    }


    private void setStarColor(int index, Color32 color) {
      int[] vertIndicies = new int[4];
      int gameObjectIndex = vertexIndiciesAndGameObjectFromStarIndex(index, ref vertIndicies);
      Color32[] colors = m_backgroundStarObjects[gameObjectIndex].GetComponent<MeshFilter>().mesh.colors32;

      if ( colors.Length == 0 ) { 
        colors = new Color32[m_backgroundStarObjects[gameObjectIndex].GetComponent<MeshFilter>().mesh.vertices.Length];
      }

      foreach(int vertIndex in vertIndicies) {
        try { 
          colors[vertIndex] = color;
        }
        catch(IndexOutOfRangeException e) {
          Debug.LogError("index " + vertIndex + " is out of bounds | colors array length: " + colors.Length);
        }
      }

      m_backgroundStarObjects[gameObjectIndex].GetComponent<MeshFilter>().mesh.colors32 = colors;
    }

    private IEnumerator printVertCount(int index, Mesh mesh) {
      string vertString = "";
      
      for ( int i=0; i<mesh.vertices.Length; i++ ) {

        vertString += mesh.vertices[i].ToString() + " | "; 


        if ( i % 1000 == 0 ) { 
          Debug.Log(vertString);
          vertString = "";
          yield return 0;
        }
      }
    }

    private Mesh quadFromPositionAndWidth(Vector3 position, float width) {
      Vector3[] verts = calculateStarVerticesFromPositionAndWidth(position, width);
      return generateQuadGeometry(verts);
    }

    private Mesh generateQuadGeometry(Vector3[] vertices) {
      Mesh mesh = new Mesh();

      if ( vertices.Length != 4 ) { 
        throw new Exception("Vertex array does not contain just 4 verticies");
      }
      
      Vector2[] uv = new Vector2[]
      {
        new Vector2(1, 1),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(0, 0),
      };
      
      int[] triangles = new int[]
      {
        0, 1, 2,
        2, 1, 3,
      };

      mesh.vertices = vertices;
      mesh.uv = uv;
      mesh.triangles = triangles;
      
      return mesh;
    }

    private int vertexIndiciesAndGameObjectFromStarIndex(int starIndex, ref int[] vertexIndicies) {
      if (vertexIndicies.Length != 4) { throw new Exception("vertex indicies must be of length 4"); }

      int gameObjectIndex = (int)Mathf.Floor(starIndex/m_batchSize);

      for(int i=0; i<vertexIndicies.Length; i++) {
        vertexIndicies[i] = (starIndex % m_batchSize) * 4 + i;
      }
      return gameObjectIndex;
    }

    private Vector3[] calculateStarVerticesFromPositionAndWidth(Vector3 position, float width) {

      if ( width == 0 ) { Debug.LogError("Width is zero"); }

      if(position == Vector3.zero) { 
        Debug.Log("Zero position."); 
      }

      Vector3[] verticies = new Vector3[4];



      Vector3 planeNormal = position.normalized;

      Vector3 worldReferenceVector = Mathf.Abs(Vector3.Dot(planeNormal, Vector3.up)) < 0.9 ? Vector3.up : Vector3.right;

      Vector3 planeUp = (worldReferenceVector.normalized - Vector3.Dot (worldReferenceVector, planeNormal) * planeNormal).normalized;
      Vector3 planeRight = Vector3.Cross(planeUp, planeNormal);

      if (
        float.IsNaN(planeUp.x) ||
        float.IsNaN(planeUp.y) ||
        float.IsNaN(planeUp.z) ||
        float.IsNaN(planeRight.x) ||
        float.IsNaN(planeRight.y) ||
        float.IsNaN(planeRight.z)) 
      {
        Debug.LogError("Hooray! NaNs!");
      }

      verticies[0] = planeRight + planeUp;
      verticies[1] = planeRight - planeUp;
      verticies[2] = -planeRight + planeUp;
      verticies[3] = -planeRight - planeUp;

      for( int i=0; i < verticies.Length; i++ ) {
        verticies[i] *= width/2.0f;
        verticies[i] += position;

        if ( verticies[i] == Vector3.zero ) { Debug.Log("zero vertex."); }
      }

      return verticies;
    }



    // Update is called once per frame
    void Update ()
    {
      if ( m_stars == null ) { return; } // Don't do antthing until star data is availible

      // If we're not in an async update consider starting one.
      if(!isUpdating) {
        identifyDirtyValues();
        if ( m_distDirty ||
             m_scaleDirty ||
             m_colorDirty ||
             m_luminosityDirty
            ) 
        {
          StartCoroutine(AsyncSetPositionsAndProperties());
        }
      }

      AsterismDrawer.GetAsterismLines(ref m_stars);
      //AsterismDrawer.DrawAsterisms(false); // don't cull asterisms.
      AsterismDrawer.UpdateAsterismBorders();
    }

    // Once a properties update of all stars is complete, call this to clear all "dirty" property flags.
    private void clearDirtyProperties() {
      m_scaleDirty = false;
      m_colorDirty = false;
      m_luminosityDirty = false;
    }

    // Once a position update of all stars is complete, call this to clear all "dirty" position flags.
    private void clearDirtyPositions() {
      m_distDirty = false;
    }

    // Looks through history and finds positions and properties that have
    // been changed since the last check.
    // 
    // All the history values start null so this will make sure everthing is marked 
    // as dirty on the first run.
    private void identifyDirtyValues() {
      if (last_minDist != minDist ||
          last_maxDist != maxDist ||
          last_distanceCurve != StarParser.DistanceCurve)
      {
        m_distDirty = true;
        last_minDist = minDist;
        last_maxDist = maxDist;
        last_distanceCurve = StarParser.DistanceCurve;
      }
      
      if(last_minScale != minScale ||
         last_maxScale != maxScale ||
         last_magnitudeCurve != StarParser.MagnitudeCurve)
      {
        m_scaleDirty = true;
        last_minScale = minScale;
        last_maxScale = maxScale;
        last_magnitudeCurve = StarParser.MagnitudeCurve;
      }
      
      if(last_blueSaturatedColor != blueColorSaturated ||
         last_redSaturatedColor != redColorSaturated ||
         last_saturationLevel != saturationLevel) 
      {
        m_colorDirty = true;
        last_blueSaturatedColor = blueColorSaturated;
        last_redSaturatedColor = redColorSaturated;
        last_saturationLevel = saturationLevel;
      }
      
      if(last_minLuminosityMod != minLuminosityMod ||
         last_maxLuminosityMod != maxLuminosityMod ||
         last_minLuminance != minLuminance ||
         last_magnitudeCurve != StarParser.MagnitudeCurve ||
         !last_luminanceCurve.Equals(luminanceCurve)) 
      {
        m_luminosityDirty = true;
        last_minLuminance = minLuminance;
        last_minLuminosityMod = minLuminosityMod;
        last_maxLuminosityMod = maxLuminosityMod;
        last_magnitudeCurve = StarParser.MagnitudeCurve;
        last_luminanceCurve = luminanceCurve;
      }
    }

    // Using normalzied RA, DEC, and DIST, calculate the position of the object in 3D space relative to the origin.
    private Vector3 starPositionFromNormalizedCoords(float ra, float dec, float dist) {
      float yRoation = ra * (360.0f) * -1.0f; 
      float xRotation = (dec - 0.5f) * 180.0f * -1.0f;
      
      Quaternion direction = Quaternion.Euler (xRotation, yRoation, 0.0f);
      //Quaternion adjustment = Quaternion.Euler (180.0f, 0.0f, 0.0f);
      Vector3 directionVector = direction * Vector3.forward;
      Vector3 position = directionVector * (minDist + (dist * (maxDist - minDist)));
      return position;
    }

    // Set the position of a GameObject or Particle based star.
    private Vector3[] setStarPosition (StarData sData)
    {
      Vector3 position = starPositionFromNormalizedCoords(sData.NormalizedRightAscention, sData.NormalizedDeclination, sData.NormalisedDistance);
      sData.WorldPosition = position;

      if ( !m_isPolarAligned &&
        sData.Name == "Polaris" ) {
        m_polarAligner.transform.up = sData.WorldPosition.normalized;
        m_isPolarAligned = true;
				iAmPolaris.transform.position = sData.WorldPosition;
				iAmPolaris.name = "Polaris";
      }
			
			if ( sData.GameObjectRepresentation != null ) {
        sData.GameObjectRepresentation.transform.position = position;
        sData.GameObjectRepresentation.GetComponent<StarBehavior>().UpdateRepresentation();
      }

      Vector3[] positions = new Vector3[4];

      float width = 0.006f;
      float scaleDistanceAdjustment = CalculateInverseScaleFactor(position);
      width *= scaleDistanceAdjustment;
      return calculateStarVerticesFromPositionAndWidth(position, width);
    }

    private void setGameObjectStarScale (StarData sData) {
      if ( m_scaleDirty || m_distDirty) { 
        if ( sData.GameObjectRepresentation != null ) 
        {
          float scaleFactor = AntiNorm(sData.NormalizedAbsoluteMagnitude, minScale, maxScale);
          // Set the scale factors for the star.
          StarBehavior behavior = sData.GameObjectRepresentation.GetComponent<StarBehavior>();
          if ( behavior != null ) {
            behavior.BaseScaleFactor = scaleFactor;
            behavior.SelectedScaleFactor = scaleFactor * 1.5f;
          }
        }
      }
    }

    // Set the visual properties of a Game Object or Particle based star.
    // This function only updates properties who's inputs have changed since the last update (via the "dirty" flags).
    private Color32 calcStarColor(StarData sData) {
      if(sData.WorldPosition == Vector3.zero) { 
        throw new Exception("No world position.");
      }

      Color32 starColor = Color32.Lerp (blueColorSaturated, redColorSaturated, sData.NormalizedColorIndex);
      starColor = Color32.Lerp(Color.white, starColor, saturationLevel);
      
      if ( sData.NormalizedMagnitude < minLuminance ) {
        starColor.a = 0;
      }
      else {
        float luminosityMod = AntiNorm(sData.NormalizedMagnitude, minLuminosityMod , maxLuminosityMod);
        if ( luminanceCurve != null ) {
          luminosityMod = luminanceCurve.Evaluate(luminosityMod);
        }
        byte byteMod = (byte)Mathf.RoundToInt(luminosityMod * 255.0f);
        starColor.a = byteMod;
      }

      if ( sData.GameObjectRepresentation != null ) 
      {
        sData.GameObjectRepresentation.transform.GetChild(0).renderer.material.color = starColor;
        return new Color32(0,0,0,0); // If we have a game object, don't show the particle version.
      }

      return starColor;
    }

    public float CalculateInverseScaleFactor(Vector3 position, float minFactor = 1.0f) {
      float maxScaleFactor = 1000.0f; 
      float normDistance = position.magnitude / (1000.0f);
      return AntiNorm(normDistance, minFactor, maxScaleFactor);
    }

    // COROUTINE FUNCTION: Run via StarCoroutine(AsyncSetPositionsAndProperties());
    // Itterate over the abstract star data and update the positions and properties of all the visual representations of those stars.
    // Space calulations out across a number of frames to perserve overall application framerate.
    //
    // This function essentially double buffers the particle field. It doens't update the field until the entire particle array
    // has been updated.
    private IEnumerator AsyncSetPositionsAndProperties() {

      if (m_stars == null ||
          m_backgroundStarObjects.Length == 0 )
      { 
        yield break; 
      }

      isUpdating = true; // Make sure we don't kick off any automated update passes. (in the update loop per-se).

      // Create placeholders for vertex and color array copies
      Vector3[][] vertexArrays = new Vector3[m_backgroundStarObjects.Length][];
      Color32[][] colorArrays = new Color32[m_backgroundStarObjects.Length][];

      // Copy the vertex and color arrays from the live geomertry
      for(int i=0; i<m_backgroundStarObjects.Length; i++) {
        vertexArrays[i] = new Vector3[m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.vertexCount];
        colorArrays[i] = new Color32[m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.vertexCount];

        Array.Copy(m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.vertices, 
                   vertexArrays[i], 
                   m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.vertexCount);

        if ( m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.colors32.Length > 0 ) { 
          Array.Copy(m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.colors32, 
                     colorArrays[i], 
                     m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.colors32.Length); 
        }
      }

      // Itterate over all the stars in the abstract star data array.
      for (int i=0; i<m_stars.Length; i++) {
        int[] vertIndicies = new int[4];
        int gameObjectIndex = vertexIndiciesAndGameObjectFromStarIndex(i, ref vertIndicies);

        //Update star positions
        if ( m_distDirty ) {
          try { 
            Vector3[] position = setStarPosition(m_stars[i]);

            for(int j=0;j<vertIndicies.Length;j++) {
              vertexArrays[gameObjectIndex][vertIndicies[j]] = position[j];
            }
          }
          catch (Exception e) {
            Debug.LogError("could not calc star position of index: " + i + " error: " + e.Message);
          }
        }

        setGameObjectStarScale(m_stars[i]);

        // Update star Color
        if ( m_colorDirty || m_luminosityDirty ) {
          try {
            Color32 color = calcStarColor(m_stars[i]);

            for(int j=0;j<vertIndicies.Length;j++) {
              colorArrays[gameObjectIndex][vertIndicies[j]] = color;
            }
          }
          catch (Exception e) {
            Debug.LogError("could not calc star color of index: " + i + " error: " + e.Message);
          }
        }

        // Every 'x' number of stars, yield until the next frame.
        if (i % 10000 == 0) {
          yield return true;
        }
      }

      //Copy the updated vertex and color bufferes into the stars
      for(int i=0; i<m_backgroundStarObjects.Length; i++) {
        m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.vertices = vertexArrays[i];
        m_backgroundStarObjects[i].GetComponent<MeshFilter>().mesh.colors32 = colorArrays[i];
      }

      if ( m_distDirty ) {
        AsterismParser.RecalculateCentroids();
      }

      // Cleanup state flags.
      isUpdating = false;
      clearDirtyPositions();
      clearDirtyProperties();

      if ( m_firstDrawDone == false ) {
        m_firstDrawDone = true;
        EventHandler handler = FirstDrawCompleteHandler;
        if( handler != null ) {
          handler(this, new EventArgs());
        }
      }

      yield break;
      
    }
    
    // Create a label object along side a star
    public void GenerateStarLabel(StarData sData, Transform root = null) {
      StarLabel LabelItem = ((GameObject) Instantiate(StarLabelPrototype)).GetComponent<StarLabel>();
      LabelItem.gameObject.SetActive(true);

      if ( root != null ) {
        LabelItem.transform.parent = root;
      }

      LabelItem.LabelComp.color = new Color(255.0f, 255.0f, 255.0f, labelOpacity);
      LabelItem.Label = sData.Label;
      LabelItem.transform.localPosition = Vector3.zero;
      if ( sData.GameObjectRepresentation != null ) {
        LabelItem.StarReference = sData.GameObjectRepresentation;
      }

      sData.LabelObject = LabelItem;
      m_starLabels.Add(LabelItem);
    }

    // Denormalize a value.
    float AntiNorm (float normalizedValue, float min, float max)
    {
      float diff = Mathf.Max (0, max - min);
      return min + (normalizedValue * (diff));
    } 
  }
}
