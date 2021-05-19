using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// # ItemDetails.cs

/// Essentially a data struct used by the settings menu to link 
/// an item's name and image in a single object.
public class ItemDetails : MonoBehaviour
{
    /// ## Properties

    /// UI label for the item's name
    public TMP_Text text = null;
    /// UI element to display the item's picture
    public Button button = null;
}