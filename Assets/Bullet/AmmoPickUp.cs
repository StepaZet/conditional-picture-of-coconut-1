using System;
using Player;
using UnityEngine;
using Weapon;

namespace Bullet
{
	public class AmmoPickUp<T>
	{
		public void PickUp(GameObject gameObject, Collider2D other, int bulletsAmount)
		{
			if (!other.GetComponent<PlayerLogic>())
				return;
        
			var weapon = PlayerController.Instance.weapons.Find(x => x is T);
			if (weapon is null || weapon.ammoState == AmmoState.Full)
				return;
			
			weapon.AddBullets(bulletsAmount);
			
			UnityEngine.Object.Destroy(gameObject);
		}
	}
}