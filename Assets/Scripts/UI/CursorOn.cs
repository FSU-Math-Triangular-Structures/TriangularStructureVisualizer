using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// # CursorOn.cs

/// Turns the movable cursor on or off when a settings button
/// is clicked.
public class CursorOn : MonoBehaviour
{
    /// ## Properties

    /// Whether or not the cursor is currently shown.
    private bool cursorOn = false;

    /// The actual cursor on the scene.
    public GameObject cursor;

    /// ## Methods

    /// **Overview** Called on a settings button clicked and 
    /// toggles the cursorOn value, setting the game object
    /// to active or inactive as appropriate.
    public void TurnOnOff()
    {
        cursorOn = !cursorOn;

        if (cursorOn)
            cursor.SetActive(true);
        else
            cursor.SetActive(false);
    }
}
