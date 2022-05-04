using Game;
using Player;
using UnityEngine;

namespace Health
{
	public class MedKit : MonoBehaviour
	{
		[SerializeField] private int healthPoints = 5;
		public Rigidbody2D rb;

		public void OnTriggerEnter2D(Collider2D other)
		{
			if (!other.GetComponent<PlayerObj>())
				return;

			if (!other.GetComponent<HealthObj>())
				return;
			var healthObj = other.GetComponent<HealthObj>(); 
			if (healthObj.Health.CurrentHealthPoints == healthObj.Health.MaxHealthPoints)
				return;

			GameData.player.character.health.Health.Heal(healthPoints);
				
			Destroy(gameObject);
		}
	}
}