using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windmill : MonoBehaviour
{
    public Transform blades;
    public Transform horizontalPart;

    public float bladeSpeed;
    public float bearing;
    public float horizontalRotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalPart.localEulerAngles = new Vector3(0, 0, bearing);
        blades.Rotate(bladeSpeed * Time.deltaTime, 0, 0, Space.Self);
    }
}
