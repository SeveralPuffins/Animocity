using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollWorld : MonoBehaviour
{
    public float bob = 1f;
    public float speed = 3f;
    private float z = 0;
    public float stride = 64f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        z += Time.deltaTime;
        
        float theta = 2*Mathf.PI*z/stride;

        float x = (float)Mathf.Cos(theta);
        float y = (float)Mathf.Sin(theta);

        x = speed * x * x;
        y = bob * y * y;

        foreach(Transform t in transform)
        {
            t.localPosition = new Vector3(t.localPosition.x-x, y, t.localPosition.z);
        }
    }
}
