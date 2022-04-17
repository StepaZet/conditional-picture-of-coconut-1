using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weapon
{
	public class Laser : Weapon
	{
		public LineRenderer lineRenderer;
		private float maxDistance = 100;
		protected override void CreateBullets()
		{
			lineRenderer.enabled = true;
			if (Physics2D.Raycast(firePoint.position, firePoint.up))
			{
				var hit = Physics2D.Raycast(firePoint.position, transform.up);
				DrawRay(firePoint.position, hit.point);
			}
			else
			{
				DrawRay(firePoint.position, firePoint.transform.up * maxDistance);
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
				lineRenderer.enabled = false;
		}
	}
}

