using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class basicCameraController : MonoBehaviour
{
    public float baseSpeed = 5f;
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

        transform.position += ((Vector3)move + Vector3.forward * zoom) * Time.deltaTime;
    }
}
