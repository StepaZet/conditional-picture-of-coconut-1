using GridTools;
using UnityEngine;
using Unity.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Player;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(PlayerLogic))]
//[RequireComponent(typeof(GridObj))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private LayerMask dashLayerMask;
	public float moveSpeed = 5f;
	public float rollSpeed;
	public Rigidbody2D rb;
    [SerializeField] public GridObj grid;
	public static PlayerController Instance { get; private set; }
    [SerializeField] private List<Weapon.Weapon> weapons;
    [SerializeField] private int selectedWeaponId;

	private PlayerLogic playerLogic;
	private PlayerInput playerInput;
	private Vector2 cursorPosition;
	private Vector2 moveDirection;
    [SerializeField] private int2 gridPosition;
	private Vector2 latestMoveDirection;
	private float latestAimAngle;
	
    private void Start()
    {
	    Instance = this;
        //grid = GetComponent<GridObj>();	
        gridPosition = grid.WorldToGridPosition(transform.position);
    }

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		playerLogic = GetComponent<PlayerLogic>();
		playerLogic.State = PlayerState.Idle;
		foreach (var weapon in weapons.Skip(1))
		{
			weapon.enabled = false;
			weapon.GetComponent<Renderer>().enabled = false;
		}
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
				Fire();
				FireHeld();
				FireReleased();
				Dash();
				Roll();
				ChangeWeapon();
				break;
			case PlayerState.Rolling:
				Aim();
				ChangeWeapon();
				break;
			case PlayerState.Dead:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

    private void FixedUpdate()
	{
		UpdateGridPosition();
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
		//weapons[selectedWeaponId].weaponPrefab.transform.eulerAngles = aimDirection;
        var aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        weapons[selectedWeaponId].weaponPrefab.transform.RotateAround(rb.position, Vector3.forward, aimAngle - latestAimAngle);
        latestAimAngle = aimAngle;
	}

    private void UpdateGridPosition()
    {
        var newGridPosition = grid.WorldToGridPosition(transform.position);
        if (newGridPosition.x == gridPosition.x && newGridPosition.y == gridPosition.y) return;
        grid.UnFillCell(gridPosition);
        grid.FillCell(newGridPosition);
        gridPosition = newGridPosition;
		grid.PlayerPosition = transform.position;
    }

	private void Fire()
	{
		weapons[selectedWeaponId].Fire(playerInput.IsFireInput);
	}
	
	private void FireHeld()
	{
		weapons[selectedWeaponId].FireHeld(playerInput.IsFireInputHeld);
	}

	private void FireReleased()
	{
		weapons[selectedWeaponId].FireReleased(playerInput.IsFireInputReleased);
	}

	private void ChangeWeapon()
	{
		if (playerInput.ChangeWeaponInput == 0)
			return;
		weapons[selectedWeaponId].enabled = false;
		weapons[selectedWeaponId].GetComponent<Renderer>().enabled = false;
		var rotation = weapons[selectedWeaponId].transform.rotation;
		var position = weapons[selectedWeaponId].transform.position;
		
		selectedWeaponId = Math.Abs(((int) (playerInput.ChangeWeaponInput*10) + selectedWeaponId) % weapons.Count);
		weapons[selectedWeaponId].transform.SetPositionAndRotation(position, rotation);
		weapons[selectedWeaponId].enabled = true;
		weapons[selectedWeaponId].GetComponent<Renderer>().enabled = true;
	}
	public Vector3 GetPosition()
	{
		return transform.position;
	}
}