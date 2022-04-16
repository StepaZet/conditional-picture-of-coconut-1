using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public Vector2 MovementInput { get; private set; }
	public Vector2 AimingInput { get; private set; }
	public bool IsFireInput { get; private set; }
	
	public bool IsDash { get; private set; }
	public bool IsRoll { get; private set; }
	

	private void Update()
	{
		MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		AimingInput = Input.mousePosition;
		IsFireInput = Input.GetMouseButtonDown(0);
		IsDash = Input.GetKeyDown(KeyCode.Space);
		IsRoll = Input.GetKeyDown(KeyCode.LeftShift);
	}

}