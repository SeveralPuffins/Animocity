using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DrillController : MonoBehaviour
{
    [Header("Control variables")]
    public float drillBitSpeed = 200f;
    public float baseMotorSpeed = 30;

    [Header("Prefab properties")]
    public Transform lineRendererBase;
    public Vector3 baseOffset;
    public Transform drillLRTarget;
    public Rigidbody2D drillHead;
    public SliderJoint2D drillSlider;
    public Transform drillHeadMesh;
    public LineRenderer lineRenderer;

    public bool Deployed { get; private set; } = false;
    public bool Powered { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.widthMultiplier = 10f;
        lineRenderer.positionCount = 2;
    }

    private void UpdateAnimations()
    {
        if(Deployed && Powered) drillHeadMesh.Rotate(0, 0, drillBitSpeed * Time.deltaTime);

        var start = lineRendererBase.localToWorldMatrix.MultiplyPoint(baseOffset);
        var end = drillLRTarget.position;

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimations();
        if(Input.GetKeyDown(KeyCode.LeftAlt)) 
        {
            SwitchMotor();
        }
        UpdateMotorSpeed();
    }

    private void UpdateMotorSpeed()
    {
        if (Powered)
        {
            if (Deployed)
            {
                SetMotorSpeed(-baseMotorSpeed);
            }
            else
            {
                SetMotorSpeed(baseMotorSpeed);
            }
        }
        else
        {
            SetMotorSpeed(4f);
        }
    }

    private void SetMotorSpeed(float speed)
    {
        var motor = drillSlider.motor;
        motor.motorSpeed = speed;
        drillSlider.motor = motor;
        //MonoBehaviour.print($"Passed in new speed {speed}- speed is now {drillSlider.motor.motorSpeed}");
    }

    private void SwitchMotor()
    {
        Deployed = !Deployed;
    }

    internal void SetMaxExtension(float maxExtensionDist)
    {
        var limits = drillSlider.limits;
        limits.min = -maxExtensionDist;
        drillSlider.limits = limits;
    }
}
