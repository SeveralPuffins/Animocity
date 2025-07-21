using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MechController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public Transform goal;

    private Vector3 initialLocalPosition;
    private float bobTimer = 0f;
    public Transform rootBone;
    public float bobFrequency = 1.5f;
    public float bobAmplitude = 0.1f; 
    public float bobSpeedScale = 0.2f; 



    private float t;

    

    private void Start()
    {
        t = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = goal.position;
        initialLocalPosition = rootBone.localPosition;


    }

    private void Update()
    {
        
        if(navMeshAgent.remainingDistance <0.2f)
        {
            navMeshAgent.destination = goal.position;
        }

        DoBob();
        

    }



    private void DoBob()
    {

        float speed = navMeshAgent.velocity.magnitude;
        if (speed > 0.01f)
        {
            bobTimer += Time.deltaTime * bobFrequency * (1.0f + speed * bobSpeedScale);
            float offset = Mathf.Sin(bobTimer) * bobAmplitude;
            rootBone.localPosition = initialLocalPosition + new Vector3(0f, offset, 0f);
        }
        else
        {

            bobTimer = 0f;
            //rootBone.localPosition = initialLocalPosition;

            StartCoroutine(ResetBone(rootBone.localPosition));
        }

    }

    IEnumerator ResetBone(Vector3 pos)
    {
        float t = 0;
        while(t<1)
        {
            t += Time.deltaTime;
            rootBone.localPosition = Vector3.Lerp(pos, initialLocalPosition, t);
            yield return new WaitForEndOfFrame();
        }
    }


}
