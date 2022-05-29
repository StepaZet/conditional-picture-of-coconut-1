using Assets.Enemies;
using Health;
using UnityEngine;

namespace Assets.Bullet
{
    public class BulletYellowBoss : MonoBehaviour
    {
        public AudioClip DieSound;

        private int damageAmount = 5;
        //private ParticleSystem fly;
        //public ParticleSystem flyPrefab;
        public ParticleSystem BoomPrefab;

        private void Start()
        {
            MakeBoomSound();
        }

        //private void Update()
        //{
        //    fly.transform.position = transform.position;
        //}

        private void MakeBoomSound()
        {
            if (DieSound != null)
            {
                AudioSource.PlayClipAtPoint(DieSound, transform.position);
            }
        }

        private void Boom()
        {
            MakeBoomSound();
            Instantiate(BoomPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<BulletYellowBoss>() 
                || collision.gameObject.GetComponent<YellowBoss>() 
                || collision.gameObject.GetComponentInParent<YellowBoss>())
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
                return;
            }

            if (collision.collider.GetComponentInChildren<HealthObj>())
                collision.collider.GetComponentInChildren<HealthObj>().Damage(damageAmount);
            Boom();
            
        }
    }
}
