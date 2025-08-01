using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animocity.Cities;
using System;
using System.Linq;

public class basicCameraController : MonoBehaviour
{
    private int _gridIndex;
    public List<CityGrid> grids;
    public float baseSpeed = 5f;
    public Rect maxBounds=  new Rect(-20,20,20,20);
    public float maxZoom = -16f;
    public float minZoom = -160f;
    Camera camera;

    // Start is called before the first frame update
    void Awake()
    {
        camera = GetComponent<Camera>();
        _gridIndex = 0;
        ChangeGrid(_gridIndex, _gridIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ChangingGrid())
        { 
            PlayerMoveCamera();
        }
    }

    private bool ChangingGrid()
    {
        int newGridIndex = _gridIndex;
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            newGridIndex =  (_gridIndex + grids.Count - 1) % grids.Count;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            newGridIndex = (_gridIndex + grids.Count + 1) % grids.Count;
        }
        if (newGridIndex != _gridIndex)
        {
            ChangeGrid(_gridIndex, newGridIndex);
            return true;
        }

        return false;
    }

    public void ChangeGrid(int oldGridIndex, int newGridIndex)
    {

        grids[oldGridIndex].Unfocus();
        grids[newGridIndex].Focus();

        _gridIndex = newGridIndex;
        CityGrid newTargetGrid = grids[newGridIndex];

        var clp = transform.localPosition;

        transform.SetParent(newTargetGrid.transform, false);
        transform.localPosition = clp;
    }

    private void PlayerMoveCamera()
    {
        Vector2 move = new Vector2();
        float zoom = 0;

        float speed = baseSpeed * Mathf.Abs(transform.position.z) * 0.1f;

        if (Input.GetKey(KeyCode.W)) move += speed * Vector2.up;
        if (Input.GetKey(KeyCode.S)) move += speed * Vector2.down;
        if (Input.GetKey(KeyCode.A)) move += speed * Vector2.left;
        if (Input.GetKey(KeyCode.D)) move += speed * Vector2.right;

        zoom = Input.mouseScrollDelta.y * speed * 12f;

        var target = transform.localPosition + ((Vector3)move + Vector3.forward * zoom) * Time.deltaTime;

        if (InBounds(target))
        {
            transform.localPosition = target;
            UpdateClipping();
        }
    }

    private bool InBounds(Vector3 target)
    {
        bool inPsn =
                   target.x > maxBounds.xMin && target.x < maxBounds.xMax
                && target.y > maxBounds.yMin && target.y < maxBounds.yMax
                && target.z > minZoom && target.z < maxZoom;

        return inPsn;
    }

    private void UpdateClipping()
    {
        camera.nearClipPlane = (transform.parent.position.z - transform.position.z)-3.5f;
    }
}
