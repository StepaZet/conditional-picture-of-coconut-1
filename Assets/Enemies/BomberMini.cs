using Extensions;
using UnityEngine;

namespace Assets.Enemies
{
    public class BomberMini : Enemy
    {
        public ParticleSystem boom;

        private const float BoomRadius = 2f;

        private void Start()
        {
            SetStartDefaults();

            homeRadius = 20;
            targetRange = 20f;
            fireRange = 2f;

            damage = 5;

            pauseTime = 1f;
            followingTime = 0.5f;
            deathTime = 0.7f;

            MoveSpeed = 3f;
        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();

            if (IsNearToPlayer(fireRange) && state != State.PrepareToDie)
                Fire();

            if (state != State.PrepareToDie)
                ChooseBehaviour();
            
            DoStateAction();
        }

        private void Fire()
        {
            sprite.color = Color.red;
            state = State.PrepareToDie;
            deathStart = Time.time;
        }

        protected void Die()
        {
            var objectsToGetDamage = Physics2D.OverlapCircleAll(transform.position, BoomRadius);

            Instantiate(boom, transform.position, Quaternion.identity);

            foreach (var obj in objectsToGetDamage)
            {
                if (!obj.GetComponentInChildren<HealthObj>() || obj.GetComponent<Bomber>())
                    continue;

                var healthObj = obj.GetComponentInChildren<HealthObj>();
                if (healthObj != null && obj != Collider)
                    healthObj.Damage(damage);
            }

            DieDefault();
        }

        private void ChooseBehaviour()
        {
            if (Distance2D(transform.position, homePosition) > homeRadius)
                UpdateTarget(homePosition);

            state = IsNearToPlayer(targetRange)
                ? State.RunToPlayer
                : State.Roaming;
        }
    }
}
