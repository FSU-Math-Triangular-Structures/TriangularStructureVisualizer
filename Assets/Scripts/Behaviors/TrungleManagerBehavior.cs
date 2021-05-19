using System.Collections.Generic;
using System.Collections;
using UnityEngine;

/// # TrungleManagerBehavior.cs

/// The manager class for all the Trungles currently on the scene. Internally 
/// has a list of every Trungle. Responsible for adding and deleting generations.
public class TrungleManagerBehavior : MonoBehaviour
{
    /// ## Properties
    
    /// Prefab for the TrungleBehavior
    public GameObject trungleStandard;
    /// Internal list of all Trungles
    private List<TrungleBehavior> trungles;
    private List<char> branches;
    // For adding to color list
    public bool addColor;
    public bool branchReflections;

    private bool blueReflect, orangeReflect, purpleReflect;

    /// ## Methods

    /// *Unity Function* 
    /// 
    /// **Overview** Sets `References.TrungleManager` to this object and inits
    /// `References.Infinity` to the appropriate value.
    void Awake()
    {
        References.TrungleManager = this;
        References.Infinity = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        References.CurrentMode = ReflectionMode.Regular;
    }

    /// *Unity Function* 
    /// 
    /// **Overview** Inits the list of Trungles to an empty list.
    void Start()
    {
        trungles = new List<TrungleBehavior>();
        branches = new List<char>();
        blueReflect = orangeReflect = purpleReflect = false;
        addColor = true;
    }

    /// **Overview** Called by `TotalAngle` when the user updates the UI angles. Deletes
    /// all trungles on the scene and redraws them all with the new angles.
    /// 
    /// **Params**
    /// - newAlpha: new alpha angle
    /// - newBeta: new beta angle
    /// - newGamma: new gamma angle
    public void AngleChanged(float newAlpha, float newBeta, float newGamma)
    {
        // clear it all out to restart
        DeleteGenerations(-1);
        
        // newBeta = newGamma;
        // then make the first triangle
        // there are never more than 0 steps when the angle is changed
        // because the counter is reset
        BuildFirstTriangle(newAlpha, newBeta, newGamma);

        int target = References.StepsManager.current;

        // check target generations & only build next trungles
        // if greater than 0
        if (References.CurrentMode == ReflectionMode.Regular)
        {
            if (target >= 1)
                BuildFirstReflections();
            if (target > 1)
                BuildReflections(References.StepsManager.current);
        }
        else
        {

            addColor = false;
            foreach (var s in branches)
            {     
                if (s == 'b')
                    BReflect();
                else if (s == 'o') 
                    OReflect();
                else if (s == 'p')
                    PReflect();              
            }
            addColor = true;
        }
        
    }

    /// **Overview** Called by `IncrementSteps` when the user presses + button. 
    /// Makes a call to AngleChanged of this manager with the angles from the
    /// original triangle in the queue.
    public void AddGenerations()
    {
        TrungleBehavior baseTrungle = trungles[0];
        AngleChanged(baseTrungle.AlphaAngle, baseTrungle.BetaAngle, baseTrungle.GammaAngle);
    }

    /// **Overview** Deletes every Trungle in the list whose generation is greater than
    /// the target generation. 
    /// 
    /// **Params**
    /// - targetGeneration: highest generation of Trungles to keep
    public void DeleteGenerations(int targetGeneration)
    {
        int top = trungles.Count - 1;
        // start at the back of the array (b/c new generations are pushed onto
        // the back) and destroy desired trungles
        while (top >= 0 && trungles[top].Generation > targetGeneration)
        {
            Pop();
            top--;
        }
    }

    private void Pop()
    {
        int top = trungles.Count - 1;

        // the first trungle has no Parent, so we can't run these checks
        if (top != 0)
        {
            // for each side of the trungle to be deleted, we unset
            // its parent's wasReflectedAcross so that they can be readded
            // in later steps
            if (AreSidesEqual(trungles[top].AB_Points, trungles[top].Parent.AB_Points))
                trungles[top].Parent.ABwasReflectedAcross = false;
            if (AreSidesEqual(trungles[top].AC_Points, trungles[top].Parent.AC_Points))
                trungles[top].Parent.ACwasReflectedAcross = false;
            if (AreSidesEqual(trungles[top].BC_Points, trungles[top].Parent.BC_Points))
                trungles[top].Parent.BCwasReflectedAcross = false;
        }

        // destroy game object and remove from queue
        trungles[top].Destroy();
        trungles.RemoveAt(top);
    }

    /// **Overview** Check if the two arrays have identical values
    /// 
    /// **Params**
    /// - first: first array to check
    /// - second: second array to check
    /// 
    /// **Returns** True if the arrays have equal values, false otherwise.
    private static bool AreSidesEqual(Vector3[] first, Vector3[] second)
    {
        // if different lengths, automatically different
        if (first.Length != second.Length)
            return false;

        // check each value, returning false if they differ
        for (int i = 0; i < first.Length; i++)
        {
            if (first[i] != second[i])
                return false;
        }

        // every check passed, so they are equal
        return true;
    }

    /// **Overview** Calculate all necessary values and build the first trungle, adding
    /// it to the scene.
    /// 
    /// **Params** 
    /// - newAlpha: new alpha angle
    /// - newBeta: new beta angle
    /// - newGamma: new gamma angle
    public void BuildFirstTriangle(float newAlpha, float newBeta, float newGamma)
    {
        GameObject go = Instantiate(trungleStandard, Vector3.zero, Quaternion.identity);
        go.name = "Gen0 a.k.a. GOD TRUNGLE";
        TrungleBehavior triangle = go.GetComponent<TrungleBehavior>();

        // required for calculations but are the same for each trungle
        Vector3 a = new Vector3(0, 0, 0);
        Vector3 b = new Vector3(1, 0, 0);

        // these are different per triangle and are set in each if branch
        Vector3 c;
        Vector3 aP, bP, cP; // prime points
        List<Vector3> bc;

        if (References.CurrentType == TriangleType.Euclidean)
        {
            var gammaX = (Mathf.Sin(newBeta) * Mathf.Cos(newAlpha)) / Mathf.Sin(newGamma);
            var gammaY = (Mathf.Sin(newBeta) * Mathf.Sin(newAlpha)) / Mathf.Sin(newGamma);

            c = new Vector3(gammaX, gammaY, 0);

            aP = bP = cP = References.Infinity;

            bc = TrungleUtils.LineBetween(b, c);

            triangle.BCisArc = false;
        }
        else
        {
            var A = (Mathf.Tan(newGamma + newAlpha) * Mathf.Tan(newAlpha)) + 1;
            var E = (Mathf.Tan(newGamma + newAlpha) + Mathf.Tan(newBeta)) - (A * Mathf.Tan(newBeta));
            var F = (Mathf.Tan(newAlpha) * Mathf.Tan(newGamma + newAlpha))
                    + (Mathf.Tan(newAlpha) * Mathf.Tan(newBeta))
                    - A;
            var G = 1 - A;
            var H = Mathf.Tan(newAlpha);

            var a_ = (E * E) + (F * F) - ((A * A) * Mathf.Pow(Mathf.Tan(newBeta), 2)) - (A * A);
            var b_ = (2 * E * G) + (2 * F * H);
            var c_ = (G * G) + (H * H);

            float centerY;

            // we commence the Weird Hyperbolic checks
            const float piOverTwo = 90 * Mathf.Deg2Rad;
            
            if ((newBeta > piOverTwo && newGamma < piOverTwo)
                || (newBeta < piOverTwo && newGamma > piOverTwo))
            {
                centerY = (-b_ + (Mathf.Sqrt((b_ * b_) - (4 * a_ * c_)))) / (2 * a_);
            }
            else
            {
                centerY = (-b_ - (Mathf.Sqrt((b_ * b_) - (4 * a_ * c_)))) / (2 * a_);
            }

            float centerX = 1 + (centerY * Mathf.Tan(newBeta));

            var radius = Mathf.Sqrt(((1 - centerX) * (1 - centerX)) + (centerY * centerY));

            var center = new Vector3(centerX, centerY, 0);

            var gammaX = (Mathf.Tan(newAlpha + newGamma) * centerY + centerX) / A;
            var gammaY = ((Mathf.Tan(newAlpha + newGamma) * centerY + centerX) / A) * Mathf.Tan(newAlpha);

            c = new Vector3(gammaX, gammaY, 0);

            aP = References.Infinity;
            bP = TrungleUtils.CalculateBPrime(center, radius, b);
            cP = TrungleUtils.CalculateCPrime(c, center, radius);

            bc = TrungleUtils.ArcBetween(b, c, center, radius, newAlpha);

            triangle.BCisArc = true;
        }

        // these are in common too but couldn't be set until after the ifs
        List<Vector3> ab = TrungleUtils.LineBetween(a, b);
        List<Vector3> ac = TrungleUtils.LineBetween(a, c);

        // everything left is in common and should be set by now:
        triangle.Generation = 0;

        triangle.ABwasReflectedAcross = false;
        triangle.ACwasReflectedAcross = false;
        triangle.BCwasReflectedAcross = false;

        triangle.ABisArc = false;
        triangle.ACisArc = false;

        triangle.SetAngles(newAlpha, newBeta, newGamma);
        triangle.SetPoints(a, b, c);
        triangle.SetPrimePoints(aP, bP, cP);
        triangle.SetSides(ab, ac, bc);

        trungles.Add(triangle);
    }

    /// **Overview** Build the first three special case reflections and add them to the 
    /// scene.
    public void BuildFirstReflections()
    {
        GameObject go;
        TrungleBehavior baseTriangle = trungles[0];

        // TRIANGLE 1
        if (References.CurrentMode == ReflectionMode.Regular || blueReflect)
        {
            go = Instantiate(trungleStandard, Vector3.zero, Quaternion.identity);
            go.name = "Gen1";

            var child1 = go.GetComponent<TrungleBehavior>();

            Vector3 reflectedPoint = baseTriangle.GammaPoint.Reflect();
            Vector3 cP;

            if (References.CurrentType == TriangleType.Euclidean)
            {
                cP = References.Infinity;

                child1.ABisArc = child1.ACisArc = child1.BCisArc = false;
            }
            else
            {
                cP = baseTriangle.CPrime.Reflect();

                child1.ABisArc = false;
                child1.ACisArc = false;
                child1.BCisArc = true;
            }

            child1.Generation = 1;

            child1.ABwasReflectedAcross = true;
            child1.ACwasReflectedAcross = false;
            child1.BCwasReflectedAcross = false;

            child1.SetAngles(baseTriangle.AlphaAngle, baseTriangle.BetaAngle, baseTriangle.GammaAngle);
            child1.SetPoints(baseTriangle.AlphaPoint, baseTriangle.BetaPoint, reflectedPoint);
            child1.SetPrimePoints(baseTriangle.APrime, baseTriangle.BPrime, cP);
            child1.SetSides((Vector3[])baseTriangle.AB_Points.Clone(), TrungleUtils.Reflect(baseTriangle.AC_Points), TrungleUtils.Reflect(baseTriangle.BC_Points));

            child1.Parent = baseTriangle;

            trungles.Add(child1);

            baseTriangle.ABwasReflectedAcross = true;
        }
        // TRIANGLE 2
        if (References.CurrentMode == ReflectionMode.Regular || purpleReflect)
        {
            go = Instantiate(trungleStandard, Vector3.zero, Quaternion.identity);
            go.name = "Gen1";
            var child2 = go.GetComponent<TrungleBehavior>();

            Matrix child2Matrix = new Matrix(baseTriangle.BetaPoint, baseTriangle.BPrime, baseTriangle.BetaAngle, RotationDirection.Clockwise);

            Vector3 rotatedAlpha = baseTriangle.AlphaPoint.ApplyMatrix(child2Matrix);
            Vector3 aP;

            if (References.CurrentType == TriangleType.Euclidean)
            {
                aP = References.Infinity;
                child2.ABisArc = child2.ACisArc = child2.BCisArc = false;
            }
            else
            {
                aP = baseTriangle.APrime.ApplyMatrix(child2Matrix);
                child2.ABisArc = child2.ACisArc = child2.BCisArc = true;
            }

            child2.Generation = 1;

            child2.ABwasReflectedAcross = false;
            child2.ACwasReflectedAcross = false;
            child2.BCwasReflectedAcross = true;

            child2.SetAngles(baseTriangle.AlphaAngle, baseTriangle.BetaAngle, baseTriangle.GammaAngle);
            child2.SetPoints(rotatedAlpha, baseTriangle.BetaPoint, baseTriangle.GammaPoint);
            child2.SetPrimePoints(aP, baseTriangle.BPrime, baseTriangle.CPrime);

            var newAB = TrungleUtils.Rotate(baseTriangle.AB_Points, child2Matrix);
            var newAC = TrungleUtils.Rotate(TrungleUtils.Reflect(baseTriangle.AC_Points), child2Matrix);
            var newBC = (Vector3[])baseTriangle.BC_Points.Clone();

            child2.SetSides(newAB, newAC, newBC);

            child2.Parent = baseTriangle;

            trungles.Add(child2);

            baseTriangle.BCwasReflectedAcross = true;
        }
        // TRIANGLE 3
        if (References.CurrentMode == ReflectionMode.Regular || orangeReflect)
        {
            go = Instantiate(trungleStandard, Vector3.zero, Quaternion.identity);
            go.name = "Gen1";
            var child3 = go.GetComponent<TrungleBehavior>();

            Matrix child3Matrix = new Matrix(baseTriangle.AlphaPoint, baseTriangle.APrime, baseTriangle.AlphaAngle, RotationDirection.Counterclockwise);

            Vector3 rotatedBeta = baseTriangle.BetaPoint.ApplyMatrix(child3Matrix);
            Vector3 bP;

            if (References.CurrentType == TriangleType.Euclidean)
            {
                bP = References.Infinity;

                child3.ABisArc = child3.ACisArc = child3.BCisArc = false;
            }
            else
            {
                bP = baseTriangle.BPrime.ApplyMatrix(child3Matrix);

                child3.ABisArc = false;
                child3.ACisArc = false;
                child3.BCisArc = true;
            }

            child3.Generation = 1;

            child3.ABwasReflectedAcross = false;
            child3.ACwasReflectedAcross = true;
            child3.BCwasReflectedAcross = false;

            child3.SetAngles(baseTriangle.AlphaAngle, baseTriangle.BetaAngle, baseTriangle.GammaAngle);
            child3.SetPoints(baseTriangle.AlphaPoint, rotatedBeta, baseTriangle.GammaPoint);
            child3.SetPrimePoints(baseTriangle.APrime, bP, baseTriangle.CPrime);

            var newerAB = TrungleUtils.Rotate(baseTriangle.AB_Points, child3Matrix);
            var newerAC = (Vector3[])baseTriangle.AC_Points.Clone();
            var newerBC = TrungleUtils.Rotate(TrungleUtils.Reflect(baseTriangle.BC_Points), child3Matrix);

            child3.SetSides(newerAB, newerAC, newerBC);

            child3.Parent = baseTriangle;

            trungles.Add(child3);

            baseTriangle.ACwasReflectedAcross = true;
        }
    }

    /// **Overview** Build and add trungles to the scene until all the trungles in the targetGeneration
    /// have been created. See the rotation algorithm slide in the docs for more information.
    ///
    /// **Params**
    /// - targetGeneration: the highest generation of trungles to build
    private void BuildReflections(int targetGeneration)
    {
        RotationDirection direction;

        int i = 1;

        if (References.CurrentMode == ReflectionMode.Branching) 
            i = trungles.Count - 1;

        for ( ; i < trungles.Count; i++)
        {
            if (trungles[i].Generation == targetGeneration)
                continue;

            //check whther the side needs to be solved for (first thing to do)
            if (!trungles[i].ABwasReflectedAcross && (References.CurrentMode == ReflectionMode.Regular || blueReflect))
            {
                //this means beta is COR
                if (trungles[i].Parent.BetaPoint == trungles[i].BetaPoint)
                {
                    direction = TrungleUtils.GetDirection(trungles[i].GammaPoint, trungles[i].BetaPoint, trungles[i].AlphaPoint, trungles[i].Parent.AlphaPoint);

                    Vector3 CoR2 = trungles[i].BPrime;

                    if (!trungles[i].ABisArc && !trungles[i].BCisArc)
                        CoR2 = References.Infinity;

                    var child = RotateTriangle(trungles[i], trungles[i].BetaAngle, trungles[i].BetaPoint, CoR2, "AB", direction);

                    trungles.Add(child);
                }

                //if alpha is COR
                if (trungles[i].Parent.AlphaPoint == trungles[i].AlphaPoint)
                {
                    direction = TrungleUtils.GetDirection(trungles[i].GammaPoint, trungles[i].AlphaPoint, trungles[i].BetaPoint, trungles[i].Parent.BetaPoint);

                    Vector3 CoR2 = trungles[i].APrime;

                    if (!trungles[i].ABisArc && !trungles[i].ACisArc)
                        CoR2 = References.Infinity;

                    var child = RotateTriangle(trungles[i], trungles[i].AlphaAngle, trungles[i].AlphaPoint, CoR2, "AB", direction);

                    trungles.Add(child);
                }

                if (References.CurrentMode == ReflectionMode.Branching) return;
            }
            if (!trungles[i].ACwasReflectedAcross && (References.CurrentMode == ReflectionMode.Regular || orangeReflect))
            {
                if (trungles[i].Parent.GammaPoint == trungles[i].GammaPoint)
                {
                    direction = TrungleUtils.GetDirection(trungles[i].BetaPoint, trungles[i].GammaPoint, trungles[i].AlphaPoint, trungles[i].Parent.AlphaPoint);

                    Vector3 CoR2 = trungles[i].CPrime;

                    if (!trungles[i].ACisArc && !trungles[i].BCisArc)
                        CoR2 = References.Infinity;

                    var child = RotateTriangle(trungles[i], trungles[i].GammaAngle, trungles[i].GammaPoint, CoR2, "AC", direction);

                    trungles.Add(child);
                }

                if (trungles[i].Parent.AlphaPoint == trungles[i].AlphaPoint)
                {
                    direction = TrungleUtils.GetDirection(trungles[i].BetaPoint, trungles[i].AlphaPoint, trungles[i].GammaPoint, trungles[i].Parent.GammaPoint);

                    Vector3 CoR2 = trungles[i].APrime;

                    if (!trungles[i].ACisArc && !trungles[i].ABisArc)
                        CoR2 = References.Infinity;

                    var child = RotateTriangle(trungles[i], trungles[i].AlphaAngle, trungles[i].AlphaPoint, CoR2, "AC", direction);

                    trungles.Add(child);
                }

                if (References.CurrentMode == ReflectionMode.Branching) return;
            }
            if (!trungles[i].BCwasReflectedAcross && (References.CurrentMode == ReflectionMode.Regular || purpleReflect))
            {
                if (trungles[i].Parent.BetaPoint == trungles[i].BetaPoint)
                {
                    direction = TrungleUtils.GetDirection(trungles[i].AlphaPoint, trungles[i].BetaPoint, trungles[i].GammaPoint, trungles[i].Parent.GammaPoint);

                    Vector3 CoR2 = trungles[i].BPrime;

                    if (!trungles[i].BCisArc && !trungles[i].ABisArc)
                        CoR2 = References.Infinity;

                    var child = RotateTriangle(trungles[i], trungles[i].BetaAngle, trungles[i].BetaPoint, CoR2, "BC", direction);

                    trungles.Add(child);
                }

                if (trungles[i].Parent.GammaPoint == trungles[i].GammaPoint)
                {
                    direction = TrungleUtils.GetDirection(trungles[i].AlphaPoint, trungles[i].GammaPoint, trungles[i].BetaPoint, trungles[i].Parent.BetaPoint);

                    Vector3 CoR2 = trungles[i].CPrime;

                    if (!trungles[i].BCisArc && !trungles[i].ACisArc)
                        CoR2 = References.Infinity;

                    var child = RotateTriangle(trungles[i], trungles[i].GammaAngle, trungles[i].GammaPoint, CoR2, "BC", direction);

                    trungles.Add(child);
                }

                if (References.CurrentMode == ReflectionMode.Branching) return;
            }
        }
    }

    /// **Overview** Calculates the correct rotation matrix and applies it to all the sides and vertices of 
    /// the given trungle. Builds a new trungle object, adds it to the scene, and returns a reference to that 
    /// new child to be added to the list.
    /// 
    /// **Params**
    /// - current - triangle to be rotated
    /// - angle - angle to rotate through
    /// - CoR1 - first center of rotation
    /// - CoR2 - second center of rotation
    /// - side - string representation of what side we are reflecting across, e.g. `"AB"`
    /// - direction - direction to rotate about
    ///
    /// **Returns** a new `TrungleBehavior` which is a rotated version of the given triangle
    private TrungleBehavior RotateTriangle(TrungleBehavior current, float angle, Vector3 CoR1, Vector3 CoR2, string side, RotationDirection direction)
    {
        Matrix m = new Matrix(CoR1, CoR2, angle, direction);

        Vector3 newAlpha, newBeta, newGamma;
        Vector3 newAP, newBP, newCP;

        if (side == "AB" || side == "AC")
        {
            newAlpha = current.AlphaPoint;
            newAP = current.APrime;
        }
        else
        {
            newAlpha = current.AlphaPoint.ApplyMatrix(m);

            if (References.CurrentType == TriangleType.Euclidean)
                newAP = References.Infinity;
            else
                newAP = current.APrime.ApplyMatrix(m);
        }

        if (side == "AB" || side == "BC")
        {
            newBeta = current.BetaPoint;
            newBP = current.BPrime;
        }
        else
        {
            newBeta = current.BetaPoint.ApplyMatrix(m);

            if (References.CurrentType == TriangleType.Euclidean)
                newBP = References.Infinity;
            else
                newBP = current.BPrime.ApplyMatrix(m);
        }

        if (side == "AC" || side == "BC")
        {
            newGamma = current.GammaPoint;
            newCP = current.CPrime;
        }
        else
        {
            newGamma = current.GammaPoint.ApplyMatrix(m);

            if (References.CurrentType == TriangleType.Euclidean)
                newCP = References.Infinity;
            else
                newCP = current.CPrime.ApplyMatrix(m);
        }

        Vector3[] newAB, newAC, newBC;

        bool newABwasReflectedAcross = false,
             newACwasReflectedAcross = false,
             newBCwasReflectedAcross = false;

        bool newABisArc, newACisArc, newBCisArc;

        // if not doing a Euclidean rotation, everything becomes an arc
        // we check the Matrix that we used for rotations to see if that is Mobius
        if (m.IsMobius)
        {
            newABisArc = newACisArc = newBCisArc = true;
        }
        // otherwise, take the previous values
        else
        {
            newABisArc = current.Parent.ABisArc;
            newACisArc = current.Parent.ACisArc;
            newBCisArc = current.Parent.BCisArc;
        }

        if (side == "AB")
        {
            newAB = (Vector3[]) current.AB_Points.Clone();
            current.ABwasReflectedAcross = true;
            newABwasReflectedAcross = true;
        }
        else
        {
            newAB = TrungleUtils.Rotate(current.Parent.AB_Points, m);
        }
        if (side == "AC")
        {
            newAC = (Vector3[]) current.AC_Points.Clone();
            current.ACwasReflectedAcross = true;
            newACwasReflectedAcross = true;
        }
        else
        {
            newAC = TrungleUtils.Rotate(current.Parent.AC_Points, m);
        }
        if (side == "BC")
        {
            newBC = (Vector3[]) current.BC_Points.Clone();
            current.BCwasReflectedAcross = true;
            newBCwasReflectedAcross = true;
        }
        else
        {
            newBC = TrungleUtils.Rotate(current.Parent.BC_Points, m);
        }

        GameObject go = Instantiate(trungleStandard, Vector3.zero, Quaternion.identity);
        go.name = "Gen" + (current.Generation + 1).ToString();
        TrungleBehavior child = go.GetComponent<TrungleBehavior>();

        child.Generation = current.Generation + 1;

        child.ABwasReflectedAcross = newABwasReflectedAcross;
        child.ACwasReflectedAcross = newACwasReflectedAcross;
        child.BCwasReflectedAcross = newBCwasReflectedAcross;

        child.ABisArc = newABisArc;
        child.ACisArc = newACisArc;
        child.BCisArc = newBCisArc;

        child.SetPoints(newAlpha, newBeta, newGamma);
        child.SetPrimePoints(newAP, newBP, newCP);
        child.SetAngles(current.AlphaAngle, current.BetaAngle, current.GammaAngle);
        child.SetSides(newAB, newAC, newBC);

        child.Parent = current;

        return child;
    }

    public void BReflect() 
    {
        blueReflect = true;

        WhiteOutAll();

        if (trungles[trungles.Count - 1].ABwasReflectedAcross)
        {
            Pop();
            branches.RemoveAt(branches.Count - 1);
            trungles[trungles.Count - 1].ColorOut();
        }
        else
        {
            if (trungles.Count == 1)
                BuildFirstReflections();
            else
                BuildReflections(100);

            if (addColor)
                branches.Add('b');
        }

        blueReflect = false;
    }

    public void OReflect()
    {
        orangeReflect = true;

        WhiteOutAll();

        if (trungles[trungles.Count - 1].ACwasReflectedAcross)
        {
            Pop();
            branches.RemoveAt(branches.Count - 1);
            trungles[trungles.Count - 1].ColorOut();
        }
        else
        {
            if (trungles.Count == 1)
                BuildFirstReflections();
            else
                BuildReflections(100);

            if (addColor)
                branches.Add('o');
        }

        orangeReflect = false;
    }

    public void PReflect()
    {
        purpleReflect = true;

        WhiteOutAll();

        if (trungles[trungles.Count - 1].BCwasReflectedAcross)
        {
            Pop();
            branches.RemoveAt(branches.Count - 1);
            trungles[trungles.Count - 1].ColorOut();
        }
        else
        {
            if (trungles.Count == 1)
                BuildFirstReflections();
            else
                BuildReflections(100);

            if (addColor)
                branches.Add('p');
        }

        purpleReflect = false;
    }

    private void WhiteOutAll()
    {
        for (int i = 0; i < trungles.Count; i++)
            trungles[i].WhiteOut();
    }
}

