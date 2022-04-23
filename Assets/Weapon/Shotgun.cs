using UnityEngine;

namespace Weapon
{
	public class Shotgun : Weapon
	{
		public override void Awake()
		{
			reloadingTime = 1;
		}

		protected override void CreateBullets()
		{
			var spreadAngle = 4;
			var bulletsCount = 5;
			for (var i = 0; i < bulletsCount; i++)
			{
				var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
				var spread = firePoint.up * fireForce;
				spread.x += Random.Range(-spreadAngle, spreadAngle);
				spread.y += Random.Range(-spreadAngle, spreadAngle);
				bullet.GetComponent<Rigidbody2D>().AddForce(spread, ForceMode2D.Impulse);
			}
		}
	}
}