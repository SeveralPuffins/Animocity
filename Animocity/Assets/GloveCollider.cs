using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloveCollider : MonoBehaviour
{
    public BoxingGloveNew boxingGlove;

    private void Start()
    {
        boxingGlove = transform.GetComponentInParent<BoxingGloveNew>();
    }


    //private void OnTriggerEnter(Collider other)
    //{

    //    boxingGlove.HitACollider();
    //}

    private void OnCollisionEnter(Collision collision)
    {
        boxingGlove.HitACollider();
    }


}
