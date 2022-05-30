using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions;
using Game;
using GridTools;
using Health;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Enemies
{
    public class MimicBoss : Enemy
    {
        [SerializeField] protected BulletMimicBoss EvilBallPrefab;
        [SerializeField] protected Enemy[] Enemies;

        private Enemy[] children;
        private List<Enemy> mobs;

        [SerializeField] private AttackStage attackStage;

        private float biteReloadStart;
        private float biteReloadTime = 0.5f;

        private float attackPauseStart;
        private const float attackPauseTime = 2f;

        private float? startTransformation;
        private const float transformationTime = 5f;

        private int nativeHealth;
        private float nativeFollowingTime;
        private float nativeMoveSpeed;
        private float nativeDrag;
        private Sprite nativeSprite;

        private Queue<AttackStage> attackStrategy;

        private int maxEvilBallWaves = 10;
        private int EvilBallWavesCount;

        private int maxMimicCount = 7;

        private enum AttackStage
        {
            None,
            Pause,
            SpawnMobs,
            EvilBalls,
            WizardSpy,
            CuteDemon
        }

        private enum enemiesOrder
        {
            self,
            CuteDemon,
            Mimic,
            Wizard
        }

        private void Start()
        {
            SetStartDefaults();

            homeRadius = 25;
            targetRange = 25f;
            fireRange = Rb.transform.localScale.x * 2f;

            damage = 15;

            pauseTime = 1f;
            nativeFollowingTime = followingTime = 1f;

            nativeMoveSpeed = MoveSpeed = 6f;
            nativeSprite = sprite.sprite;
            nativeDrag = Rb.drag;

            attackStage = AttackStage.None;
            
            followingStartTime = Time.time;
            biteReloadStart = Time.time;
            startTransformation = null;

            attackStrategy = new Queue<AttackStage>(
                new[]
                {
                    AttackStage.None,
                    AttackStage.EvilBalls,
                    AttackStage.SpawnMobs,
                    AttackStage.CuteDemon,
                    AttackStage.EvilBalls,
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

            UpdateFireDirection(target);
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
                    SpawnMobs();
                    break;
                case AttackStage.EvilBalls:
                    CreateEvilBall();
                    break;
                case AttackStage.WizardSpy:
                    BeWizard();
                    break;
                case AttackStage.CuteDemon:
                    BeCuteDemon();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ChooseState();
            DoStateAction();
        }

        private void Fire(float actualReloadTime)
        {
            var difference = Time.time - biteReloadStart;
            if (difference < actualReloadTime)
                return;
            biteReloadStart = Time.time;

            GameData.player.character.health.Damage(damage);
        }

        private bool CheckReload(float actualReloadTime)
        {
            var difference = Time.time - reloadStart;
            if (difference < actualReloadTime)
                return false;
            reloadStart = Time.time;
            return true;
        }

        private void WeaponFire()
        {
            if (Weapon == null)
                return;

            Weapon.Fire(true);
        }

        private void GetTransformation()
        {
            sprite.sprite = GameData.player.character.sprite.sprite;
            Weapon = Instantiate(GameData.player.character.weapon, transform.position + new Vector3(0, 1), Quaternion.identity);
            Weapon.transform.parent = transform;
            Health.IsImmortal = true;
        }

        private void ClearTransformation()
        {
            sprite.sprite = nativeSprite;
            Destroy(Weapon.gameObject);
            latestAimAngle = 0;
            Health.IsImmortal = false;
        }

        private void SpawnMobs()
        {
            reloadTime = 0.2f;
            if (!CheckReload(reloadTime))
                return;

            if (mobs == null)
            {
                GetTransformation();
                mobs = new List<Enemy>();
            }

            WeaponFire();

            if (mobs.Count < maxMimicCount - maxMimicCount / 4)
            {
                var mimic = Instantiate(Enemies[(int)enemiesOrder.Mimic], transform.position, Quaternion.identity);
                mimic.Grid = Grid;
                mimic.gameObject.layer = LayerMask.NameToLayer("Enemy");
                mobs.Add(mimic);
            }
            else if (mobs.Count < maxMimicCount)
            {
                var cuteDemon = Instantiate(Enemies[(int)enemiesOrder.CuteDemon], transform.position, Quaternion.identity);
                cuteDemon.gameObject.layer = LayerMask.NameToLayer("Enemy");
                cuteDemon.Grid = Grid;
                mobs.Add(cuteDemon);
            }
            else if (mobs.All(obj => obj == null))
            {
                ClearTransformation();
                mobs.Clear();
                mobs = null;
                EndStage();
            }
        }

        private void CreateEvilBall()
        {
            reloadTime = 0.2f;
            if (!CheckReload(reloadTime))
                return;

            EvilBallWavesCount++;
            if (EvilBallWavesCount > maxEvilBallWaves)
            {
                EvilBallWavesCount = 0;
                EndStage();
            }

            for (var angle = 0f; angle < Mathf.PI * 2; angle += Mathf.PI / 24)
            {
                var position = transform.position + new Vector3(fireRange * Mathf.Cos(angle), fireRange * Mathf.Sin(angle)) * 3;
                var dir = (position - transform.position).normalized;
                var ball = Instantiate(EvilBallPrefab, position, Quaternion.identity);
                ball.GetComponent<Rigidbody2D>().AddForce(dir * 30, ForceMode2D.Impulse);
            }
        }

        private void BeCuteDemon()
        {
            if (!startTransformation.HasValue)
            {
                children = new Enemy[4];
                for (var i = 0; i < children.Length; i++)
                {
                    children[i] = Instantiate(Enemies[(int) enemiesOrder.CuteDemon], transform.position, Quaternion.identity);
                    children[i].gameObject.layer = LayerMask.NameToLayer("Enemy");
                    children[i].Grid = Grid;
                }
                sprite.sprite = children[0].sprite.sprite;
                Rb.drag = children[0].Rb.drag;
                MoveSpeed = children[0].MoveSpeed;
                followingTime = children[0].followingTime;
                runRange = children[0].runRange;
                startTransformation = Time.time;
            }

            MoveWithTimer(GameData.player.transform.position, followingTime);

            var difference = Time.time - startTransformation.Value;
            if (difference < transformationTime)
                return;
            startTransformation = null;

            MoveSpeed = nativeMoveSpeed;
            sprite.sprite = nativeSprite;
            Rb.drag = nativeDrag;
            MoveSpeed = nativeMoveSpeed;

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
                var count = 12;
                children = new Enemy[count];
                transform.position = GetRandomPosition();
                for (var i = 0; i < count; i++)
                {
                    var position = GetRandomPosition();
                    children[i] = Instantiate(Enemies[(int)enemiesOrder.Wizard], position, Quaternion.identity);
                    children[i].gameObject.layer = LayerMask.NameToLayer("Enemy");
                    children[i].Grid = Grid;
                    children[i].Health.healthBarPrefab.SetActive(false);
                }

                sprite.sprite = children[0].sprite.sprite;
                Rb.drag = children[0].Rb.drag;
                MoveSpeed = children[0].MoveSpeed;
                followingTime = children[0].followingTime;
                nativeHealth = Health.CurrentHealthPoints;
                Health.healthBarPrefab.SetActive(false);
            }

            if (nativeHealth == Health.CurrentHealthPoints)
                return;

            MoveSpeed = nativeMoveSpeed;
            sprite.sprite = nativeSprite;
            Rb.drag = nativeDrag;
            MoveSpeed = nativeMoveSpeed;
            followingTime = nativeFollowingTime;
            Health.healthBarPrefab.SetActive(true);

            foreach (var demon in children)
                if (demon != null)
                {
                    demon.Health.healthBarPrefab.SetActive(true);
                    demon.Health.IsImmortal = false;
                    demon.Health.Damage(int.MaxValue);
                }

            children = null;
            EndStage();
        }

        protected override void Die()
        {
            DieDefault();
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

        private void ChooseState()
        {
            if (homePosition.DistanceTo(transform.position) > homeRadius)
                UpdateTarget(homePosition);

            if (attackStage == AttackStage.WizardSpy)
            {
                if (IsNearToPlayer(targetRange))
                {
                    if (IsNearToPlayer(runRange))
                    {
                        if (state != State.RunFromPlayer)
                            followingStartTime = int.MinValue;
                        state = State.RunFromPlayer;
                    }
                    else
                        state = State.RunToPlayer;
                    return;
                }
            }
            state = State.Roaming;
        }
    }
}
