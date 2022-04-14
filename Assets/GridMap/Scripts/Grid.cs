using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using GridTools;
using Unity.Mathematics;
using UnityEditor.UI;
using UnityEngine;

public class Grid
{
    public int Width { get; }
    public int Height { get; }
    private readonly float cellSize;
    private readonly GridCell[,] gridArray;

    private enum GridCell{
        Walkable,
        Wall
    }

    public Grid(int width, int height, float cellSize)
    {
        this.Width = width;
        this.Height = height;
        this.cellSize = cellSize;
        gridArray = new GridCell[width, height];

        for (var i = 0; i < height - 10; i++)
            gridArray[10, i] = GridCell.Wall;

        for (var x = 0; x < gridArray.GetLength(0); x++)
            for (var y = 0; y < gridArray.GetLength(1); y++)
                DrawCell(new int2(x, y), gridArray[x,y] == GridCell.Walkable ? Color.white : Color.red);
    }

    private Vector3 GetWorldPosition(int x, int y)
        => new Vector3(x, y) * cellSize;

    private bool IsInGrid(int x, int y)
        => x >= 0 && y >= 0 && x < gridArray.GetLength(0) && y < gridArray.GetLength(1);

    public int2 GetGridPosition(Vector3 position)
        => new int2(Mathf.FloorToInt(position.x / cellSize), Mathf.FloorToInt(position.y / cellSize));

    private void DrawCell(int2 position, Color color, float duration=100f)
    {
        Debug.DrawLine(new Vector3(position.x, position.y), new Vector3(position.x + cellSize, position.y), color, duration);
        Debug.DrawLine(new Vector3(position.x, position.y), new Vector3(position.x, position.y + cellSize), color, duration);
        Debug.DrawLine(new Vector3(position.x + cellSize, position.y + cellSize), new Vector3(position.x + cellSize, position.y), color, duration);
        Debug.DrawLine(new Vector3(position.x + cellSize, position.y + cellSize), new Vector3(position.x, position.y + cellSize), color, duration);
    }

    public void FillCell(Vector3 worldPosition)
    {
        var cellPosition = GetGridPosition(worldPosition);

        if (IsInGrid(cellPosition.x, cellPosition.y))
            DrawCell(cellPosition, Color.green, 1);
    }

    public void UnFillCell(Vector3 worldPosition)
    {
        var cellPosition = GetGridPosition(worldPosition);
        //if (IsInGrid(cellPosition.x, cellPosition.y))
        //    DrawCell(cellPosition, Color.white);
    }

    public void DrawPath(Vector3 start, Vector3 end)
    {
        var startPosition = GetGridPosition(start);
        var endPosition = GetGridPosition(end);

        var pathFinder = new PathFinding();
        var path = pathFinder.FindPathAStar(this, startPosition, endPosition);

        if (path == null) return;
        for (var i = 0; i < path.Count - 1; i++) 
            Debug.DrawLine(new Vector3(path[i].x, path[i].y) + new Vector3(cellSize / 2, cellSize / 2), new Vector3(path[i+1].x, path[i+1].y) + new Vector3(cellSize / 2, cellSize / 2), Color.green, 2f);
    }

    public bool isWalkable(int x, int y)
        => gridArray[x, y] == GridCell.Walkable;
}
