/// # Enums.cs
// no base class in this file

/// The possible types of Trungle, either Euclidean
/// hyperbolic, or spherical.
public enum TriangleType
{
    Euclidean,
    Hyperbolic,
    Spherical
}

/// The two rotation directions, either Clockwise
/// or Counterclockwise.
public enum RotationDirection
{
    Clockwise,
    Counterclockwise,
}

public enum ReflectionMode
{
    Regular,
    Branching
}