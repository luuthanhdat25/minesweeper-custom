using System;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    public int width = 10, height = 10;
    public float cellSize = 5f;
    private MyGrid<bool> myGrid;

    private void Start()
    {
        myGrid = new MyGrid<bool>(width, height, cellSize, transform.position);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(myGrid.GetValue(GetMouseWorldPosition()));
        }
    }

    public Vector3 GetMouseWorldPosition() {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        Debug.Log($"MousePos: {Input.mousePosition}, ScreenToPoint: {vec}");
        return vec;
    }
    
    public Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
}