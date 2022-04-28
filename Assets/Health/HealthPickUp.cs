using Player;
using UnityEngine;

namespace Health
{
	public class HealthPickUp : MonoBehaviour
	{
		[SerializeField] private int healthPoints = 5;

		public void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.GetComponent<PlayerLogic>())
				return;

			if (!other.GetComponent<HealthObj>() ||
			    PlayerController.Instance.playerLogic.health.Health.CurrentHealthPoints ==
			    PlayerController.Instance.playerLogic.health.Health.MaxHealthPoints)
				return;

			PlayerController.Instance.playerLogic.health.Health.Heal(healthPoints);

			Destroy(gameObject);
		}
	}
}