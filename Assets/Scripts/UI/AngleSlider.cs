using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// # AngleSlider.cs

/// UI script only responsible for syncing the values on the slider box
/// with the value on the associated input box. The slider is the only
/// source of truth for TotalAngle to trigger a redraw.
public class AngleSlider : MonoBehaviour
{
    /// ## Properties

    /// UI slider to check each frame
    public Slider slide;
    /// UI input field to update on angle change
    public TMP_InputField input;

    /// The value of this slider on the previous frame, used to see
    /// if the value has been changed.
    private float storePrev = 60;

    /// ## Methods

    /// *Unity Function*
    ///
    /// **Overview** Set the text of the input box to the starting value
    /// of the slider.
    void Start()
    {
        input.text = slide.value.ToString();
    }

    /// *Unity Function*
    ///
    /// **Overview** Checks if the slider value has changed and updates the input
    /// box if necessary.
    void Update()
    {
        // the sliders are only whole numbers, so we can check for exact equality
        if (storePrev != slide.value)
        {
            input.text = slide.value.ToString();
            storePrev = slide.value;
        }
    }  
}
