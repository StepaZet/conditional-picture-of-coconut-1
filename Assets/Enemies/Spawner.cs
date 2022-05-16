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
    public class Spawner : Enemy
    {
        public Fly FlyBullet;

        private Fly[] flies;
        private const int maxCountFly = 5;

        private void Start()
        {
            pathFinder = new PathFinding();
            Rb = GetComponent<Rigidbody2D>();
            Collider = GetComponent<CircleCollider2D>();
            flies = new Fly[maxCountFly + Random.Range(-1, 1)];

            homePosition = transform.position;
            startingPosition = transform.position;
            UpdateTarget(GetRandomPosition());

            homeRadius = 25;

            pauseTime = 1f;

            followingTime = 6f;

            reloadTime = 1f;

            targetRange = 10f;

            currentStage = Stage.None;
            MoveSpeed = 3f;
            followingStartTime = Time.time;
            reloadStart = Time.time;
        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();

            UpdateFlies();
            Fire();

            ChooseBehaviour();

            switch (state)
            {
                case State.Roaming:
                    if (countFailSearch > 0)
                        UpdateTarget(countFailSearch >= countFailSearchLimit
                            ? homePosition
                            : GetRandomPosition());
                    Move(roamPosition);
                    break;
                case State.RunFromPlayer:
                    var playerPosition= GameData.player.GetPosition();
                    do
                    {
                        roamPosition = GetRandomPosition();
                    } while (roamPosition.DistanceTo(playerPosition) < targetRange);

                    UpdateTarget(roamPosition);
                    MoveWithTimer(roamPosition, followingTime);
                    break;
            }
        }

        private void UpdateFlies()
        {
            foreach (var fly in flies)
                if (fly != null)
                    fly.homePosition = transform.position;
        }

        private void Fire()
        {
            var difference = Time.time - reloadStart;
            if (difference < reloadTime)
                return;
            for (var i = 0; i < flies.Length; i++)
            {
                if (flies[i] == null)
                {
                    flies[i] = Instantiate(FlyBullet, transform.position, transform.rotation);
                    flies[i].Grid = Grid;
                    flies[i].homePosition = transform.position;
                    reloadStart = Time.time;
                    break;
                }

            }
        }

        private void Die()
        {
            for (var i = 0; i < flies.Length; i++)
            {
                var fly = Instantiate(FlyBullet, transform.position + Tools.GetRandomDir(), transform.rotation);
                fly.Grid = Grid;
                fly.homePosition = transform.position;
            }
            Destroy(gameObject);
        }

        private void ChooseBehaviour()
        {
            if (homePosition.DistanceTo(transform.position) > homeRadius)
                UpdateTarget(homePosition);
            if (IsNearToPlayer(targetRange))
            {
                if (state != State.RunFromPlayer)
                    followingStartTime = int.MinValue;
                state = State.RunFromPlayer;
            }
            else
                state = State.Roaming;
        }
    }
}
