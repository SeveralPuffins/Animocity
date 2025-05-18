using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simplemover : MonoBehaviour
{
    private float t = 0;
    public bool bob = false;
    public float bobAmplitude = 1;
    private Vector3 initpos;

    private void Start()
    {
        initpos = transform.position;
    }
    private void Update()
    {
        t += Time.deltaTime;
        if (bob) bobupanddown();
        
    }

    private void bobupanddown()
    {
        transform.position = new Vector3(0, Mathf.Sin(t) * bobAmplitude, 0) + initpos;
    }
}
