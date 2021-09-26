using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraZoomTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        toggleCameraZoom();
    }

    public void toggleCameraZoom()
    {
        if(Input.GetKey(KeyCode.P))
        {
            this.GetComponent<Camera>().orthographicSize = this.GetComponent<Camera>().orthographicSize + 91 / 60 / cameraZoomTime;
        }
    }
}
