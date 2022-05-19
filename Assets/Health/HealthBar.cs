using UnityEngine;

namespace Health
{
	public class HealthBar : MonoBehaviour
	{
		public HealthObj Health { get; private set; }
		public void SetUp(HealthObj health)
		{
			Health = health;
			Health.OnHealthChanged += HealthSystemOnHealthChanged;
		}

		private void HealthSystemOnHealthChanged(object sender, System.EventArgs eventArgs)
		{
			UpdateBar();
		}

		private void UpdateBar()
		{
			transform.Find("Bar").localScale = new Vector3(Health.GetHealthPercentage(), 1);
		}

		public void ChangeHealthObj(HealthObj health)
		{
			Health = health;
			UpdateBar();
		}
	}
}
