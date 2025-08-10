using Animocity.Cities;
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
    private Building _building;
    // Start is called before the first frame update
    void Start()
    {
        _building = transform.parent.GetComponent<Building>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_building.IsPlan)
        {
            horizontalPart.localEulerAngles = new Vector3(0, 0, bearing);
            blades.Rotate(bladeSpeed * Time.deltaTime, 0, 0, Space.Self);
        }
    }
}
