using System;
using Game;
using Player;
using UnityEngine;
using Weapon;

namespace Bullet
{
	public class AmmoPickUp<T>
	{
		public void PickUp(GameObject gameObject, Collider2D other, int bulletsAmount, AudioClip pickUpSound)
		{
			if (!other.GetComponent<Character>())
				return;
			var character = GameData.player.character;
			if (!(character.weapon is T))
				return;

			var weapon = character.weapon;
			if (weapon.ammoState == AmmoState.Full)
				return;
			
			AudioSource.PlayClipAtPoint(pickUpSound, gameObject.transform.position);
			weapon.AddBullets(bulletsAmount);
			
			UnityEngine.Object.Destroy(gameObject);
		}
	}
	
	public class AmmoPickUp<T1, T2>
	{
		public void PickUp(GameObject gameObject, Collider2D other, int bulletsAmount)
		{
			if (!other.GetComponent<Character>())
				return;
			var character = GameData.player.character;
			if (!(character.weapon is T1 || character.weapon is T2 ))
				return;

			var weapon = character.weapon;
			if (weapon.ammoState == AmmoState.Full)
				return;
			
			weapon.AddBullets(bulletsAmount);
			
			UnityEngine.Object.Destroy(gameObject);
		}
	}
}