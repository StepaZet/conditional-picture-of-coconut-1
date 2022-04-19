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
    private float latestAimAngle;

    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 roamPosition;
    [SerializeField] private Vector3 direction;

    [SerializeField] private Vector3 nextTarget;
    private int nextTargetIndex;
    
    private const float MoveSpeed = 1f;

    private List<int2> path;
    private Stage currentStage;

    private void Start()
    {
        pathFinder = new PathFinding();
        currentStage = Stage.None;
    }

    private void FixedUpdate()
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
                MoveToNextTarget();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MoveToNextTarget()
    {
        var distance = transform.position.DistanceTo(nextTarget);
        direction = (nextTarget - transform.position).normalized;
        var aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        weapon.weaponPrefab.transform.RotateAround(rb.position, Vector3.forward, aimAngle - latestAimAngle);
        latestAimAngle = aimAngle;
        if (distance > transform.position.DistanceTo(direction * MoveSpeed * Time.fixedDeltaTime))
            rb.velocity = direction * MoveSpeed * Time.fixedDeltaTime;
        else
        {
            transform.position = nextTarget;
            nextTargetIndex++;
            if (nextTargetIndex == path.Count)
                currentStage = Stage.None;
            else
            {
                nextTarget = Grid.GridToWorldPosition(path[nextTargetIndex]).ToVector3();
            }
        }
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
