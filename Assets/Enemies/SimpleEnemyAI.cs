using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions;
using GridTools;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class SimpleEnemyAI : MonoBehaviour
{
    public HealthObj Health;
    public Rigidbody2D Rb;
    public Weapon.Weapon Weapon;
    private float latestAimAngle;
    private Stage currentStage;
    private State state;

    public GridObj Grid;
    private PathFinding pathFinder;
    private Vector3 startingPosition;
    private Vector3 roamPosition;
    private Vector3 nextTarget;
    private int nextTargetIndex;
    private Vector3 direction;
    private Vector3 latestPlayerPosition;

    private List<int2> path;

    private const float pauseTime = 1f;
    private float pauseStart;

    private const float followingTime = 0.5f;
    private float followingStartTime;

    private float moveSpeed;

    //private Task<List<int2>> task;

    private enum Stage
    {
        None,
        SearchingPath,
        Moving,
        Pause
    }

    private enum State
    {
        Roaming,
        ChasingPlayer
    }

    private void Start()
    {
        pathFinder = new PathFinding();
        Health = gameObject.AddComponent<HealthObj>();
       
        currentStage = Stage.None;
        moveSpeed = 5f;
        followingStartTime = Time.time;
    }

    private void FixedUpdate()
    {
        if (Health.Health.CurrentHealthPoints <= 0)
            Die();

        ChooseBehaviour();

        switch (state)
        {
            case State.Roaming:
                Move(roamPosition);
                break;
            case State.ChasingPlayer:
                //if (Vector3.Distance(latestPlayerPosition, PlayerController.Instance.GetPosition()) < 30)
                //    currentStage = Stage.None;
                latestPlayerPosition = PlayerController.Instance.GetPosition();
                MoveWithTimer(latestPlayerPosition, followingTime);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Move(Vector3 target)
    {
        switch (currentStage)
        {
            case Stage.None:
                currentStage = Stage.SearchingPath;
                StartSearchNextTarget(target);
                break;
            case Stage.SearchingPath:
                break;
            case Stage.Moving:
                MoveToNextTarget();
                break;
            case Stage.Pause:
                var difference = Time.time - pauseStart;
                if (difference >= pauseTime)
                {
                    UpdateTarget(GetRandomPosition());
                    currentStage = Stage.None;
                }
                    
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void MoveWithTimer(Vector3 target, float timeFollow)
    {
        var timeFollowing = Time.time - followingStartTime;
        if (timeFollowing >= timeFollow)
        {
            //if (currentStage == Stage.SearchingPath)
            //    task.Dispose();

            currentStage = Stage.None;
            followingStartTime = Time.time;
            return;
        }

        Move(target);
    }

    private Task<List<int2>> FindPath(int2 startGridPosition, int2 endGridPosition)
    {
        var task = new Task<List<int2>>(() => 
            pathFinder.FindPathAStar(Grid.Grid, startGridPosition, endGridPosition));

        task.Start();
        return task;
    }

    private async void StartSearchNextTarget(Vector3 target)
    {
        UpdateTarget(target);

        var startGridPosition = Grid.WorldToGridPosition(startingPosition);
        var endGridPosition = Grid.WorldToGridPosition(roamPosition);

        var originalPath = await FindPath(startGridPosition, endGridPosition);

        if (originalPath is null)
            currentStage = Stage.None;
        else
        {
            path = PathFinding.GetClearPath(originalPath);
            Grid.AddPathsToDraw(path);

            nextTargetIndex = 0;
            UpdateNextTarget();
        }
    }

    private void MoveToNextTarget()
    {
        UpdateAim();
        var distanceToNextTarget = transform.position.DistanceTo(nextTarget);

        Rb.velocity = direction * moveSpeed;
        
        if (distanceToNextTarget >= moveSpeed * Time.fixedDeltaTime)
            return;
        
        if (nextTargetIndex == path.Count - 1)
        {
            Rb.velocity = Vector2.zero;
            currentStage = Stage.Pause;
            pauseStart = Time.time;
        }

        UpdateNextTarget();
    }

    private void UpdateNextTarget()
    {
        for (var i = path.Count - 1; i > nextTargetIndex; i--)
        {
            var target = Grid.GridToWorldPosition(path[i]).ToVector3();
            var currentPosition = transform.position;
            var distance = currentPosition.DistanceTo(target);
            var currentDirection = (target - currentPosition).normalized;

            var ray = Physics2D.Raycast(currentPosition.ToVector2(), currentDirection.ToVector2(), distance, LayerMask.GetMask("Walls"));

            if (ray.collider != null)
                continue;
            
            nextTargetIndex = i;
            nextTarget = target;
            currentStage = Stage.Moving;
            break;
        }
    }

    private void UpdateTarget(Vector3 target)
    {
        startingPosition = transform.position;
        roamPosition = target;
        direction = (roamPosition - startingPosition).normalized;
    }

    private void UpdateAim()
    {
        direction = (nextTarget - transform.position).normalized;
        var aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Weapon.weaponPrefab.transform.RotateAround(Rb.position, Vector3.forward, aimAngle - latestAimAngle);
        latestAimAngle = aimAngle;
    }

    private Vector3 GetRandomPosition()
        => startingPosition + Tools.GetRandomDir() * Random.Range(10f, 70f);
    
    private void Die()
        => Destroy(gameObject);

    private void ChooseBehaviour()
    {
        var targetRange = 20f;
        state = Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < targetRange
            ? State.ChasingPlayer
            : State.Roaming;
    }
}
