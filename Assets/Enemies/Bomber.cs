using Game;
using GridTools;
using Health;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Enemies
{
    public class Bomber : Enemy
    {
        public ParticleSystem Boom;
        public BomberMini BomberMiniPrefab;

        private float BoomRadius = 4f;

        private void Start()
        {
            SetStartDefaults();

            homeRadius = 20;
            targetRange = 20f;
            fireRange = 5f;

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
                ChooseState();

            DoStateAction();
        }

        private void Fire()
        {
            sprite.color = Color.red;
            state = State.PrepareToDie;
            deathStart = Time.time;
            Rb.velocity = Vector2.zero;
            Rb.AddForce(Direction2D(GameData.player.GetPosition(), transform.position).normalized * 20, ForceMode2D.Impulse);
        }

        protected override void Die()
        {
            var objectsToGetDamage = Physics2D.OverlapCircleAll(transform.position, BoomRadius);

            Instantiate(Boom, transform.position, Quaternion.identity);

            foreach (var obj in objectsToGetDamage)
            {
                if (!obj.GetComponentInChildren<HealthObj>() || obj.GetComponent<BomberMini>())
                    continue;

                var healthObj = obj.GetComponentInChildren<HealthObj>();
                if (healthObj != null && obj != Collider)
                    healthObj.Damage(damage);
            }

            for (var i = 0; i < Random.Range(3, 5); i++)
            {
                var miniBomber = Instantiate(BomberMiniPrefab, transform.position + Tools.GetRandomDir() * Random.Range(2, 4), transform.rotation);
                miniBomber.Grid = Grid;
            }

            DieDefault();
        }

        private void ChooseState()
        {
            if (Distance2D(transform.position, homePosition) > homeRadius)
                UpdateTarget(homePosition);

            state = IsNearToPlayer(targetRange)
                ? State.RunToPlayer
                : State.Roaming;
        }
    }
}
