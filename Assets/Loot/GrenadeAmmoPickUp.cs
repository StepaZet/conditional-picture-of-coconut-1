using Player;
using UnityEngine;
using Weapon;

namespace Bullet
{
	public class GrenadeAmmoPickUp : MonoBehaviour
	{
		private int bulletsAmount = 10;
		public ParticleSystem SpawnAnimation;

		private void Start()
		{
			Instantiate(SpawnAnimation, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
		}
		public void OnTriggerEnter2D(Collider2D other)
		{
			var ammoPickup = new AmmoPickUp<GrenadeLauncher>();
			ammoPickup.PickUp(gameObject, other, bulletsAmount);
		}
	}
}