using Extensions;
using GridTools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Enemies
{
    public class Spawner : Enemy
    {
        public Fly FlyBullet;
        private Fly[] flies;
        private const int MaxCountFly = 5;

        private void Start()
        {
            SetStartDefaults();

            flies = new Fly[MaxCountFly + Random.Range(-1, 1)];
            
            homeRadius = 25;
            targetRange = 10f;

            pauseTime = 1f;
            followingTime = 6f;
            reloadTime = 1f;

            MoveSpeed = 3f;
        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();

            UpdateFlies();
            Fire();

            ChooseState();

            DoStateAction();
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
                    flies[i].gameObject.layer = LayerMask.NameToLayer("Enemy");
                    reloadStart = Time.time;
                    break;
                }

            }
        }

        protected override void Die()
        {
            for (var i = 0; i < flies.Length; i++)
            {
                Vector3 position;
                while (true)
                {
                    position = transform.position + Tools.GetRandomDir();
                    var ray = Physics2D.Raycast(
                        transform.position,
                        Direction2D(position, transform.position).normalized,
                        Distance2D(position, transform.position) * 2f,
                        Grid.WallsLayerMask);
                    if (ray.collider == null)
                        break;
                }

                var fly = Instantiate(FlyBullet, position, transform.rotation);
                fly.Grid = Grid;
                fly.homePosition = transform.position;
                fly.gameObject.layer = LayerMask.NameToLayer("Enemy");
            }

            DieDefault();
        }

        private void ChooseState()
        {
            if (Distance2D(transform.position, homePosition) > homeRadius)
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
