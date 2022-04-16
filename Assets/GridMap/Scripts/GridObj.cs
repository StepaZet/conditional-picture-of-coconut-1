using System.Collections;
using System.Collections.Generic;
using GridTools;
using Unity.Mathematics;
using UnityEngine;

public class GridObj : MonoBehaviour
{
    public Grid Grid { get; private set; }
    public Vector3 PlayerPosition { get; set; }

    public void OnEnable()
    {
        var startPosition = new float2(-10, -10);
        const int width = 100;
        const int height = 100;
        const float cellSize = 0.5f;

        Grid = new Grid(startPosition, width, height, cellSize);
        Grid.DrawGrid();
    }

    private void Update()
    {
        Debug.Log(PlayerPosition);

        if (Input.GetMouseButtonDown(0))
            DrawPath(PlayerPosition, Tools.GetMouseWordPosition());

        if (Input.GetMouseButtonDown(1))
            CreateWall(Tools.GetMouseWordPosition());
    }

    public int2 GetGridPosition(Vector3 worldPosition)
        => Grid.WorldToGridPosition(worldPosition);

    public void DrawPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        => Grid.DrawPath(startWorldPosition, endWorldPosition);

    public void CreateWall(Vector3 worldPosition)
        => Grid.CreateWall(worldPosition);

    public void FillCell(int2 gridPosition)
        => Grid.FillCell(gridPosition);

    public void UnFillCell(int2 gridPosition)
        => Grid.UnFillCell(gridPosition);

    public void CreateWall(Vector3 startWorldWall, Vector3 endWorldWall)
        => Grid.CreateWall(startWorldWall, endWorldWall);
}
