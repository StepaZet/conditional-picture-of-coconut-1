using System;
using UnityEngine;

namespace Weapon
{
	public class Weapon : MonoBehaviour
	{
		public GameObject bulletPrefab;
		public GameObject weaponPrefab;
		public Transform firePoint;
		public float fireForce = 20f;
		public WeaponState state;
		public float reloadingTime;
		public float reloadStart;
		public float d;

		public virtual void Awake()
		{
			reloadingTime = 0.5f;
		}
		public virtual void Fire(bool isButtonPressed)
		{
			switch (state)
			{
				case WeaponState.Ready when isButtonPressed:
					CreateBullets();
					reloadStart = Time.time;
					state = WeaponState.Reloading;
					break;
				case WeaponState.Reloading:
					d = Time.time - reloadStart;
					if (Time.time - reloadStart >= reloadingTime)
						state = WeaponState.Ready;
					break;
			}
		}
		
		public virtual void FireHeld(bool isButtonPushed)
		{
			Fire(isButtonPushed);
		}

		public virtual void FireReleased(bool isButtonReleased)
		{
			
		}

		protected virtual void CreateBullets()
		{
			var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
			bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
		}
	}
}