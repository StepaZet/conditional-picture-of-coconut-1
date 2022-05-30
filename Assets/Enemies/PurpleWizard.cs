using Extensions;
using Game;
using UnityEngine;

namespace Assets.Enemies
{
    public class PurpleWizard : Enemy
    {
        private void OnEnable()
        {
            SetStartDefaults();

            homeRadius = 25;
            targetRange = 25f;
            fireRange = 10f;
            runRange = 10f;

            pauseTime = 1f;
            followingTime = 6f;

            MoveSpeed = 3f;
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
            var difference = Time.time - reloadStart;
            if (difference < reloadTime)
                return;
            Weapon.Fire(true);
            reloadStart = Time.time;
        }

        protected override void Die()
        {
            DieDefault();
        }

        private void ChooseState()
        {
            if (Distance2D(transform.position, homePosition) > homeRadius)
                UpdateTarget(homePosition);

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
            }

            else
                state = State.Roaming;
        }
    }
}
