using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private readonly float cellSize;
    private readonly GridCell[,] gridArray;

    enum GridCell{
        empty,
        player
    }

    public Grid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new GridCell[width, height];

        for (var x = 0; x < gridArray.GetLength(0); x++)
            for (var y = 0; y < gridArray.GetLength(1); y++)
                DrawCell(new int2(x, y), Color.white);
    }

    private Vector3 GetWorldPosition(int x, int y)
        => new Vector3(x, y) * cellSize;

    private bool IsInGrid(int x, int y)
        => x >= 0 && y >= 0 && x < gridArray.GetLength(0) && y < gridArray.GetLength(1);

    public int2 GetGridPosition(Vector3 position)
        => new int2(Mathf.FloorToInt(position.x / cellSize), Mathf.FloorToInt(position.y / cellSize));

    private void DrawCell(int2 position, Color color)
    {
        Debug.DrawLine(new Vector3(position.x, position.y), new Vector3(position.x + cellSize, position.y), color, 100f);
        Debug.DrawLine(new Vector3(position.x, position.y), new Vector3(position.x, position.y + cellSize), color, 100f);
        Debug.DrawLine(new Vector3(position.x + cellSize, position.y + cellSize), new Vector3(position.x + cellSize, position.y), color, 100f);
        Debug.DrawLine(new Vector3(position.x + cellSize, position.y + cellSize), new Vector3(position.x, position.y + cellSize), color, 100f);
    }

    public void FillCell(Vector3 worldPosition)
    {
        var cellPosition = GetGridPosition(worldPosition);

        if (IsInGrid(cellPosition.x, cellPosition.y))
            DrawCell(cellPosition, Color.green);
    }

    public void UnFillCell(Vector3 worldPosition)
    {
        var cellPosition = GetGridPosition(worldPosition);
        if (IsInGrid(cellPosition.x, cellPosition.y))
            DrawCell(cellPosition, Color.white);
    }
}
