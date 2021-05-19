using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

/// # Vector3Extensions.cs

/// Adds several useful functions that can be directly called on a Vector3 instance.
/// For example:
/// ```csharp
/// Vector3 xPoint = new Vector3(1, 0, 0);
/// Vector3 reflected = xPoint.Reflect();
/// ```
public static class Vector3Extensions
{
    /// ## Methods
    
    /// **Overview** Apply the given matrix to a copy of this Vector3
    /// 
    /// **Params**
    /// - v: calling Vector3 object
    /// - m: matrix to apply
    /// 
    /// **Returns** A new Vector3 which is the given point multiplied
    /// by the matrix.
    public static Vector3 ApplyMatrix(this Vector3 v, Matrix m)
    {
        References.p.x = v.x;
        References.p.y = v.y;
        // NonMobius rotation
        if (!m.IsMobius)
        {
            References.p.x -= m.CoR1.x;
            References.p.y -= m.CoR1.y;

            // perform matrix multiplication
            var rotatedX = References.p.x * m.NonMobius[0, 0] + References.p.y * m.NonMobius[0, 1];
            var rotatedY = References.p.x * m.NonMobius[1, 0] + References.p.y * m.NonMobius[1, 1];

            // set point's coordinates
            References.p.x = rotatedX;
            References.p.y = rotatedY;

            // move centerOfRotation back to original position
            References.p.x += m.CoR1.x;
            References.p.y += m.CoR1.y;

            return References.p;
        }
        // Otherwise, Mobius
        else
        {
            if (References.p.IsInfinite())
            {
                References.z = m.Mobius[0, 0] / m.Mobius[1, 0];
                References.p.x = References.z.Real;
                References.p.y = References.z.Imaginary;
                References.p.z = 0;

                return References.p;
            }
            References.z = new Complex32(References.p.x, References.p.y);

            // perform matrix multiplication
            References.z = (m.Mobius[0, 0] * References.z + m.Mobius[0, 1]) / (m.Mobius[1, 0] * References.z + m.Mobius[1, 1]);

            References.p.x = References.z.Real;
            References.p.y = References.z.Imaginary;

            return References.p;
        }
    }

    /// **Overview** Check if this Vector3 is at Infinity
    /// 
    /// **Params**
    /// - v: calling Vector3 object
    /// 
    /// **Returns** True if the given Vector3 is infinite, false otherwise.
    public static bool IsInfinite(this Vector3 v)
    {
        return v.x == References.Infinity.x || v.y == References.Infinity.y || v.z == References.Infinity.z;
    }

    /// **Overview** Return a copy of this Vector3 reflected across the x-axis.
    /// 
    /// **Params**
    /// - v: calling Vector3 object
    /// 
    /// **Returns** A new Vector3 which is the calling Vector3 reflected.
    public static Vector3 Reflect(this Vector3 v)
    {
        return new Vector3(v.x, -v.y, v.z);
    }

    /// **Overview** Create a human-readable string version of this Vector3
    /// 
    /// **Params**
    /// - v: calling Vector3 object
    /// 
    /// **Returns** A string in the format (x, y)
    public static string ToString(this Vector3 v)
    {
        return "(" + v.x + ", " + v.y + ")";
    }
}
