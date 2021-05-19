using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// # MoveCursor.cs

/// If the cursor is currently shown, move it in response to the arrow
/// keys being pressed.
public class MoveCursor : MonoBehaviour
{
    /// ## Methods

    /// *Unity Function*
    ///
    /// **Overview** Check for the arrow keys being pressed and update the 
    /// game object's position in response. 
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 position = this.transform.position;
            position.x -= .2f;
            this.transform.position = position;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 position = this.transform.position;
            position.x += .2f;
            this.transform.position = position;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 position = this.transform.position;
            position.y += .2f;
            this.transform.position = position;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 position = this.transform.position;
            position.y -= .2f;
            this.transform.position = position;
        }
    }
}
