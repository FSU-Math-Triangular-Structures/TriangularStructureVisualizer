using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// # CameraZoom.cs

/// Controls the zooming of the camera on the scene. 
public class CameraZoom : MonoBehaviour
{
    /// ## Properties

    /// The main camera on the scene.
    private Camera Cam;
    /// This value is set when the user scrolls and signals
    /// when the zoom should be halted
    private float targetZoom;
    /// The value to multiply the raw mouse wheel input by.
    private float zoomFactor = 10f;
    /// How quickly to move through the various zoom levels.
    [SerializeField] private float zoomSpeed = 10;

    /// ## Methods

    /// *Unity Function*
    ///
    /// **Overview** Get the camera on the scene and update the targetZoom
    /// variable.
    void Start()
    {
        Cam = Camera.main;
        targetZoom = Cam.orthographicSize;
    }

    /// *Unity Function*
    ///
    /// **Overview** Respond to the mouse wheel being scrolled and perform
    /// the zoom operation. If a tighter or looser zoom are desired, be sure to check
    /// the "Clamp" values in this method. 
    void Update()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom -= scrollData * zoomFactor;
        targetZoom = Mathf.Clamp(targetZoom, 1f, 20f); // 4.5, 20
        Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
    }
}
