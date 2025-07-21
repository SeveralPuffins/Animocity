using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerations : MonoBehaviour
{
    public Transform initialTerrain;
    public bool moving = true;
    private Transform currentTerrain;
    public float speed = 1;
    public float secondsPerWalkCycle = 2f;
    private float moveDelay;
    private float timeToMove = 0.2f;

    private void Start()
    {
        moveDelay = secondsPerWalkCycle / 4;
        currentTerrain = initialTerrain;
        StartCoroutine(walkJerk());
    }
    private void Update()
    {
       
    }

    private void moveTerrain()
    {
       currentTerrain.transform.Translate(0, 0, -speed*Time.deltaTime);
    }

    IEnumerator walkJerk()
    {
        while(moving)
        {
            yield return new WaitForSeconds(moveDelay);
            StartCoroutine(doJerkMove());
            currentTerrain.transform.Translate(0, 0, -speed * Time.deltaTime);
        }
    }

    IEnumerator doJerkMove()
    {
        float t = 0;
        while (t < timeToMove)
        {
            yield return new WaitForEndOfFrame();
            currentTerrain.transform.Translate(0, 0, -speed * Time.deltaTime);
            t += Time.deltaTime;

        }
    }

    
}
