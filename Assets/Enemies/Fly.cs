using Extensions;
using Game;

namespace Assets.Enemies
{
    public class Fly : Enemy
    { 
        private void Start()
        {
            SetStartDefaults();

            homeRadius = 10;
            targetRange = 20f;
            fireRange = 10f;

            pauseTime = 0.4f;
            followingTime = 0.5f;

            MoveSpeed = 5f;
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
            Weapon.Fire(true);
        }

        private void Die()
        {
            Destroy(gameObject);
        }

        private void ChooseState()
        {
            if (homePosition.DistanceTo(transform.position) > homeRadius)
                UpdateTarget(homePosition);

            state = IsNearToPlayer(targetRange)
                ? State.RunToPlayer
                : State.Roaming;
        }
    }
}
