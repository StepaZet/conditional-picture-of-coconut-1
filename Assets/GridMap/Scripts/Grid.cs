using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private GridCell[,] gridArray;

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
        {
            for (var y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize;
    }

    private void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }

    private bool IsInGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < gridArray.GetLength(0) && y < gridArray.GetLength(1);
    }

    public void FillCell(Vector3 worldPosition)
    {
        var x = 0;
        var y = 0;
        GetXY(worldPosition, out x, out y);

        if (IsInGrid(x, y))
        {
            gridArray[x, y] = GridCell.player;
            var rect = new Rect(x, y, cellSize, cellSize);
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.green, 100f);
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.green, 100f);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.green, 100f);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.green, 100f);
        }      
    }

    public void UnFillCell(Vector3 worldPosition)
    {
        var x = 0;
        var y = 0;
        GetXY(worldPosition, out x, out y);
        if (IsInGrid(x, y))
        {
            gridArray[x, y] = GridCell.player;
            var rect = new Rect(x, y, cellSize, cellSize);
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.white, 100f);
            Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.white, 100f);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.white, 100f);
            Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.white, 100f);
        }
    }
}
