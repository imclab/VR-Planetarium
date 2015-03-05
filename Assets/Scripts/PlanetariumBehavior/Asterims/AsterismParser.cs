using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LMLineDrawing;

namespace Asterisms {
	public struct Asterism {
		public string name; // Te
		public uint[] HD_ids; // The HD catalog ID of the stars ordered in vertex pairs [a1, a2, b1, b2, c1, c2, ... n1, n2]
    public LineObject lineArt;
    public LineObject borderArt;
    public Vector2[] borders; // The borders of the asterism in right ascention (x) and declination (y);
    public Mesh borderMesh;
    public ConstellationLabel label;

    // For grabber
    public GameObject root;
    public GameObject rest;
    public GameObject mover;

    public float scaleFactor;

    //State
    public bool IsSelected;
	}

	public class AsterismParser {

    private static Dictionary<string, string> m_abrevLookup = new Dictionary<string, string>() {
      {"AND","Andromeda"},
      {"DOR","Dorado"},
      {"ANT","Antila"},
      {"DRA","Draco"},
      {"APS","Apus"},
      {"EQU","Equuleus"},
      {"AQR","Aquarius"},
      {"ERI","Eridanus"},
      {"AQL","Aquila"},
      {"FOR","Fornax"},
      {"ARA","Ara"},
      {"GEM","Gemini"},
      {"ARI","Aries"},
      {"GRU","Grus"},
      {"AUR","Auriga"},
      {"HER","Hercules"},
      {"BOO","Boötes"},
      {"HOR","Horologium"},
      {"CAE","Caelum"},
      {"HYA","Hydra"},
      {"CAM","Camelopardis"},
      {"HYI","Hydrus"},
      {"CNC","Cancer"},
      {"IND","Indus"},
      {"CVN","Canes Venatici"},
      {"LAC","Lacerta"},
      {"CMA","Canis Major"},
      {"LEO","Leo"},
      {"CMI","Canis Minor"},
      {"LMI","Leo Minor"},
      {"CAP","Capricornus"},
      {"LEP","Lepus"},
      {"CAR","Carina"},
      {"LIB","Libra"},
      {"CAS","Cassiopeia"},
      {"LUP","Lupus"},
      {"CEN","Centaurus"},
      {"LYN","Lynx"},
      {"CEP","Cepheus"},
      {"LYR","Lyra"},
      {"CET","Cetus"},
      {"MEN","Mensa"},
      {"CHA","Chamaeleon"},
      {"MIC","Microscopium"},
      {"CIR","Circinus"},
      {"MON","Monoceros"},
      {"COL","Columba"},
      {"MUS","Musca"},
      {"COM","Coma Berenices"},
      {"NOR","Norma"},
      {"CRA","Corona Australis"},
      {"OCT","Octans"},
      {"CRB","Corona Borealis"},
      {"OPH","Ophiuchus"},
      {"CRV","Corvus"},
      {"ORI","Orion"},
      {"CRT","Crater"},
      {"PAV","Pavo"},
      {"CRU","Crux"},
      {"PEG","Pegasus"},
      {"CYG","Cygnus"},
      {"PER","Perseus"},
      {"DEL","Delphinus"},
      {"PHE","Phoenix"},
      {"PIC","Pictor"},
      {"SEX","Sextans"},
      {"PSC","Pisces"},
      {"TAU","Taurus"},
      {"PSA","Pisces Austrinus"},
      {"TEL","Telescopium"},
      {"PUP","Puppis"},
      {"TRI","Triangulum"},
      {"PYX","Pyxis"},
      {"TRA","Triangulum Australe"},
      {"RET","Reticulum"},
      {"TUC","Tucana"},
      {"SGE","Sagitta"},
      {"UMA","Ursa Major"},
      {"SGR","Sagittarius"},
      {"UMI","Ursa Minor"},
      {"SCO","Scorpius"},
      {"VEL","Vela"},
      {"SCL","Sculptor"},
      {"VIR","Virgo"},
      {"SCT","Scutum"},
      {"VOL","Volans"},
      {"SER","Serpens Caput"},
      {"VUL","Vulpecula"},
    };

    private static Material m_borderLineMaterial;

		private static HashSet<uint> m_asterismStars = new HashSet<uint>(); //Lookup table so we don't have to search all the asterism stars to find out if a star is in an asterism
		public static Asterism[] AsterismData;

		public static HashSet<uint> StarsInAsterisms { get { return m_asterismStars; } }

		public static void LoadAsterismData() {
      Stars.StarUpdater.Instance.FirstDrawCompleteHandler += OnStarsDrawn;
      parseStarData();
			return;
	  }

    private static void OnStarsDrawn(object sender, EventArgs args) {
      setupAsterismGrabbers();
      calcAsterismScaleFactor();
      parseBoundaryData();
    }

    public static void RecalculateCentroids() {
      for( int i=0; i<AsterismData.Length; i++ ) {
        Asterism asterism = AsterismData[i];
        Vector3 newCentroid = averageStarLocations(asterism.HD_ids);
        if ( asterism.root != null ) {
          removeParents(asterism.HD_ids);
          asterism.root.transform.position = newCentroid;
          attachParents(asterism.HD_ids, asterism.mover.transform);
        }
      }
    }

    private static void removeParents(uint[] hdIDs) {
      if ( hdIDs.Length == 0 ) { return; }

      foreach(uint hdId in hdIDs) {
        if ( !Stars.StarParser.HD_idToIndex.ContainsKey(hdId) ) {
          continue;
        }

        int index = Stars.StarParser.HD_idToIndex[hdId];
        Stars.StarData star = Stars.StarParser.Instance.Stars[index];

        if ( star.GameObjectRepresentation != null ) {
          star.GameObjectRepresentation.transform.parent = null;
        }
      }
    }

    private static void attachParents(uint[] hdIDs, Transform jewelcase) {
      if ( hdIDs.Length == 0 ) { return; }

      foreach(uint hdId in hdIDs) {
        if ( !Stars.StarParser.HD_idToIndex.ContainsKey(hdId) ) {
          continue;
        }

        int index = Stars.StarParser.HD_idToIndex[hdId];
        Stars.StarData star = Stars.StarParser.Instance.Stars[index];

        if ( star.GameObjectRepresentation != null ) {
          star.GameObjectRepresentation.transform.parent = jewelcase;
        }
      }
    }

    private static void calcAsterismScaleFactor() {
      for(int i=0; i < AsterismData.Length; i++) {
        Asterism asterism = AsterismData[i];

        Bounds bounds = new Bounds();

        foreach(var r in asterism.mover.transform.GetComponentsInChildren<Renderer>())
        {
          bounds.Encapsulate(r.bounds);
        }
        
        float maxBound = Mathf.Max(new float[] {bounds.size.x, bounds.size.y, bounds.size.z}); 
        float ratio = maxBound / 35.0f;
        float scaleFactor = 1 / ratio;
        asterism.scaleFactor = scaleFactor;
        AsterismData[i] = asterism;
      }
    }


    private static void setupAsterismGrabbers() {
      GameObject starRoot = new GameObject("StarRoot");

      for(int i=0; i < AsterismData.Length; i++) {
        Asterism asterism = AsterismData[i];
        Vector3 centroid = averageStarLocations(asterism.HD_ids);

        GameObject newRoot = new GameObject();
        GameObject rest = new GameObject();
        GameObject jewelcase = new GameObject();

        newRoot.name = asterism.name + "_root";
        newRoot.transform.position = centroid;
        newRoot.transform.rotation = Quaternion.identity;

        rest.name = "rest";
        rest.transform.parent = newRoot.transform;
        rest.transform.localPosition = Vector3.zero;
        rest.transform.localRotation = Quaternion.identity;
        rest.transform.localScale = new Vector3(1.0f,1.0f,1.0f);

        jewelcase.name = "jewelcase";
        jewelcase.transform.parent = newRoot.transform;
        jewelcase.transform.localPosition = Vector3.zero;
        jewelcase.transform.localRotation = Quaternion.identity;
        jewelcase.transform.localScale = new Vector3(1.0f,1.0f,1.0f);
        PullToHold pullscript = jewelcase.AddComponent<PullToHold>();
        pullscript.asterismKey = i;
        pullscript.rest = rest.transform;
        pullscript.maxSpeed = 20.0f;

        newRoot.transform.parent = starRoot.transform;
        asterism.root = newRoot;
        asterism.rest = rest;
        asterism.mover = jewelcase;

        System.Collections.Generic.HashSet<uint> used = new System.Collections.Generic.HashSet<uint>();

        GameObject constellationLabel = GameObject.Instantiate(Stars.StarUpdater.Instance.ConstellationLabelPrototype) as GameObject;
        constellationLabel.SetActive(true);
        ConstellationLabel labelBehavior = constellationLabel.GetComponent<ConstellationLabel>();

        if ( asterism.root != null ) {
          labelBehavior.transform.parent = asterism.root.transform;
        }

        labelBehavior.Label = asterism.name;
        labelBehavior.transform.localPosition = (asterism.root.transform.position.normalized * 500.0f);
        asterism.label = labelBehavior;
        labelBehavior.LabelComp.color = new Color(1.0f, 1.0f, 1.0f, Stars.StarUpdater.Instance.LabelOpacity * 0.8f);

        Stars.StarUpdater.Instance.m_constellationLabels.Add(labelBehavior);

        foreach(uint hdId in asterism.HD_ids) {
          if ( !Stars.StarParser.HD_idToIndex.ContainsKey(hdId) ) {
            continue;
          }

          int index = Stars.StarParser.HD_idToIndex[hdId];
          Stars.StarData star = Stars.StarParser.Instance.Stars[index];

          star.AsterismIndex = i;
          Stars.StarParser.Instance.Stars[index] = star;
          if ( star.GameObjectRepresentation != null ) {
            star.GameObjectRepresentation.name = hdId.ToString();
            star.GameObjectRepresentation.transform.parent = jewelcase.transform;
            if (star.Label != ""){

              if(used.Contains(star.HD_id)) {
                continue;
              }

              Stars.StarUpdater.Instance.GenerateStarLabel(star, star.GameObjectRepresentation.transform);
              used.Add(star.HD_id);
            }
          }
        }

        AsterismData[i] = asterism;
      }
    }

    private static Vector3 averageStarLocations(uint[] hdIDs) {
      if ( hdIDs.Length == 0 ) { return Vector3.zero; }

      Vector3 sumVector = Vector3.zero;

      foreach(uint hdId in hdIDs) {
        if ( !Stars.StarParser.HD_idToIndex.ContainsKey(hdId) ) {
          continue;
        }

        int index = Stars.StarParser.HD_idToIndex[hdId];
        Stars.StarData star = Stars.StarParser.Instance.Stars[index];
        if ( star.GameObjectRepresentation != null ) {
          Vector3 position = star.GameObjectRepresentation.transform.position;
          sumVector += position;
        }

      }

      sumVector /= hdIDs.Length;

      return sumVector;
    }

    private static void parseStarData() {
      TextAsset asterismCSV = Resources.Load("Stardata/AsterismLookupTable") as TextAsset;

      if ( asterismCSV == null ) {
        Debug.LogError("could not load asterism csv.");
        return;
      }

      string[] lines = asterismCSV.ToString().Split('\n');
      AsterismData = new Asterism[lines.Length];

      for(int i=0; i<lines.Length; i++) {
        string line = lines[i];
        Asterism newAsterism = new Asterism();

        newAsterism.IsSelected = false;
        newAsterism.scaleFactor = 1.0f;

        string[] fields = line.Split(',');

        if( fields.Length <= 1 ) {
          Debug.LogError("Asterism data has fewer than 2 fields. Cannot parse.");
          continue;
        }

        newAsterism.name = fields[0];
        newAsterism.HD_ids = new uint[fields.Length - 1];

        for(int j=1;j<fields.Length;j++) {
          uint hdId;
          bool parseSuccessful = uint.TryParse(fields[j], out hdId);
          if ( parseSuccessful ) {
            newAsterism.HD_ids[j-1] = hdId;
            m_asterismStars.Add(hdId);
          }
          else {
            Debug.LogError("Could not parse asterism " + newAsterism.name + " star HDID: " + fields[j] );
          }
        }

        AsterismData[i] = newAsterism;
      }
    }

    private static int asterismIndexFromName(string name) {
      if ( AsterismData == null ) { return -1; }
      for(int i = 0; i < AsterismData.Length; i++) {
        if(AsterismData[i].name == name) { return i; }
      }
      return -1;
    }

    private static Mesh invertNormals(Mesh mesh) {
      for(int i=0;i<mesh.normals.Length;i++) {
        mesh.normals[i] = -mesh.normals[i];
      }
      return mesh;
    }

    private static void parseBoundaryData() {
      GameObject collisionRoot = new GameObject("CollisionRoot");
      collisionRoot.transform.position = Vector3.zero;
      collisionRoot.transform.rotation = Quaternion.identity;

      GameObject borderRoot = new GameObject("BordernRoot");
      borderRoot.transform.position = Vector3.zero;
      borderRoot.transform.rotation = Quaternion.identity;

      TextAsset bordersCSV = Resources.Load("Stardata/boundaries") as TextAsset;

      if ( bordersCSV == null ) {
        Debug.LogError("could not load border csv.");
        return;
      }

      string[] lines = bordersCSV.ToString().Split('\n');

      List<Vector2> borderPoints = new List<Vector2>();
      int currentAsterismIndex = -1;

      for(int i=0; i<lines.Length; i++) {
        string line = lines[i];
        string[] fields = line.Split(',');

        if ( fields.Length < 4 ) {
          Debug.Log("incompleteLine: " + line);
          continue;
        }

        string abreviation = fields[2];
        string constellationName = m_abrevLookup[abreviation];

        if(currentAsterismIndex == -1 || constellationName != AsterismData[currentAsterismIndex].name) {
          // Store vertecies from last asterism parsed.
          if ( currentAsterismIndex != -1 && borderPoints != null) {
            Asterism currentAsterism = AsterismData[currentAsterismIndex];
            Vector2[] border = borderPoints.ToArray();

            Vector3[] lineVerticies = new Vector3[border.Length + 1];
            coordsToThreeSpace(border).CopyTo(lineVerticies, 1);
            lineVerticies[0] = lineVerticies[lineVerticies.Length-1];

            if ( m_borderLineMaterial == null ) {
              m_borderLineMaterial = Resources.Load("Lines/LineMat") as Material;
            }

            LineObject borderLine = LineObject.LineFactory(lineVerticies, 5.0f, AsterismDrawer.LeftCamera, m_borderLineMaterial, true);
            borderLine.transform.parent = borderRoot.transform;
            currentAsterism.borderArt = borderLine;

            Mesh newMesh = meshFromBorders(border);
            GameObject prefab = Resources.Load("baseObj") as GameObject;
            GameObject go = GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.GetComponent<MeshFilter>().mesh = newMesh;
            go.transform.GetComponent<MeshCollider>().sharedMesh = newMesh;
            go.transform.GetComponent<AsterismReference>().Asterism = currentAsterism;
            go.transform.GetComponent<AsterismReference>().index = currentAsterismIndex;
            go.name = "Collision_"+currentAsterism.name;
            go.transform.parent = collisionRoot.transform;
            AsterismData[currentAsterismIndex] = currentAsterism;

          }

          // Grab the next asterism being parsed.
          int indexOfAsterism = asterismIndexFromName(constellationName);
          if ( indexOfAsterism == -1 ) {
            currentAsterismIndex = -1;
//            Debug.LogError("Parsing border of unknown constellation: " + constellationName);
            continue;
          }
          currentAsterismIndex = indexOfAsterism;
          borderPoints = new List<Vector2>();
        }

        float xComponent = 0.0f;
        float yComponent = 0.0f;

        bool xParsed = float.TryParse(fields[0], out xComponent);
        bool yParsed = float.TryParse(fields[1], out yComponent);

        if(!xParsed || !yParsed) {
          Debug.LogError("Could not parse x and y coords");
          continue;
        }

        Vector2 newCoord = new Vector2(xComponent, yComponent);

        borderPoints.Add(newCoord);

        // Make sure to store the last point.
        if ( i == lines.Length - 1 ) {
          if ( i < AsterismData.Length ) {
            Asterism currentAsterism = AsterismData[i];
            currentAsterism.borders = borderPoints.ToArray();
            AsterismData[i] = currentAsterism;
          }
        }
      }
    }

    private static Vector2[] avoidMeridian(Vector2[] borders) {
      for(int i = 1; i < borders.Length; i++) {
        if ( borders[i].x - borders[i-1].x > 12.0f ) {
          borders[i].x -= 24.0f;
        }
        else if( borders[i].x - borders[i-1].x < -12.0f ) {
          borders[i].x += 24.0f;
        }

      }

      return borders;
    }

    private static bool ClockWise(Vector2[] coords)
    {
      int i,j,k;
      int count = 0;
      double z;

      if (coords.Length < 3)
        return(false);

      for (i=0;i<coords.Length;i++) {
        j = (i + 1) % coords.Length;
        k = (i + 2) % coords.Length;
        z  = (coords[j].x - coords[i].x) * (coords[k].y - coords[j].y);
        z -= (coords[j].y - coords[i].y) * (coords[k].x - coords[j].x);
        if (z < 0)
          count--;
        else if (z > 0)
          count++;
      }
      if (count > 0)
        return false;
      else if (count < 0)
        return true;
      else
        return false;
    }

    private static Mesh meshFromBorders(Vector2[] borders) {

      borders = avoidMeridian(borders);
      Triangulator tri = new Triangulator(borders);
      int[] tris = tri.Triangulate();

      Vector3[] verts = coordsToThreeSpace(borders);
      Vector3[] norms = new Vector3[borders.Length];

      for(int i=0;i<borders.Length;i++) {
        norms[i] = Vector3.forward;
      }

      Mesh mesh = new Mesh();

      mesh.vertices = verts;
      mesh.triangles = tris;
      mesh.normals = norms;
      mesh.RecalculateBounds();
      //mesh.RecalculateNormals();
      return mesh;
    }

    private static Vector3[] coordsToThreeSpace(Vector2[] coords) {
      if ( coords == null ) {
        Debug.LogError("coords is null");
        return new Vector3[0];
      }
      Vector3[] threeSpaceCoords = new Vector3[coords.Length];

      for(int i=0;i<coords.Length;i++) {
        threeSpaceCoords[coords.Length - 1 - i] = positionFromCoords(coords[i].x, coords[i].y, 20.0f);
      }

      return threeSpaceCoords;
    }

    // Using normalzied RA, DEC, and DIST, calculate the position of the object in 3D space relative to the origin.
    private static Vector3 positionFromCoords(float ra, float dec, float dist) {
      float normRa = Mathf.Clamp01(ra / 24.0f);
      float normDec = Mathf.Clamp01((dec - (-90.0f)) / (90.0f - (-90.0f)));
      float yRoation = normRa * (360.0f) * -1.0f;
      float xRotation = (normDec - 0.5f) * 180.0f * -1.0f;

      Quaternion direction = Quaternion.Euler (xRotation, yRoation, 0.0f);
      //Quaternion adjustment = Quaternion.Euler (180.0f, 0.0f, 0.0f);
      Vector3 directionVector = direction * Vector3.forward;
      Vector3 position = directionVector * dist;
      return position;
    }
  }
}