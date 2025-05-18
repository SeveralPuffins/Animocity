using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footik : MonoBehaviour
{
    private Quaternion initialRot;
    public Transform body;

    public float stepDistance;
    public float footSpacing;
    public float footOffset;
    public float speed;
    public float stepHeight;
    public float bodyLean;

    public footik otherFoot;

    private Vector3 newPosition;
    private Vector3 currentPosition;
    private Vector3 oldPosition;
    public float lerp = 0;
    private bool isMoving = false;
    private bool finishedMove = false;
    void Start()
    {
        initialRot = transform.rotation;
        currentPosition = transform.position;
        oldPosition = currentPosition;
        lerp = 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = currentPosition;

        RaycastHit hit;
        Ray ray = new Ray(body.position + (body.right * footSpacing) + (body.forward * bodyLean) + (body.forward * footOffset), Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))

        {
            if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.isMoving)
            {

                lerp = 0;
                newPosition = hit.point;
            }
        }

        if(lerp<=1 && !otherFoot.IsMoving())
        {
            finishedMove = false;
            isMoving = true;
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            currentPosition = footPosition;
            lerp += Time.deltaTime * speed;
        }

        if(lerp>=1 && isMoving)
        {
            oldPosition = newPosition;
            isMoving = false;
            finishedMove = true;
            

        }
       

        if(finishedMove == true)
        {
           // footOffset = -footOffset;
            finishedMove = false;
        }


    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
