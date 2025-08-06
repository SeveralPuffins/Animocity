using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weeguyspawner : MonoBehaviour
{
    public float numberOfWeeGuys;
    public GameObject weeGuyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfWeeGuys; i++)
        {
            GameObject thisWeeGuy = Instantiate(weeGuyPrefab);
            thisWeeGuy.transform.SetParent(this.transform);
            thisWeeGuy.transform.position = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));

        }   
    }

    
}
