using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Bullet;
using Extensions;
using Game;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Enemies
{
    public class YellowBoss : Enemy
    {
        [SerializeField] protected ParticleSystem boom;
        [SerializeField] protected BulletYellowBoss FireBallPrefab;
        [SerializeField] protected Bomber BomberPrefab;
        [SerializeField] protected GameObject ForceField;
        [SerializeField] private AttackStage attackStage;

        private float reloadBoomStart;
        private const float boomReloadTime = 1f;

        private float attackPauseStart;
        private const float attackPauseTime = 2f;

        private float? startWeak;
        private const float weakTime = 5f;

        private Queue<AttackStage> attackStrategy;
        
        private List<Bomber> bombers;

        private const int maxBombers = 20;
        private const int maxFireBalls = 100;
        private int currentFireBalls;
        private Vector3 dashTarget;

        private enum AttackStage
        {
            None,
            Pause,
            SpawnBombers,
            FireBalls,
            Weak,
            Dash
        }

        private void Start()
        {
            SetStartDefaults();

            homeRadius = 25;
            targetRange = 25f;
            fireRange = Rb.transform.localScale.x * 2f;

            damage = 20;

            pauseTime = 1f;

            MoveSpeed = 6f;

            Health.IsImmortal = true;
            startWeak = null;
            attackStage = AttackStage.None;
            attackPauseStart = Time.time;
            reloadBoomStart = Time.time;

            attackStrategy = new Queue<AttackStage>(
                new[]
                {
                    AttackStage.None,
                    AttackStage.SpawnBombers,
                    AttackStage.FireBalls,
                    AttackStage.Dash,
                    AttackStage.Dash,
                    AttackStage.Dash,
                    AttackStage.FireBalls,
                    AttackStage.Weak
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
                Fire(boomReloadTime);

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
                case AttackStage.SpawnBombers:
                    SpawnBombers();
                    break;
                case AttackStage.FireBalls:
                    CreateFireBalls();
                    break;
                case AttackStage.Weak:
                    BeWeak();
                    break;
                case AttackStage.Dash:
                    dashTarget = dashTarget == Vector3.zero ? GameData.player.transform.position : dashTarget;
                    DoDash(dashTarget);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            ChooseBehaviour();
            DoStateAction();
        }

        private void Fire(float reloadTime)
        {
            var difference = Time.time - reloadBoomStart;
            if (difference < reloadTime)
                return;
            reloadBoomStart = Time.time;

            var objectsToGetDamage = Physics2D.OverlapCircleAll(transform.position, fireRange);

            for (var angle = 0f; angle < Mathf.PI * 2; angle += Mathf.PI / 3)
            {
                Instantiate(boom, transform.position + new Vector3(fireRange * Mathf.Cos(angle), fireRange * Mathf.Sin(angle)), Quaternion.identity);
            }

            foreach (var obj in objectsToGetDamage)
            {
                if (!obj.GetComponentInChildren<Character>())
                    continue;

                var healthObj = obj.GetComponentInChildren<HealthObj>();
                //obj.GetComponent<Rigidbody2D>().AddForce((obj.transform.position - transform.position).normalized * 10, ForceMode2D.Impulse);
                if (healthObj != null)
                    healthObj.Damage(damage);
            }
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
            reloadTime = 0.2f;
            if (!CheckReload(reloadTime))
                return;
            
            bombers ??= new List<Bomber>();

            if (bombers.Count < maxBombers)
            {
                var dir = (GameData.player.transform.position - transform.position).normalized;
                var bomber = Instantiate(BomberPrefab, transform.position + dir,
                    Quaternion.identity);
                bomber.GetComponent<Rigidbody2D>().AddForce(dir * 20f, ForceMode2D.Impulse);
                bomber.Grid = Grid;
                bombers.Add(bomber);
            }
               
            else if(bombers.All(obj => obj == null))
            {
                bombers.Clear();
                EndStage();
            }
        }

        private void CreateFireBalls()
        {
            reloadTime = 0.05f;
            if (!CheckReload(reloadTime))
                return;

            currentFireBalls++;
            if (currentFireBalls > maxFireBalls)
            {
                currentFireBalls = 0;
                EndStage();
            }

            var direction = (GameData.player.transform.position - transform.position).normalized 
                            + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
            var fireball = Instantiate(
                FireBallPrefab, 
                transform.position + direction * transform.localScale.x * 1.5f, 
                Quaternion.identity);
            fireball.GetComponent<Rigidbody2D>().AddForce(direction * 17, ForceMode2D.Impulse);
        }

        private void DoDash(Vector2 target)
        {
            UpdateEyeDirection(target);
            reloadTime = 0.1f;
            Fire(reloadTime);

            var dir = target - transform.position.ToVector2();
            Rb.AddForce(dir / 2, ForceMode2D.Impulse);

            if (transform.position.DistanceTo(target) < fireRange)
            {
                Fire(0);
                dashTarget = Vector3.zero;
                currentStage = Stage.Pause;
                pauseStart = Time.time;
                EndStage();
            }
                
        }

        private void BeWeak()
        {
            if (!startWeak.HasValue)
            {
                ForceField.SetActive(false);
                startWeak = Time.time;
            }
                
            Health.IsImmortal = false;
            var difference = Time.time - startWeak.Value;
            if (difference < weakTime)
                return;
            Health.IsImmortal = true;
            startWeak = null;
            ForceField.SetActive(true);
            EndStage();
        }

        protected  override void Die()
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

        private void ChooseBehaviour()
        {
            if (homePosition.DistanceTo(transform.position) > homeRadius)
                UpdateTarget(homePosition);

            state = State.Roaming;
        }
    }
}
