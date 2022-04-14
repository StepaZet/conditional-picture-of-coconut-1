using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PathFinding : MonoBehaviour
{
    private const int MoveStraightCost = 10;
    private const int MoveDiagonalCost = 14;

    private void Start()
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        for (var i = 0; i < 5; i++)
            FindPath(new int2(0,0), new int2(19, 19));
        stopWatch.Stop();
        Debug.Log("Result: " + stopWatch.ElapsedMilliseconds);
    }

    private struct PathNode
    {
        public int X;
        public int Y;

        public int index;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;

        public int previousIndex;

        public void UpdateFCost()
            => fCost = gCost + hCost;
    }

    private static int GetIndex(int x, int y, int gridSize)
        => x + y * gridSize;

    private static int GetDistanceCost(int2 start, int2 end)
    {
        var xDistance = math.abs(start.x - end.x);
        var yDistance = math.abs(start.y - end.y);
        var remainingDistance = math.abs(xDistance - yDistance);
        return MoveDiagonalCost * math.min(xDistance, yDistance) 
               + MoveStraightCost * remainingDistance;
    }
    private static bool IsInsideGrid(int2 position, int2 gridSize)
        => position.x >= 0 && position.y >= 0 &&
           position.x < gridSize.x && position.y < gridSize.y;

    private int GetLowestCostFNodeIndex(SortedSet<int> openNodes, 
        NativeArray<PathNode> pathNodeArray)
    {
        var lowestCostNode = pathNodeArray[openNodes.Min];
        foreach (var testNode in openNodes
                     .Select(openNode => pathNodeArray[openNode])
                     .Where(testNode => testNode.fCost < lowestCostNode.fCost))
        {
            lowestCostNode = testNode;
        }

        return lowestCostNode.index;
    }

    private NativeArray<PathNode> GetPathNodeArray(int2 gridSize, int2 endPosition)
    {
        var pathNodeArray = new NativeArray<PathNode>(gridSize.x * gridSize.y, Allocator.Temp);

        for (var x = 0; x < gridSize.x; x++)
        {
            for (var y = 0; y < gridSize.y; y++)
            {
                var pathNode = new PathNode
                {
                    X = x,
                    Y = y,
                    index = GetIndex(x, y, gridSize.x),
                    gCost = int.MaxValue,
                    hCost = GetDistanceCost(new int2(x, y), endPosition),
                    fCost = int.MaxValue,
                    isWalkable = true,
                    previousIndex = -1
                };
                pathNodeArray[pathNode.index] = pathNode;
            }
        }

        return pathNodeArray;
    }

    private List<int2> GetNeighboringPositions()
    {
        var neighboringPosition = new List<int2>();
        for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
                if (dx != 0 || dy != 0)
                    neighboringPosition.Add(new int2(dx, dy));
        
        return neighboringPosition;
    }

    private void OpenNode(PathNode toOpenNode, int2 neighborNode, 
        int2 gridSize, ICollection<int> visited, NativeArray<PathNode> pathNodeArray, ISet<int> openNodes)
    {
        var nextToOpenPosition = new int2(toOpenNode.X + neighborNode.x, toOpenNode.Y + neighborNode.y);
        var toOpenPosition = new int2(toOpenNode.X, toOpenNode.Y);
        var nextToOpenIndex = GetIndex(nextToOpenPosition.x, nextToOpenPosition.y, gridSize.x);
        if (!IsInsideGrid(nextToOpenPosition, gridSize) || visited.Contains(nextToOpenIndex))
            return;

        var nextToOpenNode = pathNodeArray[nextToOpenIndex];
        if (!nextToOpenNode.isWalkable)
            return;

        var tentativeGCost = toOpenNode.gCost + GetDistanceCost(toOpenPosition, nextToOpenPosition);
        if (tentativeGCost >= nextToOpenNode.gCost)
            return;

        nextToOpenNode.previousIndex = toOpenNode.index;
        nextToOpenNode.gCost = tentativeGCost;
        nextToOpenNode.UpdateFCost();
        pathNodeArray[nextToOpenIndex] = nextToOpenNode;

        openNodes.Add(nextToOpenNode.index);
    }

    private List<int2> GetFullPath(NativeArray<PathNode> pathNodeArray, PathNode endNode)
    {
        if (endNode.previousIndex == -1)
            return null;
        var result = new List<int2> {new int2(endNode.X, endNode.Y)};
        var currentNode = endNode;
        while (currentNode.previousIndex != -1)
        {
            currentNode = pathNodeArray[currentNode.previousIndex];
            result.Add(new int2(currentNode.X, currentNode.Y));
        }

        return result;

    }

    private void FindPath(int2 startPosition, int2 endPosition)
    {
        var gridSize = new int2(20, 20);
        var pathNodeArray = GetPathNodeArray(gridSize, endPosition);
        var neighboringPosition = GetNeighboringPositions();
        var endNodeIndex = GetIndex(endPosition.x, endPosition.y, gridSize.x);

        var startNode = pathNodeArray[GetIndex(startPosition.x, startPosition.y, gridSize.x)];
        startNode.gCost = 0;
        startNode.UpdateFCost();
        pathNodeArray[startNode.index] = startNode;

        var openNodes = new SortedSet<int>();
        var visited = new HashSet<int>();

        openNodes.Add(startNode.index);
        while (openNodes.Count > 0)
        {
            var toOpenNode = pathNodeArray[GetLowestCostFNodeIndex(openNodes, pathNodeArray)];
            
            if (toOpenNode.index == endNodeIndex)
                break;

            openNodes.Remove(toOpenNode.index);
            visited.Add(toOpenNode.index);

            foreach (var neighborNode in neighboringPosition)
                OpenNode(toOpenNode, neighborNode, gridSize, visited, pathNodeArray, openNodes);
        }

        var endNode = pathNodeArray[endNodeIndex];
        var resultPath = GetFullPath(pathNodeArray, endNode);
        if (resultPath is null)
            Debug.Log("Не нашел путь :(");
        else
        {
            foreach (var point in resultPath)
            {
                //Debug.Log(point);
            }
        }

        resultPath.Clear();

        openNodes.Clear();
        visited.Clear();
        pathNodeArray.Dispose();
        neighboringPosition.Clear();
    }
}
