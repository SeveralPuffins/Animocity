using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoxingGlove : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void FireGlove()
    {
        animator.SetTrigger("_punch");
    }
}
