using UnityEngine;

namespace Weapon
{
	public class Shotgun : Weapon
	{
		protected override void SetMaxBulletAmount()
		{
			maxAmmoAmount = 30;
		}

		protected override void SetReloadingTime()
		{
			reloadingTime = 1f;
		}

		protected override void CreateBullets()
		{
			var spreadAngle = 4;
			var maxBulletsFired = 5;
			var bulletsFired = CurrentAmmoAmount >= maxBulletsFired ? maxBulletsFired : CurrentAmmoAmount;
			for (var i = 0; i < bulletsFired; i++)
			{
				var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
				DecrementAmmo();
				var spread = firePoint.up * fireForce;
				spread.x += Random.Range(-spreadAngle, spreadAngle);
				spread.y += Random.Range(-spreadAngle, spreadAngle);
				bullet.GetComponent<Rigidbody2D>().AddForce(spread, ForceMode2D.Impulse);
			}
		}
	}
}