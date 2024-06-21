using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float speedMod = 10.0f;
    [SerializeField]
    private float zoomSpeed = 1.0f;
    [SerializeField]
    private float minZoomDistance = 10.0f;
    [SerializeField]
    private float maxZoomDistance = 15.0f;
    private Vector3 point;

    // Start is called before the first frame update
    void Start()
    {
        point = target.transform.position;
        transform.LookAt(point);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate around Y-Axis
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            transform.RotateAround(point, Vector3.up, mouseX * speedMod);
        }

        // Zoom in/out with mouse wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 dir = (point - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, point);
        if (scroll != 0)
        {
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minZoomDistance, maxZoomDistance);
            transform.position = point - dir * distance;
        }
    }
}
