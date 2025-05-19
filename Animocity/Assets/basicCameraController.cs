using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicCameraController : MonoBehaviour
{
    public float baseSpeed = 5f;
    public Rect maxBounds=  new Rect(-20,20,20,20);
    public float maxZoom = -16f;
    public float minZoom = -160f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = new Vector2();
        float zoom = 0;

        float speed = baseSpeed * Mathf.Abs(transform.position.z) * 0.1f;

        if (Input.GetKey(KeyCode.W)) move += speed * Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += speed * Vector2.down;
        if (Input.GetKey(KeyCode.A)) move += speed * Vector2.left;
        if (Input.GetKey(KeyCode.D)) move += speed * Vector2.right;

        zoom = Input.mouseScrollDelta.y*speed*12f;

        var target = transform.localPosition + ((Vector3)move + Vector3.forward * zoom) * Time.deltaTime;

        if(target.x > maxBounds.xMin && target.x < maxBounds.xMax && target.y > maxBounds.yMin && target.y < maxBounds.yMax && target.z > minZoom && target.z < maxZoom)
        {
            transform.localPosition = target;
        }
    }
}
