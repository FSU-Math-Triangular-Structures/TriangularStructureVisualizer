using System.Collections.Generic;
using UnityEngine;

/// # Trungle Behavior

/// This class is the main component associated with a Triangle, here
/// affectionately referred to as a "Trungle." This behavior represents
/// any kind of trungle -- Euclidean, hyperbolic, spherical, or weird hyperbolic.
public class TrungleBehavior : MonoBehaviour
{
    /// ## Properties
    
    /// The alpha vertex of this Trungle
    public Vector3 AlphaPoint { get; private set; }
    /// The beta vertex of this Trungle
    public Vector3 BetaPoint { get; private set; }
    /// The gamma vertex of this Trungle
    public Vector3 GammaPoint { get; private set; }

    /// The alpha prime point of the Trungle. In the Euclidean case, always Infinity.
    public Vector3 APrime { get; private set; }
    /// The beta prime point of the Trungle. In the Euclidean case, always Infinity.
    public Vector3 BPrime { get; private set; }
    /// The gamma prime point of the Trungle. In the Euclidean case, always Infinity.
    public Vector3 CPrime { get; private set; }

    /// The alpha angle of the Trungle
    public float AlphaAngle { get; private set; }
    /// The beta angle of the Trungle
    public float BetaAngle { get; private set; }
    /// The gamma angle of the Trungle
    public float GammaAngle { get; private set; }

    /// Whether or not the AB side is an arc. Always false for Euclidean case.
    public bool ABisArc { get; set; }
    /// Whether or not the AC side is an arc. Always false for Euclidean case.
    public bool ACisArc { get; set; }
    /// Whether or not the BC side is an arc. Always false for Euclidean case.
    public bool BCisArc { get; set; }

    /// Whether or not the AB side has been reflected across
    public bool ABwasReflectedAcross { get; set; }
    /// Whether or not the AC side has been reflected across
    public bool ACwasReflectedAcross { get; set; }
    /// Whether or not the BC side has been reflected across
    public bool BCwasReflectedAcross { get; set; }

    /// The parent Trungle of this Trungle used for rotations.
    public TrungleBehavior Parent { get; set; }

    /// The generation of this Trungle. Begins at 0; each successive
    /// click of the button is another "generation" out.
    public int Generation { get; set; }

    /// The actual arrays of points on the AB LineRenderer.
    public Vector3[] AB_Points { get => _abPoints; }
    /// The actual arrays of points on the AC LineRenderer.
    public Vector3[] AC_Points { get => _acPoints; }
    /// The actual arrays of points on the BC LineRenderer.
    public Vector3[] BC_Points { get => _bcPoints; }

    /// The LineRenderer objects for this prefab.
    /// These are set with the Unity Inspector.
    public LineRenderer AB_Render, AC_Render, BC_Render;
    
    private Vector3[] _abPoints;
    private Vector3[] _acPoints;
    private Vector3[] _bcPoints;

    public Material white;
    public Material purple;
    public Material blue;
    public Material orange;

    /// ## Methods

    /// **Overview** Set the three vertices of this Trungle.
    /// 
    /// **Params**
    /// - alphaPoint: the new alpha vertex
    /// - betaPoint: the new beta vertex
    /// - gammaPoint: the new gamma vertex
    public void SetPoints(Vector3 alphaPoint, Vector3 betaPoint, Vector3 gammaPoint)
    {
        AlphaPoint = alphaPoint;
        BetaPoint = betaPoint;
        GammaPoint = gammaPoint;
    }

    /// *This function brought to you by Amazon Prime!* 
    /// 
    /// **Overview** Set the three prime vertices of this Trungle.
    /// 
    /// **Params**
    /// - APrime: the new alpha prime vertex
    /// - BPrime: the new beta prime vertex
    /// - CPrime: the new gamma prime vertex
    public void SetPrimePoints(Vector3 APrime, Vector3 BPrime, Vector3 CPrime)
    {
        this.APrime = APrime;
        this.BPrime = BPrime;
        this.CPrime = CPrime;
    }

    /// **Overview** Set the three angles of this Trungle.
    /// 
    /// **Params**
    /// - alphaAngle: the new alpha angle
    /// - betaAngle: the new beta angle
    /// - gamamAngle: the new gamma angle
    public void SetAngles(float alphaAngle, float betaAngle, float gammaAngle)
    {
        AlphaAngle = alphaAngle;
        BetaAngle = betaAngle;
        GammaAngle = gammaAngle;
    }

    /// **Overview** Set the three sides' arrays of Vector3 of this Trungle.
    /// 
    /// **Params**
    /// - ABin: the new list of points for the AB side
    /// - ACin: the new list of points for the AC side
    /// - BCin: the new list of points for the BC side
    public void SetSides(List<Vector3> ABin, List<Vector3> ACin, List<Vector3> BCin)
    {
        AB_Render.positionCount = ABin.Count;
        AC_Render.positionCount = ACin.Count;
        BC_Render.positionCount = BCin.Count;
        
        _abPoints = new Vector3[ABin.Count];
        _acPoints = new Vector3[ACin.Count];
        _bcPoints = new Vector3[BCin.Count];

        for (int i = 0; i < ABin.Count; i++)
        {
            AB_Render.SetPosition(i, ABin[i]);
            _abPoints[i] = ABin[i];
        }
        for (int i = 0; i < ACin.Count; i++)
        {
            AC_Render.SetPosition(i, ACin[i]);
            _acPoints[i] = ACin[i];
        }
        for (int i = 0; i < BCin.Count; i++)
        {
            BC_Render.SetPosition(i, BCin[i]);
            _bcPoints[i] = BCin[i];
        }
    }

    /// **Overview** Set the three sides' arrays of Vector3 of this Trungle.
    /// 
    /// **Params**
    /// - ABin: the new array of points for the AB side
    /// - ACin: the new array of points for the AC side
    /// - BCin: the new array of points for the BC side
    public void SetSides(Vector3[] ABin, Vector3[] ACin, Vector3[] BCin)
    {
        AB_Render.positionCount = ABin.Length;
        AC_Render.positionCount = ACin.Length;
        BC_Render.positionCount = BCin.Length;

        _abPoints = ABin;
        _acPoints = ACin;
        _bcPoints = BCin;

        AB_Render.SetPositions(ABin);
        AC_Render.SetPositions(ACin);
        BC_Render.SetPositions(BCin);
    }

    public void WhiteOut()
    {
        AB_Render.material = white;
        AC_Render.material = white;
        BC_Render.material = white;
    }

    public void ColorOut()
    {
        AB_Render.material = blue;
        AC_Render.material = orange;
        BC_Render.material = purple;
    }

    /// **Overview** Destroy this Trungle and its associated GameObject, removing
    /// it from the scene.
    public void Destroy()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    /// **Overview** Helper function to get the array of points on the given LineRenderer.
    /// 
    /// **Params**
    /// - render: the LineRenderer to get the points from
    /// 
    /// **Returns** The array of Vector3s on the render
    private Vector3[] GetPoints(LineRenderer render)
    {
        Vector3[] o = new Vector3[render.positionCount];
        render.GetPositions(o);
        return o;
    }
}