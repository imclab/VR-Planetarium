using UnityEngine;
using Vectrosity;
using Asterisms;
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

    public static StarUpdater Instance;

    // Getters that may be useful for UI and other scaling needes.
    public static Vector2 MinDistRange { get { if (Instance) return Instance.minDistRange; else return new Vector2(0,0); }}
    public static Vector2 MaxDistRange { get { if (Instance) return Instance.maxDistRange; else return new Vector2(0,0); }}
    public static Vector2 MaxScaleRange { get { if (Instance) return Instance.maxScaleRange; else return new Vector2(0,0); }}
    public static Vector2 MinScaleRange { get { if (Instance) return Instance.minScaleRange; else return new Vector2(0,0); }}

    /*
     * EDITOR SERIALIZED VALUES
     */

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
    [SerializeField]
    float     scaleStep = 0.1f;
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
    float labelOpacity = 255.0f;
    [SerializeField]
    AnimationCurve luminanceCurve;

    /*
     * END EDITOR SERIALIZED VALUES
     */

    /*
     * STAR DATA AND DRAWING STRUCTURES
     */

    // Star Field Particle Objects
    private ParticleSystem m_starSystem;
    private ParticleSystem.Particle[] starParticles;

    // Abstract star data to be recieved from the star parser
    StarData[] m_stars;

    // Reference to the static StarParser instance
    private StarParser m_starParser;

    // Reference to all the star labels in the scene
    private HashSet<StarLabel> m_starLabels;

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
      opacity = Mathf.Clamp(opacity, 0.0f, 255.0f);
      foreach(StarLabel label in m_starLabels) {
        Color temp = label.LabelComp.color;
        temp.a = opacity;
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

    /*
     * END LAYOUT VALUE DIRECT SETTERS
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
    public void TurnOnLabels() { SetLabelOpacity(0.0f); }
    public void TurnOffLabels() { SetLabelOpacity(1.0f); }

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
      m_starSystem = gameObject.GetComponent<ParticleSystem> ();
      m_starParser = StarParser.Instance;
      m_starLabels = new HashSet<StarLabel>();
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
      // Generate Game Object Stars and labels
      foreach(StarData sData in m_stars) {
        if( AsterismParser.StarsInAsterisms.Contains(sData.HD_id) ) {
          GameObject newStar = GameObject.Instantiate(m_starPrefab, Vector3.zero, Quaternion.identity) as GameObject;
          sData.GameObjectRepresentation = newStar;
          
          if (sData.Label != ""){
            generateStarLabel(sData);
          } 
        }
      }
      
      // Setup Particle Stars
      starParticles = new ParticleSystem.Particle[m_stars.Length];
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

      AsterismDrawer.DrawAsterisms();
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
      float yRoation = ra * (360.0f);
      float xRotation = (dec - 0.5f) * 180.0f;
      
      Quaternion direction = Quaternion.Euler (xRotation, yRoation, 0.0f);
      Vector3 directionVector = direction * Vector3.forward;
      Vector3 position = directionVector * (minDist + (dist * (maxDist - minDist)));
      return position;
    }

    // Set the position of a GameObject or Particle based star.
    private void setStarPosition (StarData sData, int index)
    {
      if(!m_distDirty) { return; }
     
      Vector3 position = starPositionFromNormalizedCoords(sData.NormalizedRightAscention, sData.NormalizedDeclination, sData.NormalisedDistance);
      sData.WorldPosition = position;
      if ( sData.GameObjectRepresentation != null ) {
        sData.GameObjectRepresentation.transform.position = position;
      }
      else 
      {
        starParticles[index].position = position;
      }
    }

    // Set the visual properties of a Game Object or Particle based star.
    // This function only updates properties who's inputs have changed since the last update (via the "dirty" flags).
    private void setStarProperties(StarData sData, int index) {
      if(sData.WorldPosition == Vector3.zero) { Debug.Log("No world position."); return; }

      ParticleSystem.Particle starParticle = starParticles [index];
  
      if ( m_scaleDirty || m_distDirty) { 
        float scaleFactor = AntiNorm(sData.NormalizedAbsoluteMagnitude, minScale, maxScale);
        float scaleDistanceAdjustment = calculateInverseScaleFactor(sData.WorldPosition);
        scaleFactor *= scaleDistanceAdjustment;

        if ( sData.GameObjectRepresentation != null ) 
        {
          sData.GameObjectRepresentation.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        }
        else {
          starParticle.size = scaleFactor;
        }
      }

      if ( m_colorDirty || m_luminosityDirty ) {
        Color starColor = Color.Lerp (blueColorSaturated, redColorSaturated, sData.NormalizedColorIndex);
        starColor = Color.Lerp(Color.white, starColor, saturationLevel);
        
        if ( sData.NormalizedMagnitude < minLuminance ) {
          starColor.a = 0.0f;
        }
        else {
          float luminosityMod = AntiNorm(sData.NormalizedMagnitude, minLuminosityMod , maxLuminosityMod);
          if ( luminanceCurve != null ) {
            luminosityMod = luminanceCurve.Evaluate(luminosityMod);
          }
          starColor.a = luminosityMod;
        }


        if ( sData.GameObjectRepresentation != null ) 
        {
          starParticle.color = new Color(0,0,0,0); // If we have a game object, don't show the particle version.
          sData.GameObjectRepresentation.transform.GetChild(0).renderer.material.color = starColor;
        }
        else {
          starParticle.color = starColor;
        }
      }

      // Save the updated particle to the array.
      starParticles [index] = starParticle;
    }

    private float calculateInverseScaleFactor(Vector3 position) {
      float maxScaleFactor = maxDist;
      float normDistance = position.magnitude / (maxDist);
      return AntiNorm(normDistance, 1.0f, maxScaleFactor);
    }

    // WANRING BLOCKING FUNCTION: This function will take a long time to run against a large starfield. Only run if frame-rate is not a concern.
    // Itterate over the abstract star data and update the positions and properties of all the visual representations of those stars.
    private void setPositionsAndProperties () {
      if ( m_stars == null ) { return; }
      if ( starParticles == null ) { return; }

      identifyDirtyValues();
      
      // Itterate over all the stars.
      for (int i=0; i<m_stars.Length; i++) {
        setStarPosition(m_stars[i], i);
        setStarProperties(m_stars[i], i);
      }
      
      if ( m_starSystem == null ) { return; }
      
      m_starSystem.SetParticles (starParticles, starParticles.Length); // Update the actual particle system representation. We only want to do this once when the entire system is updated.

      clearDirtyPositions();
      clearDirtyProperties();
    }

    // COROUTINE FUNCTION: Run via StarCoroutine(AsyncSetPositionsAndProperties());
    // Itterate over the abstract star data and update the positions and properties of all the visual representations of those stars.
    // Space calulations out across a number of frames to perserve overall application framerate.
    //
    // This function essentially double buffers the particle field. It doens't update the field until the entire particle array
    // has been updated.
    private IEnumerator AsyncSetPositionsAndProperties() {
      if ( m_stars == null ||
          starParticles == null || 
          m_starSystem == null) { 
        yield break; 
      }
      
      isUpdating = true; // Make sure we don't kick off any automated update passes. (in the update loop per-se).
      
      // Itterate over all the stars in the abstract star data array.
      for (int i=0; i<m_stars.Length; i++) {
        setStarPosition(m_stars[i], i);
        setStarProperties(m_stars[i], i);

        // Every 'x' number of stars, yield until the next frame.
        if (i % 10000 == 0) {
          yield return true;
        }
      }

      // If positions have changed, update the Asterisms
      if ( m_distDirty ) {
        AsterismDrawer.GetAsterismLines(ref m_stars);
      }

      // Now that we've updated the particle array, update the visual representation of the field.
      m_starSystem.SetParticles (starParticles, starParticles.Length); 

      // Cleanup state flags.
      isUpdating = false;
      clearDirtyPositions();
      clearDirtyProperties();
      yield break;
      
    }

    // COROUTINE FUNCTION: Run via StarCoroutine(AsyncSetPositions());
    // Itterate over the abstract star data and update the positions of all the visual representations of those stars.
    // Space calulations out across a number of frames to perserve overall application framerate.
    //
    // This function essentially double buffers the particle field. It doens't update the field until the entire particle array
    // has been updated.
    private IEnumerator AsyncSetPositions() {
      if ( m_stars == null ||
          starParticles == null || 
          m_starSystem == null) { 
        yield break; 
      }
      
      isUpdating = true;
      
      // Itterate over all the stars.
      for (int i=0; i<m_stars.Length; i++) {
        setStarPosition(m_stars[i], i);

        // Every 'x' number of stars, yield until the next frame.
        if (i % 10000 == 0) {
          yield return true;
        }
      }

      // If positions have changed, update the Asterisms
      if ( m_distDirty ) {
        AsterismDrawer.GetAsterismLines(ref m_stars);
        AsterismDrawer.DrawAsterisms();
      }

      // Now that we've updated the particle array, update the visual representation of the field.
      m_starSystem.SetParticles (starParticles, starParticles.Length); 

      // Cleanup state flags.
      isUpdating = false;
      clearDirtyPositions();
      yield break;
      
    }
    
    // Create a label object along side a star
    private void generateStarLabel(StarData sData) {
      Vector3 position = starPositionFromNormalizedCoords(sData.NormalizedRightAscention, sData.NormalizedDeclination, sData.NormalisedDistance);
      
      StarLabel LabelItem = ((GameObject) Instantiate(Resources.Load ("Stars/Label"))).GetComponent<StarLabel>();
      LabelItem.LabelComp.color = new Color(255.0f, 255.0f, 255.0f, labelOpacity);
      LabelItem.Label = sData.Label;
      LabelItem.gameObject.transform.position = position.normalized * LABEL_DISTANCE;
      
      LabelItem.gameObject.transform.LookAt (transform.parent.parent.position, transform.parent.parent.up);
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
