using UnityEngine;
using System;
using System.Collections;

namespace LMLineDrawing {
  /// <summary>
  /// Line generators. 
  /// A set of helpers for generating line geometry.
  /// </summary>
  public static class LineGenerators {
    private const float WIDTH_CONSTANT = 0.001f;

    /// <summary>
    /// Generates the mesh data for a single line segent via the given LineSegment struct.
    /// </summary>
    /// <param name="meshData">Reference to the MeshData object in which to store the generated data.</param>
    /// <param name="segmentData">The line segment data from which to generate the mesh data.</param>
    /// <param name="normal">The normal direction of the mesh to be generated</param> 
    /// <param name="append">If set to <c>true</c> the generated mesh data will be appended to the mesh data reference. If set to <c>false<c> the data will be overritten. Defaults to <c>false</c></param>
    public static void GenerateMeshDataFromSegmentData(ref MeshData meshData, LineSegment segmentData, Vector3 worldOffset, bool append = false) {
      GenerateMeshDataFromSegmentData ( ref meshData, segmentData, worldOffset, 0.0f, 1.0f, append);
    }

    /// <summary>
    /// Generates the mesh data for a single line segent via the given LineSegment struct.
    /// </summary>
    /// <param name="meshData">Reference to the MeshData object in which to store the generated data.</param>
    /// <param name="segmentData">The line segment data from which to generate the mesh data.</param>
    /// <param name="normal">The normal direction of the mesh to be generated</param> 
    /// <param name="uCoordinateStart">The U coordinate of the mesh UV to be assinged to the starting point end-cap verticies.</param>
    /// <param name="uCoordinateEnd">The U coordinate of the mesh UV to be assinged to the ending point end-cap verticies.</param>
    /// <param name="append">If set to <c>true</c> the generated mesh data will be appended to the mesh data reference. If set to <c>false<c> the data will be overritten. Defaults to <c>false</c></param>
    public static void GenerateMeshDataFromSegmentData(ref MeshData meshData, LineSegment segmentData, Vector3 worldOffset, float uCoordinateStart, float uCoordinateEnd, bool append = false) {
      if (!segmentData.Valid) { 
        Debug.Log ("Segment: ");
        Debug.Log ("p1: " + segmentData.P1);
        Debug.Log ("p2: " + segmentData.P2);
        Debug.Log ("startD: " + segmentData.StartDirection);
        Debug.Log ("endD: " + segmentData.EndDirection);
        Debug.Log ("startWidth: " + segmentData.StartWidth);
        Debug.Log ("endWidth: " + segmentData.EndWidth);
        throw new System.ArgumentException("Segment data must be valid"); 
      }

      Vector3[] verts;
      int[] tris;
      Vector3[] normals;
      Vector2[] uvs;

      if ( !append ) { meshData.Clear(); }

      int vertsStartIndex = meshData.Verts.Length;
      int trisStartIndex = meshData.Tris.Length;
      int normalsStartIndex = meshData.Normals.Length;
      int uvStartIndex = meshData.UVs.Length;

      verts = new Vector3[meshData.Verts.Length + 8];
      tris = new int[meshData.Tris.Length + 24];
      normals = new Vector3[meshData.Normals.Length + 8];
      uvs = new Vector2[ meshData.UVs.Length + 8];  

      Array.Copy (meshData.Verts, verts, meshData.Verts.Length);
      Array.Copy (meshData.Tris, tris, meshData.Tris.Length);
      Array.Copy (meshData.Normals, normals, meshData.Normals.Length);
      Array.Copy (meshData.UVs, uvs, meshData.UVs.Length);

      const int vertexCountPerRing = 4;
      const int rings = 2;

      Vector3[] tubeVerts = QuadTubeVerts (new Vector3[] {segmentData.P1, segmentData.P2}, new float[] { segmentData.StartWidth, segmentData.EndWidth }, vertexCountPerRing, worldOffset);

      Array.Copy (tubeVerts, 0, verts, vertsStartIndex, tubeVerts.Length);

      //int[] tris = new int[vertexCountPerRing * 6 * (rings-1)];
      //Vector2[] uvs = new Vector2[verts.Length];
      
      for ( int ringIndex=0;ringIndex<rings;ringIndex++ ) {
        float vComponent = ringIndex / (float)rings;

        for ( int faceIndex=0;faceIndex<vertexCountPerRing;faceIndex++ ) {

          int ringBase = ringIndex * vertexCountPerRing;
          int vertIndex = (ringIndex * vertexCountPerRing) + faceIndex;
          
          if ( ringIndex < rings-1 ) { // No more tris to make after the second to last ring
            int triIndex = (ringIndex * vertexCountPerRing * 6) + (faceIndex * 6);
           
            // bottom-left, top-left, top-right
            tris[trisStartIndex+triIndex]   = vertsStartIndex + ringBase + faceIndex;
            tris[trisStartIndex+triIndex+1] = vertsStartIndex + ringBase + vertexCountPerRing + (faceIndex % vertexCountPerRing);
            tris[trisStartIndex+triIndex+2] = vertsStartIndex + ringBase + vertexCountPerRing + ((faceIndex +  + 1) % vertexCountPerRing);
            
            // bottom-right, bottom-left, top-right
            tris[trisStartIndex+triIndex+3] = vertsStartIndex + ringBase + ((faceIndex + 1) % vertexCountPerRing);
            tris[trisStartIndex+triIndex+4] = vertsStartIndex + ringBase + faceIndex;
            tris[trisStartIndex+triIndex+5] = vertsStartIndex + ringBase + vertexCountPerRing + ((faceIndex + 1) % vertexCountPerRing);
          }
          
          float uComponent = faceIndex / (float)(vertexCountPerRing - 1);
          
          uvs[uvStartIndex+vertIndex] = new Vector2(uComponent, vComponent);

        }
      }

      meshData.Verts = verts;
      meshData.Tris = tris;
      meshData.Normals = normals;
      meshData.UVs = uvs;
    }

    public static void GenerateMeshDataFromPoints(ref MeshData meshData, Vector3[] points, Vector3 camera, Vector3 worldCentroid, float width, bool continuous = false, bool append = false) {
      if ( points.Length < 2 ) { throw new System.ArgumentException("Points array must contain at least two points."); }

      if (!append) {
        meshData.Clear ();
      }

      Vector3 lastEndDirection = (points[1] - points[0]).normalized;
      for(int i=1; i < points.Length; i++) {
        Vector3 p1 = points[i-1];
        Vector3 p2 = points[i];

        if ( p1 == p2 ) { continue; }

        if ( lastEndDirection.magnitude == 0.0f ) { 
          lastEndDirection = (p2 - p1).normalized;
        }

        float p1Dist = (camera - p1).magnitude;
        float p2Dist = (camera - p2).magnitude;

        LineSegment segment;
        if ( continuous ) {
          Vector3 endDirection = (p2 - p1).normalized;
          if ( i + 1 < points.Length ) { 
            Vector3 nextDirection = (points[i+1] - p2).normalized;
            endDirection = Vector3.Lerp(endDirection, nextDirection, 0.5f);
          }
          segment = new LineSegment(p1, p2, width * p1Dist, width * p2Dist, lastEndDirection, endDirection);
        }
        else {
          segment = new LineSegment(p1, p2, width * p1Dist, width * p2Dist);
          i++;
        }

        GenerateMeshDataFromSegmentData(ref meshData, segment, worldCentroid, true);
      }
    }

    /// <summary>
    /// Recalculates the facing of a given quad. Assumes that the two long edges of the quad are parrallel.
    /// </summary>
    /// <param name="meshData">Mesh data.</param>
    /// <param name="verts">Verts.</param>
    /// <param name="width">Width.</param>
    /// <param name="normal">Normal.</param>
    /// <param name="append">If set to <c>true</c> append.</param>
    public static void RecalculateQuadFacing(ref MeshData meshData, Vector3[] verts, Vector3 camera, float width, bool append = false) {
      if (verts.Length != 4) {
        throw new System.ArgumentOutOfRangeException ("Verts array must be of length 4.");
      }

      if (!append) {
        meshData.Clear ();
      }

      Vector3 surfaceNormal = Vector3.Cross (verts[3] - verts[0], verts[1] - verts[0]).normalized;

      Vector3 startCapVector = verts [3] - verts [0];
      Vector3 endCapVector = verts [2] - verts [1];

      Vector3 startDirection = Vector3.Cross (surfaceNormal, startCapVector).normalized;
      Vector3 endDirection = Vector3.Cross (surfaceNormal, endCapVector).normalized;
      Vector3 p1 = Vector3.Lerp (verts [2], verts [1], 0.5f);
      Vector3 p2 = Vector3.Lerp (verts [3], verts [0], 0.5f);
      float p1Dist = (camera - p1).magnitude;
      float p2Dist = (camera - p2).magnitude;
      LineSegment segment = new LineSegment (p1, p2, width * p1Dist, width * p2Dist, startDirection, endDirection);
      GenerateMeshDataFromSegmentData (ref meshData, segment, Vector3.zero, append);
    }

    private const int MIN_LONGITUDINAL_RESOLUTION = 2;
    
    public static Vector3[] VertexRing(Vector3 centerPosition, float width, Vector3 ringNormal, int vertexCount, Vector3 worldOffset) {
      width *= WIDTH_CONSTANT;
      float halfWidth = width/2.0f;
      Vector3[] vertexRing = new Vector3[vertexCount];
      Vector3 basisX = new Vector3();
      Vector3 basisZ = new Vector3();
      
      // Create orthonormal bases vectors from normal vector. Does not guarntee direction of bases.
      Vector3.OrthoNormalize(ref ringNormal, ref basisX, ref basisZ);
      
      float angleStep = (2*Mathf.PI) / (float)(vertexCount-1);
      for(int i=0; i<vertexCount; i++) {
        float angle = angleStep * i;
        float xComponent = Mathf.Cos(angle);
        float zComponent = Mathf.Sin(angle);
        vertexRing[i] = (basisX * xComponent * halfWidth) + (basisZ * zComponent * halfWidth) + centerPosition - worldOffset;
      }
      
      return vertexRing;
      
    }
    
    public static Vector3[] QuadTubeVerts(Vector3[] centerSpine, float[] widths, int vertexCountPerRing, Vector3 worldOffset) {
        if (centerSpine.Length != widths.Length) {
          throw new System.ArgumentException("center spine and widths arrays must be the same length.");
      }

      Vector3[] quadTubeVerts = new Vector3[vertexCountPerRing * centerSpine.Length]; 
      Vector3 direction = Vector3.zero;
      //Calculate vertexes for 
      for ( int i=0; i<centerSpine.Length; i++ ) { 
        if ( i < centerSpine.Length - 1 ) {
          direction = (centerSpine[i] - centerSpine[i+1]).normalized; 
        }

        Vector3[] newVerts = LineGenerators.VertexRing(centerSpine[i], widths[i], direction, vertexCountPerRing, worldOffset);
        System.Array.Copy(newVerts, 0, quadTubeVerts, vertexCountPerRing * i, newVerts.Length);
      }
      
      return quadTubeVerts;
    }

  }
}