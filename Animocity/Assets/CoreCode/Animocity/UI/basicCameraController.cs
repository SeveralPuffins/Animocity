using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Animocity.Cities;
using System;
using System.Linq;
using BlueprintSystem;
using Animocity.Cities.CityGen;
using static Unity.Burst.Intrinsics.X86.Avx;

public class basicCameraController : MonoBehaviour
{
    public float baseSpeed = 5f;
    public Rect maxBounds=  new Rect(-20,20,20,20);
    public float maxZoom = -12f;
    public float minZoom = -160f;
    Camera camera;

    private Dictionary<CityGrid, Vector3> cameraPositionForGrid;

    private bool _firstGen = true;

    // Start is called before the first frame update
    void Awake()
    {
        cameraPositionForGrid = new();
        
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        ChangeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (ChangingGrid())
        {
            ChangeGrid();
        }
        else 
        { 
            PlayerMoveCamera();
        }
    }

    private bool ChangingGrid()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            CityOverview.Current.CityMultiGrid.FocusPrevious();
            return true;
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            CityOverview.Current.CityMultiGrid.FocusNext();
            return true;
        }
        return false;
    }
    private void SaveOldGridPosition()
    {
        var oldGrid = transform.parent.GetComponent<CityGrid>();
        if(oldGrid != null)
        {
            this.cameraPositionForGrid[oldGrid] = transform.localPosition;
        }
    }

    private void ChangeGrid()
    {
        SaveOldGridPosition();

        float currentZoom = transform.position.z;

        var newGrid = CityOverview.Current.CityMultiGrid.FocusedGrid;
        transform.SetParent(newGrid.transform, false);
        transform.localPosition = GetDefaultPosition();

        Vector3 min = newGrid.WorldFromCell(newGrid.TileBounds.min-Vector2Int.one);
        Vector3 max = newGrid.WorldFromCell(newGrid.TileBounds.max+Vector2Int.one);

        MonoBehaviour.print($"Min: {min}, max: {max}");

        Vector2 lmin = this.transform.InverseTransformPoint(min);
        Vector2 lmax = this.transform.InverseTransformPoint(max);

        this.maxBounds = new Rect(lmin, lmax-lmin);

        if (this.cameraPositionForGrid.TryGetValue(newGrid, out var position))
        {
            transform.localPosition = position;
        }

    
        UpdateClipping();
    }

    private void PlayerMoveCamera()
    {
        Vector2 move = new Vector2();
        float zoom = 0;

        float speed = baseSpeed * Mathf.Abs(transform.localPosition.z) * 0.1f;

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

    public Vector3 GetDefaultPosition()
    {
        return new Vector3
            (
                0,0, maxZoom-20f
            ); 
    }

    private void UpdateClipping()
    {
        camera.nearClipPlane = (transform.parent.position.z - transform.position.z)-3.5f;
    }
}
