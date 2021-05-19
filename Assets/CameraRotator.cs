using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{

    bool state;

    public GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        References.cameraRotate = this;
    }
    public float speed = 1.0f;
    // Update is called once per frame
    void Update()
    {
        if (state)
        {
            if (Input.GetMouseButton(0))
            {
                transform.Rotate(new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
            }
            if (Input.GetMouseButton(1))
            {
                resetCamera();
            }
        }
    }

    void resetCamera()
    {
        transform.Rotate(new Vector3(-transform.eulerAngles.x, -transform.eulerAngles.y, -transform.eulerAngles.z));
    }

    public void toggleSphere(bool toggle)
    {
        sphere.SetActive(toggle);
        state = toggle;
        if (!toggle)
        {
            resetCamera();
        }
    }

}
