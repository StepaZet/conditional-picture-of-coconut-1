using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using GridTools;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleEnemyAI : MonoBehaviour
{
    private enum Stage
    {
       SearchingPath,
       FoundPath,
       Moving,
       None,
    }
    
    public GridObj Grid;
    public Rigidbody2D rb;
    public Weapon.Weapon weapon;
    private PathFinding pathFinder;

    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private Vector3 direction;

    private Vector3 nextTarget;
    private int nextTargetIndex;

    private readonly float moveSpeed = 5f;

    private List<int2> path;
    private Stage currentStage;

    private void Start()
    {
        pathFinder = new PathFinding();
        currentStage = Stage.None;
    }

    private void Update()
    {
        switch (currentStage)
        {
            case Stage.None:
            {
                UpdateTarget();
                currentStage = Stage.SearchingPath;
                var startGridPosition = Grid.WorldToGridPosition(startingPosition);
                var endGridPosition = Grid.WorldToGridPosition(roamPosition);

                var taskFinding =
                new Task<List<int2>>(() => pathFinder.FindPathAStar(Grid.Grid,
                        startGridPosition, endGridPosition));

                taskFinding.Start();
                taskFinding.ContinueWith(task =>
                {
                    var result = task.Result;
                    if (result is null)
                        currentStage = Stage.None;
                    else
                    {
                        Grid.AddPathsToDraw(result);
                        path = result;
                        path.Reverse();
                        currentStage = Stage.FoundPath;
                    }
                });
                break;
            }
            case Stage.SearchingPath:
                break;
            case Stage.FoundPath:
                nextTargetIndex = 0;
                Debug.Log(path[nextTargetIndex]);
                nextTarget = Grid.GridToWorldPosition(path[nextTargetIndex]).ToVector3();
                direction = (nextTarget - transform.position).normalized;
                currentStage = Stage.Moving;
                break;

            case Stage.Moving:
                var distance = transform.position.DistanceTo(nextTarget);
                if (distance > moveSpeed)
                    rb.velocity = direction * moveSpeed;
                else
                {
                    rb.velocity = direction * distance;
                    nextTargetIndex++;
                    if (nextTargetIndex == path.Count)
                        currentStage = Stage.None;
                    else
                    {
                        nextTarget = Grid.GridToWorldPosition(path[nextTargetIndex]).ToVector3();
                        direction = (nextTarget - transform.position).normalized;
                    }
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private Stage FollowThePath(Rigidbody2D rb)
    {
        foreach (var positionTo in path.Select(position => Grid.GridToWorldPosition(position).ToVector3()))
        {
            direction = (positionTo - rb.position.ToVector3()).normalized;
            weapon.weaponPrefab.transform.RotateAround(rb.position, direction, 0f);
            while (true)
            {
                var distance = rb.position.ToVector3().DistanceTo(positionTo);
                if (distance > moveSpeed)
                    rb.velocity = direction * moveSpeed;
                else
                {
                    rb.velocity = direction * distance;
                    break;
                }
            }
        }

        return Stage.None;
    }

    private void UpdateTarget()
    {
        startingPosition = transform.position;
        roamPosition = GetRandomPosition();
        direction = (roamPosition - startingPosition).normalized;
        currentStage = Stage.None;
    }

    private Vector3 GetRandomPosition()
        => startingPosition + Tools.GetRandomDir() * Random.Range(10f, 70f);
}
