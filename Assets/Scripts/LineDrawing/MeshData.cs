#define VERBOSE_DEBUG_MESSAGES
using UnityEngine;
using System.Collections;

namespace  LMLineDrawing {
  /// <summary>
  /// Struct storing the component data needed to create a Unity3D mesh.
  /// </summary>
  public struct MeshData {
    private Vector3[] m_verts;
    private int[] m_tris;
    private Vector3[] m_normals;
    private Vector2[] m_uvs;
    
    /// <summary>
    /// Gets or sets the list of verticies. The array must be at least 3 verts, or empty.
    /// </summary>
    public Vector3[] Verts { 
      get {
        return m_verts;
      }
      set {
        if ( value.Length < 3 && value.Length != 0 ) { throw new System.ArgumentOutOfRangeException("Verts array must be at least 3 verts, or empty."); }
        m_verts = value;
      }
    }
    
    /// <summary>
    /// Gets or sets the tris.
    /// </summary>
    public int[] Tris { 
      get {
        return m_tris;
      }
      set {
        if ( value.Length < 3 && value.Length != 0 ) { throw new System.ArgumentOutOfRangeException("Tris array must be at least length 3, or empty."); }
        if ( value.Length % 3 != 0 ) { throw new System.ArgumentOutOfRangeException("Length of the tri array must be a multiple of 3 or empty."); }
        m_tris = value;
      }
    }
    
    /// <summary>
    /// Gets or sets the normals.
    /// </summary>
    public Vector3[] Normals { 
      get {
        return m_normals;
      }
      set {
        #if VERBOSE_DEBUG_MESSAGES
        if ( value.Length != Verts.Length ) {
          Debug.LogWarning("Warning: Setting Normals array to a list with a length different than the associated vertex array. This will invalidate the mesh data.");
        }
        #endif
        m_normals = value;
      }
    }
    
    /// <summary>
    /// Gets or sets the U vs.
    /// </summary>
    public Vector2[] UVs { 
      get {
        return m_uvs;
      }
      set {
        #if VERBOSE_DEBUG_MESSAGES
        if ( value.Length != Verts.Length ) {
          Debug.LogWarning("Warning: Setting UV array to a list with a length different than the associated vertex array. This will invalidate the mesh data.");
        }
        #endif
        m_uvs = value;
      }
    }
    
    /// <summary>
    /// Gets a value indicating whether this instance has verts.
    /// </summary>
    public bool HasVerts { get { return (m_verts.Length > 0); } }
    /// <summary>
    /// Gets a value indicating whether this instance has tris.
    /// </summary>
    public bool HasTris { get { return (m_tris.Length > 0); } }
    /// <summary>
    /// Gets a value indicating whether this instance has normals.
    /// </summary>
    public bool HasNormals { get { return (m_normals.Length > 0); } }
    /// <summary>
    /// Gets a value indicating whether this instance has U vs.
    /// </summary>
    public bool HasUVs { get { return (m_uvs.Length > 0); } }
    
    /// <summary>
    /// Clear this instance.
    /// </summary>
    public void Clear() {
      Verts = new Vector3[0];
      Tris = new int[0];
      Normals = new Vector3[0];
      UVs = new Vector2[0];
    }
    
    /// <summary>
    /// Gets a value indicating whether this <see cref="LMLineDrawing.MeshData"/> is valid.
    /// The MeshData is valid if a useable Unity3D Mesh object can be reasonably generated from the data.
    /// </summary>
    /// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
    public bool Valid {
      get {
        if ( m_verts.Length < 3 ) { return false; }
        if ( m_tris.Length < 3 ) { return false; }
        if ( m_tris.Length % 3 != 0 ) { return false; }
        if ( m_normals.Length > 0 ) { 
          if ( m_normals.Length != m_verts.Length ) { return false; } 
        }
        if ( m_uvs.Length > 0 ) { 
          if ( m_uvs.Length != m_verts.Length ) { return false; }
        }
        
        return true;
      }
    }
    
    /// <summary>
    /// Generates a Unity3D Mesh object from the instance's mesh data.
    /// </summary>
    public Mesh GenerateMeshFromData() {
      if ( !HasVerts ) { throw new InvalidDataException("MeshData must have verts to generate a mesh."); }
      if (!HasTris) { throw new InvalidDataException("MeshData must have tris to generate a mesh."); }
      if ( !Valid ) { throw new InvalidDataException("MehsData must be valid to generate a mesh."); }
      Mesh mesh = new Mesh ();
      mesh.vertices = Verts;
      mesh.triangles = Tris;
      
      if (HasNormals) {
        mesh.normals = Normals;
      } else {
        mesh.RecalculateNormals ();
      }
      
      if (HasUVs) {
        mesh.uv = UVs;
      } 
      
      mesh.RecalculateBounds ();
      
      return mesh;
    }
  }
}
