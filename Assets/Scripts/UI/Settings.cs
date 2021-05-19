using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// # Settings.cs

/// Handles whether or not to show the settings menu when the user clicks.
public class Settings : MonoBehaviour
{
    /// ## Properties

    /// UI element for the settings "gear" button
    public Button settingsButton;           
    /// Whether or not the settings object is shown
    public static bool settingsOn = false;

    /// ## Methods

    /// **Overview** Toggle the settingsOn boolean and show the settings
    /// menu if necessary.
    public void SettingsOn()
    {
        if (!settingsOn)
        {
            settingsOn = true;
            settingsButton.gameObject.SetActive(true);
        }
        else 
        {
            settingsOn = false;
            settingsButton.gameObject.SetActive(false);
        }
    }
}
