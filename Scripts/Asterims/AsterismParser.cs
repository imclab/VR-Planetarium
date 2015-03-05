using UnityEngine;
using Vectrosity;
using System.Collections;
using System.Collections.Generic;

namespace Asterisms {
	public struct Asterism {
		public string name; // Te
		public uint[] HD_ids; // The HD catalog ID of the stars ordered in vertex pairs [a1, a2, b1, b2, c1, c2, ... n1, n2]
		public VectorLine lineArt;
	}

	public class AsterismParser {
		private static HashSet<uint> m_asterismStars = new HashSet<uint>(); //Lookup table so we don't have to search all the asterism stars to find out if a star is in an asterism
		public static Asterism[] AsterismData;
   
		public static HashSet<uint> StarsInAsterisms { get { return m_asterismStars; } }

		public static void LoadAsterismData() {
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

			return;
		
	  }
  }
}