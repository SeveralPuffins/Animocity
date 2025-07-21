using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwitcher : MonoBehaviour
{
    public Transform lightContainers;
    private List<Transform> lights = new List<Transform>();
    public float percentOfLightsToTurnOn = 50;

    private void Start()
    {

        for (int i = 0; i < lightContainers.childCount; i++)
        {
            lights.Add(lightContainers.GetChild(i).GetComponentInChildren<Light>().gameObject.transform);
        }

        for (int i = 0; i < lights.Count; i++)
        {
           
                lights[i].gameObject.SetActive(false);
            
        }

        TurnOnThisPercentOfLights(percentOfLightsToTurnOn);
    }

    private void TurnOnThisPercentOfLights(float x)
    {
        float percent = x / 100;
        for (int i = 0; i < lights.Count; i++)
        {
            if(Random.value < percent)
            {
                lights[i].gameObject.SetActive(true);
            }
        }
    }
}
