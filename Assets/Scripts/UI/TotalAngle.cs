using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// # TotalAngle.cs

/// This class monitors the sliders and input boxes for the three angles
/// on the UI and makes appropriate calls to the Trungle Manager when the 
/// angle is changed.
public class TotalAngle : MonoBehaviour
{
    /// ## Properties

    /// UI slider for the alpha angle
    public Slider alphaSlider;
    /// UI slider for the beta angle
    public Slider betaSlider;
    /// UI slider for the gamma angle
    public Slider gammaSlider;

    /// UI input field for the alpha angle
    public TMP_InputField alphaInput;
    /// UI input field for the beta angle
    public TMP_InputField betaInput;
    /// UI input field for the gamma angle
    public TMP_InputField gammaInput;

    /// UI label for the total angle
    public TMP_Text totalAngleText;

    /// UI label for the current trungle type
    public TMP_Text Type;

    /// UI label for the current number of steps
    public TMP_Text stepsObject;

    /// UI element to display an error message on failed parse
    public Button Error;

    /// The current total angle
    private float currentTotal;

    /// The values of the alpha, beta, & gamma sliders
    private float alpha, beta, gamma;

    public Button BlueButton, PurpleButton, OrangeButton;

    public GameObject grid;

    /// ## Methods

    /// *Unity function*
    ///
    /// **Overview** Set up necessary values for this object and prepare for parsing.
    void Start()
    {
        currentTotal = 180;

        totalAngleText.color = Color.green;
        Type.text = "Euclidean";
        References.CurrentType = TriangleType.Euclidean;

        totalAngleText.text = currentTotal.ToString("#.##") + "°";

        alpha = 60 * Mathf.Deg2Rad; 
        beta = 60 * Mathf.Deg2Rad;
        gamma = 60 * Mathf.Deg2Rad; 

        References.TrungleManager.AngleChanged(alpha, beta, gamma);


        grid.SetActive(true);
        References.cameraRotate.toggleSphere(false);
    }

    public void beginBranching()
    {

        if (References.CurrentMode == ReflectionMode.Regular)
        {
            References.CurrentMode = ReflectionMode.Branching;
           
        }
        else
        {
      
            References.CurrentMode = ReflectionMode.Regular;

        }

        References.StepsManager.current = 0;
        References.TrungleManager.AngleChanged(alpha, beta, gamma);
        BlueButton.gameObject.SetActive(References.CurrentMode == ReflectionMode.Branching);
        PurpleButton.gameObject.SetActive(References.CurrentMode == ReflectionMode.Branching);
        OrangeButton.gameObject.SetActive(References.CurrentMode == ReflectionMode.Branching);
    }

    /// *Unity function*
    ///
    /// **Overview** Check if the angles have changed from the last frame and call 
    /// Trungle Manager's `AngleChanged()` function if necessary.
    void Update()
    {
        
        // this is a legacy check and could probably be factored out
        if (alphaInput.text == "" || betaInput.text == "" || gammaInput.text == "")
        {
            totalAngleText.text = "...";
            return;
        }

        // set the currentTotal for this frame
        currentTotal = alphaSlider.value + betaSlider.value + gammaSlider.value;
        Error.gameObject.SetActive(false);
        totalAngleText.text = currentTotal.ToString("#.##") + "°";

        // check if any of the three angles have changed
        if (Mathf.Abs(alphaSlider.value * Mathf.Deg2Rad - alpha) >= 0.001 ||
            Mathf.Abs(betaSlider.value * Mathf.Deg2Rad - beta) >= 0.001 ||
            Mathf.Abs(gammaSlider.value * Mathf.Deg2Rad - gamma) >= 0.001)
        {
            // uncomment this line to get old functionality where an angle change would
            // set generations to 0
            //References.StepsManager.Reset();

            // yes Elias, we are storing these always as radians
            alpha = alphaSlider.value * Mathf.Deg2Rad;
            beta = betaSlider.value * Mathf.Deg2Rad;
            gamma = gammaSlider.value * Mathf.Deg2Rad;

            // Euclidean!
            if (Mathf.Abs(currentTotal - 180) < 0.01)
            {
                // update UI
                totalAngleText.color = Color.green;
                Type.text = "Euclidean";
                grid.SetActive(true);
                References.cameraRotate.toggleSphere(false);
                // set type & call function
                References.CurrentType = TriangleType.Euclidean;
                References.TrungleManager.AngleChanged(alpha, beta, gamma);
            }
            // Hyperbolic!
            else if (currentTotal < 180)
            {
                // update UI
                totalAngleText.color = Color.green;
                Type.text = "Hyperbolic";
                grid.SetActive(true);
                References.cameraRotate.toggleSphere(false);
                // set type & call function
                References.CurrentType = TriangleType.Hyperbolic;
                References.TrungleManager.AngleChanged(alpha, beta, gamma);
            }
            // Spherical!
            else if (currentTotal > 180 && currentTotal < 720)
            {
                // update UI
                totalAngleText.color = Color.green;
                Type.text = "Spherical";
                grid.SetActive(false);
                References.cameraRotate.toggleSphere(true);
                // set type & call function
                References.CurrentType = TriangleType.Spherical;
                References.TrungleManager.AngleChanged(alpha, beta, gamma);
            }
            // Error!
            else
            {
                // update UI
                totalAngleText.color = Color.red;
                Type.text = "Error";

                // show the error message
                Error.gameObject.SetActive(true);
            }
        } 
        
    }
}
