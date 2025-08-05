using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaiju : MonoBehaviour
{
    public Animator animatorController;
    // Start is called before the first frame update
    void Start()
    {
        animatorController = transform.GetComponentInChildren<Animator>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Slap()
    {
        animatorController.SetTrigger("slap");
    }

    public void Taunt()
    {
        animatorController.SetTrigger("taunt");
    }
}
