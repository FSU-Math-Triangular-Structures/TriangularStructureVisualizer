using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

/// # TrungleUtils.cs

/// A collection of useful static utility functions, used to clean up the 
/// Trungle Manager class. Contains functions to reflect and rotate Trungle sides
/// and calculate lines, arcs, and prime points.
public static class TrungleUtils
{
    /// ## Methods
    
    /// **Overview** Reflect each point in the array across the x-axis.
    /// 
    /// **Params**
    /// - pointsIn: array of points to reflect
    /// 
    /// **Returns** A new array that is each original point reflected.
    public static Vector3[] Reflect(Vector3[] pointsIn)
    {
        Vector3[] newPoints = new Vector3[pointsIn.Length];
        for (int i = 0; i < pointsIn.Length; i++)
        {
            newPoints[i] = pointsIn[i].Reflect();
        }
        return newPoints;
    }

    /// **Overview** Rotate each point in the array using the given matrix.
    /// 
    /// **Params**
    /// - pointsIn: array of points to rotate
    /// - m: the rotation matrix to use
    /// 
    /// **Returns** A new array that is each original point rotated.
    public static Vector3[] Rotate(Vector3[] pointsIn, Matrix m)
    {
        Vector3[] newPoints = new Vector3[pointsIn.Length];

        for (int i = 0; i < newPoints.Length; i++)
        {
            newPoints[i] = pointsIn[i].ApplyMatrix(m);
        }
        return newPoints;
    }

    /// **Overview** Get the straight line between the two points.
    /// 
    /// **Params**
    /// - start: the starting point of the line
    /// - end: the ending point of the line
    /// 
    /// **Returns** A list of size NUM_VERTEX of points on the line.
    public static List<Vector3> LineBetween(Vector3 start, Vector3 end)
    {
        List<Vector3> points = new List<Vector3>();

        float slope = (end.y - start.y) / (end.x - start.x);
        float delta_x = (end.x - start.x) / References.NUM_VERTEX;

        for (int i = 0; i < References.NUM_VERTEX; i++)
        {
            float x = start.x + i * delta_x;
            float y = slope * (x - start.x) + start.y;

            points.Add(new Vector3(x, y));
        }

        return points;
    }

    /// **Overview** Get the arc between the two points of the desired circle.
    /// 
    /// **Params**
    /// - start: start point of the arc
    /// - end: end point of the arc
    /// - center: center of the desired arc
    /// - radius: radius of the desired arc
    /// - alpha: current alpha angle of the Trungle
    /// 
    /// **Returns** A list of size \\( \leq \\) NUM_VERTEX of points on the arc.
    public static List<Vector3> ArcBetween(Vector3 start, Vector3 end, Vector3 center, float radius, float alpha)
    {
        // switch these values if the end is less than the start
        if (end.x < start.x)
        {
            Vector3 temp;
            temp = end;
            end = start;
            start = temp;
        }

        float angle = 2 * Mathf.PI / References.NUM_VERTEX;
        List<Vector3> points = new List<Vector3>();

        // we generate NUM_VERTEX points along the circle
        for (float i = 0; i < References.NUM_VERTEX; i++)
        {
            // create rotationMatrix
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0f, 0f),
                                                     new Vector4(-1f * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0f, 0f),
                                                     new Vector4(0f, 0f, 1f, 0f),
                                                     new Vector4(0f, 0f, 0f, 1f));

            // this is just (0, radius)
            Vector3 initialRelativePosition = new Vector3(0f, radius, 0f);

            // rotate the point (0, radius) and then add it to the center, resulting in a
            // point along the outside of the circle
            Vector3 point = center + rotationMatrix.MultiplyPoint(initialRelativePosition);

            // this arc is for hyperbolic
            if (References.CurrentType == TriangleType.Hyperbolic)
            {
                // just make a rectangle and check that the point is in it, basically
                if (point.y <= start.y && point.y >= end.y && point.x <= center.x)
                {
                    points.Add(point);
                }
            }
            // this is a spherical arc
            else
            {
                // we only check the y of the tan(alpha) if alpha is acute
                if (alpha < 90 * Mathf.Deg2Rad)
                {
                    if (point.y <= (Mathf.Tan(alpha) * point.x) && point.x >= start.x && point.y >= 0)
                    {
                        points.Add(point);
                    }
                }
                // otherwise, we care not about y versus tan(alpha)
                else
                {
                    if (point.x >= start.x && point.y >= 0)
                    {
                        points.Add(point);
                    }
                }
            }
        }

        /* WE CHECK FOR JUMPS */
        // monke check -- have we rejected modernity?
        int spotToCut = -1;

        for (int i = 0; i < (points.Count - 1); i++)
        {
            // this means that there is a big jump in x-values here,
            // so prepare to cut
            if (Mathf.Abs(points[i].x - points[i + 1].x) >= 0.9)
                spotToCut = i;
        }

        // we found a jump
        if (spotToCut != -1)
        {
            // cut the range and add it back to the end
            var cutPieces = points.GetRange(0, spotToCut + 1);
            points.RemoveRange(0, spotToCut + 1);
            points.InsertRange(points.Count, cutPieces);
        }

        return points;
    }

    /// **Overview** Calculate the B' point for the non-Euclidean case.
    /// 
    /// **Params**
    /// - center: center of the circle used to generate the first arc
    /// - radius: radius of the circle used to generate the first arc
    /// - betaPoint: beta point of the first Trungle
    /// 
    /// **Returns** The new B' point
    public static Vector3 CalculateBPrime(Vector3 center, float radius, Vector3 betaPoint)
    {
        // set some easier names for the parameters
        float h = center.x;
        float k = center.y;
        float r = radius;

        // set up for the quadratic
        float A = 1;
        float B = -2 * h;
        float C = ((h * h) - (r * r) + (k * k));

        Vector3 p = new Vector3();

        // assume the - solution is the B'
        p.x = (-B - Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);

        // if we actually solved for B again, use the + solution
        if (Mathf.Abs(p.x - betaPoint.x) <= 0.001)
        {
            p.x = (-B + Mathf.Sqrt(Mathf.Pow(B, 2) - 4 * A * C)) / (2 * A);
        }

        return p;
    }

    /// **Overview** Calculate the C' point for the non-Euclidean case.
    /// 
    /// **Params**
    /// - vertex: gamma point of the first Trungle
    /// - center: center of the circle used to generate the first arc
    /// - radius: radius of the circle used to generate the first arc
    /// 
    /// **Returns** The new C' point
    public static Vector3 CalculateCPrime(Vector3 vertex, Vector3 center, float radius)
    {
        // establish some easier names for the parameter values
        float r = radius;
        float a = vertex.x;
        float b = vertex.y;
        float h = center.x;
        float k = center.y;

        // lots of math
        float A = Mathf.Pow(a, 2) + Mathf.Pow(b, 2);
        float B = -Mathf.Pow(h, 2) - Mathf.Pow(k, 2) + Mathf.Pow(r, 2);
        float Casella = 4f * Mathf.Pow(a, 2);
        float D = Mathf.Pow(a, 2) * Mathf.Pow((-2f * a * h - 2f * b * k), 2);
        float E = 2f * Mathf.Pow(a, 2) * h + 2f * a * b * k;
        float F = Mathf.Sqrt((Casella * A * B) + D);

        // assume this is the correct version of C'
        float x;
        x = (F + E) / (2f * A);

        // if we solved for the C point, use the other solution
        if (Mathf.Abs(x - vertex.x) <= 0.01)
        {
            x = (-F + E) / (2f * A);
        }

        // calculate the y (always the same) and return the point
        float y = (b / a) * x;
        return new Vector3(x, y);
    }

    /// **Overview** Determines what direction to rotate in based on the given points. Essentially achieves this by checking the current 
    /// triangle and its parents points to each other and determining if the current is above, below, left, or right of the parent.
    /// 
    /// **Params**
	/// - reflectPoint - point on the current and parent triangle that is not being rotated about
	/// - rotatePoint - point on the current and parent triangle being rotated about
	/// - lastPoint - point on the child that is not shared with the parent
	/// - parentOther - point on the parent that is not shared with the child
    ///
    /// **Returns** direction to rotate the current object in
    public static RotationDirection GetDirection(Vector3 reflectPoint, Vector3 rotatePoint, Vector3 lastPoint, Vector3 parentOther)
    {
        RotationDirection dir = RotationDirection.Clockwise;

        // right triangle exceptions. Checks if the floats are basically the same
        if (Mathf.Abs(reflectPoint.x - rotatePoint.x) < 0.001 && Mathf.Abs(parentOther.y - lastPoint.y) < 0.001)
        {
            if (reflectPoint.y < rotatePoint.y && parentOther.x < lastPoint.x)
            {
                dir = RotationDirection.Counterclockwise;
            }
            else if (reflectPoint.y > rotatePoint.y && parentOther.x < lastPoint.x)
            {
                dir = RotationDirection.Clockwise;
            }
            else if (reflectPoint.y < rotatePoint.y && parentOther.x > lastPoint.x)
            {
                dir = RotationDirection.Clockwise;
            }
            else if (reflectPoint.y > rotatePoint.y && parentOther.x > lastPoint.x)
            {
                dir = RotationDirection.Counterclockwise;
            }
        }
        else
        {
            if (reflectPoint.x < rotatePoint.x && parentOther.y < lastPoint.y)
            {
                dir = RotationDirection.Clockwise;
            }
            else if (reflectPoint.x > rotatePoint.x && parentOther.y < lastPoint.y)
            {
                dir = RotationDirection.Counterclockwise;
            }
            else if (reflectPoint.x < rotatePoint.x && parentOther.y > lastPoint.y)
            {
                dir = RotationDirection.Counterclockwise;
            }
            else if (reflectPoint.x > rotatePoint.x && parentOther.y > lastPoint.y)
            {
                dir = RotationDirection.Clockwise;
            }
        }

        return dir;
    }
}
