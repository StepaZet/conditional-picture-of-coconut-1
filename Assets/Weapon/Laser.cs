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

		public void Update()
		{
			if (playerLogic.State != PlayerState.Idle)
				TurnLaserOff();
		}

		protected override void CreateBullets()
		{
			lineRenderer.enabled = true;
			var position = firePoint.position;
			if (Physics2D.Raycast(position, firePoint.up))
			{
				
				var hit = Physics2D.Raycast(position, transform.up);
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
	}
}

