using Extensions;
using Game;
using UnityEngine;

namespace Assets.Enemies
{
    public class CuteDemon : Enemy
    {
        public ParticleSystem Death;
        public AudioSource[] BiteSounds;
        private int BiteSoundNumber;


        private void OnEnable()
        {
            //Instantiate(SpawnAnimation, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
            SetStartDefaults();

            homeRadius = 20;
            targetRange = 30f;
            fireRange = 2f;

            damage = 6;

            reloadTime = 0.5f;
            pauseTime = 1f;
            followingTime = 0.2f;

            MoveSpeed = 10f;
        }

        private void FixedUpdate()
        {
            if (Health.CurrentHealthPoints <= 0)
                Die();

            if (IsNearToPlayer(fireRange))
                Fire();

            ChooseState();
            DoStateAction();
        }

        private void MakeBiteSound()
        {
            BiteSounds[BiteSoundNumber].Play();
            BiteSoundNumber = (BiteSoundNumber + 1) % BiteSounds.Length;
        }

        private void Fire()
        {
            var difference = Time.time - reloadStart;
            if (difference < reloadTime)
                return;

            MakeBiteSound();
            reloadStart = Time.time;
            GameData.player.character.health.Damage(damage);
        }

        protected override void Die()
        {
            //Instantiate(Death, transform.position, Quaternion.identity);
            DieDefault();
        }

        private void ChooseState()
        {
            if (Distance2D(transform.position, homePosition)> homeRadius)
                UpdateTarget(homePosition);

            if (IsNearToPlayer(targetRange))
            {
                state = State.RunToPlayer;
                homePosition = transform.position;
            }
            else
            {
                state = State.Roaming;
            }
        }
    }
}
