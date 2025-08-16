using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxingGloveNew : MonoBehaviour
{
    public Transform singlePart;
    public Transform boxingGlove;
    [SerializeField] private AnimationCurve bounceBack;
    public float partWidth = 0.2f;
    public float partHeight = 1.415f;
    public float initialAngle = 0f;
    public float extendedAngle = 65f;
    public int totalDiamonds = 2;

    private bool stuckInside = false;

    //private Transform t1;
    //private Transform t2;
    //private Transform b1;
    //private Transform b2;

    private List<Transform> allDiamonds;



    private float t = 0;

    public float extendSpeed;
    public float retractSpeed;

    private bool isMoving = false;

    private void Start()
    {
        allDiamonds = new List<Transform>();
        Transform parent;
        bool first = true;

        for (int i = 0; i < totalDiamonds; i++)
        {
            if(i == 0)
            {
                parent = this.transform;
                
            }
            else
            { 
                parent = allDiamonds[i - 1].GetChild(0);
                first = false;
            }
            allDiamonds.Add(CreateDiamond(partHeight, parent, first));
            boxingGlove.SetParent(allDiamonds[allDiamonds.Count-1].GetChild(0), true);


        }
        MoveParts(); //set up initial angles
    }

    private Transform CreateDiamond(float offset, Transform parent, bool first)
    {
        Transform t1 = Instantiate(singlePart, parent);
        Transform t2 = Instantiate(singlePart, t1);
        Transform b1 = Instantiate(singlePart, t1);
        Transform b2 = Instantiate(singlePart, b1);

        if (first)
        {
            t1.transform.localPosition += new Vector3(0, 0, 0);
        }
        else
        {
            t1.transform.localPosition += new Vector3(-partWidth, offset, 0);
        }

        t2.transform.localScale = new Vector3(1, -1, 1);
        t2.transform.localPosition = new Vector3(partWidth, partHeight, 0);

        b1.transform.localScale = new Vector3(1, -1, 1);
        b1.transform.localPosition = new Vector3(partWidth, 0, 0);

        b2.transform.localScale = new Vector3(1, -1, 1);
        b2.transform.localPosition = new Vector3(partWidth * 2, partHeight, 0);

        return t1;
    }

    private void Update()
    {
        if(isMoving)
        {
            MoveParts();
        }
    }

    private void MoveParts()
    {
        float a = Mathf.Lerp(initialAngle, extendedAngle, t);

        for (int i = 0; i < allDiamonds.Count; i++)
        {
            Transform t1 = allDiamonds[i];
            Transform t2 = t1.GetChild(0);
            Transform b1 = t1.GetChild(1);
            Transform b2 = b1.GetChild(0);

            if(i == 0)
            { 
                t1.transform.localEulerAngles = new Vector3(a, 0, 0); 
            }
            else
            {
                t1.transform.localEulerAngles = new Vector3(0, 0, 0);
            }


            t2.transform.localEulerAngles = new Vector3(-a * 2, 0, 0);
            b1.transform.localEulerAngles = new Vector3(-a * 2, 0, 0);
            b2.transform.localEulerAngles = new Vector3(-a * 2, 0, 0);
        }
        boxingGlove.transform.localEulerAngles = new Vector3(-a, boxingGlove.transform.localEulerAngles.y, boxingGlove.transform.localEulerAngles.z);
    }

    public void Extend()
    {
        StartCoroutine(IncreaseTValue(t, 1, extendSpeed));
    }

    public void RetractFromFull()
    {
        StartCoroutine(IncreaseTValue(t, 0, retractSpeed));
    }

    IEnumerator IncreaseTValue (float a, float b, float s)
    {
        
        isMoving = true;
        if (a < b)
        {
            while (a < b)
            {
                a += Time.deltaTime * s;
                t = a;
                yield return new WaitForEndOfFrame();
            }
            DoBounce(true);

            
        }
        else
        {
            while (a > b)
            {
                a -= Time.deltaTime * s;
                t = a;
                yield return new WaitForEndOfFrame();
            }
            isMoving = false;
        }
    }

    private void DoBounce(bool positive)
    {
        StartCoroutine(Bounce(2f));

    }

    IEnumerator Bounce(float speed)
    {
        isMoving = true;
        float t1 = t;
        float t2 = 0;
        while(t2<1)
        {
            t2 += Time.deltaTime*speed;
       
            
                t = t1 * EvaluateCurve(bounceBack, t2);
            
            
            yield return new WaitForEndOfFrame();
        }
        isMoving = false;
    }

    private float EvaluateCurve(AnimationCurve curve, float pos)
    {
        return curve.Evaluate(pos);
    }

    public void HitACollider()
    {
        StopAllCoroutines();
        isMoving = false;
        DoBounce(true);

    }


    
}
