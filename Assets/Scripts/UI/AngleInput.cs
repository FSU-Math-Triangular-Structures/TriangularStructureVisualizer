using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// # AngleInput.cs

/// UI script only responsible for syncing the values on the input box
/// with the value on the associated slider.
public class AngleInput : MonoBehaviour
{
    /// ## Properties

    /// UI slider to change on angle input
    public Slider slide;
    /// UI input field to check each frame
    public TMP_InputField input;

    /// ## Methods

    /// *Unity Function*
    ///
    /// **Overview** Check if the user pressed Enter and update the slider
    /// if they did.
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            slide.value = float.Parse(input.text.ToString());
        }
    }
}
