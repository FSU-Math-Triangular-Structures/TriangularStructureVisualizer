using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics;

/// # Matrix.cs

/// Represents a single rotation matrix, either mobius or non-mobius, as well as 
/// some misc. values necessary for matrix multiplication.
public class Matrix
{
    /// ## Properties

    /// If this Matrix represents a mobius matrix, this is it. Otherwise, null.
    public Complex32[,] Mobius { get; private set; }

    /// If this Matrix represents a non-mobius matrix, this is it. Otherwise, null.
    public float[,] NonMobius { get; private set; }

    /// When doing a Euclidean rotation, the matrix must carry this 
    /// info with it due to the nature of the rotation algorithm
    public Vector3 CoR1 { get; private set; }
    
    /// Convenience boolean to quickly check if this Matrix is Mobius.
    public bool IsMobius { get; private set; }

    /// ## Methods

    /// **Overview** The only function on the public API that builds either a mobius
    /// or non-mobius matrix for this object.
    /// 
    /// **Params**
    /// - CoR1: first center of rotation
    /// - CoR2: second center of rotation; if Infinity, makes this matrix non-mobius
    /// - angle: angle to rotate through
    /// - direction: direction of rotation
    public Matrix(Vector3 CoR1, Vector3 CoR2, float angle, RotationDirection direction)
    {
        IsMobius = !CoR2.IsInfinite();

        if (IsMobius)
        {
            Mobius = GetMobiusMatrix(CoR1, CoR2, angle, direction);
        }
        else
        {
            NonMobius = GetMatrix(angle, direction);
            this.CoR1 = CoR1;
        }
    }

    /// ### GetMatrix
    /// 
    /// **Overview** Private function to actually generate a non-mobius matrix.
    /// The generated matrix is: \\[ \begin{pmatrix} \cos \theta & \sin \theta \\\\ -\sin \theta & \cos \theta \end{pmatrix} \\] (or using the 
    /// negative of the angle in case of counter-clockwise rotation.)
    /// where \\( \theta \\) is `angle`
    /// 
    /// **Params**
    /// - angle: angle to rotate through
    /// - direction: direction of rotation
    /// 
    /// **Returns** 2D array of floats to represent the matrix
    private static float[,] GetMatrix(float angle, RotationDirection direction)
    {
        // in Euclidean case, always use 2 * angle
        angle *= 2;

        // choose correct transform matrix
        float[,] rotationMatrix;

        if (direction == RotationDirection.Clockwise)
            rotationMatrix = new float[,] { {  Mathf.Cos(angle), Mathf.Sin(angle) },
                                            { -Mathf.Sin(angle), Mathf.Cos(angle) } };
        else
            rotationMatrix = new float[,] { { Mathf.Cos(angle), -Mathf.Sin(angle) },
                                            { Mathf.Sin(angle),  Mathf.Cos(angle) } };

        return rotationMatrix;
    }

    /// **Overview** Private function to actually generate a mobius matrix.
    /// The generated matrix is: \\[ \frac{1}{x -y} \begin{pmatrix} -y e^{i\theta} + x e^{-i\theta} & x y (e^{i\theta} - e^{-i\theta}) \\\\ -e^{i\theta} + e^{-i\theta} & x e^{i\theta} - y e^{-i\theta} \end{pmatrix} \\]
    /// 
    /// **Params**
    /// - CoR1: first center of rotation
    /// - CoR2: second center of rotation; must not be Infinity
    /// - angle: angle to rotate through
    /// - direction: direction of rotation
    /// 
    /// **Returns** 2D array of Complex32s to represent the matrix
    private static Complex32[,] GetMobiusMatrix(Vector3 CoR1, Vector3 CoR2, float angle, RotationDirection direction)
    {
        // flip the angle if going clockwise
        if (direction == RotationDirection.Clockwise)
            angle = -angle;

        Complex32 x = new Complex32(CoR1.x, CoR1.y);
        Complex32 y = new Complex32(CoR2.x, CoR2.y);
        
        // the first is e^ia, second is e^-ia
        Complex32 eToTheIAngle = new Complex32(Mathf.Cos(angle), Mathf.Sin(angle));
        Complex32 eToTheNegativeIAngle = new Complex32(Mathf.Cos(-angle), Mathf.Sin(-angle));

        // yeah honestly idk what -- this is a legacy comment left in for humor
        // in reality, this just makes a basic mobius matrix 
        Complex32[,] rotationMatrix = new Complex32[,] { { -y * eToTheIAngle + x * eToTheNegativeIAngle, x * y * (eToTheIAngle - eToTheNegativeIAngle)},
                                                         { -eToTheIAngle + eToTheNegativeIAngle        , x * eToTheIAngle - y * eToTheNegativeIAngle  } };

        // everything in the matrix needs to be multiplied by this value
        Complex32 A = 1 / (x - y);

        rotationMatrix[0, 0] *= A;
        rotationMatrix[0, 1] *= A;
        rotationMatrix[1, 0] *= A;
        rotationMatrix[1, 1] *= A;

        return rotationMatrix;
    }
}
