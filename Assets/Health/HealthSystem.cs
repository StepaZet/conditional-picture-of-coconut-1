using System;

namespace Health
{
	public class HealthSystem
	{
		public event EventHandler OnHealthChanged;

		public HealthSystem(int maxHealthPoints)
		{
			MaxHealthPoints = maxHealthPoints;
			CurrentHealthPoints = maxHealthPoints;
		}
		public HealthSystem(int maxHealthPoints, int currentHealthPoints)
		{
			MaxHealthPoints = maxHealthPoints;
			CurrentHealthPoints = currentHealthPoints;
		}

		public int CurrentHealthPoints { get; set; }
		public int MaxHealthPoints { get; set; }

		public float GetHealthPercentage() => (float)CurrentHealthPoints / MaxHealthPoints;

		public void Damage(int points)
		{
			CurrentHealthPoints = ToHealthInBounds(CurrentHealthPoints - Math.Abs(points));
			OnHealthChanged?.Invoke(this, EventArgs.Empty);
		}

		public void Heal(int points)
		{
			CurrentHealthPoints = ToHealthInBounds(CurrentHealthPoints + Math.Abs(points));
			OnHealthChanged?.Invoke(this, EventArgs.Empty);
		}

		private int ToHealthInBounds(int healthPoints)
		{
			if (healthPoints < 0)
				return 0;
			if (healthPoints > MaxHealthPoints)
				return MaxHealthPoints;
			return healthPoints;
		}
	}
}