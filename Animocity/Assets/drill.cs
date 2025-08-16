using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drill : MonoBehaviour
{
    public Transform attachmentPoint;
    public Transform arm2;
    public Transform arm3;
    public Transform arm4;
    public Transform drillBit;

    public float arm4max;
    public float arm3max;
    public float arm2max;

    public float arm2min;
    public float arm3min;
    public float arm4min;

    public float extendSpeed = 1;

    //private float distancetoGround;
    private bool extending = false;
    private bool extended = false;

    public float drillBitSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        drillBit.Rotate(0, 0, drillBitSpeed * Time.deltaTime);
        //RaycastHit hit;
        //Vector3 groundPoint = Vector3.zero ;

        //if (Physics.Raycast(drillBit.position, drillBit.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        //{
        //    groundPoint = hit.point;
        //}

        if(!extending && Input.GetKeyDown(KeyCode.LeftAlt)) 
        {
            if (extended)
            {
                Retract();
            }
            else
            {
                Extend();
            }
        }
    }

    public void Extend()
    {
        extending = true;
        StartCoroutine(Extend2());
    }

    public void Retract()
    {
        extending = true;
        StartCoroutine(Retract2());
    }

    IEnumerator Extend2()
    {
        float arm2current = arm2.localPosition.z;
        while (arm2current < arm2max)
        {
            arm2current += Time.deltaTime * extendSpeed;
            arm2.transform.localPosition = new Vector3(arm2.localPosition.x, arm2.localPosition.y, arm2current);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Extend3());
    }

    IEnumerator Extend3()
    {
        float arm3current = arm3.localPosition.z;
        while (arm3current < arm3max)
        {
            arm3current += Time.deltaTime * extendSpeed;
            arm3.transform.localPosition = new Vector3(arm3.localPosition.x, arm3.localPosition.y, arm3current);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Extend4());
    }

    IEnumerator Extend4()
    {
        float arm4current = arm4.localPosition.z;
        while (arm4current < arm4max)
        {
            arm4current += Time.deltaTime * extendSpeed;
            arm4.transform.localPosition = new Vector3(arm4.localPosition.x, arm4.localPosition.y, arm4current);
            yield return new WaitForEndOfFrame();
        }
        extending = false;
    }

    IEnumerator Retract2()
    {
        float arm2current = arm2.localPosition.z;
        while (arm2current > arm2min)
        {
            arm2current -= Time.deltaTime * extendSpeed;
            arm2.transform.localPosition = new Vector3(arm2.localPosition.x, arm2.localPosition.y, arm2current);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Retract3());
    }

    IEnumerator Retract3()
    {
        float arm3current = arm3.localPosition.z;
        while (arm3current > arm3min)
        {
            arm3current -= Time.deltaTime * extendSpeed;
            arm3.transform.localPosition = new Vector3(arm3.localPosition.x, arm3.localPosition.y, arm3current);
            yield return new WaitForEndOfFrame();
        }
        StartCoroutine(Retract4());
    }

    IEnumerator Retract4()
    {
        float arm4current = arm4.localPosition.z;
        while (arm4current > arm4min)
        {
            arm4current -= Time.deltaTime * extendSpeed;
            arm4.transform.localPosition = new Vector3(arm4.localPosition.x, arm4.localPosition.y, arm4current);
            yield return new WaitForEndOfFrame();
        }
        extending = false;
    }

}
