using System;
using UnityEngine;
using Weapon;

namespace Player
{
    public class Character : MonoBehaviour
    {
        public int currentHealth = 0;
        public GameObject healthObjPrefab;
        public Weapon.Weapon weapon;
        public HealthObj health;
        public Rigidbody2D rb;
        private GridData gridData;
        [SerializeField] protected GameObject weaponPrefab;
        [SerializeField] public Collider2D characterCollider;
        [SerializeField] private GridObj grid;
        public PlayerState State { get; set; }
        
        private void Awake()
        {
            gridData = new GridData(this, grid);
            health = Instantiate(healthObjPrefab, transform).GetComponent<HealthObj>();
            State = PlayerState.Normal;
        }
        private void Update()
        {
            if (health.Health.CurrentHealthPoints <= 0) 
                Die();
            currentHealth = health.Health.CurrentHealthPoints;
        }

        private void FixedUpdate()
        {
            gridData.Update(this);
            
        }

        private void Die()
        {
            //Destroy(gameObject);
        }
    }
}
