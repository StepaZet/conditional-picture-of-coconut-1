using Health;
using UnityEngine;

namespace Weapon
{
    public class SwordColliderHandler : MonoBehaviour
    {
        public void Start()
        {
            Physics2D.IgnoreLayerCollision(8, 8);
        }
        private HealthObj target;
        [SerializeField] private int damageAmount;
        [SerializeField] private Sword sword;
        private void OnTriggerEnter2D(Collider2D other)
        {
            target = other.GetComponentInChildren<HealthObj>();
            if (target != null && (sword.SwordState == Sword.State.AttackLeft || sword.SwordState == Sword.State.AttackRight))
                target.Damage(damageAmount);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            target = null;
        }
    }
}
