using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Extensions;
using Game;
using GridTools;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Enemies
{
    public class Enemy : MonoBehaviour
    {
        protected enum Stage
        {
            None,
            SearchingPath,
            Moving,
            Pause
        }

        protected enum State
        {
            Roaming,
            RunFromPlayer,
            RunToPlayer
        }

        public GridObj Grid;
        public HealthObj Health;
        public float MoveSpeed;
        protected PathFinding pathFinder;
        protected SpriteRenderer sprite;
        protected Rigidbody2D Rb;
        protected CircleCollider2D Collider;

        public Weapon.Weapon Weapon;
        protected float latestAimAngle;
        protected Vector3 directionFire;

        protected Stage currentStage;
        protected State state;

        public Vector3 homePosition;
        protected float homeRadius;

        protected bool isEyeDirectionInverse;
        protected Vector3 moveDirection;
        protected Vector3 startingPosition;
        protected Vector3 roamPosition;
        protected Vector3 nextTarget;
        protected int nextTargetIndex;
        protected List<int2> path;

        protected int countFailSearch;
        protected const int countFailSearchLimit = 5;
        
        protected float pauseStart;
        protected float pauseTime;

        protected float followingStartTime;
        protected float followingTime;

        protected float reloadStart;
        protected float reloadTime;

        protected float targetRange;
        protected float fireRange;
        protected float runRange;

        protected void SetStartDefaults()
        {
            pathFinder = new PathFinding();
            Rb = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            Collider = GetComponent<CircleCollider2D>();

            homePosition = transform.position;
            startingPosition = transform.position;
            UpdateTarget(GetRandomPosition());

            followingStartTime = Time.time;
            reloadStart = Time.time;

            currentStage = Stage.None;
        }

        protected void DoStateAction()
        {
            switch (state)
            {
                case State.Roaming:
                    if (countFailSearch > 0)
                        UpdateTarget(countFailSearch >= countFailSearchLimit
                            ? homePosition
                            : GetRandomPosition());
                    UpdateEyeDirection(nextTarget);
                    Move(roamPosition);
                    break;
                case State.RunFromPlayer:
                    var playerPosition = GameData.player.GetPosition();
                    do roamPosition = GetRandomPosition();
                    while (roamPosition.DistanceTo(playerPosition) < runRange);

                    UpdateTarget(roamPosition);
                    UpdateEyeDirection(nextTarget);
                    MoveWithTimer(roamPosition, followingTime);
                    break;
                case State.RunToPlayer:
                    UpdateTarget(GameData.player.GetPosition());
                    UpdateEyeDirection(GameData.player.GetPosition());
                    MoveWithTimer(roamPosition, followingTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected void Move(Vector3 target)
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
        protected void MoveWithTimer(Vector3 target, float timeFollow)
        {
            var timeFollowing = Time.time - followingStartTime;
            if (timeFollowing >= timeFollow && currentStage != Stage.SearchingPath)
            {
                currentStage = Stage.None;
                followingStartTime = Time.time;
                return;
            }

            Move(target);
        }

        protected Task<List<int2>> FindPath(int2 startGridPosition, int2 endGridPosition, int maxDeep)
        {
            var task = new Task<List<int2>>(() =>
                pathFinder.FindPathAStar(Grid.Grid, startGridPosition, endGridPosition, maxDeep));

            task.Start();
            return task;
        }

        protected async void StartSearchNextTarget(Vector3 target)
        {
            UpdateTarget(target);

            var startGridPosition = Grid.WorldToGridPosition(startingPosition);
            var endGridPosition = Grid.WorldToGridPosition(roamPosition);

            var maxDeep = (int)homeRadius;
            var originalPath = await FindPath(startGridPosition, endGridPosition, maxDeep);

            if (originalPath is null)
            {
                currentStage = Stage.None;
                countFailSearch++;
            }
            else
            {
                countFailSearch = 0;
                path = PathFinding.GetClearPath(originalPath);
                //Grid.AddPathsToDraw(path);

                nextTargetIndex = 0;
                UpdateNextTarget();
            }
        }

        protected void MoveToNextTarget()
        {
            UpdateMoveDirection(nextTarget);
            var distanceToNextTarget = transform.position.DistanceTo(nextTarget);

            Rb.velocity = moveDirection * MoveSpeed;

            if (distanceToNextTarget >= MoveSpeed * Time.fixedDeltaTime)
                return;

            if (nextTargetIndex == path.Count - 1)
            {
                Rb.velocity = Vector2.zero;
                currentStage = Stage.Pause;
                pauseStart = Time.time;
                return;
            }

            UpdateNextTarget();
        }

        protected void UpdateNextTarget()
        {
            for (var i = path.Count - 1; i > nextTargetIndex; i--)
            {
                var target = Grid.GridToWorldPosition(path[i]).ToVector3() + new Vector3(Grid.Grid.CellSize, Grid.Grid.CellSize) / 2;
                var currentPosition = transform.position;
                var distance = currentPosition.DistanceTo(target);
                var currentDirection = (target - currentPosition).normalized;

                var ray = Physics2D.CircleCast(currentPosition.ToVector2(), Collider.radius, currentDirection.ToVector2(), distance, Grid.WallsLayerMask);

                if (ray.collider != null)
                    continue;

                nextTargetIndex = i;
                nextTarget = target;
                currentStage = Stage.Moving;
                return;
            }

            nextTargetIndex++;
            nextTarget = Grid.GridToWorldPosition(path[nextTargetIndex]).ToVector3() + new Vector3(Grid.Grid.CellSize, Grid.Grid.CellSize) / 2;
            currentStage = Stage.Moving;
        }

        protected void UpdateTarget(Vector3 target)
        {
            startingPosition = transform.position;
            roamPosition = homePosition.DistanceTo(target) > homeRadius
                ? homePosition
                : target;
        }

        protected void UpdateFireDirection(Vector3 target)
        {
            directionFire = (target - transform.position).normalized;
            var aimAngle = Mathf.Atan2(directionFire.y, directionFire.x) * Mathf.Rad2Deg - 90f;
            Weapon.weaponPrefab.transform.RotateAround(Rb.position, Vector3.forward, aimAngle - latestAimAngle);
            latestAimAngle = aimAngle;
        }

        protected void UpdateEyeDirection(Vector3 target)
        {
            sprite.flipX = (int)Mathf.Sign(target.x - transform.position.x) == (isEyeDirectionInverse ? 0 : 1);
        }

        protected void UpdateMoveDirection(Vector3 target)
        {
            moveDirection = (nextTarget - transform.position).normalized;
        }

        protected Vector3 GetRandomPosition()
        {
            return homePosition + Tools.GetRandomDir() * Random.Range(10f, 15f);
        }

        protected bool IsNearToPlayer(float distance)
        {
            return Vector3.Distance(transform.position, GameData.player.transform.position) < distance;
        }
    }
}
