using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace Weapon
{
	public class Laser : Weapon
	{
		public LineRenderer lineRenderer;
		private float maxDistance = 100;
		public PlayerLogic playerLogic;
		private int damageAmount = 1;
		private RaycastHit2D hit;

		public override void Awake()
		{
			reloadingTime = 0.2f;
		}

		public void Update()
		{
			if (playerLogic.State != PlayerState.Idle)
				TurnLaserOff();
		}
		
		public override void Fire(bool isButtonPressed)
		{
			if (!isButtonPressed)
				return;
			CastRay();
			switch (state)
			{
				case WeaponState.Ready when isButtonPressed:
					Damage();
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

		private void CastRay()
		{
			lineRenderer.enabled = true;
			var position = firePoint.position;
			if (Physics2D.Raycast(position, firePoint.up))
			{
				hit = Physics2D.Raycast(position, transform.up);
				DrawRay(position, hit.point);
			}
			else
			{
				DrawRay(position, firePoint.transform.up * maxDistance);
			}
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

		private void Damage()
		{
			if (hit.collider == null)
				return;
			if (hit.collider.GetComponent<HealthObj>())
				hit.collider.GetComponent<HealthObj>().Health.Damage(damageAmount);
		}
	}
}

