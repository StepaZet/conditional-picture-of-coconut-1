using Health;
using Player;
using UnityEngine;

namespace Assets.Bullet
{
    public class BulletGold : MonoBehaviour
    {
        private int damageAmount = 5;
        private float boomRadius = 2;
        public ParticleSystem BoomPrefab;

        private void Boom()
        {
            Instantiate(BoomPrefab, transform.position, Quaternion.identity);
            var objectsToGetDamage = Physics2D.OverlapCircleAll(transform.position, boomRadius);

            foreach (var obj in objectsToGetDamage)
            {
                if (!obj.gameObject.GetComponent<HealthObj>())
                    continue;

                var healthObj = obj.GetComponentInChildren<HealthObj>();
                if (healthObj != null)
                    healthObj.Damage(damageAmount);
            }

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Character>())
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
