using System;
using Game;
using UnityEngine;

namespace Health
{
	public class HealthBar : MonoBehaviour
	{
		public HealthObj Health { get; private set; }
		[SerializeField] private GameObject bar;
		public void SetUp(HealthObj health)
		{
			Health = health;
			Health.OnHealthChanged += HealthSystemOnHealthChanged;
		}

		public void HealthSystemOnHealthChanged(object sender, System.EventArgs eventArgs)
		{
			UpdateBar();
		}

		public void UpdateBar()
		{
			if (Health == null)
				Health = GameData.player.character.health;		//Костыль, ибо nullExcepion неизвестно откуда
			bar.transform.localScale = new Vector3(Health.GetHealthPercentage(), 1);
		}

		private void Update()
		{
			//transform.Find("Bar").localScale = new Vector3(Health.GetHealthPercentage(), 1);
		}

		public void ChangeHealthObj(HealthObj health)
		{
			Health = health;
			UpdateBar();
		}
	}
}
