using System;
using Player;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerLogic))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private LayerMask dashLayerMask;
	public float moveSpeed = 8f;
	public float rollSpeed;
	public Rigidbody2D rb;
	public Weapon weapon;

	private PlayerLogic playerLogic;
	private PlayerInput playerInput;
	private Vector2 cursorPosition;
	private Vector2 moveDirection;
	private Vector2 latestMoveDirection;
	private float latestAimAngle;

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		playerLogic = GetComponent<PlayerLogic>();
		playerLogic.State = PlayerState.Idle;
	}
	private void Update()
	{
		moveDirection = playerInput.MovementInput.normalized;
		cursorPosition = Camera.main.ScreenToWorldPoint(playerInput.AimingInput);
		if (moveDirection != Vector2.zero) 
			latestMoveDirection = moveDirection.normalized;
		switch (playerLogic.State)
		{
			case PlayerState.Idle:
				Aim();
				CheckForFire();
				Dash();
				Roll();
				break;
			case PlayerState.Rolling:
				Aim();
				break;
			case PlayerState.Dead:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void FixedUpdate()
	{
		switch (playerLogic.State)
		{
			case PlayerState.Idle:
				Move();
				break;
			case PlayerState.Rolling:
				SetVelocityForRoll();
				break;
			case PlayerState.Dead:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Move()
	{
		rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
	}

	private void Dash()
	{
		if (!playerInput.IsDash)
			return;
		
		var dashAmount = 3f;
		var position = transform.position;
		var dashPosition = position + (Vector3) latestMoveDirection.normalized * dashAmount;
		var raycastHit = Physics2D.Raycast(position, latestMoveDirection.normalized, dashAmount, dashLayerMask);
		if (raycastHit.collider != null)
			dashPosition = raycastHit.point;
		rb.MovePosition(dashPosition);
	}

	private void SetVelocityForRoll()
	{
		var rollSpeedDropMultiplier = 2f;
		rollSpeed -= rollSpeed * rollSpeedDropMultiplier * Time.deltaTime;
		
		var minimumRollSpeed = 4f;
		if (rollSpeed < minimumRollSpeed) 
			playerLogic.State = PlayerState.Idle;

		rb.velocity = latestMoveDirection * rollSpeed;
	}

	private void Roll()
	{
		if (!playerInput.IsRoll)
			return;
		playerLogic.State = PlayerState.Rolling;
		

		rollSpeed = 20f;

	}
	
	private void Aim()
	{
		var aimDirection = cursorPosition - rb.position;
		var aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
		weapon.weaponPrefab.transform.RotateAround(rb.position, Vector3.forward, aimAngle - latestAimAngle);
		latestAimAngle = aimAngle;
	}

	private void CheckForFire()
	{
		if (playerInput.IsFireInput)
			weapon.Fire();
	}
}