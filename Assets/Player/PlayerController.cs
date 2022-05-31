using System;
using Game;
using MainGameScripts;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Player
{
	public class PlayerController
    {
        private readonly LayerMask dashLayerMask;
        private float moveSpeed = 10f;
		private float maxRunSpeed = 13f;
		private float minRunSpeed = 10f;
		private float runSpeed = 10f;

		private Vector2 cursorPosition;
		private Vector2 moveDirection;
		private Vector2 latestMoveDirection;
		public float latestAimAngle;

		public PlayerController(LayerMask dashLayerMask)
		{
			this.dashLayerMask = dashLayerMask;
        }
    
		public void Update(PlayerObj player)
		{ 
			moveDirection = player.input.MovementInput.normalized;
			cursorPosition = Camera.main.ScreenToWorldPoint(player.input.AimingInput);
			if (moveDirection != Vector2.zero)
				latestMoveDirection = moveDirection.normalized;
			switch (player.character.State)
			{
				case PlayerState.Normal:
					Aim(player);
					if (GameData.IsPaused)
						return;
					Fire(player);
					FireHeld(player);
					FireReleased(player);
					Dash(player);
					Run(player);
					break;
				case PlayerState.Running:
					Run(player);
					Aim(player);
					Fire(player);
					FireHeld(player);
					FireReleased(player);
					break;
				case PlayerState.Dead:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void FixedUpdate(PlayerObj player)
        {
	        switch (player.character.State)
			{
				case PlayerState.Normal:
					Move(player, moveSpeed);
					break;
				case PlayerState.Running:
					runSpeed += 0.01f;
					player.character.stamina.Damage(1);
					if (runSpeed > maxRunSpeed)
						runSpeed = maxRunSpeed;
					Move(player, runSpeed);
					if (player.character.stamina.CurrentHealthPoints <= 0)
						player.character.State = PlayerState.Normal;
					break;
				case PlayerState.Dead:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Move(PlayerObj player, float multiplier)
		{
			player.character.rb.velocity = new Vector2(moveDirection.x * multiplier, moveDirection.y * multiplier);
		}

		private void Run(PlayerObj player)
		{
			if (player.character.State == PlayerState.Normal)
				moveSpeed = minRunSpeed;
			player.character.State = player.input.IsRoll && player.character.stamina.CurrentHealthPoints > 0
				? PlayerState.Running
				: PlayerState.Normal;
		}

		private void Dash(PlayerObj player)
		{
			if (!player.input.IsDash)
				return;
			if (player.character.stamina.CurrentHealthPoints <= 0)
				return;
			
			player.character.stamina.Damage(30);

			var dashAmount = 3f;
			var position = player.transform.position;
			var dashPosition = position + (Vector3) latestMoveDirection.normalized * dashAmount;
			var raycastHit = Physics2D.Raycast(position, latestMoveDirection.normalized, dashAmount, dashLayerMask);
			if (raycastHit.collider != null)
				dashPosition = raycastHit.point;
			player.character.rb.MovePosition(dashPosition);
		}

		private void Aim(PlayerObj player)
		{
			var aimDirection = cursorPosition - player.character.rb.position;
			player.character.sprite.flipX = (int)Mathf.Sign(-aimDirection.x) == 1;
			//weapon.weaponPrefab.transform.eulerAngles = aimDirection;
			var aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
			player.character.weapon.weaponPrefab.transform.RotateAround(player.character.transform.position, Vector3.forward, aimAngle - latestAimAngle);
			latestAimAngle = aimAngle;
		}
		private static void Fire(PlayerObj player)
		{
			player.character.weapon.Fire(player.input.IsFireInput);
		}
	
		private static void FireHeld(PlayerObj player)
		{
			player.character.weapon.FireHeld(player.input.IsFireInputHeld);
		}

		private static void FireReleased(PlayerObj player)
		{
			player.character.weapon.FireReleased(player.input.IsFireInputReleased);
		}
    }
}