using System;
using UnityEngine;

namespace Bullet
{
    public class Bullet : MonoBehaviour
    {
        private int damageAmount = 2;
        private void Update()
        {
            Destroy(gameObject, 5);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponent<Bullet>())
            {
                Physics2D.IgnoreCollision(other, GetComponent<Collider2D>());
                return;
            }

            if (other.GetComponent<HealthObj>())
                other.GetComponent<HealthObj>().Health.Damage(damageAmount);

            Destroy(gameObject);
        }
    }
}
