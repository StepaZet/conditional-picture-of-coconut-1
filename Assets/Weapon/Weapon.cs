using System;
using System.ComponentModel.Design;
using Bullet;
using UnityEngine;
using Random = System.Random;

namespace Weapon
{
	public class Weapon : MonoBehaviour
    {

        public AudioSource[] shootSounds;
        protected int shootSoundNumber;

		[SerializeField]protected GameObject bulletPrefab;
		public GameObject weaponPrefab;
		[SerializeField]protected Transform firePoint;
		[SerializeField]protected float fireForce = 20f;
		[SerializeField]protected WeaponState state;
		public AmmoState ammoState;
		[SerializeField]protected float reloadingTime;
		[SerializeField]protected float reloadStart;
		[SerializeField]protected float timeDifference;
		[SerializeField] protected int maxAmmoAmount;
		[SerializeField] protected bool hasUnlimitedBullets;
		public int CurrentAmmoAmount { get; private set; }
		public static event EventHandler OnAmmoChanged;
		public int MaxAmmoAmount => maxAmmoAmount;
        protected int WallsLayerMask;

        public void Awake()
		{
			ammoState = AmmoState.Full;
			CurrentAmmoAmount = maxAmmoAmount;
			WallsLayerMask = LayerMask.GetMask("Walls");
			OnAmmoChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void SetMaxBulletAmount()
		{
			maxAmmoAmount = int.MaxValue;
		}

		protected virtual void SetReloadingTime()
		{
            reloadingTime = 0.5f;
        }

        protected void MakeShootSound()
        {
            if (shootSounds.Length == 0)
				return;
            shootSounds[shootSoundNumber].Play();
            shootSoundNumber = (shootSoundNumber + 1) % shootSounds.Length;
        }

        public virtual void Fire(bool isButtonPressed)
        {
            if (hasUnlimitedBullets)
		        ammoState = AmmoState.Unlimited;
			switch (state)
			{
				case WeaponState.Ready when isButtonPressed:
					switch (ammoState)
					{
						case AmmoState.Empty:
							break;
						case AmmoState.Full:
						case AmmoState.Normal:
                            MakeShootSound();
							CreateBullets();
							ammoState = AmmoState.Normal;
							reloadStart = Time.time;
							state = WeaponState.Reloading;
							if (CurrentAmmoAmount <= 0)
								ammoState = AmmoState.Empty;
							break;
						case AmmoState.Unlimited:
                            MakeShootSound();
							CreateBullets();
							reloadStart = Time.time;
							state = WeaponState.Reloading;
                            if (CurrentAmmoAmount <= 0)
                                CurrentAmmoAmount = maxAmmoAmount;
                            break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case WeaponState.Reloading:
					timeDifference = Time.time - reloadStart;
					if (timeDifference >= reloadingTime)
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
			if (ammoState == AmmoState.Empty)
				return;
			
			var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
			bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
			DecrementAmmo();
		}

		public void AddBullets(int amount)
		{
			var temporary = CurrentAmmoAmount;
			temporary += amount;
			if (temporary > maxAmmoAmount)
			{
				CurrentAmmoAmount = maxAmmoAmount;
				ammoState = AmmoState.Full;
			}
			else
			{
				CurrentAmmoAmount = temporary;
				ammoState = AmmoState.Normal;
			}
			
			OnAmmoChanged?.Invoke(this, EventArgs.Empty);
		}


		protected void DecrementAmmo()
		{
			CurrentAmmoAmount--;
			OnAmmoChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}