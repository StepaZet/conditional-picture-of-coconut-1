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
    public class MimicBoss : Enemy
    {
        //private float latestAimAngle;

        [SerializeField] private int maxHealth;
        [SerializeField] protected BulletMimicBoss EvilBallPrefab;
        private Rigidbody2D Rb;
        [SerializeField] protected Sprite[] sprites;
        [SerializeField] protected CuteDemon CuteDemonPrefab;
        [SerializeField] protected PurpleWizard PurpleWizardPrefab;
        private Enemy[] children;
        private SpriteRenderer sprite;
    
        //public GameObject ForceField;

        private Stage currentStage;
        private State state;
        [SerializeField] private AttackStage attackStage;

        private Vector3 homePosition;
        private float homeRadius;

        private Vector3 startingPosition;
        private Vector3 roamPosition;
        private Vector3 nextTarget;
        private int nextTargetIndex;

        private int countFailSearch;
        private const int countFailSearchLimit = 5;

        private Vector3 direction;

        private List<int2> path;

        private float followingStartTime;
        private const float followingTime = 1f;

        private float reloadBiteStart;
        private float biteReloadTime = 0.5f;

        private float attackPauseStart;
        private const float attackPauseTime = 2f;

        private float pauseStart;
        private const float pauseTime = 1f;

        private float? startTransformation;
        private const float transformationTime = 5f;

        private float reloadStart;
        //private float reloadTime = 0.3f;

        private float targetRange = 25f;
        private float fireRange;
        private float NativeMoveSpeed;

        private int biteDamage = 15;

        private Queue<AttackStage> attackStrategy;
        //public Bomber BomberPrefab;
        //private List<Bomber> bombers;

        //private int maxBombers = 20;
        private int maxEvilBallWaves = 10;
        private int currentEvilBallWaves = 0;
        //private Vector3 dashTarget;

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
            RunToPlayer,
            RunFromPlayer
        }

        private enum AttackStage
        {
            None,
            Pause,
            SpawnMobs,
            EvilBalls,
            WizardSpy,
            CuteDemon
        }

        private enum SpritesEnum
        {
            Native,
            CuteDemon
        }

        private void Start()
        {
            pathFinder = new PathFinding();
        
            sprite = GetComponent<SpriteRenderer>();
            Rb = GetComponent<Rigidbody2D>();
            fireRange = Rb.transform.localScale.x * 2f;

            homePosition = transform.position;
            startingPosition = transform.position;
            UpdateTarget(GetRandomPosition());

            homeRadius = 25;

            currentStage = Stage.None;
            attackStage = AttackStage.None;
            NativeMoveSpeed = MoveSpeed = 6f;
            followingStartTime = Time.time;
            //reloadBoomStart = Time.time;
            startTransformation = null;

            attackStrategy = new Queue<AttackStage>(
                new[]
                {
                    AttackStage.None,
                    //AttackStage.EvilBalls,
                    //AttackStage.SpawnMobs,
                    //AttackStage.EvilBalls,
                    AttackStage.CuteDemon,
                    //AttackStage.EvilBalls,
                    AttackStage.WizardSpy
                }
            );

        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();


            var target = IsNearToPlayer(targetRange)
                ? GameData.player.transform.position
                : nextTarget;

            ChooseBehaviour();

            UpdateAimFire(target);
            UpdateEyeDirection(target);

            if (IsNearToPlayer(fireRange))
                Fire(biteReloadTime);

            switch (attackStage)
            {
                case AttackStage.None:
                    if (IsNearToPlayer(targetRange))
                        EndStage();
                    break;
                case AttackStage.Pause:
                    var difference = Time.time - attackPauseStart;
                    if (difference >= attackPauseTime)
                        UpdateAttackStage();
                    break;
                case AttackStage.SpawnMobs:
                    EndStage();
                    break;
                case AttackStage.EvilBalls:
                    CreateEvilBall();
                    break;
                case AttackStage.WizardSpy:
                    EndStage();
                    break;
                case AttackStage.CuteDemon:
                    BeCuteDemon();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (state)
            {
                case State.Roaming:
                    if (countFailSearch > 0)
                        UpdateTarget(countFailSearch >= countFailSearchLimit
                            ? homePosition
                            : GetRandomPosition());
                    Move(roamPosition);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Move(Vector3 target, bool dashMode = false)
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
                    MoveToNextTarget(dashMode);
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

        private void MoveWithTimer(Vector3 target, float timeFollow, bool dashMode=false)
        {
            var timeFollowing = Time.time - followingStartTime;
            if (timeFollowing >= timeFollow && currentStage != Stage.SearchingPath)
            {
                currentStage = Stage.None;
                followingStartTime = Time.time;
                return;
            }

            Move(target, dashMode);
        }

        private Task<List<int2>> FindPath(int2 startGridPosition, int2 endGridPosition, int maxDeep)
        {
            var task = new Task<List<int2>>(() =>
                pathFinder.FindPathAStar(Grid.Grid, startGridPosition, endGridPosition, maxDeep));

            task.Start();
            return task;
        }

        private async void StartSearchNextTarget(Vector3 target)
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
                Grid.AddPathsToDraw(path);

                nextTargetIndex = 0;
                UpdateNextTarget();
            }
        }

        private void MoveToNextTarget(bool dashMode = false)
        {
            UpdateDirection(nextTarget);
            var distanceToNextTarget = nextTarget.DistanceTo(transform.position.ToVector2());

            Rb.velocity = direction * MoveSpeed;

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

        private void UpdateNextTarget()
        {
            for (var i = path.Count - 1; i > nextTargetIndex; i--)
            {
                var target = Grid.GridToWorldPosition(path[i]).ToVector3() + new Vector3(Grid.Grid.CellSize, Grid.Grid.CellSize) / 2;
                var currentPosition = transform.position;
                var distance = currentPosition.DistanceTo(target);
                var currentDirection = (target - currentPosition).normalized;

                var ray = Physics2D.CircleCast(currentPosition.ToVector2(), transform.localScale.y, currentDirection.ToVector2(), distance, Grid.WallsLayerMask);

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

        private void UpdateTarget(Vector3 target)
        {
            startingPosition = transform.position;
            roamPosition = homePosition.DistanceTo(target) > homeRadius
                ? homePosition
                : target;
        }

        private void UpdateDirection(Vector3 target)
        {
            direction = (nextTarget - transform.position).normalized;
        }

        private Vector3 GetRandomPosition()
            => homePosition + Tools.GetRandomDir() * Random.Range(10f, 15f);

        private void UpdateEyeDirection(Vector3 target)
        {
            sprite.flipX = (int)Mathf.Sign(target.x - transform.position.x) == 1;
        }

        private void Fire(float reloadTime)
        {
            var difference = Time.time - reloadBiteStart;
            if (difference < reloadTime)
                return;
            reloadBiteStart = Time.time;

            GameData.player.character.health.Damage(biteDamage);
        }

        private bool CheckReload(float reloadTime)
        {
            var difference = Time.time - reloadStart;
            if (difference < reloadTime)
                return false;
            reloadStart = Time.time;
            return true;
        }

        private void SpawnBombers()
        {
            const float reloadTime = 0.2f;
            if (!CheckReload(reloadTime))
                return;

            //bombers ??= new List<Bomber>();

            //if (bombers.Count < maxBombers)
            //{
            //    var dir = (GameData.player.transform.position - transform.position).normalized;
            //    var bomber = Instantiate(BomberPrefab, transform.position + dir,
            //        Quaternion.identity);
            //    bomber.GetComponent<Rigidbody2D>().AddForce(dir * 20f, ForceMode2D.Impulse);
            //    bomber.Grid = Grid;
            //    bombers.Add(bomber);
            //}

            //else if (bombers.All(obj => obj == null))
            //{
            //    bombers.Clear();
            //    EndStage();
            //}
        }

        private void CreateEvilBall()
        {
            const float reloadTime = 0.2f;
            if (!CheckReload(reloadTime))
                return;

            currentEvilBallWaves++;
            if (currentEvilBallWaves > maxEvilBallWaves)
            {
                currentEvilBallWaves = 0;
                EndStage();
            }

            for (var angle = 0f; angle < Mathf.PI * 2; angle += Mathf.PI / 24)
            {
                var position = transform.position + new Vector3(fireRange * Mathf.Cos(angle), fireRange * Mathf.Sin(angle));
                var dir = (position - transform.position).normalized;
                var ball = Instantiate(EvilBallPrefab, position, Quaternion.identity);
                ball.GetComponent<Rigidbody2D>().AddForce(dir * 30, ForceMode2D.Impulse);
            }
        }

        //private void DoDash(Vector2 target)
        //{
        //    const float reloadTime = 0.1f;
        //    Fire(reloadTime);

        //    var dir = target - transform.position.ToVector2();
        //    Rb.AddForce(dir / 2, ForceMode2D.Impulse);

        //    if (transform.position.DistanceTo(target) < fireRange)
        //    {
        //        Fire(0);
        //        dashTarget = Vector3.zero;
        //        currentStage = Stage.Pause;
        //        pauseStart = Time.time;
        //        EndStage();
        //    }

        //}

        private void BeCuteDemon()
        {
            if (!startTransformation.HasValue)
            {
                sprite.sprite = sprites[(int) SpritesEnum.CuteDemon];
                children = new Enemy[4];
                for (var i = 0; i < children.Length; i++)
                {
                    children[i] = Instantiate(CuteDemonPrefab, transform.position, Quaternion.identity);
                    children[i].Grid = Grid;

                }

                MoveSpeed = children[0].MoveSpeed;
                startTransformation = Time.time;
            }
            //Health.IsImmortal = false;
            var difference = Time.time - startTransformation.Value;
        
            MoveWithTimer(GameData.player.transform.position, followingTime);

            if (difference < transformationTime)
                return;
            MoveSpeed = NativeMoveSpeed;
            //Health.IsImmortal = true;
            startTransformation = null;
            sprite.sprite = sprites[(int)SpritesEnum.Native];
            foreach (var demon in children)
                if (demon != null)
                    demon.Health.Damage(int.MaxValue);
            children = null;
            EndStage();
        }

        private void BeWizard()
        {
            if (children == null)
            {
                children = new Enemy[4];
                for (var i = 0; i < children.Length; i++)
                {
                    children[i] = Instantiate(PurpleWizardPrefab, transform.position, Quaternion.identity);
                    children[i].Grid = Grid;
                }

                MoveSpeed = children[0].MoveSpeed;
            }

            //Health.IsImmortal = false;
            MoveWithTimer(GameData.player.transform.position, followingTime);

            MoveSpeed = NativeMoveSpeed;
            //Health.IsImmortal = true;
            startTransformation = null;
            sprite.sprite = sprites[(int)SpritesEnum.Native];
            foreach (var demon in children)
                if (demon != null)
                    demon.Health.Damage(int.MaxValue);
            EndStage();
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private bool IsNearToPlayer(float distance)
        {
            try
            {
                return Vector3.Distance(transform.position, GameData.player.transform.position) < distance;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateAimFire(Vector3 target)
        {
            //if (Weapon == null)
            //    return;
            //directionFire = (target - transform.position).normalized;
            //var aimAngle = Mathf.Atan2(directionFire.y, directionFire.x) * Mathf.Rad2Deg - 90f;
            //Weapon.weaponPrefab.transform.RotateAround(Rb.position, Vector3.forward, aimAngle - latestAimAngle);
            //latestAimAngle = aimAngle;
        }

        private void EndStage()
        {
            attackPauseStart = Time.time;
            attackStage = AttackStage.Pause;
        }

        private void UpdateAttackStage()
        {
            var nextStage = attackStrategy.Dequeue();
            attackStrategy.Enqueue(nextStage);
            attackStage = nextStage;
        }

        private void ChooseBehaviour()
        {
            if (homePosition.DistanceTo(transform.position) > homeRadius)
                UpdateTarget(homePosition);

            //if (attackStage == AttackStage.WizardSpy)
            //{
            //    if (IsNearToPlayer(targetRange))
            //    {
            //        if (IsNearToPlayer(RunRange))
            //        {
            //            if (state != State.RunFromPlayer)
            //                followingStartTime = int.MinValue;
            //            state = State.RunFromPlayer;
            //        }
            //        else
            //            state = State.RunToPlayer;
            //        return;
            //    }
            //}
            state = State.Roaming;

        }
    }
}
