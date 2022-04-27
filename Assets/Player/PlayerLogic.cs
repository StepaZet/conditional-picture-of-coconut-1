using System;
using Health;
using UnityEngine;

namespace Player
{
	
	public class PlayerLogic : MonoBehaviour
	{
		public HealthObj health;

		public PlayerState State { get; set; }

		private void OnEnable()
		{
			health = gameObject.AddComponent<HealthObj>();
			State = PlayerState.Idle;
		}

		private void Update()
		{
			if (health.Health.CurrentHealthPoints <= 0)
				Die();
		}

		private void Die()
		{
			Destroy(gameObject);
		}
	}
}