using UnityEngine;

namespace Player
{
	public class PlayerInput
	{
		public Vector2 MovementInput { get; private set; }
		public Vector2 AimingInput { get; private set; }
	
		public float ChangeWeaponInput { get; private set; }
		public bool IsFireInput { get; private set; }
		public bool IsFireInputHeld { get; private set; }
		public bool IsFireInputReleased { get; private set; }
		public bool IsDash { get; private set; }
		public bool IsRoll { get; private set; }
		public bool IsChangeCharacter { get; private set; }
		public bool IsChooseCharacter { get; private set; }
		private int chosenCharacterIndex;
		public int MouseScrollDelta { get; private set; }
		private int previousScrollDelta;
		
		private readonly KeyCode[] numberKeyCodes = 
		{
			KeyCode.Alpha1,
			KeyCode.Alpha2,
			KeyCode.Alpha3,
			KeyCode.Alpha4,
			KeyCode.Alpha5,
			KeyCode.Alpha6,
			KeyCode.Alpha7,
			KeyCode.Alpha8,
			KeyCode.Alpha9,
		};


		public void Update()
		{
			MovementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			ChangeWeaponInput = Input.GetAxis("Mouse ScrollWheel");
			AimingInput = Input.mousePosition;
			IsFireInput = Input.GetMouseButtonDown(0);
			IsFireInputHeld = Input.GetMouseButton(0);
			IsFireInputReleased = Input.GetMouseButtonUp(0);
			IsDash = Input.GetKeyDown(KeyCode.Space);
			IsRoll = Input.GetKey(KeyCode.LeftShift);
			if (!IsChangeCharacter)
				IsChangeCharacter = Input.GetKeyDown(KeyCode.Tab);
				
			
			for (var i = 0; i < numberKeyCodes.Length; i++)
				if (Input.GetKeyDown(numberKeyCodes[i]))
				{
					chosenCharacterIndex = i;
					IsChooseCharacter = true;
				}

			MouseScrollDelta = (int) Input.mouseScrollDelta.y;

			if (MouseScrollDelta != previousScrollDelta)
			{
				chosenCharacterIndex += MouseScrollDelta;
				IsChooseCharacter = true;
			}
		}

		public void DropIsChangeCharacter()
        {
            IsChangeCharacter = false;
            IsChooseCharacter = false;
        }

		public int GetCharacterIndex(int maxValue)
		{
			if (chosenCharacterIndex < 0)
				chosenCharacterIndex = maxValue;
			else if (chosenCharacterIndex > maxValue) 
				chosenCharacterIndex = 0;

			return chosenCharacterIndex;
		}

	}
}