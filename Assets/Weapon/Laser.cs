using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Enemies;
using Player;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Weapon
{
	public class Laser : Weapon
	{
		public LineRenderer lineRenderer;
		private float maxDistance = 100;
		public Character character;
		private int damageAmount = 1;
		//private List<RaycastHit2D> hits = new List<RaycastHit2D>();
        

		protected override void SetMaxBulletAmount()
		{
			maxAmmoAmount = 10;
		}

		protected override void SetReloadingTime()
		{
			reloadingTime = 0.2f;
		}

		public void Update()
		{
			if (character.State != PlayerState.Normal)
				TurnLaserOff();
		}
		
		public override void Fire(bool isButtonPressed)
		{
			if (!isButtonPressed)
				return;
			
			switch (state)
			{
				case WeaponState.Ready:
					switch (ammoState)
					{
						case AmmoState.Empty:
							TurnLaserOff();
							break;
						case AmmoState.Full:
						case AmmoState.Normal:
							ammoState = AmmoState.Normal;
							CastRay();
                            currentAmmoAmount--;

							if (currentAmmoAmount <= 0)
								ammoState = AmmoState.Empty;
							
							reloadStart = Time.time;
							state = WeaponState.Reloading;
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
					break;
				case WeaponState.Reloading:
					CastRay();
					timeDifference = Time.time - reloadStart;
					if (Time.time - reloadStart >= reloadingTime)
						state = WeaponState.Ready;
					break;
			}
		}

		private void CastRay()
		{
			lineRenderer.enabled = true;
            
            var position = firePoint.position;
            var endLaser = firePoint.up * maxDistance;
            var collisions = Physics2D.RaycastAll(position, firePoint.up);
            foreach (var hit2D in collisions)
            {
                if (hit2D.transform.gameObject.GetComponent<TilemapCollider2D>())
                {
                    endLaser = hit2D.point;
					break;
                }

                if (state == WeaponState.Ready)
                    Damage(hit2D);
            }

            DrawRay(position, endLaser);
		}

		private void DrawRay(Vector2 startPosition, Vector2 endPosition)
		{
			lineRenderer.widthMultiplier = 0.1f;
			lineRenderer.SetPosition(0, startPosition);
			lineRenderer.SetPosition(1, endPosition);
		}

		public override void FireReleased(bool isButtonReleased)
		{
			if (isButtonReleased)
				TurnLaserOff();
		}
		
		private void TurnLaserOff()
		{
			lineRenderer.enabled = false;
		}

        private void Damage(RaycastHit2D hit)
        {
			if (hit.collider.GetComponentInChildren<HealthObj>())
                hit.collider.GetComponentInChildren<HealthObj>().Damage(damageAmount);
		}
    }
}

