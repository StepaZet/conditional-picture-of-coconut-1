using System;
using Health;
using UnityEngine;

namespace Player
{
	
	public class PlayerLogic : MonoBehaviour
	{
		private HealthObj health;

		public PlayerState State { get; set; }

		private void OnEnable()
		{
			health = gameObject.AddComponent<HealthObj>();
            //health.Health.CurrentHealthPoints = int.MaxValue;
        }

		private void Update()
		{
            if (health.Health.CurrentHealthPoints <= 0)
            {
                Die();
			}
        }

		private void Die()
		{
            //Destroy(gameObject);
		}
	}
}