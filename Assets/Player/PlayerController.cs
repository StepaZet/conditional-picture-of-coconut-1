using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
	public float moveSpeed = 5f;
	public Rigidbody2D rb;
	public Weapon weapon;

	private PlayerInput playerInput;
	private Vector2 cursorPosition;
	private Vector2 moveDirection;

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
	}
	private void Update()
	{
		moveDirection = playerInput.MovementInput.normalized;
		cursorPosition = Camera.main.ScreenToWorldPoint(playerInput.AimingInput);
		CheckForFire();
	}

	private void FixedUpdate()
	{
		Move();
		Aim();
	}

	private void Move()
	{
		rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
	}
	
	private void Aim()
	{
		var aimDirection = cursorPosition - rb.position;
		var aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
		rb.rotation = aimAngle;
	}

	private void CheckForFire()
	{
		if (playerInput.IsFireInput)
			weapon.Fire();
	}
}