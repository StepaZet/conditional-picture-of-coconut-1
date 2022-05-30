using System;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Weapon
{
	public class Sword : Weapon
	{
		public enum State
		{
			Normal,
			AttackLeft,
			AttackRight
		}

		[SerializeField] protected Character character;

		protected float desiredRot = 60;

		protected readonly float rotationTime = 0.3f;
		protected readonly float rotSpeed = 1000;
		protected float timeWhenAttackStart;

		public State SwordState { get; protected set; }

		protected void OnDisable()
		{
			SwordState = State.Normal;
			desiredRot = 0;
			var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);
			transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, rotSpeed * Time.deltaTime);
		}

		protected void Update()
		{
			switch (SwordState)
			{
				case State.Normal:
					desiredRot = 0;
					break;
				case State.AttackRight:
					desiredRot += rotSpeed * Time.deltaTime;
					if (Math.Abs(Time.time - timeWhenAttackStart) > rotationTime)
					{
						SwordState = State.Normal;
						character.State = PlayerState.Normal;
					}

					break;
				case State.AttackLeft:
					desiredRot -= rotSpeed * Time.deltaTime;
					if (Math.Abs(Time.time - timeWhenAttackStart) > rotationTime)
					{
						SwordState = State.Normal;
						character.State = PlayerState.Normal;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);
			transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, rotSpeed * Time.deltaTime);
		}

		public override void Fire(bool isButtonPressed)
		{
			switch (state)
			{
				case WeaponState.Ready when isButtonPressed:
					if (!shootSounds[(shootSounds.Length + shootSoundNumber - 1) % shootSounds.Length].isPlaying)
                    {
                        MakeShootSound();
                    }
					Attack();
					ammoState = AmmoState.Normal;
					reloadStart = Time.time;
					state = WeaponState.Reloading;
					if (CurrentAmmoAmount <= 0)
						ammoState = AmmoState.Empty;
					break;

				case WeaponState.Reloading:
                    timeDifference = Time.time - reloadStart;
					if (timeDifference >= reloadingTime)
						state = WeaponState.Ready;
					break;
			}
		}

		protected void Attack()
		{
			timeWhenAttackStart = Time.time;
			SwordState = Math.Sign(character.transform.position.x - transform.position.x) == 1
				? State.AttackRight
				: State.AttackLeft;
		}
	}
}