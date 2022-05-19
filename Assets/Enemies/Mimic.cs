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
    public class Mimic : Enemy
    {
        private Sprite nativeSprite;
        private bool isNative;

        private void Start()
        {
            SetStartDefaults();

            homeRadius = 25;
            targetRange = 25f;
            fireRange = 20f;
            runRange = 5f;

            reloadTime = 0.3f;
            pauseTime = 1f;
            followingTime = 6f;

            MoveSpeed = 6f;

            isNative = true;
            nativeSprite = sprite.sprite;
        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();

            UpdateFireDirection(
                IsNearToPlayer(targetRange)
                    ? GameData.player.transform.position
                    : nextTarget);

            if (IsNearToPlayer(fireRange))
                Fire();

            ChooseState();

            DoStateAction();
        }

        private void Fire()
        {
            if (Weapon == null)
                return;

            var difference = Time.time - reloadStart;
            if (difference < reloadTime)
                return;
            reloadStart = Time.time;

            Weapon.Fire(true);
        }

        protected override void Die()
        {
            DieDefault();
        }

        private void ClearTransformation()
        {
            sprite.sprite = nativeSprite;
            Destroy(Weapon.gameObject);
            latestAimAngle = 0;
            isNative = true;
        }

        private void GetTransformation()
        {
            sprite.sprite = GameData.player.character.sprite.sprite;
            Weapon = Instantiate(GameData.player.character.weapon, transform.position + new Vector3(0, 1), Quaternion.identity);
            Weapon.transform.parent = transform;
            isNative = false;
        }

        private void ChooseState()
        {
            if (Distance2D(transform.position, homePosition) > homeRadius)
                UpdateTarget(homePosition);

            if (IsNearToPlayer(targetRange))
            {
                if (IsNearToPlayer(runRange))
                {
                    if (state == State.RunToPlayer)
                        followingStartTime = int.MinValue;

                    if (!isNative)
                        ClearTransformation();

                    state = State.RunFromPlayer;
                }
                else
                {
                    if (isNative)
                        GetTransformation();

                    state = State.RunToPlayer;
                }
                
            }
            else
            {
                state = State.Roaming;
                if (!isNative)
                    ClearTransformation();
            }
        }
    }
}
