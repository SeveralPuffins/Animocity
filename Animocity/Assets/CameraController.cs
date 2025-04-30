using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public float zoomSpeed;
    public float rotateSpeed;

    public float distance;
    public float height;


    private void Start()
    {
        transform.position = target.position;
        cam.transform.localPosition = new Vector3(0, height, -distance);

    }
    private void Update()
    {
        transform.position = target.position;
        MoveCamera();
    }

    private void MoveCamera()
    {
        float MouseY = Input.GetAxis("Mouse Y");
        float MouseX = Input.GetAxis("Mouse X");

        transform.RotateAround(Vector3.zero, Vector3.up, MouseX);




    }
}
