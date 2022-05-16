using System;
using Extensions;
using Game;
using GridTools;
using UnityEngine;

namespace Assets.Enemies
{
    public class Fly : Enemy
    { 
        private void Start()
        {
            pathFinder = new PathFinding();
            sprite = GetComponent<SpriteRenderer>();
            Rb = GetComponent<Rigidbody2D>();
            Collider = GetComponent<CircleCollider2D>();

            homePosition = transform.position;
            startingPosition = transform.position;
            UpdateTarget(GetRandomPosition());

            homeRadius = 10;

            targetRange = 20f;
            fireRange = 10f;

            pauseTime = 0.4f;
            followingTime = 0.5f;

            currentStage = Stage.None;
            MoveSpeed = 5f;
            followingStartTime = Time.time;
        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();

            if (IsNearToPlayer(targetRange))
                UpdateAimFire(GameData.player.transform.position);
            else
                UpdateAimFire(nextTarget);

            if (IsNearToPlayer(fireRange))
                Fire();

            ChooseBehaviour();

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
                case State.RunToPlayer:
                    UpdateTarget(GameData.player.GetPosition());
                    UpdateEyeDirection(GameData.player.GetPosition());
                    MoveWithTimer(roamPosition, followingTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UpdateAimFire(Vector3 target)
        {
            directionFire = (target - transform.position).normalized;
            var aimAngle = Mathf.Atan2(directionFire.y, directionFire.x) * Mathf.Rad2Deg - 90f;
            Weapon.weaponPrefab.transform.RotateAround(Rb.position, Vector3.forward, aimAngle - latestAimAngle);
            latestAimAngle = aimAngle;
        }

        private void Fire()
        {
            Weapon.Fire(true);
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void UpdateEyeDirection(Vector3 target)
        {
            sprite.flipX = (int) Mathf.Sign(target.x - transform.position.x) == 1;
        }

        private void ChooseBehaviour()
        {
            if (homePosition.DistanceTo(transform.position) > homeRadius)
                UpdateTarget(homePosition);

            state = IsNearToPlayer(targetRange)
                ? State.RunToPlayer
                : State.Roaming;
        }
    }
}
