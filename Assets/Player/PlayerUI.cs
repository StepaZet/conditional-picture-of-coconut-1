using System;
using Health;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
	public class PlayerUI : MonoBehaviour
	{
		[SerializeField]private Text ammoText;
		[SerializeField]private PlayerObj player;
		[SerializeField]private HealthBar healthBar;

		public void UpdateAmmoText(int current, int max)
		{
			ammoText.text = $"{current}/{max}";
		}

		public void ChangeHealthBar()
		{
			healthBar.ChangeHealthObj(player.character.health);
		}

		public void Awake()
		{
			healthBar.SetUp(player.character.health);
		}
	}
}