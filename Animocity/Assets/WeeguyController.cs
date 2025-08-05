using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WeeguyController : MonoBehaviour
{
    private float moveSpeed;
    private Animator animController;

    private Vector3 myTarget;
    private NavMeshAgent navMeshAgent;
    // Start is called before the first frame update
    void Start()
    {
        animController = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.angularSpeed = 1000;
        GetNewTarget();
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed = navMeshAgent.velocity.magnitude;
        animController.SetFloat("Blend", moveSpeed);
        if(Vector3.Distance(myTarget, transform.position)<0.1f)
        {
            GetNewTarget();
        }
    }

    void GetNewTarget()
    {
        myTarget = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
        navMeshAgent.SetDestination(myTarget);
        
    }


}
