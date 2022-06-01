using System;
using System.Runtime.CompilerServices;
using Player;
using UnityEngine;

namespace Weapon
{
	public class SwordLauncher : Sword
	{
		public void Start()
		{
			Physics2D.IgnoreLayerCollision(8, 8);
		}
		public override void Fire(bool isButtonPressed)
		{
			if (hasUnlimitedBullets)
				ammoState = AmmoState.Unlimited;
			switch (state)
			{
				case WeaponState.Ready when isButtonPressed:
					Attack();
					CreateBullets();
					ammoState = AmmoState.Normal;
					reloadStart = Time.time;
					state = WeaponState.Reloading;
					if (CurrentAmmoAmount <= 0)
						CurrentAmmoAmount = maxAmmoAmount;
					break;

				case WeaponState.Reloading:
					timeDifference = Time.time - reloadStart;
					if (timeDifference >= reloadingTime)
						state = WeaponState.Ready;
					break;
			}
		}
	}
}