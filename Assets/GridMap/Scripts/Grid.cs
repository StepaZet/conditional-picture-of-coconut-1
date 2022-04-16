using Assets;
using GridTools;
using Unity.Mathematics;
using UnityEngine;

public class Grid
{
    public readonly int Width;
    public readonly int Height;
    public readonly float CellSize;
    private readonly float2 position;
    private readonly GridCell[,] gridArray;

    public enum GridCell{
        Walkable,
        Wall
    }

    public Grid(float2 position, int width, int height, float cellSize)
    {
        this.Width = width;
        this.Height = height;
        this.CellSize = cellSize;
        this.position = position;
        gridArray = new GridCell[width, height];
    }
    public bool isWalkable(int x, int y)
        => gridArray[x, y] == GridCell.Walkable;

    public float2 GridToWorldPosition(float2 gridPosition)
        => gridPosition * CellSize + position;

    public bool IsInGrid(int2 gridPosition)
        => gridPosition.x >= 0 && gridPosition.y >= 0 && 
           gridPosition.x < gridArray.GetLength(0) && 
           gridPosition.y < gridArray.GetLength(1);

    public int2 WorldToGridPosition(Vector3 worldPosition)
        => new int2(Mathf.FloorToInt((worldPosition.x - position.x) / CellSize), 
            Mathf.FloorToInt((worldPosition.y - position.y) / CellSize));

    public void CreateWall(Vector3 startWorldWall, Vector3 endWorldWall)
    {
        var startGridWall = WorldToGridPosition(startWorldWall);
        var endGridWall = WorldToGridPosition(endWorldWall);
        for (var x = startGridWall.x; x <= endGridWall.x; x++)
        {
            CreateWall(new int2(x, startGridWall.y));
            CreateWall(new int2(x, endGridWall.y));
        }
        for (var y = startGridWall.y; y <= endGridWall.y; y++)
        {
            CreateWall(new int2(startGridWall.x, y));
            CreateWall(new int2(endGridWall.x, y));
        }

    }

    public void CreateWall(int2 gridPosition)
    {
        if (!IsInGrid(gridPosition)) return;
        gridArray[gridPosition.x, gridPosition.y] = Grid.GridCell.Wall;
        DrawCell(gridPosition, Color.red);
    }

    public void CreateWall(Vector3 worldPosition)
    {
        var gridPosition = WorldToGridPosition(worldPosition);
        if (!IsInGrid(gridPosition)) return;
        gridArray[gridPosition.x, gridPosition.y] = Grid.GridCell.Wall;
        DrawCell(gridPosition, Color.red);
    }

    // visualization 

    public void DrawGrid()
    {
        for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
                DrawCell(new float2(x, y), gridArray[x, y] == GridCell.Walkable
                    ? Color.white : Color.red);
    }

    public void DrawCell(float2 gridPosition, Color color, float duration = 100f)
    {
        var worldPosition = GridToWorldPosition(gridPosition);
        Debug.DrawLine(worldPosition.ToVector3(),
            worldPosition.ToVector3() + new Vector3(CellSize, 0), color, duration);
        Debug.DrawLine(worldPosition.ToVector3(),
            worldPosition.ToVector3() + new Vector3(0, CellSize), color, duration);
        Debug.DrawLine(worldPosition.ToVector3() + new Vector3(CellSize, CellSize),
            worldPosition.ToVector3() + new Vector3(CellSize, 0), color, duration);
        Debug.DrawLine(worldPosition.ToVector3() + new Vector3(CellSize, CellSize),
            worldPosition.ToVector3() + new Vector3(0, CellSize), color, duration);
    }

    public void DrawPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        var startGridPosition = WorldToGridPosition(startWorldPosition);
        var endGridPosition = WorldToGridPosition(endWorldPosition);

        var pathFinder = new PathFinding();
        var path = pathFinder.FindPathAStar(this, startGridPosition, endGridPosition);

        if (path == null) return;
        for (var i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(
                GridToWorldPosition(path[i]).ToVector3() + new Vector3(CellSize / 2, CellSize / 2), 
                GridToWorldPosition(path[i+1]).ToVector3() + new Vector3(CellSize / 2, CellSize / 2), Color.green, 2f);
        }
            
    }
    public void FillCell(int2 gridPosition)
    {
        if (IsInGrid(gridPosition))
            DrawCell(gridPosition, Color.green, 100f);
    }

    public void UnFillCell(int2 gridPosition)
    {
        if (IsInGrid(gridPosition))
            DrawCell(gridPosition, gridArray[gridPosition.x, gridPosition.y] == GridCell.Walkable
                ? Color.white : Color.red);
    }
}
