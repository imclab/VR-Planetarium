using UnityEngine;
using System.Collections;

namespace LMLineDrawing {
  /// <summary>
  /// Struct storing the definition of a line segment.
  /// </summary>
  public struct LineSegment {
    private Vector3 m_p1;
    private Vector3 m_p2; 
    private float m_startWidth;
    private float m_endWidth;
    private Vector3 m_startDirection;
    private Vector3 m_endDirection;
    
    /// <summary>
    /// Gets or sets the p1.
    /// </summary>
    public Vector3 P1 { 
      get { return m_p1; } 
      set { m_p1 = value; }
    }
    
    /// <summary>
    /// Gets or sets the p2.
    /// </summary>
    public Vector3 P2 { 
      get { return m_p2; }
      set { m_p2 = value; }
    }
    
    /// <summary>
    /// Gets or sets the width at the start of the segment.
    /// </summary>
    public float StartWidth { 
      get { return m_startWidth; }
      set { 
        if ( m_startWidth < 0 ) { Debug.LogWarning("Attempting to set segment width to: " + value + ". Width must be non-negative."); }
        m_startWidth = Mathf.Max(0, value);
      }
    }

    /// <summary>
    /// Gets or sets the width at the end of the segment
    /// </summary>
    public float EndWidth { 
      get { return m_endWidth; }
      set { 
        if ( m_endWidth < 0 ) { Debug.LogWarning("Attempting to set segment width to: " + value + ". Width must be non-negative."); }
        m_endWidth = Mathf.Max(0, value);
      }
    }
    
    /// <summary>
    /// Gets or sets the start direction.
    /// </summary>
    public Vector3 StartDirection { 
      get { return m_startDirection; }
      set { 
        if (value.magnitude <= 0) { throw new System.ArgumentOutOfRangeException("Direction must be a vector with a positive magnitude."); }
        m_startDirection = value.normalized; 
      }
    }
    
    /// <summary>
    /// Gets or sets the end direction.
    /// </summary>
    public Vector3 EndDirection { 
      get { return m_endDirection; }
      set { 
        if (value.magnitude <= 0) { throw new System.ArgumentOutOfRangeException("Line must be a vector with a positive magnitude."); }
        m_endDirection = value.normalized; 
      }
    }
    
    /// <summary>
    /// Gets the center.
    /// </summary>
    public Vector3 Center {
      get { return Vector3.Lerp(P1, P2, 0.5f); }
    }
    
    /// <summary>
    /// Gets a value indicating whether this <see cref="LMLineDrawing.LineSegment"/> is valid.
    /// </summary>
    /// <value><c>true</c> if the struct describes a valid line; otherwise, <c>false</c>.</value>
    public bool Valid {
      get { 
        if ( m_startDirection.magnitude <= 0 ) { return false; }
        if ( m_endDirection.magnitude <= 0 ) { return false; }
        if ( m_startWidth < 0 ) { return false; }
        if ( m_endWidth < 0 ) { return false; }
        
        return true;
      }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LMLineDrawing.LineSegment"/> struct.
    /// </summary>
    /// <param name="p1">The start of the line segment.</param>
    /// <param name="p2">The end of the line segment.</param>
    /// <param name="width">The width of the line in Unity units.</param>
    public LineSegment (Vector3 p1, Vector3 p2, float startWidth, float endWidth ) : this(p1,p2,startWidth, endWidth,(p2 - p1).normalized,(p2 - p1).normalized) {}
    
    /// <summary>
    /// Initializes a new instance of the <see cref="LMLineDrawing.LineSegment"/> struct.
    /// </summary>
    /// <param name="p1">The start of the line segment.</param>
    /// <param name="p2">The end of the line segment.</param>
    /// <param name="width">The width of the line in Unity units.</param>
    /// <param name="startDirection">The "direction" of the end-cap for the start of the line, used to smoothly connect this line segment to others.</param>
    /// <param name="endDirection">The "direction" of the end-cap for the start of the line, used to smoothly connect this line segment to others.</param>
    public LineSegment (Vector3 p1, Vector3 p2, float startWidth, float endWidth, Vector3 startDirection, Vector3 endDirection) {
      m_p1 = p1; 
      m_p2 = p2;
      m_startWidth = startWidth;
      m_endWidth = endWidth;
      m_startDirection = startDirection;
      m_endDirection = endDirection;
    }
  }
}