using System;
using UnityEngine;

namespace Bullet
{
    public class Bullet : MonoBehaviour
    {
        private int damageAmount = 1;
        private void Update()
        {
            Destroy(gameObject, 5);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponent<Bullet>())
            {
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
                return;
            }

            if (collision.collider.GetComponent<HealthObj>())
                collision.collider.GetComponent<HealthObj>().Health.Damage(damageAmount);

            Destroy(gameObject);
        }
    }
}
