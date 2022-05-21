using System;
using UnityEngine;

namespace Health
{
	public class StaminaObj : HealthObj
	{
		[SerializeField]protected float reloadingTime;
		[SerializeField]protected float reloadStart;
		private enum State
		{
			Full,
			Refilling,
			Waiting
		}

		private State state;
		public void FixedUpdate()
		{
			switch (state)
			{
				case State.Full:
					break;
				case State.Refilling:
					Heal(1);
					if (maxHealthPoints == CurrentHealthPoints)
						state = State.Full;
					break;
				case State.Waiting:
					if (Time.time - reloadStart >= reloadingTime)
						state = State.Refilling;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override void Damage(int points)
		{
			base.Damage(points);
			reloadStart = Time.time;
			state = State.Waiting;
		}
	}
}