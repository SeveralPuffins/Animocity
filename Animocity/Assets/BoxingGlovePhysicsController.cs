using GogoGaga.OptimizedRopesAndCables;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class BoxingGlovePhysicsController : MonoBehaviour
{
    public Rope ropeRenderer;
    public DistanceJoint2D distanceJoint;
    public Rigidbody2D glove;

    public float maxDistance = 120f;
    public float minDistance = 0.2f;
    public float launchImpulse = 800f;
    public float recoverySpeed = 20f;

    public float currentDistance;
    Quaternion baseLocalRotation;
    Vector3 baseLocalPosition;

    public bool Ready { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        baseLocalRotation = glove.transform.localRotation;
        baseLocalPosition = glove.transform.localPosition;

        currentDistance = minDistance;
        ropeRenderer.ropeLength = currentDistance * 1.01f;
        distanceJoint.distance = currentDistance;
    }

    // Update is called once per frame
    void Update()
    {
        CheckRealDistance();
        if (distanceJoint.enabled)
        {
            UpdateDistances();
        }
    }

    private void CheckRealDistance()
    {
        if((!distanceJoint.enabled) &&(glove.transform.localPosition - baseLocalPosition).sqrMagnitude > 0.95 * maxDistance)
        {
            distanceJoint.enabled = true;
        }
    }

    private void LaunchGlove()
    {
        currentDistance = maxDistance+recoverySpeed;
        UpdateDistances();
        glove.isKinematic = false;
        glove.velocity = new Vector2(launchImpulse, 0);
        Ready = false;
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Ready)
        {
            LaunchGlove();
        }
    }

    private void ResetGlove()
    {
        glove.transform.localPosition = baseLocalPosition;
        glove.transform.localRotation = baseLocalRotation;
        glove.velocity = Vector2.zero;
        glove.angularVelocity = 0f;
        glove.isKinematic = true;
        distanceJoint.enabled = false;
        Ready = true;
    }


    private void UpdateDistances()
    {
        if(currentDistance > minDistance)
        {
            currentDistance -= Time.deltaTime * recoverySpeed;
            currentDistance = Mathf.Max(currentDistance, minDistance);

            ropeRenderer.ropeLength = currentDistance * 1.01f;
            distanceJoint.distance = currentDistance;
        }
        else 
        {
            if ((glove.transform.localPosition - baseLocalPosition).sqrMagnitude < 2.0 * minDistance)
            {
                ResetGlove();
            }
        }
    }
}
