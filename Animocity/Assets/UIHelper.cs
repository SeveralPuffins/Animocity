using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIHelper : MonoBehaviour
{

    public Transform cameraContainer;
    public Transform cam;
    public GameObject removablePanel;
    public float rotateSpeed = 1;
    public Vector3 cameraRotatePos;
    public Vector3 cameraRotateEuler;
    public Vector3 cameraBuildEuler;
    public Vector3 cameraBuildPos;
    public Volume volume;
    private Vector3 inititalRotContainer;
    public Transform focusPoint;
    public GameObject rotateSpotlight;

    private DepthOfField depthOfField;
    private enum cameraMode { ROTATE, BUILD};
    cameraMode thisCameraMode = cameraMode.ROTATE;


    // Start is called before the first frame update
    void Start()
    {
        rotateSpotlight.SetActive(true);
        inititalRotContainer = cameraContainer.eulerAngles;
        ChangeToRotate();
        DepthOfField tempDof;
        if (volume.profile.TryGet<DepthOfField>(out tempDof))
        {
            depthOfField = tempDof;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(thisCameraMode == cameraMode.ROTATE)
        {
            cameraContainer.Rotate(new Vector3(0, Time.deltaTime * rotateSpeed, 0));
        }
       
        depthOfField.focusDistance.value = Vector3.Distance(cam.position, focusPoint.position);
    }

    public void ChangeToRotate()
    {
        rotateSpotlight.SetActive(true);
        cam.localPosition = cameraRotatePos;
        cam.localEulerAngles = cameraRotateEuler;
        thisCameraMode = cameraMode.ROTATE;
        removablePanel.SetActive(true);
    }

    public void ChangeToBuild()
    {
        rotateSpotlight.SetActive(false);
        cameraContainer.eulerAngles = inititalRotContainer;
        cam.localPosition = cameraBuildPos;
        cam.localEulerAngles = cameraBuildEuler;
        thisCameraMode = cameraMode.BUILD;
        removablePanel.SetActive(false);
    }
}
