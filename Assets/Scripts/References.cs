using UnityEngine;
using MathNet.Numerics;

/// # References.cs

/// This is a static class purely used to coordinate objects across scripts. This way,
/// each script does not need to have public members on it only to access other classes.
public static class References
{
    /// ## Properties
    
    /// The number of vertices for lines and arcs to generate. Note that arcs are NOT
    /// guaranteed to have this number of vertices; the circle from which the arc is cut
    /// always has this many vertices, but then the arc is only a subsection of that circle.
    public const int NUM_VERTEX = 150;

    /// The Vector3 used in the Mobius rotation function. That function simply reassigns the values
    /// of this variable instead of allocating space for a new Vector3 on every call.
    public static Vector3 p;
    
    /// The Complex32 used in the Mobius rotation function. That function simply reassigns the values
    /// of this variable instead of allocating space for a new Complex32 on every call.
    public static Complex32 z;

    /// The current type of the trungles, either Euclidean, hyperbolic, or spherical.
    public static TriangleType CurrentType;

    /// The only Trungle Manager on the scene. 
    public static TrungleManagerBehavior TrungleManager;

    /// The only Increment Steps on the scene, responsible for handling when the + or - 
    /// buttons are clicked
    public static IncrementSteps StepsManager;

    /// A point at infinity, defined as a Vector3 with every value float.positiveInfinity.
    public static Vector3 Infinity;

    public static ReflectionMode CurrentMode;

    public static CameraRotator cameraRotate;
}
