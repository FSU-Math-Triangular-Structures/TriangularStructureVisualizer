using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

/// # DragNDrop.cs

/// Handles the movement of the UI menu when the user attempts to drag it
/// around the scene. 
public class DragNDrop : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    /// ## Properties

    /// The position of the mouse on the last frame, used to check if the mouse
    /// has moved.
    private Vector2 lastMousePosition;

    /// ## Methods

    /// **Overview** This method will be called on the start of the mouse drag
    /// 
    /// **Params**
    /// - eventData: mouse pointer event data
    public void OnBeginDrag(PointerEventData eventData)
    {
        lastMousePosition = eventData.position;
    }

    /// **Overview** This method will be called during the mouse drag
    /// 
    /// **Params**
    /// - eventData: mouse pointer event data
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentMousePosition = eventData.position;
        Vector2 diff = currentMousePosition - lastMousePosition;
        RectTransform rect = GetComponent<RectTransform>();

        Vector3 newPosition = rect.position + new Vector3(diff.x, diff.y, transform.position.z);
        Vector3 oldPos = rect.position;
        rect.position = newPosition;

        if (!IsRectTransformInsideSreen(rect))
        {
            rect.position = oldPos;
        }

        lastMousePosition = currentMousePosition;
    }

    /// **Overview** This methods will check is the rect transform is inside the screen or not
    /// 
    /// **Params**
    /// - rectTransform: the rectangle to check
    ///
    /// **Returns** True if the given rect is inside the screen, false otherwise.
    private bool IsRectTransformInsideSreen(RectTransform rectTransform)
    {
        bool isInside = false;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        int visibleCorners = 0;
        Rect rect = new Rect(0, 0, Screen.width, Screen.height);

        foreach (Vector3 corner in corners)
        {
            if (rect.Contains(corner))
            {
                visibleCorners++;
            }
        }

        if (visibleCorners == 4)
        {
            isInside = true;
        }
        
        return isInside;
    }
}
